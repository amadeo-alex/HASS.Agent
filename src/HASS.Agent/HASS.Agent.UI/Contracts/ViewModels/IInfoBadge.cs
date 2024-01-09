using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.UI.Contracts.ViewModels;

public enum InfoBadgeType
{
    Attention,
    Information,
    Success,
    Critical
}
public interface IInfoBadge : INotifyPropertyChanged
{
    public int Value { get; set; }
    public InfoBadgeType Type { get; set; }
}
