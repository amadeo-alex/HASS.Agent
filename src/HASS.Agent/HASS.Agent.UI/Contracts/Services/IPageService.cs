using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.UI.Contracts.ViewModels;
using HASS.Agent.UI.ViewModels;

namespace HASS.Agent.UI.Contracts.Services;
public interface IPageService
{
    public BindingList<IMenuItem> Pages { get; }
    public BindingList<IMenuItem> FooterPages { get; }
    IMenuItem GetMenuItem(string navigateTo);
    Type GetPageType(string key);
    void RefreshPageData();
}
