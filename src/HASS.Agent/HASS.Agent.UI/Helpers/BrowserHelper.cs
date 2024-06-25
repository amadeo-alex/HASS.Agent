using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace HASS.Agent.UI.Helpers;
internal class BrowserHelper
{
    internal static void OpenUrl(string url)
    {
        using var _ = Process.Start(
            new ProcessStartInfo(url)
            {
                UseShellExecute = true
            });

        return;
    }
}
