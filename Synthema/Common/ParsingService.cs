using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Synthema.Common
{
    class ParsingService
    {
        public static void ParseReleasesHtml(string HtmlString)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HtmlString);

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(@".//*[@id='dle-content']/div[@class='theblock']");
            if (nodes == null)
                return;

            AppData.MainItems.Clear();

            foreach (HtmlNode node in nodes)
            {
                var title = node.SelectSingleNode(@"div[@class='tbh']/h2/a").InnerText;
                var link = node.SelectSingleNode(@"div[@class='tbh']/h2/a").GetAttributeValue("href", "http://");
                var thumbUrl = Constants.BaseUrl + node.SelectSingleNode(@"div[@class='news']/div/div[1]//img").GetAttributeValue("src", "");
                var description = node.SelectSingleNode(@"div[@class='news']/div/div/text()").InnerText;
                var pubDate = node.SelectSingleNode(@"div[@class='tbnfo']").InnerText;
                var imgUrl = string.Empty;
                try { imgUrl = node.SelectSingleNode(@"div[@class='news']/div/div[1]/a").GetAttributeValue("href", ""); }
                catch { imgUrl = thumbUrl; }

                var category = node.SelectSingleNode(@"div[@class='slink']/div[@class='atags']//a").InnerText;

                title = HttpUtility.HtmlDecode(title);
                var groupTitle = title.Substring(0, title.IndexOf("- "));
                var albumTitle = title.Remove(0, title.IndexOf("- ") + 2);

                description = HttpUtility.HtmlDecode(description);

                pubDate = pubDate.Replace("&nbsp;", "");

                AppData.MainItems.Add(new AppData.MainItem
                {
                    GroupTitle = groupTitle,
                    AlbumTitle = albumTitle,
                    Link = link,
                    ImgUrl = imgUrl,
                    ThumbUrl = thumbUrl,
                    Category = category,
                    Description = description,
                    PubDate = pubDate
                });
            }
        }

        public static void ParseNewsHtml(string HtmlString)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HtmlString);

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(@".//div[@class='col2-3'][1]/div[@class='sceneCol23c']/div[@class='sceneNews']");
            if (nodes == null)
                return;

            AppData.NewsItems.Clear();

            foreach (HtmlNode node in nodes)
            {
                var _title = node.SelectSingleNode(@"div[@class='title']/h2/a").InnerText;
                var _link = node.SelectSingleNode(@"div[@class='title']/h2/a").GetAttributeValue("href", "http://");

                var _thumbUrl = node.SelectSingleNode(@"div[@class='newsm']//img").GetAttributeValue("src", "");

                var _imgUrl = node.SelectSingleNode(@"div[@class='newsm']//a").GetAttributeValue("href", "");

                var _description = string.Empty;
                try { _description = node.SelectSingleNode(@"div[@class='newsm']/text()").InnerText; }
                catch { }

                _description = HttpUtility.HtmlDecode(_description);

                // костыль, помогающий обойти косяк в CMS, лишний раз приписывающий BaseUrl к относительному пути до картинки
                // т.е. иногда Url выглядит так: http://www.synthema.ruhttp://www.synthema.ru/uploads/posts/2013-06/thumbs/1371912912_war.jpg
                _thumbUrl = _thumbUrl.Replace(Constants.BaseUrl, string.Empty);
                _thumbUrl = Constants.BaseUrl + _thumbUrl;

                AppData.NewsItems.Add(new AppData.NewsItem
                {
                    Title = _title,
                    Link = _link,
                    ImgUrl = _imgUrl,
                    ThumbUrl = _thumbUrl,
                    Description = _description,
                });
            }
        }

        public static void ParseShortNewsHtml(string HtmlString)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HtmlString);

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(@".//*[@id='dle-content']/div/div[@class='sceneNews0']");
            if (nodes == null)
                return;

            AppData.NewsItems.Clear();

            foreach (HtmlNode node in nodes)
            {
                var _title = node.SelectSingleNode(@"div[@class='title']/h2/a").InnerText;
                var _link = node.SelectSingleNode(@"div[@class='title']/h2/a").GetAttributeValue("href", "http://");
                
                AppData.ShortNewsItems.Add(new AppData.ShortNewsItem
                {
                    Title = _title,
                    Link = _link
                });
            }
        }
    }
}