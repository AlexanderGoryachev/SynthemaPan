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
    public partial class ReleasesPage : PhoneApplicationPage
    {
        public ReleasesPage()
        {
            InitializeComponent();

            DownloadingService.webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(MainStringDownloadCompleted);

            MainListBox.ItemsSource = AppData.MainItems;
            ShortMainListBox.ItemsSource = AppData.MainItems;

            if (!AppData.IsInternetAccess)
                MessageBox.Show("Подключение к Интернету отсутствует. Для работы приложения необходим доступ к сети");

            if (AppData.IsMainDownloaded)
                LoadingBar.IsIndeterminate = false;
            else
                LoadingBar.IsIndeterminate = true;
        }

        public void MainStringDownloadCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
                return;

            AppData.MainString = e.Result;

            // Функции парсинга выполняются 5,5 секунд. Необходимо оптимизировать.
            ParsingService.ParseReleasesHtml(e.Result);
            ParsingService.ParseNewsHtml(e.Result);

            AppData.IsMainDownloaded = true;
            LoadingBar.IsIndeterminate = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (MainListBox.Visibility == Visibility.Visible)
                ApplicationBar = (Microsoft.Phone.Shell.ApplicationBar)Resources["DetailedMainAppBar"];
            else
                ApplicationBar = (Microsoft.Phone.Shell.ApplicationBar)Resources["ShortMainAppBar"];

            if (!AppData.IsMainDownloaded)
                DownloadingService.DownloadMainAndNews(Constants.BaseUrl);
            else
                LoadingBar.IsIndeterminate = false;

            base.OnNavigatedTo(e);
        }

        private void MainImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string imgUrl = AppData.MainItems.ElementAt(MainListBox.SelectedIndex).ImgUrl;
            if (imgUrl != "")
            {
                NavigationService.Navigate(new Uri("/ImagePage.xaml?imgUrl=" + imgUrl, UriKind.Relative));
            }
        }

        private void MainStackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (MainListBox.SelectedItem != null)
            {
                //WebBrowserTask task = new WebBrowserTask();
                //task.Uri = new Uri(AppData.MainItems.ElementAt(MainListBox.SelectedIndex).Link, UriKind.RelativeOrAbsolute);
                //task.Show();
                var mainDetailPath = AppData.MainItems.ElementAt(MainListBox.SelectedIndex).Link;
                NavigationService.Navigate(new Uri("/ReleasesDetail.xaml?mainDetailPath=" + mainDetailPath, UriKind.Relative));
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
            DownloadingService.DownloadMainAndNews(Constants.BaseUrl);
        }

        private void SetShortListAppButton_Click(object sender, EventArgs e)
        {
            ApplicationBar = (Microsoft.Phone.Shell.ApplicationBar)Resources["ShortMainAppBar"];
            MainListBox.Visibility = Visibility.Collapsed;
            ShortMainListBox.Visibility = Visibility.Visible;
        }

        private void SetDetailedListAppButton_Click(object sender, EventArgs e)
        {
            ApplicationBar = (Microsoft.Phone.Shell.ApplicationBar)Resources["DetailedMainAppBar"];
            MainListBox.Visibility = Visibility.Visible;
            ShortMainListBox.Visibility = Visibility.Collapsed;
        }

    }
}