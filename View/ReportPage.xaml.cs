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

		public ReportPage(int id)
		{
			//InitializeComponent();
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

				ReportItem reportItem = new ReportItem();
				reportItem.Success = (bool)myReport["success"];
				reportItem.Average_age = (int)myReport["average_age"];
				reportItem.Male_count = (int)myReport["male_count"];
				reportItem.Female_count = (int)myReport["female_count"];
				reportItem.Not_adult_count = (int)myReport["not_adult_count"];
				reportItem.Male_older_age = (int)myReport["male_older_age"];
				reportItem.Female_older_age = (int)myReport["female_older_age"];
				reportItem.Last_face_detected = myReport["last_face_detected"].ToString();
				reportItem.Number_of_face_detected_today = (int)myReport["number_of_face_detected_today"];
				reportItem.Total_face_detected = (int)myReport["total_face_detected"];
				reportItem.Description = myReport["description"].ToString();

				InitializeComponent();
				this.BindingContext = reportItem;
			}
			catch (Exception exc)
			{
				Debug.WriteLine("GetReportsException");
				Debug.WriteLine(exc);
				await DisplayAlert("Error", "There are no report", "Ok");
				await Navigation.PopAsync(true);
			}


		}
		/*
		  "success": "true",
		  "average_age": 23,
		  "male_count": 5,
		  "female_count": 0,
		  "not_adult_count": 0,
		  "male_older_age": 32,
		  "female_older_age": 0,
		  "male_younger_age": 21,
		  "female_younger_age": 0,
		  "last_face_detected": "3/22/2017 4:45:36 PM",
		  "number_of_face_detected_today": 5,
		  "total_face_detected": 5,
		  "description": "Data Analysis of ID: 18"
		*/
	}
}
