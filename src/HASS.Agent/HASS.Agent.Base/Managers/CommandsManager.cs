using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.Base.Contracts.Models.Entity;
using HASS.Agent.Base.Enums;
using HASS.Agent.Base.Models;
using HASS.Agent.Base.Models.Mqtt;
using MQTTnet;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Windows.AI.MachineLearning;

namespace HASS.Agent.Base.Managers;
public class CommandsManager : ICommandsManager, IMqttMessageHandler
{
    private readonly ISettingsManager _settingsManager;
    private readonly IEntityTypeRegistry _entityTypeRegistry;
    private readonly IMqttManager _mqttManager;

    private readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        Formatting = Formatting.Indented,
        ContractResolver = new DefaultContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        },
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Ignore,
    };

    private bool _discoveryPublished = false;

    public bool Pause { get; set; }
    public bool Exit { get; set; }

    public ObservableCollection<AbstractDiscoverable> Commands { get; set; } = [];

    public CommandsManager(ISettingsManager settingsManager, IEntityTypeRegistry entityTypeRegistry, IMqttManager mqttManager)
    {
        _settingsManager = settingsManager;
        _entityTypeRegistry = entityTypeRegistry;
        _mqttManager = mqttManager;
    }

    public async Task Initialize()
    {
        _settingsManager.ConfiguredCommands.CollectionChanged -= ConfiguredCommands_CollectionChanged;

        foreach (var configuredCommand in _settingsManager.ConfiguredCommands)
            await AddCommand(configuredCommand);

        _settingsManager.ConfiguredCommands.CollectionChanged += ConfiguredCommands_CollectionChanged; ;
    }

    private async Task AddCommand(ConfiguredEntity configuredCommand)
    {
        var command = (AbstractDiscoverable)_entityTypeRegistry.CreateCommandInstance(configuredCommand);
        command.ConfigureAutoDiscoveryConfig(_settingsManager.ApplicationSettings.MqttDiscoveryPrefix, _mqttManager.DeviceConfigModel);

        if (command.GetAutoDiscoveryConfig() is not MqttCommandDiscoveryConfigModel commandConfig)
            return;

        _mqttManager.RegisterMessageHandler(commandConfig.ActionTopic, this);
        _mqttManager.RegisterMessageHandler(commandConfig.CommandTopic, this);

        await PublishCommandAutoDiscoveryConfigAsync(command);
        Commands.Add(command);
    }

    private async Task RemoveCommand(AbstractDiscoverable command)
    {
        Commands.Remove(command);
        await PublishCommandStateAsync(command, respectChecks: false, clear: true);
        await PublishCommandAutoDiscoveryConfigAsync(command, clear: true);

        if (command.GetAutoDiscoveryConfig() is not MqttCommandDiscoveryConfigModel commandConfig)
            return;

        _mqttManager.UnregisterMessageHandler(commandConfig.CommandTopic);
        _mqttManager.UnregisterMessageHandler(commandConfig.ActionTopic);
    }

    private void ConfiguredCommands_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _discoveryPublished = false;

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems == null)
                    return;

                foreach (ConfiguredEntity configuredCommand in e.NewItems)
                    _ = AddCommand(configuredCommand);
                break;

            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems == null)
                    return;

                foreach (ConfiguredEntity configuredCommand in e.OldItems)
                {
                    var command = Commands.Where(s => s.UniqueId == configuredCommand.UniqueId.ToString()).FirstOrDefault();
                    if (command != null)
                        _ = RemoveCommand(command);
                }
                break;
        }
    }

    private async Task PublishCommandAutoDiscoveryConfigAsync(AbstractDiscoverable command, bool clear = false)
    {
        try
        {
            var topic = $"{_settingsManager.ApplicationSettings.MqttDiscoveryPrefix}/{command.Domain}/{_settingsManager.ApplicationSettings.DeviceName}/{command.EntityIdName}/config";

            var messageBuilder = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithRetainFlag(_settingsManager.ApplicationSettings.MqttUseRetainFlag);

            if (clear)
            {
                messageBuilder.WithPayload(Array.Empty<byte>());
            }
            else
            {
                var payload = command.GetAutoDiscoveryConfig();
                if (payload == null)
                    return;

                if (command.IgnoreAvailability)
                    payload.AvailabilityTopic = string.Empty;

                messageBuilder.WithPayload(JsonConvert.SerializeObject(payload, _jsonSerializerSettings));
            }

            await _mqttManager.PublishAsync(messageBuilder.Build());
        }
        catch (Exception e)
        {
            Log.Fatal("[COMMANDMGR] [{name}] Error publishing discovery: {err}", command, e.Message);
        }
    }

    private async Task PublishCommandStateAsync(AbstractDiscoverable command, bool respectChecks = true, bool clear = false)
    {
        try
        {
            if (respectChecks && command.LastUpdated.AddSeconds(command.UpdateIntervalSeconds) > DateTime.Now)
                return;

            var state = await command.GetState();
            if (state == null)
                return;

            var attributes = await command.GetAttributes();

            if (respectChecks &&
                command.PreviousPublishedState == state &&
                command.PreviousPublishedAttributes == attributes)
            {
                command.LastUpdated = DateTime.Now;
                return;
            }

            if (command.GetAutoDiscoveryConfig() is not MqttCommandDiscoveryConfigModel autodiscoveryConfig)
                return;

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(autodiscoveryConfig.StateTopic)
                .WithRetainFlag(_settingsManager.ApplicationSettings.MqttUseRetainFlag);

            if (clear)
                message.WithPayload(Array.Empty<byte>());
            else
                message.WithPayload(state);

            await _mqttManager.PublishAsync(message.Build());

            if (command.UseAttributes)
            {
                var attributesMessage = new MqttApplicationMessageBuilder()
                    .WithTopic(autodiscoveryConfig.JsonAttributesTopic)
                    .WithRetainFlag(_settingsManager.ApplicationSettings.MqttUseRetainFlag);

                if (clear)
                    attributesMessage.WithPayload(Array.Empty<byte>());
                else
                    attributesMessage.WithPayload(attributes);

                await _mqttManager.PublishAsync(attributesMessage.Build());
            }


            if (!respectChecks || clear)
                return;

            command.PreviousPublishedState = state;
            command.PreviousPublishedAttributes = attributes;
            command.LastUpdated = DateTime.Now;

        }
        catch (Exception e)
        {
            Log.Fatal("[COMMANDMGR] [{name}] Error publishing state: {err}", command, e.Message);
        }
    }

    public async Task PublishCommandsDiscoveryAsync(bool force = false)
    {
        if (force || !_discoveryPublished)
        {
            foreach (var command in Commands)
                await PublishCommandAutoDiscoveryConfigAsync(command);

            _discoveryPublished = true;
        }
    }
    public async Task PublishCommandsStateAsync()
    {
        foreach (var command in Commands)
        {
            if (command.Active)
                await PublishCommandStateAsync(command);
        }
    }

    public async Task UnpublishCommandsDiscoveryAsync()
    {
        foreach (var command in Commands)
            await PublishCommandAutoDiscoveryConfigAsync(command, clear: true);
    }

    public async Task Process()
    {
        var firstRun = true;
        var firstRunDone = false;

        while (!Exit)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(750)); //TODO(Amadeo): add application config for this
                if (Pause || _mqttManager.Status != MqttStatus.Connected)
                    continue;

                await PublishCommandsDiscoveryAsync();
                await PublishCommandsStateAsync();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "[COMMANDMGR] Error while processing: {err}", e.Message);
            }
        }
    }

    public void ResetAllCommandsChecks()
    {
        foreach (var command in Commands)
            command.ResetChecks();
    }

    public async Task HandleMqttMessage(MqttApplicationMessage message)
    {
        foreach (var commandDiscoverable in Commands)
        {
            if (!commandDiscoverable.Active)
                continue;

            var command = (AbstractCommand)commandDiscoverable;
            if(command.GetAutoDiscoveryConfig() is not MqttCommandDiscoveryConfigModel commandConfig)
                return;

            if (commandConfig.ActionTopic == message.Topic || commandConfig.CommandTopic == message.Topic)
            {
                var payload = message.PayloadSegment.Count > 0
                    ? Encoding.UTF8.GetString(message.PayloadSegment)
                    : string.Empty;

                if (!string.IsNullOrWhiteSpace(payload))
                    await command.TurnOn(payload);
                else
                    await command.TurnOn();
            }
        }
    }
}
