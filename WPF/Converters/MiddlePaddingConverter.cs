using BridgeTimer.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace BridgeTimer
{
    public class MiddlePaddingConverter :DependencyObject, IValueConverter
    {


        public int DesiredTotalLength
        {
            get { return (int)GetValue(DesiredTotalLengthProperty); }
            set { SetValue(DesiredTotalLengthProperty, value); }
        }

        public static readonly DependencyProperty DesiredTotalLengthProperty =
            DependencyProperty.Register(nameof(DesiredTotalLength),
                                        typeof(int),
                                        typeof(MiddlePaddingConverter),
                                        new PropertyMetadata(defaultValue: 0));



        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value as string;
            if (text == null) return string.Empty;

            return text.PadLeftAndRight(DesiredTotalLength);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
