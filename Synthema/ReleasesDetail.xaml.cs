﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MSPToolkit.Encodings;
using HtmlAgilityPack;
using Microsoft.Phone.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using Synthema.Common;
using System.Windows.Data;

namespace Synthema
{
    public partial class NewsDetail : PhoneApplicationPage
    {
        private string _releasesDetailPath = string.Empty;
        private Uri coverUri;

        public NewsDetail()
        {
            InitializeComponent();

            if (!AppData.IsInternetAccess)
                MessageBox.Show("Подключение к Интернету отсутствует. Для работы приложения необходим доступ к сети");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _releasesDetailPath = NavigationContext.QueryString["releasesDetailPath"].ToString();
            DownloadMainDetail(_releasesDetailPath);
        }

        private void DownloadMainDetail(string Path)
        {
            WebClient newsDetails = new WebClient();
            newsDetails.Encoding = new Windows1251Encoding();
            newsDetails.DownloadStringAsync(new Uri(Path));
            newsDetails.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadReleasesDetailStringCompleted);
            TopPageProgressBar.IsIndeterminate = true;
        }

        private void DownloadReleasesDetailStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
                return;

            ParseReleasesDetail(e.Result);

            CommentsListBox.ItemsSource = AppData.Comments;
            //SimilarListBox.ItemsSource = AppData.SimilarLinks;

            TopPageProgressBar.IsIndeterminate = false;
        }

        private void ParseReleasesDetail(string HtmlString)
        {
            AppData.Comments.Clear();

            HtmlDocument doc = new HtmlDocument();
            //HtmlNode.ElementsFlags.Remove("form");
            doc.LoadHtml(HtmlString);

            #region Information

            var title = doc.DocumentNode.SelectSingleNode(@".//*[@id='dle-content']/div[@class='theblock']/div[@class='tbh']/h1").InnerText;
            title = HttpUtility.HtmlDecode(title);
            GroupTitleTextBlock.Text = title.Substring(0, title.IndexOf("- "));
            AlbumTitleTextBlock.Text = title.Remove(0, title.IndexOf("- ") + 2);

            HtmlNode informationBaseNode = doc.DocumentNode.SelectSingleNode(@".//*[@id='dle-content']/div[1]/div[3]");

            try
            {
                var votesPercent = doc.DocumentNode.SelectSingleNode(@".//*[@id='dle-content']/div[@class='theblock']/div[@class='rateblock']/div[@class='rate']/div[@class='rating'][1]/ul/li").InnerText;
                VotesRating.Value = Convert.ToByte(votesPercent) / 20;
                var votesCount = doc.DocumentNode.SelectSingleNode(@".//*[@id='dle-content']/div[@class='theblock']/div[@class='rateblock']/div[@class='rate']/div[@class='rating'][2]").InnerText;
                VotesCountTextBlock.Text = HttpUtility.HtmlDecode(votesCount);
                RatingStackPanel.Visibility = Visibility.Visible;
            }
            catch 
            {
                RatingStackPanel.Visibility = Visibility.Collapsed;
            }



            try
            {
                var coverImageBaseNode = doc.DocumentNode.SelectSingleNode(@".//*[@id='dle-content']/div[@class='theblock']/div[@class='newf']/div/div[1]/a");
                coverUri = new Uri(Constants.BaseUrl + coverImageBaseNode.SelectSingleNode("img").GetAttributeValue("src", ""), UriKind.Absolute);
                CoverImage.Source = new BitmapImage(coverUri);
                BackgroundCoverImage.Visibility = Visibility.Visible;
            }
            catch (NullReferenceException)
            {
                var coverImageBaseNode = doc.DocumentNode.SelectSingleNode(@".//*[@id='dle-content']/div[@class='theblock']/div[@class='newf']/div/div[1]");
                try
                {
                    coverUri = new Uri(Constants.BaseUrl + coverImageBaseNode.SelectSingleNode("img").GetAttributeValue("src", ""), UriKind.Absolute);
                    CoverImage.Source = new BitmapImage(coverUri);
                    BackgroundCoverImage.Visibility = Visibility.Visible;
                }
                catch
                {
                    BackgroundCoverImage.Visibility = Visibility.Collapsed;
                }
            }

            try
            {
                HtmlNodeCollection Links = informationBaseNode.SelectNodes("div//b/a");

                var linksList = new Dictionary<string, string>();

                foreach (HtmlNode node in Links)
                {   
                    if (!linksList.Keys.Contains(node.GetAttributeValue("href", "")))
                        linksList.Add(node.GetAttributeValue("href", ""), node.InnerText);
                }

                LinksListBox.ItemsSource = linksList;

                LinksListBox.Visibility = Visibility.Visible;
            }
            catch (NullReferenceException)
            {
                LinksListBox.Visibility = Visibility.Collapsed;
            }

            try
            {
                var promotext = (informationBaseNode.SelectSingleNode("div/div[text()][1]")).InnerText;
                promotext = HttpUtility.HtmlDecode(promotext);
                promotext = promotext.Replace("<br>", "\n");
                PromotextTextBlock.Text = promotext;
            }
            catch (NullReferenceException)
            {
                PromotextTextBlock.Visibility = Visibility.Collapsed;
            }

            try 
            {
                HtmlNodeCollection Video = informationBaseNode.SelectNodes("div//iframe");

                var VideoList = new List<string>();

                foreach (HtmlNode node in Video)
                {
                    var source = node.GetAttributeValue("src", string.Empty);
                    source = source.Replace("embed/", "watch?v=");
                    source = source.Replace("www", "m");
                    source = source.Replace("?rel=1", string.Empty);
                    VideoList.Add(source);
                }

                VideoListBox.ItemsSource = VideoList;
                VideoTextBlock.Visibility = Visibility.Visible;
            }
            catch
            {
                VideoTextBlock.Visibility = Visibility.Collapsed;
            }

            #endregion

            #region Details

            try
            {
                HtmlNodeCollection Details = informationBaseNode.SelectNodes(@"//div/b[
                                                                                    contains(text(),'Label') or
                                                                                    contains(text(),'Format') or
                                                                                    contains(text(),'Style') or
                                                                                    contains(text(),'Country') or
                                                                                    contains(text(),'Quality') or
                                                                                    contains(text(),'Size') or
                                                                                    contains(text(),'Artist') or
                                                                                    contains(text(),'Title')
                                                                                ]/text()");
                var details = new Dictionary<string, string>();
                foreach (HtmlNode detailNode in Details)
                {
                    var detail = HttpUtility.HtmlDecode(detailNode.InnerHtml);

                    if (detail.Contains("Label"))
                    {
                        detail = detail.Remove(0, 7);
                        details.Add("Лейбл", detail);
                    }
                    else if (detail.Contains("Format"))
                    {
                        detail = detail.Remove(0, 8);
                        details.Add("Формат", detail);
                    }
                    else if (detail.Contains("Style"))
                    {
                        detail = detail.Remove(0, 7);
                        details.Add("Стиль", detail);
                    }
                    else if (detail.Contains("Country"))
                    {
                        detail = detail.Remove(0, 9);
                        details.Add("Страна", detail);
                    }
                    else if (detail.Contains("Quality"))
                    {
                        detail = detail.Remove(0, 9);
                        details.Add("Качество", detail);
                    }
                    else if (detail.Contains("Size"))
                    {
                        detail = detail.Remove(0, 6);
                        details.Add("Размер", detail);
                    }
                    else if (detail.Contains("Artist"))
                    {
                        detail = detail.Remove(0, 8);
                        details.Add("Исполнитель", detail);
                    }
                    else if (detail.Contains("Title"))
                    {
                        detail = detail.Remove(0, 7);
                        details.Add("Название", detail);
                    }
                }
                DetailsListBox.ItemsSource = details;
            }
            catch (NullReferenceException)
            {
                DetailsListBox.Visibility = Visibility.Collapsed;
            }

            #endregion

            #region Tracklist

            try
            {
                //var tracklist = informationBaseNode.SelectSingleNode("div/div[b[contains(text(),'Tracklist')]]").InnerHtml;
                //tracklist = HttpUtility.HtmlDecode(tracklist);
                //tracklist = tracklist.Remove(0, tracklist.IndexOf("<br><br>") + 8);
                //tracklist = tracklist.Replace("<br>", "\n").Replace("<b>", string.Empty).Replace("</b>", string.Empty);
                //TracklistTextBlock.Text = tracklist;

                HtmlNodeCollection tracklist = informationBaseNode.SelectNodes("div/div[b[contains(text(),'Tracklist')]]/text()");
                var tracks = new Dictionary<int, string>();
                var track = string.Empty;
                var i = 0;
                foreach (HtmlNode node in tracklist)
                {
                    track = node.InnerText;
                    track = HttpUtility.HtmlDecode(track);
                    track = track.Trim();
                    track = track.Remove(0, track.IndexOf(" ") + 1);
                    tracks.Add(i + 1, track);
                    i++;
                }
                TracklistListBox.ItemsSource = tracks;

                //tracklist = HttpUtility.HtmlDecode(tracklist);
                //tracklist = tracklist.Remove(0, tracklist.IndexOf("<br><br>") + 8);
                //tracklist = tracklist.Replace("<br>", "\n").Replace("<b>", string.Empty).Replace("</b>", string.Empty);
                //TracklistTextBlock.Text = tracklist;
            }
            catch (NullReferenceException)
            {

            }

            #endregion

            #region Comments

            Stopwatch commentsWatch = new Stopwatch();
            commentsWatch.Start();
            try
            {
                HtmlNodeCollection CommentsNodes = doc.DocumentNode.SelectNodes(@".//*[@id='dle-content']/div/div[@class='theblock']");

                foreach (HtmlNode node in CommentsNodes)
                {
                    var username = node.SelectSingleNode(@"div[@class='comtext']/b").InnerText;
                    var userpic = Constants.BaseUrl + node.SelectSingleNode(@"div[@class='avatar']/img").GetAttributeValue("src", "");
                    var text = node.SelectSingleNode(@"div[@class='comtext']/div").InnerText;
                    var date = node.SelectSingleNode(@"div[@class='comtext']/text()").InnerText;

                    text = HttpUtility.HtmlDecode(text);

                    text = StringHelper.FormatSmiles(text, AppData.Smiles);

                    var commentText = StringHelper.FormatQuotes(text);

                    date = date.Replace("&nbsp;", "");
                    date = date.Trim();

                    AppData.Comments.Add(new AppData.Comment
                    {
                        Username = username,
                        Userpic = userpic,
                        Text = commentText,
                        Date = date
                    });
                }
            }
            catch (NullReferenceException)
            {
                NoCommentsTextBlock.Visibility = Visibility.Visible;
            }

            commentsWatch.Stop();
            var commentsParasingTime = commentsWatch.Elapsed.Milliseconds.ToString();
            #endregion

            //#region Similar links

            //try
            //{
            //    HtmlNode similarLinksBaseNode = doc.DocumentNode.SelectSingleNode(@".//div[@id='dle-content']/div[@class='theblock']");
            //    HtmlNodeCollection similarLinks = similarLinksBaseNode.SelectNodes(@"div[@class='stext']/ul/li/a");

            //    AppData.SimilarLinks.Clear();
            //    foreach (HtmlNode node in similarLinks)
            //    {
            //        var fullTitle = node.InnerText;
            //        fullTitle = HttpUtility.HtmlDecode(fullTitle);
            //        AppData.SimilarLinks.Add(new AppData.SimilarLink
            //        {
            //            GroupTitle = fullTitle.Substring(0, fullTitle.IndexOf("- ")),
            //            AlbumTitle = fullTitle.Remove(0, fullTitle.IndexOf("- ") + 2),
            //            Url = "/ReleasesDetail.xaml?releasesDetailPath=" + node.GetAttributeValue("href", "")
            //        });
            //    }
            //}
            //catch (NullReferenceException)
            //{
            //    NoSimilarTextBlock.Visibility = Visibility.Visible;
            //}

            //#endregion

        }

        private void OpenInBrowser_Click(object sender, EventArgs e)
        {
            WebBrowserTask openInBrowser = new WebBrowserTask();
            openInBrowser.Uri = new Uri(_releasesDetailPath, UriKind.Absolute);
            openInBrowser.Show();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            NavigationService.Navigate(new Uri("/ReleasesPage.xaml", UriKind.Relative));
        }

        private void CoverImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {                
            NavigationService.Navigate(new Uri("/ImagePage.xaml?imgUrl=" + coverUri.ToString(), UriKind.Relative));
        }
    }
}