using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.UI.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace HASS.Agent.UI.Contracts.Services;
public interface INavigationViewService
{
    IList<object>? MenuItems
    {
        get;
    }

    object? SettingsItem
    {
        get;
    }

    void Initialize(NavigationView navigationView);

    void UnregisterEvents();

    MenuItem? GetSelectedItem(Type pageType);
}
