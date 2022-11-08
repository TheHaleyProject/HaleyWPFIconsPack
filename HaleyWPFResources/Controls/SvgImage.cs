using Haley.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class SvgImage : Control {

        static Brush _defaultBrush = Brushes.Black;
        static SvgImage() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SvgImage), new FrameworkPropertyMetadata(typeof(SvgImage)));
        }

        public SvgImage() {}

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
        }

        public ImageSource Source {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(ImageSource), typeof(SvgImage), new FrameworkPropertyMetadata(null,propertyChangedCallback:ProcessChange));

        private static void ProcessChange(DependencyObject d, DependencyPropertyChangedEventArgs e) {
           //If image
           if (d is SvgImage simage) {
                simage.ChangeColor();
            }
        }

        private void ChangeColor() {
            if (Fill == _defaultBrush) return; //Nothing to change. What if we had a color first and then we moved to default later?
            if (Source is DrawingImage dimage) {
                DrawingImage _target = dimage;
                if (dimage.IsFrozen) {
                    _target = dimage.Clone();
                }
                if (_target.Drawing is DrawingGroup dgroup) {
                    foreach (var child in dgroup.Children) {
                        if (child is GeometryDrawing geo_dwg) {
                            if (geo_dwg.Brush.IsFrozen) {
                                geo_dwg.Brush = geo_dwg.Brush.Clone();
                            }

                            geo_dwg.SetCurrentValue(GeometryDrawing.BrushProperty, Fill);
                            //dgroup.Children[i] = _clone;
                        }
                    }
                    SetCurrentValue(SourceProperty,_target);
                }
            } else {
                //If source is not a drawing image, then we can consider directly changing the color using Color Utils (covered in PlainImage)
            }
        }

        public Brush Fill {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(SvgImage), new FrameworkPropertyMetadata(_defaultBrush, propertyChangedCallback: ProcessChange));
    }
}
