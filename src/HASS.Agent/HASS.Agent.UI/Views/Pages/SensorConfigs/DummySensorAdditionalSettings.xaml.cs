using HASS.Agent.Base.Models;
using HASS.Agent.Base.Sensors.SingleValue;
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
public sealed partial class DummySensorAdditionalSettings : Page
{
    private ConfiguredEntity _entity;

    public bool EnsureRandom
    {
        get => _entity.GetBoolParameter(DummySensor.EnsureRandomKey, false);
        set => _entity.SetBoolParameter(DummySensor.EnsureRandomKey, value);
    }

    public int MinValue
    {
        get => _entity.GetIntParameter(DummySensor.MinValueKey, 0);
        set => _entity.SetIntParameter(DummySensor.MinValueKey, value);
    }

    public int MaxValue
    {
        get => _entity.GetIntParameter(DummySensor.MaxValueKey, 100);
        set => _entity.SetIntParameter(DummySensor.MaxValueKey, value);
    }

    public DummySensorAdditionalSettings(ConfiguredEntity entity)
    {
        _entity = entity;

        this.InitializeComponent();
    }
}
