using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Base.Enums;
public enum MqttStatus
{
    NotInitialized,
    Initializing,
    Initialized,
    Connecting,
    Connected,
    Ready,
    Disconnecting,
    Disconnected,
    Error
}
