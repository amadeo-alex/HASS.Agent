using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Base.Models.Entity;
public class RegisteredEntity
{
    public Type EntityType { get; set;} = typeof(RegisteredEntity); //TODO(Amadeo): ugly
    public bool ClientCompatible { get; set; }
    public bool SatelliteCompatible { get; set; }
}
