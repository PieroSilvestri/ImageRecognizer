using System;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Forms;
using Xamarin.Media;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(ImageRecognizer.Droid.MainActivity))]

namespace ImageRecognizer.Droid
{
	[Activity(Label = "ImageRecognizer.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : FormsAppCompatActivity, IPictureTaker
	{
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			Forms.Init(this, bundle);

			LoadApplication(new App());
		}

		public void SnapPic()
		{
			var activity = Forms.Context as Activity;
			var picker = new MediaPicker(activity);
			var intent = picker.GetTakePhotoUI(new StoreCameraMediaOptions
			{
				Name = DateTime.Now.ToString(),
				Directory = "ImageRecognizer"
			});
			activity.StartActivityForResult(intent, 1);
		}

		protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			if (resultCode == Result.Canceled)
				return;

			var mediaFile = await data.GetMediaFileExtraAsync(Forms.Context);
			System.Diagnostics.Debug.WriteLine(mediaFile.Path);
		}
	}
}

