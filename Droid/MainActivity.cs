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
using Plugin.Permissions;
using Plugin.Media;

[assembly: Dependency(typeof(ImageRecognizer.Droid.MainActivity))]

namespace ImageRecognizer.Droid
{
	[Activity(Label = "ImageRecognizer.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : FormsAppCompatActivity //IPictureTaker
	{
		protected override async void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			Forms.Init(this, bundle);

			await CrossMedia.Current.Initialize();

			LoadApplication(new App());
		}

		/*public void SnapPic()
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

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			if (resultCode == Result.Canceled)
				return;

		}*/

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

	}
}

