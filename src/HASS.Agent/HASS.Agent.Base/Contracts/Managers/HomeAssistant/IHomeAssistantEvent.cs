using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Base.Contracts.Managers.HomeAssistant;
public interface IHomeAssistantEvent
{
    public string Type { get; set; }
    public object? EventData { get; set; }
}
