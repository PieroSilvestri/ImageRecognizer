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

			var picture = ImageSource.FromStream(() => file.GetStream());

			var myPath = file.AlbumPath;

			Debug.WriteLine("MYPATH");
			Debug.WriteLine(file);

			//await Navigation.PushAsync(new PasswordPage(file.AlbumPath));

			//var url = "http://l-raggioli2.eng.teorema.net/api/upload/";

			var newUrl = await vm.UploadDoc(file);

			await vm.PostUrlToServer(newUrl);

			if (vm.GetJsonItem != null)
			{
				await Navigation.PushAsync(new PasswordPage(vm.GetJsonItem));
			}
		}
		/*
		public byte[] ReadImageFile(string imageLocation)
		{
			byte[] imageData = null;
			FileInfo fileInfo = new FileInfo(imageLocation);
			long imageFileLength = fileInfo.Length;
			FileStream fs = new FileStream(imageLocation, FileMode.Open, FileAccess.Read);
			BinaryReader br = new BinaryReader(fs);
			imageData = br.ReadBytes((int)imageFileLength);
			return imageData;
		}
		*/

		public async void Get_OnClicked(object o, EventArgs e) 
		{

			var key = "8f5a96d2-007a-478a-bb2e-e8098891becf";

			var url = "http://l-raggioli2.eng.teorema.net/api/values/" + key;

			await vm.JsonGetProva(url);

			if (vm.GetJsonItem != null)
			{
				await Navigation.PushAsync(new PasswordPage(vm.GetJsonItem));
			}
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
