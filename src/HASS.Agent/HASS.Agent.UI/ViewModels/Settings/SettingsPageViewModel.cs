using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.Base.Models;
using HASS.Agent.Base.Sensors.SingleValue;
using HASS.Agent.UI.Contracts.Services;
using HASS.Agent.UI.Contracts.ViewModels;
using HASS.Agent.UI.Services;
using HASS.Agent.UI.ViewModels.Settings;
using HASS.Agent.UI.Views.Dialogs;
using HASS.Agent.UI.Views.Pages;
using HASS.Agent.UI.Views.Pages.Settings;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace HASS.Agent.UI.ViewModels;
public partial class SettingsPageViewModel : ViewModelBase
{
    private readonly IPageService _pageService;

    [ObservableProperty]
    private MenuItem? selected;

    public INavigationService NavigationService { get; }
    public INavigationViewService NavigationViewService { get; }

    public BindingList<IMenuItem> MenuItems => _pageService.Pages;

    public SettingsPageViewModel(DispatcherQueue dispatcherQueue) : base(dispatcherQueue)
    {
        var menuPages = new Dictionary<IMenuItem, Type?>()
        {
            { new MenuItem { NavigateTo = "settingsGeneral", ViewModelType = typeof(GeneralSettingsPageViewModel), Title = "General"}, typeof(GeneralSettingsPage) },
            { new MenuItem { NavigateTo = "settingsMqtt", ViewModelType = typeof(MqttSettingsPageViewModel), Title = "MQTT"}, typeof(MqttSettingsPage) },
            { new MenuItem { NavigateTo = "settingsNotification", ViewModelType = typeof(NotificationSettingsPageViewModel), Title = "Notification"}, typeof(NotificationSettingsPage) },

        };

        _pageService = new PageService(menuPages, null);

        NavigationService = new NavigationService(_pageService);
        NavigationViewService = new NavigationViewService(NavigationService, _pageService);
    }
}
