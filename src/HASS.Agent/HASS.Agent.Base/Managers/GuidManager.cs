using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Base.Contracts.Managers;
using Serilog;

namespace HASS.Agent.Base.Managers;
public class GuidManager : IGuidManager
{
    private readonly List<string> _usedGuids = [];

    public void MarkAsUsed(Guid guid)
    {
        MarkAsUsed(guid.ToString());
    }
    public void MarkAsUsed(string guid)
    {
        if (string.IsNullOrWhiteSpace(guid))
            return;

        Log.Debug("[GUID] {guid} marked as used", guid);
        _usedGuids.Add(guid);
    }

    public Guid GenerateGuid()
    {
        var guid = Guid.NewGuid();
        while (_usedGuids.Contains(guid.ToString()))
            guid = Guid.NewGuid();

        _usedGuids.Add(guid.ToString());
        return guid;
    }
    public string GenerateShortGuid()
    {
        var guid = GenerateGuid().ToString()[..8];
        while (_usedGuids.Contains(guid))
            guid = GenerateGuid().ToString()[..8];

        _usedGuids.Add(guid);
        return guid;
    }
}
