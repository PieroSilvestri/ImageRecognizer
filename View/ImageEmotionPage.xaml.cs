using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Face;
using Newtonsoft.Json.Linq;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace ImageRecognizer
{
	public partial class ImageEmotionPage : ContentPage
	{

		public readonly IFaceServiceClient faceServiceClient;
		public readonly EmotionServiceClient emotionServiceClient;
		private string faceKey = "e5d1028e78c14c75b0e1ca0b30cb9d3e";
		private string emotionKey = "9384bddf115345fc94d27bf69723de98";
		private string computerVisionKey = "1ffcd7811f1d4980a936adc5aaf63dc8";


		private MainViewModel cognityServices;

		public ImageEmotionPage(MediaFile imageSource)
		{
			InitializeComponent();
			Image1.Source = ImageSource.FromStream(() => imageSource.GetStream());;
			//Ozecky sdk 

			this.Title = "Foto Scattata";

			if(imageSource != null){
				DoTheProgramm(imageSource);
			}
		}

		public async void RetakeAPhoto(object o, EventArgs e)
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

			Image1.Source = ImageSource.FromStream(() => file.GetStream());
		}

		private async void DoTheProgramm(MediaFile file)
		{
			
			JArray emozione = await cognityServices.DetectFaceAndEmotionsAsync(emotionServiceClient, file);
			if (emozione != null)
			{
				Debug.WriteLine("DETECTEMOTION");
				Debug.WriteLine(emozione.First["Key"]);
			}

			string peopleEmotions = await cognityServices.GetPeopleEmotions(emotionKey, file);
			if (peopleEmotions != null)
			{
				JArray jArrayResponse = JArray.Parse(peopleEmotions);
				Debug.WriteLine("GETPEOPLEEMOTIONS");
				Debug.WriteLine(jArrayResponse);
			}

			string imageAnalysis = await cognityServices.MakeAnalysisRequest(computerVisionKey, file);
			if (imageAnalysis != null)
			{
				Debug.WriteLine("MAKEANALYSISREQUEST");
				Debug.WriteLine(imageAnalysis);
				Debug.WriteLine("MAKEANALYSISJSON");
				JObject imageAnalysisJSON = JObject.Parse(imageAnalysis);
				Debug.WriteLine(imageAnalysisJSON);
			}


		}
	}
}
