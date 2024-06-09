using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Managers;
using Microsoft.UI.Xaml;

namespace HASS.Agent.UI.ViewModels.Settings;
public class GeneralSettingsPageViewModel
{
    private readonly ISettingsManager _settingsManager;

    public string ConfiguredDeviceName
    {
        get => _settingsManager.Settings.Application.ConfiguredDeviceName;
        set => _settingsManager.Settings.Application.ConfiguredDeviceName = value;
    }
    public string DeviceName => _settingsManager.Settings.Application.DeviceName;
    public bool SanitizeName
    {
        get => _settingsManager.Settings.Application.SanitizeName;
        set => _settingsManager.Settings.Application.SanitizeName = value;
    }
    public string Language
    {
        get => _settingsManager.Settings.Application.InterfaceLanguage;
        set => _settingsManager.Settings.Application.InterfaceLanguage = value;
    }
    public string Theme
    {
        get => _settingsManager.Settings.Application.Theme;
        set => _settingsManager.Settings.Application.Theme = value;
    }

    public List<string> Themes { get; } = ["Default", "Light", "Dark"];

    public GeneralSettingsPageViewModel(ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }
}
