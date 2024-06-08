using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HASS.Agent.UI.Models.Notifications;
public class NotificationData
{
    public const string NoAction = "noAction";
    public const string ImportanceHigh = "high";

    public int Duration { get; set; } = 0;
    public string Image { get; set; } = string.Empty;
    public string ClickAction { get; set; } = NoAction;
    public string Tag { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    [JsonProperty("icon_url")]
    public string IconUrl { get; set; } = string.Empty;
    public bool Sticky { get; set; }
    public string Importance { get; set; } = string.Empty;

    public List<NotificationAction> Actions { get; set; } = [];

    public List<NotificationInput> Inputs { get; set; } = [];
}
