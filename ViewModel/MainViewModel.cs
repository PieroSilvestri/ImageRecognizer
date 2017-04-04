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

		public async Task<JObject> MakeDetectRequest(bool userFlag, MediaFile imageFile)
		{
			var client = new HttpClient();

			string faceKey;
			if (userFlag)
			{
				faceKey = "1297619ba72542d38347e044905ed499";
			}
			else
			{
				faceKey = "e5d1028e78c14c75b0e1ca0b30cb9d3e";
			}

			// Request headers - replace this example key with your valid key.
			client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", faceKey);

			// Request parameters and URI string.
			string queryString = "returnFaceId=true";
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
		}

		public async Task<string> FindSimilarFace(string newFaceId)
		{
			var client = new HttpClient();

			JObject jsonToPass = new JObject(
				new JProperty("faceId",newFaceId),
				new JProperty("faceListId","face_list_v3"),  
				new JProperty("maxNumOfCandidatesReturned",10),
				new JProperty("mode", "matchFace"));
			
			// Request headers - replace this example key with your valid key.
			client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "e5d1028e78c14c75b0e1ca0b30cb9d3e");

			var content = new StringContent(jsonToPass.ToString(), null, "application/json");

			var response = await client.PostAsync("https://westus.api.cognitive.microsoft.com/face/v1.0/findsimilars", content);
			response.EnsureSuccessStatusCode();

			var JsonResult = response.Content.ReadAsStringAsync().Result;
			Debug.WriteLine("KNOW YOUR ENEMIES");
			Debug.WriteLine(JsonResult);
			JArray facesFound = JArray.Parse(JsonResult);
			double max = 0;
			JObject faceFound = new JObject();
			string faceId = "";
			foreach (JObject face in facesFound)
			{
				if ((double)face["confidence"] > max)
				{
					max = (double)face["confidence"];
					faceId = face["persistedFaceId"].ToString();
				}
			}


			return faceId;
		}

		public async Task<JObject> PostFaceIdToServer(bool userFlag, string newFaceId)
		{
			HttpClient client = new HttpClient();
			//client.BaseAddress = new Uri(url);

			string url;
			if (userFlag)
			{
				url = "http://l-raggioli2.eng.teorema.net/api/values/";
				var content = new StringContent(url, null, "application/json");

				var response = await client.PostAsync(url, content);
				response.EnsureSuccessStatusCode();

				var JsonResult = response.Content.ReadAsStringAsync().Result;
				Debug.WriteLine("KNOW YOUR ENEMIES");
				Debug.WriteLine(JsonResult);
				//var items = JsonConvert.ToString(JsonResult);

				JObject a = JObject.Parse(JsonResult);
				Debug.WriteLine("JSONPOSTPROVA");
				Debug.WriteLine(a);

				var success = (Boolean)a["success"];
				if (success)
				{
					Debug.WriteLine("TRUE");
					return await GetUserByFaceId(userFlag, a["value"].ToString());
				}
			}
			else
			{
				string userFaceId = await FindSimilarFace(newFaceId);
				return await GetUserByFaceId(userFlag, userFaceId);
			}

			//string trumpUrl = "https://dl.dropboxusercontent.com/apitl/1/AAA-vXk3UAyO45sE8upAMMp6wsYXdKps6JeJurLlftYGOuF55BDrLzXTniDTVUWfbWBeYlLOR0DmeGvN0wrWiXJELlhppN1vqcMBYXjWiCnAOBqw56WFb18M8YEfuJxQ1eqKbMeMbLS8fqz4TCiavrVA5ujktQkCTPJbeX5fJsTWJh88MGfP9Olcfr99OHqmEItzPb5yW7Eor7HTGqeoxiBguVheQ8XIKsUP0ZyRDEIHbTBtWVxqoVwxj4nPp-cj3ziqxa4jKXtkcPgRHRJNunuT".ToString();
			string tempUrl = @"'"+newFaceId+"'";



			Debug.WriteLine("FALSE");
			return null;
		}

		public async Task<JObject> GetUserByFaceId(bool userFlag, string faceId)
		{

			HttpClient client = new HttpClient();
			string url = "";

			if (userFlag)
			{
				url = "http://l-raggioli2.eng.teorema.net/api/values/" + faceId;

				client.BaseAddress = new Uri(url); ;

				var response = await client.GetAsync(client.BaseAddress);
				response.EnsureSuccessStatusCode();

				var JsonResult = response.Content.ReadAsStringAsync().Result;
				Debug.WriteLine("KNOW YOUR ENEMIES");
				Debug.WriteLine(JObject.Parse(JsonResult));
				//var items = JsonConvert.ToString(JsonResult);

				return JObject.Parse(JsonResult);
			}
			else
			{
				url = "https://image-recognizer-v1.herokuapp.com/api/v1/users/login";
				JObject tempJ = new JObject(
					new JProperty("faceId", faceId));
				var content = new StringContent(tempJ.ToString(), null, "application/json");

				var response = await client.PostAsync(url, content);
				response.EnsureSuccessStatusCode();

				var JsonResult = response.Content.ReadAsStringAsync().Result;
				Debug.WriteLine("KNOW YOUR ENEMIES");
				Debug.WriteLine(JsonResult);
				//var items = JsonConvert.ToString(JsonResult);

				JObject a = JObject.Parse(JsonResult);
				Debug.WriteLine("JSONPOSTPROVA");
				Debug.WriteLine(a);

				return a;
			}

		}


		public async Task<JObject> JsonGetProva(string url)
		{
			HttpClient client = new HttpClient();
			client.BaseAddress = new Uri(url); ;

			var response = await client.GetAsync(client.BaseAddress);
			try
			{
				response.EnsureSuccessStatusCode();
			}
			catch
			{
				return null;
			}

			var JsonResult = response.Content.ReadAsStringAsync().Result;
			Debug.WriteLine("KNOW YOUR ENEMIES");
			//var items = JsonConvert.ToString(JsonResult);

			JObject a = JObject.Parse(JsonResult);

			Debug.WriteLine("JSONGETPROVA");
			Debug.WriteLine(a);

			return a;
		}


		public async Task<int> CreateANewUser(bool userFlag, JObject newPerson)
		{

			string url; 

			if (userFlag)
			{
				url = @"http://l-raggioli2.eng.teorema.net/api/registration/";
			}
			else
			{
				url = @"https://image-recognizer-v1.herokuapp.com/api/v1/users/add";
			}

			HttpClient client = new HttpClient();

			var content = new StringContent(newPerson.ToString(), null, "application/json");

			var response = await client.PostAsync(url, content);
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

			if (userFlag)
			{
				if ((bool)a["success"])
				{
					int newId = (int)a["value"];

					return newId;
				}
			}
			else
			{
				if ((bool)a["success"])
				{
					JObject body = (JObject)a["body"];
					int newId = (int)body["insertId"];

					return newId;
				}
			}

			return -1;

		}

		public async Task<bool> RegistrationRequest(bool userFlag, JObject newJson)
		{
			string url;

			if (userFlag)
			{
				url = @"http://l-raggioli2.eng.teorema.net/api/url/";

			}
			else
			{
				url = @"https://image-recognizer-v1.herokuapp.com/api/v1/users/add/images";
			}

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

		public async Task<JObject> GetListReport(bool userFlag, int user_id)
		{

			var url = "http://l-raggioli2.eng.teorema.net/api/list/" + user_id;

			HttpClient client = new HttpClient();
			client.BaseAddress = new Uri(url); ;

			var response = await client.GetAsync(client.BaseAddress);
			var status = response.EnsureSuccessStatusCode();

			var JsonResult = response.Content.ReadAsStringAsync().Result;
			Debug.WriteLine("KNOW YOUR ENEMIES");
			Debug.WriteLine(JObject.Parse(JsonResult));
			//var items = JsonConvert.ToString(JsonResult);

			return JObject.Parse(JsonResult);
		}

		public async Task<JObject> CreateNewList(bool userFlag, string newListName, int user_id)
		{
			var url = @"http://l-raggioli2.eng.teorema.net/api/list/";

			HttpClient client = new HttpClient();

			JObject jsonToPass = new JObject(
				new JProperty("User_id", user_id),
				new JProperty("Name", newListName),
				new JProperty("DataCreation", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")));

			var content = new StringContent(jsonToPass.ToString(), null, "application/json");

			var response = await client.PostAsync(url, content);
			var status = response.EnsureSuccessStatusCode();

			var JsonResult = response.Content.ReadAsStringAsync().Result;
			Debug.WriteLine("Create new list Request");
			//Debug.WriteLine(JsonResult);
			//var items = JsonConvert.ToString(JsonResult);

			JObject a = JObject.Parse(JsonResult);

			return a;
		}

		public async Task<JArray> DetectFaceAndEmotionsAsync(bool userFlag, EmotionServiceClient emotionServiceClient, MediaFile inputFile)
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

		public async Task<string> GetPeopleEmotions(bool userFlag, string emotionKey, MediaFile imageFile)
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

		public async Task<string> MakeAnalysisRequest(bool userFlag, string computerVisionKey, MediaFile imageFile)
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

		public async Task<bool> SendEmotions(bool userFlag, JObject jsonToPass)
		{
			
			string url = "";
			if (userFlag)
			{
				url = @"http://l-raggioli2.eng.teorema.net/api/detect/";
			}
			else
			{
				url = "";
			}	

			HttpClient client = new HttpClient();

			var content = new StringContent(jsonToPass.ToString(), null, "application/json");

			var response = await client.PostAsync(url, content);
			response.EnsureSuccessStatusCode();

			var JsonResult = response.Content.ReadAsStringAsync().Result;
			Debug.WriteLine("registration Request");
			Debug.WriteLine(JsonResult);
			//var items = JsonConvert.ToString(JsonResult);

			JObject a = JObject.Parse(JsonResult);

			bool myFlag = (bool)a["success"];

			return myFlag;
		}

		public async Task<JObject> GetEmotionsReport(bool userFlag, int listId)
		{
			var url = @"http://l-raggioli2.eng.teorema.net/api/report/" + listId;
			HttpClient client = new HttpClient();

			var response = await client.GetAsync(url);
			response.EnsureSuccessStatusCode();

			var JsonResult = response.Content.ReadAsStringAsync().Result;
			//var items = JsonConvert.ToString(JsonResult);

			JObject a = JObject.Parse(JsonResult);

			return a;
		}

		public async Task<JObject> AddFaceToList(bool userFlag, string faceKey, MediaFile imageFile)
		{
			var client = new HttpClient();

			// Request headers. Replace the second parameter with a valid subscription key.
			client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", faceKey);

			// Request parameters. A third optional parameter is "details".
			string listName;
			if (userFlag)
			{
				listName = "teorema_faces";

			}
			else
			{
				listName = "face_list_v3";
			}
			string uri = "https://westus.api.cognitive.microsoft.com/face/v1.0/facelists/"+listName+"/persistedFaces" ;
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
