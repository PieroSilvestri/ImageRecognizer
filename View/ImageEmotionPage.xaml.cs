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
				JObject jsonEmotionDetected = SetEmotionUI(jsonToPass);
				Debug.WriteLine("JSON EMOTIUON DETECTED");
				Debug.WriteLine(jsonEmotionDetected);

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

		private JObject SetEmotionUI(JObject jsonToParse)
		{
			JArray faces = (JArray)jsonToParse["faces"];
			JArray emotions = (JArray)jsonToParse["emotions"];
			int personsCount = faces.Count;
			int ageAverage = 0;
			int maleCount = 0;
			int femaleCount = 0;

			foreach (JObject face in faces)
			{
				ageAverage += (int)face["age"];
				if ((String)face["gender"] == "Male")
				{
					maleCount++;
				}
				else if ((String)face["gender"] == "Female")
				{
					femaleCount++;
				}
			}
			ageAverage = ageAverage / personsCount;

			double anger = 0;
			double contempt = 0;
			double disgust = 0;
			double fear = 0;
			double happiness = 0; 
			double neutral = 0;
			double sadness = 0;
			double surprise = 0;
			string maxEmotion = "";
			double max = 0;
			foreach (JObject emotion in emotions)
			{
				anger += (double)emotion["anger"];
				contempt += (double)emotion["contempt"];
				disgust += (double)emotion["disgust"];
				fear += (double)emotion["fear"];
				happiness += (double)emotion["happiness"];
				neutral += (double)emotion["neutral"];
				sadness += (double)emotion["sadness"];
				surprise += (double)emotion["surprise"];
			}
			anger = anger / personsCount;
			contempt = contempt / personsCount;
			disgust = disgust / personsCount;
			fear = fear / personsCount;
			happiness = happiness / personsCount;
			neutral = neutral / personsCount;
			sadness = sadness / personsCount;
			surprise = surprise / personsCount;

			double[] emotionValues = { anger, contempt, disgust, fear, happiness, neutral, sadness, surprise};
			int index = 0;
			for (int i = 0; i < 8; i++)
			{
				if (emotionValues[i] > max)
				{
					max = emotionValues[i];
					index = i;
				}
			}

			switch (index)
			{
				case 0:
					maxEmotion = "Anger";
					break;
				case 1:
					maxEmotion = "Contempt";
					break;
				case 2:
					maxEmotion = "Disgust";
					break;
				case 3:
					maxEmotion = "Fear";
					break;
				case 4:
					maxEmotion = "Happiness";
					break;
				case 5:
					maxEmotion = "Neutral";
					break;
				case 6:
					maxEmotion = "Sadness";
					break;
				case 7:
					maxEmotion = "Surprise";
					break;
			}

			JObject results = new JObject(
				new JProperty("PersonCount", personsCount),
				new JProperty("MaleCount", maleCount),
				new JProperty("Age", ageAverage));

			return results;
		}
	}
}
