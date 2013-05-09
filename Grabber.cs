/********************************************************************************
*
*    Copyright(C) 2003-2008 Jarrett Vance http://jvance.com
*
*    This file is part of ChapterGrabber
*
*	 ChapterGrabber is free software; you can redistribute it and/or modify
*    it under the terms of the GNU General Public License as published by
*    the Free Software Foundation; either version 2 of the License, or
*    (at your option) any later version.
*
*    ChapterGrabber is distributed in the hope that it will be useful,
*    but WITHOUT ANY WARRANTY; without even the implied warranty of
*    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*    GNU General Public License for more details.
*
*    You should have received a copy of the GNU General Public License
*    along with this program; if not, write to the Free Software
*    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*
********************************************************************************/
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;


namespace JarrettVance.ChapterTools
{
    /// <summary>
    /// Summary description for ChapterGrabber.
    /// </summary>
    public static class Grabber
    {
        //this is a bullshit method because 
        //   TimeSpan.ToString(string format) is missing
        public static string ToShortString(this TimeSpan ts)
        {
            string time;
            time = ts.Hours.ToString("00");
            time = time + ":" + ts.Minutes.ToString("00");
            time = time + ":" + ts.Seconds.ToString("00");
            time = time + "." + ts.Milliseconds.ToString("000");
            return time;
        }

        public static string ToShorterString(this TimeSpan ts)
        {
            string time;
            time = ts.Hours.ToString("00");
            time = time + ":" + ts.Minutes.ToString("00");
            time = time + ":" + ts.Seconds.ToString("00");
            return time;
        }

        private static IEnumerable<string> GoogleTitle(string title)
        {
            string json = string.Empty;
            using (var web = new WebClient())
            {
                json = web.DownloadString(string.Format(
                    "http://ajax.googleapis.com/ajax/services/search/web?v=1.0&q={0}",
                    Uri.EscapeUriString("movie " + title)));
            }

            // {"responseData": {"results":[{"GsearchResultClass":"GwebSearch","unescapedUrl":"http://www.imdb.com/title/tt0253867/","url":"http://www.imdb.com/title/tt0253867/","visibleUrl":"www.imdb.com","cacheUrl":"http://www.google.com/search?q\u003dcache:pd6Fc1Wx1TgJ:www.imdb.com","title":"The Sweetest \u003cb\u003eThing\u003c/b\u003e (2002)","titleNoFormatting":"The Sweetest Thing (2002)","content":"The Sweetest \u003cb\u003eThing\u003c/b\u003e -- A girl finds she is forced to educate herself on the \u003cb\u003e....\u003c/b\u003e   Not a family \u003cb\u003emovie\u003c/b\u003e. It\u0026#39;s meant more for the dirty-minded, if you will. :) \u003cb\u003e...\u003c/b\u003e"},{"GsearchResultClass":"GwebSearch","unescapedUrl":"http://www.imdb.com/title/tt0159780/","url":"http://www.imdb.com/title/tt0159780/","visibleUrl":"www.imdb.com","cacheUrl":"http://www.google.com/search?q\u003dcache:rUCUtqkJtW8J:www.imdb.com","title":"\u003cb\u003eSweet Thing\u003c/b\u003e (1999)","titleNoFormatting":"Sweet Thing (1999)","content":"Contact: View company contact information for \u003cb\u003eSweet Thing\u003c/b\u003e on IMDbPro. \u003cb\u003e...\u003c/b\u003e   Discuss this \u003cb\u003emovie\u003c/b\u003e with other users on IMDb message board for \u003cb\u003eSweet Thing\u003c/b\u003e (1999)   \u003cb\u003e...\u003c/b\u003e"},{"GsearchResultClass":"GwebSearch","unescapedUrl":"http://www.shamelove.com/sweetthing/","url":"http://www.shamelove.com/sweetthing/","visibleUrl":"www.shamelove.com","cacheUrl":"http://www.google.com/search?q\u003dcache:vDT6ygU4kGgJ:www.shamelove.com","title":"\u003cb\u003eSweet Thing\u003c/b\u003e","titleNoFormatting":"Sweet Thing","content":"Nineteen-year-old free spirit Liz is on a mission to find love in between her   shifts at an espresso drive-thru. Across town, Jody seeks answers in drugs \u003cb\u003e...\u003c/b\u003e"},{"GsearchResultClass":"GwebSearch","unescapedUrl":"http://www.myspace.com/sweetthingmovie","url":"http://www.myspace.com/sweetthingmovie","visibleUrl":"www.myspace.com","cacheUrl":"http://www.google.com/search?q\u003dcache:W-BSYXt76PoJ:www.myspace.com","title":"\u003cb\u003eSWEET THING\u003c/b\u003e on MySpace Films - New Films \u0026amp; Documentaries","titleNoFormatting":"SWEET THING on MySpace Films - New Films \u0026amp; Documentaries","content":"MySpace Filmmaker profile for \u003cb\u003eSWEET THING\u003c/b\u003e. Watch, \u003cb\u003emovie\u003c/b\u003e clips \u0026amp; trailers from   \u003cb\u003eSWEET THING\u003c/b\u003e, including films, movies \u0026amp; documentaries."}],"cursor":{"pages":[{"start":"0","label":1},{"start":"4","label":2},{"start":"8","label":3},{"start":"12","label":4},{"start":"16","label":5},{"start":"20","label":6},{"start":"24","label":7},{"start":"28","label":8}],"estimatedResultCount":"1840000","currentPageIndex":0,"moreResultsUrl":"http://www.google.com/search?oe\u003dutf8\u0026ie\u003dutf8\u0026source\u003duds\u0026start\u003d0\u0026hl\u003den\u0026q\u003dsweet+thing+movie"}}, "responseDetails": null, "responseStatus": 200}
            int idx = 0;
            string search = "titleNoFormatting\":\"";
            while (idx > -1)
            {

                idx = json.IndexOf(search, idx);
                if (idx > 0)
                {
                    idx = idx + search.Length;
                    int length = json.IndexOf('"', idx) - idx;
                    string t = json.Substring(idx, length);
                    yield return t.Trim().Replace("\\u0026amp;", "&");
                }
            }
        }

        public static List<KeyValuePair<int, string>> SuggestTitles(string title)
        {
            if (string.IsNullOrEmpty(title)) return new List<KeyValuePair<int, string>>();

            return Grabber.FilterMovieTitles(Grabber.GoogleTitle(title))
                .GroupBy(k => k.Value).Select(k => new KeyValuePair<int, string>(k.Sum(t => t.Key), k.Key))
                .OrderByDescending(k => k.Key).ToList();
        }

        public static IEnumerable<KeyValuePair<int, string>> FilterMovieTitles(IEnumerable<string> titles)
        {
            foreach (string t in titles)
            {
                if (t.IndexOf(" (20") > 0) yield return new KeyValuePair<int, string>(1, t.Substring(0, t.IndexOf(" (20")));
                else if (t.IndexOf(" (19") > 0) yield return new KeyValuePair<int, string>(1, t.Substring(0, t.IndexOf(" (19")));
                else if (t.IndexOf(" (film) - Wikipedia") > 0) yield return new KeyValuePair<int, string>(1, t.Substring(0, t.IndexOf(" (film) - Wikipedia")));
                else if (t.IndexOf(" - Wikipedia") > 0) yield return new KeyValuePair<int, string>(1, t.Substring(0, t.IndexOf(" - Wikipedia")));
                else if (t.IndexOf(" Movie Reviews") > 0) yield return new KeyValuePair<int, string>(1, t.Substring(0, t.IndexOf(" Movie Reviews")));
                else if (t.IndexOf(" - YouTube") > 0) yield return new KeyValuePair<int, string>(1, t.Substring(0, t.IndexOf(" - YouTube")));
                else yield return new KeyValuePair<int, string>(0, t);
            }
        }


        //public static List<Chapter> LoadTextChapters(string filename)
        //{
        //  List<Chapter> list = new List<Chapter>();

        //  int num = 0;
        //  TimeSpan ts = new TimeSpan(0);
        //  string time = String.Empty;
        //  string name = String.Empty;
        //  bool onTime = true;
        //  string[] lines = File.ReadAllLines(filename);
        //  foreach (string line in lines)
        //  {
        //    if (onTime)
        //    {
        //      num++;
        //      //read time
        //      time = line.Replace("CHAPTER" + num.ToString("00") + "=", "");
        //      ts = TimeSpan.Parse(time);
        //    }
        //    else
        //    {
        //      //read name
        //      name = line.Replace("CHAPTER" + num.ToString("00") + "NAME=", "");
        //      //add it to list
        //      list.Add(new Chapter() { Name = name, Time = ts });
        //    }
        //    onTime = !onTime;
        //  }
        //  return list;
        //}

        public static void ImportFromClipboard(List<ChapterEntry> chapters, string clipboard, bool includeDuration)
        {
            clipboard = clipboard.Replace("\t", string.Empty);
            for (int i = 0; i < chapters.Count; i++)
                chapters[i] = new ChapterEntry() { Time = chapters[i].Time, Name = ExtractFromCopy(clipboard, i + 1, includeDuration) };
        }

        //  public static bool ImportFromWeb(List<Chapter> chapters, string title, string ean, bool includeDuration)
        //  {
        //throw new NotImplementedException();
        //      //first download page
        //      string html = "";
        //      WebResponse result = null;
        //      try 
        //      {
        //          string URL = "http://video.barnesandnoble.com/search/product.asp?EAN="+ean+"&VIEW=SCN";
        //          WebRequest req = WebRequest.Create(URL);
        //          result = req.GetResponse();
        //          Stream ReceiveStream = result.GetResponseStream();
        //          Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
        //          StreamReader sr = new StreamReader( ReceiveStream, encode );

        //          Char[] read = new Char[256];
        //          int count = sr.Read( read, 0, 256 );

        //          while (count > 0) 
        //          {
        //              String str = new String(read, 0, count);
        //              html = html + str;
        //              count = sr.Read(read, 0, 256);
        //          }
        //      } 
        //      catch(Exception) 
        //      {
        //      } 
        //      finally 
        //      {
        //          if ( result != null ) 
        //          {
        //              result.Close();
        //          }
        //      }
        //      //try and get movie title
        //      string strTitle = "";

        //      int intB, intL;
        //      string strFind = "<title>Barnes&nbsp;&amp;&nbsp;Noble.com - ";

        //      intB = html.IndexOf(strFind) + strFind.Length;
        //      intL = html.IndexOf("</title>") - intB;

        //      try
        //      {
        //          title = html.Substring(intB, intL);
        //      }
        //      catch (Exception) {
        //      }

        //      for(int i=0; i<chapters.Count; i++)
        //          //chapters[i].Name = ExtractFromHtml(html, i+1, includeDuration);
        //      return true;
        //  }



        private static string ExtractFromHtml(string html, int chapterNum, bool includeDuration)
        {
            int searchAt = html.IndexOf("<a name=\"SCN\"><b>Scene Index</b></a>");
            if (searchAt == -1) return "Chapter " + chapterNum.ToString();
            string lookfor = "<br><br> " + chapterNum.ToString() + ". ";
            if (chapterNum > 9) lookfor = "<br><br>" + chapterNum.ToString() + ". ";
            int nameAt = html.IndexOf(lookfor, searchAt);
            int nameTo = html.IndexOf("<br>", nameAt + 5);
            if (nameAt == -1 || nameTo == -1) return "ChapterName " + chapterNum.ToString();
            string name = html.Substring(nameAt, nameTo - nameAt).Replace(lookfor, String.Empty);
            if (!includeDuration && name.LastIndexOf('[') > 1) name = name.Substring(0, name.LastIndexOf('[') - 1);
            return name;
        }

        private static string ExtractFromCopy(string clipboard, int chapterNum, bool includeDuration)
        {
            int nameAt = clipboard.IndexOf(chapterNum.ToString() + ". ");
            int nameTo = clipboard.IndexOf("\n", nameAt + 2 + chapterNum.ToString().Length);
            if (nameAt == -1 || nameTo == -1) return "Chapter " + chapterNum.ToString();
            string name = clipboard.Substring(nameAt + 2 + chapterNum.ToString().Length, nameTo - (nameAt + 2 + chapterNum.ToString().Length));
            if (!includeDuration) return name.RemoveDuration();
            else return name.Trim();
        }

        public static string RemoveDuration(this string val)
        {
            if (val.LastIndexOf('[') > 1) val = val.Substring(0, val.LastIndexOf('[') - 1);
            return val.Trim();
        }
    }
}
