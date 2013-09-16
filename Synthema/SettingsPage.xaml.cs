using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace Synthema
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LiveTileSwitch.IsChecked = AppData.IsLiveTileOn;
            //if (AppData.IsLiveTileOn)
            //    LiveTileSignsStacPanel.Visibility = Visibility.Visible;
            //else
            //    LiveTileSignsStacPanel.Visibility = Visibility.Collapsed;
        }

        private void LiveTileSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (LiveTileSwitch.IsChecked == true)
            {
                AppData.IsLiveTileOn = true;
                //LiveTileSignsStacPanel.Visibility = Visibility.Visible;
            }
            else
            {
                AppData.IsLiveTileOn = false;
                //LiveTileSignsStacPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void LiveTileSignsSwitch_Click(object sender, RoutedEventArgs e)
        {
            //if (LiveTileSwitch.IsChecked == true)
            //    AppData.IsLiveTileSignsOn = true;
            //else
            //    AppData.IsLiveTileSignsOn = false;
        }
    }
}