using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Net;
using Xamarin.Forms;
using Plugin.Media;
using System.Net.Http;
using Plugin.Media.Abstractions;
using Newtonsoft.Json.Linq;

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
				await Navigation.PushAsync(new PasswordPage(null));
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

			spinner.IsVisible = true;
			spinner.IsRunning = true;
			loadingLabel.IsVisible = true;
			buttonGet.IsVisible = false;
			buttonLogin.IsVisible = false;
			reg.IsVisible = false;
			or.IsVisible = false;
			boxUno.IsVisible = false;
			boxDue.IsVisible = false;


			var picture = ImageSource.FromStream(() => file.GetStream());

			var myPath = file.AlbumPath;

			Debug.WriteLine("MYFILE");
			Debug.WriteLine(file);

			//await Navigation.PushAsync(new PasswordPage(file.AlbumPath));

			//var url = "http://l-raggioli2.eng.teorema.net/api/upload/";

			var newUrl = await vm.MakeDetectRequest(file);

			if (newUrl != null)
			{
				try
				{
					JObject myUser = await vm.PostFaceIdToServer(newUrl["faceId"].ToString());

					if ((bool)myUser["success"])
					{
						await Navigation.PushAsync(new PasswordPage(myUser));
						HideSpinner();
					}
					else
					{
						HideSpinner();
						await DisplayAlert("Error!", "Your are FAKE! Try again.", "OK");
					}
				}
				catch (Exception exc)
				{
					Debug.WriteLine("FaceId Error");
					Debug.WriteLine(exc);
					await DisplayAlert("Error!", "Your face id is not valid. Try again.", "OK");
					HideSpinner();
				}

			}
			else
			{
				HideSpinner();
				await DisplayAlert("Error!", "Your photo is not valid. Try again.", "OK");
			}

		}

		private void HideSpinner()
		{
			spinner.IsVisible = false;
			spinner.IsRunning = false;
			buttonGet.IsVisible = true;
			buttonLogin.IsVisible = true;
			reg.IsVisible = true;
			or.IsVisible = true;
			boxUno.IsVisible = true;
			boxDue.IsVisible = true;
			loadingLabel.IsVisible = false;
		}

		public async void Get_OnClicked(object o, EventArgs e) 
		{

			var key = "8f5a96d2-007a-478a-bb2e-e8098891becf";

			var url = "http://l-raggioli2.eng.teorema.net/api/values/" + key;

			var getItem = await vm.JsonGetProva(url);

			if (getItem != null)
			{
				await Navigation.PushAsync(new PasswordPage(getItem));
			}
		}

		public void DoRegistrator(object o, EventArgs e)
		{
			Navigation.PushAsync(new RegistrationPage());
		}

		bool i = true;

		public void EasterEgg(object sender, EventArgs e)
		{

			if (i) {
				LogoImage.IsVisible = false;
				LogoImage2.IsVisible = true;
				//LogoImage.Source = ImageSource.FromFile("NewLogo2.png");
				sfondo.BackgroundColor = Color.Black;
				titolo.TextColor = Color.White;
				or.TextColor = Color.White;
				reg.TextColor = Color.White;
				reg.BorderColor = Color.White;
				i = false;
			}
			else
			{
				LogoImage2.IsVisible = false;
				LogoImage.IsVisible = true;
				//LogoImage.Source = ImageSource.FromFile("NewLogo.png");
				sfondo.BackgroundColor = Color.Default;
				titolo.TextColor = Color.Default;
				or.TextColor = Color.Default;
				reg.TextColor = Color.Default;
				reg.BorderColor = Color.Black;
				i = true;
			}

		}

	}
}
