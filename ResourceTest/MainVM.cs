using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Haley.Enums;

namespace ResourceTest
{
    public class MainVM : ObservableObject
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

		public RelayCommand CMDChangeRandomImage => new RelayCommand(ChangeRandomImage);

		private void ChangeRandomImage() {
			//Pick a random image.
			SourceEnum = (BrandKind)_values.GetValue(_random.Next(_values.Length));
		}

		public MainVM() {
		
		}
    }
}
