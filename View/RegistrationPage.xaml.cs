using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace ImageRecognizer
{
	public partial class RegistrationPage : ContentPage
	{
		public RegistrationPage()
		{
			InitializeComponent();

			this.Title = "Registration";

		}

		void nextPage(object o, EventArgs e)
		{
			Navigation.PushAsync(new RegistrationPage2());
		}

	}
}
