using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.UI.Contracts.Managers;
using HASS.Agent.UI.Helpers;
using HASS.Agent.UI.Models.Notifications;
using Microsoft.Extensions.Hosting;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using MQTTnet;
using Newtonsoft.Json;
using Serilog;

namespace HASS.Agent.UI.Managers;
public class NotificationManager : INotificationManager, IMqttMessageHandler
{
    private const string ActionKey = "action";
    private const string UriKey = "uri";
    private const string ClickActionKey = "clickAction";

    private const string ActionPrefix = $"{ActionKey}=";
    private const string UriPrefix = $"{UriKey}=";
    private const string ClickActionPrefix = $"{ClickActionKey}=";

    private const string SpecialClear = "clear_notification";
    private const string HomeAssistantNotificationEvent = "hass_agent_notifications";

    public const string NotificationLaunchArgument = "----AppNotificationActivated:";

    private readonly ISettingsManager _settingsManager;
    private readonly IMqttManager _mqttManager;
    private readonly IHomeAssistantApiManager _homeAssistantApiManager;

    private readonly AppNotificationManager _notificationManager = AppNotificationManager.Default;

    private readonly Dictionary<string, INotificationActionHandler> _notificationActionHandlers = [];

    public bool Ready { get; private set; }

    public NotificationManager(ISettingsManager settingsManager, IMqttManager mqttManager, IHomeAssistantApiManager homeAssistantApiManager)
    {
        _settingsManager = settingsManager;
        _mqttManager = mqttManager;
        _homeAssistantApiManager = homeAssistantApiManager;
    }

    public void Initialize()
    {
        try
        {
            if (!_settingsManager.Settings.Notification.Enabled)
            {
                Log.Information("[NOTIFICATIONS] Disabled");
                return;
            }

            if (!_settingsManager.Settings.Application.LocalApiEnabled && !_settingsManager.Settings.Mqtt.Enabled)
            {
                Log.Warning("[NOTIFICATIONS] Both local API and MQTT are disabled, unable to receive notifications");
                return;
            }

            if (_settingsManager.Settings.Mqtt.Enabled)
                _mqttManager.RegisterMessageHandler($"hass.agent/notifications/{_settingsManager.Settings.Application.DeviceName}", this);
            else
                Log.Warning("[NOTIFICATIONS] MQTT is disabled, not all aspects of actions might work as expected");

            if (_notificationManager.Setting != AppNotificationSetting.Enabled)
                Log.Warning("[NOTIFICATIONS] Showing notifications might fail, reason: {r}", _notificationManager.Setting.ToString());


            _notificationManager.NotificationInvoked += NotificationManager_NotificationInvoked;

            _notificationManager.Register();
            Ready = true;

            Log.Information("[NOTIFICATIONS] Ready");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[NOTIFICATIONS] Error while initializing: {err}", ex.Message);
        }
    }

    private async void NotificationManager_NotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args) => await HandleNotificationInvoked(args);


    private async Task HandleNotificationInvoked(AppNotificationActivatedEventArgs args)
    {
        try
        {
            var action = GetValueFromEventArgs(args, ActionPrefix);
            var input = GetInputFromEventArgs(args);
            var uri = GetValueFromEventArgs(args, UriPrefix);
            var clickAction = GetValueFromEventArgs(args, ClickActionPrefix);

            if (!string.IsNullOrWhiteSpace(uri))
                BrowserHelper.OpenUrl(uri);
            else if (!string.IsNullOrWhiteSpace(clickAction))
                BrowserHelper.OpenUrl(clickAction);

            await _homeAssistantApiManager.FireEventAsync(HomeAssistantNotificationEvent, new
            {
                device_name = _settingsManager.Settings.Application.DeviceName,
                action,
                input,
                uri
            });

        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[NOTIFICATIONS] Unable to process notification action: {err}", ex.Message);
        }
    }

    private string GetValueFromEventArgs(AppNotificationActivatedEventArgs args, string startText)
    {
        var start = args.Argument.IndexOf(startText) + startText.Length;
        if (start < startText.Length)
            return string.Empty;

        var separatorIndex = args.Argument.IndexOf(";", start);
        var end = separatorIndex < 0 ? args.Argument.Length : separatorIndex;
        return DecodeNotificationParameter(args.Argument[start..end]);
    }

    private static IDictionary<string, string> GetInputFromEventArgs(AppNotificationActivatedEventArgs args)
    {
        return args.UserInput.Count > 0 ? args.UserInput : new Dictionary<string, string>();
    }

    private static string EncodeNotificationParameter(string parameter)
    {
        var encodedParameter = Convert.ToBase64String(Encoding.UTF8.GetBytes(parameter));
        // for some reason, Windows App SDK URL encodes the arguments even if they are already encoded
        // this is the reason the WebUtility.UrlEncode is missing from here
        return encodedParameter;
    }

    private static string DecodeNotificationParameter(string encodedParameter)
    {
        var urlDecodedParameter = WebUtility.UrlDecode(encodedParameter);
        return Encoding.UTF8.GetString(Convert.FromBase64String(urlDecodedParameter));
    }

    public async Task ShowNotification(Notification notification)
    {
        if (!Ready)
            throw new Exception("NotificationManager is not initialized");

        try
        {
            if (!_settingsManager.Settings.Notification.Enabled)
                return;

            var toastBuilder = new AppNotificationBuilder()
                .AddText(notification.Title)
                .AddText(notification.Message);

            if (notification.Data.ClickAction != NotificationData.NoAction)
                toastBuilder.AddArgument(ClickActionKey, EncodeNotificationParameter(notification.Data.ClickAction));

            //TODO(Amadeo): add configuration for optional hero image
            //TODO(Amadeo): add option to disable caching of downloaded files
            //TODO(Amadeo): finish storage manager and use it to retrieve the image
            if (!string.IsNullOrEmpty(notification.Data.Image))
                toastBuilder.SetInlineImage(new Uri(notification.Data.Image));

            if (notification.Data.Actions.Count > 0)
            {
                foreach (var action in notification.Data.Actions)
                {
                    if (string.IsNullOrEmpty(action.Action))
                        continue;

                    var button = new AppNotificationButton(action.Title)
                        .AddArgument(ActionKey, EncodeNotificationParameter(action.Action));

                    if (action.Uri != null)
                        button.AddArgument(UriKey, EncodeNotificationParameter(action.Uri));

                    toastBuilder.AddButton(button);
                }
            }

            if (notification.Data.Inputs.Count > 0)
            {
                foreach (var input in notification.Data.Inputs)
                {
                    if (string.IsNullOrEmpty(input.Id))
                        continue;

                    toastBuilder.AddTextBox(input.Id, input.Text, input.Title);
                }
            }

            if (!string.IsNullOrWhiteSpace(notification.Data.Group))
                toastBuilder.SetGroup(notification.Data.Group);
            if (!string.IsNullOrWhiteSpace(notification.Data.Tag))
                toastBuilder.SetTag(notification.Data.Tag);

            if (notification.Data.Sticky)
            {
                toastBuilder.SetScenario(AppNotificationScenario.Reminder);
                if (notification.Data.Actions.Count == 0)
                    toastBuilder.AddButton(new AppNotificationButton(LocalizerHelper.GetLocalizedString("General_Dismiss"))); //Note(Amadeo): required for reminder scenario
            }

            if (AppNotificationBuilder.IsUrgentScenarioSupported() && notification.Data.Importance == NotificationData.ImportanceHigh)
            {
                toastBuilder.SetScenario(AppNotificationScenario.Urgent);
                if (notification.Data.Sticky)
                    Log.Warning("[NOTIFICATIONS] Notification importance overrides sticky", notification.Title);
            }

            if (!string.IsNullOrWhiteSpace(notification.Data.IconUrl))
                toastBuilder.SetAppLogoOverride(new Uri(notification.Data.IconUrl));

            var toast = toastBuilder.BuildNotification();

            if (notification.Data.Duration > 0)
                toast.Expiration = DateTime.Now.AddSeconds(notification.Data.Duration);

            _notificationManager.Show(toast);

            if (toast.Id == 0)
                Log.Error("[NOTIFICATIONS] Notification '{err}' failed to show", notification.Title);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[NOTIFICATIONS] Error while showing notification: {err}\r\n{json}", ex.Message, JsonConvert.SerializeObject(notification, Formatting.Indented));
        }
    }

    public void RegisterNotificationActionHandler(string handlerId, INotificationActionHandler handler)
    {
        if (_notificationActionHandlers.ContainsKey(handlerId))
            throw new ArgumentException($"handler with id {handlerId} already registered");

        _notificationActionHandlers[handlerId] = handler;
    }

    public void UnregisterNotificationActionHandler(string handlerId)
    {
        _notificationActionHandlers.Remove(handlerId);
    }

    public async Task HandleAppActivation(AppActivationArguments activationArguments)
    {
        var appNotificationArgs = activationArguments.Data as AppNotificationActivatedEventArgs;
        if (appNotificationArgs == null || appNotificationArgs.Argument == null)
            return;

        await HandleNotificationInvoked(appNotificationArgs);
    }

    public async Task HandleMqttMessage(MqttApplicationMessage message)
    {
        try
        {
            var notification = JsonConvert.DeserializeObject<Notification>(Encoding.UTF8.GetString(message.PayloadSegment));
            if (notification == null)
                return;

            if (notification.Message == SpecialClear) //NOTE(Amadeo): consider gathering all "groups" of notifications
            {                                         // with given tag and then removing them sequentially.
                if (!string.IsNullOrWhiteSpace(notification.Data.Tag) && !string.IsNullOrWhiteSpace(notification.Data.Group))
                    await _notificationManager.RemoveByTagAndGroupAsync(notification.Data.Tag, notification.Data.Group);
                else if (!string.IsNullOrWhiteSpace(notification.Data.Tag))
                    await _notificationManager.RemoveByTagAsync(notification.Data.Tag);

                return;
            }

            await ShowNotification(notification);
        }
        catch (Exception ex)
        {
            Log.Error("[NOTIFICATIONS] Error handling MQTT notification: {msg}", ex.Message);
        }

    }
}
