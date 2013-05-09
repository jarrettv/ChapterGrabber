using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Reflection;
using System.Xml;

namespace JarrettVance.ChapterTools
{
    public class ChapterInfo
    {
        public string Title { get; set; }
        public int? ChapterSetId { get; set; }
        public String ImdbId { get; set; }
        public int? MovieDbId { get; set; }
        public string Extractor { get; set; }
        public string LangCode { get; set; }
        public string VolumeName { get; set; }
        public string SourceName { get; set; }
        public string SourceType { get; set; }
        public string SourceHash { get; set; }
        public double FramesPerSecond { get; set; }
        public TimeSpan Duration { get; set; }
        public int Confirmations { get; set; }
        public List<ChapterEntry> Chapters { get; set; }

        public ChapterInfo() { }

        public bool HasNames()
        {
            if (this.Chapters.Count == 0) return false;
            string name = this.Chapters[0].Name ?? string.Empty;
            // assume either blank or "Chapter 1"
            var charCount = this.Chapters
                .Select(x => x.Name ?? string.Empty)
                .Select(x => RemoveStandardChapterNameText(x))
                .Sum(x => x.Length);

            // on average each chapter has 2 characters
            return charCount > this.Chapters.Count * 2;
        }

        public bool NamesNeedPopulated()
        {
            if (Chapters.Count == 0) return false;
            string name = Chapters[0].Name;
            // assume either blank or "Chapter 1"
            return name.ToLowerInvariant()
                .Replace("chapter", "")
                .Replace("kapitel", "")
                .Replace("capítulo", "")
                .Replace("capitulo", "")
                .Replace("chapitre", "")
                .Replace("глава", "")
                .Replace("章", "")
                .Replace("kapitola", "")
                .Replace("hoofdstuk", "")
                .Trim().Length < 3;
        }

        public float Similarity(ChapterInfo other)
        {
            float count = 0F;
            float matches = 0F;

            count++;
            if (Title != null && Title.Equals(other.Title, StringComparison.InvariantCultureIgnoreCase)) matches++;

            count++;
            if (SourceHash != null && SourceHash.Equals(other.SourceHash)) matches++;

            count++;
            if (LangCode != null && LangCode.Equals(other.LangCode)) matches++;

            count++;
            if (Math.Abs(Duration.TotalSeconds - other.Duration.TotalSeconds) < 1) matches++;

            count++;
            if (Math.Round(FramesPerSecond * 1000) - Math.Round(other.FramesPerSecond * 1000) == 0) matches++;

            for (int i = 0; i < Chapters.Count; i++)
            {
                count = count + 2F;
                if (i < other.Chapters.Count)
                {
                    if (Chapters[i].Name != null && Chapters[i].Name.Equals(other.Chapters[i].Name, StringComparison.InvariantCultureIgnoreCase)) matches++;
                    if (Math.Abs(Chapters[i].Time.TotalSeconds - other.Chapters[i].Time.TotalSeconds) < 1) matches++;
                }
            }
            return matches / count;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2} chapter(s)", SourceName, Duration.ToShortString(), Chapters.Count);
        }

        public void ChangeFps(double fps)
        {
            for (int i = 0; i < Chapters.Count; i++)
            {
                ChapterEntry c = Chapters[i];
                double frames = c.Time.TotalSeconds * FramesPerSecond;
                Chapters[i] = new ChapterEntry() { Name = c.Name, Time = new TimeSpan((long)Math.Round(frames / fps * TimeSpan.TicksPerSecond)) };
            }

            double totalFrames = Duration.TotalSeconds * FramesPerSecond;
            Duration = new TimeSpan((long)Math.Round((totalFrames / fps) * TimeSpan.TicksPerSecond));
            FramesPerSecond = fps;
        }

        public void SaveText(string filename)
        {
            List<string> lines = new List<string>();
            int i = 0;
            foreach (ChapterEntry c in Chapters)
            {
                i++;
                lines.Add("CHAPTER" + i.ToString("00") + "=" + c.Time.ToShortString());
                lines.Add("CHAPTER" + i.ToString("00") + "NAME=" + c.Name);
            }
            File.WriteAllLines(filename, lines.ToArray());
        }

        public void SaveQpfile(string filename)
        {
            List<string> lines = new List<string>();
            foreach (ChapterEntry c in Chapters)
            {
                lines.Add(string.Format("{0} I -1", (long)Math.Round(c.Time.TotalSeconds * FramesPerSecond)));
            }
            File.WriteAllLines(filename, lines.ToArray());
        }

        public void SaveCelltimes(string filename)
        {
            List<string> lines = new List<string>();
            foreach (ChapterEntry c in Chapters)
            {
                lines.Add(((long)Math.Round(c.Time.TotalSeconds * FramesPerSecond)).ToString());
            }
            File.WriteAllLines(filename, lines.ToArray());
        }

        public void SaveTsmuxerMeta(string filename)
        {
            string text = "--custom-" + Environment.NewLine + "chapters=";
            foreach (ChapterEntry c in Chapters)
            {
                text += c.Time.ToShortString() + ";";
            }
            text = text.Substring(0, text.Length - 1);
            File.WriteAllText(filename, text);
        }

        public void SaveTimecodes(string filename)
        {
            List<string> lines = new List<string>();
            foreach (ChapterEntry c in Chapters)
            {
                lines.Add(c.Time.ToShortString());
            }
            File.WriteAllLines(filename, lines.ToArray());
        }

        public static readonly XNamespace CgNs = "http://jvance.com/2008/ChapterGrabber";

        public static ChapterInfo Load(XmlReader r)
        {
            XDocument doc = XDocument.Load(r);
            return ChapterInfo.Load(doc.Root);
        }

        public static ChapterInfo Load(string filename)
        {
            XDocument doc = XDocument.Load(filename);
            return ChapterInfo.Load(doc.Root);
        }


        public static ChapterInfo Load(XElement root)
        {
            ChapterInfo ci = new ChapterInfo();
            ci.LangCode = (string)root.Attribute(XNamespace.Xml + "lang");
            ci.Extractor = (string)root.Attribute("extractor");

            if (root.Element(CgNs + "title") != null)
                ci.Title = (string)root.Element(CgNs + "title");

            XElement @ref = root.Element(CgNs + "ref");
            if (@ref != null)
            {
                ci.ChapterSetId = (int?)@ref.Element(CgNs + "chapterSetId");
                ci.ImdbId = (string)@ref.Element(CgNs + "imdbId");
                ci.MovieDbId = (int?)@ref.Element(CgNs + "movieDbId");
            }

            if (root.Attribute("confirmations") != null)
                ci.Confirmations = (int)root.Attribute("confirmations");


            XElement src = root.Element(CgNs + "source");
            if (src != null)
            {
                ci.SourceName = (string)src.Element(CgNs + "name");
                if (src.Element(CgNs + "type") != null)
                    ci.SourceType = (string)src.Element(CgNs + "type");
                ci.SourceHash = (string)src.Element(CgNs + "hash");
                ci.FramesPerSecond = Convert.ToDouble(src.Element(CgNs + "fps").Value, new System.Globalization.NumberFormatInfo());
                ci.Duration = TimeSpan.Parse(src.Element(CgNs + "duration").Value);
                if (src.Element(CgNs + "volume") != null)
                    ci.VolumeName = (string)src.Element(CgNs + "volume");
            }

            ci.Chapters = root.Element(CgNs + "chapters").Elements(CgNs + "chapter")
              .Select(e => new ChapterEntry() { Name = (string)e.Attribute("name"), Time = TimeSpan.Parse((string)e.Attribute("time")) }).ToList();
            return ci;
        }

        public void Save(string filename)
        {
            ToXElement().Save(filename);
        }

        public void Save(XmlWriter x)
        {
            ToXElement().Save(x);
        }

        public XElement ToXElement()
        {
            var reference = new XElement(CgNs + "ref");
            if (ChapterSetId.HasValue) reference.Add(new XElement(CgNs + "chapterSetId", ChapterSetId));
            if (MovieDbId.HasValue) reference.Add(new XElement(CgNs + "movieDbId", MovieDbId));
            if (ImdbId != null) reference.Add(new XElement(CgNs + "imdbId", ImdbId));

            return new XElement(new XElement(CgNs + "chapterInfo",
              new XAttribute(XNamespace.Xml + "lang", LangCode),
              new XAttribute("version", "3"),
              Extractor != null ? new XAttribute("extractor", Extractor) : null,
              Title != null ? new XElement(CgNs + "title", Title) : null,
              reference.Elements().Count() > 0 ? reference : null,
              new XElement(CgNs + "source",
                new XElement(CgNs + "name", SourceName),
                SourceType != null ? new XElement(CgNs + "type", SourceType) : null,
                VolumeName != null ? new XElement(CgNs + "volume", VolumeName) : null,
                new XElement(CgNs + "hash", SourceHash),
                new XElement(CgNs + "fps", FramesPerSecond),
                new XElement(CgNs + "duration", Duration.ToString())),
              new XElement(CgNs + "chapters",
                Chapters.Select(c =>
                  new XElement(CgNs + "chapter",
                    new XAttribute("time", c.Time.ToString()),
                    new XAttribute("name", c.Name))))));
        }

        public void SaveXml(string filename)
        {
            new XDocument(new XElement("Chapters",
              new XElement("EditionEntry",
                new XElement("EditionFlagHidden", "0"),
                new XElement("EditionFlagDefault", "0"),
                //new XElement("EditionUID", "1"),
                Chapters.Select(c =>
                  new XElement("ChapterAtom",
                  new XElement("ChapterDisplay",
                    new XElement("ChapterString", c.Name),
                    new XElement("ChapterLanguage", LangCode == null ? "und" : LangCode)),
                  new XElement("ChapterTimeStart", c.Time.ToString()),
                  new XElement("ChapterFlagHidden", "0"),
                  new XElement("ChapterFlagEnabled", "1")))
                ))).Save(filename);

            //    <Chapters>
            //<EditionEntry>
            //  <EditionFlagHidden>0</EditionFlagHidden>
            //  <EditionFlagDefault>0</EditionFlagDefault>
            //  <EditionUID>62811788</EditionUID>
            //  <ChapterAtom>
            //    <ChapterDisplay>
            //      <ChapterString>Test1</ChapterString>
            //      <ChapterLanguage>und</ChapterLanguage>
            //    </ChapterDisplay>
            //    <ChapterUID>2401693056</ChapterUID>
            //    <ChapterTimeStart>00:01:40.000000000</ChapterTimeStart>
            //    <ChapterFlagHidden>0</ChapterFlagHidden>
            //    <ChapterFlagEnabled>1</ChapterFlagEnabled>
            //  </ChapterAtom>
        }

        private static string RemoveStandardChapterNameText(string str)
        {
            return str
                .ToLowerInvariant()
                .Replace("chapter", "")
                .Replace("scene", "")
                .Replace("kapitel", "")
                .Replace("capítulo", "")
                .Replace("capitulo", "")
                .Replace("chapitre", "")
                .Replace("глава", "")
                .Replace("章", "")
                .Replace("kapitola", "")
                .Replace("hoofdstuk", "")
                .Replace("array", "")
                .Replace("{empty chapter}", "")
                .Trim()
                .RemoveSpecialCharacters()
                .RemoveNumbers();
        }
    }
}
