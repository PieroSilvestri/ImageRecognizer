using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Net;
using Xamarin.Forms;
using System.Net.Http;


namespace ImageRecognizer
{
	public partial class LoginPage : ContentPage
	{

		MainViewModel vm;

		public LoginPage()
		{

			vm = new MainViewModel();

			InitializeComponent();
			NavigationPage.SetHasNavigationBar(this, false);

		}

		public async void LoginImage_OnClicked(object o, EventArgs e)
		{
			await Navigation.PushAsync(new PasswordPage());
		}

		/*
		public async void DoLogin(object o, EventArgs e)
		{
			//var username = UsernameMy.Text;
			//var password = Password.Text;

		}
		*/

		public void DoRegistrator(object o, EventArgs e)
		{
			Navigation.PushAsync(new RegistrationPage());
		}
	}
}
