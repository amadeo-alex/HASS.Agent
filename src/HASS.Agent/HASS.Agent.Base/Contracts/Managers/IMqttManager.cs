using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Models.Entity;
using HASS.Agent.Base.Contracts.Models.Mqtt;
using HASS.Agent.Base.Enums;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;

namespace HASS.Agent.Base.Contracts.Managers;

public interface IMqttMessageHandler
{
    Task HandleMqttMessage(MqttApplicationMessage message);
}
public interface IMqttManager
{

    MqttStatus Status { get; }
    bool Ready { get; }
    AbstractMqttDeviceConfigModel DeviceConfigModel { get; }

    void RegisterMessageHandler(string topic, IMqttMessageHandler handler);
    void UnregisterMessageHandler(string topic);
    Task StartClient();
    Task PublishAsync(MqttApplicationMessage message);
    Task AnnounceDeviceConfigModelAsync();
    Task ClearDeviceConfigModelAsync();
    Task DisconnectAsync();
    Task SubscribeAsync(AbstractDiscoverable command);
    Task UnsubscribeAsync(AbstractDiscoverable command);
    Task ReinitializeAsync();
}
