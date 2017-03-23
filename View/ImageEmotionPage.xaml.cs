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
		private int user_id;

		public ImageEmotionPage(int id, MediaFile imageSource)
		{
			InitializeComponent();
			Image1.Source = ImageSource.FromStream(() => imageSource.GetStream());;
			//Ozecky sdk
			user_id = id;
			cognityServices = new MainViewModel();
			this.faceServiceClient = new FaceServiceClient(faceKey);
			this.emotionServiceClient = new EmotionServiceClient(emotionKey);
			this.Title = "Foto Scattata";


			DoTheProgramm(imageSource);
		}

		public async void RetakeAPhoto(object o, EventArgs e)
		{
			await CrossMedia.Current.Initialize();
			facesDetected.IsVisible = false;
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
			spinner.IsVisible = true;
			spinner.IsRunning = true;
			DoTheProgramm(file);
		}

		private async void DoTheProgramm(MediaFile file)
		{

			/*
			JArray emozione = await cognityServices.DetectFaceAndEmotionsAsync(emotionServiceClient, file);
			if (emozione != null)
			{
				Debug.WriteLine("DETECTEMOTION");
				Debug.WriteLine(emozione.First["Key"]);
			}
			*/

			JArray scores = new JArray();
			JArray faces = new JArray();

			string peopleEmotions = await cognityServices.GetPeopleEmotions(emotionKey, file);
			if (peopleEmotions != null)
			{
				JArray jArrayResponse = JArray.Parse(peopleEmotions);
				Debug.WriteLine("GETPEOPLEEMOTIONS");
				Debug.WriteLine(jArrayResponse);

				foreach (JObject item in jArrayResponse)
				{
					scores.Add((JObject)item["scores"]);
				}
			}

			string imageAnalysis = await cognityServices.MakeAnalysisRequest(computerVisionKey, file);

			if (imageAnalysis != null)
			{
				Debug.WriteLine("MAKEANALYSISREQUEST");
				Debug.WriteLine(imageAnalysis);
				Debug.WriteLine("MAKEANALYSISJSON");
				JObject imageAnalysisJSON = JObject.Parse(imageAnalysis);
				Debug.WriteLine(imageAnalysisJSON);

				foreach (JObject item in imageAnalysisJSON["faces"])
				{
					faces.Add(item);
				}
			}

			Debug.WriteLine("JSONTOPOST");
			JObject jsonToPass = new JObject(
				new JProperty("success", true),
				new JProperty("faces", faces),
				new JProperty("emotions", scores),
				new JProperty("id_user", this.user_id));
			Debug.WriteLine(jsonToPass);

			bool response;

			if (!faces.HasValues)
			{
				response = false;
			}
			else if (!scores.HasValues)
			{
				response = false;
			}
			else
			{
				response = await cognityServices.SendEmotions(jsonToPass);
			}

			if (response)
			{
				spinner.IsVisible = false;
				spinner.IsRunning = false;
				await DisplayAlert("Success!", "Everything has been done correctly.", "Ok");
				facesDetected.Text = "Face detected: " + faces.Count.ToString();
				facesDetected.IsVisible = true;
			}
			else
			{
				spinner.IsVisible = false;
				spinner.IsRunning = false;
				await DisplayAlert("Error!", "Value not inserted.", "Ok");
				facesDetected.Text = "No faces Detected";
				facesDetected.IsVisible = true;
			}

		}
	}
}
