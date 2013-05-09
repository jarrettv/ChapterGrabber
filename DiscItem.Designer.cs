namespace JarrettVance.ChapterTools
{
    partial class DiscItem
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
            this.picDisc = new System.Windows.Forms.PictureBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnUpdate = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picDisc)).BeginInit();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(89, 4);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(196, 20);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "2001_SPACE_ODYSSEY";
            // 
            // picDisc
            // 
            this.picDisc.Image = global::JarrettVance.ChapterTools.Properties.Resources.HDDVD;
            this.picDisc.Location = new System.Drawing.Point(3, 3);
            this.picDisc.Name = "picDisc";
            this.picDisc.Size = new System.Drawing.Size(80, 40);
            this.picDisc.TabIndex = 1;
            this.picDisc.TabStop = false;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblStatus.Location = new System.Drawing.Point(90, 27);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(128, 13);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "No chapter files extracted";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdate.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnUpdate.Image = global::JarrettVance.ChapterTools.Properties.Resources.folder;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnUpdate.Location = new System.Drawing.Point(378, 3);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(66, 40);
            this.btnUpdate.TabIndex = 4;
            this.btnUpdate.Text = "Open";
            this.btnUpdate.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // DiscItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.picDisc);
            this.Controls.Add(this.lblName);
            this.Name = "DiscItem";
            this.Size = new System.Drawing.Size(447, 47);
            ((System.ComponentModel.ISupportInitialize)(this.picDisc)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.PictureBox picDisc;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnUpdate;
    }
}
