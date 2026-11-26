using HASS.Agent.Enums;

namespace HASS.Agent.Models.HomeAssistant;

public class MqttMediaPlayerMessage
{
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string AlbumTitle { get; set; } = string.Empty;
    public string AlbumArtist { get; set; } = string.Empty;

    public double Duration { get; set; }
    public double CurrentPosition { get; set; }
    public MediaPlayerState State { get; set; }
    public int Volume { get; set; }
    public bool Muted { get; set; }
}