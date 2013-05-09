using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace JarrettVance.ChapterTools
{
  public partial class QuickSaveDialog : Form
  {
      private readonly ChapterInfo pgc;

      public QuickSaveDialog(ChapterInfo pgc)
    {
      InitializeComponent();

          this.pgc = pgc;
          QuickSave(Settings.Default.LastOpenDir);

    }
      private void QuickSave(string path)
      {
          lblStatus.Text = string.Empty;
          txtFolder.Text = path;

          if (String.IsNullOrEmpty(path))
          {
              lblStatus.Text = "Please choose a destination folder.";
              btnDir.Focus();
              return;
          }

          if (this.pgc.Chapters.Count == 0)
          {
              lblStatus.Text = "No chapters to save.";
              btnOK.Focus();
              return;
          }

          try
          {
              string name = (pgc.Title ?? pgc.SourceName ?? "Chapters");
              name = Pathing.FileNameUtils.MakeValidFileName(name);

              path = Path.Combine(path, name);

              lblStatus.Text += "Saving to '" + path + ".chapters" + "'" + Environment.NewLine;
              pgc.Save(path + ".chapters");

              lblStatus.Text += "Saving to '" + path + ".xml" + "'" + Environment.NewLine;
              pgc.SaveXml(path + ".xml");

              lblStatus.Text += "Saving to '" + path + ".chapters.txt" + "'" + Environment.NewLine;
              pgc.SaveText(path + ".chapters.txt");

              lblStatus.Text += "Done.";
          }
          catch (UnauthorizedAccessException)
          {
              lblStatus.Text = "The folder is read-only.  Please choose a different folder.";
              btnDir.Focus();
          }
          catch (Exception ex)
          {
              lblStatus.Text += ex.Message;
          }
      }


      private void btnDir_Click(object sender, EventArgs e)
      {
          using (FolderBrowserDialog d = new FolderBrowserDialog())
          {
              d.Description = "Select a folder.";
              if (d.ShowDialog() == DialogResult.OK)
              {
                  QuickSave(d.SelectedPath);
              }
          }
      }
  }
}
