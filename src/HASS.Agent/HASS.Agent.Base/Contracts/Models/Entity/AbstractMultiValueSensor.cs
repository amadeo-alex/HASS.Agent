using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Enums;

namespace HASS.Agent.Base.Contracts.Models.Entity;

/// <summary>
/// Base for all multi-value sensors.
/// </summary>
public abstract class AbstractMultiValueSensor : AbstractDiscoverable
{
    public abstract IDictionary<string, AbstractSingleValueSensor> Sensors { get; protected set; }

    protected AbstractMultiValueSensor(string entityIdName, string name, int updateIntervalSeconds, string uniqueId, bool useAttributes)
    {
        UniqueId = uniqueId;
        EntityIdName = entityIdName;
        Name = name;
        UpdateIntervalSeconds = updateIntervalSeconds;
        Domain = HassDomain.Sensor.ToString();
        UseAttributes = useAttributes;
    }

    public override void ResetChecks()
    {
        foreach(var sensor in Sensors)
        {
            sensor.Value.ResetChecks();
        }
    }
}
