using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WarmPenguin.StorageBroker;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace PenguinDeals
{
	/// <summary>
	/// A page that displays a collection of item previews.  In the Split Application this page
	/// is used to display and select one of the available groups.
	/// </summary>
	public sealed partial class CountryDeals : PenguinDeals.Common.LayoutAwarePage
	{
		public CountryDeals()
		{
			this.InitializeComponent();
		}

		private string CountryIdParam;

		public async void BindDataAsync()
		{
			await LoadViewModel();
			await LoadCategories();
		}


		public static Dictionary<string, List<CategoryViewModel>> countryCache = new Dictionary<string, List<CategoryViewModel>>();

		public DateTime RefreshedNext { get; set; }
		private static List<CategoryViewModel> viewModelList = null;

		private async Task LoadCategories()
		{
			if (!countryCache.Keys.Contains(CountryIdParam) || RefreshedNext < DateTime.Now)
			{
				RefreshedNext = DateTime.Now.AddMinutes(40);

				var categories = await CategoriesData.GetCategoriesAsync();

				viewModelList = new List<CategoryViewModel>();
				CategoryViewModel cvm;
				foreach (var item in categories)
				{
					if (!string.IsNullOrEmpty(item.LatestDealId))
					{
						cvm = new CategoryViewModel();
						cvm.CategoryName = item.Name;
						cvm.LatestDealId = item.LatestDealId;
						cvm.CategoryId = item.CategoryId;

						viewModelList.Add(cvm);
					}
				}

				countryCache.Add(CountryIdParam, viewModelList);
				var source = countryCache.Where(e => e.Key == CountryIdParam).Select(e => e.Value).Single();
				
				categoryGridView.ItemsSource = source;

				foreach (var item in source)
				{
					item.DealCountInCategory = await DealData.GetDealCount(CountryIdParam, item.CategoryId);
					var image = await DealImageData.GetDealImageAsync(item.LatestDealId, "250x250");
					item.LatestImageUrl = image.ImageUrl;
				}
			}
			else
			{
				var source = countryCache.Where(e => e.Key == CountryIdParam).Select(e => e.Value).Single();
				categoryGridView.ItemsSource = source;
			}

		}

		public static Dictionary<string, List<DealsInCategoryViewModel>> dealsCache = new Dictionary<string, List<DealsInCategoryViewModel>>();
		private static List<DealsInCategoryViewModel> viewModel = null;
		private async Task LoadViewModel()
		{
			if (!dealsCache.Keys.Contains(CountryIdParam))
			{
				var deals = await DealData.GetDealsByCountryAsync(CountryIdParam);

				viewModel = new List<DealsInCategoryViewModel>();
				DealsInCategoryViewModel cvm;
				foreach (var item in deals)
				{
					if (!string.IsNullOrEmpty(item.DealId))
					{
						cvm = new DealsInCategoryViewModel();

						if (item.Redwine)
						{
							cvm.WineColor = "#FF660E00";
						}
						else {
							cvm.WineColor = "#FFDCE08C";
						}

						cvm.Title = item.Title;
						cvm.Price = string.Format("{0:C}", item.Price);
						cvm.DealId = item.DealId;

						viewModel.Add(cvm);
					}
				}

				dealsCache.Add(CountryIdParam, viewModel);

				var source = dealsCache.Where(e => e.Key == CountryIdParam).Select(e => e.Value).Single();
				itemsViewSource.Source = source;

				foreach (var item in viewModel)
				{
					var images = await DealImageData.GetDealImagesAsync(item.DealId, "250x250");
					item.ImageUrls = images.Select(x => x.ImageUrl).First();
				}
			}
			else
			{
				var source = dealsCache.Where(e => e.Key == CountryIdParam).Select(e => e.Value).Single();
				itemsViewSource.Source = source;
			}
		}

		public string ViewName { get; set; }

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			Country c = (Country)e.Parameter;
			pageTitle.Text = "Wine from " + c.CountryName;
			CountryIdParam = c.CountryId;

			BindDataAsync();
		}

		private void CategoryDeals_ItemClick(object sender, ItemClickEventArgs e)
		{
			this.Frame.Navigate(typeof(DealDetails2), e.ClickedItem);
		}

		private void btnTrolley_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(DealsTrolley));
		}

		private void categoryGridView_ItemClick_1(object sender, ItemClickEventArgs e)
		{
			var obj = new {CategoryId = ((CategoryViewModel)e.ClickedItem).CategoryId, CountryIdParam};
			this.Frame.Navigate(typeof(Category), obj);
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
	}
}
