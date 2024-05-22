using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using HASS.Agent.Base.Enums;
using HASS.Agent.Base.Models;

namespace HASS.Agent.Base.Contracts.Models.Entity;

/// <summary>
/// Base for all commands.
/// </summary>
public abstract class AbstractCommand : AbstractDiscoverable
{
    private const string _configuredActionParam = "configuredAction";

    public const string StateOn = "ON";
    public const string StateOff = "OFF";

    public abstract string DefaultEntityIdName { get; }

    public string ConfiguredAction => _configuration.GetParameter(_configuredActionParam);

    protected AbstractCommand(IServiceProvider serviceProvider, ConfiguredEntity configuredEntity) : base(configuredEntity)
    {
        UniqueId = configuredEntity.UniqueId.ToString();
        EntityIdName = configuredEntity.EntityIdName;
        Name = configuredEntity.Name;
        UpdateIntervalSeconds = configuredEntity.UpdateIntervalSeconds;
        Domain = HassDomain.Button.ToString().ToLower();
        UseAttributes = configuredEntity.UseAttributes;
    }

    public abstract void TurnOn();
    public abstract void TurnOn(string action);
    public abstract void TurnOff();

    public override void ResetChecks()
    {
        LastUpdated = DateTime.MinValue;

        PreviousPublishedState = string.Empty;
        PreviousPublishedAttributes = string.Empty;
    }
}
