using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WarmPenguin.StorageBroker;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace PenguinDeals
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class Category : PenguinDeals.Common.LayoutAwarePage
    {
		public Category()
        {
            this.InitializeComponent();
        }

        public async void BindDataAsync()
        {
			var category = await CategoriesData.GetCategoryAsync(CategoryIdParam);
			var country = await CountryData.GetCountryAsync(CountryIdParam);

			pageTitle.Text = category.Name + " from " + country.CountryName;

            await LoadViewModel();
        }

		private string CategoryIdParam;
		private string CountryIdParam;

		public static Dictionary<string, List<DealsInCategoryViewModel>> vm = new Dictionary<string, List<DealsInCategoryViewModel>>();
		private static List<DealsInCategoryViewModel> viewModel = null;
		private async Task LoadViewModel()
		{
			if (!vm.Keys.Contains(CategoryIdParam + CountryIdParam))
			{
				var deals = await DealData.GetDealsAsync(CountryIdParam, CategoryIdParam);

				viewModel = new List<DealsInCategoryViewModel>();
				DealsInCategoryViewModel cvm;
				foreach (var item in deals)
				{
					if (!string.IsNullOrEmpty(item.DealId))
					{
						cvm = new DealsInCategoryViewModel();

						cvm.Title = item.Title;
						cvm.Price = string.Format("{0:C}", item.Price);
						cvm.DealId = item.DealId;

						viewModel.Add(cvm);
					}
				}

				vm.Add(CategoryIdParam+CountryIdParam, viewModel);

				var source = vm.Where(e => e.Key == CategoryIdParam + CountryIdParam).Select(e => e.Value).Single();
				itemsViewSource.Source = source;

				foreach (var item in viewModel)
				{
					var images = await DealImageData.GetDealImagesAsync(item.DealId, "250x250");
					item.ImageUrls = images.Select(x => x.ImageUrl).First();
				}
			}
			else
			{
				var source = vm.Where(e => e.Key == CategoryIdParam + CountryIdParam).Select(e => e.Value).Single();
				itemsViewSource.Source = source;
			}
		}

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
			dynamic obj = e.Parameter;

			CategoryIdParam = obj.CategoryId;
			CountryIdParam = obj.CountryIdParam;

			BindDataAsync();
        }

		private void dealDetails_PointerEntered(object sender, PointerEventArgs e)
		{
			var stackpanel = (Grid)sender;
			stackpanel.SetValue(Grid.BackgroundProperty, Resources["mouseOver"]);
		}

		private void dealDetails_PointerExited2(object sender, PointerEventArgs e)
		{
			var stackpanel = (Grid)sender;
			stackpanel.SetValue(Grid.BackgroundProperty, Application.Current.Resources["ListViewItemOverlayBackgroundBrush"]);
		}

		private void dealDetails_PointerExited(object sender, Windows.UI.Xaml.Input.PointerEventArgs e)
		{
			var stackpanel = (Grid)sender;

			stackpanel.SetValue(Grid.BackgroundProperty, Resources["flagBackground"]);
		}

		private void categoryGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(DealDetails2), e.ClickedItem);
        }

		private void btnTrolley_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DealsTrolley));
        }
    }
}
