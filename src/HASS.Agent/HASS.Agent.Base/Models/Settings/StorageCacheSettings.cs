using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Base.Models.Settings;
public class StorageCacheSettings
{
    public int ImageCacheRetentionDays { get; set; } = 7;
    public int AudioCacheRetentionDays { get; set; } = 7;
    public int WebViewCacheRetentionDays { get; set; } = 0;
    public DateTime WebViewCacheLastCleared { get; set; } = DateTime.Now;
}
