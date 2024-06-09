using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HASS.Agent.Base.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Foundation.Metadata;

namespace HASS.Agent.Base.Models;
public partial class ApplicationSettings
{
    [GeneratedRegex(@"[^a-zA-Z0-9_-]")]
    private static partial Regex SanitizeRegex();

    public OnboardingStatus OnboardingStatus { get; set; } = OnboardingStatus.NaverDone;
    public bool SanitizeName { get; set; } = true;
    [Obsolete("Configuration variable, please use DeviceName")]
    public string ConfiguredDeviceName { get; set; } = string.Empty;
    [JsonIgnore]
    public string DeviceName => SanitizeName ? SanitizeRegex().Replace(ConfiguredDeviceName, "_") : ConfiguredDeviceName;
    public string InterfaceLanguage { get; set; } = string.Empty;
    public bool EnableStateNotifications { get; set; } = true;


    

    public string ServiceAuthId { get; set; } = string.Empty;



    public string CustomExecutorName { get; set; } = string.Empty;
    public string CustomExecutorBinary { get; set; } = string.Empty;

    public bool LocalApiEnabled { get; set; } = false;
    public int LocalApiPort { get; set; } = 5115;


    public bool MediaPlayerEnabled { get; set; } = true;


    public bool QuickActionsHotKeyEnabled { get; set; } = true;
    public string QuickActionsHotKey { get; set; } = string.Empty;
}

