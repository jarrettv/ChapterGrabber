using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace JarrettVance.ChapterTools.Extractors
{
  public class Eac3toExtractor : ChapterExtractor
  {

    public override string[] Extensions
    {
      get { return new string[0]; }
    }

    public override List<ChapterInfo> GetStreams(string location)
    {
      List<ChapterInfo> pgcs = new List<ChapterInfo>();

      ProcessStartInfo psi = new ProcessStartInfo(@"d:\programs\eac3to\eac3to.exe", location);
      psi.CreateNoWindow = true;
      psi.UseShellExecute = false;
      //psi.RedirectStandardError = true;
      psi.RedirectStandardOutput = true;
      psi.WorkingDirectory = Application.StartupPath;
      Process p = Process.Start(psi);
      string output = p.StandardOutput.ReadToEnd();
      p.WaitForExit();
      
      foreach (Match m in Regex.Matches(output, @"\d\).+:\d\d"))
      {
        string[] data = m.Value.Split(',');

        string sourceFile = Path.Combine(Path.Combine(Path.Combine(
          location, "BDMV"), "PLAYLIST"), data[0].Split(')')[1].Trim());

        ChapterInfo pgc = new ChapterInfo()
        {
          Duration = TimeSpan.Parse(data[data.Length - 1].Trim()),
          SourceName = data[0].Split(')')[0],
          SourceHash = ChapterExtractor.ComputeMD5Sum(sourceFile)
        };
        OnStreamDetected(pgc);
        pgcs.Add(pgc);
      }
      /*
1) 00001.mpls, 00002.m2ts, 1:34:15
   - h264/AVC, 1080p24 /1.001 (16:9)
   - AC3, Spanish, multi-channel, 48khz
   - DTS Master Audio, English, multi-channel, 48khz

2) 00027.mpls, 00036.m2ts, 0:24:19
   - h264/AVC, 1080p24 /1.001 (16:9)
   - AC3, English, stereo, 48khz
       */

      foreach (ChapterInfo pgc in pgcs)
      {
        psi.Arguments = location + " " + pgc.SourceName + ")";
        p = Process.Start(psi);
        output = p.StandardOutput.ReadToEnd();
        p.WaitForExit();
        if (output.Contains("Chapters"))
        {
          if (File.Exists("chapters.txt")) File.Delete("chapters.txt");
          psi.Arguments = location + " " + pgc.SourceName + ") chapters.txt";
          p = Process.Start(psi);
          output = p.StandardOutput.ReadToEnd();
          p.WaitForExit();
          if (!output.Contains("Creating file \"chapters.txt\"...") && !output.Contains("Done!"))
          {
            throw new Exception("Error creating chapters file.");
          }
          TextExtractor extractor = new TextExtractor();
          pgc.Chapters = extractor.GetStreams("chapters.txt")[0].Chapters;
          OnChaptersLoaded(pgc);
        }
      }

      /*
M2TS, 1 video track, 2 audio tracks, 6 subtitle tracks, 1:34:15
1: Chapters, 25 chapters
2: h264/AVC, 1080p24 /1.001 (16:9)
3: AC3, Spanish, 5.1 channels, 448kbps, 48khz, dialnorm: -27dB
4: DTS Master Audio, English, 5.1 channels, 24 bits, 48khz
   (core: DTS, 5.1 channels, 24 bits, 1509kbps, 48khz)
5: Subtitle (PGS), English
6: Subtitle (PGS), Spanish
7: Subtitle (PGS), Spanish
8: Subtitle (PGS), French
9: Subtitle (PGS), Chinese
10: Subtitle (PGS), Korean
       */

      OnExtractionComplete();
      return pgcs;
    }
  }
}
