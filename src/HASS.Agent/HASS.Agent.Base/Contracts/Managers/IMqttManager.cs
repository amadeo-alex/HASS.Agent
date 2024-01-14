using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Models.Entity;
using MQTTnet;

namespace HASS.Agent.Base.Contracts.Managers;
public interface IMqttManager
{
    bool Conncted { get; }
    bool Ready { get; }

    void CreateDeviceConfigModel();
    Task<bool> PublishAsync(MqttApplicationMessage message);
    Task AnnounceAutoDiscoveryConfigAsync(AbstractDiscoverable discoverable, string domain, bool clearConfig = false);
    MqttStatus GetStatus();
    Task AnnounceAvailabilityAsync(bool offline = false);
    Task ClearDeviceConfigAsync();
    void Disconnect();
    Task SubscribeAsync(AbstractDiscoverable command);
    Task UnsubscribeAsync(AbstractDiscoverable command);

    Task SubscribeNotificationsAsync();

    string MqttDiscoveryPrefix();
    MqttDe GetDeviceConfigModel();
    void ReloadConfiguration();
    bool UseRetainFlag();
    Task SubscribeMediaCommandsAsync();
}
