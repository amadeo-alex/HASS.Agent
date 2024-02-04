using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Enums;
using Newtonsoft.Json.Linq;

namespace HASS.Agent.Base.Contracts.Models.MediaPlayer;
public class MediaPlayerCommand
{
    public MediaPlayerCommandType Type { get; set; }
    public JObject? Data { get; set; }
}
