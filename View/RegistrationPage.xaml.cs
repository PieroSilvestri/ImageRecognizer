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

namespace ImageRecognizer
{
	public partial class RegistrationPage : ContentPage
	{
		private string pass;
		string confPass = "";

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
			myPerson.age = age.Text;

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

		void Age_Control(object sender, EventArgs e)
		{
			var text = ((Entry)sender).Text; //cast sender to access the properties of the Entry
			AgeError.IsVisible = false;
			if (text != "")
			{
				Debug.WriteLine("valore:" + text);
				int textToInt = Convert.ToInt32(text);

				if (textToInt > 120 || textToInt < 5)
				{
					AgeError.IsVisible = true;
					AgeError.Text = "Age not valid, insert a number between 5 and 120 ";
				}
				if (textToInt < 121 && textToInt > 4)
				{
					AgeError.IsVisible = false;
				}
			}
		}

		void Password_Length_Control(object sender, EventArgs e)
		{
			var text = ((Entry)sender).Text; //cast sender to access the properties of the Entry
			pass = text;
			PswError.IsVisible = false;
			testPass();
			if (text != "")
			{
				
				if (text.Length < 4)
				{
					PswError.IsVisible = true;
					PswError.Text = "Password too short (min 4 characters)";
				}
				if (text.Length > 4)
				{
					PswError.IsVisible = false;
				}
			}
		}


		void PasswordConfirm(object sender, EventArgs e)
		{
			var text = ((Entry)sender).Text; //cast sender to access the properties of the Entry
			ConfPsw.IsVisible = false;
			confPass = text;
			if (text != "")
			{
				if (text != pass)
				{
					Debug.WriteLine("confPass true");
					ConfPsw.IsVisible = true;
					ConfPsw.Text = "Passwords do not match!";
				}
				else
				{
					ConfPsw.IsVisible = false;
				}
			}
		}

		void testPass()
		{
			if (confPass != "")
			{
				if (pass == confPass)
				{
					ConfPsw.IsVisible = false;
				}
				else
				{
					Debug.WriteLine("testPass true");
					Debug.WriteLine("confpass =" + confPass);
					ConfPsw.IsVisible = true;
					ConfPsw.Text = "Passwords do not match!";
				}
			}
			else
			{ 
				ConfPsw.IsVisible = false;
			}
		}


	}
}
