using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using HASS.Agent.UI.Contracts.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.Extensions.DependencyInjection;
using HASS.Agent.UI.Services;
using HASS.Agent.UI.Activation;
using HASS.Agent.UI.ViewModels;
using HASS.Agent.UI.Views.Pages;
using Newtonsoft.Json;
using HASS.Agent.Base.Models.Mqtt;
using HASS.Agent.Base.Sensors.SingleValue;
using HASS.Agent.Base.Contracts.Models.Entity;
using HASS.Agent.Base.Models;
using HASS.Agent.Base.Helpers;
using HASS.Agent.Base.Contracts.Managers;
using HASS.Agent.Base.Managers;
using HASS.Agent.Base;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Xml.Linq;
using System.Diagnostics;

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
        try
        {
            /*        IEntityTypeRegistry repo = new EntityTypeRegistry();
                    repo.RegisterSensorType(typeof(DummySensor));

                    var sourceSensor = new DummySensor(uniqueId: Guid.NewGuid().ToString());
                    var sourceConfiguredEntitiy = sourceSensor.ToConfiguredEntity();

                    var serializedCE = JsonConvert.SerializeObject(sourceConfiguredEntitiy);
                    var deserializedCE = JsonConvert.DeserializeObject<ConfiguredEntity>(serializedCE) ?? throw new Exception("bruf");

                    var sensor = repo.CreateSensorInstance(deserializedCE);*/

            ConfigureServices();
            SetupLogger();

            var variableManager = GetService<IVariableManager>();
            Log.Information("[MAIN] HASS.Agent version: {version}", variableManager.ClientVersion);

            var aa = GetService<ApplicationInfo>();
            var sm = GetService<ISettingsManager>();

            var ds = new DummySensor(uniqueId: Guid.NewGuid().ToString(), updateIntervalSeconds: 2137);
            var ce = ds.ToConfiguredEntity();

            sm.ConfiguredSensors.Add(ce);
            sm.StoreConfiguredEntities();

            InitializeComponent();
        }
        catch (Exception ex)
        {
            Debugger.Break();
        }
    }

    private void SetupLogger()
    {
        var launchArguments = Environment.GetCommandLineArgs();
        var logManager = GetService<ILogManager>();
        var logger = logManager.GetLogger(launchArguments);
        Log.Logger = logger;

#if DEBUG
        logManager.ExtendedLoggingEnabled = true;
        Log.Information("[MAIN] DEBUG BUILD - TESTING PURPOSES ONLY");
        Log.Information("[MAIN] Started with arguments: {a}", launchArguments);
#endif

        if (logManager.ExtendedLoggingEnabled)
        {
            logManager.LoggingLevelSwitch.MinimumLevel = LogEventLevel.Debug;
            Log.Information("[MAIN] Extended logging enabled");

            AppDomain.CurrentDomain.FirstChanceException += logManager.OnFirstChanceExceptionHandler;
        }
    }

    private void ConfigureServices()
    {
        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            services.AddSingleton(new ApplicationInfo()
            {
                Name = Assembly.GetExecutingAssembly().GetName().Name ?? "HASS.Agent",
                Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? throw new Exception("cannot obtain application version"),
                ExecutablePath = AppDomain.CurrentDomain.BaseDirectory,
                Executable = Process.GetCurrentProcess().MainModule?.ModuleName ?? throw new Exception("cannot obtain application executable"),
            });

            services.AddSingleton<IVariableManager, VariableManager>();
            services.AddSingleton<ISettingsManager, SettingsManager>();


            services.AddSingleton<ILogManager, LogManager>();

            services.AddSingleton<IEntityTypeRegistry, EntityTypeRegistry>();
            services.AddSingleton<ISensorManager, SensorManager>();

            services.AddSingleton<AgentBase>();

            services.AddTransient<ActivationHandler<Microsoft.UI.Xaml.LaunchActivatedEventArgs>, DefaultActivationHandler>();

            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();

            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationViewService, NavigationViewService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IActivationService, ActivationService>();

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
        }).
        Build();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected async override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        //App.GetService<IAppNotificationService>().Show(string.Format("AppNotificationSamplePayload".GetLocalized(), AppContext.BaseDirectory));
        await GetService<IActivationService>().ActivateAsync(args);
    }
}
