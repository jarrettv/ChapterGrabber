using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace JarrettVance.ChapterTools.Extractors
{
  public class DvdExtractor : ChapterExtractor
  {
    public override string[] Extensions
    {
      get { return new string[] { }; }
    }

    public override List<ChapterInfo> GetStreams(string location)
    {
      string path = Path.Combine(location, "VIDEO_TS");

      if (!Directory.Exists(path))
        throw new FileNotFoundException("The VIDEO_TS folder was not found on the DVD.");

      List<ChapterInfo> streams = new List<ChapterInfo>();

      Ifo2Extractor ex = new Ifo2Extractor();
      ex.StreamDetected += (sender, args) => OnStreamDetected(args.ProgramChain);
      ex.ChaptersLoaded += (sender, args) => OnChaptersLoaded(args.ProgramChain);

      string vol = Pathing.VolumeInfo.GetVolumeLabel(new DirectoryInfo(location));

      string videoIFO = Path.Combine(path, "VIDEO_TS.IFO");
      if (File.Exists(videoIFO))
      {
        byte[] bytRead = new byte[4];
        long VMG_PTT_STPT_Position = IfoUtil.ToFilePosition(IfoUtil.GetFileBlock(videoIFO, 0xC4, 4));
        int titlePlayMaps = IfoUtil.ToInt16(IfoUtil.GetFileBlock(videoIFO, VMG_PTT_STPT_Position, 2));
        //string longestIfo = GetLongestIFO(videoTSDir);
        for (int currentTitle = 1; currentTitle <= titlePlayMaps; ++currentTitle)
        {
          long titleInfoStart = 8 + ((currentTitle - 1) * 12);
          int titleSetNumber = IfoUtil.GetFileBlock(videoIFO, (VMG_PTT_STPT_Position + titleInfoStart) + 6L, 1)[0];
          int titleSetTitleNumber = IfoUtil.GetFileBlock(videoIFO, (VMG_PTT_STPT_Position + titleInfoStart) + 7L, 1)[0];
          string vtsIFO = Path.Combine(path, string.Format("VTS_{0:D2}_0.IFO", titleSetNumber));
          if (!File.Exists(vtsIFO))
          {
            Trace.WriteLine(string.Format("VTS IFO file missing: {0}", Path.GetFileName(vtsIFO)));
            continue;
          }
          var pgc = ex.GetStreams(vtsIFO)[0];
          if (string.IsNullOrEmpty(vol)) pgc.VolumeName = vol;
          streams.Add(pgc);
        }
      }
      else
      {
        Trace.WriteLine("VIDEO_TS.IFO file is missing missing on the DVD.");
        //read all the ifo files
        foreach (string file in Directory.GetFiles(path, "VTS_*_0.IFO"))
        {
          ChapterInfo pgc = ex.GetStreams(file)[0];
          pgc.SourceName = Path.GetFileNameWithoutExtension(file);
          if (!string.IsNullOrEmpty(vol)) pgc.VolumeName = vol;
          streams.Add(pgc);
        }
      }

      streams = streams.OrderByDescending(p => p.Duration).ToList();

      OnExtractionComplete();
      return streams;
    }
  }
}
