using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haley.Enums;
using System.Windows.Media;
using Haley.Utils;

namespace Isolated.Haley.WpfIconPack {
    public static class IconFinder {
        static bool _initialized = false;
        const string PACK_PATH = "pack://application:,,,/Haley.WPF.IconsPack;component/Dictionaries/Icons/";
        static object sourceLock = new object();
        static void EnsureInitialized() {
            if (_initialized) return;

            lock (sourceLock) {
                if (_initialized) return;
                //try to initialize
                ResourceFetcher.AddSource(ImgSourceKey.BootStrapKind, new Uri($@"{PACK_PATH}internalBootStrap.xaml", UriKind.RelativeOrAbsolute));
                ResourceFetcher.AddSource(ImgSourceKey.BrandKind, new Uri($@"{PACK_PATH}internalBranded.xaml", UriKind.RelativeOrAbsolute));
                ResourceFetcher.AddSource(ImgSourceKey.FAKind_Solid, new Uri($@"{PACK_PATH}internalFASolid.xaml", UriKind.RelativeOrAbsolute));
                ResourceFetcher.AddSource(ImgSourceKey.FAKind_Light, new Uri($@"{PACK_PATH}internalFALight.xaml", UriKind.RelativeOrAbsolute));
                _initialized = true;
            }
        }

        public static ImageSource GetIcon(string resource_key,ImgSourceKey sourceKey) {
            EnsureInitialized();
            return ResourceFetcher.GetResource(sourceKey, resource_key) as ImageSource;
        }

        public static ImageSource GetIcon(string resource_key) {
            EnsureInitialized();
            //Get from any
            return ResourceFetcher.GetResourceAny(resource_key) as ImageSource;
        }

        public static ImageSource GetDefaultIcon() {
            return GetIcon(BrandKind.brand_haley_circle.ToString(), ImgSourceKey.BrandKind);
        }

        public static List<Uri> GetAllSourcePaths() {
            return ResourceFetcher.GetAllSourcePaths();
        }
    }
}
