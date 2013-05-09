using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JarrettVance.ChapterTools
{
  public partial class StreamSelectDialog : Form
  {
      private static readonly TimeSpan TWENTY = TimeSpan.FromMinutes(20);
      private readonly List<ChapterInfo> extracted;

    public StreamSelectDialog(ChapterExtractor extractor)
    {
      InitializeComponent();
      extracted = new List<ChapterInfo>();

      extractor.StreamDetected += (sender, arg) =>
        {
            extracted.Add(arg.ProgramChain);
            if (!Hidden(arg.ProgramChain)) listBox1.Items.Add(arg.ProgramChain);
        };
      extractor.ChaptersLoaded += (sender, arg) =>
        {
          for (int i = 0; i < listBox1.Items.Count; i++)
          {
            if (((ChapterInfo)listBox1.Items[i]).SourceName == arg.ProgramChain.SourceName)
            {
              listBox1.Items[i] = arg.ProgramChain;
              break;
            }
          }
        };
      extractor.ExtractionComplete += (sender, arg) =>
      {
          Reload();
        };

    }

    private bool Hidden(ChapterInfo pgc)
    {
        if (checkBoxLessThan20mins.Checked && pgc.Duration < TWENTY) return true;
        if (checkBoxLessThan5.Checked && pgc.Chapters.Count < 5) return true;
        if (checkBoxGreaterThan50.Checked && pgc.Chapters.Count > 50) return true;
        return false;
    }

    private void Reload()
    {
        ChapterInfo[] show = extracted
            .Where(x => !Hidden(x))
            .OrderByDescending(x => x.Duration)
            .ToArray();
        int hidden = extracted.Count - show.Length;
        lblCounts.Text = string.Format("{0} shown {1} hidden", show.Length, hidden);
        listBox1.Items.Clear();
        listBox1.Items.AddRange(show);

        if (extracted.Count > 0)
        {
            var vol = extracted.First().VolumeName;
            if (!string.IsNullOrEmpty(vol))
                this.Text = "Select Stream from " + vol;
        }
    }

    public ChapterInfo ProgramChain
    {
      get { return listBox1.SelectedItems.Count > 0 ? listBox1.SelectedItem as ChapterInfo : null; }
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      if (listBox1.SelectedItems.Count == 1)
        DialogResult = DialogResult.OK;
      else
        MessageBox.Show("Please select a stream.");
    }

    private void checkBox_CheckedChanged(object sender, EventArgs e)
    {
        Reload();
    }
  }
}
