using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Base.Models.Settings;
public class UpdateSettings
{
    public bool CheckForUpdates { get; set; } = true;
    public string LastUpdateNotificationShown { get; set; } = string.Empty;
    public bool EnableExecuteUpdateInstaller { get; set; } = true;
    public bool ShowBetaUpdates { get; set; }
}
