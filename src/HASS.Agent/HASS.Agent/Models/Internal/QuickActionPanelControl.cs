using HASS.Agent.Controls;

namespace HASS.Agent.Models.Internal
{
    public class QuickActionPanelControl
    {
        public QuickActionControl? QuickActionControl { get; set; }
        public int Row { get; set; } = -1;
        public int Column { get; set; } = -1;
        
        /// <summary>
        /// Disposes the QuickActionControl
        /// </summary>
        public void Dispose() => QuickActionControl?.Dispose();
    }
}
