using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.Base.Contracts.Models.Entity;
using HASS.Agent.Base.Models;
using HASS.Agent.Base.Models.Entity;
using HASS.Agent.Base.Sensors.SingleValue;
using HASS.Agent.UI.Contracts.Managers;
using HASS.Agent.UI.Models;
using HASS.Agent.UI.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.WindowsAppSDK.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUI3Localizer;

namespace HASS.Agent.UI.Managers;
public class EntityUiTypeRegistry : IEntityUiTypeRegistry
{
    private IServiceProvider _serviceProvider;
    private IEntityTypeRegistry _entityTypeRegistry;
    public Dictionary<string, RegisteredUiEntity> SensorUiTypes { get; } = [];

    public Dictionary<string, RegisteredUiEntity> CommandUiTypes { get; } = [];

    public EntityUiTypeRegistry(IServiceProvider serviceProvider, IEntityTypeRegistry entityTypeRegistry)
    {
        _serviceProvider = serviceProvider;
        _entityTypeRegistry = entityTypeRegistry;

        RegisterSensorUiType(typeof(DummySensor), null, $"Sensor_{typeof(DummySensor).Name}_DisplayName", $"Sensor_{typeof(DummySensor).Name}_Description");
    }

    public void RegisterSensorUiType(Type sensorType, Type? additionalSettingsUiType, string displayNameResourceKey, string descriptionResourceKey)
    {
        if (!sensorType.IsAssignableTo(typeof(IDiscoverable)))
            throw new ArgumentException($"{sensorType} is not derived from {nameof(IDiscoverable)}");

        var typeName = sensorType.Name;

        if (SensorUiTypes.ContainsKey(typeName))
            throw new ArgumentException($"ui for sensor {typeName} already registered");

        SensorUiTypes[typeName] = new RegisteredUiEntity
        {
            EntityType = sensorType,
            AdditionalSettingsUiType = additionalSettingsUiType,
            DisplayNameResourceKey = displayNameResourceKey,
            DescriptionResourceKey = descriptionResourceKey
        };
    }

    public void RegisterCommandUiType(RegisteredEntity registeredEntity, Type? additionalSettingsUiType, string displayNameResourceKey, string descriptionResourceKey)
    {
        /*        var typeName = registeredEntity.EntityType.Name;

                if (CommandUiTypes.ContainsKey(typeName))
                    throw new ArgumentException($"ui for command {typeName} already registered");

                CommandUiTypes[typeName] = new RegisteredUiEntity
                {
                    Entity = registeredEntity,
                    AdditionalSettingsUiType = additionalSettingsUiType,
                    DisplayNameResourceKey = displayNameResourceKey,
                    DescriptionResourceKey = descriptionResourceKey
                };*/
    }

    public EntityContentDialog CreateSensorUiInstance(Control control, ConfiguredEntity entity)
    {
        var registeredUiEntity = SensorUiTypes.TryGetValue(entity.Type, out var uiEntity)
            ? uiEntity
            : new RegisteredUiEntity();

        var dialog = new EntityContentDialog(control, entity, registeredUiEntity);
        if (registeredUiEntity.AdditionalSettingsUiType != null)
        {
            var additionalSettingsUi = ActivatorUtilities.CreateInstance(_serviceProvider, registeredUiEntity.AdditionalSettingsUiType);
            dialog.AdditionalSettings = additionalSettingsUi;
        }

        return dialog;
    }

    public EntityContentDialog CreateCommandUiInstance(Control control, ConfiguredEntity entity)
    {
        var registeredUiEntity = CommandUiTypes[entity.Type];
        var localizer = Localizer.Get();

        var dialog = new ContentDialog
        {
            XamlRoot = control.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = localizer.GetLocalizedString("Dialog_SensorDetail_NewSensor"),
            PrimaryButtonText = localizer.GetLocalizedString("Dialog_SensorDetail_Add"),
            CloseButtonText = localizer.GetLocalizedString("Dialog_SensorDetail_Cancel"),
            DefaultButton = ContentDialogButton.Primary
        };
        dialog.Resources["ContentDialogMaxWidth"] = 1080;

        if (registeredUiEntity.AdditionalSettingsUiType != null)
        {

        }

        return new EntityContentDialog(control, new ConfiguredEntity(), new RegisteredUiEntity());
    }
}
