using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Media;
using ImageRecognizer;

[assembly: Dependency(typeof(TakeAPicture.iOS.PictureTaker_iOS))]

namespace TakeAPicture.iOS
{
	public class PictureTaker_iOS : IPictureTaker
	{
		public async void SnapPic()
		{
			var picker = new MediaPicker();

			var mediaFile = await picker.PickPhotoAsync();
			System.Diagnostics.Debug.WriteLine(mediaFile.Path);
		}
	}
}
