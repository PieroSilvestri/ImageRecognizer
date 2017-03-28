using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace ImageRecognizer
{
	public partial class ReportPage : ContentPage
	{

		private int user_id;
		MainViewModel vm;
		double angerValue;
		double contemptValue;
		double disgustValue;
		double fearValue;
		double happinessValue;
		double neutralValue;
		double sadnessValue;
		double surpriseValue;
		string topEmotion;


		public ReportPage(int id)
		{
			InitializeComponent();
			vm = new MainViewModel();
			Title = "Report";
			this.user_id = id;
			GetReports();

		}

		public async void GetReports()
		{
			try
			{
				JObject myReport = await vm.GetEmotionsReport(user_id);

				Debug.WriteLine("GetEmotionsReport");
				Debug.WriteLine(myReport);

				maleCounter.Text = myReport["male_count"].ToString();
				femaleCounter.Text = myReport["female_count"].ToString();
				childCounter.Text = myReport["not_adult_count"].ToString();
				maleOldest.Text = myReport["male_older_age"].ToString();
				femaleOldest.Text = myReport["female_older_age"].ToString();
				maleYoungest.Text = myReport["male_younger_age"].ToString();
				femaleYoungest.Text = myReport["female_younger_age"].ToString();

				ageAverage.Text = "Average: " + (myReport["average_age"]).ToString();

				double tot = (double)myReport["total_face_detected"];

				angerValue = ((double)myReport["anger_count"])/tot;
				contemptValue = ((double)myReport["contempt_count"]) / tot;
				disgustValue = ((double)myReport["disgust_count"]) / tot;
				fearValue = ((double)myReport["fear_count"]) / tot;
				happinessValue = ((double)myReport["happiness_count"]) / tot;
				neutralValue = ((double)myReport["neutral_count"]) / tot;
				sadnessValue = ((double)myReport["sadness_count"]) / tot;
				surpriseValue = ((double)myReport["surprise_count"]) / tot;

				happyLabel.Text = "Happiness: " + (int)myReport["happiness_count"];
				surprisedLabel.Text = "Surprised: " + (int)myReport["surprise_count"];
				neutralLabel.Text = "Neutral: " + (int)myReport["neutral_count"];
				sadLabel.Text = "Sadness: " + (int)myReport["sadness_count"];
				fearLabel.Text = "Fear: " + (int)myReport["fear_count"];
				disgustLabel.Text = "Disgust: " + (int)myReport["disgust_count"];
				contemptLabel.Text = "Contempt: " + (int)myReport["contempt_count"];
				angerLabel.Text = "Anger: " + (int)myReport["anger_count"];

				topEmotion = myReport["top_emotion"].ToString();
				Debug.WriteLine("TOP: " + topEmotion);
				if (topEmotion == "happiness")
				{
					happyTab.BackgroundColor = Color.FromHex("#1A80FD");
					happyLabel.TextColor = Color.White;
				}
				if (topEmotion == "neutral")
				{
					neutralTab.BackgroundColor = Color.FromHex("#1A80FD");
					neutralLabel.TextColor = Color.White;
				}
				if (topEmotion == "fear")
				{
					fearTab.BackgroundColor = Color.FromHex("#1A80FD");
					fearLabel.TextColor = Color.White;
				}
				if (topEmotion == "anger")
				{
					angerTab.BackgroundColor = Color.FromHex("#1A80FD");
					angerLabel.TextColor = Color.White;
				}
				if (topEmotion == "disgust")
				{
					disgustTab.BackgroundColor = Color.FromHex("#1A80FD");
					disgustLabel.TextColor = Color.White;
				}
				if (topEmotion == "contempt")
				{
					contemptTab.BackgroundColor = Color.FromHex("#1A80FD");
					contemptLabel.TextColor = Color.White;
				}
				if (topEmotion == "sadness")
				{
					sadTab.BackgroundColor = Color.FromHex("#1A80FD");
					sadLabel.TextColor = Color.White;
				}
				if (topEmotion == "surprise")
				{
					surprisedTab.BackgroundColor = Color.FromHex("#1A80FD");
					surprisedLabel.TextColor = Color.White;
				}

				calculateEmotions();
				/*
				  "success": "true",
  "average_age": 27,
  "male_count": 20,
  "female_count": 0,
  "not_adult_count": 0,
  "male_older_age": 32,
  "female_older_age": 0,
  "male_younger_age": 22,
  "female_younger_age": 0,
  "last_face_detected": "3/24/2017 4:00:52 PM",
  "number_of_face_detected_today": 0,
  "total_face_detected": 20,
  "anger_count": 0,
  "contempt_count": 0,
  "disgust_count": 0,
  "fear_count": 0,
  "happiness_count": 13,
  "neutral_count": 6,
  "sadness_count": 1,
  "surprise_count": 0,
  "top_emotion": "happiness",
  "description": "Data Analysis of ID: 1110"
}
				 * 
				 * 
				 * reportItem.Success = (bool)myReport["success"];
				reportItem.Male_count = (int)myReport["male_count"];
				reportItem.Female_count = (int)myReport["female_count"];
				reportItem.Not_adult_count = (int)myReport["not_adult_count"];
				reportItem.Male_older_age = (int)myReport["male_older_age"];
				reportItem.Female_older_age = (int)myReport["female_older_age"];
				reportItem.Last_face_detected = myReport["last_face_detected"].ToString();
				reportItem.Number_of_face_detected_today = (int)myReport["number_of_face_detected_today"];
				reportItem.Total_face_detected = (int)myReport["total_face_detected"];
				reportItem.Description = myReport["description"].ToString();
*/

				//this.BindingContext = reportItem;
			}
			catch (Exception exc)
			{
				Debug.WriteLine("GetReportsException");
				Debug.WriteLine(exc);
				await DisplayAlert("Error", "There are no report", "Ok");
				await Navigation.PopAsync(true);
			}


		}

		async void calculateEmotions()
		{
			await Task.Delay(1000); 
			if (happinessValue != 0) { await barHappy.ProgressTo(happinessValue, 750, Easing.Linear); }
			if (surpriseValue != 0) { await barSurprised.ProgressTo(surpriseValue, 750, Easing.Linear); }
			if (neutralValue != 0) { await barNeutral.ProgressTo(neutralValue, 750, Easing.Linear); }
			if (sadnessValue != 0) { await barSadness.ProgressTo(sadnessValue, 750, Easing.Linear); }
			if (fearValue != 0) { await barFear.ProgressTo(fearValue, 750, Easing.Linear); }
			if (disgustValue != 0) { await barDisgust.ProgressTo(disgustValue, 750, Easing.Linear); }
			if (contemptValue != 0) { await barContempt.ProgressTo(contemptValue, 750, Easing.Linear); }
			if (angerValue != 0) { await barAnger.ProgressTo(angerValue, 750, Easing.Linear); }
		}
	}
}

