using System;
namespace ImageRecognizer
{
	public class ReportItem
	{
		public bool Success { get; set; }
		public double Average_age { get; set; }
		public int Male_count { get; set; }
		public int Female_count { get; set; }
		public int Not_adult_count { get; set; }
		public int Male_older_age { get; set; }
		public int Female_older_age { get; set; }
		public int Male_younger_age { get; set; }
		public int Female_younger_age { get; set; }
		public string Last_face_detected { get; set; }
		public int Number_of_face_detected_today { get; set; }
		public int Total_face_detected { get; set; }
		public string Description { get; set; }
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