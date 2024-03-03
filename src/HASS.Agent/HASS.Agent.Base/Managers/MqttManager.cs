using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.Base.Contracts.Models.Entity;
using HASS.Agent.Base.Contracts.Models.MediaPlayer;
using HASS.Agent.Base.Contracts.Models.Mqtt;
using HASS.Agent.Base.Contracts.Models.Notification;
using HASS.Agent.Base.Enums;
using HASS.Agent.Base.Models;
using HASS.Agent.Base.Models.Mqtt;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Windows.Globalization;

namespace HASS.Agent.Base.Managers;
internal class MqttManager : IMqttManager
{
    public const string DefaultMqttDiscoveryPrefix = "homeassistant";

    private readonly JsonSerializerSettings jsonSerializerSettings = new()
    {
        ContractResolver = new DefaultContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        },
        NullValueHandling = NullValueHandling.Ignore,
    };

    private ISettingsManager _settingsManager;
    private ApplicationInfo _applicationInfo;
    private IGuidManager _guidManager;
    private INotificationManager _notificationManager;
    private IMediaManager _mediaManager;
    private ICommandsManager _commandsManager;

    private IManagedMqttClient? _mqttClient = null;

    private bool _connectionErrorLogged = false;

    public MqttStatus Status { get; private set; } = MqttStatus.NotInitialized;
    public bool Ready { get; private set; } = false;

    public AbstractMqttDeviceConfigModel DeviceConfigModel { get; set; }

    public MqttManager(ISettingsManager settingsManager, ApplicationInfo applicationInfo, IGuidManager guidManager,
        INotificationManager notificationManager, IMediaManager mediaManager, ICommandsManager commandsManager)
    {
        _settingsManager = settingsManager;
        _applicationInfo = applicationInfo;
        _guidManager = guidManager;
        _notificationManager = notificationManager;
        _mediaManager = mediaManager;
        _commandsManager = commandsManager;
    }

    public async Task InitializeAsync()
    {
        Log.Information("[MQTT] Initializing");

        if (!_settingsManager.ApplicationSettings.MqttEnabled)
        {
            Log.Information("[MQTT] Initialization stopped, disabled through settings");
            return;
        }

        var deviceName = _settingsManager.ApplicationSettings.DeviceName;
        DeviceConfigModel = new MqttDeviceDiscoveryConfigModel()
        {
            Name = deviceName,
            Identifiers = $"hass.agent-{deviceName}",
            Manufacturer = "HASS.Agent Team",
            Model = Environment.OSVersion.ToString(),
            SoftwareVersion = _applicationInfo.Version.ToString(),
        };

        _mqttClient = new MqttFactory().CreateManagedMqttClient();
        _mqttClient.ConnectedAsync += OnConnected;
        _mqttClient.ConnectingFailedAsync += OnConnectingFailed;
        _mqttClient.ApplicationMessageReceivedAsync += OnApplicationMessageReceived;
        _mqttClient.DisconnectedAsync += OnDisconnected;
        _mqttClient.ApplicationMessageSkippedAsync += OnApplicationMessageSkipped;

        var options = GetMqttClientOptions();
        if (options == null)
        {
            Log.Warning("[MQTT] Required configuration missing");
            return;
        }

        Log.Information("[MQTT] Initialized");
    }

    private async Task OnDisconnected(MqttClientDisconnectedEventArgs args)
    {

    }

    private async Task OnApplicationMessageSkipped(ApplicationMessageSkippedEventArgs args)
    {

    }

    private async Task OnApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs arg)
    {
        var applicationMessage = arg.ApplicationMessage;

        try
        {
            if (applicationMessage.Topic == $"hass.agent/notifications/{DeviceConfigModel.Name}")
            {
                var payload = Encoding.UTF8.GetString(applicationMessage.PayloadSegment).ToLower();
                if (payload == null)
                    return;

                var notification = JsonConvert.DeserializeObject<Notification>(payload, jsonSerializerSettings);
                _notificationManager.HandleReceivedNotification(notification);
                //TODO(Amadeo): event/observable to show notification

                return;
            }

            if (applicationMessage.Topic == $"hass.agent/media_player/{DeviceConfigModel.Name}/cmd")
            {
                var payload = Encoding.UTF8.GetString(applicationMessage.PayloadSegment).ToLower();
                if (payload == null)
                    return;

                var command = JsonConvert.DeserializeObject<MediaPlayerCommand>(payload, jsonSerializerSettings)!;
                _mediaManager.HandleReceivedCommand(command);

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

            _commandsManager.HandleReceivedCommand(applicationMessage);

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

        return;
    }

    private async Task OnConnectingFailed(ConnectingFailedEventArgs arg)
    {
        if (_mqttClient == null)
            return;

        var runningTimer = Stopwatch.StartNew();
        while (runningTimer.Elapsed.TotalSeconds < _settingsManager.ApplicationSettings.DisconnectedGracePeriodSeconds)
        {
            if (_mqttClient.IsConnected)
            {
                Status = MqttStatus.Connected;
                Log.Information("[MQTT] Connected");

                return;
            }

            await Task.Delay(TimeSpan.FromSeconds(5));
        }

        Status = MqttStatus.Error;

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

    private ManagedMqttClientOptions? GetMqttClientOptions()
    {
        if (string.IsNullOrWhiteSpace(_settingsManager.ApplicationSettings.MqttAddress))
            return null;

        // id can be random, but we'll store it for consistency (unless user-defined)
        if (string.IsNullOrWhiteSpace(_settingsManager.ApplicationSettings.MqttClientId))
        {
            Log.Information("[MQTT] ClientId is empty, generating new one");
            _settingsManager.ApplicationSettings.MqttClientId = _guidManager.GenerateShortGuid();
            //TODO(Amadeo): save settings to file
            //SettingsManager.StoreAppSettings();
        }

        var clientOptionsBuilder = new MqttClientOptionsBuilder()
            .WithClientId(_settingsManager.ApplicationSettings.MqttClientId)
            .WithTcpServer(_settingsManager.ApplicationSettings.MqttAddress, _settingsManager.ApplicationSettings.MqttPort)
            .WithCleanSession()
            .WithWillTopic($"{_settingsManager.ApplicationSettings.MqttDiscoveryPrefix}/sensor/{DeviceConfigModel.Name}/availability")
            .WithWillPayload("offline")
            .WithWillRetain(_settingsManager.ApplicationSettings.MqttUseRetainFlag)
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(15));

        if (!string.IsNullOrEmpty(_settingsManager.ApplicationSettings.MqttUsername))
            clientOptionsBuilder.WithCredentials(_settingsManager.ApplicationSettings.MqttUsername, _settingsManager.ApplicationSettings.MqttPassword);

        var certificates = new List<X509Certificate>();
        if (!string.IsNullOrEmpty(_settingsManager.ApplicationSettings.MqttRootCertificate))
        {
            if (!File.Exists(_settingsManager.ApplicationSettings.MqttRootCertificate))
                Log.Error("[MQTT] Provided root certificate not found: {cert}", _settingsManager.ApplicationSettings.MqttRootCertificate);
            else
                certificates.Add(new X509Certificate2(_settingsManager.ApplicationSettings.MqttRootCertificate));
        }

        if (!string.IsNullOrEmpty(_settingsManager.ApplicationSettings.MqttClientCertificate))
        {
            if (!File.Exists(_settingsManager.ApplicationSettings.MqttClientCertificate))
                Log.Error("[MQTT] Provided client certificate not found: {cert}", _settingsManager.ApplicationSettings.MqttClientCertificate);
            else
                certificates.Add(new X509Certificate2(_settingsManager.ApplicationSettings.MqttClientCertificate));
        }

        var clientTlsOptions = new MqttClientTlsOptions()
        {
            UseTls = _settingsManager.ApplicationSettings.MqttUseTls,
            AllowUntrustedCertificates = _settingsManager.ApplicationSettings.MqttAllowUntrustedCertificates,
            SslProtocol = _settingsManager.ApplicationSettings.MqttUseTls ? SslProtocols.Tls12 : SslProtocols.None,
        };

        //TODO(Amadeo): add more granular control to the UI
        if (_settingsManager.ApplicationSettings.MqttAllowUntrustedCertificates)
        {
            clientTlsOptions.IgnoreCertificateChainErrors = _settingsManager.ApplicationSettings.MqttAllowUntrustedCertificates;
            clientTlsOptions.IgnoreCertificateRevocationErrors = _settingsManager.ApplicationSettings.MqttAllowUntrustedCertificates;
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

        return;
    }

    public async Task PublishAsync(MqttApplicationMessage message)
    {

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
}
