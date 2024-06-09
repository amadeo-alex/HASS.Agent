using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Base.Contracts.Managers;
public interface IFileStorageManager
{
    Task<string> GetFile(string path);
}
