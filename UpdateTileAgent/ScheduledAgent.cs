using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Scheduler;

using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.IO;
using System.Diagnostics;
using System.Net;
using HtmlAgilityPack;

namespace UpdateTileAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static ScheduledAgent()
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            //TODO: Add code to perform your task in background
            WebClient webClient = new WebClient();
            webClient.DownloadStringAsync(new Uri("http://synthema.ru"));
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadStringCompleted);
        }

        private void DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(e.Result);

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(@".//*[@id='dle-content']/div[@class='theblock'][1]");
            if (nodes == null)
                return;

            var thumbUrl = string.Empty;
            var imgUrl = string.Empty;
            foreach (HtmlNode node in nodes)
            {
                thumbUrl = "http://www.synthema.ru" + node.SelectSingleNode(@"div[@class='news']/div/div[1]//img").GetAttributeValue("src", "");
                imgUrl = string.Empty;
                //try { imgUrl = node.SelectSingleNode(@"div[@class='news']/div/div[1]/a").GetAttributeValue("href", ""); }
                //catch { imgUrl = thumbUrl; }
            }

            ShellTile apptile = ShellTile.ActiveTiles.First();
            FlipTileData appFlipTileData = new FlipTileData();

            appFlipTileData.BackTitle = " ";
            appFlipTileData.BackContent = " ";
            //appFlipTileData.WideBackContent = " ";

            appFlipTileData.BackBackgroundImage = new Uri(thumbUrl, UriKind.Absolute);
            //appFlipTileData.WideBackBackgroundImage = new Uri(imgUrl, UriKind.Absolute);

            apptile.Update(appFlipTileData);

            NotifyComplete();
        }
    }
}