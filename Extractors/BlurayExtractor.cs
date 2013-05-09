using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace JarrettVance.ChapterTools.Extractors
{
    public class BlurayExtractor : ChapterExtractor
    {
        public override string[] Extensions
        {
            get { return new string[] { }; }
        }

        public override List<ChapterInfo> GetStreams(string location)
        {
            List<ChapterInfo> pgcs = new List<ChapterInfo>();
            string path = Path.Combine(Path.Combine(location, "BDMV"), "PLAYLIST");
            if (!Directory.Exists(path))
                throw new FileNotFoundException("Could not find PLAYLIST folder on BluRay disc.");

            ChapterExtractor ex = new BDInfoExtractor();
            ex.StreamDetected += (sender, args) => OnStreamDetected(args.ProgramChain);
            ex.ChaptersLoaded += (sender, args) => OnChaptersLoaded(args.ProgramChain);

            string vol = Pathing.VolumeInfo.GetVolumeLabel(new DirectoryInfo(location));
            foreach (string file in Directory.GetFiles(path, "*.mpls"))
            {
                var pgc = ex.GetStreams(file)[0];
                if (!string.IsNullOrEmpty(vol)) pgc.VolumeName = vol;
                pgcs.Add(pgc);
            }

            pgcs = pgcs.OrderByDescending(p => p.Duration).ToList();

            OnExtractionComplete();
            return pgcs;
        }
    }
}
