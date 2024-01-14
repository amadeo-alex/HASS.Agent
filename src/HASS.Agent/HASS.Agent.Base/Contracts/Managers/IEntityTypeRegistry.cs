using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Models.Entity;
using HASS.Agent.Base.Models;

namespace HASS.Agent.Base.Contracts.Managers;
public interface IEntityTypeRegistry
{
    Dictionary<string, Type> RegisteredSensorTypes { get; }
    Dictionary<string, Type> RegisteredCommandTypes { get; }

    void RegisterSensorType(Type sensorType);
    void RegisterCommandType(Type commandType);
    IDiscoverable CreateSensorInstance(ConfiguredEntity configuredEntity);
    IDiscoverable CreateCommandInstance(ConfiguredEntity configuredEntity);
}
