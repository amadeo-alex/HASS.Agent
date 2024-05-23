using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.Base.Models;
using HASS.Agent.Base.Sensors.SingleValue;
using Microsoft.UI.Dispatching;

namespace HASS.Agent.UI.ViewModels;
public partial class SettingsPageViewModel : ViewModelBase
{
    private ISettingsManager _settingsManager;
    private IMqttManager _mqttManager;
    private int x = 0;


    public RelayCommand ButtonCommand { get; set; }
    public RelayCommand ButtonCommand2 { get; set; }

    public Base.Enums.MqttStatus MqttStatus => _mqttManager.Status;

    public SettingsPageViewModel(DispatcherQueue dispatcherQueue, ISettingsManager settingsManager, IMqttManager mqttManager) : base(dispatcherQueue)
    {
        _settingsManager = settingsManager;
        _mqttManager = mqttManager;

        ButtonCommand = new RelayCommand(() =>
        {
            _settingsManager.ConfiguredSensors.Add(new ConfiguredEntity
            {
                Type = typeof(DummySensor).Name,
                Name = $"added sensor {x}",
                EntityIdName = $"added sensor {x}",
                UniqueId = Guid.NewGuid(),
                UpdateIntervalSeconds = 1
            });

            x++;
        });

        ButtonCommand2 = new RelayCommand(() =>
        {
            _mqttManager.DisconnectAsync();
        });

        _mqttManager.PropertyChanged += OnMqttPropertyChanged;
    }

    private void OnMqttPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_mqttManager.Status))
        {
            RaiseOnPropertyChanged(nameof(MqttStatus));
        }
    }
}
