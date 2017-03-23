using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImageRecognizer
{
	public partial class ProfilePage : ContentPage
	{

		public ProfilePage(JObject oggetto)
		{
			InitializeComponent();
			NavigationPage.SetHasNavigationBar(this, false);
			profileImage.Source = "http://placehold.it/350x350";



			/*    "ID": 1110,
				  "LastName": "Scocchi",
				  "FirstName": "Andrea",
				  "UserName": "AndreaScocchi",
				  "Password": null,
				  "Age": 22,
				  "DataRegistration": "3/23/2017 2:11:59 PM",
				  "Birthday": null,
				  "Role": null,
				  "Email": null,
				  "success": "true"   */


			string nome = oggetto["FirstName"].ToString();
			string cognome = oggetto["LastName"].ToString();
			//string age = oggetto["Age"].ToString();
			string username = oggetto["UserName"].ToString();
			//string dataReg = oggetto["DataRegistration"].ToString();
			string birthday = oggetto["Birthday"].ToString();
			string role = oggetto["Role"].ToString();
			string email = oggetto["Email"].ToString();

			this.Title = username;

			labelNome.Text = nome + " " + cognome;
			//labelAge.Text = age;
			//labelDataReg.Text = dataReg;
			labelBirthday.Text = birthday;
			labelRole.Text = role;
			labelEmail.Text = email;

		}

		void backEvent(object sender, EventArgs e)
		{
			Navigation.PopAsync(true);
		}

	}
}
