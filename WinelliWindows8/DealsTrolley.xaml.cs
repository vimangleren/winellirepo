using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace PenguinDeals
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class DealsTrolley : PenguinDeals.Common.LayoutAwarePage
    {
        public DealsTrolley()
        {
            this.InitializeComponent();

        }

        private async void PopulateViewModel()
        {
            List<DealsTrolleyViewModel> viewModelList = new List<DealsTrolleyViewModel>();
            foreach (var item in App._DealBasket.GetDealsInBasket())
            {
                var viewModel = new DealsTrolleyViewModel();
                viewModel._Deal = await DealData.GetDealAsync(item.DealId);
                var image = await DealImageData.GetDealImageAsync(item.DealId, "250x250");
                viewModel.ImageUrl = image.ImageUrl;
                viewModel.TotalPrice = String.Format("{0:C}", App._DealBasket.BasketTotalPrice);

                viewModelList.Add(viewModel);
            }

            tbPrice.Text = string.Format("{0:C}", App._DealBasket.BasketTotalPrice);
            itemsViewSource.Source = viewModelList;
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            pageTitle.Text = "Your Winelli trolley";

            PopulateViewModel();
        }

        private async void Rectangle_PointerPressed_1(object sender, PointerEventArgs e)
        {
			if (!App.IsUserLoggedIn)
			{
				MessageDialog md = new MessageDialog("You need to be logged in to checkout.");
				await md.ShowAsync();

				return;
			}
			else {	
				MessageDialog md = new MessageDialog("Are you sure you want to place this order ?");
				var cancel = new UICommand("Cancel", (cmd) => {
					return;
				});
				var placeOrder = new UICommand("Place order", (cmd) =>
				{
					Order order = new Order();
					order.OrderRowKey = Guid.NewGuid().ToString();
					order.UserRowKey = "something";

					OrdersData.InitOrder(order.OrderRowKey, order.UserRowKey);

					this.Frame.Navigate(typeof(OrderThanks), order);
				});
				
				md.Commands.Add(cancel);
				md.Commands.Add(placeOrder);

				await md.ShowAsync();
			}
        }

        private void Rectangle_PointerEntered_1(object sender, PointerEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1);
        }

        private void Rectangle_PointerExited_1(object sender, PointerEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
        }
    }

    public class DealsTrolleyViewModel 
    {
        public Deal _Deal { get; set; }
        public string ImageUrl { get; set; }
        public string TotalPrice { get; set; }
    }

    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter == null)
            {
                return value;
            }

            return String.Format((String)parameter, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
