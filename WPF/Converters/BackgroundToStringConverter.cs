using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace BridgeTimer
{
    public class BackgroundToStringConverter : BoolToValueConverter<string>
    {


        public int Threshold
        {
            get { return (int)GetValue(ThresholdProperty); }
            set { SetValue(ThresholdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Threshold.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThresholdProperty =
            DependencyProperty.Register(nameof(Threshold), 
                               typeof(int), 
                               typeof(BackgroundToStringConverter),
                               new PropertyMetadata(defaultValue:384));


        
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            return (Threshold - color.R - color.G - color.B) > 0 ? TrueValue : FalseValue;
            //return ((Color)value).GetBrightness() < .5 ? TrueValue : FalseValue;
        }
    }
}
