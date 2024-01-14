using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Models.Mqtt;
using HASS.Agent.Base.Models;

namespace HASS.Agent.Base.Contracts.Models.Entity;
public abstract partial class AbstractDiscoverable : IDiscoverable
{
    [GeneratedRegex("[^a-zA-Z0-9_-]")]
    private static partial Regex SanitizeRegex();
    private static string Sanitize(string inputString)
    {
        return SanitizeRegex().Replace(inputString, "_");
    }

    private IServiceProvider? _serviceProvider;

    public string Domain { get; set; } = string.Empty;
    public string EntityIdName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string TopicName { get; set; } = string.Empty;
    public string UniqueId { get; set; } = string.Empty;
    public bool UseAttributes { get; set; } = false;
    public bool IgnoreAvailability { get; set; } = false;
    public int UpdateIntervalSeconds { get; set; } = 1;
    public abstract string State { get; }
    public virtual string Attributes { get; } = string.Empty;
    public DateTime LastPublished { get; protected set; } = DateTime.MinValue;
    public string PreviousPublishedState { get; protected set; } = string.Empty;
    public string PreviousPublishedAttributes { get; protected set; } = string.Empty;

    public abstract AbstractMqttDiscoveryConfigModel ConfigureAutoDiscoveryConfig(string discoveryPrefix, AbstractMqttDeviceConfigModel deviceConfigModel);
    public abstract AbstractMqttDiscoveryConfigModel? GetAutoDiscoveryConfig();
    //public abstract void ClearAutoDiscoveryConfig();
    public abstract void ResetChecks();
    public abstract ConfiguredEntity ToConfiguredEntity();
}
