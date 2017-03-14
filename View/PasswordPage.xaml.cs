using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace ImageRecognizer
{
	public partial class PasswordPage : ContentPage
	{

		MainViewModel vm;

		public PasswordPage(string pathImageLogin)
		{
			vm = new MainViewModel();
			InitializeComponent();
			this.Title = "Password";

			ProfilePicture.Source = pathImageLogin;

		}

		public async void DoLoginWithPassword(object o, EventArgs e)
		{
			var password = Password.Text;

			await DisplayAlert("Login Effettuato!", "Con la password" + password + " hai fatto il Login.", "OK");

			string url = "http://jsonplaceholder.typicode.com/posts";

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

				await Navigation.PushAsync(new ListPage_v1(newList));
			}
		}
	}
}
