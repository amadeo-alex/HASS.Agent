using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Models.Entity;
using HASS.Agent.Base.Models;
using HASS.Agent.Base.Models.Entity;

namespace HASS.Agent.Base.Contracts.Managers;
public interface IEntityTypeRegistry
{
    Dictionary<string, RegisteredEntity> SensorTypes { get; }
    Dictionary<string, RegisteredEntity> CommandTypes { get; }

    void RegisterSensorType(Type sensorType, bool clientCompatible, bool satelliteCompatible);
    void RegisterCommandType(Type commandType, bool clientCompatible, bool satelliteCompatible);
    IDiscoverable CreateSensorInstance(ConfiguredEntity configuredEntity);
    IDiscoverable CreateCommandInstance(ConfiguredEntity configuredEntity);
}
