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
        public ImgExtension() { 
            //IMPORTANT: AT ANY COST, DONOT SET ANY DEFAULT VALUES. DEFAULT VALUES CAN BE SET AS FALL BACK ONLY
        }

        //[ConstructorArgument("@enum")] //If we setup constructor argument, empty values will throw exception
        public BrandKind? Brand { get; set; }
        public BootStrapKind? BStrap { get; set; }
        public FAKind? FA { get; set; }

        public ImgPreference? Preference { get; set; }

        bool Process(out ImgSourceKey? source_key, out string resource_key) {
            source_key = null;
            resource_key = string.Empty;
            try {
                //Everything boils down to the preference.
                if (Preference == null) {
                    //If user has not specifically set the preference, set it as brand
                    Preference = ImgPreference.BrandKind; //Change this later based on the available value.
                }
                int loopCount = 0;
                while (!(source_key.HasValue) && string.IsNullOrWhiteSpace(resource_key)) {
                    //First ensure the values are available, else move to next
                    switch (Preference) {
                        case ImgPreference.BrandKind:
                            if (loopCount > 6) {
                                //loopcount is to ensure we do not end up in stack overflow situation
                                SetDefault();
                            }
                            //Brand is top most preference.
                            if (Brand != null) {
                                source_key = ImgSourceKey.BrandKind;
                                resource_key = Brand.ToString();
                            }
                            break;
                        case ImgPreference.FAKind:
                            if (FA == null) {
                                //redirect and loop again
                                Preference = ImgPreference.BrandKind;
                                break;
                            }
                            resource_key = FA.ToString();
                            if (resource_key.ToLower().EndsWith("solid")) {
                                source_key = ImgSourceKey.FAKind_Solid;
                            } else {
                                source_key = ImgSourceKey.FAKind_Light;
                            }
                            break;
                        case ImgPreference.BootStrapKind:
                            if (BStrap == null) {
                                //redirect and loop again
                                Preference = ImgPreference.BrandKind;
                                break;
                            }
                            source_key = ImgSourceKey.BootStrapKind;
                            resource_key = BStrap.ToString();
                            break;
                    }

                    if (source_key == null && Preference == ImgPreference.BrandKind) {
                        //Even for brand we are not able to find a matching key, so we just try to see if we have a default enum value.
                        TrySetPreference();
                    }
                    loopCount++;
                }
                if (!source_key.HasValue || string.IsNullOrWhiteSpace(resource_key)) return false;
                return true;
            } catch (Exception) {
                return false;
            }
        }

        void SetDefault() {
            Preference = ImgPreference.BrandKind;
            Brand = BrandKind.brand_haley_circle;
        }

        void TrySetPreference() {

            if (Brand != null) {
                Preference = ImgPreference.BrandKind;
            } else if (BStrap != null) {
                Preference = ImgPreference.BootStrapKind;
            } else if (FA != null) {
                Preference = ImgPreference.FAKind;
            } else {
                SetDefault();
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            if (!Process(out var @enum, out var @key)) return null;
            return ResourceHelper.GetIcon(key, @enum.Value);
        }
    }
}