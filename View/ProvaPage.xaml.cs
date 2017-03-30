using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace ImageRecognizer
{
	public partial class ProvaPage : ContentPage
	{
		private MainViewModel vm;
		List<string> listNames = new List<string>();
		List<JObject> listItems = new List<JObject>();
		private int usrid;


		public ProvaPage(int id)
		{
			InitializeComponent();

			this.Title = "Report";
			vm = new MainViewModel();
			GetListReports(id);


			ToolbarItems.Add(new ToolbarItem("Add to list", null, async () =>
			{
				var page = new ContentPage();
				//var result = await page.DisplayAlert("Title", "Message", "Accept", "Cancel");
				int idLista = await InputBox(this.Navigation, listNames);
				Debug.WriteLine("success: {0}", idLista);
				if (idLista > 0)
				{
					
				}
				else
				{
					await DisplayAlert("Error", "New List not inserted.", "OK");
				}

			}));

		}

		public void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
			{
				return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
			}
			ListItem itemSelected = (ListItem)e.SelectedItem;
			Debug.WriteLine(itemSelected.ListItemName);

			JObject jItemSelected = GetIdByString(itemSelected.ListItemName);

			if (itemSelected != null)
			{
				DisplayAlert("Ottimo", "Hai premuto " + itemSelected.ListItemName + " con l'id: " + jItemSelected, "OK");
				//Navigation.PushAsync(new ReportPage(jItemSelected));
			}
			else
			{
				DisplayAlert("Error", "Valore non trovato.", "OK");

			}
		}



		private async void GetListReports(int user_id)
		{
			Debug.WriteLine(user_id);
			JObject prova = await vm.GetListReport(user_id);
			JArray listArray = (JArray)prova["Lists"];
			if (listArray.Count > 0)
			{
				Debug.WriteLine("Maggiore");
			}
			else
			{
				Debug.WriteLine("Uguale: " + listArray.Count);
			}

			foreach (JObject item in listArray)
			{
				listItems.Add(item);
				listNames.Add((string)item["Name"]);
			}

		}

		private JObject GetIdByString(string text)
		{
			foreach (JObject item in listItems)
			{
				if (item["Name"].ToString() == text)
				{
					return item;
				}
			}
			return null;
		}

		public static Task<int> InputBox(INavigation navigation, List<string>  listNames)
		{
			// wait in this proc, until user did his input 

			var tcs = new TaskCompletionSource<int>();

			var lblTitle = new Label { Text = "Add to list", HorizontalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };

			var newList = new ListView();
			newList.ItemsSource = listNames;

			//newList.ItemSelected

		/*	var btnOk = new Button
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
			};*/


			var slButtons = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Children = {newList}
			};

			var layout = new StackLayout
			{
				Padding = new Thickness(0, 40, 0, 0),
				VerticalOptions = LayoutOptions.StartAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Orientation = StackOrientation.Vertical,
				Children = { lblTitle, slButtons },
			};

			// create and show page
			var page = new ContentPage();
			page.Content = layout;
			navigation.PushModalAsync(page);
			// code is waiting her, until result is passed with tcs.SetResult() in btn-Clicked
			// then proc returns the result
			return tcs.Task;
		}

		public void test(object o, EventArgs e)
		{
		}


	}
}
