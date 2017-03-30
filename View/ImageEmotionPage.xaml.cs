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

			DoTheProgramm(imageSource);
		}

		public async void RetakeAPhoto(object o, EventArgs e)
		{
			await CrossMedia.Current.Initialize();
			facesDetected.IsVisible = false;
			ageInfo.IsVisible = false;
			maleFemale.IsVisible = false;
			angerTab.IsVisible = false;
			sadTab.IsVisible = false;
			fearTab.IsVisible = false;
			happyTab.IsVisible = false;
			neutralTab.IsVisible = false;
			disgustTab.IsVisible = false;
			contemptTab.IsVisible = false;
			surprisedTab.IsVisible = false;

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
			try
			{
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
			}
			catch (Exception exc)
			{
				Debug.WriteLine("Error");
				Debug.WriteLine(exc);
				spinner.IsVisible = false;
				spinner.IsRunning = false;
				await DisplayAlert("Error!", "Value not inserted.", "Ok");
				facesDetected.Text = "No faces Detected";
				facesDetected.IsVisible = true;
				return;
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
				new JProperty("id_user", this.user_id),
				new JProperty("id_list", 12));
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
				ageInfo.IsVisible = true;
				maleFemale.IsVisible = true;
				JObject jsonEmotionDetected = SetEmotionUI(jsonToPass);
				Debug.WriteLine("JSON EMOTIUON DETECTED");
				Debug.WriteLine(jsonEmotionDetected);
				int personCount = (int) jsonEmotionDetected["PersonCount"];
				int maleCount = (int)jsonEmotionDetected["MaleCount"];
				int femaleCount = (int)jsonEmotionDetected["FemaleCount"];
				int age = (int)jsonEmotionDetected["Age"];

				int angerCount = (int)jsonEmotionDetected["AngerCount"];
				int contemptCount = (int)jsonEmotionDetected["ContemptCount"];
				int disgustCount = (int)jsonEmotionDetected["DisgustCount"];
				int fearCount = (int)jsonEmotionDetected["FearCount"];
				int happinessCount = (int)jsonEmotionDetected["HappinessCount"];
				int neutralCount = (int)jsonEmotionDetected["NeutralCount"];
				int sadnessCount = (int)jsonEmotionDetected["SadnessCount"];
				int surpriseCount = (int)jsonEmotionDetected["SurpriseCount"];

				if (angerCount != 0)
				{
					angerTab.IsVisible = true;
					angerLabel.Text = "Anger: " + angerCount;
				}
				else { angerTab.IsVisible = false; }

				if (contemptCount != 0)
				{
					contemptTab.IsVisible = true;
					contemptLabel.Text = "Contempt: " + contemptCount;
				}
				else { contemptTab.IsVisible = false; }

				if (disgustCount != 0)
				{
					disgustTab.IsVisible = true;
					disgustLabel.Text = "Disgust: " + disgustCount;
				}
				else { disgustTab.IsVisible = false; }

				if (fearCount != 0)
				{
					fearTab.IsVisible = true;
					fearLabel.Text = "Fear: " + fearCount;
				}
				else { fearTab.IsVisible = false; }

				if (happinessCount != 0)
				{
					happyTab.IsVisible = true;
					happyLabel.Text = "Happiness: " + happinessCount;
				}
				else { happyTab.IsVisible = false; }

				if (neutralCount != 0)
				{
					neutralTab.IsVisible = true;
					neutralLabel.Text = "Neutral: " + neutralCount;
				}
				else { neutralTab.IsVisible = false; }

				if (sadnessCount != 0)
				{
					sadTab.IsVisible = true;
					sadLabel.Text = "Sadness: " + sadnessCount;
				}
				else { sadTab.IsVisible = false; }

				if (surpriseCount != 0)
				{
					surprisedTab.IsVisible = true;
					surprisedLabel.Text = "Surprised: " + surpriseCount;
				}
				else { surprisedTab.IsVisible = false; }

				ageAverage.Text = "Age average: " + age;
				maleNum.Text = "" + maleCount;
				femaleNum.Text = "" + femaleCount;

				/*
					  "PersonCount": 1,
					  "MaleCount": 1,
					  "FemaleCount": 0,
					  "Age": 32,
					  "AngerCount": 0,
					  "ContemptCount": 0,
					  "DisgustCount": 0,
					  "FearCount": 0,
					  "HappinessCount": 0,
					  "NeutralCount": 0,
					  "SadnessCount": 0,
					  "SurpriseCount": 1
				*/

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





	private  JObject SetEmotionUI(JObject jsonToParse)
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

			int angerCount = 0;
			int contemptCount = 0;
			int disgustCount = 0;
			int fearCount = 0;
			int happinessCount = 0;
			int neutralCount = 0;
			int sadnessCount = 0;
			int surpriseCount = 0;

			foreach (JObject emotion in emotions)
			{
				double max = 0;
				double anger = (double)emotion["anger"];

				double contempt = (double)emotion["contempt"];
				double disgust = (double)emotion["disgust"];
				double fear = (double)emotion["fear"];
				double happiness = (double)emotion["happiness"];
				double neutral = (double)emotion["neutral"];
				double sadness = (double)emotion["sadness"];
				double surprise = (double)emotion["surprise"];

				double[] emotionValues = { anger, contempt, disgust, fear, happiness, neutral, sadness, surprise };
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
						angerCount++;
						break;
					case 1:
						contemptCount++;
						break;
					case 2:
						disgustCount++;
						break;
					case 3:
						fearCount++;
						break;
					case 4:
						happinessCount++;
						break;
					case 5:
						neutralCount++;
						break;
					case 6:
						sadnessCount++;
						break;
					case 7:
						surpriseCount++;
						break;
				}
			}

	


			JObject results = new JObject(
				new JProperty("PersonCount", personsCount),
				new JProperty("MaleCount", maleCount),
				new JProperty("FemaleCount", femaleCount),
				new JProperty("Age", ageAverage),
				new JProperty("AngerCount", angerCount),
				new JProperty("ContemptCount", contemptCount),
				new JProperty("DisgustCount", disgustCount),
				new JProperty("FearCount", fearCount),
				new JProperty("HappinessCount", happinessCount),
				new JProperty("NeutralCount", neutralCount),
				new JProperty("SadnessCount", sadnessCount),
				new JProperty("SurpriseCount", surpriseCount));

			return results;
		}

	}
}
