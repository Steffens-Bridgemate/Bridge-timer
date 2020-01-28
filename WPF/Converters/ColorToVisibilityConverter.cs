using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace BridgeTimer.Converters
{
    public class ColorToVisibilityConverter:BoolToVisibilityConverter
    {


        public Color ColorToHide
        {
            get { return (Color)GetValue(ColorToHideProperty); }
            set { SetValue(ColorToHideProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorToHide.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorToHideProperty =
            DependencyProperty.Register(nameof(ColorToHide),
                                        typeof(Color),
                                        typeof(ColorToVisibilityConverter),
                                        new PropertyMetadata(Colors.Transparent));

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Color)value == ColorToHide ? TrueValue : FalseValue;
        }
    }
}
