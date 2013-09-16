using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Synthema.Common;

namespace Synthema
{
    public partial class NewsPage : PhoneApplicationPage
    {
        public NewsPage()
        {
            InitializeComponent();

            DownloadingService.webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadStringCompleted);

            NewsListBox.ItemsSource = AppData.NewsItems;
            ShortNewsListBox.ItemsSource = AppData.ShortNewsItems;

            if (!AppData.IsInternetAccess)
                MessageBox.Show("Подключение к Интернету отсутствует. Для работы приложения необходим доступ к сети");
        }

        public void DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
                return;

            if (AppData.IsShortNewsListOn)
            {
                AppData.ShortNewsString = e.Result;
                ParsingService.ParseShortNewsHtml(e.Result);
                AppData.IsShortNewsDownloaded = true; 
            }
            else
            {
                AppData.NewsString = e.Result;
                ParsingService.ParseNewsHtml(e.Result);
                AppData.IsNewsDownloaded = true;
            }

            LoadingBar.IsIndeterminate = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (AppData.IsShortNewsListOn)
            {
                SetShortList();
            }
            else
            {
                SetDetailedList();
            }

            base.OnNavigatedTo(e);
        }
        
        private void NewsImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string imgUrl = AppData.NewsItems.ElementAt(NewsListBox.SelectedIndex).ImgUrl;
            if (imgUrl != "")
            {
                NavigationService.Navigate(new Uri("/ImagePage.xaml?imgUrl=" + imgUrl, UriKind.Relative));
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            base.OnBackKeyPress(e);
        }

        private void RefreshAppButton_Click(object sender, EventArgs e)
        {
            LoadingBar.IsIndeterminate = true;
            if (AppData.IsShortNewsListOn)
                DownloadingService.DownloadString(Constants.NewsUrl);
            else
                DownloadingService.DownloadString(Constants.BaseUrl);
        }

        private void SetShortListAppButton_Click(object sender, EventArgs e)
        {
            SetShortList();

            AppData.IsShortNewsListOn = true;
        }

        private void SetDetailedListAppButton_Click(object sender, EventArgs e)
        {
            SetDetailedList();
            AppData.IsShortNewsListOn = false;
        }

        private void SetShortList()
        {
            ApplicationBar = (Microsoft.Phone.Shell.ApplicationBar)Resources["ShortMainAppBar"];
            NewsListBox.Visibility = Visibility.Collapsed;
            ShortNewsListBox.Visibility = Visibility.Visible;

            if (!AppData.IsShortNewsDownloaded)
            {
                LoadingBar.IsIndeterminate = true;
                DownloadingService.DownloadString(Constants.NewsUrl);
            }
        }

        private void SetDetailedList()
        {
            ApplicationBar = (Microsoft.Phone.Shell.ApplicationBar)Resources["DetailedMainAppBar"];
            NewsListBox.Visibility = Visibility.Visible;
            ShortNewsListBox.Visibility = Visibility.Collapsed;

            if (!AppData.IsNewsDownloaded)
            {
                LoadingBar.IsIndeterminate = true;
                DownloadingService.DownloadString(Constants.BaseUrl);
            }
        }
    }
}