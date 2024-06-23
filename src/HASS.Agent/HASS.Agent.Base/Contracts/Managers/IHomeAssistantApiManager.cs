using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Base.Contracts.Managers;
internal interface IHomeAssistantApiManager
{
    public const string EVENT_NOTIFICATION = "hass_agent_notifications";

    public Task InitializeAsync();
    public Task FireEventAsync();
}
