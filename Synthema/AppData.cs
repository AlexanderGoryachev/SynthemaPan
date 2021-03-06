﻿using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Synthema
{

    class AppData
    {

        #region Flags

        public static bool IsMainDownloaded = false;
        public static bool IsNewsDownloaded = false;
        public static bool IsShortNewsDownloaded = false;
        public static bool IsInternetAccess = true;

        #endregion

        #region Smiles

        public static Dictionary<string, string> Smiles = new Dictionary<string, string>()
        {
            { "smile", ":-)" },
            { "wink", ";-)" },
            { "biggrin", ":-D" },
            { "belay", ":-O" },
            { "laughing", ":-D" }
        };

        #endregion

        #region Main

        public static string MainString = string.Empty;

        public static ObservableCollection<MainItem> MainItems = new ObservableCollection<MainItem>();

        public class MainItem
        {
            public string GroupTitle { get; set; }
            public string AlbumTitle { get; set; }
            public string Link { get; set; }
            public string ImgUrl { get; set; }
            public string ThumbUrl { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }
            public string PubDate { get; set; }
        }

        #endregion

        #region News

        public static string NewsString = string.Empty;
        public static string ShortNewsString = string.Empty;

        public static ObservableCollection<NewsItem> NewsItems = new ObservableCollection<NewsItem>();
        public static ObservableCollection<ShortNewsItem> ShortNewsItems = new ObservableCollection<ShortNewsItem>();

        public class NewsItem
        {
            public string Title { get; set; }
            public string Link { get; set; }
            public string ImgUrl { get; set; }
            public string ThumbUrl { get; set; }
            public string Description { get; set; }
            public string PubDate { get; set; }
        }

        public class ShortNewsItem
        {
            public string Title { get; set; }
            public string Link { get; set; }
        }

        #endregion

        #region MainDetail

        public static ObservableCollection<Comment> Comments = new ObservableCollection<Comment>();

        public class Comment
        {
            public string Username { get; set; }
            public string Userpic { get; set; }
            public string Date { get; set; }
            public List<Synthema.Common.StringHelper.CommentText> Text { get; set; }
        }

        public static List<SimilarLink> SimilarLinks = new List<SimilarLink>();

        public class SimilarLink
        {
            public string GroupTitle { get; set; }
            public string AlbumTitle { get; set; }
            public string Url { get; set; }
        }


        #endregion

        #region Links

        public static List<Link> Links = new List<Link>() 
        {
            new Link { DisplayText = "Synthema.ru",                 LinkUrl = "http://synthema.ru",                        IconSource = new Uri(@"/Resources/LinksIcons/synthema-32.png", UriKind.Relative),        IconBackgroundColor = new SolidColorBrush(Colors.Black), },
            new Link { DisplayText = "Twitter",                     LinkUrl = "http://twitter.com/synthema_ru",            IconSource = new Uri(@"/Resources/LinksIcons/twitter-32.png", UriKind.Relative),         IconBackgroundColor = new SolidColorBrush(Color.FromArgb(255,0,172,238)), },
            new Link { DisplayText = "Last.fm",                     LinkUrl = "http://www.lastfm.ru/group/synthema.ru",    IconSource = new Uri(@"/Resources/LinksIcons/lastfm-32.png", UriKind.Relative),          IconBackgroundColor = new SolidColorBrush(Color.FromArgb(255,226,19,3)), },
            new Link { DisplayText = "Deutsche Alternative Charts", LinkUrl = "http://www.deutsche-alternative-charts.de", IconText="DAC",                                                                          IconBackgroundColor = new SolidColorBrush(Color.FromArgb(255,158,38,34)), },
            new Link { DisplayText = "European Alternative Charts", LinkUrl = "http://www.alt-charts.eu",                  IconText="EAC",                                                                          IconBackgroundColor = new SolidColorBrush(Color.FromArgb(255,50,70,85)), },
            new Link { DisplayText = "German Electronic Webcharts", LinkUrl = "http://www.gewc.de/reviews,0,2,1.html",     IconText="GEW",                                                                          IconBackgroundColor = new SolidColorBrush(Color.FromArgb(255,242,88,36)), },
            new Link { DisplayText = "Дети Ночи",                   LinkUrl = "http://deti-nochi.kiev.ua",                 IconText="ДН",                                                                           IconBackgroundColor = new SolidColorBrush(Colors.Black), },
            new Link { DisplayText = "OldSchoolEBM.ru",             LinkUrl = "http://www.oldschoolebm.ru",                IconSource = new Uri(@"/Resources/LinksIcons/oldschoolebm-32.png", UriKind.Relative),    IconBackgroundColor = new SolidColorBrush(Color.FromArgb(255,250,130,70)), },
            new Link { DisplayText = "DarkBelarus",                 LinkUrl = "http://dark.by",                            IconText="DB",                                                                           IconBackgroundColor = new SolidColorBrush(Colors.Black), },
            new Link { DisplayText = "Alldayplus.ru",               LinkUrl = "http://alldayplus.ru",                      IconSource = new Uri(@"/Resources/LinksIcons/alldayplus-32.png", UriKind.Relative),      IconBackgroundColor = new SolidColorBrush(Color.FromArgb(255,43,156,230)), },
            new Link { DisplayText = "Zillo",                       LinkUrl = "http://zillo.de",                           IconText="Z",                                                                            IconBackgroundColor = new SolidColorBrush(Color.FromArgb(255,65,115,115)), }
        };

        public class Link
        {
            public string LinkUrl { get; set; }
            public string DisplayText { get; set; }
            public string IconText { get; set; }
            public Uri IconSource { get; set; }
            public Brush IconBackgroundColor { get; set; }
        }        
        
        #endregion

        #region Settings

        public static bool IsLiveTileOn;
        public static bool IsShortReleasesListOn;
        public static bool IsShortNewsListOn;
        //public static bool IsLiveTileSignsOn;

        public static void LoadSettings()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            settings.TryGetValue<bool>("isLiveTileOn", out IsLiveTileOn);
            settings.TryGetValue<bool>("isShortReleasesListOn", out IsShortReleasesListOn);
            settings.TryGetValue<bool>("isShortNewsListOn", out IsShortNewsListOn);
            //settings.TryGetValue<bool>("isLiveTileSignsOn", out IsLiveTileSignsOn);
        }

        public static void SaveSettings()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            settings["isLiveTileOn"] = IsLiveTileOn;
            settings["isShortReleasesListOn"] = IsShortReleasesListOn;
            settings["isShortNewsListOn"] = IsShortNewsListOn;
            //settings["isLiveTileSignsOn"] = IsLiveTileSignsOn;
            settings.Save();
        }
        
        #endregion

        #region AppMethods

        public static void ChangeBackFlipTileData(string backTitle, string backContent, Uri backImage, string wideBackContent, Uri wideBackIamge)
        {
            ShellTile apptile = ShellTile.ActiveTiles.First();
            FlipTileData appFlipTileData = new FlipTileData();

            appFlipTileData.BackTitle = " ";
            appFlipTileData.BackContent = " ";
            appFlipTileData.WideBackContent = " ";

            appFlipTileData.BackBackgroundImage = backImage;
            appFlipTileData.WideBackBackgroundImage = wideBackIamge;
            apptile.Update(appFlipTileData);
        }

        public static string ReplaceHtmlTags(string HtmlString)
        {
            HtmlString.Replace("&quot;", "\"");
            HtmlString.Replace("&nbsp;", " ");
            HtmlString.Replace("<br>", "\n"); 
            HtmlString.Replace("<br/>", "\n"); 
            HtmlString.Replace("<br />", "\n");

            return HtmlString;
        }

        #endregion
    }
}
