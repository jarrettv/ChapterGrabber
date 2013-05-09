using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JarrettVance.ChapterTools.Properties;

namespace JarrettVance.ChapterTools
{
    public partial class DiscItem : UserControl
    {
        public event EventHandler Opened;

        public DiscItem()
        {
            InitializeComponent();
        }
        public DiscItem(Disc disc)
        {
            InitializeComponent();
            this.Tag = disc;

            lblName.Text = disc.Name;

            if (disc.FoundExtractions.Length > 0)
                this.lblStatus.Text = "Possible chapters files: " + string.Join(", ", disc.FoundExtractions);
            else
                this.lblStatus.Text = "No possible chapter files found";

            if (disc.Type == "DVD")
                picDisc.Image = Resources.DVD;
            else if (disc.Type == "Bluray")
                picDisc.Image = Resources.Bluray;
            else if (disc.Type == "HDDVD")
                picDisc.Image = Resources.HDDVD;
            else
                picDisc.Image = null;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Opened != null) Opened(this, e);
        }
    }
}
