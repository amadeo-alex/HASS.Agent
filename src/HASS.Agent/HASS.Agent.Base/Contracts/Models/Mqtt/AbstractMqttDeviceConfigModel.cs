using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HASS.Agent.Base.Contracts.Models.Mqtt;

/// <summary>
/// This information will be used when announcing this device on the mqtt topic
/// </summary>
public abstract class AbstractMqttDeviceConfigModel
{
    /// <summary>
    /// (Optional) A list of connections of the device to the outside world as a list of tuples [connection_type, connection_identifier]. For example the MAC address of a network interface: "connections": [["mac", "02:5b:26:a8:dc:12"]].
    /// </summary>
    /// <value></value>
    public ICollection<Tuple<string, string>>? Connections { get; set; }

    /// <summary>
    /// (Optional) An Id to identify the device. For example a serial number.
    /// </summary>
    /// <value></value>
    [DefaultValue("")]
    public string Identifiers { get; set; } = string.Empty;

    /// <summary>
    /// (Optional) The manufacturer of the device.
    /// </summary>
    /// <value></value>
    [DefaultValue("")]
    public string Manufacturer { get; set; } = string.Empty;

    /// <summary>
    /// (Optional) The model of the device.
    /// </summary>
    /// <value></value>
    [DefaultValue("")]
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// (Optional) The name of the device.
    /// </summary>
    /// <value></value>
    [DefaultValue("")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// (Optional) The firmware version of the device.
    /// </summary>
    /// <value></value>
    [DefaultValue("")]
    [JsonProperty("sw_version")]
    public string SoftwareVersion { get; set; } = string.Empty;

    /// <summary>
    /// (Optional) Identifier of a device that routes messages between this device and Home Assistant. Examples of such devices are hubs, or parent devices of a sub-device. This is used to show device topology in Home Assistant.
    /// </summary>
    /// <value></value>
    [DefaultValue("")]
    [JsonProperty("via_device")]
    public string ViaDevice { get; set; } = string.Empty;
}
