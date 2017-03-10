using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ImageRecognizer
{
	public partial class ListPage_v1 : ContentPage
	{
		public ListPage_v1(List<JsonItem> myJsonList)
		{
			InitializeComponent();

			this.Title = "Lista";

			//Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);

			//var listView = new ListView();


			//myListView.ItemsSource = ;

			myListView.ItemsSource =
				from c in myJsonList
				select c.title;

			myListView.ItemSelected += (sender, e) =>
			{
				if (e.SelectedItem != null)
					// Vedo l'elemento che ho selezionato
					Debug.WriteLine("Selected: " + e.SelectedItem);

				// Tolgo la select dall'elemento
				myListView.SelectedItem = null;
			};


			Content = myLayout;
		}
	}
}
