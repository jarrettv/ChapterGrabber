using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JarrettVance.ChapterTools
{
    public partial class ErrorItem : UserControl
    {
        public ErrorItem(string error)
        {
            InitializeComponent();
            this.lblError.Text = error;
        }
    }
}
