// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using HASS.Agent.Base.Models;
using HASS.Agent.UI.ViewModels;
using HASS.Agent.UI.Views.Pages.Dialogs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HASS.Agent.UI.Views.Pages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SensorsPage : Page
{
    public SensorsPageViewModel ViewModel
    {
        get;
    }

    public SensorsPage()
    {
        ViewModel = App.GetService<SensorsPageViewModel>();
        ViewModel.SensorEditEventHandler += ViewModel_SensorEditEventHandler;
        ViewModel.NewSensorEventHandler += ViewModel_NewSensorEventHandler;
        this.InitializeComponent();
    }

    private async void ViewModel_NewSensorEventHandler(object? sender, ConfiguredEntity entity)
    {
        var dialogContent = new SensorDetailDialogContent(entity);
        dialogContent.CustomDetails = new Button { Content = "asdasdds123123" };
        dialogContent.SensorsCategories = ViewModel.SensorsCategories;

        var dialog = new ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = "New sensor",
            PrimaryButtonText = "Add",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            Content = dialogContent
        };
        dialog.Resources["ContentDialogMaxWidth"] = 1080;

        await dialog.ShowAsync();
    }

    private async void ViewModel_SensorEditEventHandler(object? sender, ConfiguredEntity entity)
    {
        var dialogContent = new SensorDetailDialogContent(entity);
        dialogContent.CustomDetails = new Button { Content = "123123123" };
        var d = new ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = "Edit sensor",
            PrimaryButtonText = "Save",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            Content = dialogContent
        };

        await d.ShowAsync();
    }
}
