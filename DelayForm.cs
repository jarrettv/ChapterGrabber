using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JarrettVance.ChapterTools
{
    public partial class DelayForm : Form
    {
        public DelayForm()
        {
            InitializeComponent();
        }

        public double DelaySeconds
        {
            get { return (double)this.numUpDown.Value; }
            set { this.numUpDown.Value = (decimal)value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
