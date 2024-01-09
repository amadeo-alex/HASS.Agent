using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HASS.Agent.UI.Contracts.ViewModels;

namespace HASS.Agent.UI.ViewModels;
public partial class SensorsPageViewModel : INotifyPropertyChanged, IInfoBadgeAware, INavigationAware
{

    private IInfoBadge _badge = new InfoBadge()
    {
        Type = InfoBadgeType.Success,
        Value = 0
    };

    public event PropertyChangedEventHandler? PropertyChanged;

    public RelayCommand ButtonCommand { get; set; }

    public SensorsPageViewModel()
    {
        ButtonCommand = new RelayCommand(() =>
        {
            _badge.Value++;
            OnPropertyChanged(nameof(InfoBadge));
        });
    }

    public IInfoBadge InfoBadge
    {
        get => _badge;
        set {
            _badge = value;
            OnPropertyChanged();
        }
    }
    public void OnNavigatedFrom()
    {
        //value++;
    }
    public void OnNavigatedTo(object parameter)
    {
        //value++;
    }

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
