using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Enums;

namespace HASS.Agent.Base.Contracts.Models.Entity;

/// <summary>
/// Base for all single-value sensors.
/// </summary>
public abstract class AbstractSingleValueSensor : AbstractDiscoverable
{
    protected AbstractSingleValueSensor(string entityIdName, string name, int updateIntervalSeconds, string uniqueId, bool useAttributes)
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
        LastPublished = DateTime.MinValue;

        PreviousPublishedState = string.Empty;
        PreviousPublishedAttributes = string.Empty;
    }
}
