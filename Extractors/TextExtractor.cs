using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace JarrettVance.ChapterTools.Extractors
{
  public class TextExtractor : ChapterExtractor
  {
    public override bool SupportsMultipleStreams
    {
      get { return false; }
    }

    public override string[] Extensions
    {
      get { return new string[] { "txt" }; }
    }

    public override List<ChapterInfo> GetStreams(string location)
    {
      List<ChapterInfo> pgcs = new List<ChapterInfo>();

      List<ChapterEntry> list = new List<ChapterEntry>();

      int num = 0;
      TimeSpan ts = new TimeSpan(0);
      string time = String.Empty;
      string name = String.Empty;
      bool onTime = true;
      string[] lines = File.ReadAllLines(location);
      foreach (string line in lines)
      {
        if (onTime)
        {
          num++;
          //read time
          time = line.Replace("CHAPTER" + num.ToString("00") + "=", "");
          ts = TimeSpan.Parse(time);
        }
        else
        {
          //read name
          name = line.Replace("CHAPTER" + num.ToString("00") + "NAME=", "");
          //add it to list
          list.Add(new ChapterEntry() { Name = name, Time = ts });
        }
        onTime = !onTime;
      }

      pgcs.Add(new ChapterInfo()
      {
        Chapters = list,
        SourceName = location,
        SourceHash = ChapterExtractor.ComputeMD5Sum(location),
        FramesPerSecond = Settings.Default.DefaultFps,
        Title = Path.GetFileNameWithoutExtension(location),
        Extractor = "ChapterText"
      });

      OnStreamDetected(pgcs[0]);
      OnChaptersLoaded(pgcs[0]);
      OnExtractionComplete();
      return pgcs;
    }
  }
}
