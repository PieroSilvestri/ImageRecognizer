using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Net;
using Xamarin.Forms;
using Plugin.Media;
using System.Net.Http;
using Plugin.Media.Abstractions;

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
			//await Navigation.PushAsync(new PasswordPage());
			await CrossMedia.Current.Initialize();

			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported) 
			{
				await DisplayAlert("No Camera", "No camera avaiable", "Ok");
				await Navigation.PushAsync(new PasswordPage("non trovato"));
				return;
			}

			var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
			{
				Directory = "ImageRecognizer",
				Name = DateTime.Now.ToString()
			});

			if (file == null)
			{
				return;
			}

			Debug.WriteLine("PATHHHHH");
			Debug.WriteLine(file.AlbumPath);


			var picture = ImageSource.FromStream(() => file.GetStream());

			goToPasswordPage(file.AlbumPath);

		}

		void goToPasswordPage(string path) {
			Navigation.PushAsync(new PasswordPage(path));
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

		int i = 1;

		public void EasterEgg(object sender, EventArgs e)
		{

			if (i == 1) {
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
