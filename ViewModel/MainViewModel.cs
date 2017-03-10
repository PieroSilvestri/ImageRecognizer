using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Net.Http;
using System.Diagnostics;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ImageRecognizer
{
	public class MainViewModel : INotifyPropertyChanged
	{

		private List<JsonItem> newJsonItemList;
		public List<JsonItem> NewJsonItemList
		{
			get
			{
				return newJsonItemList;
			}

			set
			{
				newJsonItemList = value;
				NotifyPropertyChanged();
			}
		}

		private int userId;
		public int UserId { get { return userId; } set { userId = value; NotifyPropertyChanged(); } }

		private int id;
		public int Id { get { return id; } set { id = value; NotifyPropertyChanged(); } }

		private int title;
		public int Title { get { return title; } set { title = value; NotifyPropertyChanged(); } }

		private int body;
		public int Body { get { return body; } set { body = value; NotifyPropertyChanged(); } }

		public async Task GetJsonResponse(string url)
		{
			HttpClient client = new HttpClient();
			client.BaseAddress = new Uri(url); ;

			var response = await client.GetAsync(client.BaseAddress);
			response.EnsureSuccessStatusCode();

			var JsonResult = response.Content.ReadAsStringAsync().Result;
			Debug.WriteLine("KNOW YOUR ENEMIES");
			//var items = JsonConvert.ToString(JsonResult);

			JArray a = JArray.Parse(JsonResult);

			List<JsonItem> myJsonList = new List<JsonItem>();

			foreach (JObject o in a.Children<JObject>())
			{

				Debug.WriteLine("Object");
				Debug.WriteLine(o);

				int newUserId = (int)o["userId"];
				int newId = (int)o["id"];
				string newTitle = o["title"].ToString();
				string newBody = o["body"].ToString();


				/*
				foreach (JProperty p in o.Properties())
				{
					string name = p.Name;
					string value = (string)p.Value;

					Debug.WriteLine(name + " -- " + value);
				}
				*/

				JsonItem newItem = new JsonItem();
				newItem.userId = newUserId;
				newItem.id = newId;
				newItem.title = newTitle;
				newItem.body = newBody;

				myJsonList.Add(newItem);
			}
			SetValues(myJsonList);
		}


		private void SetValues(List<JsonItem> myNewList)
		{
			NewJsonItemList = myNewList;
		}

		public MainViewModel()
		{
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
