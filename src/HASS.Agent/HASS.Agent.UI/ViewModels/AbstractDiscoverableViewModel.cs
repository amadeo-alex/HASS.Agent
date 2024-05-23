using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HASS.Agent.UI.ViewModels;

public partial class AbstractDiscoverableViewModel : ObservableObject
{
    [ObservableProperty]
    public bool active = false;
    [ObservableProperty]
    public string name = string.Empty;
    [ObservableProperty]
    public string type = string.Empty;
    [ObservableProperty]
    public string uniqueId = string.Empty;
}
