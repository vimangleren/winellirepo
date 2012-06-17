using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WarmPenguin.StorageBroker;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace PenguinDeals
{
	/// <summary>
	/// A basic page that provides characteristics common to most applications.
	/// </summary>
	public sealed partial class OrderThanks : PenguinDeals.Common.LayoutAwarePage
	{
		Storyboard sb = new Storyboard();

		public OrderThanks()
		{
			this.InitializeComponent();

			sb.Duration = new Duration(new TimeSpan(0, 0, 10));
			sb.Begin();
			sb.Completed += sb_Completed;
		}

		bool orderHandled = false;

		private async void sb_Completed(object sender, object e)
		{
			sb.Stop();

			App._DealBasket.EmptyBasket();
			this.Frame.Navigate(typeof(FrontPage));
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			pageTitle.Text = "Thank you for your order";
		}
	}
}