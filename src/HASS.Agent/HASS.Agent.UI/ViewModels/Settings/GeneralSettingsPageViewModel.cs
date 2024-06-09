using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using HASS.Agent.Base.Contracts.Managers;
using Microsoft.UI.Xaml;

namespace HASS.Agent.UI.ViewModels.Settings;
public class GeneralSettingsPageViewModel : ObservableObject
{
    public ISettingsManager SettingsManager { get; private set; }

    public List<string> Themes { get; } = ["Default", "Light", "Dark"];

    public GeneralSettingsPageViewModel(ISettingsManager settingsManager)
    {
        SettingsManager = settingsManager;
    }
}
