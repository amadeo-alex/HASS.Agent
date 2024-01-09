using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;
using HASS.Agent.UI.Contracts.ViewModels;

namespace HASS.Agent.UI.ValueConverters;
public class InfoBadgeTypeToStyleConverter : IValueConverter
{
    public Style AttentionStyle { get; set; }
    public Style InformationStyle { get; set; }
    public Style SuccessStyle { get; set; }
    public Style CriticalStyle { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not InfoBadgeType type)
        {
            throw new InvalidOperationException("provided value is not InfoBadgeType");
        }
        
        return type switch
        {
            InfoBadgeType.Attention => AttentionStyle,
            InfoBadgeType.Information => InformationStyle,
            InfoBadgeType.Success => SuccessStyle,
            InfoBadgeType.Critical => CriticalStyle,
            _ => throw new InvalidOperationException($"conversion for type '{type}' not implemented")
        };
    }

    public object Convert(object value, Type targetType, object parameter, string language) => Convert(value, targetType, parameter, CultureInfo.InvariantCulture);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return new NotImplementedException("conversion from style to type not supported");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return new NotImplementedException("conversion from style to type not supported");
    }
}