using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;

namespace BridgeTimer.Converters
{
    public class BoolToVisibilityConverter:BoolToValueConverter<Visibility>
    {
        public override object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TrueValue = Visibility.Visible;
            var parameterValue = string.Empty;
            if(parameter!=null)
            {
                parameterValue = (string)parameter;
            }
            FalseValue = parameterValue.ToUpper() == "COLLAPSE" ? Visibility.Collapsed : Visibility.Hidden;
            return base.Convert(value, targetType, parameter, culture);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return base.ConvertBack(value, targetType, parameter, culture);
        }
    }
}
