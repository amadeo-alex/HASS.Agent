using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.UI.Contracts.Managers;
using HASS.Agent.UI.Contracts.Services;
using HASS.Agent.UI.Managers;
using HASS.Agent.UI.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;

namespace HASS.Agent.UI.Activation;
public class NotificationActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
{
    private readonly INotificationManager _notificationManager;

    public NotificationActivationHandler(INotificationManager notificationManager)
    {
        _notificationManager = notificationManager;
    }

    protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
    {
        return Environment.GetCommandLineArgs().Contains(NotificationManager.NotificationLaunchArgument);
    }

    protected async override Task HandleInternalAsync(LaunchActivatedEventArgs args)
    {
        var activationArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
        if (activationArgs.Kind != ExtendedActivationKind.AppNotification)
            return;

        await _notificationManager.HandleAppActivation(activationArgs);
    }
}
