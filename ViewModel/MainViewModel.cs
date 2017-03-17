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

		private JObject getJsonItem;
		public JObject GetJsonItem
		{
			get
			{
				return getJsonItem;
			}
			set
			{
				getJsonItem = value;
				NotifyPropertyChanged();
			}
		}

		private string newLink;
		public string NewLink
		{
			get
			{
				return newLink;
			}
			set
			{
				newLink = value;
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

		public async Task<string> UploadDoc(MediaFile file)
		{
			using (var dbx = new DropboxClient("mRUyhXfE2KEAAAAAAAAIvIEHO5v-5iBbhBgRf5BYslj-bzR0fmjtC_NXLVi8xgfU"))
			{
				var full = await dbx.Users.GetCurrentAccountAsync();
				var fileName = DateTime.Now;
				return await Upload(dbx, @"/MyApp/imageRecognizer_v1", "ciaoATutti.jpg", file);
			}
		}

		async Task<string> Upload(DropboxClient dbx, string folder, string file, MediaFile content)
		{

			using (var mem = content.GetStream())
			{

				var updated = await dbx.Files.UploadAsync(
					folder + "/" + file,
					WriteMode.Overwrite.Instance,
					body: mem);

				Debug.WriteLine("Saved {0}/{1} rev {2}", folder, file, updated.Rev);

			}

			var newUrl = @"" + folder + "/" + file;
			Debug.WriteLine("newUrl");
			Debug.WriteLine(newUrl);
			return await GetDropboxResponse(newUrl);

		}

		public async Task<string> GetDropboxResponse(string myUrlPath)
		{
			HttpClient dropboxClient = new HttpClient();
			var url = @"https://api.dropboxapi.com/2/files/get_temporary_link";

			var param = JsonConvert.SerializeObject(new { path = myUrlPath });

			var myToken = @"mRUyhXfE2KEAAAAAAAAIvIEHO5v-5iBbhBgRf5BYslj-bzR0fmjtC_NXLVi8xgfU";

			var content = new StringContent(param, null, "application/json");
			dropboxClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" , myToken);
			//content.Headers.Add("Authorization", myToken);

			var response = await dropboxClient.PostAsync(url, content);
			response.EnsureSuccessStatusCode();

			var JsonResult = response.Content.ReadAsStringAsync().Result;
			Debug.WriteLine("DROPBOX JSONRESULT");
			Debug.WriteLine(JsonResult);
			//var items = JsonConvert.ToString(JsonResult);

			JObject a = JObject.Parse(JsonResult);

			Debug.WriteLine("DROPBOX JSONOBJECT");
			Debug.WriteLine(a);

			var newMyUrl = a["link"].ToString();

			return newMyUrl;
		}

		public async Task PostUrlToServer(string myUrl)
		{
			HttpClient client = new HttpClient();
			//client.BaseAddress = new Uri(url);

			var url = "http://l-raggioli2.eng.teorema.net/api/values/";
			//string trumpUrl = "https://dl.dropboxusercontent.com/apitl/1/AAA-vXk3UAyO45sE8upAMMp6wsYXdKps6JeJurLlftYGOuF55BDrLzXTniDTVUWfbWBeYlLOR0DmeGvN0wrWiXJELlhppN1vqcMBYXjWiCnAOBqw56WFb18M8YEfuJxQ1eqKbMeMbLS8fqz4TCiavrVA5ujktQkCTPJbeX5fJsTWJh88MGfP9Olcfr99OHqmEItzPb5yW7Eor7HTGqeoxiBguVheQ8XIKsUP0ZyRDEIHbTBtWVxqoVwxj4nPp-cj3ziqxa4jKXtkcPgRHRJNunuT".ToString();
			string tempUrl = @"'"+myUrl+"'";

			var content = new StringContent(tempUrl, null, "application/json");

			var response = await client.PostAsync(url, content);
			response.EnsureSuccessStatusCode();

			var JsonResult = response.Content.ReadAsStringAsync().Result;
			Debug.WriteLine("KNOW YOUR ENEMIES");
			Debug.WriteLine(JsonResult);
			//var items = JsonConvert.ToString(JsonResult);

			JObject a = JObject.Parse(JsonResult);
			var success = (Boolean) a["success"];
			if (success)
			{
				Debug.WriteLine("TRUE");
				await GetUserByFaceId(a["value"].ToString());
			}
			else
			{
				Debug.WriteLine("FALSE");
			}

			Debug.WriteLine("JSONPOSTPROVA");
			Debug.WriteLine(a);
		}

		public async Task GetUserByFaceId(string faceId)
		{

			var url = "http://l-raggioli2.eng.teorema.net/api/values/" + faceId;

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

			GetJsonItem = a;
		}

		public async Task JsonGetProva(string url)
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

			GetJsonItem = a;
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
