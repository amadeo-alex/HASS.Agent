using HASS.Agent.Base.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.UI.Models;
public class RegisteredUiEntity
{
    public RegisteredEntity Entity { get; set; } = new RegisteredEntity();
    public Type InterfaceType { get; set; } = typeof(RegisteredUiEntity);
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
