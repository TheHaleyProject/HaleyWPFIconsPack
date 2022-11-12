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
using Haley.IconsPack.Abstractions;
using Haley.IconsPack.Utils;

namespace Haley.Utils {
    internal class IconSourceProvider : ObservableObject, IIconSourceProvider {

        string resource_key;

        public ImageSource IconSource {
            get {
                //Based on the enum value, we try to fetch the data.
                return IconFinder.GetIcon(resource_key);
            }
        }

        public void OnDataChanged(object input) {
            //We expect the data to be in the format of Enum or string.
            object data = input; //incoming string

            if (data is string dstr) {
                resource_key = dstr;  
            } else if (data is Enum @enum) {
                resource_key = @enum.ToString();
            } else {
                resource_key = BrandKind.brand_haley_circle.ToString();
            }

            OnPropertyChanged(nameof(IconSource));
        }

        void SetDefault() {
            resource_key = BrandKind.brand_haley_circle.ToString();
        }

        public IconSourceProvider() { SetDefault(); }

    }
}
