using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.Base.Contracts.Models.Entity;
using HASS.Agent.Base.Models;

namespace HASS.Agent.Base.Managers;

public class EntityTypeRegistry : IEntityTypeRegistry
{
    public Dictionary<string, Type> RegisteredSensorTypes { get; } = [];

    public Dictionary<string, Type> RegisteredCommandTypes { get; } = [];


    public void RegisterSensorType(Type sensorType)
    {
        if (!sensorType.IsAssignableTo(typeof(IDiscoverable)))
            throw new ArgumentException($"{sensorType} is not derived from {nameof(IDiscoverable)}");

        var typeName = sensorType.Name;

        if (RegisteredSensorTypes.ContainsKey(typeName))
            throw new ArgumentException($"sensor {sensorType} already registered");

        RegisteredSensorTypes[typeName] = sensorType;
    }

    public void RegisterCommandType(Type commandType)
    {
        if (!commandType.IsAssignableTo(typeof(IDiscoverable)))
            throw new ArgumentException($"{commandType} is not derived from {nameof(IDiscoverable)}");

        var typeName = commandType.Name;

        if (RegisteredCommandTypes.ContainsKey(typeName))
            throw new ArgumentException($"command {commandType} already registered");

        RegisteredCommandTypes[typeName] = commandType;
    }

    private IDiscoverable CreateDiscoverableInstance(Type discoverableType, ConfiguredEntity configuredEntity)
    {
        var constructorMethod = discoverableType.GetConstructor([typeof(ConfiguredEntity)])
            ?? throw new MethodAccessException($"type {discoverableType} is missing required constructor accepting ConfiguredEntity");

        var obj = constructorMethod.Invoke(new object[] { configuredEntity })
            ?? throw new Exception($"{discoverableType} instance cannot be created");

        return (IDiscoverable)obj;
    }

    public IDiscoverable CreateSensorInstance(ConfiguredEntity configuredEntity)
    {
        if (!RegisteredSensorTypes.TryGetValue(configuredEntity.Type, out var type))
            throw new ArgumentException($"sensor type {configuredEntity.Type} is not registered");

        return CreateDiscoverableInstance(type, configuredEntity);
    }

    public IDiscoverable CreateCommandInstance(ConfiguredEntity configuredEntity)
    {
        if (!RegisteredCommandTypes.TryGetValue(configuredEntity.Type, out var type))
            throw new ArgumentException($"command type {configuredEntity.Type} is not registered");

        return CreateDiscoverableInstance(type, configuredEntity);
    }
}
