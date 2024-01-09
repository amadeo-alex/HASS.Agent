using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HASS.Agent.UI;
internal partial class SumBackend : ObservableObject
{
    [ObservableProperty]
    public int sumNum;
}
