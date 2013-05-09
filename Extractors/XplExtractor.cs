using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Windows.Forms;

namespace JarrettVance.ChapterTools.Extractors
{
  public class XplExtractor : ChapterExtractor
  {
    public override string[] Extensions
    {
      get { return new string[] { "xpl" }; }
    }

    public override List<ChapterInfo> GetStreams(string location)
    {
      List<ChapterInfo> pgcs = new List<ChapterInfo>();
      XDocument doc = XDocument.Load(location);
      XNamespace ns = "http://www.dvdforum.org/2005/HDDVDVideo/Playlist";
      foreach (XElement ts in doc.Element(ns + "Playlist").Elements(ns + "TitleSet"))
      {
        float timeBase = GetFps((string)ts.Attribute("timeBase"));
        float tickBase = GetFps((string)ts.Attribute("tickBase"));
        foreach (XElement title in ts.Elements(ns + "Title").Where(t => t.Element(ns + "ChapterList") != null))
        {
          ChapterInfo pgc = new ChapterInfo();
          List<ChapterEntry> chapters = new List<ChapterEntry>();
          pgc.SourceName = location;
          pgc.SourceHash = ChapterExtractor.ComputeMD5Sum(location);
          pgc.SourceType = "HD-DVD";
          pgc.FramesPerSecond = 24D;
          pgc.Extractor = Application.ProductName + " " + Application.ProductVersion;
          OnStreamDetected(pgc);

          int tickBaseDivisor = (int?)title.Attribute("tickBaseDivisor") ?? 1;
          pgc.Duration = GetTimeSpan((string)title.Attribute("titleDuration"), timeBase, tickBase, tickBaseDivisor);
          string titleName = Path.GetFileNameWithoutExtension(location);
          if (title.Attribute("id") != null) titleName = (string)title.Attribute("id");
          if (title.Attribute("displayName") != null) titleName = (string)title.Attribute("displayName");
          pgc.Title = titleName;
          foreach (XElement chapter in title.Element(ns + "ChapterList").Elements(ns + "Chapter"))
          {
            chapters.Add(new ChapterEntry()
            {
              Name = (string)chapter.Attribute("displayName"),
              Time = GetTimeSpan((string)chapter.Attribute("titleTimeBegin"), timeBase, tickBase, tickBaseDivisor)
            });
          }
          pgc.Chapters = chapters;
          OnChaptersLoaded(pgc);
          //pgc.ChangeFps(24D / 1.001D);
          pgcs.Add(pgc);
        }
      }
      pgcs = pgcs.OrderByDescending(p => p.Duration).ToList();
      OnExtractionComplete();
      return pgcs;
    }
    protected static float GetFps(string fps)
    {
      fps = fps.Replace("fps", string.Empty);
      return Convert.ToSingle(fps, new System.Globalization.NumberFormatInfo());
    }

    protected static TimeSpan GetTimeSpan(string timeSpan, float timeBase, float tickBase, int tickBaseDivisor)
    {
      TimeSpan ts = TimeSpan.Parse(timeSpan.Substring(0, timeSpan.LastIndexOf(':')));
      ts = new TimeSpan((long)((ts.TotalSeconds / 60D) * (double)timeBase) * TimeSpan.TicksPerSecond);

      //convert ticks to ticks timebase
      //NOTE: eac3to does not utilize tickBaseDivisor (they have bug?)
      decimal convert = (decimal)TimeSpan.TicksPerSecond / ((decimal)tickBase / (decimal)tickBaseDivisor);
      decimal ticks = decimal.Parse(timeSpan.Substring(timeSpan.LastIndexOf(':') + 1)) * convert;
      return ts.Add(new TimeSpan((long)ticks));
    }
  }
}
