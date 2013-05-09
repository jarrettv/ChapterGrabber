using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TidyNet;
using System.Net;
using System.Collections.Specialized;
using System.Web;
using System.IO;
using System.Xml.Linq;
using System.Diagnostics;

namespace JarrettVance.ChapterTools.Grabbers
{
  public class MetaServiceGrabber : ChapterGrabber
  {
    //static XDocument searchXhtml = new XDocument();
    public override void PopulateNames(SearchResult result, ChapterInfo chapterInfo, bool includeDurations)
    {
      XDocument xdoc = XDocument.Load(string.Format("http://www.e-home.no/metaservices/XML/GetDVDInfo.aspx?Extended=True&DVDID={0}", result.Id));
      XElement title = xdoc.Descendants("title").Where(t => t.Elements("chapter").Count() > 0)
        .OrderByDescending(t => t.Elements("chapter").Count()).FirstOrDefault();
      if (title == null) return;

      for (int i = 0; i < chapterInfo.Chapters.Count; i++)
      {
        XElement chapter = title.Elements("chapter").Where(c => (int)c.Element("chapterNum") == i+1)
          .FirstOrDefault();
        if (chapter != null)
          chapterInfo.Chapters[i] = new Chapter()
          {
            Time = chapterInfo.Chapters[i].Time,
            Name = includeDurations ? (string)chapter.Element("chapterTitle") : 
            ((string)chapter.Element("chapterTitle")).RemoveDuration()
          };
      }
    }

    public override List<SearchResult> Search(ChapterInfo chapterInfo)
    {
      string result = string.Empty;
      using (WebClient wc = new WebClient())
      {
        //NameValueCollection vars = new NameValueCollection();
        //vars.Add("txtTitle", chapterInfo.Title);
        //vars.Add("btnSearch", "Search");
        //wc.UploadValues(uri, "POST", vars);
        wc.Headers["Content-Type"] = "application/x-www-form-urlencoded";
        Uri uri = new Uri("http://www.e-home.no/metaservices/search.aspx");
        result = wc.UploadString(uri, "POST",
          //__VIEWSTATE=%2FwEPDwUKLTM3MTkwMDA5NQ9kFgICAQ9kFgICDQ88KwALAGRkg%2BhH%2F3tiaQDjnQncD1sYDdeni%2BA%3D&txtTitle=batman&btnSearch=Search&__EVENTVALIDATION=%2FwEWAwLXiqPdDAL55JyzBAKln%2FPuCgMJnDvHIVAx2tPEYdjNUbwqrR67
        string.Format("__VIEWSTATE=%2FwEPDwUKLTM3MTkwMDA5NQ9kFgICAQ9kFgICDQ88KwALAGRkg%2BhH%2F3tiaQDjnQncD1sYDdeni%2BA%3D&txtTitle={0}&btnSearch=Search&__EVENTVALIDATION=%2FwEWAwLXiqPdDAL55JyzBAKln%2FPuCgMJnDvHIVAx2tPEYdjNUbwqrR67", HttpUtility.UrlEncode(chapterInfo.Title)));
      }
      //__VIEWSTATE=%2FwEPDwUKLTM3MTkwMDA5NQ9kFgICAQ9kFgICDQ88KwALAGRkg%2BhH%2F3tiaQDjnQncD1sYDdeni%2BA%3D&txtTitle=batman&btnSearch=Search&__EVENTVALIDATION=%2FwEWAwLXiqPdDAL55JyzBAKln%2FPuCgMJnDvHIVAx2tPEYdjNUbwqrR67
              
      Tidy tidy = new Tidy(); 
      /* Set the options you want */ 
      tidy.Options.DocType = DocType.Strict; 
      //tidy.Options.DropFontTags = true; 
      tidy.Options.LogicalEmphasis = true; 
      tidy.Options.Xhtml = true; 
      tidy.Options.XmlOut = true; 
      tidy.Options.MakeClean = true; 
      tidy.Options.TidyMark = false; 
      tidy.Options.QuoteNbsp = false; 
      tidy.Options.NumEntities = true; 
      tidy.Options.CharEncoding = CharEncoding.UTF8;
      tidy.Options.FixBackslash = true;
      tidy.Options.FixComments = true;

      TidyMessageCollection tmc = new TidyMessageCollection(); 
      using (MemoryStream input = new MemoryStream())
      using (MemoryStream output = new MemoryStream())
      {
        byte[] bytes = Encoding.UTF8.GetBytes(result);
        input.Write(bytes, 0, bytes.Length);
        input.Position = 0;
        tidy.Parse(input, output, tmc);
        result = Encoding.UTF8.GetString(output.ToArray());
        if (tmc.Errors > 0) throw new Exception(
          string.Format("{0} HTML Tidy Error(s)" + Environment.NewLine, tmc.Errors)
          + string.Join(Environment.NewLine, 
          tmc.Cast<TidyMessage>()
          .Where(m => m.Level == MessageLevel.Error)
          .Select(m => m.ToString()).ToArray()));
        XNamespace ns = "http://www.w3.org/1999/xhtml";
        //parse titles
        XDocument searchXhtml = XDocument.Parse(result);
        Debug.Write(searchXhtml.Descendants(ns + "tr")
          .Where(tr => (tr.Attribute("id") != null && tr.Attribute("id").Value.Length == 17)).Count());

        var titles = searchXhtml.Descendants(ns + "tr")
          .Where(tr => (tr.Attribute("id") != null && tr.Attribute("id").Value.Length == 17))
          .Select(tr => new SearchResult()
          {
            Id = (string)tr.Attribute("id"),
            Name = (string)tr.Elements(ns + "td").First()
          });
        OnSearchComplete();
        return titles.ToList();
      }
    }
  }
}
