using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;
using HASS.Agent.UI.Contracts.ViewModels;
using WinUI3Localizer;

namespace HASS.Agent.UI.ValueConverters;
public class CategoryDisplayNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var category = (string)value;
        var template = (string)parameter;
        var resourceKey = template.Replace("{}", category);
        return Localizer.Get().GetLocalizedString(resourceKey);
    }

    public object Convert(object value, Type targetType, object parameter, string language) => Convert(value, targetType, parameter, CultureInfo.InvariantCulture);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return new NotImplementedException("conversion from display name to category not supported");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return new NotImplementedException("conversion from display name to category not supported");
    }
}