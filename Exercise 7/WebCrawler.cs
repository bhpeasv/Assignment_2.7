using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace Exercise_7
{
    class WebCrawler
    {
        private Queue<WebLink> frontier;
        private Dictionary<Uri, bool> visitedUrls;
        private int maxLevel;
        private int maxLinks;

        private WebClient wc;
       
        public WebCrawler(Queue<WebLink> initialUrls, int maxDepth, int maxLinksToFind)
        {
            frontier = initialUrls;
            visitedUrls = new Dictionary<Uri, bool>();
            maxLevel = maxDepth;
            maxLinks = maxLinksToFind;

            wc = new WebClient();
            wc.Headers.Add(HttpRequestHeader.UserAgent, "User Agent. Development and testing. bhp@easv.dk");
            crawl();
        }

        private void crawl()
        {
            while (frontier.Count > 0 && visitedUrls.Count < maxLinks)
            {
                WebLink link = frontier.Dequeue();
                if (!visitedUrls.ContainsKey(link.WebUrl))
                {
                    resolvePage(link);
                }
            }
        }

 
        private void resolvePage(WebLink webLink)
        {
            visitedUrls[webLink.WebUrl] = true;

            try
            {
                // try to download the webpage. Throws exception if the url is bad.
                string webPage = wc.DownloadString(webLink.WebUrl.ToString());

                if (webLink.Level < maxLevel)
                {
                    extractLinks(webPage, webLink);
                }
            }
            catch 
            {
                // unable to load page
                visitedUrls[webLink.WebUrl] = false;
            }
        }

        private void extractLinks(string webPage, WebLink link)
        {
            // look for links in the webpage
            var urlTagPattern = new Regex(@"<a.*?href\s*=\s*[""'](?<url>.*?)[""'].*?</a>", RegexOptions.IgnoreCase);

            var urls = urlTagPattern.Matches(webPage);

            Uri baseUrl = new UriBuilder(link.WebUrl.Host).Uri;

            foreach (Match url in urls)
            {
                string newUrl = url.Groups["url"].Value.ToLower();
                try
                {

                    Uri absoluteUrl = normalizedUrl(baseUrl, newUrl);
                    string urlString = absoluteUrl.ToString().ToLower();

                    if (!visitedUrls.ContainsKey(absoluteUrl) && absoluteUrl.Scheme == Uri.UriSchemeHttp || absoluteUrl.Scheme == Uri.UriSchemeHttps)
                    {
                        frontier.Enqueue(new WebLink(absoluteUrl, link.Level + 1));
                    }
                }
                catch
                {
                    // un-resolveable url.. just continue...
                }
            }
        }

        private Uri normalizedUrl(Uri baseUrl, string newUrl)
        {
            Uri normalizedUrl;
            Uri.TryCreate(baseUrl, newUrl, out normalizedUrl);
            return normalizedUrl;
        }


        public Dictionary<Uri, bool> GetVisitedUrls()
        {
            return new Dictionary<Uri, bool>(visitedUrls);
        }

        public class WebLink
        { 
            public Uri WebUrl {get; set; }
            public int Level {get;  set; }
            public WebLink(Uri link, int level)
            {
                WebUrl = link;
                Level = level;
            }
        }
    }
}
