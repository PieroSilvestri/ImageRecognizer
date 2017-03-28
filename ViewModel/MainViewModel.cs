using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Media.Abstractions;

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
				try
				{
					return (JObject)JArray.Parse(responseContent.ToString()).First;

				}
				catch(Exception exc)
				{
					Debug.WriteLine(exc);
					return JObject.Parse(responseContent);
				}
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
			Debug.WriteLine(JObject.Parse(JsonResult));
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
			var url2 = @"https://image-recognizer-v1.herokuapp.com/api/v1/users/add";

			HttpClient client = new HttpClient();

			var content = new StringContent(newPerson.ToString(), null, "application/json");

			var response = await client.PostAsync(url2, content);
			try
			{
				response.EnsureSuccessStatusCode();
			}
			catch (Exception exc)
			{
				Debug.WriteLine("Error");
				Debug.WriteLine(exc);
				return -1;
			}

			var JsonResult = response.Content.ReadAsStringAsync().Result;
			Debug.WriteLine("KNOW YOUR ENEMIES");
			Debug.WriteLine(JsonResult);
			//var items = JsonConvert.ToString(JsonResult);

			JObject a = JObject.Parse(JsonResult);

			if ((bool)a["success"])
			{
				JObject body = (JObject)a["body"];
				int newId = (int)body["insertId"];

				return newId;
			}
			else
			{
				return -1;
			}
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

		public async Task<JArray> DetectFaceAndEmotionsAsync(EmotionServiceClient emotionServiceClient, MediaFile inputFile)
		{
			Emotion[] emotionResult = await emotionServiceClient.RecognizeAsync(inputFile.GetStream());
			if (emotionResult.Any())
			{
				// Emotions detected are happiness, sadness, surprise, anger, fear, contempt, disgust, or neutral.
				// emotionResult.FirstOrDefault().Scores.ToRankedList().FirstOrDefault().Key;
				var emozione = emotionResult.FirstOrDefault().Scores.ToRankedList().ToList();
				JArray a = JArray.Parse(JsonConvert.SerializeObject(emozione));

				return a;
			}
			else
			{
				Debug.WriteLine("Error");
				return null;
			}

		}

		public async Task<string> GetPeopleEmotions(string emotionKey, MediaFile imageFile)
		{
			var client = new HttpClient();

			// Request headers
			client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", emotionKey);

			string uri = "https://westus.api.cognitive.microsoft.com/emotion/v1.0/recognize?";
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
			}

			//A peak at the JSON response.

			return responseContent;
		}

		public async Task<string> MakeAnalysisRequest(string computerVisionKey, MediaFile imageFile)
		{
			var client = new HttpClient();

			// Request headers. Replace the second parameter with a valid subscription key.
			client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", computerVisionKey);

			// Request parameters. A third optional parameter is "details".
			string requestParameters = "visualFeatures=Categories,Faces&language=en";
			string uri = "https://westus.api.cognitive.microsoft.com/vision/v1.0/analyze?" + requestParameters;
			//Debug.WriteLine(uri);

			HttpResponseMessage response;

			// Request body. Try this sample with a locally stored JPEG image.
			byte[] byteData = GetImageAsByteArray(imageFile);

			using (var content = new ByteArrayContent(byteData))
			{
				// This example uses content type "application/octet-stream".
				// The other content types you can use are "application/json" and "multipart/form-data".
				content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
				response = await client.PostAsync(uri, content);

			}

			var JsonResult = response.Content.ReadAsStringAsync().Result;


			return JsonResult;
		}

		public async Task<bool> SendEmotions(JObject jsonToPass)
		{
			var url = @"http://l-raggioli2.eng.teorema.net/api/detect/";

			HttpClient client = new HttpClient();

			var content = new StringContent(jsonToPass.ToString(), null, "application/json");

			var response = await client.PostAsync(url, content);
			response.EnsureSuccessStatusCode();

			var JsonResult = response.Content.ReadAsStringAsync().Result;
			Debug.WriteLine("registration Request");
			//Debug.WriteLine(JsonResult);
			//var items = JsonConvert.ToString(JsonResult);

			JObject a = JObject.Parse(JsonResult);

			bool myFlag = (bool)a["success"];

			return myFlag;
		}

		public async Task<JObject> GetEmotionsReport(int userId)
		{
			var url = @"http://l-raggioli2.eng.teorema.net/api/detect/" + userId;
			HttpClient client = new HttpClient();

			var response = await client.GetAsync(url);
			response.EnsureSuccessStatusCode();

			var JsonResult = response.Content.ReadAsStringAsync().Result;
			//var items = JsonConvert.ToString(JsonResult);

			JObject a = JObject.Parse(JsonResult);

			return a;
		}

		public async Task<JObject> AddFaceToList(string faceKey, MediaFile imageFile)
		{
			var client = new HttpClient();

			// Request headers. Replace the second parameter with a valid subscription key.
			client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", faceKey);

			// Request parameters. A third optional parameter is "details".
			string requestParameters = "teorema_faces";
			string uri = "https://westus.api.cognitive.microsoft.com/face/v1.0/facelists/"+requestParameters+"/persistedFaces" ;
			//Debug.WriteLine(uri);

			HttpResponseMessage response;

			// Request body. Try this sample with a locally stored JPEG image.
			byte[] byteData = GetImageAsByteArray(imageFile);

			using (var content = new ByteArrayContent(byteData))
			{
				// This example uses content type "application/octet-stream".
				// The other content types you can use are "application/json" and "multipart/form-data".
				content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
				response = await client.PostAsync(uri, content);

			}

			var JsonResult = response.Content.ReadAsStringAsync().Result;


			return JObject.Parse(JsonResult);
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
