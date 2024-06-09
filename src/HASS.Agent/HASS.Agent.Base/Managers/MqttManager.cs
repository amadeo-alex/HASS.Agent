using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.Base.Contracts.Models.Entity;
using HASS.Agent.Base.Contracts.Models.MediaPlayer;
using HASS.Agent.Base.Contracts.Models.Mqtt;
using HASS.Agent.Base.Enums;
using HASS.Agent.Base.Models;
using HASS.Agent.Base.Models.Mqtt;
using MQTTnet;
using MQTTnet.Adapter;
using MQTTnet.Client;
using MQTTnet.Exceptions;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Windows.Globalization;

namespace HASS.Agent.Base.Managers;

public partial class MqttManager : ObservableObject, IMqttManager
{
    public const string DefaultMqttDiscoveryPrefix = "homeassistant";
    public const string PayloadOnline = "online";
    public const string PayloadOffline = "offline";

    private readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        Formatting = Formatting.Indented,
        ContractResolver = new DefaultContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        },
        NullValueHandling = NullValueHandling.Ignore,
    };

    private readonly ISettingsManager _settingsManager;
    private readonly ApplicationInfo _applicationInfo;
    private readonly IGuidManager _guidManager;

    private IManagedMqttClient _mqttClient;
    private readonly ManagedMqttClientOptions _mqttClientOptions;

    private bool _connectionErrorLogged = false;

    private DateTime _lastAvailabilityAnnouncment = DateTime.MinValue;
    private DateTime _lastAvailabilityAnnouncmentFailed = DateTime.MinValue;

    private readonly Dictionary<string, IMqttMessageHandler> _mqttMessageHandlers = [];

    [ObservableProperty]
    public MqttStatus status = MqttStatus.NotInitialized;
    public bool Ready { get; private set; } = false;

    public AbstractMqttDeviceConfigModel DeviceConfigModel { get; set; }

    public MqttManager(ISettingsManager settingsManager, ApplicationInfo applicationInfo, IGuidManager guidManager)
    {
        _settingsManager = settingsManager;
        _applicationInfo = applicationInfo;
        _guidManager = guidManager;

        var deviceName = _settingsManager.Settings.Application.DeviceName;
        DeviceConfigModel = new MqttDeviceDiscoveryConfigModel()
        {
            Name = deviceName,
            Identifiers = $"hass.agent-{deviceName}",
            Manufacturer = "HASS.Agent Team",
            Model = Environment.OSVersion.ToString(),
            SoftwareVersion = _applicationInfo.Version.ToString(),
        };

        _mqttClient = GetMqttClient();
        _mqttClientOptions = GetMqttClientOptions();
    }

    private IManagedMqttClient GetMqttClient()
    {
        Log.Information("[MQTT] Initializing");

        if (!_settingsManager.Settings.Mqtt.Enabled)
        {
            Log.Information("[MQTT] Initialization stopped, disabled through settings");
            return new MqttFactory().CreateManagedMqttClient();
        }

        _mqttClient = new MqttFactory().CreateManagedMqttClient();
        _mqttClient.ConnectedAsync += OnConnected;
        _mqttClient.ConnectingFailedAsync += OnConnectingFailed;
        _mqttClient.ApplicationMessageReceivedAsync += OnApplicationMessageReceived;
        _mqttClient.DisconnectedAsync += OnDisconnected;
        _mqttClient.ApplicationMessageSkippedAsync += OnApplicationMessageSkipped;

        Log.Information("[MQTT] Initialized");
        return _mqttClient;
    }

    public void RegisterMessageHandler(string topic, IMqttMessageHandler handler)
    {
        if (_mqttMessageHandlers.ContainsKey(topic))
            throw new ArgumentException($"handler for {topic} already registered");

        _mqttMessageHandlers[topic] = handler;
        _mqttClient.SubscribeAsync(topic);
    }

    public void UnregisterMessageHandler(string topic)
    {
        _mqttClient.UnsubscribeAsync(topic);
        _mqttMessageHandlers.Remove(topic);
    }

    public async Task StartClient()
    {
        try
        {
            await _mqttClient.StartAsync(_mqttClientOptions);
            InitialRegistration();
        }
        catch (MqttConnectingFailedException e)
        {
            Log.Error("[MQTT] Unable to connect to broker: {msg}", e.Result.ToString());
        }
        catch (MqttCommunicationException e)
        {
            Log.Error("[MQTT] Unable to communicate with broker: {msg}", e.Message);
        }
        catch (Exception e)
        {
            Log.Error("[MQTT] Exception while connecting with broker: {msg}", e.ToString());
        }
    }

    private async void InitialRegistration()
    {
        while (!_mqttClient.IsConnected)
            await Task.Delay(2000);

        await AnnounceAvailabilityAsync();
        Ready = true;

        Log.Information("[MQTT] Initial registration completed");
    }

    private async Task AnnounceAvailabilityAsync(bool offline = false)
    {
        try
        {
            if (!offline)
            {
                if ((DateTime.Now - _lastAvailabilityAnnouncment).TotalSeconds < 30) //TODO(Amadeo): make configurable via UI
                    return;
            }

            if (_mqttClient.IsConnected)
            {
                var topic = $"{_settingsManager.Settings.Mqtt.DiscoveryPrefix}/hass.agent/{_settingsManager.Settings.Application.DeviceName}/availability";
                var availabilityMessage = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(offline ? PayloadOffline : PayloadOnline)
                    .WithRetainFlag(_settingsManager.Settings.Mqtt.UseRetainFlag)
                    .Build();

                await _mqttClient.EnqueueAsync(availabilityMessage);

                //TODO: integration message

            }
            else
            {
                if ((DateTime.Now - _lastAvailabilityAnnouncmentFailed).TotalMinutes < 5) //TODO(Amadeo): make configurable?
                    return;

                _lastAvailabilityAnnouncmentFailed = DateTime.Now;
                Log.Warning("[MQTT] Not connected, availability announcement dropped");
            }

            _lastAvailabilityAnnouncment = DateTime.Now;
        }
        catch (Exception e)
        {
            Log.Fatal(e, "[MQTT] Error while announcing availability: {err}", e.Message);
        }
    }

    private async Task OnDisconnected(MqttClientDisconnectedEventArgs args)
    {
        Status = MqttStatus.Disconnected;
        Log.Information("[MQTT] Disconnected");
    }

    private async Task OnApplicationMessageSkipped(ApplicationMessageSkippedEventArgs args)
    {
        Log.Information("[MQTT] Message skipped/dropped");
    }

    private async Task OnApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs arg)
    {
        var applicationMessage = arg.ApplicationMessage;
        if (applicationMessage.PayloadSegment.Count == 0)
        {
            Log.Information("[MQTT] Received empty payload on {topic}", applicationMessage.Topic);
            return;
        }

        try
        {
            foreach(var (registeredTopic, handler) in _mqttMessageHandlers)
            {
                if(CheckTopicMatch(applicationMessage.Topic, registeredTopic))
                    await handler.HandleMqttMessage(applicationMessage);
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[MQTT] Error while processing received message: {err}", ex.Message);
        }

        return;

        try
        {
            if (applicationMessage.Topic == $"hass.agent/notifications/{DeviceConfigModel.Name}")
            {
                var payload = Encoding.UTF8.GetString(applicationMessage.PayloadSegment).ToLower();
                if (payload == null)
                    return;

                //var notification = JsonConvert.DeserializeObject<Notification>(payload, _jsonSerializerSettings);
                //_notificationManager.HandleReceivedNotification(notification);
                //TODO(Amadeo): event/observable to show notification

                return;
            }

            if (applicationMessage.Topic == $"hass.agent/media_player/{DeviceConfigModel.Name}/cmd")
            {
                var payload = Encoding.UTF8.GetString(applicationMessage.PayloadSegment).ToLower();
                if (payload == null)
                    return;

                var command = JsonConvert.DeserializeObject<MediaPlayerCommand>(payload, _jsonSerializerSettings)!;
                //_mediaManager.HandleReceivedCommand(command);

                /*                switch (command.Type)
                                {
                                    case MediaPlayerCommandType.PlayMedia:
                                        MediaManager.ProcessMedia(command.Data.GetString());
                                        break;
                                    case MediaPlayerCommandType.Seek:
                                        MediaManager.ProcessSeekCommand(TimeSpan.FromSeconds(command.Data.GetDouble()).Ticks);
                                        break;
                                    case MediaPlayerCommandType.SetVolume:
                                        MediaManagerCommands.SetVolume(command.Data.GetInt32());
                                        break;
                                    default:
                                        MediaManager.ProcessCommand(command.Command);
                                        break;
                                }*/

                return;
            }

            //_commandsManager.HandleReceivedCommand(applicationMessage);

            /*            foreach (var command in Variables.Commands)
                        {
                            var commandConfig = (CommandDiscoveryConfigModel)command.GetAutoDiscoveryConfig();

                            if (commandConfig.Command_topic == applicationMessage.Topic)
                                HandleCommandReceived(applicationMessage, command);
                            else if (commandConfig.Action_topic == applicationMessage.Topic)
                                HandleActionReceived(applicationMessage, command);
                        }*/
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[MQTT] Error while processing received message: {err}", ex.Message);
        }
    }

    private async Task OnConnected(MqttClientConnectedEventArgs arg)
    {
        Status = MqttStatus.Connected;
        Log.Information("[MQTT] Connected");

        _connectionErrorLogged = false;

        return;
    }

    private async Task OnConnectingFailed(ConnectingFailedEventArgs arg)
    {
        Status = MqttStatus.Error;
        Log.Information("[MQTT] Connecting failed");

        if (_connectionErrorLogged)
            return;

        _connectionErrorLogged = true;

        var exceptionMessage = arg.Exception.ToString();
        if (exceptionMessage.Contains("SocketException"))
            Log.Error("[MQTT] Error while connecting: {err}", arg.Exception.Message);
        else if (exceptionMessage.Contains("MqttCommunicationTimedOutException"))
            Log.Error("[MQTT] Error while connecting: {err}", "Connection timed out");
        else if (exceptionMessage.Contains("NotAuthorized"))
            Log.Error("[MQTT] Error while connecting: {err}", "Not authorized, check your credentials.");
        else
            Log.Fatal(arg.Exception, "[MQTT] Error while connecting: {err}", arg.Exception.Message);

        //TODO(Amadeo): event/observable and notify user
    }

    private ManagedMqttClientOptions GetMqttClientOptions()
    {
        if (string.IsNullOrWhiteSpace(_settingsManager.Settings.Mqtt.Address))
        {
            Log.Warning("[MQTT] Required configuration missing");

            return new ManagedMqttClientOptionsBuilder().Build();
        }

        // id can be random, but we'll store it for consistency (unless user-defined)
        if (string.IsNullOrWhiteSpace(_settingsManager.Settings.Mqtt.ClientId))
        {
            Log.Information("[MQTT] ClientId is empty, generating new one");
            _settingsManager.Settings.Mqtt.ClientId = _guidManager.GenerateShortGuid();
            //TODO(Amadeo): save settings to file
            //SettingsManager.StoreAppSettings();
        }

        var clientOptionsBuilder = new MqttClientOptionsBuilder()
            .WithClientId(_settingsManager.Settings.Mqtt.ClientId)
            .WithTcpServer(_settingsManager.Settings.Mqtt.Address, _settingsManager.Settings.Mqtt.Port)
            .WithCleanSession()
            .WithWillTopic($"{_settingsManager.Settings.Mqtt.DiscoveryPrefix}/sensor/{DeviceConfigModel.Name}/availability")
            .WithWillPayload(PayloadOffline)
            .WithWillRetain(_settingsManager.Settings.Mqtt.UseRetainFlag)
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(15));

        if (!string.IsNullOrEmpty(_settingsManager.Settings.Mqtt.Username))
            clientOptionsBuilder.WithCredentials(_settingsManager.Settings.Mqtt.Username, _settingsManager.Settings.Mqtt.Password);

        var certificates = new List<X509Certificate>();
        if (!string.IsNullOrEmpty(_settingsManager.Settings.Mqtt.RootCertificate))
        {
            if (!File.Exists(_settingsManager.Settings.Mqtt.RootCertificate))
                Log.Error("[MQTT] Provided root certificate not found: {cert}", _settingsManager.Settings.Mqtt.RootCertificate);
            else
                certificates.Add(new X509Certificate2(_settingsManager.Settings.Mqtt.RootCertificate));
        }

        if (!string.IsNullOrEmpty(_settingsManager.Settings.Mqtt.ClientCertificate))
        {
            if (!File.Exists(_settingsManager.Settings.Mqtt.ClientCertificate))
                Log.Error("[MQTT] Provided client certificate not found: {cert}", _settingsManager.Settings.Mqtt.ClientCertificate);
            else
                certificates.Add(new X509Certificate2(_settingsManager.Settings.Mqtt.ClientCertificate));
        }

        var clientTlsOptions = new MqttClientTlsOptions()
        {
            UseTls = _settingsManager.Settings.Mqtt.UseTls,
            AllowUntrustedCertificates = _settingsManager.Settings.Mqtt.AllowUntrustedCertificates,
            SslProtocol = _settingsManager.Settings.Mqtt.UseTls ? SslProtocols.Tls12 : SslProtocols.None,
        };

        //TODO(Amadeo): add more granular control to the UI
        if (_settingsManager.Settings.Mqtt.AllowUntrustedCertificates)
        {
            clientTlsOptions.IgnoreCertificateChainErrors = _settingsManager.Settings.Mqtt.AllowUntrustedCertificates;
            clientTlsOptions.IgnoreCertificateRevocationErrors = _settingsManager.Settings.Mqtt.AllowUntrustedCertificates;
            clientTlsOptions.CertificateValidationHandler = delegate (MqttClientCertificateValidationEventArgs _)
            {
                return true;
            };
        }

        if (certificates.Count > 0)
            clientTlsOptions.ClientCertificatesProvider = new DefaultMqttCertificatesProvider(certificates);

        clientOptionsBuilder.WithTlsOptions(clientTlsOptions);
        clientOptionsBuilder.Build();

        return new ManagedMqttClientOptionsBuilder()
            .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
            .WithClientOptions(clientOptionsBuilder).Build();
    }

    public async Task AnnounceDeviceConfigModelAsync()
    {

        return;
    }

    public async Task ClearDeviceConfigModelAsync()
    {

        return;
    }

    public async Task DisconnectAsync()
    {
        Status = MqttStatus.Disconnecting;
        await _mqttClient.StopAsync();
        return;
    }

    public async Task PublishAsync(MqttApplicationMessage message)
    {
        await _mqttClient.EnqueueAsync(message);
        return;
    }

    public async Task ReinitializeAsync()
    {

        return;
    }

    public async Task SubscribeAsync(AbstractDiscoverable command)
    {

        return;
    }

    public async Task UnsubscribeAsync(AbstractDiscoverable command)
    {

        return;
    }

    /// Idea thanks to https://github.com/hobbyquaker/mqtt-wildcard implementation
    private bool CheckTopicMatch(string messageTopic, string registeredTopic)
    {
        if (messageTopic == registeredTopic || registeredTopic == "#")
            return true;

        var splitTopic = messageTopic.Split('/');
        var splitRegisteredTopic = registeredTopic.Split('/');
        if(splitTopic.Length > splitRegisteredTopic.Length && splitRegisteredTopic.Last() != "#")
            return false;

        var index = 0;
        for (; index < splitTopic.Length; index++)
        {
            if (splitRegisteredTopic[index] == "+") 
                continue;
            else if (splitRegisteredTopic[index] == "#")
                return true;
            else if (splitRegisteredTopic[index] != splitTopic[index])
                return false;
        }

       if (splitRegisteredTopic.Length > index && splitRegisteredTopic[index] == "#")
            index += 1;

        return index == splitRegisteredTopic.Length;
    }
}
