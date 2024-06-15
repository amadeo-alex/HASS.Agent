using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CommunityToolkit.Mvvm.Input;
using HASS.Agent.UI.Contracts.Services;
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

namespace HASS.Agent.UI.Views;
public sealed partial class TrayIconView : UserControl
{
    private readonly IActivationService _activationService;

    [RelayCommand]
    public void TrayIconLeftClick()
    {

    }

    [RelayCommand]
    public void TrayIconDoubleClick()
    {
        var window = App.MainWindow;
        if (window == null)
            return;

        if (window.Visible)
            window.Hide();
        else
            window.Show();
    }

    [RelayCommand]
    public void TrayIconExit()
    {
        _activationService.HandleClosedEvents = false;
        TrayIcon.Dispose();
        App.MainWindow?.Close();
    }

    public TrayIconView()
    {
        _activationService = App.GetService<IActivationService>();
        this.InitializeComponent();

        var flyout = new MenuFlyout();
        flyout.Items.Add(new MenuFlyoutItem() { Text = "Item1", Command = TrayIconDoubleClickCommand } );
        flyout.Items.Add(new MenuFlyoutItem() { Text = "Item2" });
        flyout.Items.Add(new MenuFlyoutItem() { Text = "Item3" });
        flyout.Items.Add(new MenuFlyoutSeparator());
        flyout.Items.Add(new MenuFlyoutItem() { Text = "Exit", Command = TrayIconExitCommand });
        TrayIcon.ContextFlyout = flyout;
    }
}
