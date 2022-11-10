using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Haley.Enums;
using Isolated.Haley.WpfIconPack;

namespace Haley.WPF.Models {
    internal class IconSourceProvider : ObservableObject {

        Enum resource_key;
        ImgSourceKey source_key;

        public ImageSource IconSource {
            get {
                //Based on the enum value, we try to fetch the data.
                return IconFinder.GetIcon(resource_key.ToString(), source_key);
            }
        }

        public void OnDataChanged(object data) {
            //We expect the data to be in the format of Enum

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
