using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.UI.Helpers;

namespace HASS.Agent.UI.ViewModels;
public class TextInputDialogContentViewModel
{
    public string QueryTextResourceKey { get; set; } = string.Empty;
    public string QueryText => LocalizerHelper.GetLocalizedString(QueryTextResourceKey);
    public string TextBoxContent {  get; set; } = string.Empty;

}
