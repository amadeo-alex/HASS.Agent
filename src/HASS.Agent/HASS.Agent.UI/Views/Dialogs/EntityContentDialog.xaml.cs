using CommunityToolkit.Mvvm.ComponentModel;
using HASS.Agent.Base.Models;
using HASS.Agent.Base.Models.Entity;
using HASS.Agent.UI.Contracts.Views;
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
using Microsoft.WindowsAppSDK.Runtime;
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

[INotifyPropertyChanged]
public sealed partial class EntityContentDialog : ContentDialog
{
    private IServiceProvider _serviceProvider;
    private ILocalizer _localizer;

    public EntityContentDialogViewModel? ViewModel { get; set; }
    public ConfiguredEntity? NewConfiguredEntity { get; private set; }

    [ObservableProperty]
    public object? additionalSettings;

    public EntityContentDialog(IServiceProvider serviceProvider, Control parentControl, EntityContentDialogViewModel viewModel)
    {
        _serviceProvider = serviceProvider;
        _localizer = Localizer.Get();

        ViewModel = viewModel;
        DataContext = viewModel;

        this.InitializeComponent();

        DataContext = viewModel;
        if (ViewModel != null)
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

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
        (AdditionalSettings as IAdditionalSettingsPage)?.Cleanup();
        //Bindings.StopTracking();

        if (ViewModel != null)
        {
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            NewConfiguredEntity = ViewModel.Entity;
        }

        DataContext = null;
        ViewModel = null;
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(EntityContentDialogViewModel.UiEntity)
            && ViewModel?.UiEntity.AdditionalSettingsUiType != null)
        {
            AdditionalSettings = ActivatorUtilities.CreateInstance(_serviceProvider, ViewModel.UiEntity.AdditionalSettingsUiType, ViewModel.Entity);
        }
    }

    private async void EntityContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (ViewModel == null)
            return;

        ViewModel.ReevaluateInput();

        if (ViewModel.ShowSensorCategories)
        {
            if (ViewModel.EntityIdNameInvalid || ViewModel.EntityNameInvalid)
                args.Cancel = true;
        }
    }

    ~EntityContentDialog()
    {
        Console.WriteLine();
    }
}
