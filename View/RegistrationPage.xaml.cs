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
		string errorMsg = "This field can not be empty!";

		public RegistrationPage()
		{
			InitializeComponent();

			this.Title = "Registration";

			FirstName.Text = "";
			LastName.Text = "";
			age.Text = "";
			nickname.Text = "";
			password.Text = "";
			confPassword.Text = "";
		}


		void nextPage(object o, EventArgs e)
		{



			if (AgeError.IsVisible == true || 
			    PswError.IsVisible == true || 
			    ConfPsw.IsVisible == true || 
			    FirstName.Text == "" || 
			    LastName.Text == "" ||
			    age.Text == "" ||
			    nickname.Text == "" ||
			    password.Text == "" ||
			    confPassword.Text == "")
			{
				DisplayAlert("Error", "Resolve errors!", "Ok");
				if (FirstName.Text == "") {
					FirstNameError.Text = errorMsg;
					FirstNameError.IsVisible = true;
				}
				if (LastName.Text == "" || LastName.Text == null)
				{
					LastNameError.Text = errorMsg;
					LastNameError.IsVisible = true;
				}
				if (age.Text == "")
				{
					AgeError.Text = errorMsg;
					AgeError.IsVisible = true;
				}
				if (nickname.Text == "")
				{
					NicknameError.Text = errorMsg;
					NicknameError.IsVisible = true;
				}
				if (password.Text == "")
				{
					PswError.Text = errorMsg;
					PswError.IsVisible = true;
				}
				if (confPassword.Text == "")
				{
					ConfPsw.Text = errorMsg;
					ConfPsw.IsVisible = true;
				}

			}
			else{

				RegistrationPerson myPerson = new RegistrationPerson();

				myPerson.FirstName = FirstName.Text;
				myPerson.LastName = LastName.Text;
				string eta = age.Text;
				myPerson.Age = Convert.ToInt32(eta);
				myPerson.UserName = nickname.Text;
				myPerson.Password = password.Text;
				myPerson.DataRegistration = DateTime.Now.ToString();

				JObject myObj = JObject.Parse(JsonConvert.SerializeObject(myPerson));

				Debug.WriteLine("MYPERSON");
				Debug.WriteLine(myObj);

				Navigation.PushAsync(new RegistrationPage2());
			}
		}

		bool i = true;

		public void EasterEgg(object sender, EventArgs e)
		{

			if (i)
			{
				LogoImage.Source = ImageSource.FromFile("NewLogo2.png");
				Debug.WriteLine("Cambio");
				i = false;
			}
			else
			{
				LogoImage.Source = ImageSource.FromFile("NewLogo.png");
				Debug.WriteLine("cambio2");
				i = true;
			}

		}

		void CleanFirstName(object sender, EventArgs e)
		{
			if (FirstNameError.Text == errorMsg)
			{
				FirstNameError.IsVisible = false;
			}
		}

		void CleanLastName(object sender, EventArgs e)
		{
			if (LastNameError.Text == errorMsg)
			{
				LastNameError.IsVisible = false;
			}
		}


		void Age_Control(object sender, EventArgs e)
		{
			try {
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
			} catch(FormatException exForm) {
				Debug.WriteLine(exForm);
				age.Text = "";

			}

			if (AgeError.Text == errorMsg) 
			{
				AgeError.IsVisible = false;
			}

		}

		void CleanNick(object sender, EventArgs e)
		{
			if (NicknameError.Text == errorMsg)
			{
				NicknameError.IsVisible = false;
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

			if (PswError.Text == errorMsg)
			{
				PswError.IsVisible = false;
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

			if (ConfPsw.Text == errorMsg)
			{
				ConfPsw.IsVisible = false;
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
