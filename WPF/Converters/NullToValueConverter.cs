using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BridgeTimer
{
    public class NullToValueConverter<TValue>:BoolToValueConverter<TValue>
    {
        public override object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? TrueValue : FalseValue;
        }
    }
}
