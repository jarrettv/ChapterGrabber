using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using BDInfo;
using System.Windows.Forms;

namespace JarrettVance.ChapterTools.Extractors
{
  public class BDInfoExtractor : ChapterExtractor
  {
    public override string[] Extensions
    {
      get { return new string[] { "mpls" }; }
    }

    public override bool SupportsMultipleStreams
    {
      get { return false; }
    }

    public override List<ChapterInfo> GetStreams(string location)
    {
      ChapterInfo pgc = new ChapterInfo();
      pgc.Chapters = new List<ChapterEntry>();
      pgc.SourceHash = ChapterExtractor.ComputeMD5Sum(location);
      pgc.SourceName = location;
      pgc.Title = Path.GetFileNameWithoutExtension(location);
      pgc.SourceType = "Blu-Ray";
      pgc.Extractor = Application.ProductName + " " + Application.ProductVersion;

      FileInfo fileInfo = new FileInfo(location);
      
      OnStreamDetected(pgc);
      TSPlaylistFile mpls = new TSPlaylistFile(fileInfo);
      //Dictionary<string, TSStreamClipFile> clips = new Dictionary<string,TSStreamClipFile>();
      mpls.Scan();
      foreach (double d in mpls.Chapters)
      {
        pgc.Chapters.Add(new ChapterEntry()
          {
            Name = string.Empty,
            Time = new TimeSpan((long)(d * (double)TimeSpan.TicksPerSecond))
          });
      }

      pgc.Duration = new TimeSpan((long)(mpls.TotalLength * (double)TimeSpan.TicksPerSecond));

      foreach (TSStreamClip clip in mpls.StreamClips)
      {
          try
          {
              clip.StreamClipFile.Scan();
                foreach (TSStream stream in clip.StreamClipFile.Streams.Values)
                {
                  if (stream.IsVideoStream)
                  {
                    pgc.FramesPerSecond = (double)((TSVideoStream)stream).FrameRateEnumerator /
                    (double)((TSVideoStream)stream).FrameRateDenominator;
                    break;
                  }
                }
                if (pgc.FramesPerSecond != 0) break;
          }
          catch (Exception ex)
          {
              Trace.WriteLine(ex);
          }
      }

      OnChaptersLoaded(pgc);
      OnExtractionComplete();
      return new List<ChapterInfo>() { pgc };
    }
  }
}