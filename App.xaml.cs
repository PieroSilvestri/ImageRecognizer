using Xamarin.Forms;

namespace ImageRecognizer
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			var starterPage = new NavigationPage(new ProvaPage());

			MainPage = starterPage;
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
