using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Base.Models.Settings;
public class WebViewSettings
{
    public string BrowserName { get; set; } = string.Empty;
    public string BrowserBinary { get; set; } = string.Empty;
    public string BrowserIncognitoArg { get; set; } = string.Empty;
}
