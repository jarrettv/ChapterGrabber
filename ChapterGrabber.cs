using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JarrettVance.ChapterTools
{
  public abstract class ChapterGrabber
  {
      public static List<ChapterGrabber> Grabbers = new List<ChapterGrabber> { new Grabbers.TagChimpGrabber(), new Grabbers.DatabaseGrabber() };

    public abstract void PopulateNames(SearchResult result, ChapterInfo chapterInfo, bool includeDurations);
    public abstract void PopulateNames(string hash, ChapterInfo chapterInfo);
    public abstract List<SearchResult> Search(ChapterInfo chapterInfo);
    public abstract void Upload(ChapterInfo chapterInfo);

    public bool SupportsHash { get; set; }
    public bool SupportsUpload { get; set; }

    public event EventHandler SearchComplete;

    protected void OnSearchComplete()
    {
      if (SearchComplete != null) SearchComplete(this, EventArgs.Empty);
    }
  }
  
  public class SearchResult
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public int Relevance { get; set; }
    public TimeSpan Duration { get; set; }
    public int Count { get; set; }
    public string Type { get; set; }
    public bool? HasNames { get; set; }
  }
}
