using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;

namespace JarrettVance.ChapterTools
{
    public partial class UpdateForm : Form
    {
        public UpdateForm()
        {
            InitializeComponent();
        }

        public string Info { get { return lblInfo.Text; } set { lblInfo.Text = value; } }
        public string MoreInfoLink { get; set; }

        private void linkInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkInfo.LinkVisited = true;
            System.Diagnostics.Process.Start(MoreInfoLink);
        }
    }
}
