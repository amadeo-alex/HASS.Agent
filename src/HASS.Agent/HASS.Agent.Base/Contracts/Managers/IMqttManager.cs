using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Models.Entity;
using HASS.Agent.Base.Contracts.Models.Mqtt;
using HASS.Agent.Base.Enums;
using MQTTnet;

namespace HASS.Agent.Base.Contracts.Managers;
public interface IMqttManager
{
    MqttStatus Status { get; }
    bool Ready { get; }
    AbstractMqttDeviceConfigModel DeviceConfigModel { get; }

    Task InitializeAsync();
    Task PublishAsync(MqttApplicationMessage message);
    Task AnnounceDeviceConfigModelAsync();
    Task ClearDeviceConfigModelAsync();
    Task DisconnectAsync();
    Task SubscribeAsync(AbstractDiscoverable command);
    Task UnsubscribeAsync(AbstractDiscoverable command);
    Task ReinitializeAsync();
}
