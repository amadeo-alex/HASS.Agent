using HASS.Agent.Models.HomeAssistant;

namespace HASS.Agent.Models.Internal
{
    public class Action
    {
        public string Description { get; set; } = string.Empty;
        public HassEntity? Entity { get; set; }
    }
}
