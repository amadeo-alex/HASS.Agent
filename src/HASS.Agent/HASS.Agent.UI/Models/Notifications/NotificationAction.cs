using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.UI.Models.Notifications;
public class NotificationAction
{
    public string Action { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}