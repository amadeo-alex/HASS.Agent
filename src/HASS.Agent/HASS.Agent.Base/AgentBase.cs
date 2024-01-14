using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.Base.Models;
using HASS.Agent.Base.Sensors.SingleValue;

namespace HASS.Agent.Base;

public class AgentBase
{
    private IEntityTypeRegistry _entityTypeRegistry;
    private ISensorManager _sensorManager;

    public AgentBase(IEntityTypeRegistry entityTypeRegistry, ISensorManager sensorManager)
    {
        _entityTypeRegistry = entityTypeRegistry;
        _sensorManager = sensorManager;

        InitializeEntityRegistry();
    }

    public void InitializeEntityRegistry()
    {
        _entityTypeRegistry.RegisterSensorType(typeof(DummySensor));
    }
}
