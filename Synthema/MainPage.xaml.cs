using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Synthema;
using Microsoft.Phone.Net.NetworkInformation;

namespace Synthema
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!DeviceNetworkInformation.IsNetworkAvailable)
                MessageBox.Show("Проверьте подключение и снова зайдите в приложение", "Отсутствует доступ к Интернету", MessageBoxButton.OK);
        }

        private void ContactWithAuthor_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailComposeTask email = new EmailComposeTask();
            email.To = Constants.AuthorMail;
            email.Show();
        }

        private void RateApp_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var a = new MarketplaceReviewTask();
            a.Show();
        }

        private void Settings_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        private void Releases_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ReleasesPage.xaml", UriKind.Relative));
        }
    }
}