using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Models.Entity;
using HASS.Agent.Base.Models;

namespace HASS.Agent.Base.Helpers;
public static class Extensions
{
    public static string GetShortType(this Type type)
    {
        return $"{type.FullName}, {type.Assembly.GetName().Name}";
    }

}
