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
		int x = 4;

		public RegistrationPage2()
		{
			InitializeComponent();

		}

		void add1Clicked(object sender, EventArgs args)
		{
			add1.Opacity = .5;

			photoTaker(1);


			//IPictureTaker pictureTake = DependencyService.Get<IPictureTaker>();
			//pictureTake.SnapPic();

		}

		void add2Clicked(object sender, EventArgs args)
		{
			add2.Opacity = .5;
			photoTaker(2);
		}

		void add3Clicked(object sender, EventArgs args)
		{
			add3.Opacity = .5;
			photoTaker(3);
		}

		void add4Clicked(object sender, EventArgs args)
		{
			add4.Opacity = .5;
			photoTaker(4);
		}


		async void photoTaker(int buttonNumber) 
		{ 
		
			var action = await DisplayActionSheet("Load image: ", "From gallery", "Take a photo");
			switch (action)
			{
				case "From gallery":
					Debug.WriteLine("dalla galleria");
					break;
				case "Take a photo":
					Debug.WriteLine("dalla fotocamera");
					TakePictureButton_Clicked(buttonNumber);
					break;
			}
		
		}


		async void TakePictureButton_Clicked(int buttonNumber)
		{
			await CrossMedia.Current.Initialize();

			/*if (!CrossMedia.Current.IsCameraAvailable || CrossMedia.Current.IsTakePhotoSupported) 
			{
				await DisplayAlert("No Camera", "No camera avaiable", "Ok");
				return;
			}*/

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
					updateLabel();
					break;
				case 2:
					add2.Opacity = 1;
					add2.Source = ImageSource.FromStream(() => file.GetStream());
					updateLabel();
					break;
				case 3:
					add3.Opacity = 1;
					add3.Source = ImageSource.FromStream(() => file.GetStream());
					updateLabel();
					break;
				case 4:
					add4.Opacity = 1;
					add4.Source = ImageSource.FromStream(() => file.GetStream());
					updateLabel();
					break;	
			}


		}

		void UploadPictureButton_Clicked(object sender, EventArgs e)
		{


		}


		void updateLabel()
		{
			x--;
			string numero = x.ToString();
			if (x == 1)
			{
				numPic.Text = "add 1 picture of your face: ";
			}
			else { 
				if (x == 0)
				{
					numPic.Text = "Well done!";
				}
				else
				{
					numPic.Text = "add " + numero + " pictures of your face: ";
				}
			}

		}




	}
}
