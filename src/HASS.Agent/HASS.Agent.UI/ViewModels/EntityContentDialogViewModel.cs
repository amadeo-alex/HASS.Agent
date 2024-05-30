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

    [ObservableProperty]
    public ConfiguredEntity entity;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayName))]
    [NotifyPropertyChangedFor(nameof(Description))]
    public RegisteredUiEntity uiEntity;

    public string DisplayName => _localizer.GetLocalizedString(UiEntity.DisplayNameResourceKey);
    public string Description => _localizer.GetLocalizedString(UiEntity.DescriptionResourceKey);

    public List<EntityCategory>? SensorsCategories { get; set; }
    public bool ShowSensorCategories => string.IsNullOrWhiteSpace(Entity.Type);

    [ObservableProperty]
    public EntityCategory? selectedItem;

    public EntityContentDialogViewModel(IEntityUiTypeRegistry entityUiTypeRegistry, IEntityTypeRegistry entityTypeRegistry, ConfiguredEntity entity)
    {
        _localizer = Localizer.Get();
        _entityUiTypeRegistry = entityUiTypeRegistry;
        _entityTypeRegistry = entityTypeRegistry;

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
}
