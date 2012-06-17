using System;
using System.Collections.Generic;
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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace PenguinDeals
{
	/// <summary>
	/// A basic page that provides characteristics common to most applications.
	/// </summary>
	public sealed partial class FrontPage : PenguinDeals.Common.LayoutAwarePage
	{
		private List<Country> smallFlags = null;
		private List<Country> largeFlags = null;

		public FrontPage()
		{
			this.InitializeComponent();
		}


		private async Task SetLargeFlags()
		{
			var ctrs = await CountryData.GetCountriesAsync();
			smallFlags = ctrs.ToList();

			largeFlags = new List<Country>();

			var firstLargeFlag = smallFlags.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
			var secondLargeFlag = smallFlags.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
			var thirdLargeFlag = smallFlags.OrderBy(x => Guid.NewGuid()).FirstOrDefault();

			smallFlags.Remove(firstLargeFlag);
			smallFlags.Remove(secondLargeFlag);
			smallFlags.Remove(thirdLargeFlag);

			largeFlags.Clear();
			largeFlags.Add(firstLargeFlag);
			largeFlags.Add(secondLargeFlag);
			largeFlags.Add(thirdLargeFlag);
		}

		private void BindFlags()
		{
			largeFlagsListView.ItemsSource = largeFlags;
			smallFlagsGrid.ItemsSource = smallFlags;
		}

	
		public DateTime RefreshedNext { get; set; }
		private static List<CategoryViewModel> viewModelList = new List<CategoryViewModel>();

		private async Task LoadCategories()
		{
			if (viewModelList == null || RefreshedNext < DateTime.Now)
			{
				RefreshedNext = DateTime.Now.AddMinutes(40);

				var categories = await CategoriesData.GetCategoriesAsync();

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

				categoryGridView.ItemsSource = viewModelList;

				foreach (var item in viewModelList)
				{
					item.DealCountInCategory = await DealData.GetDealCount(item.CategoryId);
					var image = await DealImageData.GetDealImageAsync(item.LatestDealId, "250x250");
					item.LatestImageUrl = image.ImageUrl;
				}
			}
		}


		public DateTime RedWineRefreshedNext { get; set; }
		private static List<DealsInCategoryViewModel> redwineModelList = new List<DealsInCategoryViewModel>();

		private async Task LoadRedWines()
		{
			if (redwineModelList.Count == 0 || RedWineRefreshedNext < DateTime.Now)
			{
				RedWineRefreshedNext = DateTime.Now.AddMinutes(40);

				var categories = await DealData.GetDealByWineTypeAsync(true);

				redwineModelList = new List<DealsInCategoryViewModel>();
				DealsInCategoryViewModel cvm;
				foreach (var item in categories)
				{
					if (!string.IsNullOrEmpty(item.DealId))
					{
						cvm = new DealsInCategoryViewModel();

						cvm.Title = item.Title;
						cvm.Price = string.Format("{0:C}", item.Price);
						cvm.DealId = item.DealId;
						cvm.WineColor = "#FF660E00";

						redwineModelList.Add(cvm);
					}
				}

				RedWineDealsGridView.ItemsSource = redwineModelList;

				foreach (var item in redwineModelList)
				{
					var images = await DealImageData.GetDealImagesAsync(item.DealId, "250x250");
					item.ImageUrls = images.Select(x => x.ImageUrl).First();
				}
			}
		}


		public DateTime WhiteWineRefreshedNext { get; set; }
		private static List<DealsInCategoryViewModel> whitewineModelList = new List<DealsInCategoryViewModel>();

		private async Task LoadWhiteWines()
		{
			if (whitewineModelList.Count == 0 || WhiteWineRefreshedNext < DateTime.Now)
			{
				WhiteWineRefreshedNext = DateTime.Now.AddMinutes(40);

				var categories = await DealData.GetDealByWineTypeAsync(false);

				whitewineModelList = new List<DealsInCategoryViewModel>();
				DealsInCategoryViewModel cvm;
				foreach (var item in categories)
				{
					if (!string.IsNullOrEmpty(item.DealId))
					{
						cvm = new DealsInCategoryViewModel();

						cvm.Title = item.Title;
						cvm.Price = string.Format("{0:C}", item.Price);
						cvm.DealId = item.DealId;
						cvm.WineColor = "#FFDCE08C";

						whitewineModelList.Add(cvm);
					}
				}

				WhiteWineDealsGridView.ItemsSource = whitewineModelList;

				foreach (var item in whitewineModelList)
				{
					var images = await DealImageData.GetDealImagesAsync(item.DealId, "250x250");
					item.ImageUrls = images.Select(x => x.ImageUrl).First();
				}
			}
		}


		private DateTime RefreshPage;
		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			if (RefreshPage < DateTime.Now)
			{
				await SetLargeFlags();
				BindFlags();

				await LoadCategories();

				await LoadRedWines();
				await LoadWhiteWines();

				RefreshPage = DateTime.Now.AddMinutes(45);
			}
		}

		private void categoryGridView_ItemClick_1(object sender, ItemClickEventArgs e)
		{
			this.Frame.Navigate(typeof(CategoryDeals), e.ClickedItem);
		}

		private void RedWineDealsGridView_ItemClick_1(object sender, ItemClickEventArgs e)
		{
			this.Frame.Navigate(typeof(DealDetails2), e.ClickedItem);
		}

		private void WhiteWineDealsGridView_ItemClick_1(object sender, ItemClickEventArgs e)
		{
			this.Frame.Navigate(typeof(DealDetails2), e.ClickedItem);
		}

		private void smallFlagsGrid_ItemClick_1(object sender, ItemClickEventArgs e)
		{
			this.Frame.Navigate(typeof(CountryDeals), e.ClickedItem);
		}

		private void largeFlagsListView_ItemClick_1(object sender, ItemClickEventArgs e)
		{
			this.Frame.Navigate(typeof(CountryDeals), e.ClickedItem);
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
