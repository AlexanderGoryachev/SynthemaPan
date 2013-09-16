﻿using MSPToolkit.Encodings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Synthema.Common
{
    class DownloadingService
    {
        public static WebClient webClient = new WebClient();
        public static void DownloadString(string Path)
        {
            webClient.Encoding = new Windows1251Encoding();
            webClient.DownloadStringAsync(new Uri(Path));
        }
    }
}
