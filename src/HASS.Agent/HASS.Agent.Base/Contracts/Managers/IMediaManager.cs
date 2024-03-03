using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Models.MediaPlayer;
using MQTTnet;

namespace HASS.Agent.Base.Contracts.Managers;
public interface IMediaManager
{
    void HandleReceivedCommand(MediaPlayerCommand command);
}
