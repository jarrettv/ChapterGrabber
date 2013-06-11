using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace JarrettVance.ChapterTools
{
    public class FontHelper
    {
        private static PrivateFontCollection sCustomFonts;
        private static Dictionary<string, FontFamily> sFamilies;

        static FontHelper()
        {
            sCustomFonts = new PrivateFontCollection();
            sFamilies = new Dictionary<string, FontFamily>(
                StringComparer.InvariantCultureIgnoreCase);
        }

        public static void RegisterFont(byte[] font)
        {
            var buffer = Marshal.AllocCoTaskMem(font.Length);
            Marshal.Copy(font, 0, buffer, font.Length);
            sCustomFonts.AddMemoryFont(buffer, font.Length);
        }

        public static void RegisterFont(string fontFilePath)
        {
            sCustomFonts.AddFontFile(fontFilePath);
        }

        public static FontFamily[] GetAllFamilies()
        {
            var families = new List<FontFamily>();

            families.AddRange(FontFamily.Families);
            families.AddRange(sCustomFonts.Families);

            families.Sort((f1, f2) => { return f1.Name.CompareTo(f2.Name); });
            return families.ToArray();
        }

        public static FontFamily GetFamily(string family)
        {
            try
            {
                // cache the family in a dictionary for fast lookup.
                if (!sFamilies.ContainsKey(family))
                    sFamilies.Add(family, new FontFamily(family, sCustomFonts));
            }
            catch (ArgumentException)
            {
                sFamilies.Add(family, new FontFamily(family));
            }
            return sFamilies[family];
        }

        public static Font Create(
            string family,
            float emSize,
            FontStyle style = FontStyle.Regular,
            GraphicsUnit unit = GraphicsUnit.Pixel)
        {
            var fam = GetFamily(family);
            return new Font(family, emSize, style, unit);
        }

    }
}
