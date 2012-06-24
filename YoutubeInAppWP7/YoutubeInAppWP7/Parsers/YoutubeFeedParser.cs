using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using YoutubeInAppWP7.Models;

namespace YoutubeInAppWP7.Parsers
{
    /// <summary>
    /// Parses content from an Atom feed - http://www.w3.org/2005/Atom
    /// </summary>
    public static class YoutubeFeedParser
    {
        public static IEnumerable<YoutubeFeedItem> Parse(String streamData)
        {
            var feed = XElement.Parse(streamData);

            XNamespace ns = "http://www.w3.org/2005/Atom";
            XNamespace media = "http://search.yahoo.com/mrss/";
            if (feed != null)
            {
                var entries = from entry in feed.Descendants(ns + "entry")
                              select new YoutubeFeedItem
                              {
                                  Title = entry.Element(ns + "title").Value,
                                  Description = entry.Element(ns + "content").Value,
                                  //Note that I am specifically interested in the 'http://www.youtube.com/watch?v=' formatted url
                                  VideoUrl = (from el in entry.Elements(media + "group").Elements(media + "player")
                                              select el.Attribute("url").Value).First(),
                                  ThumbnailUrl = (from el in entry.Elements(media + "group").Elements(media + "thumbnail")
                                                  select el.Attribute("url").Value).First(),
                                  PostedDate = DateTime.Parse(entry.Element(ns + "published").Value)
                              };

                
                return entries;
            }

            return null;
        }
    }
}

