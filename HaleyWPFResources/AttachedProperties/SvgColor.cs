using Haley.Enums;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Collections.Generic;

namespace Haley.Models {
    public class SvgColor: DependencyObject {

        public static Brush GetColor(DependencyObject obj) {
            return (Brush)obj.GetValue(ColorProperty);
        }
        public static void SetColor(DependencyObject obj, Brush value) {
            obj.SetValue(ColorProperty, value);
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.RegisterAttached("Color", typeof(Brush), typeof(SvgColor), new FrameworkPropertyMetadata(Brushes.Purple,FrameworkPropertyMetadataOptions.AffectsRender));

    }
}
