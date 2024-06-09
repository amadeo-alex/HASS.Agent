using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Serilog.Core;
using static System.Net.Mime.MediaTypeNames;

namespace HASS.Agent.Base.Contracts.Managers;
public interface IVariableManager
{
    string RootRegKey { get; }
    string CertificateHash { get; }

    string ClientVersion { get; }
    static bool IsClientBeta { get; }
    string ApplicationName { get; }
    string StartupPath { get; }
    string CachePath { get; }
    string ImageCachePath { get; }
    string AudioCachePath { get; }
    string WebViewCachePath { get; }
    string LogPath { get; }
    string ConfigPath { get; }
    string ApplicationSettingsFile { get; }
    string HomeAssistantSettingsFile { get; }
    string NotificationSettingsFile { get; }
    string MqttSettingsSettingsFile { get; }
    string StorageCacheSettingsFile { get; }
    string TraySettingsFile { get; }
    string UpdateSettingsFile { get; }
    string WebViewSettingsFile { get; }
    string QuickActionsFile { get; }
    string CommandsFile { get; }
    string SensorsFile { get; }
}
