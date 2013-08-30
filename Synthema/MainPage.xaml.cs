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
using Microsoft.Phone.Scheduler;

namespace Synthema
{
    public partial class MainPage : PhoneApplicationPage
    {
        const string UpdateTileAgentName = "Agent-Tile";
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            PeriodicTask myPeriodicTask = ScheduledActionService.Find(UpdateTileAgentName) as PeriodicTask;

            if (myPeriodicTask != null)
            {
                try { ScheduledActionService.Remove(UpdateTileAgentName); }
                catch (Exception ex) { MessageBox.Show("Невозможно удалить ранее созданный сервис:" + ex.Message); }

                if (AppData.IsLiveTileOn)
                {
                    myPeriodicTask = new PeriodicTask(UpdateTileAgentName);
                    myPeriodicTask.Description = "Agent-Tile";

                    try 
                    { 
                        ScheduledActionService.Add(myPeriodicTask);
                        #if DEBUG
                            if (System.Diagnostics.Debugger.IsAttached == true)
                                ScheduledActionService.LaunchForTest(UpdateTileAgentName, TimeSpan.FromSeconds(10));
                        #endif
                    }
                    catch (Exception ex) { MessageBox.Show("Невозможно создать сервис:" + ex.Message); }
                }
            }       
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

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            while (NavigationService.BackStack.Any())
                NavigationService.RemoveBackEntry();
            base.OnBackKeyPress(e);
        }

        private void News_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/NewsPage.xaml", UriKind.Relative));
        }
    }
}