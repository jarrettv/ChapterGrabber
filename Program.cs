using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace JarrettVance.ChapterTools
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // register file association
            Register(".chapters", "application/xml+chapters", "Video Chapters", Application.ExecutablePath, Application.ExecutablePath, 0);

            Updater.UpdateUpdater();

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            Application.EnableVisualStyles();
            var f = new MainForm();
            if (args.Length > 0) f.StartupFile = args[0];
            Application.Run(f);
        }

        /// <summary>
        /// Registers a file type via it's extension. If the file type is already registered, nothing is changed.
        /// </summary>
        /// <param name="extension">The extension to register</param>
        /// <param name="progId">A unique identifier for the program to work with the file type</param>
        /// <param name="description">A brief description of the file type</param>
        /// <param name="executeable">Where to find the executeable.</param>
        /// <param name="iconFile">Location of the icon.</param>
        /// <param name="iconIdx">Selects the icon within <paramref name="iconFile"/></param>
        public static void Register(string extension, string progId, string description, string executeable, string iconFile, int iconIdx)
        {
            try
            {
                if (extension.Length != 0)
                {
                    if (extension[0] != '.')
                    {
                        extension = "."+extension;
                    }

                    // register the extension, if necessary
                    using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(extension))
                    {
                        if (key == null)
                        {
                            using (RegistryKey extKey = Registry.ClassesRoot.CreateSubKey(extension))
                            {
                                extKey.SetValue(string.Empty, progId);
                            }
                        }
                    }

                    // register the progId, if necessary
                    using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(progId))
                    {
                        if (key == null)
                        {
                            using (RegistryKey progIdKey = Registry.ClassesRoot.CreateSubKey(progId))
                            {
                                progIdKey.SetValue(string.Empty, description);
                                using (RegistryKey defaultIcon = progIdKey.CreateSubKey("DefaultIcon"))
                                {
                                    defaultIcon.SetValue(string.Empty, String.Format("\"{0}\",{1}", iconFile, iconIdx));
                                }

                                using (RegistryKey command = progIdKey.CreateSubKey("shell\\open\\command"))
                                {
                                    command.SetValue(string.Empty, String.Format("\"{0}\" \"%1\"", executeable));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

    }
}
