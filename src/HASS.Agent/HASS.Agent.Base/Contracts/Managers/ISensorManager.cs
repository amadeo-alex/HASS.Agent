using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Models.Entity;
using HASS.Agent.Base.Models;

namespace HASS.Agent.Base.Contracts.Managers;
public interface ISensorManager
{
    ObservableCollection<AbstractDiscoverable> Sensors { get; }

    bool Pause { get; set; }
    bool Exit { get; set; }

    Task Initialize();
    Task PublishSensorsDiscoveryAsync(bool force);
    Task UnpublishSensorsDiscoveryAsync();
    Task PublishSensorsStateAsync();
    Task Process();
    void ResetAllSensorChecks();
    //public Task LoadAsync(List<ConfiguredEntity> sensors, List<ConfiguredEntity> toBeDeletedSensors);
    //public Task<List<ConfiguredEntity>> SaveAsync();
}
