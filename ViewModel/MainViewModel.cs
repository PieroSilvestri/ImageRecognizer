using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Net.Http;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http.Headers;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Plugin.Media;
using System.Text;
using Dropbox;
using Dropbox.Api;
using Dropbox.Api.Files;
using Plugin.Media.Abstractions;
using Xamarin.Forms;


namespace ImageRecognizer
{
	public class MainViewModel : INotifyPropertyChanged
	{

		public MainViewModel()
		{
		}


		static byte[] GetImageAsByteArray(MediaFile imageFile)
		{
			BinaryReader binaryReader = new BinaryReader(imageFile.GetStream());
			return binaryReader.ReadBytes((int)imageFile.GetStream().Length);
		}

		public async Task JsonPostProva() 
		{
			HttpClient client = new HttpClient();
			//client.BaseAddress = new Uri(url);

			var url = "http://l-raggioli2.eng.teorema.net/api/values/";
			string trumpUrl = @"'http://st.ilfattoquotidiano.it/wp-content/uploads/2017/01/trump-don-675-675x275.jpg'";

			var content = new StringContent(trumpUrl, null, "application/json");

			var response = await client.PostAsync(url, content);
			response.EnsureSuccessStatusCode();

			var JsonResult = response.Content.ReadAsStringAsync().Result;
			Debug.WriteLine("KNOW YOUR ENEMIES");
			Debug.WriteLine(JsonResult);
			//var items = JsonConvert.ToString(JsonResult);

			JObject a = (JObject) JArray.Parse(JsonResult).First;

			Debug.WriteLine("JSONPOSTPROVA");
			Debug.WriteLine(a);
		}

		public async Task<JObject> MakeDetectRequest(MediaFile imageFile)
		{
			var client = new HttpClient();

			// Request headers - replace this example key with your valid key.
			client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "1297619ba72542d38347e044905ed499");

			// Request parameters and URI string.
			string queryString = "returnFaceId=true&returnFaceLandmarks=false&returnFaceAttributes=age,gender";
			string uri = "https://westus.api.cognitive.microsoft.com/face/v1.0/detect?" + queryString;

			HttpResponseMessage response;
			string responseContent;

			// Request body. Try this sample with a locally stored JPEG image.
			byte[] byteData = GetImageAsByteArray(imageFile);

			using (var content = new ByteArrayContent(byteData))
			{
				// This example uses content type "application/octet-stream".
				// The other content types you can use are "application/json" and "multipart/form-data".
				content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
				response = await client.PostAsync(uri, content);
				responseContent = response.Content.ReadAsStringAsync().Result;

				Debug.WriteLine(responseContent);
				return (JObject) JArray.Parse(responseContent.ToString()).First;
			}

			//A peak at the JSON response.
			Debug.WriteLine(responseContent);
			return null;
		}

		public async Task<JObject> PostFaceIdToServer(string newFaceId)
		{
			HttpClient client = new HttpClient();
			//client.BaseAddress = new Uri(url);

			var url = "http://l-raggioli2.eng.teorema.net/api/values/";
			//string trumpUrl = "https://dl.dropboxusercontent.com/apitl/1/AAA-vXk3UAyO45sE8upAMMp6wsYXdKps6JeJurLlftYGOuF55BDrLzXTniDTVUWfbWBeYlLOR0DmeGvN0wrWiXJELlhppN1vqcMBYXjWiCnAOBqw56WFb18M8YEfuJxQ1eqKbMeMbLS8fqz4TCiavrVA5ujktQkCTPJbeX5fJsTWJh88MGfP9Olcfr99OHqmEItzPb5yW7Eor7HTGqeoxiBguVheQ8XIKsUP0ZyRDEIHbTBtWVxqoVwxj4nPp-cj3ziqxa4jKXtkcPgRHRJNunuT".ToString();
			string tempUrl = @"'"+newFaceId+"'";

			var content = new StringContent(tempUrl, null, "application/json");

			var response = await client.PostAsync(url, content);
			response.EnsureSuccessStatusCode();

			var JsonResult = response.Content.ReadAsStringAsync().Result;
			Debug.WriteLine("KNOW YOUR ENEMIES");
			Debug.WriteLine(JsonResult);
			//var items = JsonConvert.ToString(JsonResult);

			JObject a = JObject.Parse(JsonResult);
			Debug.WriteLine("JSONPOSTPROVA");
			Debug.WriteLine(a);

			var success = (Boolean) a["success"];
			if (success)
			{
				Debug.WriteLine("TRUE");
				return await GetUserByFaceId(a["value"].ToString());
			}

			Debug.WriteLine("FALSE");
			return null;
		}

		public async Task<JObject> GetUserByFaceId(string faceId)
		{

			var url = "http://l-raggioli2.eng.teorema.net/api/values/" + faceId;

			HttpClient client = new HttpClient();
			client.BaseAddress = new Uri(url); ;

			var response = await client.GetAsync(client.BaseAddress);
			response.EnsureSuccessStatusCode();

			var JsonResult = response.Content.ReadAsStringAsync().Result;
			Debug.WriteLine("KNOW YOUR ENEMIES");
			//var items = JsonConvert.ToString(JsonResult);

			return JObject.Parse(JsonResult);
		}


		public async Task<JObject> JsonGetProva(string url)
		{
			HttpClient client = new HttpClient();
			client.BaseAddress = new Uri(url); ;

			var response = await client.GetAsync(client.BaseAddress);
			response.EnsureSuccessStatusCode();

			var JsonResult = response.Content.ReadAsStringAsync().Result;
			Debug.WriteLine("KNOW YOUR ENEMIES");
			//var items = JsonConvert.ToString(JsonResult);

			JObject a = JObject.Parse(JsonResult);

			Debug.WriteLine("JSONGETPROVA");
			Debug.WriteLine(a);

			return a;
		}


		public async Task<int> CreateANewUser(JObject newPerson)
		{

			var url = @"http://l-raggioli2.eng.teorema.net/api/registration/";

			HttpClient client = new HttpClient();

			var content = new StringContent(newPerson.ToString(), null, "application/json");

			var response = await client.PostAsync(url, content);
			response.EnsureSuccessStatusCode();

			var JsonResult = response.Content.ReadAsStringAsync().Result;
			Debug.WriteLine("KNOW YOUR ENEMIES");
			Debug.WriteLine(JsonResult);
			//var items = JsonConvert.ToString(JsonResult);

			JObject a = JObject.Parse(JsonResult);

			int newId = (int)a["value"];

			return newId;
			/*
			var success = (Boolean)a["success"];
			if (success)
			{
				Debug.WriteLine("TRUE");
				await GetUserByFaceId(a["persistedFaceId"].ToString());
			}
			else
			{
				Debug.WriteLine("FALSE");
			}

			Debug.WriteLine("JSONPOSTPROVA");
			Debug.WriteLine(a);
			*/
		}

		public async Task<bool> RegistrationRequest(JObject newJson)
		{
			var url = @"http://l-raggioli2.eng.teorema.net/api/url/";

			HttpClient client = new HttpClient();

			var content = new StringContent(newJson.ToString(), null, "application/json");

			var response = await client.PostAsync(url, content);
			response.EnsureSuccessStatusCode();

			var JsonResult = response.Content.ReadAsStringAsync().Result;
			Debug.WriteLine("registration Request");
			Debug.WriteLine(JsonResult);
			//var items = JsonConvert.ToString(JsonResult);

			JObject a = JObject.Parse(JsonResult);

			bool myFlag = (bool) a["success"];

			return myFlag;
		}

		/*

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

		*/


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
