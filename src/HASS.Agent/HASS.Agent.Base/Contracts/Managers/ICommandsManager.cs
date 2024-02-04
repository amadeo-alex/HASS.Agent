using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Models.Entity;

namespace HASS.Agent.Base.Contracts.Managers;
internal interface ICommandsManager
{
    List<AbstractDiscoverable> Commands { get; }

    void Initialize();
    void Stop();
    void Pause();
    void Resume();
    Task PublishAllCommandsAsync();
    Task UnpublishAllCommandsAsync();
    Task UpdateCommandsStateAsync();
    void Process();
    void ResetAllCommandsChecks();
}
