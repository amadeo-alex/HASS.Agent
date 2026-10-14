using HASS.Agent.Shared.Enums;
using static HASS.Agent.Shared.Functions.Inputs;

namespace HASS.Agent.Shared.HomeAssistant.Commands.KeyCommands
{
    /// <summary>
    /// Simulates a 'mute' mediakey press
    /// </summary>
    public class MediaMuteCommand : KeyCommand
    {
        private const string DefaultName = "mute";

        public MediaMuteCommand(string entityName = DefaultName, string? name = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string? id = null) : base(VirtualKeyShort.VOLUME_MUTE, entityName ?? DefaultName, name, entityType, id) { }
    }
}
