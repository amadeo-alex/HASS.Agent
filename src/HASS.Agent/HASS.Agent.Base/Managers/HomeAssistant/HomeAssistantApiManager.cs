using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using HADotNet.Core;
using HADotNet.Core.Clients;
using HASS.Agent.Base.Contracts.Managers;
using Serilog;

namespace HASS.Agent.Base.Managers.HomeAssistant;
public class HomeAssistantApiManager : IHomeAssistantApiManager
{
    private readonly ISettingsManager _settingsManager;

    private ServiceClient? _serviceClient;

    public HomeAssistantApiManager(ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    public async Task InitializeAsync()
    {
        Log.Debug("[HAAPI] Initializing");

        try
        {
            var uri = new Uri(_settingsManager.Settings.HomeAssistant.HassUri);
            var httpClientHandler = new HttpClientHandler();

            if (_settingsManager.Settings.HomeAssistant.HassAutoClientCertificate)
            {
                httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Automatic;
            }
            else
            {
                //TODO(Amadeo): certificate loading error handling
                httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                httpClientHandler.ClientCertificates.Add(new X509Certificate2(_settingsManager.Settings.HomeAssistant.HassClientCertificate));
            }

            if (_settingsManager.Settings.HomeAssistant.HassAllowUntrustedCertificates)
            {
                httpClientHandler.CheckCertificateRevocationList = false;
                httpClientHandler.ServerCertificateCustomValidationCallback += (_, _, _, _) => true;
            }

            ClientFactory.Initialize(uri, _settingsManager.Settings.HomeAssistant.HassToken, httpClientHandler);

            _serviceClient = ClientFactory.GetClient<ServiceClient>();

            Log.Information("[HAAPI] Initialized");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[HAAPI] Error during initialization: {err}", ex.Message);
        }
    }

    public async Task FireEventAsync(string eventName, object eventData)
    {
        if (_serviceClient != null)
            await _serviceClient.CallService(eventName, eventData);
    }
}
