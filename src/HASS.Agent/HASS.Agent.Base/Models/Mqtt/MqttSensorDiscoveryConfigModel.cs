using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Models.Mqtt;
using Newtonsoft.Json;

namespace HASS.Agent.Base.Models.Mqtt;
public class MqttSensorDiscoveryConfigModel : AbstractMqttDiscoveryConfigModel
{
    /// <summary>
    /// (Optional) Defines the number of seconds after the sensor’s state expires, if it’s not updated. After expiry, the sensor’s state becomes unavailable. Defaults to 0 in hass.
    /// </summary>
    /// <value></value>
    [JsonProperty("expire_after")]
    public int ExpireAfter { get; set; }

    /// <summary>
    /// (Optional) Defines the units of measurement of the sensor, if any.
    /// </summary>
    /// <value></value>
    [DefaultValue("")]
    [JsonProperty("unit_of_measurement")]
    public string UnitOfMeasurement { get; set; } = string.Empty;

    /// <summary>
    /// (Optional) Defines a template to extract the value.
    /// </summary>
    /// <value></value>
    [DefaultValue("")]
    [JsonProperty("value_template")]
    public string ValueTemplate { get; set; } = string.Empty;
}
