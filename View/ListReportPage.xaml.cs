using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace ImageRecognizer
{
	public partial class ListReportPage : ContentPage
	{

		private MainViewModel vm;
		ObservableCollection<ListItem> listNames = new ObservableCollection<ListItem>();


		public ListReportPage(int id)
		{
			InitializeComponent();
			vm = new MainViewModel();
			ListViewReports.ItemsSource = listNames;

			GetListReports(id);

			ToolbarItems.Add(new ToolbarItem("Add list", null, async () =>
			{
				var page = new ContentPage();
				//var result = await page.DisplayAlert("Title", "Message", "Accept", "Cancel");
				string entryString = await InputBox(this.Navigation);
				Debug.WriteLine("success: {0}", entryString);
				bool flag = await vm.CreateNewList(entryString, id);
				if (flag)
				{
					listNames.Add(new ListItem { ListName = entryString });
				}
				else
				{
					await DisplayAlert("Error", "New List not inserted.", "OK");
				}
			}));
		}

		private async void GetListReports(int user_id)
		{
			JObject prova = await vm.GetListReport(user_id);
		}


		public static Task<string> InputBox(INavigation navigation)
		{
			// wait in this proc, until user did his input 
			var tcs = new TaskCompletionSource<string>();

			var lblTitle = new Label { Text = "Add list", HorizontalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
			var txtInput = new Entry { Placeholder = "Enter the name" };

			var btnOk = new Button
			{
				Text = "Ok",
				WidthRequest = 100,
				BackgroundColor = Color.FromRgb(0.8, 0.8, 0.8),
			};
			btnOk.Clicked += async (s, e) =>
			{
				// close page
				var result = txtInput.Text;
				await navigation.PopModalAsync();
				// pass result
				tcs.SetResult(result);
			};

			var btnCancel = new Button
			{
				Text = "Cancel",
				WidthRequest = 100,
				BackgroundColor = Color.FromRgb(0.8, 0.8, 0.8)
			};
			btnCancel.Clicked += async (s, e) =>
			{
				// close page
				await navigation.PopModalAsync();
				// pass empty result
				tcs.SetResult(null);
			};

			var slButtons = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Children = { btnOk, btnCancel },
			};

			var layout = new StackLayout
			{
				Padding = new Thickness(0, 40, 0, 0),
				VerticalOptions = LayoutOptions.StartAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Orientation = StackOrientation.Vertical,
				Children = { lblTitle, txtInput, slButtons },
			};

			// create and show page
			var page = new ContentPage();
			page.Content = layout;
			navigation.PushModalAsync(page);
			// open keyboard
			txtInput.Focus();

			// code is waiting her, until result is passed with tcs.SetResult() in btn-Clicked
			// then proc returns the result
			return tcs.Task;
		}
	}
}
