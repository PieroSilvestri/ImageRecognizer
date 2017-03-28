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
		List<JObject> listItems = new List<JObject>();

		public ListReportPage(int id)
		{
			InitializeComponent();
			vm = new MainViewModel();
			ListViewReports.ItemsSource = listNames;
			this.Title = "List Report";
			GetListReports(id);

			ToolbarItems.Add(new ToolbarItem("Add list", null, async () =>
			{
				var page = new ContentPage();
				//var result = await page.DisplayAlert("Title", "Message", "Accept", "Cancel");
				string entryString = await InputBox(this.Navigation);
				Debug.WriteLine("success: {0}", entryString);
				if (entryString != null)
				{
					JObject newList = await vm.CreateNewList(entryString, id);
					if ((bool)newList["success"])
					{
						listNames.Add(new ListItem { ListItemName = entryString });
					}
					else
					{
						await DisplayAlert("Error", "New List not inserted.", "OK");
					}
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

			int idSelected = GetIdByString(itemSelected.ListItemName);

			if (idSelected > 0)
			{
				DisplayAlert("Ottimo", "Hai premuto " + itemSelected.ListItemName + " con l'id: " + idSelected, "OK");
			}
			else
			{
				DisplayAlert("Error", "Valore non trovato.", "OK");

			}
		}

		private int GetIdByString(string text)
		{
			foreach (JObject item in listItems)
			{
				if (item["Name"].ToString() == text)
				{
					return (int) item["ID_List"];
				}
			}
			return -1;
		}

		private async void GetListReports(int user_id)
		{
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
				listNames.Add(new ListItem { ListItemName = (string)item["Name"] });
			}

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
