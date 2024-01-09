using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using HASS.Agent.UI.Contracts.Services;
using HASS.Agent.UI.Contracts.ViewModels;
using HASS.Agent.UI.Views.Pages;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;
using static HASS.Agent.UI.Services.PageService;

namespace HASS.Agent.UI.ViewModels;
[INotifyPropertyChanged]
public partial class ShellPageViewModel
{
    [ObservableProperty]
    private bool isBackEnabled;

    [ObservableProperty]
    private MenuItem? selected;

    private readonly IPageService _pageService;

    public INavigationService NavigationService { get; }

    public INavigationViewService NavigationViewService { get; }

    public BindingList<IMenuItem> MenuItems => _pageService.Pages;
    public BindingList<IMenuItem> FooterMenuItems => _pageService.FooterPages;

    public ShellPageViewModel(INavigationService navigationService, INavigationViewService navigationViewService, IPageService pageService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;
        NavigationViewService = navigationViewService;

        _pageService = pageService;

        _pageService.Pages.ListChanged += Pages_ListChanged;
        _pageService.FooterPages.ListChanged += Pages_ListChanged;
    }

    private void Pages_ListChanged(object? sender, ListChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MenuItems)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FooterMenuItems)));
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MenuItems)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FooterMenuItems)));
        IsBackEnabled = NavigationService.CanGoBack;

        /*        if (e.SourcePageType == typeof(SettingsPage))
                {
                    Selected = NavigationViewService.SettingsItem;
                    return;
                }*/

        var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
        if (selectedItem != null)
        {
            Selected = selectedItem;
        }


    }
}
