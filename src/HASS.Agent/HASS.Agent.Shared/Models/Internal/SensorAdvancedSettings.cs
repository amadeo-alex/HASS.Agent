using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Shared.Models.Internal;
public class SensorAdvancedSettings
{
    public string DeviceClass { get; set; } = string.Empty;
    public string UnitOfMeasurement { get; set; } = string.Empty;
    public string StateClass { get; set; } = string.Empty;
}
