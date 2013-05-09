/********************************************************************************
*
*    Copyright(C) 2003-2008 Jarrett Vance http://jvance.com
*
*    This file is part of ChapterGrabber
*
*	 ChapterGrabber is free software; you can redistribute it and/or modify
*    it under the terms of the GNU General Public License as published by
*    the Free Software Foundation; either version 2 of the License, or
*    (at your option) any later version.
*
*    ChapterGrabber is distributed in the hope that it will be useful,
*    but WITHOUT ANY WARRANTY; without even the implied warranty of
*    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*    GNU General Public License for more details.
*
*    You should have received a copy of the GNU General Public License
*    along with this program; if not, write to the Free Software
*    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*
********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using BDInfo;
using JarrettVance.ChapterTools.Extractors;
using JarrettVance.ChapterTools.Grabbers;
using NDepend.Helpers.FileDirectoryPath;

namespace JarrettVance.ChapterTools
{
  /// <summary>
  /// Summary description for frmMain.
  /// </summary>
  public partial class MainForm : System.Windows.Forms.Form
  {
    public string StartupFile { get; set; }

    private ChapterGrabber grabber = null;
    private ChapterInfo pgc;
    private int intIndex;
    private volatile bool dbWait, titleWait = false;


    private void frmMain_Load(object sender, System.EventArgs e)
    {
      this.Height = Math.Min(560, Screen.GetWorkingArea(this).Height - 30);
      this.listChapters.Columns.Add("Time", 80, HorizontalAlignment.Left);
      this.listChapters.Columns.Add("Name", 190, HorizontalAlignment.Left);
      //this.listChapters.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);

      try
      {
        //when using new version, upgrade settings
        var nfi = new NumberFormatInfo();
        object lastConfigVersion = Settings.Default.GetPreviousVersion("ConfigVersion");
        if (lastConfigVersion != null && Convert.ToDouble(lastConfigVersion.ToString(), nfi) <
          Convert.ToDouble(Settings.Default.ConfigVersion, nfi))
        {
          Trace.WriteLine("Upgrading settings from previous version.");
          Settings.Default.Upgrade();
          Settings.Default.Save();
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex);
      }

      if (Settings.Default.RecentFiles == null)
        Settings.Default.RecentFiles = new StringCollection();

      miIgnoreShortLastChapter.Checked = Settings.Default.IgnoreShortLastChapter;
      miImportDurations.Checked = Settings.Default.ImportDurations;
      miAutoCheck.Checked = Settings.Default.AutoCheckForUpdate;
      miAutoUseDb.Checked = Settings.Default.AutoUseDatabase;

      ContextMenu m = new ContextMenu();
      m.MenuItems.Add(new MenuItem("TagChimp by Title",
        (s, args) => { grabber = new TagChimpGrabber(); Search(); }));
      //m.MenuItems.Add(new MenuItem("MetaService",
      //  (s, args) => {grabber = new MetaServiceGrabber(); Search();}));
      m.MenuItems.Add(new MenuItem("ChapterDB by Title",
        (s, args) => { grabber = new DatabaseGrabber(); Search(); }));
      btnSearch.ContextMenu = m;

      LoadFpsMenus();
      LoadLangMenu();

      IList<MenuItem> menu = new List<MenuItem>();
      foreach (MenuItem mi in menuCurrentFps.MenuItems)
      {
        menu.Add(mi);
      }
      tsslFps.DropDownItems.AddRange(
        menu.Select(mi => new ToolStripMenuItem(mi.Text, null, (s, args) => CurrentFps((double)((ToolStripMenuItem)s).Tag)) { Tag = mi.Tag }).ToArray());

      menu = new List<MenuItem>();
      foreach (MenuItem mi in menuLang.MenuItems)
      {
        menu.Add(mi);
      }
      tsslLang.DropDownItems.AddRange(
        menu.Where(mi => mi.Text != "-").Select(mi => new ToolStripMenuItem(mi.Text, null,
          (s, args) => ChangeLang(((ToolStripMenuItem)s).Tag as string)) { Tag = mi.Tag }).ToArray());
      RefreshRecent();
      OnNew();

//#if RELEASE
      if (Settings.Default.AutoUseDatabase && 
          (Settings.Default.DatabaseApiKey == "a784c7d08e5fe192ca247d1a2dd5c27f" || string.IsNullOrEmpty(Settings.Default.DatabaseApiKey)))
      {
          MessageBox.Show(this, "Please enter your API key to access the database.");
          miDatabaseCredentials_Click(null, null);
      }
//#endif

      if (!string.IsNullOrEmpty(StartupFile)) OpenFile(StartupFile);

      if (Settings.Default.AutoCheckForUpdate)
          ThreadPool.QueueUserWorkItem((w) => Updater.CheckForUpdate(ShowUpdateDialog));
    }

    void ShowUpdateDialog(Version appVersion, Version newVersion, XDocument doc)
    {
      if (InvokeRequired)
      {
        Invoke(new Action<Version, Version, XDocument>(ShowUpdateDialog), appVersion, newVersion, doc);
        return;
      }

      using (UpdateForm f = new UpdateForm())
      {
        f.Text = string.Format(f.Text, appVersion);
        f.MoreInfoLink = (string)doc.Root.Element("info");
        f.Info = string.Format(f.Info, newVersion, (DateTime)doc.Root.Element("date"));
        if (f.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
        {
          Updater.LaunchUpdater(doc);
          this.Close();
        }
      }
    }

    void LoadFpsMenus()
    {
      menuChangeFps.MenuItems.Clear();
      var nfi = new NumberFormatInfo();

      foreach (string val in Settings.Default.FpsValues)
      {
        double fps = val.Contains('/') ? Convert.ToDouble(val.Split('/')[0], nfi) /
          Convert.ToDouble(val.Split('/')[1], nfi) : Convert.ToDouble(val, nfi);
        menuChangeFps.MenuItems.Add(new MenuItem("to " + fps.ToString("0.000"),
          (s, args) => ChangeFps((double)((MenuItem)s).Tag)) { Tag = fps, Checked = false });
      }
      menuCurrentFps.MenuItems.Clear();
      foreach (string val in Settings.Default.FpsValues)
      {
        double fps = val.Contains('/') ? Convert.ToDouble(val.Split('/')[0], nfi) /
          Convert.ToDouble(val.Split('/')[1], nfi) : Convert.ToDouble(val, nfi);
        menuCurrentFps.MenuItems.Add(new MenuItem(fps.ToString("0.000"),
          (s, args) => CurrentFps((double)((MenuItem)s).Tag)) { Tag = fps, RadioCheck = true });
      }
    }

    void LoadLangMenu()
    {
      menuLang.MenuItems.Clear();
      foreach (KeyValuePair<string, string> lang in LanguageCodes.GetLanguages()
        .Where(kvp => Settings.Default.LangValues.Contains(kvp.Key)).Concat(
        LanguageCodes.GetLanguages().Where(u => u.Key == "und")))
      {
        menuLang.MenuItems.Add(new MenuItem(lang.Key + " (" + lang.Value + ")",
          (s, args) => ChangeLang(((MenuItem)s).Tag as string))
          {
            Tag = lang.Key,
            RadioCheck = true,
            Checked = lang.Key == (pgc == null ? Settings.Default.DefaultLangCode : pgc.LangCode)
          });
      }
      menuLang.MenuItems.Add(menuLang.MenuItems.Count - 1, new MenuItem("-"));
    }

    void ChangeLang(string langCode)
    {
      pgc.LangCode = langCode;
      tsslLang.Text = langCode + " (" + LanguageCodes.GetName(langCode) + ")";
      LoadLangMenu(); //refresh list & selection
    }


    void ChangeFps(double fps)
    {
      pgc.ChangeFps(fps);
      FreshChapterView();
    }

    void CurrentFps(double fps)
    {
      pgc.FramesPerSecond = fps;
      FreshChapterView();
    }

    private void menuFileSave_Click(object sender, System.EventArgs e)
    {
        UpdateDatabase(pgc);

      saveFileDialog.Filter = 
        "Chapters File (*.chapters)|*.chapters|" + 
        "Chapters Text File (*.txt)|*.txt|" + 
        "Matroska Chapter XML File (*.xml)|*.xml|" + 
        "TsMuxeR Meta Text File (*.txt)|*.txt|" +
        "Celltimes Text File (*.txt)|*.txt|" +
        "x264 QP File (*.txt)|*.txt|" +
        "Timecodes Text File (*.txt)|*.txt|" +
        "All Files (*.*)|*.*";

      saveFileDialog.RestoreDirectory = true;
      saveFileDialog.FileName = (pgc.Title == null ? pgc.SourceName : pgc.Title)
        + Settings.Default.LastSaveExt;

      //Note: filter index is one based (dumb)
      switch (Settings.Default.LastSaveExt)
      {
        case ".chapters": saveFileDialog.FilterIndex = 1; break;
        case ".txt": saveFileDialog.FilterIndex = 2; break;
        case ".xml": saveFileDialog.FilterIndex = 3; break;
        default: saveFileDialog.FilterIndex = 8; break;
      }

      if (saveFileDialog.ShowDialog() == DialogResult.OK)
      {
        string ext = Path.GetExtension(saveFileDialog.FileName).ToLower();
        if (ext == ".xml")
          pgc.SaveXml(saveFileDialog.FileName);
        else if (ext == ".txt")
          switch (saveFileDialog.FilterIndex)
          {
            case 4: pgc.SaveTsmuxerMeta(saveFileDialog.FileName); break;
            case 5: pgc.SaveCelltimes(saveFileDialog.FileName); break;
            case 6: pgc.SaveQpfile(saveFileDialog.FileName); break;
            case 7: pgc.SaveTimecodes(saveFileDialog.FileName); break;
            default: pgc.SaveText(saveFileDialog.FileName); break;
          }
        else
          pgc.Save(saveFileDialog.FileName);
        Settings.Default.LastSaveExt = ext;
        Settings.Default.Save();
      }
    }

    private void UpdateDatabase(ChapterInfo pgc)
    {
        picDb.Visible = true;
        dbWait = true;
        //duplicate
        ChapterInfo ci = ChapterInfo.Load(pgc.ToXElement());
        ThreadPool.QueueUserWorkItem((w) =>
            {

                // look for direct hits on the names and auto-load them
                if (Settings.Default.AutoUseDatabase && pgc.SourceHash != null)
                {
                    foreach (ChapterGrabber g in ChapterGrabber.Grabbers)
                        if (g.SupportsUpload) g.Upload(ci);
                }
                dbWait = false;
                Invoke(new Action(() => picDb.Visible = titleWait || dbWait));
            });
    }

    private void menuFileExit_Click(object sender, System.EventArgs e)
    {
      this.Close();
    }

    private void frmMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      Settings.Default.Save();
    }

    private void menuFileNew_Click(object sender, System.EventArgs e)
    {
      OnNew();
    }

    void OnNew()
    {
        txtTitle.Text = String.Empty;
        flowResults.Controls.Clear();
        menuTitles.Items.Clear();
      intIndex = 0;
      pgc = new ChapterInfo()
      {
        Chapters = new List<ChapterEntry>(),
        FramesPerSecond = Settings.Default.DefaultFps,
        LangCode = Settings.Default.DefaultLangCode
      };
      FreshChapterView();
    }

    private void menuEditClipboardImport_Click(object sender, System.EventArgs e)
    {
      try
      {
        if (pgc.Chapters.Count == 0)
          throw new Exception("There are no chapters to import names into.");

        //get clipboard
        IDataObject iData = Clipboard.GetDataObject();
        if (!iData.GetDataPresent(DataFormats.Text))
          throw new Exception("There is no valid text to copy from the clipboard.");

        Grabber.ImportFromClipboard(pgc.Chapters, (String)iData.GetData(DataFormats.Text) + "\n", this.miImportDurations.Checked);

        FreshChapterView();
        tsslStatus.Text = "Names successfully imported from clipboard.";
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error");
        Trace.WriteLine(ex);
        tsslStatus.Text = "Error importing names from clipboad.";
      }
    }

    void AddToRecent(string file)
    {
      StringCollection files = Settings.Default.RecentFiles;

      //clear existing entries of this file
      while (files.IndexOf(file) >= 0) files.RemoveAt(files.IndexOf(file));

      //insert at beginning
      files.Insert(0, file);

      //truncate pass max entries
      while (files.Count > Settings.Default.MaxRecentFiles)
        files.RemoveAt(Settings.Default.MaxRecentFiles);

      Settings.Default.RecentFiles = files; //?
      RefreshRecent();
    }

    void RefreshRecent()
    {
      menuRecentFiles.MenuItems.Clear();
      foreach (string f in Settings.Default.RecentFiles)
      {
        menuRecentFiles.MenuItems.Add(new MenuItem(Path.GetFileName(f),
          (s, args) => OpenFile(((MenuItem)s).Tag as string)) { Tag = f });
      }
    }

    private void menuFileOpen_Click(object sender, System.EventArgs e)
    {
      openFileDialog.Filter = "Supported Files |*.ifo;*.xpl;*.mpls;*.txt;*.xml;*.chapters|DVD IFO Files (*.ifo)|*.ifo|HD-DVD Xml Playlist Files (*.xpl)|*.xpl|Blu-Ray Playlist Files (*.mpls)|*.mpls|Chapters Text Files (*.txt)|*.txt|Matroska XML Files (*.xml)|*.xml|Chapters Files (*.chapters)|*.chapters|All files (*.*)|*.*";
      openFileDialog.FilterIndex = 0;
      openFileDialog.AutoUpgradeEnabled = true;

        var initDir = Pathing.FileNameUtils.GetClosestExistingDirectory(Settings.Default.LastOpenDir);
      if (initDir != null)
      {
          openFileDialog.InitialDirectory = initDir;
      }
      openFileDialog.RestoreDirectory = true;

      if (openFileDialog.ShowDialog() == DialogResult.OK)
      {
        Settings.Default.LastOpenDir = new DirectoryPathAbsolute(openFileDialog.FileName).Path;
        Settings.Default.Save();
        OpenFile(openFileDialog.FileName);
      }
    }

    public static List<ChapterInfo> ReadPgcListFromFile(string file)
    {
        ChapterExtractor ex = null;
        string fileLower = file.ToLower();
        if (fileLower.EndsWith("txt"))
            ex = new TextExtractor();
        else if (fileLower.EndsWith("xpl"))
            ex = new XplExtractor();
        else if (fileLower.EndsWith("ifo"))
            ex = new Ifo2Extractor();
        else if (fileLower.EndsWith("mpls"))
            ex = new MplsExtractor();
        else if (fileLower.EndsWith("xml"))
            throw new Exception("Format not yet supported.");
        else if (fileLower.EndsWith("chapters"))
        {
            List<ChapterInfo> ret = new List<ChapterInfo>();
            ret.Add(ChapterInfo.Load(file));
            return ret;
        }
        else
        {
            throw new Exception("The selected file is not a recognized format.");
        }

        return ex.GetStreams(file);
    }

    public void OpenFile(string file)
    {
      Cursor = Cursors.WaitCursor;
      tsslStatus.Text = "Parsing chapters from file...";
      try
      {
        List<ChapterInfo> temp = ReadPgcListFromFile(file);
        pgc = temp[0];
        if (pgc.FramesPerSecond == 0) pgc.FramesPerSecond = Settings.Default.DefaultFps;
        if (pgc.LangCode == null) pgc.LangCode = Settings.Default.DefaultLangCode;
        AutoLoadNames();

        FreshChapterView();

        AddToRecent(file);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
        Trace.WriteLine(ex);
        tsslStatus.Text = "Failed to load chapters from file.";
      }
      finally
      {
        Cursor = Cursors.Default;
      }
    }

    private void AutoLoadNames()
    {
        dbWait = false;
        picDb.Visible = true;
        ThreadPool.QueueUserWorkItem((w) =>
            {
                // look for direct hit by hash and auto-load names if needed
                if (Settings.Default.AutoUseDatabase && pgc.SourceHash != null)
                {
                    // only auto-populate if names are missing
                    if (pgc.NamesNeedPopulated())
                    {
                        foreach (ChapterGrabber g in ChapterGrabber.Grabbers)
                            if (g.SupportsHash)
                            {
                                g.PopulateNames(pgc.SourceHash, pgc);
                                FreshChapterView();
                            }
                    }
                }
                dbWait = false;
                Invoke(new Action(() => picDb.Visible = titleWait || dbWait));
            });
    }

    private void FreshChapterView()
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(FreshChapterView));
            return;
        }

      this.Cursor = Cursors.WaitCursor;
      try
      {
        listChapters.Items.Clear();
        txtTitle.Text = pgc.Title;
        //fill list
        foreach (ChapterEntry c in pgc.Chapters)
        {
          //don't show short last chapter depending on settings
          if (pgc.Duration != TimeSpan.Zero && miIgnoreShortLastChapter.Checked &&
            c.Equals(pgc.Chapters.Last()) && (pgc.Duration.Add(-c.Time).TotalSeconds <
            Settings.Default.ShortChapterSeconds))
            continue;

          listChapters.Items.Add(new ListViewItem(new string[] { c.Time.ToShortString(), c.Name }));
        }
        if (intIndex < listChapters.Items.Count && listChapters.Items[intIndex] != null)
        {
          //select it
          listChapters.Items[intIndex].Selected = true;
          //scroll to it
          listChapters.Items[intIndex].EnsureVisible();
        }

        //update status
        tsslDuration.Text = "Duration: " + pgc.Duration.ToShortString();
        tsslFps.Text = pgc.FramesPerSecond.ToString("0.000") + "fps";
        tsslLang.Text = pgc.LangCode + " (" + LanguageCodes.GetName(pgc.LangCode) + ")";
        tsslStatus.Text = "Chapters loaded.";

        //check the current fps
        foreach (MenuItem mi in menuCurrentFps.MenuItems)
        {
          double fps = Convert.ToDouble(mi.Text, new NumberFormatInfo());
          mi.Checked = (Math.Round(fps, 3) == Math.Round(pgc.FramesPerSecond, 3));
        }
      }
      catch (Exception ex) { Trace.WriteLine(ex); }
      finally { this.Cursor = Cursors.Default; }
    }


    private void menuEditTimesImport_Click(object sender, System.EventArgs e)
    {
      miImportDurations.Checked = !miImportDurations.Checked;
      Settings.Default.ImportDurations = miImportDurations.Checked;
    }

    private void listChapters_SelectedIndexChanged(object sender, System.EventArgs e)
    {
      if (listChapters.Items.Count < 1) return;

      txtChapterName.TextChanged -= new System.EventHandler(this.txtChapter_TextChanged);
      txtChapterTime.TextChanged -= new System.EventHandler(this.txtChapter_TextChanged);
      ListView lv = (ListView)sender;

      if (lv.SelectedItems.Count == 1) intIndex = lv.SelectedItems[0].Index;
      if (pgc.Chapters.Count > 0)
      {
        txtChapterTime.Text = pgc.Chapters[intIndex].Time.ToShortString();
        txtChapterName.Text = pgc.Chapters[intIndex].Name;
      }

      txtChapterName.TextChanged += new System.EventHandler(this.txtChapter_TextChanged);
      txtChapterTime.TextChanged += new System.EventHandler(this.txtChapter_TextChanged);
    }

    private void txtChapter_TextChanged(object sender, System.EventArgs e)
    {
      try
      {
        intIndex = listChapters.SelectedIndices[0];
        pgc.Chapters[intIndex] = new ChapterEntry()
        {
          Time = TimeSpan.Parse(txtChapterTime.Text),
          Name = txtChapterName.Text
        };
        listChapters.SelectedItems[0].SubItems[0].Text = txtChapterTime.Text;
        listChapters.SelectedItems[0].SubItems[1].Text = txtChapterName.Text;
      }
      catch (Exception parse)
      { /*/invalid time input
				txtChapterTime.Focus();
				txtChapterTime.SelectAll();
				MessageBox.Show("Invalid time format! Must be in the form of \"00:00:00.000\""
					+ Environment.NewLine + parse.Message);
				return;*/
        Trace.WriteLine(parse);
      }
    }

    private void ChapterControls_Click(object sender, System.EventArgs e)
    {
      ChapterEntry c;
      string name = null;
      Button btn = (Button)sender;
      switch (btn.Name)
      {
        case "btnAdd":
          TimeSpan ts = new TimeSpan(0);
          try
          {//try to get a valid time input					
            ts = TimeSpan.Parse(txtChapterTime.Text);
          }
          catch (Exception parse)
          { //invalid time input
            txtChapterTime.Focus();
            txtChapterTime.SelectAll();
            MessageBox.Show("Invalid time format! Must be in the form of \"00:00:00.000\""
              + Environment.NewLine + parse.Message);
            return;
          }
          //create a new chapter
          c = new ChapterEntry() { Time = ts, Name = txtChapterName.Text };
          pgc.Chapters.Insert(intIndex, c);
          FreshChapterView();
          break;
        case "btnDelete":
          if (listChapters.Items.Count < 1 || pgc.Chapters.Count < 1) return;
          intIndex = listChapters.SelectedIndices[0];
          pgc.Chapters.Remove(pgc.Chapters[intIndex]);
          if (intIndex != 0) intIndex--;
          FreshChapterView();
          break;
        case "btnUp":
          if (pgc.Chapters.Count < 2 || intIndex < 1) return;
          name = pgc.Chapters[intIndex].Name;
          pgc.Chapters[intIndex] = new ChapterEntry() { Name = pgc.Chapters[intIndex - 1].Name, Time = pgc.Chapters[intIndex].Time };
          pgc.Chapters[intIndex - 1] = new ChapterEntry() { Name = name, Time = pgc.Chapters[intIndex - 1].Time };
          intIndex--;
          FreshChapterView();
          break;
        case "btnDn":
          if (pgc.Chapters.Count < 2 || intIndex >= pgc.Chapters.Count - 1) return;
          name = pgc.Chapters[intIndex].Name;
          pgc.Chapters[intIndex] = new ChapterEntry() { Name = pgc.Chapters[intIndex + 1].Name, Time = pgc.Chapters[intIndex].Time };
          pgc.Chapters[intIndex + 1] = new ChapterEntry() { Name = name, Time = pgc.Chapters[intIndex + 1].Time };
          intIndex++;
          FreshChapterView();
          break;
      }

    }

    private void menuHelpAbout_Click(object sender, System.EventArgs e)
    {
      using (AboutDialog frm = new AboutDialog())
      {
        frm.ShowDialog();
      }
    }

    void Search()
    {
      Cursor = Cursors.WaitCursor;
      tsslStatus.Text = "Searching for chapter names...";
      picSearch.Visible = true;
      try
      {
          if (string.IsNullOrEmpty(txtTitle.Text))
              throw new Exception("You must supply a title to search for chapter names.");

          Action<List<SearchResult>> a = (titles) =>
          {
              flowResults.Controls.Clear();
              flowResults.Controls.AddRange(
                  titles.Select(x => new SearchResultItem(x)).ToArray());
              var items = flowResults.Controls.OfType<SearchResultItem>();
              foreach (var item in items)
                  item.OnSelected += (s, e) =>
                  {

                      try
                      {
                          Cursor = Cursors.WaitCursor;

                          // unselect others
                          foreach (var x in items.Where(x => x != s))
                              x.Unselect();

                          if (grabber == null) return;

                          var result = (s as SearchResultItem).Tag as SearchResult;

                          grabber.PopulateNames(result, pgc, miImportDurations.Checked);
                          FreshChapterView();
                      }
                      catch (Exception ex)
                      {
                          Trace.WriteLine(ex);
                          MessageBox.Show(ex.Message);
                      }
                      finally
                      {
                          Cursor = Cursors.Default;
                      }
                  };
                  
            //lstResults.Items.Clear();
            //lstResults.Items.AddRange(titles.ToArray());
            //lstResults.ValueMember = "Id";
            //lstResults.DisplayMember = "Name";
            tsslStatus.Text = string.Format("Search returned {0} result(s).", titles.Count);
            picSearch.Visible = false;
            Cursor = Cursors.Default;
          };
        ThreadPool.QueueUserWorkItem((w) =>
            {
                try
                {
                    var titles = grabber.Search(pgc);
                    Invoke(a, titles);
                }
                catch (Exception ex)
                {
                    Invoke(new Action<Exception>(SearchError), ex);
                }
            });
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex);
        MessageBox.Show(ex.Message, "Error");
        tsslStatus.Text = "Failed to search for chapter names.";
        picSearch.Visible = false;
        Cursor = Cursors.Default;
      }
    }

    private void SearchError(Exception ex)
    {
        Trace.WriteLine(ex);
        MessageBox.Show(ex.Message, "Error");
        tsslStatus.Text = "Failed to search for chapter names.";
        picSearch.Visible = false;
        Cursor = Cursors.Default;
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.ContextMenu.Show(btnSearch, new Point(0, btnSearch.Height));
    }

    //private void lstResults_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    var sel = flowResults.Controls.OfType<SearchResultItem>()
    //        .Where(x => x.Selected);
    //    if (!sel.Any()) return;

    //  if (grabber == null) return;

    //  var result = lstResults.Items[lstResults.SelectedIndex] as SearchResult;

    //  try
    //  {
    //    Cursor = Cursors.WaitCursor;
    //    grabber.PopulateNames(result, pgc, miImportDurations.Checked);
    //    FreshChapterView();
    //  }
    //  catch (Exception ex)
    //  {
    //    Trace.WriteLine(ex);
    //    MessageBox.Show(ex.Message);
    //  }
    //  finally
    //  {
    //    Cursor = Cursors.Default;
    //  }
    //  //Grabber.ImportFromSearchXml(pgc.Chapters, searchXml, result.Id);
    //}

    private void menuResetNames_Click(object sender, EventArgs e)
    {
      for (int i = 0; i < pgc.Chapters.Count; i++)
      {
        pgc.Chapters[i] = new ChapterEntry() { Name = "Chapter " + (i + 1), Time = pgc.Chapters[i].Time };
      }
      FreshChapterView();
      tsslStatus.Text = "Chapter names reset successfully.";
    }

    private void miOpenDisc_Click(object sender, EventArgs e)
    {
      using (FolderBrowserDialog d = new FolderBrowserDialog())
      {
        d.ShowNewFolderButton = false;
        d.Description = "Select DVD, HD-DVD, BluRay disc, or folder.";
        var initDir = Pathing.FileNameUtils.GetClosestExistingDirectory(Settings.Default.LastOpenDir);
        if (initDir != null)
        {
            d.SelectedPath = initDir;
        }
        if (d.ShowDialog() == DialogResult.OK)
        {
            OpenDisc(d.SelectedPath);
        }
      }
    }

    private void OpenDisc(string path)
    {
        Cursor = Cursors.WaitCursor;
        tsslStatus.Text = "Scanning for chapters...";
        try
        {
            ChapterExtractor ex =
              Directory.Exists(Path.Combine(path, "VIDEO_TS")) ?
              new DvdExtractor() as ChapterExtractor :
              Directory.Exists(Path.Combine(path, "ADV_OBJ")) ?
              new HddvdExtractor() as ChapterExtractor :
              Directory.Exists(Path.Combine(Path.Combine(path, "BDMV"), "PLAYLIST")) ?
              new BlurayExtractor() as ChapterExtractor :
              null;

            if (ex == null)
                throw new Exception("The location was not detected as DVD, HD-DVD or Blu-Ray.");


            using (StreamSelectDialog frm = new StreamSelectDialog(ex))
            {
                ex.GetStreams(path);
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    Settings.Default.LastOpenDir = new DirectoryPathAbsolute(path).Path;
                    Settings.Default.Save();
                    pgc = frm.ProgramChain;
                    if (pgc.FramesPerSecond == 0) pgc.FramesPerSecond = Settings.Default.DefaultFps;
                    if (pgc.LangCode == null) pgc.LangCode = Settings.Default.DefaultLangCode;
                    AutoLoadNames();
                    FreshChapterView();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            Trace.WriteLine(ex);
            tsslStatus.Text = "Could not load chapters from disc.";
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private void miIgnoreShortLastChapter_Click(object sender, EventArgs e)
    {
      miIgnoreShortLastChapter.Checked = !miIgnoreShortLastChapter.Checked;
      FreshChapterView();
      Settings.Default.IgnoreShortLastChapter = miIgnoreShortLastChapter.Checked;
    }

    private void txtTitle_TextChanged(object sender, EventArgs e)
    {
      pgc.Title = txtTitle.Text.Trim();
      if (!string.IsNullOrEmpty(pgc.Title))
      {
          picDb.Visible = true;
          titleWait = true;
          ThreadPool.QueueUserWorkItem((w) =>
              {
                  var titles = Grabber.SuggestTitles(pgc.Title);
                  //txtTitle.Invoke(new Action<List<KeyValuePair<int, string>>>(this.UpdateTitles), titles);
                  this.UpdateTitles(titles);
              });
      }
    }

    private void UpdateTitles(List<KeyValuePair<int, string>> titles)
    {
        if (this.InvokeRequired)
        {
            Invoke(new Action<List<KeyValuePair<int, string>>>(this.UpdateTitles), titles);
            return;
        }

      menuTitles.Items.Clear();

      foreach (var t in titles)
      {
          menuTitles.Items.Add(t.Value.Replace("&", "&&"), t.Key > 0 ? Properties.Resources.star : null, (o, s) =>
              {
                  pgc.Title = ((ToolStripItem)o).Text.Replace("&&", "&");
                  txtTitle.TextChanged -= new EventHandler(txtTitle_TextChanged);
                  txtTitle.Text = pgc.Title;
                  txtTitle.TextChanged += new EventHandler(txtTitle_TextChanged);
                  if (((ToolStripItem)o).Image != null) btnTitles.Image = Properties.Resources.apply_good;
              });
      }
        int num = -1;
        if (titles.Count > 0 && titles[0].Value == pgc.Title) SetGoodTitle();
        else if (int.TryParse(pgc.Title, out num)) SetBadTitle();
        else if (pgc.Title.StartsWith("VTS_")) SetBadTitle();
        else if (pgc.Title == "Main Movie") SetBadTitle();
        else if (titles.Where(t => t.Value.Contains(pgc.Title)).Count() > 0) SetOkTitle();
        else SetBadTitle();

        titleWait = false;
      picDb.Visible = titleWait || dbWait;
    }

    private void SetGoodTitle()
    {
        btnTitles.Image = Properties.Resources.apply_good;
        toolTipTitle.ToolTipTitle = "Good Title";
        toolTipTitle.SetToolTip(btnTitles, "You've entered a good title.");
    }

    private void SetOkTitle()
    {
        btnTitles.Image = Properties.Resources.apply_ok;
        toolTipTitle.ToolTipTitle = "OK Title";
        toolTipTitle.SetToolTip(btnTitles, "Click to choose a better title.");
    }

    private void SetBadTitle()
    {
        btnTitles.Image = Properties.Resources.apply_bad;
        toolTipTitle.ToolTipTitle = "Bad Title";
        toolTipTitle.SetToolTip(btnTitles, "Please enter a good movie title.");
    }

    private void miUpdate_Click(object sender, EventArgs e)
    {
      UpdateStatus status = Updater.CheckForUpdate(ShowUpdateDialog);

      if (status == UpdateStatus.UpdateFailed)
        MessageBox.Show(this, "Failed to check for update.  Please ty again later.", "Warning");
      else if (status == UpdateStatus.NoUpdate)
        MessageBox.Show(this, "There are no updates available at this time.", "Update Check");
    }

    private void miAutoCheck_Click(object sender, EventArgs e)
    {
      miAutoCheck.Checked = !miAutoCheck.Checked;
      Settings.Default.AutoCheckForUpdate = miAutoCheck.Checked;
      Settings.Default.Save();
    }

    private void miAutoUseDb_Click(object sender, EventArgs e)
    {
        miAutoUseDb.Checked = !miAutoUseDb.Checked;
        Settings.Default.AutoUseDatabase = miAutoUseDb.Checked;
        Settings.Default.Save();
    }

    private void miDatabaseCredentials_Click(object sender, EventArgs e)
    {
        using (DatabaseCredentialsDialog d = new DatabaseCredentialsDialog())
            d.ShowDialog(this);
    }

    private void txtChapterName_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        // got to next
        int i = listChapters.SelectedIndices[0];
        i++;
        if (i < listChapters.Items.Count && listChapters.Items[i] != null)
        {
          //select it
          listChapters.Items[i].Selected = true;
          //scroll to it
          listChapters.Items[i].EnsureVisible();
          txtChapterName.SelectAll();
        }
      }
    }

    private void btnTitles_Click(object sender, EventArgs e)
    {
      btnTitles.ContextMenuStrip.Show(btnTitles, new Point(0, btnTitles.Height));
    }

    private void menuQuickSave_Click(object sender, EventArgs e)
    {
        UpdateDatabase(pgc);
        using (var d = new QuickSaveDialog(pgc))
        {
            d.ShowDialog(this);
        }
    }

    private void menuQuickOpen_Click(object sender, EventArgs e)
    {
        using (var dlg = new QuickOpenDialog())
        {
            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                OpenDisc(dlg.DiscPath);
            }
        }
    }

    private void label1_Click(object sender, EventArgs e)
    {

    }

    private void linkDatabase_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {

        // Change the color of the link text by setting LinkVisited 
        // to true.
        ((LinkLabel)sender).LinkVisited = true;
        //Call the Process.Start method to open the default browser 
        //with a URL:
        System.Diagnostics.Process.Start("http://chapterdb.org");
    }

    private void miDelay_Click(object sender, EventArgs e)
    {
        using (var dlg = new DelayForm())
        {
            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                for (int i = 0; i < this.pgc.Chapters.Count; i++)
                {
                    if (i == 0 && this.pgc.Chapters[i].Time.TotalSeconds == 0) continue;
                    this.pgc.Chapters[i] = new ChapterEntry()
                    {
                        Name = this.pgc.Chapters[i].Name,
                        Time = this.pgc.Chapters[i].Time.Add(TimeSpan.FromSeconds(dlg.DelaySeconds)),
                    };
                }
                this.FreshChapterView();
            }
        }

    }
  }
}
