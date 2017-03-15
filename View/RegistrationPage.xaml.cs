using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Net;
using Xamarin.Forms;
using Plugin.Media;
using System.Net.Http;
using Plugin.Media.Abstractions;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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

			RegistrationPerson myPerson = new RegistrationPerson();

			myPerson.firstName = FirstName.Text;
			myPerson.lastName = LastName.Text;

			JObject myObj = JObject.Parse(JsonConvert.SerializeObject(myPerson));

			Debug.WriteLine("MYPERSON");
			Debug.WriteLine(myObj);

			Navigation.PushAsync(new RegistrationPage2());
		}

		int i = 1;

		public void EasterEgg(object sender, EventArgs e)
		{

			if (i == 1)
			{
				LogoImage.Source = ImageSource.FromFile("NewLogo2.png");
				Debug.WriteLine("Cambio");
				i = 2;
			}
			else
			{
				LogoImage.Source = ImageSource.FromFile("NewLogo.png");
				Debug.WriteLine("cambio2");
				i = 1;
			}

		}


	}
}
