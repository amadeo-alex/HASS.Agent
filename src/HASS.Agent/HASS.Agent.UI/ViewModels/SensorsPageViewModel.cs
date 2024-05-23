﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using Microsoft.UI.Dispatching;
using Windows.AI.MachineLearning;

namespace HASS.Agent.UI.ViewModels;

public partial class SensorsPageViewModel : ViewModelBase, IInfoBadgeAware, INavigationAware
{
    private ISensorManager _sensorManager;
    private ISettingsManager _settingsManager;

    private IInfoBadge _badge = new InfoBadge()
    {
        Type = InfoBadgeType.Success,
        Value = 0
    };

    public ObservableCollection<AbstractDiscoverableViewModel> Sensors = [];

    public RelayCommand<AbstractDiscoverableViewModel> EditCommand { get; set; }
    public RelayCommand<AbstractDiscoverableViewModel> StartStopCommand { get; set; }
    public RelayCommand<AbstractDiscoverableViewModel> DeleteCommand { get; set; }

    public SensorsPageViewModel(DispatcherQueue dispatcherQueue, ISensorManager sensorManager, ISettingsManager settingsManager) : base(dispatcherQueue)
    {
        _sensorManager = sensorManager;
        _settingsManager = settingsManager;

        foreach (var sensor in _sensorManager.Sensors)
            Sensors.Add(new AbstractDiscoverableViewModel
            {
                Active = true,
                Name = sensor.Name,
                Type = sensor.GetType().Name,
                UniqueId = sensor.UniqueId
            });

        _sensorManager.Sensors.CollectionChanged += Sensors_CollectionChanged;

        _badge.Value = Sensors.Count;

        EditCommand = new RelayCommand<AbstractDiscoverableViewModel>((sensorViewModel) =>
        {
            Console.Write("Asd");
        });

        StartStopCommand = new RelayCommand<AbstractDiscoverableViewModel>((sensorViewModel) =>
        {
            if (sensorViewModel == null)
                return;

            var sensor = _sensorManager.Sensors.FirstOrDefault(s => s.UniqueId == sensorViewModel.UniqueId);
            if (sensor != null)
            {
                sensor.Active = !sensor.Active;
                sensorViewModel.Active = sensor.Active;
            }
        });

        DeleteCommand = new RelayCommand<AbstractDiscoverableViewModel>((sensorViewModel) =>
        {
            if (sensorViewModel == null)
                return;

            var configuredEntity = _settingsManager.ConfiguredSensors.FirstOrDefault(cs => cs.UniqueId.ToString() == sensorViewModel.UniqueId);
            if (configuredEntity != null)
                _settingsManager.ConfiguredSensors.Remove(configuredEntity);
        });
    }

    private void Sensors_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems == null)
                    return;

                foreach (AbstractDiscoverable newSensor in e.NewItems)
                    RunOnDispatcher(() => Sensors.Add(new AbstractDiscoverableViewModel
                    {
                        Active = true,
                        Name = newSensor.Name,
                        Type = newSensor.GetType().Name,
                        UniqueId = newSensor.UniqueId
                    }));

                break;

            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems == null)
                    return;

                foreach (AbstractDiscoverable oldSensor in e.OldItems)
                {
                    var sensorViewModel = Sensors.FirstOrDefault(s => s.UniqueId == oldSensor.UniqueId);
                    if (sensorViewModel == null)
                        continue;

                    RunOnDispatcher(() => Sensors.Remove(sensorViewModel));
                }

                break;
        }


        _badge.Value = Sensors.Count;
        RaiseOnPropertyChanged(nameof(Sensors));
    }

    public IInfoBadge InfoBadge
    {
        get => _badge;
        set
        {
            _badge = value;
            RaiseOnPropertyChanged(nameof(InfoBadge));
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
}
