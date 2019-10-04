﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace BridgeTimer
{
    public abstract class EnumToValueConverter<TEnum,TValue>  :DependencyObject, 
        IValueConverter where TEnum : struct, IConvertible
    {

        public EnumToValueConverter()
        {
            Conversions = new Dictionary<TEnum, TValue>();
        }

        public Dictionary<TEnum,TValue>  Conversions
        {
            get { return (Dictionary<TEnum,TValue> )GetValue(ConversionsProperty); }
            set { SetValue(ConversionsProperty, value); }
        }

        public static readonly DependencyProperty ConversionsProperty =
            DependencyProperty.Register(nameof(Conversions), 
                                        typeof(Dictionary<TEnum,TValue> ), typeof(EnumToValueConverter<TEnum,TValue>), 
                                        new PropertyMetadata(defaultValue:new Dictionary<TEnum,TValue>()));


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException($"The value is not an enum.");
            
            var key = (TEnum) value;

            return Conversions.SafeGet(key);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
