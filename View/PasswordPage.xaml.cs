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

		public JObject oggetto;

		//MainViewModel vm;
		//JArray myJsonArray;

		public PasswordPage(JObject getJsonItem)
		{
			
			//vm = new MainViewModel();
			InitializeComponent();
			this.Title = "Password";
			NavigationPage.SetHasNavigationBar(this, false);

			Debug.WriteLine("GetJsonItem");
			Debug.WriteLine(getJsonItem["results"]);

			JObject proviamo = getJsonItem;
			takeObject(proviamo);
			string nome = proviamo["FirstName"].ToString();
			string cognome = proviamo["LastName"].ToString();

			string profileName = "Benvenuto " + nome + " " + cognome + "!";

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



		async void backEvent(object sender, EventArgs e)
		{
			var disp = await DisplayAlert("Exit?", "Press 'yes' to logout", "Yes", "No");

			if (disp)
			{
				await Navigation.PopAsync(true);
			}



		}

		bool i = true;
		public void EasterEgg(object sender, EventArgs e)
		{

			if (i)
			{
				LogoImage.IsVisible = false;
				LogoImage2.IsVisible = true;

				i = false;
			}
			else
			{
				LogoImage2.IsVisible = false;
				LogoImage.IsVisible = true;

				i = true;
			}

		}

	}
}
