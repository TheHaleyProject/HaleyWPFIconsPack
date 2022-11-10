using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Haley.Models {
    internal class DependencyElement {
        public FrameworkElement TargetObject { get; set; }
        public DependencyProperty TargetProperty { get; set; }
        public object DataContext { get; set; }
    }
}
