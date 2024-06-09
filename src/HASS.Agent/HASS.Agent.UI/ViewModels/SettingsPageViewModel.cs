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
using HASS.Agent.UI.Contracts.ViewModels;
using HASS.Agent.UI.Views.Dialogs;
using HASS.Agent.UI.Views.Pages;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace HASS.Agent.UI.ViewModels;
public partial class SettingsPageViewModel : ViewModelBase
{
    public BindingList<IMenuItem> MenuItems { get; set; }

    public SettingsPageViewModel(DispatcherQueue dispatcherQueue) : base(dispatcherQueue)
    {
        MenuItems = new BindingList<IMenuItem>()
        {
            new MenuItem { NavigateTo = "main", ViewModelType = typeof(MainPageViewModel), Title = "General", Glyph = "\uF8B0" }
        };
    }
}
