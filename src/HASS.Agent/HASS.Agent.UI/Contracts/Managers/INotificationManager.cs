using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.UI.Models.Notifications;
using Microsoft.Windows.AppLifecycle;
using MQTTnet;

namespace HASS.Agent.UI.Contracts.Managers;

public interface INotificationActionHandler
{
    Task HandleNotificationAction(Notification notification);
}

public interface INotificationManager
{
    bool Ready { get; }
    Task Initialize();
    Task ShowNotification(Notification notification, string handlerId);
    void RegisterNotificationActionHandler(string handlerId, INotificationActionHandler handler);
    void UnregisterNotificationActionHandler(string handlerId);
    Task HandleAppActivation(AppActivationArguments activationArguments);
}
