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

			var refresh = new ToolbarItem
			{
				Icon = "refresh.png",
				Name = "refresh",
				Priority = 0,
				Order = ToolbarItemOrder.Primary
			};
			var add = new ToolbarItem
			{
				Name = "add",
				Priority = 1,
				Order = ToolbarItemOrder.Secondary
			};
			var add2 = new ToolbarItem
			{
				Name = "add",
				Priority = 1,
				Order = ToolbarItemOrder.Secondary
			};

			ToolbarItems.Add(refresh);
			ToolbarItems.Add(add);
			ToolbarItems.Add(add2);


		}


		public void test(object o, EventArgs e)
		{
		}


	}
}
