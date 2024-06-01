using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace HASS.Agent.Base.Models;
public class ConfiguredEntity : ICloneable //TODO(Amadeo): interface?
{
    public Dictionary<string, string> Properties { get; set; } = [];

    [JsonIgnore]
    public string Type
    {
        get => GetParameter(nameof(Type));
        set => SetParameter(nameof(Type), value);
    }

    [JsonIgnore]
    public Guid UniqueId
    {
        get => Guid.Parse(GetParameter(nameof(UniqueId)));
        set => SetParameter(nameof(UniqueId), value.ToString());
    }

    [JsonIgnore]
    public string Name
    {
        get => GetParameter(nameof(Name));
        set => SetParameter(nameof(Name), value);
    }

    [JsonIgnore]
    public string EntityIdName
    {
        get => GetParameter(nameof(EntityIdName));
        set => SetParameter(nameof(EntityIdName), value);
    }

    [JsonIgnore]
    public int UpdateIntervalSeconds
    {
        get => GetIntParameter(nameof(UpdateIntervalSeconds), 0);
        set => SetIntParameter(nameof(UpdateIntervalSeconds), value);
    }

    [JsonIgnore]
    public bool IgnoreAvailability
    {
        get => GetBoolParameter(nameof(IgnoreAvailability), false);
        set => SetBoolParameter(nameof(IgnoreAvailability), value);
    }

    [JsonIgnore]
    public bool UseAttributes
    {
        get => GetBoolParameter(nameof(UseAttributes), false);
        set => SetBoolParameter(nameof(UseAttributes), value);
    }

    [JsonIgnore]
    public bool Active
    {
        get => GetBoolParameter(nameof(Active), true);
        set => SetBoolParameter(nameof(Active), value);
    }

    public void SetParameter(string parameterName, string value)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
            throw new ArgumentException("parameter name cannot be empty");

        Properties[parameterName] = value;
    }

    public void SetIntParameter(string parameterName, int value)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
            throw new ArgumentException("parameter name cannot be empty");

        Properties[parameterName] = value.ToString();
    }

    public void SetBoolParameter(string parameterName, bool value)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
            throw new ArgumentException("parameter name cannot be empty");

        Properties[parameterName] = value.ToString();
    }

    public string GetParameter(string parameterName, string defaultValue = "")
    {
        if (string.IsNullOrWhiteSpace(parameterName))
            throw new ArgumentException("parameter name cannot be empty");

        if (!Properties.ContainsKey(parameterName))
            Properties[parameterName] = defaultValue;

        return Properties[parameterName];
    }
    
    public int GetIntParameter(string parameterName, int defaultValue)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
            throw new ArgumentException("parameter name cannot be empty");

        var stringParam = GetParameter(parameterName, defaultValue.ToString());
        return Convert.ToInt32(stringParam);
    }

    public bool GetBoolParameter(string parameterName, bool defaultValue)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
            throw new ArgumentException("parameter name cannot be empty");

        var stringParam = GetParameter(parameterName, defaultValue.ToString());
        return Convert.ToBoolean(stringParam);
    }

    public object Clone()
    {
        var clone = JsonConvert.DeserializeObject<ConfiguredEntity>(JsonConvert.SerializeObject(this));
        return clone ?? throw new InvalidOperationException("cannot clone configured entity");
    }
}
