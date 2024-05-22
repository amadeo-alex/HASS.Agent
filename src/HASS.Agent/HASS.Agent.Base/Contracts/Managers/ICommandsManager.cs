using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Models.Entity;
using MQTTnet;

namespace HASS.Agent.Base.Contracts.Managers;
public interface ICommandsManager
{
    ObservableCollection<AbstractDiscoverable> Commands { get; }

    bool Pause { get; set; }
    bool Exit { get; set; }

    void Initialize();
    Task PublishCommandsDiscoveryAsync(bool force);
    Task UnpublishCommandsDiscoveryAsync();
    Task PublishCommandsStateAsync();
    Task Process();
    void ResetAllCommandsChecks();
}
