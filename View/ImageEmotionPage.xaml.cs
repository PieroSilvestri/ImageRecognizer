using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
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

		private MainViewModel vm;
		private int user_id;
		private bool userFlag;
		List<ListItem> listNames = new List<ListItem>();
		static List<JObject> listItems = new List<JObject>();
		private JObject jsonToSend = new JObject();


		public ImageEmotionPage(bool flag, int id, MediaFile imageSource)
		{
			InitializeComponent();
			this.userFlag = flag;
			if (flag)
			{
				faceKey = "e5d1028e78c14c75b0e1ca0b30cb9d3e";
				emotionKey = "9384bddf115345fc94d27bf69723de98";
				computerVisionKey = "1ffcd7811f1d4980a936adc5aaf63dc8";
			}
			else
			{
				faceKey = "e5d1028e78c14c75b0e1ca0b30cb9d3e";
				emotionKey = "9384bddf115345fc94d27bf69723de98";
				computerVisionKey = "1ffcd7811f1d4980a936adc5aaf63dc8";
			}
			Image1.Source = ImageSource.FromStream(() => imageSource.GetStream()); ;
			//Ozecky sdk
			Title = "Emotion Page";
			user_id = id;
			vm = new MainViewModel();
			GetListReports(flag, id);
			this.faceServiceClient = new FaceServiceClient(faceKey);
			this.emotionServiceClient = new EmotionServiceClient(emotionKey);

			DoTheProgramm(imageSource);

			ToolbarItems.Add(new ToolbarItem("Add to list", null, async () =>
			{
				var page = new ContentPage();
			//var result = await page.DisplayAlert("Title", "Message", "Accept", "Cancel");
			int idLista = await InputBox(this.Navigation, listNames);
				Debug.WriteLine("success: {0}", idLista);
				if (idLista > 0)
				{
					Debug.WriteLine("IdLista: " + idLista);
					JObject jsonToBeSend = new JObject(
						new JProperty("success", true),
						new JProperty("faces", jsonToSend["faces"]),
						new JProperty("emotions", jsonToSend["emotions"]),
						new JProperty("id_user", jsonToSend["id_user"]),
						new JProperty("DateCreate", "44-44-44"),
						new JProperty("id_list", idLista));

					bool reportInserted = await vm.SendEmotions(flag, jsonToBeSend);
					if (reportInserted)
					{
						await DisplayAlert("Success", "Report inserted.", "OK");

					}
					else
					{
						await DisplayAlert("Error", "Something bad happened.", "OK");
					}
				}
				else
				{
					await DisplayAlert("Error", "New List not inserted.", "OK");
				}

			}));
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
				await Navigation.PushAsync(new PasswordPage(userFlag, null));
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


		private async void GetListReports(bool flag, int user_id)
		{

			JObject prova = await vm.GetListReport(flag, user_id);
			JArray listArray = new JArray();
			if (flag)
			{
				listArray = (JArray)prova["Lists"];

			}
			else
			{
				if ((bool)prova["success"])
				{
					listArray = (JArray)prova["body"];
				}
			}

			if (listArray.Count > 0)
			{
				foreach (JObject item in listArray)
				{
					listItems.Add(item);
					listNames.Add(new ListItem 
					{ 
						ListItemName = (string)item["Name"]
					});
				}
			}
			else
			{
				await DisplayAlert("Error", "No list found", "OK");
				await Navigation.PopAsync();
			}


		}

		private static JObject GetIdByString(string text)
		{
			foreach (JObject item in listItems)
			{
				if (item["Name"].ToString() == text)
				{
					return item;
				}
			}
			return null;
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

			string peopleEmotions = await vm.GetPeopleEmotions(userFlag, emotionKey, file);
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


			string imageAnalysis = await vm.MakeAnalysisRequest(userFlag, computerVisionKey, file);

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
				new JProperty("id_list", 0));
			Debug.WriteLine(jsonToPass);

			bool response = true;

			if (!faces.HasValues)
			{
				response = false;
			}
			else if (!scores.HasValues)
			{
				response = false;
			}
			/*
			else
			{
				response = await vm.SendEmotions(userFlag, jsonToPass);
			}
			*/

			if (response)
			{
				spinner.IsVisible = false;
				spinner.IsRunning = false;
				await DisplayAlert("Success!", "Everything has been done correctly.", "Ok");
				this.jsonToSend = jsonToPass;
				facesDetected.Text = "Face detected: " + faces.Count.ToString();
				facesDetected.IsVisible = true;
				ageInfo.IsVisible = true;
				maleFemale.IsVisible = true;
				JObject jsonEmotionDetected = SetEmotionUI(jsonToPass);
				Debug.WriteLine("JSON EMOTIUON DETECTED");
				Debug.WriteLine(jsonEmotionDetected);
				int personCount = (int)jsonEmotionDetected["PersonCount"];
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

		public static Task<int> InputBox(INavigation navigation, List<ListItem> listNames)
		{
			// wait in this proc, until user did his input 

			var tcs = new TaskCompletionSource<int>();

			var lblTitle = new Label { Text = "Add to list", HorizontalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

			var newList = new ListView();
			List<string> listStringNames = new List<string>();
			foreach (ListItem name in listNames)
			{
				listStringNames.Add(name.ListItemName);
			}
			newList.ItemsSource = listStringNames;

			newList.IsPullToRefreshEnabled = true;
			newList.ItemSelected += (sender, e) =>
			{
				if (e.SelectedItem == null)
				{
					// close page
					navigation.PopModalAsync();
					// pass empty result
					tcs.SetResult(-1); //ItemSelected is called on deselection, which results in SelectedItem being set to null
				}
				if(e.SelectedItem != null)
				{
					string itemSelected = (string)e.SelectedItem;
					Debug.WriteLine(itemSelected);

					JObject tempJ = GetIdByString(itemSelected);
					// close page
					navigation.PopModalAsync();
					tcs.SetResult((int)tempJ["ID_List"]);
				}
			};

			var btnCancel = new Button
			{
				Text = "Cancel",
				WidthRequest = 100,
				BackgroundColor = Color.FromRgb(0.8, 0.8, 0.8)
			};
			btnCancel.Clicked += async (s, e) =>
			{
				// close page
				await navigation.PopModalAsync();
				// pass empty result
				tcs.SetResult(-1);
			};

			var layout = new StackLayout
			{
				Padding = new Thickness(0, 40, 0, 0),
				VerticalOptions = LayoutOptions.StartAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Orientation = StackOrientation.Vertical,
				Children = { lblTitle, newList, btnCancel },
			};

			// create and show page
			var page = new ContentPage();
			page.Content = layout;
			navigation.PushModalAsync(page);
			// code is waiting her, until result is passed with tcs.SetResult() in btn-Clicked
			// then proc returns the result
			return tcs.Task;
		}

		public void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
			{
				return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
			}
			ListItem itemSelected = (ListItem)e.SelectedItem;
			Debug.WriteLine(itemSelected.ListItemName);

			JObject jItemSelected = GetIdByString(itemSelected.ListItemName);

			if (itemSelected != null)
			{
				//DisplayAlert("Ottimo", "Hai premuto " + itemSelected.ListItemName + " con l'id: " + idSelected, "OK");
				Navigation.PushAsync(new ReportPage(true, jItemSelected));
			}
			else
			{
				DisplayAlert("Error", "Valore non trovato.", "OK");

			}
		}

	}
}
