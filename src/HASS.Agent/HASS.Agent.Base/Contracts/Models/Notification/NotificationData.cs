using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Base.Contracts.Models.Notification;
public class NotificationData
{
    public int Duration { get; set; } = 8;
    public string Image { get; set; } = string.Empty;

    public List<NotificationAction> Actions { get; set; } = [];

    public List<NotificationInput> Inputs { get; set; } = [];
}
