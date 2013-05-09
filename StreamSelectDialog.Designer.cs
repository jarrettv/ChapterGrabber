﻿namespace JarrettVance.ChapterTools
{
  partial class StreamSelectDialog
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.btnOK = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.lblCounts = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxLessThan20mins = new System.Windows.Forms.CheckBox();
            this.checkBoxLessThan5 = new System.Windows.Forms.CheckBox();
            this.checkBoxGreaterThan50 = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnOK.AutoSize = true;
            this.btnOK.Image = global::JarrettVance.ChapterTools.Properties.Resources.accept;
            this.btnOK.Location = new System.Drawing.Point(444, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(64, 30);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(13, 15);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(584, 186);
            this.listBox1.TabIndex = 2;
            this.listBox1.DoubleClick += new System.EventHandler(this.btnOK_Click);
            // 
            // lblCounts
            // 
            this.lblCounts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCounts.Location = new System.Drawing.Point(423, 207);
            this.lblCounts.Name = "lblCounts";
            this.lblCounts.Size = new System.Drawing.Size(174, 17);
            this.lblCounts.TabIndex = 9;
            this.lblCounts.Text = "loading...";
            this.lblCounts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 209);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 16);
            this.label1.TabIndex = 8;
            this.label1.Text = "Hide";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.checkBoxLessThan20mins);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxLessThan5);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxGreaterThan50);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(13, 226);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(584, 27);
            this.flowLayoutPanel1.TabIndex = 7;
            // 
            // checkBoxLessThan20mins
            // 
            this.checkBoxLessThan20mins.AutoSize = true;
            this.checkBoxLessThan20mins.Checked = true;
            this.checkBoxLessThan20mins.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLessThan20mins.Location = new System.Drawing.Point(3, 3);
            this.checkBoxLessThan20mins.Name = "checkBoxLessThan20mins";
            this.checkBoxLessThan20mins.Size = new System.Drawing.Size(111, 17);
            this.checkBoxLessThan20mins.TabIndex = 3;
            this.checkBoxLessThan20mins.Text = "Less than 20 mins";
            this.checkBoxLessThan20mins.UseVisualStyleBackColor = true;
            this.checkBoxLessThan20mins.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxLessThan5
            // 
            this.checkBoxLessThan5.AutoSize = true;
            this.checkBoxLessThan5.Checked = true;
            this.checkBoxLessThan5.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLessThan5.Location = new System.Drawing.Point(120, 3);
            this.checkBoxLessThan5.Name = "checkBoxLessThan5";
            this.checkBoxLessThan5.Size = new System.Drawing.Size(125, 17);
            this.checkBoxLessThan5.TabIndex = 6;
            this.checkBoxLessThan5.Text = "Less than 5 chapters";
            this.checkBoxLessThan5.UseVisualStyleBackColor = true;
            this.checkBoxLessThan5.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxGreaterThan50
            // 
            this.checkBoxGreaterThan50.AutoSize = true;
            this.checkBoxGreaterThan50.Checked = true;
            this.checkBoxGreaterThan50.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxGreaterThan50.Location = new System.Drawing.Point(251, 3);
            this.checkBoxGreaterThan50.Name = "checkBoxGreaterThan50";
            this.checkBoxGreaterThan50.Size = new System.Drawing.Size(144, 17);
            this.checkBoxGreaterThan50.TabIndex = 7;
            this.checkBoxGreaterThan50.Text = "Greater than 50 chapters";
            this.checkBoxGreaterThan50.UseVisualStyleBackColor = true;
            this.checkBoxGreaterThan50.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancel.AutoSize = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = global::JarrettVance.ChapterTools.Properties.Resources.cancel;
            this.btnCancel.Location = new System.Drawing.Point(514, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(67, 30);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel2.Controls.Add(this.btnCancel);
            this.flowLayoutPanel2.Controls.Add(this.btnOK);
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(13, 252);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(584, 36);
            this.flowLayoutPanel2.TabIndex = 11;
            // 
            // StreamSelectDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(609, 291);
            this.ControlBox = false;
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.lblCounts);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.listBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StreamSelectDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Stream";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.ListBox listBox1;
    private System.Windows.Forms.Label lblCounts;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private System.Windows.Forms.CheckBox checkBoxLessThan20mins;
    private System.Windows.Forms.CheckBox checkBoxLessThan5;
    private System.Windows.Forms.CheckBox checkBoxGreaterThan50;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
  }
}