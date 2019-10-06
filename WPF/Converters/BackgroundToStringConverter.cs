using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace BridgeTimer
{
    public class ColorToColorConverter:ColorToValueConverter<Color>
    { }

    public class ColorToStringConverter: ColorToValueConverter<string>
    {}

    public class ColorToValueConverter<TValue> : BoolToValueConverter<TValue>
    {

        public int Threshold
        {
            get { return (int)GetValue(ThresholdProperty); }
            set { SetValue(ThresholdProperty, value); }
        }

        public static readonly DependencyProperty ThresholdProperty =
            DependencyProperty.Register(nameof(Threshold), 
                               typeof(int), 
                               typeof(ColorToValueConverter<TValue>),
                               new PropertyMetadata(defaultValue:384));


        
        public override object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            return (Threshold - color.R - color.G - color.B) > 0 ? TrueValue : FalseValue;
            //return ((Color)value).GetBrightness() < .5 ? TrueValue : FalseValue;
        }
    }
}
