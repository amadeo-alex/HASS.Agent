using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Base.Models.Settings;
public class MqttSettings
{
    public bool Enabled { get; set; } = true;
    public string Address { get; set; } = "homeassistant.local";
    public int Port { get; set; } = 1883;
    public bool UseTls { get; set; }
    public bool AllowUntrustedCertificates { get; set; } = true;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string DiscoveryPrefix { get; set; } = "homeassistant";
    public string ClientId { get; set; } = string.Empty;
    public bool UseRetainFlag { get; set; } = true;
    public string RootCertificate { get; set; } = string.Empty;
    public string ClientCertificate { get; set; } = string.Empty;
    public int DisconnectedGracePeriodSeconds { get; set; } = 60;
}
