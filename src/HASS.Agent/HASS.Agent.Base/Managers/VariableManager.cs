using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.Base.Models;
using static System.Net.Mime.MediaTypeNames;

namespace HASS.Agent.Base.Managers;
public class VariableManager : IVariableManager
{
    public string RootRegKey { get; } = @"HKEY_CURRENT_USER\SOFTWARE\HASSAgent\Client";
    public string CertificateHash { get; } = "F5E2C6F0BB7C78E82BFFE33ABC98A689BE690D10";

    public string ClientVersion { get; }
    public bool IsClientBeta { get; }
    public string ApplicationName { get; }
    public string StartupPath { get; }
    public string CachePath { get; }
    public string ImageCachePath { get; }
    public string AudioCachePath { get; }
    public string WebViewCachePath { get; }
    public string LogPath { get; }
    public string ConfigPath { get; }
    public string ApplicationSettingsFile { get; }
    public string HomeAssistantSettingsFile { get; }
    public string NotificationSettingsFile { get; }
    public string MqttSettingsSettingsFile { get; }
    public string StorageCacheSettingsFile { get; }
    public string TraySettingsFile { get; }
    public string UpdateSettingsFile { get; }
    public string WebViewSettingsFile { get; }
    public string QuickActionsFile { get; }
    public string CommandsFile { get; }
    public string SensorsFile { get; }

    public VariableManager(ApplicationInfo applicationInfo)
    {
        ClientVersion = applicationInfo.Version;
        IsClientBeta = ClientVersion.Contains('b');
        ApplicationName = applicationInfo.Name;

        StartupPath = Path.GetDirectoryName(applicationInfo.ExecutablePath) ?? throw new Exception("cannot get executable path directory name");
        CachePath = Path.Combine(StartupPath, "cache");
        ImageCachePath = Path.Combine(CachePath, "images");
        AudioCachePath = Path.Combine(CachePath, "audio");
        WebViewCachePath = Path.Combine(CachePath, "webview");
        LogPath = Path.Combine(StartupPath, "logs");
        ConfigPath = Path.Combine(StartupPath, "config");

        ApplicationSettingsFile = Path.Combine(ConfigPath, "applicationSettings.json");
        HomeAssistantSettingsFile = Path.Combine(ConfigPath, "homeAssistantSettings.json");
        NotificationSettingsFile = Path.Combine(ConfigPath, "notificationSettings.json");
        MqttSettingsSettingsFile = Path.Combine(ConfigPath, "mqttSettings.json");
        StorageCacheSettingsFile = Path.Combine(ConfigPath, "storageSettings.json");
        TraySettingsFile = Path.Combine(ConfigPath, "traySettings.json");
        UpdateSettingsFile = Path.Combine(ConfigPath, "updateSettings.json");
        WebViewSettingsFile = Path.Combine(ConfigPath, "webViewSettings.json");

        QuickActionsFile = Path.Combine(ConfigPath, "quickactions.json");
        CommandsFile = Path.Combine(ConfigPath, "commands.json");
        SensorsFile = Path.Combine(ConfigPath, "sensors.json");
    }
}
