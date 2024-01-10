using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Base.Models;
public class ConfiguredEntity : Dictionary<string, object>
{
    public Type? Type
    {
        get => GetParameter(new object().GetType()); //TODO(Amadeo): ugly?
        set => SetParameter(value);
    }

    public Guid UniqueId
    {
        get => GetParameter(Guid.Empty);
        set => SetParameter(value);
    }

    public string Name
    {
        get => GetParameter(string.Empty);
        set => SetParameter(value);
    }

    public string EntityIdName
    {
        get => GetParameter(string.Empty);
        set => SetParameter(value);
    }

    public int UpdateIntervalSeconds
    {
        get => GetParameter(30);
        set => SetParameter(value);
    }

    public bool IgnoreAvailability
    {
        get => GetParameter(false);
        set => SetParameter(value);
    }

    public T GetParameter<T>(T defaultValue, [CallerMemberName] string parameterName = "")
    {
        if(defaultValue == null)
            throw new ArgumentNullException(nameof(defaultValue));

        if (string.IsNullOrWhiteSpace(parameterName))
            throw new ArgumentException("parameter name cannot be empty");

        if (!ContainsKey(parameterName))
            this[parameterName] = defaultValue;

        return (T)this[parameterName];
    }

    public void SetParameter<T>(T value, [CallerMemberName] string parameterName = "")
    {
        if(value == null)
            throw new ArgumentNullException(nameof(value));

        this[parameterName] = value;
    }
}
