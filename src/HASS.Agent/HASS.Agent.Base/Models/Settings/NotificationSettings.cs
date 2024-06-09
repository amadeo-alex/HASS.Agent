using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Base.Models.Settings;
public class NotificationSettings
{
    public bool Enabled { get; set; } = true;
    public bool IgnoreImageCertificateErrors { get; set; } = false;
    public bool OpenActionUri { get; set; } = false;
}
