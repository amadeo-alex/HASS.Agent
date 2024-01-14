using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using Serilog.Core;

namespace HASS.Agent.Base.Contracts.Managers;

public interface ILogManager
{
    bool ExtendedLoggingEnabled { get; set; }
    LoggingLevelSwitch LoggingLevelSwitch { get; }
    Logger GetLogger(string[] arguments);
    void OnFirstChanceExceptionHandler(object? sender, FirstChanceExceptionEventArgs? eventArgs);
}
