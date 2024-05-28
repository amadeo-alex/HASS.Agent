using HASS.Agent.Base.Contracts.Models.Entity;
using HASS.Agent.Base.Models.Entity;
using HASS.Agent.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.UI.Models;

namespace HASS.Agent.UI.Contracts.Managers;

public interface IEntityUiTypeRegistry
{
    Dictionary<string, RegisteredUiEntity> SensorTypes { get; }
    Dictionary<string, RegisteredUiEntity> CommandTypes { get; }

    void RegisterSensorUiType(RegisteredEntity registeredEntity);
    void RegisterCommandUiType(RegisteredEntity registeredEntity);
    object CreateSensorUiInstance(RegisteredUiEntity registeredUiEntity);
    object CreateCommandUiInstance(RegisteredUiEntity registeredUiEntity);
}