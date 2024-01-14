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

namespace HASS.Agent.Base.Sensors.SingleValue;

/// <summary>
/// Dummy sensor containing random values between 0 and 100
/// </summary>
public class DummySensor : AbstractSingleValueSensor
{
    private const string c_defaultName = "dummy";

    private MqttSensorDiscoveryConfigModel? _discoveryConfigModel;

    public DummySensor(string entityIdName = DummySensor.c_defaultName, string name = DummySensor.c_defaultName, int updateIntervalSeconds = 30, string uniqueId = "")
        : base(entityIdName ?? c_defaultName, name ?? c_defaultName, updateIntervalSeconds, uniqueId, false)
    {

    }
    public DummySensor(ConfiguredEntity configuredSensor)
    : base(configuredSensor.EntityIdName, configuredSensor.Name, configuredSensor.UpdateIntervalSeconds, configuredSensor.UniqueId.ToString(), false)
    {

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
            AvailabilityTopic = $"{discoveryPrefix}/{Domain}/{deviceConfigModel.Name}/availability"
        };

        return _discoveryConfigModel;
    }

    public override MqttSensorDiscoveryConfigModel? GetAutoDiscoveryConfig() => _discoveryConfigModel;

    public override string State => Random.Shared.Next(0, 100).ToString();

    public override ConfiguredEntity ToConfiguredEntity()
    {
        var configuredSensor = new ConfiguredEntity()
        {
            Type = typeof(DummySensor).Name,
            EntityIdName = EntityIdName,
            Name = Name,
            UpdateIntervalSeconds = UpdateIntervalSeconds,
            UniqueId = Guid.Parse(UniqueId),
        };

        configuredSensor.SetParameter("someSecretValue", "secretParameter");

        return configuredSensor;
    }
}
