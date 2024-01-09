using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.UI.Contracts.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace HASS.Agent.UI.ViewModels;

public class MenuItem : IMenuItem
{
    public MenuItemType Type { get; set; } = MenuItemType.Normal;
    public Type? ViewModelType
    {
        get; set;
    }
    public string NavigateTo { get; set; } = "";
    public string Title { get; set; } = "";
    public string Glyph { get; set; } = "";
    private IInfoBadge? _infoBadge;
    public IInfoBadge? InfoBadge
    {
        get => _infoBadge;
        set
        {
            if (_infoBadge != null)
                _infoBadge.PropertyChanged -= PropertyChangedPassthrough;
            _infoBadge = value;
            if (_infoBadge != null)
                _infoBadge.PropertyChanged += PropertyChangedPassthrough;
            OnPropertyChanged();
        }
    }
    public BindingList<IMenuItem> MenuItems { get; } = new BindingList<IMenuItem>();

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public MenuItem()
    {

    }
    private void PropertyChangedPassthrough(object sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InfoBadge)));
    }
}