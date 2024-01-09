using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.UI.Contracts.Services;
using HASS.Agent.UI.Contracts.ViewModels;
using HASS.Agent.UI.Helpers;
using HASS.Agent.UI.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace HASS.Agent.UI.Services;
public class NavigationViewService : INavigationViewService
{
    private readonly INavigationService _navigationService;
    private readonly IPageService _pageService;

    private NavigationView? _navigationView;

    public IList<object>? MenuItems => _navigationView?.MenuItems;

    public object? SettingsItem => _navigationView?.SettingsItem;

    public NavigationViewService(INavigationService navigationService, IPageService pageService)
    {
        _navigationService = navigationService;
        _pageService = pageService;
    }

    [MemberNotNull(nameof(_navigationView))]
    public void Initialize(NavigationView navigationView)
    {
        _navigationView = navigationView;
        _navigationView.BackRequested += OnBackRequested;
        _navigationView.ItemInvoked += OnItemInvoked;
    }

    public void UnregisterEvents()
    {
        if (_navigationView != null)
        {
            _navigationView.BackRequested -= OnBackRequested;
            _navigationView.ItemInvoked -= OnItemInvoked;
        }
    }

    public MenuItem? GetSelectedItem(Type pageType)
    {
        if (_navigationView == null)
        {
            return null;
        }

        return GetSelectedItem(_pageService.Pages, pageType) ?? GetSelectedItem(_pageService.FooterPages, pageType);
    }

    private MenuItem? GetSelectedItem(IEnumerable<object> menuItems, Type pageType)
    {
        foreach (var item in menuItems.OfType<MenuItem>())
        {
            if (item.Type != MenuItemType.Normal)
            {
                continue;
            }

            if (IsMenuItemForPageType(item, pageType))
            {
                return item;
            }

            var selectedChild = GetSelectedItem(item.MenuItems, pageType);
            if (selectedChild != null)
            {
                return selectedChild;
            }
        }

        return null;
    }

    private bool IsMenuItemForPageType(MenuItem pageEntry, Type sourcePageType)
    {
        return _pageService.GetPageType(pageEntry.NavigateTo) == sourcePageType;
    }

    private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) => _navigationService.GoBack();

    private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        /*        if (args.IsSettingsInvoked)
                {
                    _navigationService.NavigateTo(typeof(SettingsPageViewModel).FullName!);
                }
                else
                {*/
        var selectedItem = args.InvokedItemContainer as NavigationViewItem;

        if (selectedItem?.GetValue(NavigationHelper.NavigateToProperty) is string pageKey)
        {
            _navigationService.NavigateTo(pageKey);
        }
        /*        }*/
    }
}

