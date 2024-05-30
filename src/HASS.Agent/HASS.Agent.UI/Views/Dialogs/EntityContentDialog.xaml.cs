using HASS.Agent.Base.Models;
using HASS.Agent.Base.Models.Entity;
using HASS.Agent.UI.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUI3Localizer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HASS.Agent.UI.Views.Dialogs;
public sealed partial class EntityContentDialog : ContentDialog
{
    private ILocalizer _localizer;

    public ConfiguredEntity Entity { get; set; }
    public RegisteredUiEntity UiEntity { get; set; }
    public string DisplayName => _localizer.GetLocalizedString(UiEntity.DisplayNameResourceKey);
    public string Description => _localizer.GetLocalizedString(UiEntity.DescriptionResourceKey);
    public object? AdditionalSettings { get; set; }
    public bool AdditionalSettingsPresent => AdditionalSettings != null;

    public List<EntityCategory>? SensorsCategories { get; set; }
    public bool ShowSensorCategories => string.IsNullOrWhiteSpace(Entity.Type);

    public EntityContentDialog(Control parentControl, ConfiguredEntity entity, RegisteredUiEntity uiEntity)
    {
        _localizer = Localizer.Get();

        Entity = entity;
        UiEntity = uiEntity;

        this.InitializeComponent();

        XamlRoot = parentControl.XamlRoot;
        Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;

        var titleResourceKey = string.IsNullOrWhiteSpace(entity.Type) ? "Dialog_SensorDetail_NewSensor" : "Dialog_SensorDetail_EditSensor";
        Title = _localizer.GetLocalizedString(titleResourceKey);
        var saveButtonResourceKey = string.IsNullOrWhiteSpace(entity.Type) ? "Dialog_SensorDetail_Add" : "Dialog_SensorDetail_Save";
        PrimaryButtonText = _localizer.GetLocalizedString(saveButtonResourceKey);
        CloseButtonText = _localizer.GetLocalizedString("Dialog_SensorDetail_Cancel");

        DefaultButton = ContentDialogButton.Primary;
        Resources["ContentDialogMaxWidth"] = 1080;
    }
}
