using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.Base.Contracts.Models.Entity;
using HASS.Agent.Base.Models;

namespace HASS.Agent.Base.Managers;

public class SensorManager : ISensorManager
{
    private readonly ISettingsManager _settingsManager;
    private readonly IEntityTypeRegistry _entityTypeRegistry;
    private readonly IGuidManager _guidManager;

    public List<AbstractDiscoverable> Sensors { get; set; } = [];

    public SensorManager(ISettingsManager settingsManager, IEntityTypeRegistry entityTypeRegistry, IGuidManager guidManager)
    {
        _settingsManager = settingsManager;
        _entityTypeRegistry = entityTypeRegistry;
        _guidManager = guidManager;
    }

    public void Initialize()
    {
        foreach (var configuredSensor in _settingsManager.ConfiguredSensors)
        {
            _guidManager.MarkAsUsed(configuredSensor.UniqueId);
        }
    }

    public async Task LoadAsync(List<ConfiguredEntity> sensors, List<ConfiguredEntity> toBeDeletedSensors)
    {
        foreach (var sensor in sensors)
        {
            //Sensors.Append(sens)
        }

        return;
    }
    public void Pause()
    {
    }
    public void Process()
    {
    }
    public async Task PublishAllSensorsAsync()
    {

        return;
    }
    public void ResetAllSensorChecks()
    {
    }
    public void Resume()
    {
    }
    public async Task<List<ConfiguredEntity>> SaveAsync()
    {

        return [];
    }
    public void Stop()
    {
    }
    public async Task UnpublishAllSensorsAsync()
    {

        return;
    }
    public async Task UpdateSensorsStateAsync()
    {

        return;
    }

}
