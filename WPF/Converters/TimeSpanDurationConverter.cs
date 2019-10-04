using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace BridgeTimer
{
    public class TimeSpanDurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeSpan = (TimeSpan)value;
            var change = (int)parameter;
            if (change >= 0)
                return timeSpan.Add(TimeSpan.FromSeconds(change));
            else
                return timeSpan.Subtract(TimeSpan.FromSeconds(-1 * change));

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
