using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Managers;

namespace HASS.Agent.Base.Managers;
internal class HomeAssistantApiManager : IHomeAssistantApiManager
{
    private readonly ISettingsManager _settingsManager;

    public HomeAssistantApiManager(ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;

    }

    public Task InitializeAsync()
    {

    }
    public Task FireEventAsync()
    {
    }
}
