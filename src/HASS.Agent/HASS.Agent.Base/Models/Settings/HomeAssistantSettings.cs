using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Base.Models.Settings;
public class HomeAssistantSettings
{
    public string HassUri { get; set; } = "http://homeassistant.local:8123";
    public string HassToken { get; set; } = string.Empty;
    public bool HassAutoClientCertificate { get; set; } = false;
    public bool HassAllowUntrustedCertificates { get; set; } = true;
    public string HassClientCertificate { get; set; } = string.Empty;
}
