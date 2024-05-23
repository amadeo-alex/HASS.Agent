using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.Base.Contracts.Models.Entity;
using HASS.Agent.Base.Enums;
using HASS.Agent.Base.Models;
using HASS.Agent.Base.Models.Mqtt;
using MQTTnet;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Windows.AI.MachineLearning;

namespace HASS.Agent.Base.Managers;

public class SensorManager : ISensorManager
{
    private readonly ISettingsManager _settingsManager;
    private readonly IEntityTypeRegistry _entityTypeRegistry;
    private readonly IMqttManager _mqttManager;

    private readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        Formatting = Formatting.Indented,
        ContractResolver = new DefaultContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        },
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Ignore,
    };

    private bool _discoveryPublished = false;

    public bool Pause { get; set; }
    public bool Exit { get; set; }

    public ObservableCollection<AbstractDiscoverable> Sensors { get; set; } = [];

    public SensorManager(ISettingsManager settingsManager, IEntityTypeRegistry entityTypeRegistry, IMqttManager mqttManager)
    {
        _settingsManager = settingsManager;
        _entityTypeRegistry = entityTypeRegistry;
        _mqttManager = mqttManager;
    }

    private void AddSensor(ConfiguredEntity configuredSensor)
    {
        var sensor = (AbstractDiscoverable)_entityTypeRegistry.CreateSensorInstance(configuredSensor);
        sensor.ConfigureAutoDiscoveryConfig(_settingsManager.ApplicationSettings.MqttDiscoveryPrefix, _mqttManager.DeviceConfigModel);
        _ = PublishSensorAutoDiscoveryConfigAsync(sensor);
        Sensors.Add(sensor);
    }

    private void RemoveSensor(AbstractDiscoverable sensor)
    {
        Sensors.Remove(sensor);
        _ = PublishSingleSensorStateAsync(sensor, respectChecks: false, clear: true);
        _ = PublishSensorAutoDiscoveryConfigAsync(sensor, clear: true);
    }

    public void Initialize()
    {
        _settingsManager.ConfiguredSensors.CollectionChanged -= ConfiguredSensors_CollectionChanged;

        foreach (var configuredSensor in _settingsManager.ConfiguredSensors)
            AddSensor(configuredSensor);

        _settingsManager.ConfiguredSensors.CollectionChanged += ConfiguredSensors_CollectionChanged;
    }

    private void ConfiguredSensors_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _discoveryPublished = false;

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems == null)
                    return;

                foreach (ConfiguredEntity configuredSensor in e.NewItems)
                    AddSensor(configuredSensor);
                break;

            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems == null)
                    return;

                foreach (ConfiguredEntity configuredSensor in e.OldItems)
                {
                    var sensor = Sensors.Where(s => s.UniqueId == configuredSensor.UniqueId.ToString()).FirstOrDefault();
                    if (sensor != null)
                        RemoveSensor(sensor);
                }
                break;
        }
    }

    private async Task PublishSensorAutoDiscoveryConfigAsync(AbstractDiscoverable sensor, bool clear = false)
    {
        if (sensor is AbstractSingleValueSensor)
        {
            await PublishSingleSensorAutoDiscoveryConfigAsync(sensor, clear);
        }
        /*        else if (sensor is AbstractMultiValueSensor multiValueSensor)
                {
                    foreach (var singleSensor in multiValueSensor.Sensors)
                        await PublishSingleSensorAutoDiscoveryConfigAsync(singleSensor.Value, clear);
                }*/
    }

    private async Task PublishSingleSensorAutoDiscoveryConfigAsync(AbstractDiscoverable sensor, bool clear)
    {
        try
        {
            var topic = $"{_settingsManager.ApplicationSettings.MqttDiscoveryPrefix}/{sensor.Domain}/{_settingsManager.ApplicationSettings.DeviceName}/{sensor.EntityIdName}/config";

            var messageBuilder = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithRetainFlag(_settingsManager.ApplicationSettings.MqttUseRetainFlag);

            if (clear)
            {
                messageBuilder.WithPayload(Array.Empty<byte>());
            }
            else
            {
                var payload = sensor.GetAutoDiscoveryConfig();
                if (sensor.IgnoreAvailability)
                    payload.AvailabilityTopic = string.Empty;

                messageBuilder.WithPayload(JsonConvert.SerializeObject(payload, _jsonSerializerSettings));
            }

            await _mqttManager.PublishAsync(messageBuilder.Build());
        }
        catch (Exception e)
        {
            Log.Fatal("[SENSORMGR] [{name}] Error publishing discovery: {err}", sensor, e.Message);
        }
    }

    private async Task PublishSensorStateAsync(AbstractDiscoverable sensor)
    {
        if (sensor is AbstractSingleValueSensor)
        {
            await PublishSingleSensorStateAsync(sensor);
        }
        /*        else if (sensor is AbstractMultiValueSensor multiValueSensor)
                {
                    foreach (var singleSensor in multiValueSensor.Sensors)
                        await PublishSingleSensorStateAsync(singleSensor.Value);
                }*/
    }

    private async Task PublishSingleSensorStateAsync(AbstractDiscoverable sensor, bool respectChecks = true, bool clear = false)
    {
        try
        {
            if (respectChecks && sensor.LastUpdated.AddSeconds(sensor.UpdateIntervalSeconds) > DateTime.Now)
                return;

            var state = sensor.State;
            if (state == null)
                return;

            var attributes = sensor.Attributes;


            if (respectChecks &&
                sensor.PreviousPublishedState == state &&
                sensor.PreviousPublishedAttributes == attributes)
            {
                sensor.LastUpdated = DateTime.Now;
                return;
            }

            var autodiscoveryConfig = (MqttSensorDiscoveryConfigModel)sensor.GetAutoDiscoveryConfig();
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(autodiscoveryConfig.StateTopic)
                .WithRetainFlag(_settingsManager.ApplicationSettings.MqttUseRetainFlag);

            if (clear)
                message.WithPayload(Array.Empty<byte>());
            else
                message.WithPayload(state);

            await _mqttManager.PublishAsync(message.Build());

            if (sensor.UseAttributes)
            {
                var attributesMessage = new MqttApplicationMessageBuilder()
                    .WithTopic(autodiscoveryConfig.JsonAttributesTopic)
                    .WithRetainFlag(_settingsManager.ApplicationSettings.MqttUseRetainFlag);

                if (clear)
                    attributesMessage.WithPayload(Array.Empty<byte>());
                else
                    attributesMessage.WithPayload(attributes);

                await _mqttManager.PublishAsync(attributesMessage.Build());
            }

            if (!respectChecks || clear)
                return;

            sensor.PreviousPublishedState = state;
            sensor.PreviousPublishedAttributes = attributes;
            sensor.LastUpdated = DateTime.Now;
        }
        catch (Exception e)
        {
            Log.Fatal("[SENSORMGR] [{name}] Error publishing state: {err}", sensor, e.Message);
        }
    }

    public async Task PublishSensorsDiscoveryAsync(bool force = false)
    {
        if (force || !_discoveryPublished)
        {
            foreach (var sensor in Sensors)
                await PublishSensorAutoDiscoveryConfigAsync(sensor);

            _discoveryPublished = true;
        }
    }
    public async Task PublishSensorsStateAsync()
    {
        foreach (var sensor in Sensors)
        {
            if (sensor.Active)
                await PublishSensorStateAsync(sensor);
        }
    }

    public async Task UnpublishSensorsDiscoveryAsync()
    {
        foreach (var sensor in Sensors)
            await PublishSensorAutoDiscoveryConfigAsync(sensor, clear: true);
    }

    public async Task Process()
    {
        var firstRun = true;
        var firstRunDone = false;

        while (!Exit)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(750)); //TODO(Amadeo): add application config for this
                if (Pause || _mqttManager.Status != MqttStatus.Connected)
                    continue;

                await PublishSensorsDiscoveryAsync();
                await PublishSensorsStateAsync();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "[SENSORMGR] Error while processing: {err}", e.Message);
            }
        }
    }

    public void ResetAllSensorChecks()
    {
        foreach (var sensor in Sensors)
            sensor.ResetChecks();
    }

}
