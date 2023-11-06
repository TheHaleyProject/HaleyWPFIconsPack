using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haley.Enums;
using Haley.Models;

namespace ResourceTest
{
    public class MainVM : ChangeNotifier
    {
		Random _random = new Random();
		Array _values = Enum.GetValues(typeof(BrandKind));
		Enum _source = BrandKind.brand_haley_square;

		public Enum SourceEnum {
			get { return _source; }
			set { _source = value;
				OnPropertyChanged(); //Raise notification
			}
		}

		public DelegateCommandBase CMDChangeRandomImage => new DelegateCommandBase(ChangeRandomImage);

		private void ChangeRandomImage() {
			//Pick a random image.
			SourceEnum = (BrandKind)_values.GetValue(_random.Next(_values.Length));
		}

		public MainVM() {
		
		}
    }
}
