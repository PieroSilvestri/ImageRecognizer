using System;
using System.Collections.Generic;
using System.Diagnostics;
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
				int tot = (int)myReport["total_face_detected"];

				angerValue = ((int)myReport["anger_count"])/tot;
				contemptValue = ((int)myReport["contempt_count"]) / tot;
				disgustValue = ((int)myReport["disgust_count"]) / tot;
				fearValue = ((int)myReport["fear_count"]) / tot;
				happinessValue = ((int)myReport["happiness_count"]) / tot;
				neutralValue = ((int)myReport["neutral_count"]) / tot;
				sadnessValue = ((int)myReport["sadness_count"]) / tot;
				surpriseValue = ((int)myReport["surprise_count"]) / tot;

				/*
				 *   "total_face_detected": 4,
					  "anger_count": 0,
					  "contempt_count": 0,
					  "disgust_count": 1,
					  "fear_count": 0,
					  "happiness_count": 1,
					  "neutral_count": 2,
					  "sadness_count": 0,
					  "surprise_count": 0,
					  "top_emotion": "neutral"
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

		async void calculateEmotions(object o, EventArgs e)
		{ 
			await barHappy.ProgressTo(.7, 1000, Easing.Linear);
			await barFear.ProgressTo(.5, 1000, Easing.Linear);
			await barAnger.ProgressTo(.3, 1000, Easing.Linear);
			await barContempt.ProgressTo(contemptValue, 1000, Easing.Linear);
			await barDisgust.ProgressTo(disgustValue, 1000, Easing.Linear);
			await barSadness.ProgressTo(sadnessValue, 1000, Easing.Linear);
			await barSurprised.ProgressTo(surpriseValue, 1000, Easing.Linear);
			await barNeutral.ProgressTo(neutralValue, 1000, Easing.Linear);
		}
	}
}
/*
"total_face_detected": 4,
					  "anger_count": 0,
					  "contempt_count": 0,
					  "disgust_count": 1,
					  "fear_count": 0,
					  "happiness_count": 1,
					  "neutral_count": 2,
					  "sadness_count": 0,
					  "surprise_count": 0,
*/