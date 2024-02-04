using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Foundation.Metadata;

namespace HASS.Agent.Base.Enums;
public enum MediaPlayerCommandType
{
    [Default]
    [JsonProperty("unknown")]
    Unknown,

    [JsonProperty("setvolume")]
    SetVolume,

    [JsonProperty("volumeup")]
    VolumeUp,

    [JsonProperty("volumedown")]
    VolumeDown,

    [JsonProperty("mute")]
    Mute,

    [JsonProperty("play")]
    Play,

    [JsonProperty("pause")]
    Pause,

    [JsonProperty("stop")]
    Stop,

    [JsonProperty("next")]
    Next,

    [JsonProperty("previous")]
    Previous,

    [JsonProperty("play_media")]
    PlayMedia,

    [JsonProperty("seek")]
    Seek
}
