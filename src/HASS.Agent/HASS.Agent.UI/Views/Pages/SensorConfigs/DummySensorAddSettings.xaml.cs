using HASS.Agent.Base.Models;
using HASS.Agent.Base.Sensors.SingleValue;
using HASS.Agent.UI.ViewModels.SensorConfigs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HASS.Agent.UI.Views.Pages.SensorConfigs;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class DummySensorAddSettings : Page
{
    private readonly DummySensorAddSettingsViewModel _viewModel;

    public DummySensorAddSettings(ConfiguredEntity entity)
    {
        _viewModel = new DummySensorAddSettingsViewModel(entity);
        DataContext = _viewModel;

        this.InitializeComponent();
    }
}
