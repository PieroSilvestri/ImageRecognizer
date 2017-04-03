using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Diagnostics;
using ImageRecognizer;
using Newtonsoft.Json.Linq;

namespace ImageRecognizer
{
	public partial class RegistrationPage2 : ContentPage
	{

		private MediaFile foto1, foto2;
		private MainViewModel vm;
		private int newId;
		private string faceKey;
		private bool userFlag;

		public RegistrationPage2(bool flag, int id)
		{
			vm = new MainViewModel();
			InitializeComponent();
			this.userFlag = flag;
			if (flag)
			{
				faceKey = "1297619ba72542d38347e044905ed499";
			}
			else
			{
				faceKey = "e5d1028e78c14c75b0e1ca0b30cb9d3e";
			}
			spinner.IsVisible = false;
			spinner.IsRunning = false;
			loadingLabel.IsVisible = false;
			buttonDone.IsVisible = true;
			newId = id;
		}

		void add1Clicked(object sender, EventArgs args)
		{
			add1.Opacity = .5;

			TakePictureButton_Clicked(1);

		}

		void add2Clicked(object sender, EventArgs args)
		{
			add2.Opacity = .5;
			TakePictureButton_Clicked(2);
		}

		async void TakePictureButton_Clicked(int buttonNumber)
		{
			await CrossMedia.Current.Initialize();

			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported) 
			{
				await DisplayAlert("No Camera", "No camera avaiable", "Ok");
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

			switch (buttonNumber)
			{
				case 1:
					add1.Opacity = 1;
					add1.Source = ImageSource.FromStream(() => file.GetStream());
					foto1 = file;
					break;
				case 2:
					add2.Opacity = 1;
					add2.Source = ImageSource.FromStream(() => file.GetStream());
					foto2 = file;
					break;
			}


		}

		async void DoneButton_OnCLicked(object sender, EventArgs e)
		{
			spinner.IsVisible = true;
			spinner.IsRunning = true;
			loadingLabel.IsVisible = true;
			buttonDone.IsVisible = false;


			Debug.WriteLine("Prima foto: " + foto1.AlbumPath);
			JObject pfi1 = await vm.AddFaceToList(userFlag, faceKey, foto1);
			Debug.WriteLine("Seconda foto: " + foto2.AlbumPath);
			JObject pfi2 = await vm.AddFaceToList(userFlag, faceKey, foto2);

			Debug.WriteLine(pfi1);
			Debug.WriteLine(pfi2);

			try
			{
				JObject sendRegistrationImages = new JObject(
				new JProperty("ID", newId),
				new JProperty("faceId_1", pfi1["persistedFaceId"].ToString()),
				new JProperty("faceId_2", pfi2["persistedFaceId"].ToString()));
				Debug.WriteLine("sendregistrationimages");
				Debug.WriteLine(sendRegistrationImages);

				var myFlag = await vm.RegistrationRequest(userFlag, sendRegistrationImages);

				if (myFlag)
				{
					spinner.IsVisible = false;
					spinner.IsRunning = false;
					loadingLabel.IsVisible = false;
					await DisplayAlert("Well Done!", "The registration has been done! :)", "OK");
					await Navigation.PushAsync(new LoginPage());
				}
				else
				{
					spinner.IsVisible = false;
					spinner.IsRunning = false;
					loadingLabel.IsVisible = false;
					buttonDone.IsVisible = true;
					await DisplayAlert("OPS!", "Something went wrong! :(", "OK");
				}
			}
			catch (Exception exc)
			{
				Debug.WriteLine("Error");
				Debug.WriteLine(exc);
				spinner.IsVisible = false;
				spinner.IsRunning = false;
				loadingLabel.IsVisible = false;
				buttonDone.IsVisible = true;
				await DisplayAlert("Error", "Something went wrong. Try again!", "OK");
			}
				

		}

	}
}
