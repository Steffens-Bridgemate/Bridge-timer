using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace BridgeTimer
{
    public class ThresholdToColorConverter:ThresholdConverter<Color>
    { }

    public class ThresholdToBrushConverter : ThresholdConverter<SolidColorBrush>
    { }

    public class ThresholdConverter<TValue>:EnumToValueConverter<CountDownTimer.ThresholdReached,TValue>
    {
        public TValue RoundStartedValue
        {
            get { return (TValue)GetValue(RoundStartedValueProperty); }
            set { SetValue(RoundStartedValueProperty, value); }
        }

        public static readonly DependencyProperty RoundStartedValueProperty =
            DependencyProperty.Register(nameof(RoundStartedValue),
                typeof(TValue),
                typeof(ThresholdConverter<TValue>),
                new PropertyMetadata(defaultValue: default(TValue),
                                     propertyChangedCallback: OnRoundStartedValueChanged));

        private static void OnRoundStartedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as ThresholdConverter<TValue>;
            if (dp == null) return;

            dp.Conversions.AddOrEdit(CountDownTimer.ThresholdReached.RoundStarted,dp.RoundStartedValue);

        }



        public TValue WarningGivenValue
        {
            get { return (TValue)GetValue(WarningGivenValueProperty); }
            set { SetValue(WarningGivenValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WarningGivenValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WarningGivenValueProperty =
            DependencyProperty.Register(nameof(WarningGivenValue),
                                        typeof(TValue), 
                                        typeof(ThresholdConverter<TValue>), 
                                        new PropertyMetadata(defaultValue:default(TValue),
                                                             propertyChangedCallback:OnWarningGivenValueChanged));



        private static void OnWarningGivenValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as ThresholdConverter<TValue>;
            if (dp == null) return;

            dp.Conversions.AddOrEdit(CountDownTimer.ThresholdReached.EndOfRoundWarning, dp.WarningGivenValue);

        }

        public TValue RoundEndedValue
        {
            get { return (TValue)GetValue(RoundEndedValueProperty); }
            set { SetValue(RoundEndedValueProperty, value); }
        }

        public static readonly DependencyProperty RoundEndedValueProperty =
            DependencyProperty.Register(nameof(RoundEndedValue),
                typeof(TValue),
                typeof(ThresholdConverter<TValue>),
                new PropertyMetadata(defaultValue:default(TValue),
                                     propertyChangedCallback: OnRoundEndedValueChanged));

        private static void OnRoundEndedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as ThresholdConverter<TValue>;
            if (dp == null) return;

            dp.Conversions.AddOrEdit(CountDownTimer.ThresholdReached.RoundEnded, dp.RoundEndedValue);

        }


    }
}
