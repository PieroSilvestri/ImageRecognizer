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
				sfondo.BackgroundColor = Color.Black;
				titolo.TextColor = Color.White;
				or.TextColor = Color.White;
				reg.TextColor = Color.White;
				reg.BorderColor = Color.White;
				i = 2;
			}
			else
			{
				LogoImage.Source = ImageSource.FromFile("NewLogo.png");
				sfondo.BackgroundColor = Color.Default;
				titolo.TextColor = Color.Default;
				or.TextColor = Color.Default;
				reg.TextColor = Color.Default;
				reg.BorderColor = Color.Default;
				i = 1;
			}

		}

	}
}
