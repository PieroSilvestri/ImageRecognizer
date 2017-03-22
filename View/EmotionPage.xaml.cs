using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace ImageRecognizer
{
	public partial class EmotionPage : ContentPage
	{

		private bool i = true;

		public EmotionPage()
		{
			InitializeComponent();
			this.Title = "Emotion Page";
		}


		public void TakeAPhoto(object o, EventArgs e)
		{
			
		}

		public void ShowReports(object o, EventArgs e)
		{

		}


		public void EasterEgg(object sender, EventArgs e)
		{

			if (i)
			{
				LogoImage.IsVisible = false;
				LogoImage2.IsVisible = true;
			}
			else
			{
				LogoImage2.IsVisible = false;
				LogoImage.IsVisible = true;
			}

			i = !i;

		}
	}
}
