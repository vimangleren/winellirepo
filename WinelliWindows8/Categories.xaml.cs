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
    public sealed partial class Categories : PenguinDeals.Common.LayoutAwarePage
    {
        public Categories()
        {
            this.InitializeComponent();
        }

        public async void BindDataAsync()
        {
            await LoadViewModel();
        }

		public DateTime RefreshedNext { get; set; }
		private static List<CategoryViewModel> viewModelList = new List<CategoryViewModel>();

        private async Task LoadViewModel()
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

                itemsViewSource.Source = viewModelList;

                foreach (var item in viewModelList)
                {
                    item.DealCountInCategory = await DealData.GetDealCount(item.CategoryId);
                    var image = await DealImageData.GetDealImageAsync(item.LatestDealId, "250x250");
                    item.LatestImageUrl = image.ImageUrl;
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            BindDataAsync();
        }

		private void dealDetails_PointerEntered(object sender, PointerEventArgs e)
        {
            var stackpanel = (StackPanel)sender;
            stackpanel.SetValue(StackPanel.BackgroundProperty, Resources["MousePointerBackground"]);
        }

		private void dealDetails_PointerExited(object sender, Windows.UI.Xaml.Input.PointerEventArgs e)
        {
            var stackpanel = (StackPanel)sender;
            stackpanel.SetValue(StackPanel.BackgroundProperty, Application.Current.Resources["ListViewItemOverlayBackgroundBrush"]);
        }

		private void categoryGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(CategoryDeals), e.ClickedItem);
        }

		private void btnTrolley_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DealsTrolley));
        }
    }


    public class CategoryViewModel : INotifyPropertyChanged
    {
        private string _categoryId = string.Empty;
        public string CategoryId { get { return _categoryId; } set { _categoryId = value; NotifyPropertyChanged("CategoryId"); } }

        private int _dealCountInCategory = 0;
        public int DealCountInCategory { get { return _dealCountInCategory; } set { _dealCountInCategory = value; NotifyPropertyChanged("DealCountInCategory"); } }

        private string _latestDealId = string.Empty;
        public string LatestDealId { get { return _latestDealId; } set { _latestDealId = value; NotifyPropertyChanged("LatestDealId"); } }

        private string _latestImageUrl = string.Empty;
        public string LatestImageUrl { get { return _latestImageUrl; } set { _latestImageUrl = value; NotifyPropertyChanged("LatestImageUrl"); } }

        private string _categoryName;
        public string CategoryName { get { return _categoryName; } set { _categoryName = value; NotifyPropertyChanged("CategoryName"); } }

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
