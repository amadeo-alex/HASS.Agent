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

    public void Initialize()
    {
        _settingsManager.ConfiguredCommands.CollectionChanged -= ConfiguredCommands_CollectionChanged;

        foreach (var configuredCommand in _settingsManager.ConfiguredCommands)
            AddCommand(configuredCommand);

        _settingsManager.ConfiguredCommands.CollectionChanged += ConfiguredCommands_CollectionChanged; ;
    }

    private void AddCommand(ConfiguredEntity configuredCommand)
    {
        var command = (AbstractDiscoverable)_entityTypeRegistry.CreateCommandInstance(configuredCommand);
        command.ConfigureAutoDiscoveryConfig(_settingsManager.ApplicationSettings.MqttDiscoveryPrefix, _mqttManager.DeviceConfigModel);

        var commandConfig = (MqttCommandDiscoveryConfigModel)command.GetAutoDiscoveryConfig();
        _mqttManager.RegisterMessageHandler(commandConfig.ActionTopic, this);
        _mqttManager.RegisterMessageHandler(commandConfig.CommandTopic, this);

        _ = PublishCommandAutoDiscoveryConfigAsync(command);
        Commands.Add(command);
    }

    private void RemoveCommand(AbstractDiscoverable command)
    {
        Commands.Remove(command);
        _ = PublishCommandAutoDiscoveryConfigAsync(command, clear: true);

        var commandConfig = (MqttCommandDiscoveryConfigModel)command.GetAutoDiscoveryConfig();
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
                    AddCommand(configuredCommand);
                break;

            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems == null)
                    return;

                foreach (ConfiguredEntity configuredCommand in e.OldItems)
                {
                    var command = Commands.Where(s => s.UniqueId == configuredCommand.UniqueId.ToString()).FirstOrDefault();
                    if (command != null)
                        RemoveCommand(command);
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

    private async Task PublishCommandStateAsync(AbstractDiscoverable command, bool respectChecks = true)
    {
        try
        {
            if (respectChecks)
            {
                if (command.LastUpdated.AddSeconds(command.UpdateIntervalSeconds) > DateTime.Now)
                    return;

                var state = command.State;
                if (state == null)
                    return;

                var attributes = command.Attributes;

                if (respectChecks)
                {
                    if (command.PreviousPublishedState == state && command.PreviousPublishedAttributes == attributes)
                    {
                        command.LastUpdated = DateTime.Now;
                        return;
                    }
                }

                var autodiscoveryConfig = (MqttCommandDiscoveryConfigModel)command.GetAutoDiscoveryConfig();
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(autodiscoveryConfig.StateTopic)
                    .WithPayload(state)
                    .WithRetainFlag(_settingsManager.ApplicationSettings.MqttUseRetainFlag)
                    .Build();

                await _mqttManager.PublishAsync(message);

                if (command.UseAttributes)
                {
                    var attributesMessage = new MqttApplicationMessageBuilder()
                        .WithTopic(autodiscoveryConfig.JsonAttributesTopic)
                        .WithPayload(attributes)
                        .WithRetainFlag(_settingsManager.ApplicationSettings.MqttUseRetainFlag)
                        .Build();

                    await _mqttManager.PublishAsync(attributesMessage);
                }


                if (!respectChecks)
                    return;

                command.PreviousPublishedState = state;
                command.PreviousPublishedAttributes = attributes;
                command.LastUpdated = DateTime.Now;
            }
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
            await PublishCommandStateAsync(command);
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

    public Task HandleMqttMessage(MqttApplicationMessage message)
    {
        foreach (var commandDiscoverable in Commands)
        {
            var command = (AbstractCommand)commandDiscoverable;
            var commandConfig = (MqttCommandDiscoveryConfigModel)command.GetAutoDiscoveryConfig();

            if (commandConfig.ActionTopic == message.Topic || commandConfig.CommandTopic == message.Topic)
            {
                var payload = message.PayloadSegment.Count > 0
                    ? Encoding.UTF8.GetString(message.PayloadSegment)
                    : string.Empty;

                if (!string.IsNullOrWhiteSpace(payload))
                    command.TurnOn(payload);
                else
                    command.TurnOn();
            }
        }

        return Task.CompletedTask;
    }
}
