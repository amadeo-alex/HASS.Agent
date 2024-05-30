using HASS.Agent.Base.Models.Entity;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.UI.Models;
public class RegisteredUiEntity
{
    private const string _emptyStringResourceKey = "General_Empty";
    public Type? EntityType { get; set; }
    public Type? AdditionalSettingsUiType { get; set; }
    public string DisplayNameResourceKey { get; set; } = _emptyStringResourceKey;
    public string DescriptionResourceKey { get; set; } = _emptyStringResourceKey;
}
