using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BridgeTimer.WPF.Controls
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        public ColorPicker()
        {
            InitializeComponent();
            var blackColors = new[] {Colors.Black, Colors.LightGray, Colors.Gray, Colors.DarkGray,Colors.Navy,Colors.MidnightBlue,Colors.Transparent };
            var whiteColors = new[] {Colors.White,Colors.GhostWhite,Colors.MintCream, Colors.Transparent };
            var redColors = new[] {Colors.Red, Colors.OrangeRed, Colors.Tomato,Colors.Crimson, Colors.Transparent };
            var purpleColors = new[] {Colors.Purple, Colors.MediumPurple, Colors.Plum,Colors.Pink, Colors.Transparent };
            var blueColors = new[] {Colors.Blue, Colors.LightBlue,Colors.DarkBlue,Colors.LightSkyBlue,Colors.SkyBlue,Colors.DeepSkyBlue, Colors.Transparent };
            var greenColors = new[] {Colors.Green, Colors.LightGreen,Colors.DarkGreen,Colors.YellowGreen , Colors.Olive, Colors.Transparent };
            var yellowColors = new[] {Colors.Yellow, Colors.LightYellow, Colors.Gold,Colors.LightGoldenrodYellow,Colors.Goldenrod,Colors.DarkGoldenrod, Colors.Transparent };
            var orangeColors = new[] {Colors.Orange,Colors.DarkOrange,Colors.SandyBrown, Colors.Brown,Colors.SaddleBrown, Colors.Transparent };

            this.RedColors = redColors.ToList();
            this.PurpleColors = purpleColors.ToList();
            this.BlueColors = blueColors.ToList();
            this.GreenColors = greenColors.ToList();
            this.YellowColors = yellowColors.ToList();
            this.OrangeColors = orangeColors.ToList();
            this.BlackColors = blackColors.ToList();
            this.WhiteColors = whiteColors.ToList();
        }



        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(nameof(SelectedColor), 
                                        typeof(Color), 
                                        typeof(ColorPicker), 
                                        new PropertyMetadata(defaultValue:Colors.White,
                                                             propertyChangedCallback:OnSelectedColorChanged));

        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ColorPicker;
            if (control == null) return;

            if (e.OldValue == e.NewValue) return;
            control.tbColorPicker.IsChecked = false;
        }

        public List<Color> RedColors
        {
            get { return (List<Color>)GetValue(RedColorsProperty); }
            set { SetValue(RedColorsProperty, value); }
        }

        public static readonly DependencyProperty RedColorsProperty =
            DependencyProperty.Register(nameof(RedColors), 
                                        typeof(List<Color>),
                                        typeof(ColorPicker),
                                        new PropertyMetadata(new List<Color>()));


        public Color SelectedRedColor
        {
            get { return (Color)GetValue(SelectedRedColorProperty); }
            set { SetValue(SelectedRedColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedRedColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedRedColorProperty =
            DependencyProperty.Register(nameof(SelectedRedColor), 
                                        typeof(Color), 
                                        typeof(ColorPicker),
                                        new PropertyMetadata(defaultValue: Colors.Transparent,
                                            propertyChangedCallback: OnColorChanged));

        public List<Color> PurpleColors
        {
            get { return (List<Color>)GetValue(PurpleColorsProperty); }
            set { SetValue(PurpleColorsProperty, value); }
        }

        public static readonly DependencyProperty PurpleColorsProperty =
            DependencyProperty.Register(nameof(PurpleColors),
                                        typeof(List<Color>),
                                        typeof(ColorPicker),
                                        new PropertyMetadata(new List<Color>()));

        public Color SelectedPurpleColor
        {
            get { return (Color)GetValue(SelectedPurpleColorProperty); }
            set { SetValue(SelectedPurpleColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedPurpleColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedPurpleColorProperty =
            DependencyProperty.Register(nameof(SelectedPurpleColor),
                                        typeof(Color),
                                        typeof(ColorPicker),
                                        new PropertyMetadata(defaultValue: Colors.Transparent,
                                            propertyChangedCallback: OnColorChanged));
        public List<Color> BlueColors
        {
            get { return (List<Color>)GetValue(BlueColorsProperty); }
            set { SetValue(BlueColorsProperty, value); }
        }

        public static readonly DependencyProperty BlueColorsProperty =
            DependencyProperty.Register(nameof(BlueColors),
                                        typeof(List<Color>),
                                        typeof(ColorPicker),
                                        new PropertyMetadata(new List<Color>()));

        public Color SelectedBlueColor
        {
            get { return (Color)GetValue(SelectedBlueColorProperty); }
            set { SetValue(SelectedBlueColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedBlueColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedBlueColorProperty =
            DependencyProperty.Register(nameof(SelectedBlueColor),
                                        typeof(Color),
                                        typeof(ColorPicker),
                                        new PropertyMetadata(defaultValue: Colors.Transparent,
                                            propertyChangedCallback: OnColorChanged));
        public List<Color> GreenColors
        {
            get { return (List<Color>)GetValue(GreenColorsProperty); }
            set { SetValue(GreenColorsProperty, value); }
        }

        public static readonly DependencyProperty GreenColorsProperty =
            DependencyProperty.Register(nameof(GreenColors),
                                        typeof(List<Color>),
                                        typeof(ColorPicker),
                                        new PropertyMetadata(new List<Color>()));

        public Color SelectedGreenColor
        {
            get { return (Color)GetValue(SelectedGreenColorProperty); }
            set { SetValue(SelectedGreenColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedGreenColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedGreenColorProperty =
            DependencyProperty.Register(nameof(SelectedGreenColor),
                                        typeof(Color),
                                        typeof(ColorPicker),
                                        new PropertyMetadata(defaultValue: Colors.Transparent,
                                            propertyChangedCallback: OnColorChanged));
        public List<Color> YellowColors
        {
            get { return (List<Color>)GetValue(YellowColorsProperty); }
            set { SetValue(YellowColorsProperty, value); }
        }

        public static readonly DependencyProperty YellowColorsProperty =
            DependencyProperty.Register(nameof(YellowColors),
                                        typeof(List<Color>),
                                        typeof(ColorPicker),
                                        new PropertyMetadata(new List<Color>()));

        public Color SelectedYellowColor
        {
            get { return (Color)GetValue(SelectedYellowColorProperty); }
            set { SetValue(SelectedYellowColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedYellowColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedYellowColorProperty =
            DependencyProperty.Register(nameof(SelectedYellowColor),
                                        typeof(Color),
                                        typeof(ColorPicker),
                                        new PropertyMetadata(defaultValue: Colors.Transparent,
                                                             propertyChangedCallback:OnColorChanged));
        public List<Color> OrangeColors
        {
            get { return (List<Color>)GetValue(OrangeColorsProperty); }
            set { SetValue(OrangeColorsProperty, value); }
        }

        public static readonly DependencyProperty OrangeColorsProperty =
            DependencyProperty.Register(nameof(OrangeColors),
                                        typeof(List<Color>),
                                        typeof(ColorPicker),
                                        new PropertyMetadata(new List<Color>()));

        public Color SelectedOrangeColor
        {
            get { return (Color)GetValue(SelectedOrangeColorProperty); }
            set { SetValue(SelectedOrangeColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedOrangeColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedOrangeColorProperty =
            DependencyProperty.Register(nameof(SelectedOrangeColor),
                                        typeof(Color),
                                        typeof(ColorPicker),
                                        new PropertyMetadata(defaultValue: Colors.Transparent,
                                            propertyChangedCallback: OnColorChanged));
        public List<Color> WhiteColors
        {
            get { return (List<Color>)GetValue(WhiteColorsProperty); }
            set { SetValue(WhiteColorsProperty, value); }
        }

        public static readonly DependencyProperty WhiteColorsProperty =
            DependencyProperty.Register(nameof(WhiteColors),
                                        typeof(List<Color>),
                                        typeof(ColorPicker),
                                        new PropertyMetadata(new List<Color>()));

        public Color SelectedWhiteColor
        {
            get { return (Color)GetValue(SelectedWhiteColorProperty); }
            set { SetValue(SelectedWhiteColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedWhiteColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedWhiteColorProperty =
            DependencyProperty.Register(nameof(SelectedWhiteColor),
                                        typeof(Color),
                                        typeof(ColorPicker),
                                        new PropertyMetadata(defaultValue: Colors.Transparent,
                                            propertyChangedCallback: OnColorChanged));

        public List<Color> BlackColors
        {
            get { return (List<Color>)GetValue(BlackColorsProperty); }
            set { SetValue(BlackColorsProperty, value); }
        }

        public static readonly DependencyProperty BlackColorsProperty =
            DependencyProperty.Register(nameof(BlackColors),
                                        typeof(List<Color>),
                                        typeof(ColorPicker),
                                        new PropertyMetadata(new List<Color>()));

        public Color SelectedBlackColor
        {
            get { return (Color)GetValue(SelectedBlackColorProperty); }
            set { SetValue(SelectedBlackColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedBlackColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedBlackColorProperty =
            DependencyProperty.Register(nameof(SelectedBlackColor),
                                        typeof(Color),
                                        typeof(ColorPicker),
                                        new PropertyMetadata(defaultValue: Colors.Transparent,
                                                             propertyChangedCallback: OnColorChanged));

        #region Update selected color

        private bool _isUpdating;

        private void UpdateSelectedColor(Color updatedColor)
        {
            _isUpdating = true;
            SelectedColor = updatedColor;
            if (SelectedRedColor != updatedColor)
                SelectedRedColor=Colors.Transparent;
            if (SelectedPurpleColor != updatedColor)
                SelectedPurpleColor = Colors.Transparent;
            if (SelectedBlueColor != updatedColor)
                SelectedBlueColor = Colors.Transparent;
            if (SelectedGreenColor != updatedColor)
                SelectedGreenColor = Colors.Transparent;
            if (SelectedYellowColor != updatedColor)
                SelectedYellowColor = Colors.Transparent;
            if (SelectedOrangeColor != updatedColor)
                SelectedOrangeColor = Colors.Transparent;
            if (SelectedWhiteColor != updatedColor)
                SelectedWhiteColor = Colors.Transparent;
            if (SelectedBlackColor != updatedColor)
                SelectedBlackColor = Colors.Transparent;
            _isUpdating = false;
        }

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control=d as ColorPicker;
            if (control == null) return;
            if (control._isUpdating) return;
            if (e.NewValue == null) return;
            control.UpdateSelectedColor((Color) e.NewValue);
         



        }
        #endregion
    }
}
