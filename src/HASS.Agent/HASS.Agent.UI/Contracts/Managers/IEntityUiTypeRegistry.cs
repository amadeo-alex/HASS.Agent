using HASS.Agent.Base.Contracts.Models.Entity;
using HASS.Agent.Base.Models.Entity;
using HASS.Agent.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.UI.Models;
using Microsoft.UI.Xaml.Controls;
using HASS.Agent.UI.Views.Dialogs;

namespace HASS.Agent.UI.Contracts.Managers;

public interface IEntityUiTypeRegistry
{
    Dictionary<string, RegisteredUiEntity> SensorUiTypes { get; }
    Dictionary<string, RegisteredUiEntity> CommandUiTypes { get; }

    void RegisterSensorUiType(Type sensorType, Type? additionalSettingsUiType, string displayNameResourceKey, string descriptionResourceKey);
    void RegisterCommandUiType(RegisteredEntity registeredEntity, Type? additionalSettingsUiType, string displayNameResourceKey, string descriptionResourceKey);
    EntityContentDialog CreateSensorUiInstance(Control control, ConfiguredEntity entity);
    EntityContentDialog CreateCommandUiInstance(Control control, ConfiguredEntity entity);
}