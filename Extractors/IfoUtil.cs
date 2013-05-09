using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace JarrettVance.ChapterTools
{
    public static class IfoUtil
    {
        public static byte[] GetFileBlock(string strFile, long pos, int count)
        {
            using (FileStream stream = new FileStream(strFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] buf = new byte[count];
                stream.Seek(pos, SeekOrigin.Begin);
                if (stream.Read(buf, 0, count) != count)
                    return buf;
                return buf;
            }
        }
        public static short ToInt16(byte[] bytes) { return (short)((bytes[0] << 8) + bytes[1]); }
        public static uint ToInt32(byte[] bytes) { return (uint)((bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + bytes[3]); }
        public static short ToShort(byte[] bytes) { return ToInt16(bytes); }
        public static long ToFilePosition(byte[] bytes) { return ToInt32(bytes) * 0x800L; }
        public static long GetTotalFrames(TimeSpan time, int fps)
        {
            return (long)Math.Round(fps * time.TotalSeconds);
        }
        
        static string TwoLong(int val) { return string.Format("{0:D2}", val); }

        static int AsHex(int val)
        {
            int ret;
            int.TryParse(string.Format("{0:X2}", val), out ret);
            return ret;
        }

        internal static short? GetFrames(byte val)
        {
            int byte0_high = val >> 4;
            int byte0_low = val & 0x0F;
            if (byte0_high > 11)
                return (short)(((byte0_high - 12) * 10) + byte0_low);
            if ((byte0_high <= 3) || (byte0_high >= 8))
                return null;
            return (short)(((byte0_high - 4) * 10) + byte0_low);
        }

        internal static int GetFrames(TimeSpan time, int fps)
        {
            return (int)Math.Round(fps * time.Milliseconds / 1000.0);
        }
        internal static long GetPCGIP_Position(string ifoFile)
        {
            return ToFilePosition(GetFileBlock(ifoFile, 0xCC, 4));
        }

        internal static int GetProgramChains(string ifoFile, long pcgitPosition)
        {
            return ToInt16(GetFileBlock(ifoFile, pcgitPosition, 2));
        }

        internal static uint GetChainOffset(string ifoFile, long pcgitPosition, int programChain)
        {
            return ToInt32(GetFileBlock(ifoFile, (pcgitPosition + (8 * programChain)) + 4, 4));
        }

        internal static int GetNumberOfPrograms(string ifoFile, long pcgitPosition, uint chainOffset)
        {
            return GetFileBlock(ifoFile, (pcgitPosition + chainOffset) + 2, 1)[0];
        }

        internal static TimeSpan? ReadTimeSpan(string ifoFile, long pcgitPosition, uint chainOffset, out double fps)
        {
            return ReadTimeSpan(GetFileBlock(ifoFile, (pcgitPosition + chainOffset) + 4, 4), out fps);
        }

        internal static TimeSpan? ReadTimeSpan(byte[] playbackBytes, out double fps)
        {
            short? frames = GetFrames(playbackBytes[3]);
            int fpsMask = playbackBytes[3] >> 6;
            fps = fpsMask == 0x01 ? 25D : fpsMask == 0x03 ? (30D / 1.001D): 0;
            if (frames == null)
                return null;

            try
            {
                int hours = AsHex(playbackBytes[0]);
                int minutes = AsHex(playbackBytes[1]);
                int seconds = AsHex(playbackBytes[2]);
                TimeSpan ret = new TimeSpan(hours, minutes, seconds);
                if (fps != 0)
                    ret = ret.Add(TimeSpan.FromSeconds((double)frames / fps));
                return ret;
            }
            catch { return null; }
        }
   }
}
