using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

namespace HASS.Agent.UI.Contracts.Services;
public interface IThemeSelectorService
{
    void SetThemeAsync(ElementTheme theme);
}