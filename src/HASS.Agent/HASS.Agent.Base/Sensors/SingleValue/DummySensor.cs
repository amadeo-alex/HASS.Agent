using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using HASS.Agent.Base.Contracts.Models.Entity;
using HASS.Agent.Base.Contracts.Models.Mqtt;
using HASS.Agent.Base.Helpers;
using HASS.Agent.Base.Models;
using HASS.Agent.Base.Models.Mqtt;
using HASS.Agent.Base.Contracts.Managers;
using Microsoft.Extensions.DependencyInjection;

namespace HASS.Agent.Base.Sensors.SingleValue;

/// <summary>
/// Dummy sensor containing random values between 0 and 100
/// </summary>
public class DummySensor : AbstractSingleValueSensor
{
    public const string EnsureRandomKey = "ensureRandom";
    public const string MinValueKey = "min";
    public const string MaxValueKey = "max";
    public const string MaxRetriesKey = "maxRetries";
        
    public override string DefaultEntityIdName { get; } = "dummySensor";

    private MqttSensorDiscoveryConfigModel? _discoveryConfigModel;

    public DummySensor(IServiceProvider serviceProvider, ConfiguredEntity configuredSensor) : base(serviceProvider, configuredSensor)
    {
        var mqtt = serviceProvider.GetService<IMqttManager>();

        //Category = EntityCategory.Parse("Other/Debug");
    }

    public override AbstractMqttDiscoveryConfigModel ConfigureAutoDiscoveryConfig(string discoveryPrefix, AbstractMqttDeviceConfigModel deviceConfigModel)
    {
        _discoveryConfigModel = new MqttSensorDiscoveryConfigModel()
        {
            Name = Name,
            UniqueId = UniqueId,
            ObjectId = $"{deviceConfigModel.Name}_{EntityIdName}",
            Device = deviceConfigModel,
            StateTopic = $"{discoveryPrefix}/{Domain}/{deviceConfigModel.Name}/{EntityIdName}/state",
            AvailabilityTopic = $"{discoveryPrefix}/hass.agent/{deviceConfigModel.Name}/availability"
        };

        return _discoveryConfigModel;
    }

    public override MqttSensorDiscoveryConfigModel? GetAutoDiscoveryConfig() => _discoveryConfigModel;

    public async override Task<string> GetState()
    {
        var start = _configuration.GetIntParameter(MinValueKey, 0);
        var end = _configuration.GetIntParameter(MaxValueKey, 100);
        var maxRetries = _configuration.GetIntParameter(MaxRetriesKey, 100);

        var retries = 0;

        string? someValue;
        do
        {
            someValue = Random.Shared.Next(start, end).ToString();
            retries++;
        } while (_configuration.GetBoolParameter(EnsureRandomKey, false) && someValue == PreviousPublishedState && retries <= maxRetries);

        return someValue;
    }

    public override ConfiguredEntity ToConfiguredEntity()
    {
        var configuredSensor = new ConfiguredEntity()
        {
            Type = typeof(DummySensor).Name,
            EntityIdName = EntityIdName,
            Name = Name,
            UpdateIntervalSeconds = UpdateIntervalSeconds,
            UniqueId = Guid.Parse(UniqueId),
            Active = Active,
        };

        configuredSensor.SetParameter("someSecretValue", "secretParameter");

        return configuredSensor;
    }
}
