using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using HASS.Agent.UI.Contracts.Services;
using HASS.Agent.UI.Contracts.ViewModels;
using HASS.Agent.UI.ViewModels;
using HASS.Agent.UI.Views.Pages;
using HASS.Agent.UI.Views.Pages.Settings;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Windows.ApplicationModel.VoiceCommands;
using static HASS.Agent.UI.Contracts.Services.IPageService;

namespace HASS.Agent.UI.Services;
public class PageService : IPageService
{
    private readonly Dictionary<string, Type> _pages = new();

    public BindingList<IMenuItem> Pages { get; private set; } = new BindingList<IMenuItem>();
    public BindingList<IMenuItem> FooterPages { get; private set; } = new BindingList<IMenuItem>();

    public PageService()
    {
        //TODO: localization
        //TODO: move to App.xaml.cs for readability?
        var pages = new Dictionary<IMenuItem, Type?>()
        {
            { new MenuItem { NavigateTo = "main", ViewModelType = typeof(MainPageViewModel), Title = "Main", Glyph = "\uE80F" }, typeof(MainPage) },

            { new MenuItem { Type = MenuItemType.Separator }, null},
            { new MenuItem { Type = MenuItemType.Header, Title = "HASS.Agent" }, null },
            { new MenuItem { NavigateTo = "sensors", ViewModelType = typeof(SensorsPageViewModel), Title = "Sensors", Glyph = "\uE957" }, typeof(SensorsPage) },
            { new MenuItem { NavigateTo = "commands", ViewModelType = typeof(CommandsPageViewModel), Title = "Commands", Glyph = "\uE756" }, typeof(CommandsPage) },

            { new MenuItem { Type = MenuItemType.Separator }, null},
            { new MenuItem { Type = MenuItemType.Header, Title = "Satellite Service" }, null },
            { new MenuItem { NavigateTo = "sensors-sat", ViewModelType = typeof(SensorsPageViewModel), Title = "Sensors", Glyph = "\uE957"}, typeof(SatelliteSensorsPage) },
            { new MenuItem { NavigateTo = "commands-sat", ViewModelType = typeof(CommandsPageViewModel), Title = "Commands", Glyph = "\uE756" }, typeof(SatelliteCommandsPage) },
        };
        ConfigurePages(pages);

        var footerPages = new Dictionary<IMenuItem, Type?>()
        {
            { new MenuItem { NavigateTo = "debug", ViewModelType = typeof(DebugPageViewModel), Title = "Debug", Glyph = "\uEBE8" }, typeof(DebugPage) },
            { new MenuItem { NavigateTo = "settings", ViewModelType = typeof(SettingsPageViewModel), Title = "Settings", Glyph = "\uE713" }, typeof(SettingsPage) }
        };
        ConfigureFooterPages(footerPages);
    }

    public IMenuItem GetMenuItem(string navigateTo)
    {
        var menuItem = Pages.FirstOrDefault(item => item.NavigateTo == navigateTo) ?? FooterPages.FirstOrDefault(item => item.NavigateTo == navigateTo);
        return menuItem ?? throw new ArgumentException($"MenuItem with {navigateTo} not found");
    }
    public Type GetPageType(string key)
    {
        Type? pageType;
        lock (_pages)
        {
            if (!_pages.TryGetValue(key, out pageType))
            {
                throw new ArgumentException($"Page not found: {key}. Did you forget to call PageService.Configure?");
            }
        }

        return pageType;
    }

    public void Test()
    {
        foreach (var menuItem in Pages)
        {
            if(menuItem.InfoBadge != null)
            {
                menuItem.InfoBadge.Value++;
            }
            if (menuItem.ViewModelType != null)
            {
                var viewModel = App.GetService(menuItem.ViewModelType);
                if (viewModel != null && viewModel is IInfoBadgeAware badgeAware)
                {
                    menuItem.InfoBadge = badgeAware.InfoBadge;
                }
            }
        }
        foreach (var menuItem in FooterPages)
        {
            if (menuItem.InfoBadge != null)
            {
                menuItem.InfoBadge.Value++;
            }
            if (menuItem.ViewModelType != null)
            {
                var viewModel = App.GetService(menuItem.ViewModelType);
                if (viewModel != null && viewModel is IInfoBadgeAware badgeAware)
                {
                    menuItem.InfoBadge = badgeAware.InfoBadge;
                }
            }
        }
    }

    private void ConfigurePage(Type? pageType, IMenuItem menuItem)
    {
        if (pageType != null)
        {
            if (menuItem.ViewModelType != null)
            {
                var viewModel = App.GetService(menuItem.ViewModelType);
                if (viewModel != null && viewModel is IInfoBadgeAware badgeAware)
                {
                    menuItem.InfoBadge = badgeAware.InfoBadge;
                }
            }

            lock (_pages)
            {
                if (_pages.ContainsKey(menuItem.NavigateTo))
                {
                    throw new ArgumentException($"The key {menuItem.NavigateTo} is already configured in PageService");
                }

                _pages.Add(menuItem.NavigateTo, pageType);
            }
        }
    }
    private void ConfigurePages(Dictionary<IMenuItem, Type?> pageConfiguration)
    {
        foreach (var (menuItem, pageType) in pageConfiguration)
        {
            ConfigurePage(pageType, menuItem);
            Pages.Add(menuItem);
        }
    }

    private void ConfigureFooterPages(Dictionary<IMenuItem, Type?> pageConfiguration)
    {
        foreach (var (menuItem, pageType) in pageConfiguration)
        {
            ConfigurePage(pageType, menuItem);
            FooterPages.Add(menuItem);
        }
    }
}

