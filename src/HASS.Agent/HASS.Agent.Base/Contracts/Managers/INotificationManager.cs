using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Models.Notification;

namespace HASS.Agent.Base.Contracts.Managers;
public interface INotificationManager
{
    void HandleReceivedNotification(Notification notification);
}
