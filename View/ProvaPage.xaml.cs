using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ImageRecognizer
{
	public partial class ProvaPage : ContentPage
	{
		public ProvaPage()
		{
			InitializeComponent();
			this.Title = "Report";
			progressBar();


		}

		async void progressBar() 
		{ 
			await Task.Delay(2000);
			await barHappy.ProgressTo(.8, 1000, Easing.Linear);
		}

		public async void loadProgress(object o, EventArgs e)
		{



		}



	}
}
