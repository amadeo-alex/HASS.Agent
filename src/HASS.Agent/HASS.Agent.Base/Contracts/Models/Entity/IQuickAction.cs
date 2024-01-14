using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Enums;

namespace HASS.Agent.Base.Contracts.Models.Entity;
public interface IQuickAction
{
    public Guid UniqueId { get; set; }
    public HassDomain Domain { get; set; }
    public string Entity { get; set; }
    public HassAction Action { get; set; }
    public bool HotKeyEnabled { get; set; }
    public string HotKey { get; set; }
    public string Description { get; set; }
}
