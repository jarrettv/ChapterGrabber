using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using NDepend.Helpers.FileDirectoryPath;
using System.Diagnostics;

namespace JarrettVance.ChapterTools.Pathing
{
    public static class FileNameUtils
    {
        public static string MakeValidFileName(string name)
        {
            name = name.Replace(":", " -");
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidReStr = string.Format(@"[{0}]+", invalidChars);
            return Regex.Replace(name, invalidReStr, "_");
        }

        public static string GetClosestExistingDirectory(string dir)
        {
            if (string.IsNullOrEmpty(dir)) return null;

            try
            {
                DirectoryPathAbsolute last = new DirectoryPathAbsolute(dir);
                if (last.Exists)
                    return last.Path;
                else if (last.ParentDirectoryPath.Exists)
                    return last.ParentDirectoryPath.Path;
                else if (last.ParentDirectoryPath.ParentDirectoryPath.Exists)
                    return last.ParentDirectoryPath.ParentDirectoryPath.Path;
            }
            catch (Exception ex)
            {
                Trace.Write(ex);
            }

            return null;
        }
    }
}
