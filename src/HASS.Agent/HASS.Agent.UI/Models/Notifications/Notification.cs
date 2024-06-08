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
    public NotificationData Data { get; set; } = new NotificationData();
}