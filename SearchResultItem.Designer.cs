namespace JarrettVance.ChapterTools
{
    partial class SearchResultItem
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblName = new System.Windows.Forms.Label();
            this.lblSourceType = new System.Windows.Forms.Label();
            this.lblDuration = new System.Windows.Forms.Label();
            this.lblCount = new System.Windows.Forms.Label();
            this.lblRelevance = new System.Windows.Forms.Label();
            this.lblHasNames = new System.Windows.Forms.Label();
            this.lblArrow = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.BackColor = System.Drawing.Color.Transparent;
            this.lblName.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblName.Location = new System.Drawing.Point(45, 1);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(226, 17);
            this.lblName.TabIndex = 0;
            this.lblName.Tag = "SelText";
            this.lblName.Text = "Indiana Jones and the Last Crusade";
            this.lblName.Click += new System.EventHandler(this.SearchResultItem_Click);
            // 
            // lblSourceType
            // 
            this.lblSourceType.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblSourceType.AutoSize = true;
            this.lblSourceType.BackColor = System.Drawing.Color.Transparent;
            this.lblSourceType.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblSourceType.Location = new System.Drawing.Point(131, 17);
            this.lblSourceType.Name = "lblSourceType";
            this.lblSourceType.Size = new System.Drawing.Size(42, 13);
            this.lblSourceType.TabIndex = 2;
            this.lblSourceType.Tag = "SelText";
            this.lblSourceType.Text = "BluRay";
            this.lblSourceType.Click += new System.EventHandler(this.SearchResultItem_Click);
            // 
            // lblDuration
            // 
            this.lblDuration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDuration.AutoSize = true;
            this.lblDuration.BackColor = System.Drawing.Color.Transparent;
            this.lblDuration.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblDuration.Location = new System.Drawing.Point(204, 17);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(49, 13);
            this.lblDuration.TabIndex = 3;
            this.lblDuration.Tag = "SelText";
            this.lblDuration.Text = "02:12:58";
            this.lblDuration.Click += new System.EventHandler(this.SearchResultItem_Click);
            // 
            // lblCount
            // 
            this.lblCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCount.AutoSize = true;
            this.lblCount.BackColor = System.Drawing.Color.Transparent;
            this.lblCount.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblCount.Location = new System.Drawing.Point(45, 17);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(66, 13);
            this.lblCount.TabIndex = 4;
            this.lblCount.Tag = "SelText";
            this.lblCount.Text = "21 chapters";
            this.lblCount.Click += new System.EventHandler(this.SearchResultItem_Click);
            // 
            // lblRelevance
            // 
            this.lblRelevance.BackColor = System.Drawing.Color.Transparent;
            this.lblRelevance.Font = new System.Drawing.Font("Segoe UI Symbol", 9F);
            this.lblRelevance.ForeColor = System.Drawing.Color.ForestGreen;
            this.lblRelevance.Location = new System.Drawing.Point(0, 0);
            this.lblRelevance.Name = "lblRelevance";
            this.lblRelevance.Size = new System.Drawing.Size(24, 28);
            this.lblRelevance.TabIndex = 5;
            this.lblRelevance.Tag = "NonSelText";
            this.lblRelevance.Text = "12";
            this.lblRelevance.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblRelevance.Click += new System.EventHandler(this.SearchResultItem_Click);
            // 
            // lblHasNames
            // 
            this.lblHasNames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblHasNames.BackColor = System.Drawing.Color.Transparent;
            this.lblHasNames.Font = new System.Drawing.Font("FontAwesome", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHasNames.ForeColor = System.Drawing.Color.ForestGreen;
            this.lblHasNames.Location = new System.Drawing.Point(24, 0);
            this.lblHasNames.Name = "lblHasNames";
            this.lblHasNames.Size = new System.Drawing.Size(20, 28);
            this.lblHasNames.TabIndex = 6;
            this.lblHasNames.Tag = "NonSelText";
            this.lblHasNames.Text = "";
            this.lblHasNames.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblHasNames.Click += new System.EventHandler(this.SearchResultItem_Click);
            // 
            // lblArrow
            // 
            this.lblArrow.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblArrow.BackColor = System.Drawing.Color.Transparent;
            this.lblArrow.Font = new System.Drawing.Font("FontAwesome", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblArrow.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblArrow.Location = new System.Drawing.Point(259, 0);
            this.lblArrow.Name = "lblArrow";
            this.lblArrow.Size = new System.Drawing.Size(26, 30);
            this.lblArrow.TabIndex = 7;
            this.lblArrow.Tag = "SelText";
            this.lblArrow.Text = "";
            this.lblArrow.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblArrow.Visible = false;
            this.lblArrow.Click += new System.EventHandler(this.SearchResultItem_Click);
            // 
            // SearchResultItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.lblHasNames);
            this.Controls.Add(this.lblArrow);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.lblDuration);
            this.Controls.Add(this.lblSourceType);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblRelevance);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SearchResultItem";
            this.Size = new System.Drawing.Size(285, 35);
            this.Load += new System.EventHandler(this.SearchResultItem_Load);
            this.Click += new System.EventHandler(this.SearchResultItem_Click);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.this_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblSourceType;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Label lblRelevance;
        private System.Windows.Forms.Label lblHasNames;
        private System.Windows.Forms.Label lblArrow;
    }
}
