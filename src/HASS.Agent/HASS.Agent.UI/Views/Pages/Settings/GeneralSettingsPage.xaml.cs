using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using HASS.Agent.UI.Helpers;
using HASS.Agent.UI.ViewModels;
using HASS.Agent.UI.ViewModels.Settings;
using HASS.Agent.UI.Views.Dialogs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HASS.Agent.UI.Views.Pages.Settings;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class GeneralSettingsPage : Page
{
    private readonly GeneralSettingsPageViewModel _viewModel;
    public GeneralSettingsPage()
    {
        _viewModel = App.GetService<GeneralSettingsPageViewModel>();
        this.InitializeComponent();
        DataContext = _viewModel;
    }

    private async void ChangeNameButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ContentDialog
        {
            XamlRoot = XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = LocalizerHelper.GetLocalizedString("Page_GeneralSettings_NameDialog_Title"),
            PrimaryButtonText = LocalizerHelper.GetLocalizedString("General_Save"),
            CloseButtonText = LocalizerHelper.GetLocalizedString("General_Cancel"),
            DefaultButton = ContentDialogButton.Primary,
            Content = new TextInputDialogContent("Page_GeneralSettings_NameDialog_Query", _viewModel.ConfiguredDeviceName)
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            var newDeviceName = ((dialog.Content as TextInputDialogContent)?.DataContext as TextInputDialogContentViewModel)?.TextBoxContent;
            if (newDeviceName != null)
                _viewModel.ConfiguredDeviceName = newDeviceName;
        }
    }
}
