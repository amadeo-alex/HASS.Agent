using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.Base.Contracts.Models.Entity;
using HASS.Agent.Base.Models;

namespace HASS.Agent.Base.Managers;

internal class SensorManager : ISensorManager
{
    public List<AbstractDiscoverable> Sensors { get; set; } = [];

    public void Initialize()
    {
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
