using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.Base.Contracts.Models.Entity;
using HASS.Agent.Base.Models;
using HASS.Agent.Base.Models.Entity;
using HASS.Agent.Base.Sensors.SingleValue;

namespace HASS.Agent.Base.Managers;

public class EntityTypeRegistry : IEntityTypeRegistry
{
    public Dictionary<string, RegisteredEntity> SensorTypes { get; } = [];
    public Dictionary<string, RegisteredEntity> CommandTypes { get; } = [];

    public Dictionary<string, RegisteredEntity> ClientSensorTypes => SensorTypes.Where(st => st.Value.ClientCompatible)
        .ToDictionary(st => st.Key, st => st.Value);
    public Dictionary<string, RegisteredEntity> SatelliteSensorTypes => SensorTypes.Where(st => st.Value.SatelliteCompatible)
        .ToDictionary(st => st.Key, st => st.Value);

    public EntityTypeRegistry()
    {
        RegisterSensorType(typeof(DummySensor), true, true);
    }

    public void RegisterSensorType(Type sensorType, bool clientCompatible, bool satelliteCompatible)
    {
        if (!sensorType.IsAssignableTo(typeof(IDiscoverable)))
            throw new ArgumentException($"{sensorType} is not derived from {nameof(IDiscoverable)}");

        var typeName = sensorType.Name;

        if (SensorTypes.ContainsKey(typeName))
            throw new ArgumentException($"sensor {sensorType} already registered");

        SensorTypes[typeName] = new RegisteredEntity{
            EntityType = sensorType,
            ClientCompatible = clientCompatible,
            SatelliteCompatible = satelliteCompatible
        };
    }

    public void RegisterCommandType(Type commandType, bool clientCompatible, bool satelliteCompatible)
    {
        if (!commandType.IsAssignableTo(typeof(IDiscoverable)))
            throw new ArgumentException($"{commandType} is not derived from {nameof(IDiscoverable)}");

        var typeName = commandType.Name;

        if (CommandTypes.ContainsKey(typeName))
            throw new ArgumentException($"command {commandType} already registered");

        CommandTypes[typeName] = new RegisteredEntity
        {
            EntityType = commandType,
            ClientCompatible = clientCompatible,
            SatelliteCompatible = satelliteCompatible
        };
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
        if (!SensorTypes.TryGetValue(configuredEntity.Type, out var registeredEntity))
            throw new ArgumentException($"sensor type {configuredEntity.Type} is not registered");

        return CreateDiscoverableInstance(registeredEntity.EntityType, configuredEntity);
    }

    public IDiscoverable CreateCommandInstance(ConfiguredEntity configuredEntity)
    {
        if (!CommandTypes.TryGetValue(configuredEntity.Type, out var registeredEntity))
            throw new ArgumentException($"command type {configuredEntity.Type} is not registered");

        return CreateDiscoverableInstance(registeredEntity.EntityType, configuredEntity);
    }
}
