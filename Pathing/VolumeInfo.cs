using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace JarrettVance.ChapterTools.Pathing
{
    public static class VolumeInfo
    {
        public static string GetVolumeLabel(DirectoryInfo dir)
        {
            uint serialNumber = 0;
            uint maxLength = 0;
            uint volumeFlags = new uint();
            StringBuilder volumeLabel = new StringBuilder(256);
            StringBuilder fileSystemName = new StringBuilder(256);
            string label = "";

            try
            {
                long result = GetVolumeInformation(
                    dir.Name,
                    volumeLabel,
                    (uint)volumeLabel.Capacity,
                    ref serialNumber,
                    ref maxLength,
                    ref volumeFlags,
                    fileSystemName,
                    (uint)fileSystemName.Capacity);

                label = volumeLabel.ToString();
            }
            catch { }

            if (label.Length == 0)
            {
                label = dir.Name;
            }

            return label;
        }

        [DllImport("kernel32.dll")]
        private static extern long GetVolumeInformation(
            string PathName,
            StringBuilder VolumeNameBuffer,
            uint VolumeNameSize,
            ref uint VolumeSerialNumber,
            ref uint MaximumComponentLength,
            ref uint FileSystemFlags,
            StringBuilder FileSystemNameBuffer,
            uint FileSystemNameSize);
    }
}
