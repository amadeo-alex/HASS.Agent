using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUI3Localizer;

namespace HASS.Agent.UI.Helpers;
public static class LocalizerHelper
{
    public static string GetLocalizedString(string key, string prefix = "", string postfix = "")
    {
        return Localizer.Get().GetLocalizedString($"{prefix}{key}{postfix}");
    }

    public static string GetLocalizedString(string key, string prefix)
    {
        return GetLocalizedString(key, prefix, "");
    }

    public static string GetLocalizedString(string key)
    {
        return GetLocalizedString("", key, "");
    }
}