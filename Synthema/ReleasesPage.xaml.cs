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

            DownloadingService.webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadStringCompleted);

            MainListBox.ItemsSource = AppData.MainItems;
            ShortMainListBox.ItemsSource = AppData.MainItems;

            if (!AppData.IsInternetAccess)
                MessageBox.Show("Подключение к Интернету отсутствует. Для работы приложения необходим доступ к сети");
        }

        public void DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
                return;

            AppData.MainString = e.Result;

            // Функции парсинга выполняются 5,5 секунд. Необходимо оптимизировать. UPD. Удалил парсинг News
            ParsingService.ParseReleasesHtml(e.Result);

            AppData.IsMainDownloaded = true;
            LoadingBar.IsIndeterminate = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (AppData.IsShortReleasesListOn) 
                SetShortList();
            else 
                SetDetailedList();

            if (AppData.IsMainDownloaded)
                LoadingBar.IsIndeterminate = false;
            else
            {
                LoadingBar.IsIndeterminate = true;
                DownloadingService.DownloadString(Constants.BaseUrl);
            }

            base.OnNavigatedTo(e);
        }

        private void MainImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            int index;
            if (AppData.IsShortReleasesListOn) index = ShortMainListBox.SelectedIndex;
            else index = MainListBox.SelectedIndex;

            string imgUrl = AppData.MainItems.ElementAt(index).ImgUrl;
            NavigationService.Navigate(new Uri("/ImagePage.xaml?imgUrl=" + imgUrl, UriKind.Relative));
        }

        private void MainStackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            int index;
            if (AppData.IsShortReleasesListOn) index = ShortMainListBox.SelectedIndex;
            else index = MainListBox.SelectedIndex;

            var releasesDetailPath = AppData.MainItems.ElementAt(index).Link;
            NavigationService.Navigate(new Uri("/ReleasesDetail.xaml?releasesDetailPath=" + releasesDetailPath, UriKind.Relative));
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            base.OnBackKeyPress(e);
        }

        private void RefreshAppButton_Click(object sender, EventArgs e)
        {
            LoadingBar.IsIndeterminate = true;
            DownloadingService.DownloadString(Constants.BaseUrl);
        }

        private void SetShortListAppButton_Click(object sender, EventArgs e)
        {
            SetShortList();
            AppData.IsShortReleasesListOn = true;
        }

        private void SetDetailedListAppButton_Click(object sender, EventArgs e)
        {
            SetDetailedList();
            AppData.IsShortReleasesListOn = false;
        }

        private void SetShortList()
        {
            ApplicationBar = (Microsoft.Phone.Shell.ApplicationBar)Resources["ShortMainAppBar"];
            MainListBox.Visibility = Visibility.Collapsed;
            ShortMainListBox.Visibility = Visibility.Visible;
        }

        private void SetDetailedList()
        {
            ApplicationBar = (Microsoft.Phone.Shell.ApplicationBar)Resources["DetailedMainAppBar"];
            MainListBox.Visibility = Visibility.Visible;
            ShortMainListBox.Visibility = Visibility.Collapsed;
        }

    }
}