using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Media;

namespace BridgeTimer
{
    public class BackgroundToAssociatedForegroundConverter:BoolToValueConverter<Color>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            //return (384 - color.R - color.G - color.B) > 0 ? TrueValue:FalseValue;
            return ((Color)value).GetBrightness() < .5 ? TrueValue : FalseValue;
        }
    }
}
