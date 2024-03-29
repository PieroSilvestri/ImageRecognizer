﻿using System;
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

		public ListReportPage(bool flag, int id)
		{
			InitializeComponent();
			vm = new MainViewModel();
			ListViewReports.ItemsSource = listNames;
			this.Title = "List Report";
			GetListReports(flag, id);

			ToolbarItems.Add(new ToolbarItem("Add list", null, async () =>
			{
				var page = new ContentPage();
				//var result = await page.DisplayAlert("Title", "Message", "Accept", "Cancel");
				string entryString = await InputBox(this.Navigation);
				Debug.WriteLine("success: {0}", entryString);
				if (entryString != null)
				{
					JObject tempList = await vm.CreateNewList(flag, entryString, id);
					JObject newList;
					if (flag)
					{
						newList = new JObject(
							new JProperty("User_id", (int)tempList["value"]),
							new JProperty("Name", entryString),
							new JProperty("DataCreation", tempList["DataCreation"])
						);
					}
					else
					{
						JObject tempJ = (JObject)tempList["body"];
						newList = new JObject(
							new JProperty("User_id", (int)tempJ["insertId"]),
							new JProperty("Name", entryString));
					}

						
					if ((bool)tempList["success"])
					{
						listItems.Add(newList);
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

			JObject jItemSelected = GetIdByString(itemSelected.ListItemName);

			if (itemSelected != null)
			{
				//DisplayAlert("Ottimo", "Hai premuto " + itemSelected.ListItemName + " con l'id: " + idSelected, "OK");
				Navigation.PushAsync(new ReportPage(true, jItemSelected));
			}
			else
			{
				DisplayAlert("Error", "Valore non trovato.", "OK");

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

		private async void GetListReports(bool flag, int user_id)
		{

			JObject prova = await vm.GetListReport(flag, user_id);
			JArray listArray = new JArray();
			if (flag)
			{
				listArray = (JArray)prova["Lists"];

			}
			else
			{
				if ((bool)prova["success"])
				{
					listArray = (JArray)prova["body"];
				}
			}

			if (listArray.Count > 0)
			{
				foreach (JObject item in listArray)
				{
					listItems.Add(item);
					listNames.Add(new ListItem { ListItemName = (string)item["Name"] });
				}
			}
			else
			{
				await DisplayAlert("Error", "No list found", "OK");
				await Navigation.PopAsync();
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
