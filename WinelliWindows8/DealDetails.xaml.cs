using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using WarmPenguin.StorageBroker;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Group Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234229

namespace PenguinDeals
{
	/// <summary>
	/// A page that displays an overview of a single group, including a preview of the items
	/// within the group.
	/// </summary>
	public sealed partial class DealDetails2 : PenguinDeals.Common.LayoutAwarePage
	{
		public DealDetails2()
		{
			this.InitializeComponent();
		}

		private Deal thisDeal;

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			var dealPar = (DealsInCategoryViewModel)e.Parameter;

			pageTitle.Text = dealPar.Title;

			thisDeal = await DealData.GetDealAsync(dealPar.DealId);
			var dealImages = await DealImageData.GetDealImagesAsync(thisDeal.DealId, "250x250");

			imageGrid.ItemsSource = dealImages;

			title.Text = thisDeal.Title;
			description.Text = thisDeal.Description;
			tblPrice.Text = string.Format("{0:C}", thisDeal.Price);

			tbBeginDeal.Text = string.Format("{0:d MMM}", thisDeal.BeginDeal) + " - " + string.Format("{0:d MMM}", thisDeal.EndDeal);
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(DealsTrolley));
		}

		private async void Image_PointerPressed_1(object sender, Windows.UI.Xaml.Input.PointerEventArgs e)
		{
			await App._DealBasket.AddToBasket(thisDeal.DealId);

			MessageDialog md = new MessageDialog("You have added a product to your trolley, cool eh. The total now comes to " + string.Format("{0:C}", App._DealBasket.BasketTotalPrice));
			md.Commands.Add(new UICommand("ok", 
				(command) => { 
					appbar.IsOpen = true;
				}, 
				"1"));

			await md.ShowAsync();
		}

		private void imgPrice_PointerEntered_1(object sender, PointerEventArgs e)
		{
			Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1);
		}

		private void imgPrice_PointerExited_1(object sender, PointerEventArgs e)
		{
			Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
		}
	}
}
