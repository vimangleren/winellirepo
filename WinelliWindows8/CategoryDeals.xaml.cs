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
	public sealed partial class CategoryDeals : PenguinDeals.Common.LayoutAwarePage
	{
		public CategoryDeals()
		{
			this.InitializeComponent();
		}

		public static Dictionary<string, List<DealsInCategoryViewModel>> vm = new Dictionary<string, List<DealsInCategoryViewModel>>();

		private string CategoryIdParam;
		private string CountryIdParam;

		public async void BindDataAsync()
		{
			await LoadViewModel();
		}

		private static List<DealsInCategoryViewModel> viewModel = null;
		private async Task LoadViewModel()
		{
			if (!vm.Keys.Contains(CategoryIdParam))
			{
				var categories = await DealData.GetDealsAsync(CategoryIdParam);

				viewModel = new List<DealsInCategoryViewModel>();
				DealsInCategoryViewModel cvm;
				foreach (var item in categories)
				{
					if (!string.IsNullOrEmpty(item.DealId))
					{
						cvm = new DealsInCategoryViewModel();

						if (item.Redwine)
						{
							cvm.WineColor = "#FF660E00";
						}
						else
						{
							cvm.WineColor = "#FFDCE08C";
						}

						cvm.Title = item.Title;
						cvm.Price = string.Format("{0:C}", item.Price);
						cvm.DealId = item.DealId;

						viewModel.Add(cvm);
					}
				}

				vm.Add(CategoryIdParam, viewModel);

				var source = vm.Where(e => e.Key == CategoryIdParam).Select(e => e.Value).Single();
				itemsViewSource.Source = source;

				foreach (var item in viewModel)
				{
					var images = await DealImageData.GetDealImagesAsync(item.DealId, "250x250");
					item.ImageUrls = images.Select(x => x.ImageUrl).First();
				}
			}
			else
			{
				var source = vm.Where(e => e.Key == CategoryIdParam).Select(e => e.Value).Single();
				itemsViewSource.Source = source;
			}
		}

		public string ViewName { get; set; }

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			CategoryViewModel cvm = (CategoryViewModel)e.Parameter;
			pageTitle.Text = cvm.CategoryName + " from all over the world";
			CategoryIdParam = cvm.CategoryId;

			BindDataAsync();
		}

		private void CategoryDeals_ItemClick(object sender, ItemClickEventArgs e)
		{
			this.Frame.Navigate(typeof(DealDetails2), e.ClickedItem);
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

		private void btnTrolley_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(DealsTrolley));
		}
	}

	public class DealsInCategoryViewModel : INotifyPropertyChanged
	{
		private string _dealId = string.Empty;
		public string DealId
		{
			get
			{
				return _dealId;
			}
			set
			{
				if (_dealId != value)
				{
					_dealId = value;
					NotifyPropertyChanged("DealId");
				}
			}
		}

		private string _wineColor = string.Empty;
		public string WineColor { get { return _wineColor; } set { if (_wineColor != value) { _wineColor = value; NotifyPropertyChanged("WineColor"); } } }

		private string _title = string.Empty;
		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				if (_title != value)
				{
					_title = value;
					NotifyPropertyChanged("Title");
				}
			}
		}

		private string _price = string.Empty;
		public string Price
		{
			get
			{
				return _price;
			}
			set
			{
				if (_price != value)
				{
					_price = value;
					NotifyPropertyChanged("Price");
				}
			}
		}

		private string _imageUrls;
		public string ImageUrls
		{
			get
			{
				return _imageUrls;
			}
			set
			{
				_imageUrls = value;
				NotifyPropertyChanged("ImageUrls");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

	}
}
