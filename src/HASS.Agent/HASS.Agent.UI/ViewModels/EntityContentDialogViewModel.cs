using CommunityToolkit.Mvvm.ComponentModel;
using HASS.Agent.Base.Models.Entity;
using HASS.Agent.Base.Models;
using HASS.Agent.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUI3Localizer;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.UI.Contracts.Managers;

namespace HASS.Agent.UI.ViewModels;
public partial class EntityContentDialogViewModel : ObservableObject
{
    private ILocalizer _localizer;
    private IEntityUiTypeRegistry _entityUiTypeRegistry;
    private IEntityTypeRegistry _entityTypeRegistry;
    private ISettingsManager _settingsManager;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowSensorCategories))]
    public ConfiguredEntity entity;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayName))]
    [NotifyPropertyChangedFor(nameof(Description))]
    [NotifyPropertyChangedFor(nameof(AdditionalSettingsPresent))]
    public RegisteredUiEntity uiEntity;

    public string DisplayName => _localizer.GetLocalizedString(UiEntity.DisplayNameResourceKey);
    public string Description => _localizer.GetLocalizedString(UiEntity.DescriptionResourceKey);

    public List<EntityCategory>? SensorsCategories { get; set; }
    public bool ShowSensorCategories => string.IsNullOrWhiteSpace(Entity.Type);
    public bool AdditionalSettingsPresent => UiEntity.AdditionalSettingsUiType != null;

    [ObservableProperty]
    public EntityCategory? selectedItem;

    [ObservableProperty]
    public bool entityIdNameInvalid;

    [ObservableProperty]
    public bool entityNameInvalid;
    public EntityContentDialogViewModel(IEntityUiTypeRegistry entityUiTypeRegistry, IEntityTypeRegistry entityTypeRegistry, ISettingsManager settingsManager, ConfiguredEntity entity)
    {
        _localizer = Localizer.Get();
        _entityUiTypeRegistry = entityUiTypeRegistry;
        _entityTypeRegistry = entityTypeRegistry;
        _settingsManager = settingsManager;

        Entity = entity;
        UiEntity = _entityUiTypeRegistry.SensorUiTypes.TryGetValue(entity.Type, out var uiEntity)
            ? uiEntity
            : new RegisteredUiEntity();
    }

    partial void OnSelectedItemChanged(EntityCategory? value)
    {
        if (value == null || !value.EntityType)
            return;

        var entity = Entity;
        entity.Type = value.Name;
        Entity = entity;
        UiEntity = _entityUiTypeRegistry.SensorUiTypes.TryGetValue(entity.Type, out var uiEntity)
            ? uiEntity
            : new RegisteredUiEntity();
    }

    public void ReevaluateInput()
    {
        var existingEntity = _settingsManager.ConfiguredSensors.FirstOrDefault(cs => cs.UniqueId == Entity.UniqueId);
        if (existingEntity == null)
        {
            EntityIdNameInvalid = _settingsManager.ConfiguredSensors.FirstOrDefault(cs => cs.EntityIdName == Entity.EntityIdName) != null;
            EntityNameInvalid = _settingsManager.ConfiguredSensors.FirstOrDefault(cs => cs.Name == Entity.Name) != null;
        }
        else
        {
            EntityIdNameInvalid = _settingsManager.ConfiguredSensors.FirstOrDefault(cs =>
                cs.UniqueId != existingEntity.UniqueId &&
                cs.EntityIdName == Entity.EntityIdName) != null;

            EntityNameInvalid = _settingsManager.ConfiguredSensors.FirstOrDefault(
                cs => cs.UniqueId != existingEntity.UniqueId &&
                cs.Name == Entity.Name) != null;
        }

        //EntityIdNameInvalid = Entity.EntityIdName.Contains("asd");
        /*        EntityIdNameInvalid = _settingsManager.ConfiguredSensors.FirstOrDefault(
                    cs => cs.EntityIdName == Entity.EntityIdName
                    ) != null;
                EntityNameInvalid = _settingsManager.ConfiguredSensors.FirstOrDefault(cs => cs.Name == Entity.Name) != null;*/
        //EntityNameInvalid = Entity.Name.Contains("123");

        OnPropertyChanged(nameof(EntityIdNameInvalid));
        OnPropertyChanged(nameof(EntityNameInvalid));
    }
}
