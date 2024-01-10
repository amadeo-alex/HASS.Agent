using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Models;

namespace HASS.Agent.Base.Contracts.Managers;
internal interface ISensorManager
{
    public void Initialize();
    public void Stop();
    public void Pause();
    public void Resume();
    public Task PublishAllSensorsAsync();
    public Task UnpublishAllSensorsAsync();
    public Task UpdateSensorsStateAsync();
    public void Process();
    public void ResetAllSensorChecks();
    public Task LoadAsync(List<ConfiguredEntity> sensors, List<ConfiguredEntity> toBeDeletedSensors);
    public Task<List<ConfiguredEntity>> SaveAsync();
}
