using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.UI.ViewModels;

namespace HASS.Agent.UI.Contracts.ViewModels;
public interface IInfoBadgeAware
{
    public IInfoBadge InfoBadge { get; set; }
}
