﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore.CoreAudioAPI;
using Serilog;

namespace HASS.Agent.Shared.Managers.Audio.Internal;
internal class InternalAudioDevice : IDisposable
{
    private bool _disposed;

    public MMDevice MMDevice { get; private set; }
    public AudioEndpointVolume AudioEndpointVolume { get; private set; }
    public InternalAudioSessionManager Manager { get; private set; }

    public string DeviceId { get; private set; }
    public string FriendlyName { get; private set; }
    public bool Reinitialized { get; private set; }

    public InternalAudioDevice(MMDevice device)
    {
        MMDevice = device;
        var sessionManager2 = AudioSessionManager2.FromMMDevice(device);
        Manager = new InternalAudioSessionManager(sessionManager2);
        AudioEndpointVolume = AudioEndpointVolume.FromDevice(device);

        DeviceId = device.DeviceID;
        FriendlyName = device.FriendlyName;
    }

    public void Activate()
    {
        using var configClient = new CPolicyConfigVistaClient();
        configClient.SetDefaultDevice(MMDevice.DeviceID);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        try
        {
            AudioEndpointVolume?.Dispose();
            Manager?.Dispose();
            MMDevice?.Dispose();
        }
        catch (Exception ex)
        {
            Log.Debug(ex, "[AUDIOMGR] Exception disposing device '{name}': {msg}", FriendlyName, ex.Message);
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }

    ~InternalAudioDevice()
    {
        Dispose();
    }
}