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
        private BrandKind _brand_kind = BrandKind.brand_haley_circle;

        public ImgExtension(BrandKind kind) {
            _brand_kind = kind;
            Preference = ImgSource.BrandKind;
        }
        public ImgExtension() { }

        [ConstructorArgument("kind")]
        public BrandKind BrandKind {
            get { return _brand_kind; }
            set { _brand_kind = value; }
        }

        public BootStrapKind BSKind { get; set; }
        public FAKind FAKind { get; set; }

        public ImgSource Preference { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            switch (Preference) {
                case ImgSource.BrandKind:
                    break;
                case ImgSource.FAKind:
                    break;
                case ImgSource.BootStrapKind:
                    break;
                default:
                    break;
            }
            if(Preference == ImgSource.IconKind) {
                return ResourceHelper.GetIcon(IconKind.ToString(), IconTargetType.Image); //only from svg
            }

            //Default SVG Kind
            //For the given kind, we need to fetch the image and send it back as imagesource (DrawingImage).
            return ResourceHelper.GetIcon(SvgKind.ToString(),IconTargetType.Svg); //only from svg
        }
    }
}