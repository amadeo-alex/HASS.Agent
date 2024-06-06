using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Models;
using HASS.Agent.Base.Sensors.SingleValue;
using Microsoft.WindowsAppSDK.Runtime;

namespace HASS.Agent.UI.ViewModels.SensorConfigs;
public class DummySensorAddSettingsViewModel
{
    private readonly ConfiguredEntity _entity;

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

    public int MaxRetries
    {
        get => _entity.GetIntParameter(DummySensor.MaxRetriesKey, 100);
        set => _entity.SetIntParameter(DummySensor.MaxRetriesKey, value);
    }

    public DummySensorAddSettingsViewModel(ConfiguredEntity entity)
    {
        _entity = entity;
    }
}
