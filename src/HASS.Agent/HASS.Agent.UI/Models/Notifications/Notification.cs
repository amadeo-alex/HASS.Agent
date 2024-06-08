using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.UI.Models.Notifications;
public class Notification
{
    public string Message { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;

    public int Duration { get; set; } = 8;

    public List<NotificationAction> Actions { get; set; } = [];

    public List<NotificationInput> Inputs { get; set; } = [];
}