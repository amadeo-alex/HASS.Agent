using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HASS.Agent.Base.Contracts.Models.Mqtt;

/// <summary>
/// Configuration model of all discerable objects
/// </summary>
public abstract class AbstractMqttDiscoveryConfigModel
{
    /// <summary>
    /// (Optional) The type/class of the sensor to set the icon in the frontend. See https://www.home-assistant.io/integrations/sensor/#device-class for options.
    /// </summary>
    /// <value></value>
    [DefaultValue("")]
    [JsonProperty("device_class")]
    public string DeviceClass { get; set; } = string.Empty;

    /// <summary>
    /// (Optional) The MQTT topic subscribed to receive availability (online/offline) updates.
    /// </summary>
    /// <value></value>
    [DefaultValue("")]
    [JsonProperty("availability_topic")]
    public string AvailabilityTopic { get; set; } = string.Empty;

    /// <summary>
    /// (Optional) Information about the device this entity is a part of to tie it into the device registry. Only works through MQTT discovery and when unique_id is set.
    /// </summary>
    /// <value></value>
    public AbstractMqttDeviceConfigModel? Device { get; set; }

    /// <summary>
    /// (Optional) The friendly name of the MQTT entity. Defaults to its name.
    /// </summary>
    /// <value></value>
    [DefaultValue("")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The MQTT topic subscribed to receive entity values.
    /// </summary>
    /// <value></value>
    [DefaultValue("")]
    [JsonProperty("state_topic")]
    public string StateTopic { get; set; } = string.Empty;

    /// <summary>
    /// Sends update events even if the value hasn’t changed. Useful if you want to have meaningful value graphs in history.
    /// </summary>
    /// <value></value>
    [DefaultValue(false)]
    [JsonProperty("force_update")]
    public bool ForceUpdate { get; set; } = false;

    /// <summary>
    /// (Optional) The icon for the entity.
    /// </summary>
    /// <value></value>
    [DefaultValue("")]
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// (Optional) Defines a template to extract the JSON dictionary from messages received on the json_attributes_topic.
    /// </summary>
    /// <value></value>
    [DefaultValue("")]
    [JsonProperty("json_attributes_template")]
    public string JsonAttributesTemplate { get; set; } = string.Empty;

    /// <summary>
    /// (Optional) The MQTT topic subscribed to receive a JSON dictionary payload and then set as entity attributes. Implies force_update of the current sensor state when a message is received on this topic.
    /// </summary>
    /// <value></value>
    [DefaultValue("")]
    [JsonProperty("json_attributes_topic")]
    public string JsonAttributesTopic { get; set; } = string.Empty;

    /// <summary>
    /// (Optional) The payload that represents the available state.
    /// </summary>
    /// <value></value>
    [DefaultValue("")]
    [JsonProperty("payload_available")]
    public string PayloadAvailable { get; set; } = string.Empty;

    /// <summary>
    /// (Optional) The payload that represents the unavailable state.
    /// </summary>
    /// <value></value>
    [DefaultValue("")]
    [JsonProperty("payload_not_available")]
    public string PayloadNotAvailable { get; set; } = string.Empty;

    /// <summary>
    /// (Optional) The maximum QoS level of the state topic.
    /// </summary>
    /// <value></value>
    [DefaultValue(0)]
    public int Qos { get; set; }

    /// <summary>
    /// (Optional) An ID that uniquely identifies this entity. If two entities have the same unique ID, Home Assistant will raise an exception.
    /// </summary>
    /// <value></value>
    [DefaultValue("")]
    [JsonProperty("unique_id")]
    public string UniqueId { get; set; } = string.Empty;

    /// <summary>
    /// (Optional) An ID that will be used by Home Assistant to generate the entity ID.
    /// If not provided, will be generated based on the sensor name and the device name.
    /// If not provided and sensor name already includes the device name, will return the sensor name.
    /// </summary>
    /// <value></value>
    [DefaultValue("")]
    [JsonProperty("object_id")]
    public string ObjectId { get; set; } = string.Empty;
}
