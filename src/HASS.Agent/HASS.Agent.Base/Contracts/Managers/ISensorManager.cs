using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Models.Entity;
using HASS.Agent.Base.Models;

namespace HASS.Agent.Base.Contracts.Managers;
public interface ISensorManager
{
    List<AbstractDiscoverable> Sensors { get; }

    void Initialize();
    void Stop();
    void Pause();
    void Resume();
    Task PublishAllSensorsAsync();
    Task UnpublishAllSensorsAsync();
    Task UpdateSensorsStateAsync();
    void Process();
    void ResetAllSensorChecks();
    //public Task LoadAsync(List<ConfiguredEntity> sensors, List<ConfiguredEntity> toBeDeletedSensors);
    //public Task<List<ConfiguredEntity>> SaveAsync();
}
