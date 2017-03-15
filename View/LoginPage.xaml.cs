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

			if (!CrossMedia.Current.IsCameraAvailable || CrossMedia.Current.IsTakePhotoSupported) 
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

			await Navigation.PushAsync(new PasswordPage(file.AlbumPath));

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
