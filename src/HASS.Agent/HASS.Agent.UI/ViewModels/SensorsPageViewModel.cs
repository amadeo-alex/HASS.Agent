using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.Base.Contracts.Models.Entity;
using HASS.Agent.UI.Contracts.ViewModels;

namespace HASS.Agent.UI.ViewModels;

public class TestVM
{
    public string Name { get; set; }
}

public partial class SensorsPageViewModel : INotifyPropertyChanged, IInfoBadgeAware, INavigationAware
{
    private ISensorManager _sensorManager;

    private IInfoBadge _badge = new InfoBadge()
    {
        Type = InfoBadgeType.Success,
        Value = 0
    };

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<AbstractDiscoverable> Sensors => _sensorManager.Sensors;

    public RelayCommand ButtonCommand { get; set; }

    public SensorsPageViewModel(ISensorManager sensorManager)
    {
        _sensorManager = sensorManager;

        _sensorManager.Sensors.CollectionChanged += Sensors_CollectionChanged;

        _badge.Value = Sensors.Count;

        ButtonCommand = new RelayCommand(() =>
        {
            _badge.Value++;
            OnPropertyChanged(nameof(InfoBadge));
        });
    }

    private void Sensors_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Sensors));
        _badge.Value = Sensors.Count;
        OnPropertyChanged(nameof(InfoBadge));
    }

    public IInfoBadge InfoBadge
    {
        get => _badge;
        set
        {
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
