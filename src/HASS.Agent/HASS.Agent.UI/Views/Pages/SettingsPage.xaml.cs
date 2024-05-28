// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
public sealed partial class SettingsPage : Page
{
    public SettingsPageViewModel ViewModel
    {
        get;
    }

    public SettingsPage()
    {
        ViewModel = App.GetService<SettingsPageViewModel>();
        this.InitializeComponent();
        DataContext = ViewModel;
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        var d = new ContentDialog();
        d.XamlRoot = this.XamlRoot;
        d.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        d.Title = "Save your work?";
        d.PrimaryButtonText = "Save";
        d.SecondaryButtonText = "Don't Save";
        d.CloseButtonText = "Cancel";
        d.DefaultButton = ContentDialogButton.Primary;
        //d.Content = new SensorDetailDialogContent();

        await d.ShowAsync();
    }
}

public class SomeItem
{
    public string Name { get; set; } = string.Empty;
    public List<SomeItem> Children { get; set; } = new List<SomeItem>();
}
