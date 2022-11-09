using Haley.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Haley.Enums;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Converters;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Haley.WPF.Controls {
    public class Image : Control {

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(Image), new FrameworkPropertyMetadata(default(CornerRadius)));

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(Image), new FrameworkPropertyMetadata(_defaultBrush, propertyChangedCallback: (d, e) => ProcessChange(d, false)));

        public static readonly DependencyProperty HoverFillProperty =
            DependencyProperty.Register(nameof(HoverFill), typeof(Brush), typeof(Image), new FrameworkPropertyMetadata(null, propertyChangedCallback: ProcessHoverChange));

        public static readonly DependencyProperty RotateAngleProperty =
            DependencyProperty.Register(nameof(RotateAngle), typeof(double), typeof(Image), new PropertyMetadata(0.0));

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(ImageSource), typeof(Image), new FrameworkPropertyMetadata(ResourceHelper.GetDefaultIcon(), propertyChangedCallback: (d, e) => ProcessChange(d, true)));

        internal static readonly DependencyProperty HoverEnabledProperty =
            DependencyProperty.Register(nameof(HoverEnabled), typeof(bool), typeof(Image), new FrameworkPropertyMetadata(false));

        internal static readonly DependencyProperty HoverSourceProperty =
            DependencyProperty.Register(nameof(HoverSource), typeof(ImageSource), typeof(Image), new FrameworkPropertyMetadata(null));

        static Brush _defaultBrush = Brushes.DarkSlateGray;
        bool _change_in_progress = false;

        static Image() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Image), new FrameworkPropertyMetadata(typeof(Image)));
        }

        public Image() {}

        public CornerRadius CornerRadius {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public Brush Fill {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public Brush HoverFill {
            get { return (Brush)GetValue(HoverFillProperty); }
            set { SetValue(HoverFillProperty, value); }
        }

        public double RotateAngle {
            get { return (double)GetValue(RotateAngleProperty); }
            set { SetValue(RotateAngleProperty, value); }
        }

        public ImageSource Source {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        internal bool HoverEnabled {
            get { return (bool)GetValue(HoverEnabledProperty); }
            set { SetValue(HoverEnabledProperty, value); }
        }

        internal ImageSource HoverSource {
            get { return (ImageSource)GetValue(HoverSourceProperty); }
            set { SetValue(HoverSourceProperty, value); }
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
        }
        private static void ProcessChange(DependencyObject d, bool is_sourceChange) {
           //If image
           if (d is Image simage) {
                //If source is changed, we also need to change the hover image and its color. (because image itself got changed)
                simage.ChangeColor(false);
                if (is_sourceChange) {
                    simage.ChangeHoverColor();
                }
            }
        }

        private static void ProcessHoverChange(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            //If image
            if (d is Image simage) {
                simage.ChangeHoverColor();
            }
        }

        private void ChangeColor(bool is_hover) {
            try {
                if (_change_in_progress) return;
                _change_in_progress = true;

                if (Source is DrawingImage dimg) {
                    var converted_source = ImageUtilsInternal.ChangeDrawingColor(dimg, is_hover ? HoverFill : Fill ?? _defaultBrush);
                    if (!is_hover) {
                        SetCurrentValue(SourceProperty, converted_source);
                    } else {
                        SetCurrentValue(HoverSourceProperty, converted_source);
                    }
                } else if (Source is BitmapSource bsource) {
                    //If source is not a drawing image, then we can consider directly changing the color using Color Utils (covered in PlainImage)
                    var img_source = ImageUtilsInternal.ChangeImageColor(bsource, is_hover ? HoverFill : Fill ?? _defaultBrush);
                    if (!is_hover) {
                        SetCurrentValue(SourceProperty, img_source);
                    } else {
                        SetCurrentValue(HoverSourceProperty, img_source);
                    }
                }
                _change_in_progress = false;
            } catch {
                _change_in_progress = false; //On failure, set it to false
            }
        }

        private void ChangeHoverColor() {
            if (HoverFill == null) {
                SetCurrentValue(HoverEnabledProperty, false); //disable the hover, so that  it is not displayed via ui
                SetCurrentValue(HoverSourceProperty,null); //remove the previous hover
                return;
            }

            SetCurrentValue(HoverEnabledProperty, true);
            ChangeColor(true);
        }
    }
}
