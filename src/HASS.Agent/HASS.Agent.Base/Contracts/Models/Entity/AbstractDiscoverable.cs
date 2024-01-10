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


    public static AbstractDiscoverable FromConfiguredEntity(ConfiguredEntity configuredEntity)
    {
        //TODO(Amadeo): add proper exception messages
        var type = configuredEntity.Type ?? throw new ArgumentException("type property is null");
        if (!type.IsAssignableTo(typeof(AbstractDiscoverable)))
            throw new ArgumentException("type property is not assignable from abstract discoverable");

        var instanceMethod = type.GetMethod("CreateInstance", BindingFlags.Public | BindingFlags.Static)
            ?? throw new MethodAccessException("missing CreateInstance method");

        var obj = instanceMethod.Invoke(null, new object[] { configuredEntity }) ?? throw new Exception("no object returned");

        return (AbstractDiscoverable)obj;
    }

    private static string Sanitize(string inputString)
    {
        return SanitizeRegex().Replace(inputString, "_");
    }
}
