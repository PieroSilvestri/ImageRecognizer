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
		int x = 4;


		MediaFile foto1, foto2;
		MainViewModel vm;
		int newId;

		public RegistrationPage2(int id)
		{
			vm = new MainViewModel();
			InitializeComponent();

			newId = id;
		}

		void add1Clicked(object sender, EventArgs args)
		{
			add1.Opacity = .5;

			TakePictureButton_Clicked(1);


			//IPictureTaker pictureTake = DependencyService.Get<IPictureTaker>();
			//pictureTake.SnapPic();

		}

		void add2Clicked(object sender, EventArgs args)
		{
			add2.Opacity = .5;
			TakePictureButton_Clicked(2);
		}

		void add3Clicked(object sender, EventArgs args)
		{
			add3.Opacity = .5;
			TakePictureButton_Clicked(3);
		}

		void add4Clicked(object sender, EventArgs args)
		{
			add4.Opacity = .5;
			TakePictureButton_Clicked(4);
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
					updateLabel();
					break;
				case 2:
					add2.Opacity = 1;
					add2.Source = ImageSource.FromStream(() => file.GetStream());
					foto2 = file;
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

		async void DoneButton_OnCLicked(object sender, EventArgs e)
		{
			Debug.WriteLine("Prima foto: " + foto1.AlbumPath);
			var url1 = await vm.UploadDoc(foto1);
			Debug.WriteLine("Seconda foto: " + foto2.AlbumPath);
			var url2 = await vm.UploadDoc(foto2);

			string newStringJson = "{" +
				"ID: " + newId + "," +
				"Url1: " + url1 + "," +
				"Url2:" + url2 + "}";

			dynamic newJson = new JObject();
			newJson.ID = newId;
			newJson.Url1 = url1;
			newJson.Url2 = url2;

			Debug.WriteLine(newJson);

			var myFlag = vm.RegistrationRequest(newJson);

			if (myFlag)
			{
				await DisplayAlert("Well Done!", "The registration has been done! :)", "OK");
				await Navigation.PushAsync(new LoginPage());
			}
			else
			{
				await DisplayAlert("OPS!", "Something went wrong! :(", "OK");
			}

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
