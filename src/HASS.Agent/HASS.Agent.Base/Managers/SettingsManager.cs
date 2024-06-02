using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.Base.Contracts.Models.Entity;
using HASS.Agent.Base.Models;
using Microsoft.Win32;
using Newtonsoft.Json;
using Serilog;

namespace HASS.Agent.Base.Managers;
public class SettingsManager : ISettingsManager
{
    private readonly IVariableManager _variableManager;
    private readonly IGuidManager _guidManager;

    public ApplicationSettings ApplicationSettings { get; private set; }
    public ObservableCollection<ConfiguredEntity> ConfiguredSensors { get; private set; }
    public ObservableCollection<ConfiguredEntity> ConfiguredCommands { get; private set; }
    public ObservableCollection<IQuickAction> ConfiguredQuickActions { get; private set; }

    public SettingsManager(IVariableManager variableManager, IGuidManager guidManager)
    {
        _variableManager = variableManager;
        _guidManager = guidManager;

        if (!Directory.Exists(_variableManager.ConfigPath))
        {
            Log.Debug("[SETTINGS] Creating initial config directory: {path}", _variableManager.ConfigPath);
            Directory.CreateDirectory(_variableManager.ConfigPath);
        }

        ApplicationSettings = GetApplicationSettings();
        ConfiguredSensors = GetConfiguredSensors();
        ConfiguredCommands = GetConfiguredCommands();
        ConfiguredQuickActions = GetConfiguredQuickActions();

        foreach (var configuredSensor in ConfiguredSensors)
            _guidManager.MarkAsUsed(configuredSensor.UniqueId);

        foreach (var configuredCommand in ConfiguredCommands)
            _guidManager.MarkAsUsed(configuredCommand.UniqueId);

        foreach (var configuredQuickAction in ConfiguredQuickActions)
            _guidManager.MarkAsUsed(configuredQuickAction.UniqueId);

        ConfiguredSensors.CollectionChanged += Configured_CollectionChanged;
        ConfiguredCommands.CollectionChanged += Configured_CollectionChanged;
        ConfiguredQuickActions.CollectionChanged += Configured_CollectionChanged;
    }

    private void Configured_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems == null)
                    return;

                foreach (var configured in e.NewItems)
                {
                    if (configured is ConfiguredEntity configuredEntity)
                    {
                        _guidManager.MarkAsUsed(configuredEntity.UniqueId);
                    }
                    else if (configured is IQuickAction configuredQuickAction)
                    {
                        _guidManager.MarkAsUsed(configuredQuickAction.UniqueId);
                    }
                }
                break;

            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems == null)
                    return;

                foreach (var configured in e.OldItems)
                {
                    if (configured is ConfiguredEntity configuredEntity)
                    {
                        _guidManager.MarkAsUnused(configuredEntity.UniqueId);
                    }
                    else if (configured is IQuickAction configuredQuickAction)
                    {
                        _guidManager.MarkAsUnused(configuredQuickAction.UniqueId);
                    }
                }
                break;
        }
    }

    private ObservableCollection<IQuickAction> GetConfiguredQuickActions()
    {
        Log.Debug("[SETTINGS] Loading quick action configuration");

        var configuredQuickActions = new ObservableCollection<IQuickAction>();

        try
        {
            if (File.Exists(_variableManager.QuickActionsFile))
            {
                Log.Debug("[SETTINGS] Configuration file found, loading");

                var quickActionsConfigurationJson = File.ReadAllText(_variableManager.QuickActionsFile);
                var quickActionConfiguration = JsonConvert.DeserializeObject<ObservableCollection<IQuickAction>>(quickActionsConfigurationJson);
                if (quickActionConfiguration == null)
                {
                    Log.Warning("[SETTINGS] Configuration file cannot be parsed");
                    configuredQuickActions = [];
                }
                else
                {
                    Log.Information("[SETTINGS] Quick actions configuration loaded");
                    configuredQuickActions = quickActionConfiguration;
                }
            }
            else
            {
                Log.Debug("[SETTINGS] Commands configuration not found");
            }
        }
        catch (Exception ex)
        {
            Log.Fatal("[SETTINGS] Exception loading quick actions configuration: {ex}", ex);
            throw;
        }

        return configuredQuickActions;
    }

    private ObservableCollection<ConfiguredEntity> GetConfiguredCommands()
    {
        Log.Debug("[SETTINGS] Loading commands configuration");

        var configuredCommands = new ObservableCollection<ConfiguredEntity>();

        try
        {
            if (File.Exists(_variableManager.CommandsFile))
            {
                Log.Debug("[SETTINGS] Configuration file found, loading");

                var commandsConfigurationJson = File.ReadAllText(_variableManager.CommandsFile);
                var commandsConfiguration = JsonConvert.DeserializeObject<ObservableCollection<ConfiguredEntity>>(commandsConfigurationJson);
                if (commandsConfiguration == null)
                {
                    Log.Warning("[SETTINGS] Configuration file cannot be parsed");
                    configuredCommands = [];
                }
                else
                {
                    Log.Information("[SETTINGS] Commands configuration loaded");
                    configuredCommands = commandsConfiguration;
                }
            }
            else
            {
                Log.Debug("[SETTINGS] Commands configuration not found");
            }
        }
        catch (Exception ex)
        {
            Log.Fatal("[SETTINGS] Exception loading commands configuration: {ex}", ex);
            throw;
        }

        return configuredCommands;
    }

    private ObservableCollection<ConfiguredEntity> GetConfiguredSensors()
    {
        Log.Debug("[SETTINGS] Loading sensor configuration");

        var configuredCommands = new ObservableCollection<ConfiguredEntity>();

        try
        {
            if (File.Exists(_variableManager.SensorsFile))
            {
                Log.Debug("[SETTINGS] Configuration file found, loading");

                var sensorsConfigurationJson = File.ReadAllText(_variableManager.SensorsFile);
                var sensorConfiguration = JsonConvert.DeserializeObject<ObservableCollection<ConfiguredEntity>>(sensorsConfigurationJson);
                if (sensorConfiguration == null)
                {
                    Log.Warning("[SETTINGS] Configuration file cannot be parsed");
                    configuredCommands = [];
                }
                else
                {
                    Log.Information("[SETTINGS] Sensors configuration loaded");
                    configuredCommands = sensorConfiguration;
                }
            }
            else
            {
                Log.Debug("[SETTINGS] Sensors configuration not found");
            }
        }
        catch (Exception ex)
        {
            Log.Fatal("[SETTINGS] Exception loading sensor configuration: {ex}", ex);
            throw;
        }

        return configuredCommands;
    }

    private ApplicationSettings GetApplicationSettings()
    {
        Log.Debug("[SETTINGS] Loading application settings");

        var applicationSettings = new ApplicationSettings();

        try
        {
            if (File.Exists(_variableManager.ApplicationSettingsFile))
            {
                Log.Debug("[SETTINGS] Configuration file found, loading");

                var applicationSettingsJson = File.ReadAllText(_variableManager.ApplicationSettingsFile);
                var loadedApplicationSettings = JsonConvert.DeserializeObject<ApplicationSettings>(applicationSettingsJson);
                if (loadedApplicationSettings != null)
                {
                    Log.Information("[SETTINGS] Application settings loaded");
                    applicationSettings = loadedApplicationSettings;
                }
                else
                {
                    Log.Warning("[SETTINGS] Configuration file cannot be parsed, using default settings");
                }
            }
            else
            {
                Log.Debug("[SETTINGS] Configuration file does not exist, creating default one");
                var applicationSettingsJson = JsonConvert.SerializeObject(applicationSettings, Formatting.Indented);
                File.WriteAllText(_variableManager.ApplicationSettingsFile, applicationSettingsJson);
            }
        }
        catch (Exception ex)
        {
            Log.Fatal("[SETTINGS] Exception loading application settings: {ex}", ex);
            throw;
        }

        return applicationSettings;
    }

    public bool StoreConfiguredEntities()
    {
        return StoreConfiguredSensors()
            && StoreConfiguredCommands()
            && StoreConfiguredQuickActions();
    }

    private bool StoreConfiguredQuickActions()
    {
        Log.Debug("[SETTINGS] Storing configured quick actions");

        try
        {
            var configuredQuickActionsJson = JsonConvert.SerializeObject(ConfiguredQuickActions, Formatting.Indented);
            File.WriteAllText(_variableManager.QuickActionsFile, configuredQuickActionsJson);

            Log.Information("[SETTINGS] Quick actions configuration stored");
        }
        catch (Exception ex)
        {
            Log.Fatal("[SETTINGS] Exception storing quick actions configuration: {ex}", ex);
            return false;
        }

        return true;
    }

    private bool StoreConfiguredCommands()
    {
        Log.Debug("[SETTINGS] Storing configured commands");

        try
        {
            var configuredCommandsJson = JsonConvert.SerializeObject(ConfiguredCommands, Formatting.Indented);
            File.WriteAllText(_variableManager.CommandsFile, configuredCommandsJson);

            Log.Information("[SETTINGS] Commands configuration stored");
        }
        catch (Exception ex)
        {
            Log.Fatal("[SETTINGS] Exception storing commands configuration: {ex}", ex);
            return false;
        }

        return true;
    }

    private bool StoreConfiguredSensors()
    {
        Log.Debug("[SETTINGS] Storing configured sensors");

        try
        {
            var configuredSensorsJson = JsonConvert.SerializeObject(ConfiguredSensors, Formatting.Indented);
            File.WriteAllText(_variableManager.SensorsFile, configuredSensorsJson);

            Log.Information("[SETTINGS] Sensor configuration stored");
        }
        catch (Exception ex)
        {
            Log.Fatal("[SETTINGS] Exception storing sensor configuration: {ex}", ex);
            return false;
        }

        return true;
    }

    public bool StoreApplicationSettings()
    {
        Log.Debug("[SETTINGS] Storing application settings");

        try
        {
            var applicationSettingsJson = JsonConvert.SerializeObject(ApplicationSettings, Formatting.Indented);
            File.WriteAllText(_variableManager.ApplicationSettingsFile, applicationSettingsJson);

            Log.Information("[SETTINGS] Application settings stored");
        }
        catch (Exception ex)
        {
            Log.Fatal("[SETTINGS] Exception storing application settings: {ex}", ex);
            return false;
        }

        return true;
    }

    public void AddUpdateConfiguredCommand(ConfiguredEntity command)
    {
        var existingCommand = ConfiguredCommands.FirstOrDefault(c => c.UniqueId == command.UniqueId);
        if (existingCommand != null)
        {
            if (existingCommand.Type != command.Type)
                throw new ArgumentException($"command with ID {existingCommand.UniqueId} of different type ({existingCommand.Type}) than {command.Type} already exists");

            ConfiguredSensors.Remove(existingCommand);
        }

        ConfiguredCommands.Add(command);
    }

    public bool GetExtendedLoggingSetting()
    {
        try
        {
            var setting = Registry.GetValue(_variableManager.RootRegKey, "ExtendedLogging", "0") as string;
            if (string.IsNullOrEmpty(setting))
                return false;

            return setting == "1";
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[SETTINGS] Error retrieving extended logging setting: {ex}", ex);
            return false;
        }
    }

    public void SetExtendedLoggingSetting(bool enabled)
    {
        try
        {
            Registry.SetValue(_variableManager.RootRegKey, "ExtendedLogging", enabled ? "1" : "0", RegistryValueKind.String);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[SETTINGS] Error storing extended logging setting: {ex}", ex);
        }
    }

    //TODO(Amadeo): verify if necessary
    public bool GetDpiWarningShown()
    {
        try
        {
            var setting = Registry.GetValue(_variableManager.RootRegKey, "DpiWarningShown", "0") as string;
            if (string.IsNullOrEmpty(setting))
                return false;

            return setting == "1";
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[SETTINGS] Error retrieving DPI-warning-shown setting: {ex}", ex);
            return false;
        }
    }

    public void SetDpiWarningShown(bool shown)
    {
        try
        {
            Registry.SetValue(_variableManager.RootRegKey, "DpiWarningShown", shown ? "1" : "0", RegistryValueKind.String);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[SETTINGS] Error storing DPI-warning-shown setting: {ex}", ex);
        }
    }

    //TODO(Amadeo): remove
    /// <summary>
    /// Sends the current MQTT appsettings to the satellite service, optionally with a new client ID
    /// </summary>
    /// <returns></returns>
    public async Task<bool> SendMqttSettingsToServiceAsync(bool sendNewClientId = false)
    {
        try
        {
            // create settings obj
            /*            var config = new ServiceMqttSettings
                        {
                            MqttAddress = Variables.AppSettings.MqttAddress,
                            MqttPort = Variables.AppSettings.MqttPort,
                            MqttUseTls = Variables.AppSettings.MqttUseTls,
                            MqttUsername = Variables.AppSettings.MqttUsername,
                            MqttPassword = Variables.AppSettings.MqttPassword,
                            MqttDiscoveryPrefix = Variables.AppSettings.MqttDiscoveryPrefix,
                            MqttClientId = sendNewClientId ? Guid.NewGuid().ToString()[..8] : string.Empty,
                            MqttRootCertificate = Variables.AppSettings.MqttRootCertificate,
                            MqttClientCertificate = Variables.AppSettings.MqttClientCertificate,
                            MqttAllowUntrustedCertificates = Variables.AppSettings.MqttAllowUntrustedCertificates,
                            MqttUseRetainFlag = Variables.AppSettings.MqttUseRetainFlag
                        };

                        // store
                        var (storedOk, _) = await Task.Run(async () => await Variables.RpcClient.SetServiceMqttSettingsAsync(config).WaitAsync(Variables.RpcConnectionTimeout));
                        if (!storedOk)
                        {
                            Log.Error("[SETTINGS] Sending MQTT settings to service failed");
                            return false;
                        }*/

            // done
            return true;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[SETTINGS] Error sending MQTT settings to service: {err}", ex.Message);
            return false;
        }
    }

    public string GetDeviceSerialNumber()
    {
        var serialNumber = string.Empty;
        try
        {
            serialNumber = Registry.GetValue(_variableManager.RootRegKey, "DeviceSerialNumber", string.Empty) as string;
            if (string.IsNullOrEmpty(serialNumber))
            {
                Log.Debug("[SETTINGS] Generating new device serial number");
                serialNumber = Guid.NewGuid().ToString();
                SetDeviceSerialNumber(serialNumber);
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[SETTINGS] Error retrieving DPI-warning-shown setting: {err}", ex.Message);
        }

        return serialNumber ?? string.Empty;
    }

    public void SetDeviceSerialNumber(string deviceSerialNumber)
    {
        try
        {
            Registry.SetValue(_variableManager.RootRegKey, "DeviceSerialNumber", deviceSerialNumber, RegistryValueKind.String);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[SETTINGS] Error storing device serial number setting: {ex}", ex);
        }
    }

    public bool GetHideDonateButtonSetting()
    {
        try
        {
            var setting = Registry.GetValue(_variableManager.RootRegKey, "HideDonateButton", "0") as string;
            if (string.IsNullOrEmpty(setting))
                return false;

            return setting == "1";
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[SETTINGS] Error retrieving 'hide donate button from the main window' setting: {err}", ex.Message);
            return false;
        }
    }

    public void SetHideDonateButtonSetting(bool hide)
    {
        try
        {
            Registry.SetValue(_variableManager.RootRegKey, "HideDonateButton", hide ? "1" : "0", RegistryValueKind.String);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[SETTINGS] Error storing 'hide donate button from the main window' setting: {err}", ex.Message);
        }
    }
}
