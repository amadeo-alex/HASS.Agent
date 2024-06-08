using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.UI.Contracts.Managers;
using HASS.Agent.UI.Models.Notifications;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Newtonsoft.Json;
using Serilog;

namespace HASS.Agent.UI.Managers;
public class NotificationManager : INotificationManager
{
    private const string ActionPrefix = "action=";
    private const string UriPrefix = "uri=";

    private readonly ISettingsManager _settingsManager;

    private readonly AppNotificationManager _notificationManager = AppNotificationManager.Default;

    private readonly Dictionary<string, INotificationActionHandler> _notificationActionHandlers = [];

    public bool Ready { get; private set; }

    public NotificationManager(ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    public async Task Initialize()
    {
        try
        {
            if (!_settingsManager.ApplicationSettings.NotificationsEnabled)
            {
                Log.Information("[NOTIFICATIONS] Disabled");

                return;
            }

            if (!_settingsManager.ApplicationSettings.LocalApiEnabled && !_settingsManager.ApplicationSettings.MqttEnabled)
            {
                Log.Warning("[NOTIFICATIONS] Both local API and MQTT are disabled, unable to receive notifications");

                return;
            }

            if (_settingsManager.ApplicationSettings.MqttEnabled)
            {
                //_ = Task.Run(Variables.MqttManager.SubscribeNotificationsAsync); //TODO(Amadeo)
            }
            else
            {
                Log.Warning("[NOTIFICATIONS] MQTT is disabled, not all aspects of actions might work as expected");
            }

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

    public async Task ShowNotification(Notification notification, string handlerId)
    {
        if (!Ready)
            throw new Exception("NotificationManager is not initialized");

        try
        {
            if (!_settingsManager.ApplicationSettings.NotificationsEnabled)
                return;

            var toastBuilder = new AppNotificationBuilder()
                .AddText(notification.Title)
                .AddText(notification.Message);

            /*            if (!string.IsNullOrWhiteSpace(notification.Data?.Image))
                        {
                            var (success, localFile) = await StorageManager.DownloadImageAsync(notification.Data.Image);
                            if (success)
                                toastBuilder.SetInlineImage(new Uri(localFile));
                            else
                                Log.Error("[NOTIFIER] Image download failed, dropping: {img}", notification.Data.Image);
                        }*/
            //TODO(Amadeo): implement

            if (notification.Actions.Count > 0)
            {
                foreach (var action in notification.Actions)
                {
                    if (string.IsNullOrEmpty(action.Action))
                        continue;

                    var button = new AppNotificationButton(action.Title)
                        .AddArgument("action", EncodeNotificationParameter(action.Action));

                    if (action.Uri != null)
                        button.AddArgument("uri", EncodeNotificationParameter(action.Uri));

                    toastBuilder.AddButton(button);
                }
            }

            if (notification.Inputs.Count > 0)
            {
                foreach (var input in notification.Inputs)
                {
                    if (string.IsNullOrEmpty(input.Id))
                        continue;

                    toastBuilder.AddTextBox(input.Id, input.Text, input.Title);
                }
            }

            var toast = toastBuilder.BuildNotification();

            if (notification.Duration > 0)
            {
                //TODO: unreliable
                toast.Expiration = DateTime.Now.AddSeconds(notification.Duration);
            }

            _notificationManager.Show(toast);

            if (toast.Id == 0)
            {
                Log.Error("[NOTIFICATIONS] Notification '{err}' failed to show", notification.Title);
            }

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
        //TODO(Amadeo): implement
    }
}
