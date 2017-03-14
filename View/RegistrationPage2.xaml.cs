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

namespace ImageRecognizer
{
	public partial class RegistrationPage2 : ContentPage
	{
		public RegistrationPage2()
		{
			InitializeComponent();

		}

		async void add1Clicked(object sender, EventArgs args)
		{
			add1.Opacity = .5;

			var action = await DisplayActionSheet("Load image: ", "From gallery", "Take a photo");
			switch (action)
			{
				case "From gallery":
					Debug.WriteLine("dalla galleria");
					break;
				case "Take a photo":
					Debug.WriteLine("dalla fotocamera");
					TakePictureButton_Clicked();
					break;               
            }
			//IPictureTaker pictureTake = DependencyService.Get<IPictureTaker>();
			//pictureTake.SnapPic();

		}

		void add2Clicked(object sender, EventArgs args)
		{
			add2.Opacity = .5;
		}

		void add3Clicked(object sender, EventArgs args)
		{
			add3.Opacity = .5;
		}

		void add4Clicked(object sender, EventArgs args)
		{
			add4.Opacity = .5;
		}


		async void TakePictureButton_Clicked()
		{
			await CrossMedia.Current.Initialize();

			if (!CrossMedia.Current.IsCameraAvailable || CrossMedia.Current.IsTakePhotoSupported) 
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

			add1.Source = ImageSource.FromStream(() => file.GetStream());


		}

		void UploadPictureButton_Clicked(object sender, EventArgs e)
		{



		}



	}
}
