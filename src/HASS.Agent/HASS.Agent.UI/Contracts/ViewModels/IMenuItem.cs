using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.UI.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace HASS.Agent.UI.Contracts.ViewModels;

public enum MenuItemType
{
    Normal,
    Separator,
    Header
}

public interface IMenuItem : INotifyPropertyChanged
{
    public MenuItemType Type { get; }
    public Type? ViewModelType { get; set; }
    public string NavigateTo { get; set; }
    public string Title { get; set; }
    public string Glyph { get; set; }
    public IInfoBadge? InfoBadge { get; set; }
    public BindingList<IMenuItem> MenuItems { get; }
}
