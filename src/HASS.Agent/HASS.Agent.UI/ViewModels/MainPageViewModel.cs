using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HASS.Agent.UI.ViewModels;
public partial class MainPageViewModel : ObservableRecipient
{
    public const string NavigateTo = "main";
    public MainPageViewModel()
    {
    }
}
