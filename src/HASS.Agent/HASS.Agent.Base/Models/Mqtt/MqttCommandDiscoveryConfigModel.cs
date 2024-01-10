using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Models.Mqtt;
using Newtonsoft.Json;

namespace HASS.Agent.Base.Models.Mqtt;
public class MqttCommandDiscoveryConfigModel : AbstractMqttDiscoveryConfigModel
{
    /// <summary>
    /// (Optional) The MQTT topic to set the command
    /// </summary>
    /// <value></value>
    [DefaultValue("")]
    [JsonProperty("Command_topic")]
    public string CommandTopic { get; set; } = string.Empty;

    /// <summary>
    /// (Optional) The MQTT topic to set the action
    /// </summary>
    /// <value></value>
    [DefaultValue("")]
    [JsonProperty("Action_topic")]
    public string ActionTopic { get; set; } = string.Empty;

    /// <summary>
    /// (Optional) Defines a template to extract the value.
    /// </summary>
    /// <value></value>
    [DefaultValue("")]
    [JsonProperty("Value_template")]
    public string ValueTemplate { get; set; } = string.Empty;
}
