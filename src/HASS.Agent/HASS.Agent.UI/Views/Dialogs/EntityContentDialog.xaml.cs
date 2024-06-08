using CommunityToolkit.Mvvm.ComponentModel;
using HASS.Agent.Base.Models;
using HASS.Agent.Base.Models.Entity;
using HASS.Agent.UI.Models;
using HASS.Agent.UI.ViewModels;
using HASS.Agent.UI.Views.Pages.SensorConfigs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUI3Localizer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HASS.Agent.UI.Views.Dialogs;

[INotifyPropertyChanged]
public sealed partial class EntityContentDialog : ContentDialog
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILocalizer _localizer;
    private readonly EntityContentDialogViewModel _viewModel;

    public EntityContentDialogViewModel ViewModel => _viewModel;
    public ConfiguredEntity? NewConfiguredEntity { get; private set; }

    [ObservableProperty]
    public object? additionalSettings;

    public EntityContentDialog(IServiceProvider serviceProvider, Control parentControl, EntityContentDialogViewModel viewModel)
    {
        Debug.WriteLine("ec page constructor");

        _serviceProvider = serviceProvider;
        _localizer = Localizer.Get();

        _viewModel = viewModel;
        DataContext = _viewModel;

        this.InitializeComponent();

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;

        XamlRoot = parentControl.XamlRoot;
        Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;

        var titleResourceKey = string.IsNullOrWhiteSpace(viewModel.Entity.Type) ? "Dialog_SensorDetail_NewSensor" : "Dialog_SensorDetail_EditSensor";
        Title = _localizer.GetLocalizedString(titleResourceKey);
        var saveButtonResourceKey = string.IsNullOrWhiteSpace(viewModel.Entity.Type) ? "Dialog_SensorDetail_Add" : "Dialog_SensorDetail_Save";
        PrimaryButtonText = _localizer.GetLocalizedString(saveButtonResourceKey);
        CloseButtonText = _localizer.GetLocalizedString("Dialog_SensorDetail_Cancel");

        DefaultButton = ContentDialogButton.Primary;
        Resources["ContentDialogMaxWidth"] = 1080;

        Closed += EntityContentDialog_Closed;
    }

    private void EntityContentDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        NewConfiguredEntity = _viewModel.Entity;

        //DataContext = null;
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(EntityContentDialogViewModel.UiEntity)
            && _viewModel.UiEntity.AdditionalSettingsUiType != null)
        {
            AdditionalSettings = ActivatorUtilities.CreateInstance(_serviceProvider, _viewModel.UiEntity.AdditionalSettingsUiType, ViewModel.Entity);
        }
    }

    private async void EntityContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        _viewModel.ReevaluateInput();

        if (_viewModel.IsNewSensor)
        {
            if (_viewModel.EntityIdNameInvalid || _viewModel.EntityNameInvalid)
                args.Cancel = true;
        }
    }

    ~EntityContentDialog()
    {
        Debug.WriteLine("ec page destructor");
    }
}
