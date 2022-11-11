using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Haley.Enums;
using Haley.Models;
using Abstractions.IconsPack.Haley;

namespace Haley.Utils {
    internal class IconSourceProvider : ObservableObject, IIconSourceProvider {

        Enum resource_key;
        ImgSourceKey source_key;

        public ImageSource IconSource {
            get {
                //Based on the enum value, we try to fetch the data.
                return IconFinder.GetIcon(resource_key.ToString(), source_key);
            }
        }

        public void OnDataChanged(object input) {
            //We expect the data to be in the format of Enum or string.
            object data = input; //incoming string

            if (data is string dstr) {
                //Try to change the string to enum
                do {
                    //Check brand kind
                    if (Enum.TryParse<BrandKind>(dstr, true, out var _bkind)) {
                        data = _bkind;
                        break;
                    }

                    //Check bootstrap
                    if (Enum.TryParse<BootStrapKind>(dstr, true, out var _bskind)) {
                        data = _bskind;
                        break;
                    }

                    //Check FontAwesome

                    if (Enum.TryParse<FAKind>(dstr, true, out var _fakind)) {
                        data = _fakind;
                        break;
                    }
                } while (false);
            }

            if (!(data is Enum @enum)) {
                SetDefault();
            } else {
                resource_key = @enum;
                if (@enum is BrandKind) {
                    source_key = ImgSourceKey.BrandKind;
                } else if (@enum is FAKind) {
                    
                    if (@enum.ToString().ToLower().EndsWith("light")) {
                        source_key = ImgSourceKey.FAKind_Light;
                    } else {
                        source_key = ImgSourceKey.FAKind_Solid;
                    }
                } else if (@enum is BootStrapKind) {
                    source_key = ImgSourceKey.BootStrapKind;
                } else {
                    //If enum itself is not our kind, we just reset the resource_key as well
                    SetDefault();
                }
            }

            OnPropertyChanged(nameof(IconSource));
        }

        void SetDefault() {
            resource_key = BrandKind.brand_haley_circle;
            source_key = ImgSourceKey.BrandKind;
        }

        public IconSourceProvider() { SetDefault(); }

    }
}
