using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Base.Contracts.Managers;
public interface IGuidManager
{
    void MarkAsUsed(Guid guid);
    void MarkAsUsed(string guid);
    Guid GenerateGuid();
    string GenerateShortGuid();
}
