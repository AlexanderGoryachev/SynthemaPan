﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthema
{
    class Constants
    {
        public const string BaseUrl = @"http://www.synthema.ru";
        public const string NewsUrl = BaseUrl + @"/news.html";
        // public const string BaseUrl = @"http://www.synthema.ru/page/2";

        public const string MainItemsStorageFileName = "MainItems.json";
        public const string NewsItemsStorageFileName = "NewsItems.json";

        public const string ReviewsRssPath = @"http://www.synthema.ru/reviews/rss.xml";

        public const string AuthorMail = "sppro@rambler.ru";
    }
}
