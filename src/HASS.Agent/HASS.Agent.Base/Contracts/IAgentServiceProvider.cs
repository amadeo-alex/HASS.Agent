using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Base.Contracts;
public interface IAgentServiceProvider
{
    T GetAgentService<T>() where T : class;
    object GetAgentService(Type type);
}
