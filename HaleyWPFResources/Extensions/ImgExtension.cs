using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using Haley.Enums;
using Haley.WPF.Controls;
using Haley.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Haley.Models;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Markup;

namespace Haley.Utils
{
    public class ImgExtension : MarkupExtension {
        private SvgKind _kind = SvgKind.brand_contao;
        private IconKind _iconKind = IconKind.empty_image;

        public ImgExtension(SvgKind kind) {
            _kind = kind;
            Preference = ImageSourcePreference.SvgKind;
        }
        public ImgExtension() { }

        [ConstructorArgument("kind")]
        public SvgKind SvgKind {
            get { return _kind; }
            set { _kind = value; }
        }

        public IconKind IconKind {
            get { return _iconKind; }
            set { _iconKind = value; }
        }

        public ImageSourcePreference Preference { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            if(Preference == ImageSourcePreference.IconKind) {
                return ResourceHelper.GetIcon(IconKind.ToString(), IconTargetType.Image); //only from svg
            }

            //Default SVG Kind
            //For the given kind, we need to fetch the image and send it back as imagesource (DrawingImage).
            return ResourceHelper.GetIcon(SvgKind.ToString(),IconTargetType.Svg); //only from svg
        }
    }
}