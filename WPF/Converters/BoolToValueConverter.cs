using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace BridgeTimer
{
    public class BoolToValueConverter<TValue> :DependencyObject, IValueConverter 
    {

        public TValue TrueValue
        {
            get { return (TValue)GetValue(TrueValueProperty); }
            set { SetValue(TrueValueProperty, value); }
        }

        public static readonly DependencyProperty TrueValueProperty =
            DependencyProperty.Register(nameof(TrueValue),
                                        typeof(TValue),
                                        typeof(BoolToValueConverter<TValue>),
                                        new PropertyMetadata(default(TValue)!));

        public TValue FalseValue
        {
            get { return (TValue)GetValue(FalseValueProperty); }
            set { SetValue(FalseValueProperty, value); }
        }

        public static readonly DependencyProperty FalseValueProperty =
            DependencyProperty.Register(nameof(FalseValue),
                                        typeof(TValue),
                                        typeof(BoolToValueConverter<TValue>),
                                        new PropertyMetadata(default(TValue)!));


        public virtual object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var booleanValue = (bool)value;
            return booleanValue ? TrueValue : FalseValue;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
