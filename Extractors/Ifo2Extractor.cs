using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace JarrettVance.ChapterTools.Extractors
{
  public class Ifo2Extractor : ChapterExtractor
  {
    public override string[] Extensions
    {
      get { return new string[] { "ifo" }; }
    }

    public override bool SupportsMultipleStreams
    {
      get { return false; }
    }

    public override List<ChapterInfo> GetStreams(string location)
    {
      int titleSetNum = 1;

      if (location.StartsWith("VTS_"))
      {
        titleSetNum = int.Parse(Path.GetFileNameWithoutExtension(location)
         .ToUpper()
         .Replace("VTS_", string.Empty)
         .Replace("_0.IFO", string.Empty));
      }

      ChapterInfo pgc = new ChapterInfo();
      pgc.SourceType = "DVD";
      pgc.SourceName = location;
      pgc.SourceHash = ChapterExtractor.ComputeMD5Sum(location);
      pgc.Extractor = Application.ProductName + " " + Application.ProductVersion;
      pgc.Title = Path.GetFileNameWithoutExtension(location);
      OnStreamDetected(pgc);

      TimeSpan duration;
      double fps;
      pgc.Chapters = GetChapters(location, titleSetNum, out duration, out fps);
      pgc.Duration = duration;
      pgc.FramesPerSecond = fps;
      OnChaptersLoaded(pgc);
      
      OnExtractionComplete();
      return Enumerable.Repeat(pgc, 1).ToList();
    }

    List<ChapterEntry> GetChapters(string ifoFile, int programChain, out TimeSpan duration, out double fps)
    {
      List<ChapterEntry> chapters = new List<ChapterEntry>();
      duration = TimeSpan.Zero;
      fps = 0;
      
      long pcgITPosition = IfoUtil.GetPCGIP_Position(ifoFile);
      int programChainPrograms = -1;
      TimeSpan programTime = TimeSpan.Zero;
      if (programChain >= 0)
      {
        double FPS;
        uint chainOffset = IfoUtil.GetChainOffset(ifoFile, pcgITPosition, programChain);
        programTime = IfoUtil.ReadTimeSpan(ifoFile, pcgITPosition, chainOffset, out FPS) ?? TimeSpan.Zero;
        programChainPrograms = IfoUtil.GetNumberOfPrograms(ifoFile, pcgITPosition, chainOffset);
      }
      else
      {
        int programChains = IfoUtil.GetProgramChains(ifoFile, pcgITPosition);
        for (int curChain = 1; curChain <= programChains; curChain++)
        {
          double FPS;
          uint chainOffset = IfoUtil.GetChainOffset(ifoFile, pcgITPosition, curChain);
          TimeSpan? time = IfoUtil.ReadTimeSpan(ifoFile, pcgITPosition, chainOffset, out FPS);
          if (time == null)
            break;

          if (time.Value > programTime)
          {
            programChain = curChain;
            programChainPrograms = IfoUtil.GetNumberOfPrograms(ifoFile, pcgITPosition, chainOffset);
            programTime = time.Value;
          }
        }
      }
      if (programChain < 0)
        return null;

      chapters.Add(new ChapterEntry() { Name = "Chapter 1" });

      uint longestChainOffset = IfoUtil.GetChainOffset(ifoFile, pcgITPosition, programChain);
      int programMapOffset = IfoUtil.ToInt16(IfoUtil.GetFileBlock(ifoFile, (pcgITPosition + longestChainOffset) + 230, 2));
      int cellTableOffset = IfoUtil.ToInt16(IfoUtil.GetFileBlock(ifoFile, (pcgITPosition + longestChainOffset) + 0xE8, 2));
      for (int currentProgram = 0; currentProgram < programChainPrograms; ++currentProgram)
      {
        int entryCell = IfoUtil.GetFileBlock(ifoFile, ((pcgITPosition + longestChainOffset) + programMapOffset) + currentProgram, 1)[0];
        int exitCell = entryCell;
        if (currentProgram < (programChainPrograms - 1))
          exitCell = IfoUtil.GetFileBlock(ifoFile, ((pcgITPosition + longestChainOffset) + programMapOffset) + (currentProgram + 1), 1)[0] - 1;

        TimeSpan totalTime = TimeSpan.Zero;
        for (int currentCell = entryCell; currentCell <= exitCell; currentCell++)
        {
          int cellStart = cellTableOffset + ((currentCell - 1) * 0x18);
          byte[] bytes = IfoUtil.GetFileBlock(ifoFile, (pcgITPosition + longestChainOffset) + cellStart, 4);
          int cellType = bytes[0] >> 6;
          if (cellType == 0x00 || cellType == 0x01)
          {
            bytes = IfoUtil.GetFileBlock(ifoFile, ((pcgITPosition + longestChainOffset) + cellStart) + 4, 4);
            TimeSpan time = IfoUtil.ReadTimeSpan(bytes, out fps) ?? TimeSpan.Zero;
            totalTime += time;
          }
        }
        
        //add a constant amount of time for each chapter?
        //totalTime += TimeSpan.FromMilliseconds(fps != 0 ? (double)1000 / fps / 8D : 0);

        duration += totalTime;
        if (currentProgram + 1 < programChainPrograms)
          chapters.Add(new ChapterEntry() { Name = string.Format("Chapter {0}", currentProgram + 2), Time = duration });        
      }
      return chapters;
    }
  }
}
