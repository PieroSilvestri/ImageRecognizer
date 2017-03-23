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

		MainViewModel vm;

		public RegistrationPage()
		{
			vm = new MainViewModel();

			InitializeComponent();

			this.Title = "Registration";

			Name.Text = "";
			Surname.Text = "";
			Role.Text = "";
			Email.Text = "";
			Password.Text = "";
			ConfPassword.Text = "";
		}


		async void nextPage(object o, EventArgs e)
		{

			if (AgeError.IsVisible == true || 
			    PswError.IsVisible == true || 
			    ConfPsw.IsVisible == true || 
			    Name.Text == "" || 
			    Surname.Text == "" ||
			    Role.Text == "" ||
			    Email.Text == "" ||
			    Password.Text == "" ||
			    ConfPassword.Text == "")
			{
				await DisplayAlert("Error", "Resolve errors!", "Ok");
				if (Name.Text == "") {
					FirstNameError.Text = errorMsg;
					FirstNameError.IsVisible = true;
				}
				if (Surname.Text == "" || Surname.Text == null)
				{
					LastNameError.Text = errorMsg;
					LastNameError.IsVisible = true;
				}
				if (Role.Text == "")
				{
					AgeError.Text = errorMsg;
					AgeError.IsVisible = true;
				}
				if (Email.Text == "")
				{
					NicknameError.Text = errorMsg;
					NicknameError.IsVisible = true;
				}
				if (Password.Text == "")
				{
					PswError.Text = errorMsg;
					PswError.IsVisible = true;
				}
				if (ConfPassword.Text == "")
				{
					ConfPsw.Text = errorMsg;
					ConfPsw.IsVisible = true;
				}

			}
			else{

				RegistrationPerson myPerson = new RegistrationPerson();

				myPerson.Name = Name.Text;
				myPerson.Surname = Surname.Text;
				myPerson.Role = Role.Text;
				myPerson.Email = Email.Text;
				myPerson.Password = Password.Text;
				myPerson.Birthday = DatePickerText.Date.ToString("yyyy-MM-dd");
				myPerson.DataRegistration = DateTime.Now.ToString();

				JObject myObj = JObject.Parse(JsonConvert.SerializeObject(myPerson));

				Debug.WriteLine("MYPERSON");
				Debug.WriteLine(myObj);

				var id = await vm.CreateANewUser(myObj);

				await Navigation.PushAsync(new RegistrationPage2(id));
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
				Role.Text = "";
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
