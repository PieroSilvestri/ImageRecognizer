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

			this.oggetto = getJsonItem;
			string nome = oggetto["FirstName"].ToString();
			string cognome = oggetto["LastName"].ToString();

			string profileName = "Benvenuto " + nome + " " + cognome + "!";

			labelNome.Text = profileName;
		}

		public async void GoToEmotionpage(object o, EventArgs e)
		{
			int user_id = (int)oggetto["ID"];
			await Navigation.PushAsync(new EmotionPage(user_id));
		}
			

		public async void GoToProfile(object o, EventArgs e)
		{
			await Navigation.PushAsync(new ProfilePage(oggetto));
		}

		public async void GoToEmotionPage(object o, EventArgs e)
		{
			await Navigation.PushAsync(new EmotionPage(1));
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
			}
			else
			{
				LogoImage2.IsVisible = false;
				LogoImage.IsVisible = true;
			}

			i = !i;

		}

	}
}
