using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.UI.Contracts.ViewModels;
using HASS.Agent.UI.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Xml.Linq;
using Windows.UI.Xaml.Markup;

namespace HASS.Agent.UI.TemplateSelectors;

public class MenuItemDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate? NormalTemplate { get; set; }
    public DataTemplate? NormalBadgeTemplate { get; set; }
    public DataTemplate? SeparatorTemplate { get; set; }
    public DataTemplate? HeaderTemplate { get; set; }

    protected override DataTemplate? SelectTemplateCore(object item)
    {
        if (item is not MenuItem pageEntry)
        {
            return null;
        }

        return pageEntry.Type switch
        {
            MenuItemType.Normal => GetNormalTemplate(pageEntry),
            MenuItemType.Separator => SeparatorTemplate,
            MenuItemType.Header => HeaderTemplate,
            _ => null,
        };
    }

    private DataTemplate? GetNormalTemplate(MenuItem item)
    {
        return item.InfoBadge != null ? NormalBadgeTemplate : NormalTemplate;
    }
}
