using System;
using System.Collections.Generic;

namespace Exercise_7
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter url: ");
            string urlStr = Console.ReadLine();

            UriBuilder ub = new UriBuilder(urlStr);

            Queue<WebCrawler.WebLink> initialUrls = new Queue<WebCrawler.WebLink>();
            initialUrls.Enqueue(new WebCrawler.WebLink(ub.Uri, 1));

            WebCrawler wc = new WebCrawler(initialUrls, 2, 20);

            Console.WriteLine("\n\nValid links: ");
            printLinks(wc.GetVisitedUrls(), true);

            Console.WriteLine("\n\nInvalid links: ");
            printLinks(wc.GetVisitedUrls(), false);
        }

        private static void printLinks(Dictionary<Uri, bool> links, bool isValid)
        {
            foreach (KeyValuePair<Uri, bool> kv in links)
            {
                if (kv.Value == isValid)
                {
                    Console.WriteLine("    {0}", kv.Key);
                }
            }
        }
    }
}
