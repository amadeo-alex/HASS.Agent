using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.Base.Contracts.Models.Entity;
using HASS.Agent.Base.Contracts.Models.Mqtt;
using HASS.Agent.Base.Models;
using HASS.Agent.Base.Models.Mqtt;
using Microsoft.Extensions.DependencyInjection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HASS.Agent.Base.Commands;
public class DummyCommand : AbstractCommand
{

    public override string DefaultEntityIdName { get; } = "dummyCommand";

    private MqttCommandDiscoveryConfigModel? _discoveryConfigModel;
    private string _state = StateOff;

    public DummyCommand(IServiceProvider serviceProvider, ConfiguredEntity configuredSensor) : base(serviceProvider, configuredSensor)
    {
        var mqtt = serviceProvider.GetService<IMqttManager>();
    }

    public override AbstractMqttDiscoveryConfigModel ConfigureAutoDiscoveryConfig(string discoveryPrefix, AbstractMqttDeviceConfigModel deviceConfigModel)
    {
        _discoveryConfigModel = new MqttCommandDiscoveryConfigModel()
        {
            Name = Name,
            UniqueId = UniqueId,
            ObjectId = $"{deviceConfigModel.Name}_{EntityIdName}",
            Device = deviceConfigModel,
            StateTopic = $"{discoveryPrefix}/{Domain}/{deviceConfigModel.Name}/{EntityIdName}/state",
            AvailabilityTopic = $"{discoveryPrefix}/hass.agent/{deviceConfigModel.Name}/availability",
            CommandTopic = $"{discoveryPrefix}/{Domain}/{deviceConfigModel.Name}/{EntityIdName}/set",
            ActionTopic = $"{discoveryPrefix}/{Domain}/{deviceConfigModel.Name}/{EntityIdName}/action"
        };

        return _discoveryConfigModel;
    }

    public override MqttCommandDiscoveryConfigModel? GetAutoDiscoveryConfig() => _discoveryConfigModel;

    public override string State => _state;

    public override void TurnOn()
    {
        TurnOn(ConfiguredAction);
    }
    public override void TurnOn(string action)
    {
        if(_state == StateOff)
            _state = StateOn;
        else
            _state = StateOff;
    }
    public override void TurnOff()
    {
        _state = StateOff;
    }

    public override ConfiguredEntity ToConfiguredEntity()
    {
        var configuredCommand = new ConfiguredEntity()
        {
            Type = typeof(DummyCommand).Name,
            EntityIdName = EntityIdName,
            Name = Name,
            UpdateIntervalSeconds = UpdateIntervalSeconds,
            UniqueId = Guid.Parse(UniqueId),
        };

        configuredCommand.SetParameter("someSecretValueCMD", "secretParameterCMD");

        return configuredCommand;
    }
}
