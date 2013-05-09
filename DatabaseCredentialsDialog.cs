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
    public partial class DatabaseCredentialsDialog : Form
    {
        public DatabaseCredentialsDialog()
        {
            InitializeComponent();
        }

        private void DatabaseCredentialsDialog_Load(object sender, EventArgs e)
        {
            linkDatabase.Text = Settings.Default.DatabaseSite;            
            txtApiKey.Text = Settings.Default.DatabaseApiKey;
        }

        private void txtApiKey_Validating(object sender, CancelEventArgs e)
        {
            if (txtApiKey.Text.Trim().Length != 20)
            {
                errorProvider1.SetError(txtApiKey, "You must provide a valid 20 digit api key.");
            }
            else errorProvider1.SetError(txtApiKey, "");
        }

        private void DatabaseCredentialsDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsValid() && this.DialogResult == DialogResult.OK)
            {
                if (e.CloseReason == CloseReason.FormOwnerClosing || e.CloseReason == CloseReason.UserClosing || 
                    e.CloseReason == CloseReason.None)
                    e.Cancel = true;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (IsValid())
            {
                Settings.Default.DatabaseApiKey = txtApiKey.Text.Trim();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private bool IsValid()
        {
            return errorProvider1.GetError(txtApiKey) == string.Empty;
        }

        private void linkDatabase_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(linkDatabase.Text);
            linkDatabase.LinkVisited = true;
        }
    }
}
