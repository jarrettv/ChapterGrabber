using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Xml.Linq;

namespace JarrettVance.ChapterTools.Grabbers
{
  public class TagChimpGrabber : ChapterGrabber
  {
    static XDocument searchXml = new XDocument();

    public override void PopulateNames(SearchResult result, ChapterInfo chapterInfo, bool includeDurations)
    {
      ImportFromSearchXml(chapterInfo.Chapters, searchXml, int.Parse(result.Id));
    }

    public override List<SearchResult> Search(ChapterInfo chapterInfo)
    {
      using (WebClient wc = new WebClient())
      {
        string url = "https://www.tagchimp.com/ape/lookup.php?token=10615158044869B9D96D50D&type=search&title={0}&totalChapters={1}&videoKind=Movie";
        url = string.Format(url, Uri.EscapeUriString(chapterInfo.Title), chapterInfo.Chapters.Count);
          //Uri.EscapeUriString
        //TODO: update to use streaming rather than downloading complete.
        string result = wc.DownloadString(new Uri(url));

        //remove invalid chars from result
        result = new string(result.ToCharArray().Where(c => IsLegalXmlChar((int)c)).ToArray());
        //result = result.Replace("&", "&amp;"); //tagChimp fixed, no longer needed, it's back
        //result = result.Replace("'", "&apos;"); //tagChimp fixed, no longer needed
        searchXml = XDocument.Parse(result);
        var titles = from m in searchXml.Descendants("movie")
                     select new SearchResult
                     {
                       Id = (string)m.Element("tagChimpID"),
                       Name = (string)m.Descendants("movieTitle").First(),
                       Count = m.Descendants("chapter").Count(),
                       Relevance = (string)m.Element("locked") == "yes" ? 5 : 0,
                     };
        OnSearchComplete();
        return titles.ToList();
      }
    }


    static void ImportFromSearchXml(List<ChapterEntry> chapters, XDocument xml, int id)
    {
      //if (chapters.Count == 0) throw new Exception("Cannot import names to an empty chapter list.");

      var movie = xml.Descendants("movie").Where(m => (int)m.Element("tagChimpID") == id).FirstOrDefault();
      var names = movie.Descendants("chapterNumber").Select(c => new { Number = int.Parse(c.Value), Title = c.ElementsAfterSelf("chapterTitle").First().Value });
      //var names = from c in movie.Descendants("chapter")
      //            select new
      //            {
      //              Number = (int)c.Element("chapterNumber"),
      //              Title = (string)c.Element("chapterTitle")
      //            };
      names = names.OrderBy(n => n.Number).Distinct();
      if (chapters.Count > 0)
      {
        for (int i = 0; i < chapters.Count; i++)
        {
          string name = names.Where(n => n.Number == i + 1).Select(n => n.Title).FirstOrDefault();
          if (!string.IsNullOrEmpty(name))
            chapters[i] = new ChapterEntry() { Name = name, Time = chapters[i].Time };
        }
      }
      else
      {
        chapters.AddRange(names.Select(n => new ChapterEntry() { Name = n.Title }));
      }
    }

    /// <summary>
    /// Whether a given character is allowed by XML 1.0.
    /// </summary>
    static bool IsLegalXmlChar(int character)
    {
      return
      (
         character == 0x9 /* == '\t' == 9   */          ||
         character == 0xA /* == '\n' == 10  */          ||
         character == 0xD /* == '\r' == 13  */          ||
        (character >= 0x20 && character <= 0xD7FF) ||
        (character >= 0xE000 && character <= 0xFFFD) ||
        (character >= 0x10000 && character <= 0x10FFFF)
      );
    }

    public override void PopulateNames(string hash, ChapterInfo info)
    {
        throw new NotImplementedException();
    }

    public override void Upload(ChapterInfo chapterInfo)
    {
        throw new NotImplementedException();
    }
  }
}
