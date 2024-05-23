using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Models.Mqtt;
using HASS.Agent.Base.Enums;
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

    protected readonly ConfiguredEntity _configuration;

    public string Domain { get; set; } = string.Empty;
    public string EntityIdName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string TopicName { get; set; } = string.Empty;
    public string UniqueId { get; set; } = string.Empty;
    public bool UseAttributes { get; set; } = false;
    public bool IgnoreAvailability { get; set; } = false;
    public int UpdateIntervalSeconds { get; set; } = 1;
    public bool Active { get; set; } = true;

    public abstract string State { get; }
    public virtual string Attributes { get; } = string.Empty;
    public DateTime LastUpdated { get; set; } = DateTime.MinValue;
    public string PreviousPublishedState { get; set; } = string.Empty;
    public string PreviousPublishedAttributes { get; set; } = string.Empty;

    protected AbstractDiscoverable(ConfiguredEntity configuredEntity)
    {
        _configuration = configuredEntity;

        UniqueId = configuredEntity.UniqueId.ToString();
        EntityIdName = configuredEntity.EntityIdName;
        Name = configuredEntity.Name;
        UpdateIntervalSeconds = configuredEntity.UpdateIntervalSeconds;
        Domain = HassDomain.Sensor.ToString();
        UseAttributes = configuredEntity.UseAttributes;
    }

    public abstract AbstractMqttDiscoveryConfigModel ConfigureAutoDiscoveryConfig(string discoveryPrefix, AbstractMqttDeviceConfigModel deviceConfigModel);
    public abstract AbstractMqttDiscoveryConfigModel GetAutoDiscoveryConfig();
    //public abstract void ClearAutoDiscoveryConfig();
    public abstract void ResetChecks();
    public abstract ConfiguredEntity ToConfiguredEntity();
}
