using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.UI.Contracts;
public interface IActivationHandler
{
    bool CanHandle(object args);
    Task HandleAsync(object args);
}
