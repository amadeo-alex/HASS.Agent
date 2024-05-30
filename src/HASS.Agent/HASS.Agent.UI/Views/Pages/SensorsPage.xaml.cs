// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
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
    private IEntityUiTypeRegistry _entityUiTypeRegistry;
    private IEntityTypeRegistry _entityTypeRegistry;

    private bool _dialogShown = false;
    public SensorsPageViewModel ViewModel
    {
        get;
    }

    public SensorsPage()
    {
        _entityUiTypeRegistry = App.GetService<IEntityUiTypeRegistry>();
        _entityTypeRegistry = App.GetService<IEntityTypeRegistry>();

        ViewModel = App.GetService<SensorsPageViewModel>();
        ViewModel.SensorEditEventHandler += ViewModel_SensorEditEventHandler;
        ViewModel.NewSensorEventHandler += ViewModel_NewSensorEventHandler;
        this.InitializeComponent();
    }

    private async void ViewModel_NewSensorEventHandler(object? sender, ConfiguredEntity entity)
    {
        if (_dialogShown)
            return;

        _dialogShown = true;

        var dialog = _entityUiTypeRegistry.CreateSensorUiInstance(this, entity);
        dialog.ViewModel.SensorsCategories = _entityTypeRegistry.SensorsCategories.SubCategories;
        await dialog.ShowAsync();

        _dialogShown = false;
    }

    private async void ViewModel_SensorEditEventHandler(object? sender, ConfiguredEntity entity)
    {
        if (_dialogShown)
            return;

        _dialogShown = true;

        var dialog = _entityUiTypeRegistry.CreateSensorUiInstance(this, entity);
        await dialog.ShowAsync();

        _dialogShown = false;
    }
}
