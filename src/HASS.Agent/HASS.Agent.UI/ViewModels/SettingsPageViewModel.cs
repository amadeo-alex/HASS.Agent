using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.Base.Models;
using HASS.Agent.Base.Sensors.SingleValue;
using HASS.Agent.UI.Views.Dialogs;
using HASS.Agent.UI.Views.Pages;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace HASS.Agent.UI.ViewModels;
public partial class SettingsPageViewModel : ViewModelBase
{
    private ISettingsManager _settingsManager;
    private IMqttManager _mqttManager;
    private int x = 0;


    public RelayCommand ButtonCommand { get; set; }
    public RelayCommand ButtonCommand2 { get; set; }
    public RelayCommand ButtonCommand3 { get; set; }
    public RelayCommand ButtonCommand4 { get; set; }
    public Base.Enums.MqttStatus MqttStatus => _mqttManager.Status;

    public List<SomeItem> SomeItems = new List<SomeItem>()
    {
        new() { Name = "a"},
        new() { Name = "b"},
        new() { Name = "c"},
    };

    public SettingsPageViewModel(DispatcherQueue dispatcherQueue, ISettingsManager settingsManager, IMqttManager mqttManager) : base(dispatcherQueue)
    {
        _settingsManager = settingsManager;
        _mqttManager = mqttManager;

        ButtonCommand = new RelayCommand(() =>
        {
            _settingsManager.ConfiguredSensors.Add(new ConfiguredEntity
            {
                Type = typeof(DummySensor).Name,
                Name = $"Dummy Sensor {x}",
                EntityIdName = $"dummysensor{x}",
                UniqueId = Guid.NewGuid(),
                UpdateIntervalSeconds = 20
            });

            x++;
        });

        ButtonCommand2 = new RelayCommand(() =>
        {
            _mqttManager.DisconnectAsync();
        });


        ButtonCommand3 = new RelayCommand(async () =>
        {
            Debug.WriteLine("GC collect");
            GC.Collect();

        });

        ButtonCommand4 = new RelayCommand(async () =>
        {
            _settingsManager.StoreConfiguredEntities();
            _settingsManager.StoreSettings();
        });

        _mqttManager.PropertyChanged += OnMqttPropertyChanged; //Note(Amadeo): leaks
    }

    private void OnMqttPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_mqttManager.Status))
        {
            RaiseOnPropertyChanged(nameof(MqttStatus));
        }
    }
}
