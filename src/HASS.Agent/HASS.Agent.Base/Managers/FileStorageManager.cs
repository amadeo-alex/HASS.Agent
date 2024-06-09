using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Managers;
using Serilog;

namespace HASS.Agent.Base.Managers;
public class FileStorageManager : IFileStorageManager
{
    private readonly ISettingsManager _settingsManager;

    public FileStorageManager(ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;       
    }

    public async Task<string> GetFile(string uri)
    {
        if (string.IsNullOrWhiteSpace(uri))
        {
            Log.Warning("[FILESTORAGE] Received empty file path");
            return string.Empty;
        }

        if (uri.ToLower().StartsWith("file://"))
        {
            Log.Information("[FILESTORAGE] Received 'file://' type URI, returning as provided");
            return uri;
        }

        if (!uri.ToLower().StartsWith("http"))
        {
            Log.Error("[FILESTORAGE] Unsupported URI, only 'http/s' and 'file://' URIs are allowed, got: {uri}", uri);
            return string.Empty;
        }

        if (!Directory.Exists(_settingsManager.ApplicationSettings.))
            Directory.CreateDirectory(Variables.ImageCachePath);

        return "";
    }
}
