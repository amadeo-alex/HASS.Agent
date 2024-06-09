using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using HASS.Agent.Base.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Foundation.Metadata;

namespace HASS.Agent.Base.Models;
public partial class ApplicationSettings : ObservableObject
{
    [GeneratedRegex(@"[^a-zA-Z0-9_-]")]
    private static partial Regex SanitizeRegex();

    [ObservableProperty]
    public OnboardingStatus onboardingStatus = OnboardingStatus.NaverDone;
    [ObservableProperty]
    private bool sanitizeName = true;
    [Obsolete("Configuration variable, please use DeviceName")]
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DeviceName))]
    private string configuredDeviceName = string.Empty;
    [ObservableProperty]
    private string language = string.Empty;
    [ObservableProperty]
    private bool enableStateNotifications = true;
    [ObservableProperty]
    private string theme = "Default";

    [JsonIgnore]
    public string DeviceName => SanitizeName ? SanitizeRegex().Replace(ConfiguredDeviceName, "_") : ConfiguredDeviceName;


    public string ServiceAuthId { get; set; } = string.Empty;



    public string CustomExecutorName { get; set; } = string.Empty;
    public string CustomExecutorBinary { get; set; } = string.Empty;

    public bool LocalApiEnabled { get; set; } = false;
    public int LocalApiPort { get; set; } = 5115;


    public bool MediaPlayerEnabled { get; set; } = true;


    public bool QuickActionsHotKeyEnabled { get; set; } = true;
    public string QuickActionsHotKey { get; set; } = string.Empty;
}

