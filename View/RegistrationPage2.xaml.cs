﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Diagnostics;
using ImageRecognizer;
using Newtonsoft.Json.Linq;

namespace ImageRecognizer
{
	public partial class RegistrationPage2 : ContentPage
	{

		MediaFile foto1, foto2;
		MainViewModel vm;
		int newId;

		public RegistrationPage2(int id)
		{
			vm = new MainViewModel();
			InitializeComponent();
			spinner.IsVisible = false;
			spinner.IsRunning = false;
			loadingLabel.IsVisible = false;
			buttonDone.IsVisible = true;
			newId = id;
		}

		void add1Clicked(object sender, EventArgs args)
		{
			add1.Opacity = .5;

			TakePictureButton_Clicked(1);

		}

		void add2Clicked(object sender, EventArgs args)
		{
			add2.Opacity = .5;
			TakePictureButton_Clicked(2);
		}

		async void TakePictureButton_Clicked(int buttonNumber)
		{
			await CrossMedia.Current.Initialize();

			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported) 
			{
				await DisplayAlert("No Camera", "No camera avaiable", "Ok");
				return;
			}
				

			var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
			{
				Directory = "ImageRecognizer",
				Name = DateTime.Now.ToString()
			});

			if (file == null)
			{
				return;
			}

			switch (buttonNumber)
			{
				case 1:
					add1.Opacity = 1;
					add1.Source = ImageSource.FromStream(() => file.GetStream());
					foto1 = file;
					break;
				case 2:
					add2.Opacity = 1;
					add2.Source = ImageSource.FromStream(() => file.GetStream());
					foto2 = file;
					break;
			}


		}

		async void DoneButton_OnCLicked(object sender, EventArgs e)
		{
			spinner.IsVisible = true;
			spinner.IsRunning = true;
			loadingLabel.IsVisible = true;
			buttonDone.IsVisible = false;

			/*
			Debug.WriteLine("Prima foto: " + foto1.AlbumPath);
			var url1 = await vm.UploadDoc(foto1);
			Debug.WriteLine("Seconda foto: " + foto2.AlbumPath);
			var url2 = await vm.UploadDoc(foto2);

			string newStringJson = "{" +
				"ID: " + newId + "," +
				"Url1: " + url1 + "," +
				"Url2:" + url2 + "}";

			dynamic newJson = new JObject();
			newJson.ID = newId;
			newJson.Url1 = url1;
			newJson.Url2 = url2;

			Debug.WriteLine(newJson);

			var myFlag = await vm.RegistrationRequest(newJson);

			if (myFlag)
			{
				spinner.IsVisible = false;
				spinner.IsRunning = false;
				loadingLabel.IsVisible = false;
				await DisplayAlert("Well Done!", "The registration has been done! :)", "OK");
				await Navigation.PushAsync(new LoginPage());
			}
			else
			{
				spinner.IsVisible = false;
				spinner.IsRunning = false;
				loadingLabel.IsVisible = false;
				buttonDone.IsVisible = true;
				await DisplayAlert("OPS!", "Something went wrong! :(", "OK");
			}
			*/

		}

	}
}
