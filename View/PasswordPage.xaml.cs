using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

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

			loadDelayAnimation();

		}

		public async void GoToEmotionPage(object o, EventArgs e)
		{
			int user_id = (int)oggetto["ID"];
			await Navigation.PushAsync(new EmotionPage(user_id));
		}
			

		public async void GoToProfile(object o, EventArgs e)
		{
			await Navigation.PushAsync(new ProfilePage(oggetto));
		}

		async void backEvent(object sender, EventArgs e)
		{
			var disp = await DisplayAlert("Exit?", "Press 'yes' to logout", "Yes", "No");

			if (disp)
			{
				await Navigation.PopAsync(true);
			}



		}



		async void loadDelayAnimation()
		{
			profileIcon.Opacity = 0;
			emoticonIcon.Opacity = 0;
			prova1.Opacity = 0;
			prova2.Opacity = 0;
			feedbackIcon.Opacity = 0;
			logoutIcon.Opacity = 0;
			labelNome.Opacity = 0;
			await Task.Delay(500);
			await labelNome.FadeTo(1, 200);
			await profileIcon.FadeTo(1, 200);
			await emoticonIcon.FadeTo(1, 200);
			await prova1.FadeTo(1, 200);
			await prova2.FadeTo(1, 200);
			await feedbackIcon.FadeTo(1, 200);
			await logoutIcon.FadeTo(1, 200);
		}

	}
}
