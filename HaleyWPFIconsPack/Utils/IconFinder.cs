using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haley.Enums;
using System.Windows.Media;
using Haley.Utils;
using Haley.IconsPack.Models;

namespace Haley.IconsPack.Utils {
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
            ImgSourceKey source_key = ImgSourceKey.BrandKind;
            var res_key = BrandKind.brand_haley_circle.ToString();

            do {
                //Check brand kind
                if (Enum.TryParse<BrandKind>(resource_key, true, out var _bkind)) {
                    res_key = _bkind.ToString();
                    source_key = ImgSourceKey.BrandKind;
                    break;
                }

                //Check bootstrap
                if (Enum.TryParse<BootStrapKind>(resource_key, true, out var _bskind)) {
                    res_key = _bskind.ToString();
                    source_key = ImgSourceKey.BootStrapKind;
                    break;
                }

                //Check FontAwesome
                if (Enum.TryParse<FAKind>(resource_key, true, out var _fakind)) {
                    res_key = _fakind.ToString();
                    if (res_key.ToLower().EndsWith("light")) {
                        source_key = ImgSourceKey.FAKind_Light;
                    } else {
                        source_key = ImgSourceKey.FAKind_Solid;
                    }
                    break;
                }
            } while (false);

            //Get from any
            return GetIcon(res_key, source_key);
        }

        public static ImageSource GetDefaultIcon() {
            return GetIcon(BrandKind.brand_haley_circle.ToString(), ImgSourceKey.BrandKind);
        }

        public static List<Uri> GetAllSourcePaths() {
            return ResourceFetcher.GetAllSourcePaths();
        }


    }
}
