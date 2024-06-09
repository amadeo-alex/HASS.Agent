using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.Base.Models;
using HASS.Agent.UI.Contracts.Services;
using HASS.Agent.UI.Helpers;
using Microsoft.UI.Xaml;

namespace HASS.Agent.UI.Services;
public class ThemeSelectorService : IThemeSelectorService
{
    private readonly ISettingsManager _settingsManager;

    public ThemeSelectorService(ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
        _settingsManager.Settings.Application.PropertyChanged += Application_PropertyChanged;
    }

    private void Application_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ApplicationSettings.Theme))
        {
            var theme = Enum.Parse<ElementTheme>(_settingsManager.Settings.Application.Theme);
            SetThemeAsync(theme);
        }
    }
    public void SetThemeAsync(ElementTheme theme)
    {
        if (App.MainWindow.Content is FrameworkElement rootElement)
        {
            rootElement.RequestedTheme = theme;

            TitleBarHelper.UpdateTitleBar(theme);
        }
    }
}

