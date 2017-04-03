using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace ImageRecognizer
{
	public partial class EmotionPage : ContentPage
	{

		private bool i = true;
		private int user_id;
		private bool userFlag;
		MainViewModel vm;

		public EmotionPage(bool flag, int id)
		{
			InitializeComponent();
			this.userFlag = flag;
			NavigationPage.SetHasNavigationBar(this, false);
			this.user_id = id;
			loadDelayAnimation();
		}
		 

		public async void TakeAPhoto(object o, EventArgs e)
		{
			await CrossMedia.Current.Initialize();

			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
			{
				await DisplayAlert("No Camera", "No camera avaiable", "Ok");
				await Navigation.PushAsync(new PasswordPage(this.userFlag, null));
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


			await Navigation.PushAsync(new ImageEmotionPage(this.userFlag, this.user_id, file));
		}

		async public void ShowReports(object o, EventArgs e)
		{
			await Navigation.PushAsync(new ListReportPage(this.userFlag, user_id));
		}




		async void loadDelayAnimation()
		{
			cameraIcon.Opacity = 0;
			reportsIcon.Opacity = 0;

			await Task.Delay(500);
			await cameraIcon.FadeTo(1, 200);
			await reportsIcon.FadeTo(1, 200);

		}



	}
}
