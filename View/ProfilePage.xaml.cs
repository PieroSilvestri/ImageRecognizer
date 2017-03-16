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

			profileImage.Source = "http://placehold.it/350x350";

			string nome = oggetto["FirstName"].ToString();
			string cognome = oggetto["LastName"].ToString();

			this.Title = nome + " " + cognome;

			labelNome.Text = nome + " " + cognome;
			labelAge.Text = oggetto["Age"].ToString();

		}
	}
}
