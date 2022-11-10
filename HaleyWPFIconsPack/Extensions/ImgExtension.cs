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
using Isolated.Haley.WpfIconPack;
using Haley.WPF.Models;

namespace Haley.Utils
{
    //Binding to markup extension requires should be two way. Whenever the value changes, we need to return it back and whenever the propertyname (binded) changes, we need to act upon it.
    //https://stackoverflow.com/questions/10328802/markupextension-with-binding-parameters Taking an idea from here and mixing it up with our LangExtension (in Haley.MVVM) to make this ImgExtension work with a bindable property
    //We definitely need to keep this simple, so strictly no creation of attached property to deal with this.
    
    public class ImgExtension : MarkupExtension {
        private string _bindingPropName;
        private DependencyElement _target;
        private IconSourceProvider _sourceProvider = new IconSourceProvider();

        //If we set the value of BindingPropertyName here, then during debug runtime, we will not be able to change to new property
        public ImgExtension(string binding_name) : this() {
            //IMPORTANT: AT ANY COST, DONOT SET ANY DEFAULT VALUES. DEFAULT VALUES CAN BE SET AS FALL BACK ONLY (Via SetDefault() Method)
            _bindingPropName = binding_name;
        }

        public ImgExtension() {
            //IMPORTANT: AT ANY COST, DONOT SET ANY DEFAULT VALUES. DEFAULT VALUES CAN BE SET AS FALL BACK ONLY (Via SetDefault() Method)
        }


        #region Properties
        //[ConstructorArgument("@enum")] //If we setup constructor argument, empty values will throw exception
        public BrandKind? Brand { get; set; }
        public BootStrapKind? BStrap { get; set; }
        public FAKind? FA { get; set; }
        /// <summary>
        /// If complete datacontext needs to be considered, type "this" else direclty provide the "propertyname"
        /// </summary>

        public string BindingPropertyName {
            get { return _bindingPropName; }
            set { _bindingPropName = value; }
        }

        #endregion


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
            //DATA FLOW (DO to EXT):
            //Dependencyobject (DO) which consume this Extension (EXT) will provide "BindingPropertyName". This property name itself is not a bindable value. It is merely the name of the property to which our EXT should bind to. Value of the property can change during run time.
            //Once EXT receives the property name, it hooks up to the DataContext Changes of the DO.
            //When the value of the property value changes during runtime, it will trigger the DataContext change and eventually EXT will receive it in runtime.

            //DATAFLOW (EXT TO DO) 
            //Since EXT is binding to the DataContext changes of the DO, whenever the value is changing, we invoke the internal Binding to provide value.

            //Always remove the old target subscriptions
            if (_target != null) {
                _target.TargetObject.DataContextChanged -= TargetDataChanged;
            }

            //PROVIDE VALUE WILL BE CALLED WHENEVER ANY PROPERTY ON THIS WILL BE CHANGED.
            if (!string.IsNullOrWhiteSpace(BindingPropertyName)) {
                //Binding takes top most priority
                //If binding is not null, we try to fetch the datacontext of the target element and then bind to the changes.
                if (GetTargetElement(serviceProvider, out var target)) {
                    //create binding and then return the binding expression, rather than directly returning the value.
                    _target = target;
                    _target.TargetObject.DataContextChanged += TargetDataChanged; //To receive the property changes during runtime
                    var binding = CreateBinding();
                    return binding.ProvideValue(serviceProvider); //This will provide the value of IconSource Property
                    //If we directly add a binding to the property name, then whenever the value of that property changes, we will 
                }
            }

            if (!Process(out var @enum, out var @key)) return null;
            return IconFinder.GetIcon(key, @enum.Value);
        }

        private void TargetDataChanged(object sender, DependencyPropertyChangedEventArgs e) {
            object propValue = e.NewValue;
            try {
                //To receive message whenever the property value is changed.
                //Since we are dealing with DataContextChange, we will always get DataContext Property
                if (e.NewValue != null && !(e.NewValue is Enum)) {
                    //If newvalue is an object.
                    PropertyInfo tarProp = e.NewValue.GetType().GetProperty(BindingPropertyName);
                    propValue = tarProp?.GetValue(e.NewValue);
                    } 
                } catch (Exception) {
            }
            _sourceProvider.OnDataChanged(e.NewValue); //this will be the new data.
        }

        bool GetTargetElement(IServiceProvider serviceProvider,out DependencyElement target) {
                target = null;
            try {
                var targetProvider = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
                target = new DependencyElement() { TargetObject = targetProvider.TargetObject as FrameworkElement, TargetProperty = targetProvider.TargetProperty as DependencyProperty };
                return true;
            } catch (Exception) {
                return false;
            }
        }

        Binding CreateBinding() {

            //This binding itself is to return value from EXT to DO
            var binding = new Binding(nameof(IconSourceProvider.IconSource)) {
                Source = _sourceProvider
            };
            return binding;
        }

        ~ImgExtension() {
            if (_target != null) {
                _target.TargetObject.DataContextChanged -= TargetDataChanged;
            }

        }
    }
}