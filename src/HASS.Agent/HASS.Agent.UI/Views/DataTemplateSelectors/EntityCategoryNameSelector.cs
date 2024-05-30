using HASS.Agent.Base.Models.Entity;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.UI.Views.DataTemplateSelectors;
public class EntityCategoryTemplateSelector : DataTemplateSelector
{
    public DataTemplate? CategoryTemplate { get; set; }
    public DataTemplate? EntityTemplate { get; set; }

    protected override DataTemplate? SelectTemplateCore(object item)
    {
        var entityCategory = (EntityCategory)item;
        if (entityCategory.EntityType)
            return EntityTemplate;
        else
            return CategoryTemplate;
    }
}
