using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Base.Contracts.Managers;
public interface IHomeAssistantApiManager
{
    public Task InitializeAsync();
    public Task FireEventAsync(string eventName, object eventData);
}
