// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.Base.Models;
using HASS.Agent.UI.Contracts.Managers;
using HASS.Agent.UI.Contracts.ViewModels;
using HASS.Agent.UI.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUI3Localizer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HASS.Agent.UI.Views.Pages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SensorsPage : Page
{
    private readonly IEntityUiTypeRegistry _entityUiTypeRegistry;
    private readonly IEntityTypeRegistry _entityTypeRegistry;
    private readonly ISettingsManager _settingsManager;
    private readonly SensorsPageViewModel _viewModel;

    private bool _dialogShown = false;

    public SensorsPage()
    {
        Debug.WriteLine("page constructor");

        _entityUiTypeRegistry = App.GetService<IEntityUiTypeRegistry>();
        _entityTypeRegistry = App.GetService<IEntityTypeRegistry>();
        _settingsManager = App.GetService<ISettingsManager>();

        _viewModel = App.GetService<SensorsPageViewModel>();
        DataContext = _viewModel;

        this.InitializeComponent();
    }

    private async void ViewModel_NewSensorEventHandler(object? sender, ConfiguredEntity entity)
    {
        if (_dialogShown)
            return;

        _dialogShown = true;

        var dialog = _entityUiTypeRegistry.CreateSensorUiInstance(this, entity);
        if (dialog.ViewModel != null)
            dialog.ViewModel.SensorsCategories = _entityTypeRegistry.SensorsCategories.SubCategories;

        var result = await dialog.ShowAsync();

        _dialogShown = false;

        if (result == ContentDialogResult.Primary && dialog.NewConfiguredEntity != null)
            _viewModel.AddUpdateConfiguredSensor(dialog.NewConfiguredEntity);
    }

    private async void ViewModel_SensorEditEventHandler(object? sender, ConfiguredEntity entity)
    {
        if (_dialogShown)
            return;

        _dialogShown = true;

        var dialog = _entityUiTypeRegistry.CreateSensorUiInstance(this, entity);
        var result = await dialog.ShowAsync();

        _dialogShown = false;

        if (result == ContentDialogResult.Primary && dialog.NewConfiguredEntity != null)
            _viewModel.AddUpdateConfiguredSensor(dialog.NewConfiguredEntity);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);

        if (_viewModel != null)
        {
            _viewModel.SensorEditEventHandler -= ViewModel_SensorEditEventHandler;
            _viewModel.NewSensorEventHandler -= ViewModel_NewSensorEventHandler;
        }

        SensorsListView.ItemsSource = null;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        _viewModel.SensorEditEventHandler += ViewModel_SensorEditEventHandler;
        _viewModel.NewSensorEventHandler += ViewModel_NewSensorEventHandler;
    }

    ~SensorsPage()
    {
        Debug.WriteLine("page destructor");
    }
}
