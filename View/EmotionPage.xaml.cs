using System;
using System.Collections.Generic;
using System.Diagnostics;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace ImageRecognizer
{
	public partial class EmotionPage : ContentPage
	{

		private bool i = true;
		private int user_id;

		public EmotionPage(int id)
		{
			InitializeComponent();
			this.user_id = id;
			this.Title = "Emotion Page";
		}


		public async void TakeAPhoto(object o, EventArgs e)
		{
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

			Debug.WriteLine("MYFILE");
			Debug.WriteLine(file);


			await Navigation.PushAsync(new ImageEmotionPage(this.user_id, file));
		}

		public void ShowReports(object o, EventArgs e)
		{

		}


		public void EasterEgg(object sender, EventArgs e)
		{

			if (i)
			{
				LogoImage.IsVisible = false;
				LogoImage2.IsVisible = true;
			}
			else
			{
				LogoImage2.IsVisible = false;
				LogoImage.IsVisible = true;
			}

			i = !i;

		}
	}
}
