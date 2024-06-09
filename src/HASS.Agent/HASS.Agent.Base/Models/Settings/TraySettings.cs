using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Base.Models.Settings;
public class TraySettings
{
    public bool IconShowDefaultMenu { get; set; } = true;
    public bool IconShowWebView { get; set; } = false;
    public int IconWebViewWidth { get; set; } = 700;
    public int IconWebViewHeight { get; set; } = 560;
    public string IconWebViewUrl { get; set; } = string.Empty;
    public bool IconWebViewBackgroundLoading { get; set; } = false;
    public bool IconWebViewShowMenuOnLeftClick { get; set; } = false;
}
