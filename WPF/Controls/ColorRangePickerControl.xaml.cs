﻿using System;
using System.Collections.Generic;
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
    /// Interaction logic for ColorRangePickerControl.xaml
    /// </summary>
    public partial class ColorRangePickerControl : UserControl
    {
        public ColorRangePickerControl()
        {
            InitializeComponent();
        }



        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label),
                                        typeof(string), 
                                        typeof(ColorRangePickerControl), 
                                        new PropertyMetadata(string.Empty));


        public List<Color> Colors
        {
            get { return (List<Color>)GetValue(ColorsProperty); }
            set { SetValue(ColorsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Colors.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorsProperty =
            DependencyProperty.Register(nameof(Colors), 
                                        typeof(List<Color>), 
                                        typeof(ColorRangePickerControl), 
                                        new PropertyMetadata(new List<Color>()));


        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(nameof(SelectedColor), 
                                        typeof(Color),
                                        typeof(ColorRangePickerControl),
                                        new PropertyMetadata(System.Windows.Media.Colors.White));

    }
}
