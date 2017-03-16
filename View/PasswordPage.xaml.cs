using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImageRecognizer
{
	public partial class PasswordPage : ContentPage
	{

		JObject oggetto;
		//MainViewModel vm;
		//JArray myJsonArray;

		public PasswordPage(JObject getJsonItem)
		{
			//vm = new MainViewModel();
			InitializeComponent();
			this.Title = "Password";

			ProfilePicture.Source = "";

			Debug.WriteLine("GetJsonItem");
			Debug.WriteLine(getJsonItem["results"]);

			JArray myJsonArray = JArray.Parse(getJsonItem["results"].ToString());

			JObject proviamo = (JObject) myJsonArray.First;
			takeObject(proviamo);
			string nome = proviamo["FirstName"].ToString();
			string cognome = proviamo["LastName"].ToString();

			string profileName = nome + " " + cognome + " inserisci la tua password:";

			labelNome.Text = profileName;
		}

		public void takeObject(JObject obj)
		{
			oggetto = obj;
			Debug.WriteLine("oggetto:");
			Debug.WriteLine(oggetto);
		}

		public async void DoLoginWithPassword(object o, EventArgs e)
		{
			var password = Password.Text;

			await DisplayAlert("Login Effettuato!", "Con la password " + password + " hai fatto il Login.", "OK");

			await Navigation.PushAsync(new ProfilePage(oggetto));
			/*string url = "http://jsonplaceholder.typicode.com/posts";

			try
			{
				await vm.GetJsonResponse(url);

			}
			catch (Exception exc)
			{
				Debug.WriteLine("ERROR EXCEPTION");
				Debug.WriteLine(exc);
			}

			if (vm.NewJsonItemList != null)
			{
				List<JsonItem> newList = vm.NewJsonItemList;

			}*/
		}



		bool i = true;
		public void EasterEgg(object sender, EventArgs e)
		{

			if (i == true)
			{
				LogoImage.Source = ImageSource.FromFile("NewLogo2.png");
				i = false;
			}
			else
			{
				LogoImage.Source = ImageSource.FromFile("NewLogo.png");

				i = true;
			}

		}



	}
}
