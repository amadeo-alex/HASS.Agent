using System;
using HASS.Agent.UI.Contracts.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using HASS.Agent.UI.Services;
using HASS.Agent.UI.Activation;
using HASS.Agent.UI.ViewModels;
using HASS.Agent.UI.Views.Pages;
using HASS.Agent.Base.Models;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.Base.Managers;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Diagnostics;
using HASS.Agent.Base.Contracts;
using System.Threading.Tasks;
using MQTTnet;
using HASS.Agent.Base.Sensors.SingleValue;
using System.Globalization;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using System.Xml.Linq;
using System.Xml;
using HASS.Agent.Base.Models.Entity;
using Windows.Storage;
using WinUI3Localizer;
using System.IO;
using HASS.Agent.UI.Contracts.Managers;
using HASS.Agent.UI.Managers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HASS.Agent.UI;
/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    public IHost Host { get; private set; }

    public static T GetService<T>() where T : class
    {
        return (Current as App)!.Host.Services.GetService(typeof(T)) is not T service
            ? throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.")
            : service;
    }

    public static object GetService(Type type)
    {
        var service = (Current as App)!.Host.Services.GetService(type);
        return service ?? throw new ArgumentException($"{type} needs to be registered in ConfigureServices within App.xaml.cs.");
    }

    public T GetAgentService<T>() where T : class => GetService<T>();
    public object GetAgentService(Type type) => GetService(type);

    public static UIElement? AppTitlebar
    {
        get; set;
    }
    public static WindowEx MainWindow { get; } = new MainWindow();

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        Host = ConfigureServices();
        SetupLogger();

        InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected async override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        await InitializeLocalizer();

        try
        {
            var variableManager = GetService<IVariableManager>();
            Log.Information("[MAIN] HASS.Agent version: {version}", variableManager.ClientVersion);

            InitializeComponent();

            var initializationTask = Task.Run(async () =>
            {
                var settingsManager = GetService<ISettingsManager>();
                var guidManager = GetService<IGuidManager>();
                guidManager.MarkAsUsed(settingsManager.Settings.Mqtt.ClientId);

                if (settingsManager.ConfiguredSensors.Count == 0)
                {
                    var ce = new ConfiguredEntity()
                    {
                        Type = typeof(DummySensor).Name,
                        EntityIdName = "DummySensor1",
                        Name = "Dummy Sensor 1",
                        UpdateIntervalSeconds = 5,
                        UniqueId = Guid.NewGuid(),
                        Active = true,
                    };
                    settingsManager.ConfiguredSensors.Add(ce);
                }

                var mqtt = GetService<IMqttManager>();
                await mqtt.StartClient();

                var sensorManager = GetService<ISensorManager>();
                await sensorManager.Initialize();
                _ = Task.Run(sensorManager.Process);

                var commandsManager = GetService<ICommandsManager>();
                await commandsManager.Initialize();
                _ = Task.Run(commandsManager.Process);

                var notificationManager = GetService<INotificationManager>();
                notificationManager.Initialize();

                await Task.Run(async () =>
                {
                    while (!mqtt.Ready)
                        await Task.Delay(1000);

                    var testMsg = new MqttApplicationMessageBuilder()
                        .WithTopic("sumtest/sumimportantmsg")
                        .WithPayload("much importando")
                        .WithRetainFlag(false)
                        .Build();

                    await mqtt.PublishAsync(testMsg);
                });

                Log.Debug("[MAIN] initialization completed");
            });


            Log.Debug("[MAIN] attempting to display main window");
        }
        catch (Exception ex)
        {
            Debugger.Break();
        }

        //App.GetService<IAppNotificationService>().Show(string.Format("AppNotificationSamplePayload".GetLocalized(), AppContext.BaseDirectory));
        await GetService<IActivationService>().ActivateAsync(args);
    }

    private async Task InitializeLocalizer()
    {
        var stringsFolderPath = Path.Combine(AppContext.BaseDirectory, "Strings");
        var stringsFolder = await StorageFolder.GetFolderFromPathAsync(stringsFolderPath);

        ILocalizer localizer = await new LocalizerBuilder()
            .AddStringResourcesFolderForLanguageDictionaries(stringsFolderPath)
            .SetOptions(options =>
            {
                options.DefaultLanguage = "en-US";
            })
            .Build();
    }
    private void SetupLogger()
    {
        var launchArguments = Environment.GetCommandLineArgs();
        var logManager = GetService<ILogManager>();
        var logger = logManager.GetLogger(launchArguments);
        Log.Logger = logger;

        Log.Information("----------------------------------------------------------------");
        Log.Information("[MAIN] HASS.Agent started");

#if DEBUG
        logManager.ExtendedLoggingEnabled = true;
        Log.Information("[MAIN] DEBUG BUILD - TESTING PURPOSES ONLY");
#endif

        if (logManager.ExtendedLoggingEnabled)
        {
            logManager.LoggingLevelSwitch.MinimumLevel = LogEventLevel.Debug;
            Log.Debug("[MAIN] Extended logging enabled");
            Log.Debug("[MAIN] Started with arguments: {a}", launchArguments);

            AppDomain.CurrentDomain.FirstChanceException += logManager.OnFirstChanceExceptionHandler;
        }
    }

    private IHost ConfigureServices()
    {
        var uiDispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

        var host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder().UseContentRoot(AppContext.BaseDirectory).
            ConfigureServices((context, services) =>
            {
                services.AddSingleton<IGuidManager, GuidManager>();

                services.AddSingleton(new ApplicationInfo()
                {
                    Name = Assembly.GetExecutingAssembly().GetName().Name ?? "HASS.Agent",
                    Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? throw new Exception("cannot obtain application version"),
                    ExecutablePath = AppDomain.CurrentDomain.BaseDirectory,
                    Executable = Process.GetCurrentProcess().MainModule?.ModuleName ?? throw new Exception("cannot obtain application executable"),
                });

                services.AddSingleton<IVariableManager, VariableManager>();
                services.AddSingleton<ILogManager, LogManager>();

                services.AddSingleton<IGuidManager, GuidManager>();
                services.AddSingleton<ISettingsManager, SettingsManager>();

                services.AddSingleton<IMqttManager, MqttManager>();

                services.AddSingleton<IEntityTypeRegistry, EntityTypeRegistry>();
                services.AddSingleton<ISensorManager, SensorManager>();
                services.AddSingleton<ICommandsManager, CommandsManager>();

                services.AddSingleton<INotificationManager, NotificationManager>();

                services.AddTransient<ActivationHandler<Microsoft.UI.Xaml.LaunchActivatedEventArgs>, DefaultActivationHandler>();

                services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();

                services.AddSingleton<IPageService, PageService>();
                services.AddSingleton<INavigationViewService, NavigationViewService>();
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<IActivationService, ActivationService>();

                services.AddSingleton<IEntityUiTypeRegistry, EntityUiTypeRegistry>();

                services.AddTransient<ShellPageViewModel>();
                services.AddTransient<ShellPage>();

                services.AddTransient<MainPageViewModel>();
                services.AddTransient<MainPage>();

                services.AddTransient<SensorsPageViewModel>();
                services.AddTransient<SensorsPage>();

                services.AddTransient<CommandsPageViewModel>();
                services.AddTransient<CommandsPage>();

                services.AddTransient<SettingsPageViewModel>();
                services.AddTransient<SettingsPage>();

                services.AddSingleton(uiDispatcherQueue);

                services.AddSingleton(sp => sp);
            })
            .Build();

        return host;
    }
}
