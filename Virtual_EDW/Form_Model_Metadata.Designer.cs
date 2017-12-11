using System;

namespace Virtual_EDW
{
    partial class FormModelMetadata
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormModelMetadata));
            this.richTextBoxInformation = new System.Windows.Forms.RichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.metadataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.labelInformation = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelVersion = new System.Windows.Forms.Label();
            this.trackBarVersioning = new System.Windows.Forms.TrackBar();
            this.outputGroupBoxVersioning = new System.Windows.Forms.GroupBox();
            this.radioButtonMinorRelease = new System.Windows.Forms.RadioButton();
            this.radiobuttonMajorRelease = new System.Windows.Forms.RadioButton();
            this.radiobuttonNoVersionChange = new System.Windows.Forms.RadioButton();
            this.MetadataGenerationGroupBox = new System.Windows.Forms.GroupBox();
            this.checkBoxIgnoreVersion = new System.Windows.Forms.CheckBox();
            this.checkBoxClearMetadata = new System.Windows.Forms.CheckBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dataGridViewTableMetadata = new Virtual_EDW.CustomDataGridViewTable();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dataGridViewAttributeMetadata = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.dataGridViewDrivingKeyMetadata = new Virtual_EDW.CustomDataGridViewTable();
            this.button2 = new System.Windows.Forms.Button();
            this.labelResult = new System.Windows.Forms.Label();
            this.buttonStart = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.grpTableName = new System.Windows.Forms.GroupBox();
            this.radioButtonIntegrationLayer = new System.Windows.Forms.RadioButton();
            this.radioButtonStagingLayer = new System.Windows.Forms.RadioButton();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVersioning)).BeginInit();
            this.outputGroupBoxVersioning.SuspendLayout();
            this.MetadataGenerationGroupBox.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTableMetadata)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAttributeMetadata)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDrivingKeyMetadata)).BeginInit();
            this.grpTableName.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBoxInformation
            // 
            this.richTextBoxInformation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxInformation.Location = new System.Drawing.Point(16, 648);
            this.richTextBoxInformation.Name = "richTextBoxInformation";
            this.richTextBoxInformation.Size = new System.Drawing.Size(997, 101);
            this.richTextBoxInformation.TabIndex = 2;
            this.richTextBoxInformation.Text = "";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.metadataToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1337, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // metadataToolStripMenuItem
            // 
            this.metadataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem});
            this.metadataToolStripMenuItem.Name = "metadataToolStripMenuItem";
            this.metadataToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.metadataToolStripMenuItem.Text = "File";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Image = global::Virtual_EDW.Properties.Resources.ExitApplication;
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.closeToolStripMenuItem.Text = "Close Window";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // labelInformation
            // 
            this.labelInformation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelInformation.AutoSize = true;
            this.labelInformation.Location = new System.Drawing.Point(13, 632);
            this.labelInformation.Name = "labelInformation";
            this.labelInformation.Size = new System.Drawing.Size(59, 13);
            this.labelInformation.TabIndex = 5;
            this.labelInformation.Text = "Information";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(1028, 52);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 40);
            this.button1.TabIndex = 19;
            this.button1.Text = "Reverse Engineer Model";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.buttonReverseEngineer_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.labelVersion);
            this.groupBox1.Controls.Add(this.trackBarVersioning);
            this.groupBox1.Location = new System.Drawing.Point(1028, 417);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(178, 76);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Version selection";
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(9, 57);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(42, 13);
            this.labelVersion.TabIndex = 18;
            this.labelVersion.Text = "Version";
            // 
            // trackBarVersioning
            // 
            this.trackBarVersioning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarVersioning.Location = new System.Drawing.Point(7, 23);
            this.trackBarVersioning.Name = "trackBarVersioning";
            this.trackBarVersioning.Size = new System.Drawing.Size(163, 45);
            this.trackBarVersioning.TabIndex = 17;
            this.trackBarVersioning.ValueChanged += new System.EventHandler(this.trackBarVersioning_ValueChanged);
            // 
            // outputGroupBoxVersioning
            // 
            this.outputGroupBoxVersioning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputGroupBoxVersioning.Controls.Add(this.radioButtonMinorRelease);
            this.outputGroupBoxVersioning.Controls.Add(this.radiobuttonMajorRelease);
            this.outputGroupBoxVersioning.Controls.Add(this.radiobuttonNoVersionChange);
            this.outputGroupBoxVersioning.Location = new System.Drawing.Point(1154, 157);
            this.outputGroupBoxVersioning.Name = "outputGroupBoxVersioning";
            this.outputGroupBoxVersioning.Size = new System.Drawing.Size(178, 95);
            this.outputGroupBoxVersioning.TabIndex = 20;
            this.outputGroupBoxVersioning.TabStop = false;
            this.outputGroupBoxVersioning.Text = "Version control";
            // 
            // radioButtonMinorRelease
            // 
            this.radioButtonMinorRelease.AutoSize = true;
            this.radioButtonMinorRelease.Location = new System.Drawing.Point(7, 67);
            this.radioButtonMinorRelease.Name = "radioButtonMinorRelease";
            this.radioButtonMinorRelease.Size = new System.Drawing.Size(111, 17);
            this.radioButtonMinorRelease.TabIndex = 2;
            this.radioButtonMinorRelease.Text = "Minor release (0.x)";
            this.radioButtonMinorRelease.UseVisualStyleBackColor = true;
            // 
            // radiobuttonMajorRelease
            // 
            this.radiobuttonMajorRelease.AutoSize = true;
            this.radiobuttonMajorRelease.Location = new System.Drawing.Point(7, 44);
            this.radiobuttonMajorRelease.Name = "radiobuttonMajorRelease";
            this.radiobuttonMajorRelease.Size = new System.Drawing.Size(111, 17);
            this.radiobuttonMajorRelease.TabIndex = 1;
            this.radiobuttonMajorRelease.Text = "Major release (x.0)";
            this.radiobuttonMajorRelease.UseVisualStyleBackColor = true;
            // 
            // radiobuttonNoVersionChange
            // 
            this.radiobuttonNoVersionChange.AutoSize = true;
            this.radiobuttonNoVersionChange.Location = new System.Drawing.Point(7, 21);
            this.radiobuttonNoVersionChange.Name = "radiobuttonNoVersionChange";
            this.radiobuttonNoVersionChange.Size = new System.Drawing.Size(115, 17);
            this.radiobuttonNoVersionChange.TabIndex = 0;
            this.radiobuttonNoVersionChange.Text = "No version change";
            this.radiobuttonNoVersionChange.UseVisualStyleBackColor = true;
            // 
            // MetadataGenerationGroupBox
            // 
            this.MetadataGenerationGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MetadataGenerationGroupBox.Controls.Add(this.checkBoxIgnoreVersion);
            this.MetadataGenerationGroupBox.Controls.Add(this.checkBoxClearMetadata);
            this.MetadataGenerationGroupBox.Location = new System.Drawing.Point(1028, 305);
            this.MetadataGenerationGroupBox.Name = "MetadataGenerationGroupBox";
            this.MetadataGenerationGroupBox.Size = new System.Drawing.Size(304, 66);
            this.MetadataGenerationGroupBox.TabIndex = 22;
            this.MetadataGenerationGroupBox.TabStop = false;
            this.MetadataGenerationGroupBox.Text = "Metadata generation options";
            // 
            // checkBoxIgnoreVersion
            // 
            this.checkBoxIgnoreVersion.AutoSize = true;
            this.checkBoxIgnoreVersion.Checked = true;
            this.checkBoxIgnoreVersion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxIgnoreVersion.Location = new System.Drawing.Point(6, 43);
            this.checkBoxIgnoreVersion.Name = "checkBoxIgnoreVersion";
            this.checkBoxIgnoreVersion.Size = new System.Drawing.Size(219, 17);
            this.checkBoxIgnoreVersion.TabIndex = 26;
            this.checkBoxIgnoreVersion.Text = "Use live database / ignore model version";
            this.checkBoxIgnoreVersion.UseVisualStyleBackColor = true;
            // 
            // checkBoxClearMetadata
            // 
            this.checkBoxClearMetadata.AutoSize = true;
            this.checkBoxClearMetadata.Location = new System.Drawing.Point(6, 21);
            this.checkBoxClearMetadata.Name = "checkBoxClearMetadata";
            this.checkBoxClearMetadata.Size = new System.Drawing.Size(150, 17);
            this.checkBoxClearMetadata.TabIndex = 10;
            this.checkBoxClearMetadata.Text = "Clear generation metadata";
            this.checkBoxClearMetadata.UseVisualStyleBackColor = true;
            this.checkBoxClearMetadata.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Location = new System.Drawing.Point(1154, 499);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(178, 96);
            this.richTextBox1.TabIndex = 24;
            this.richTextBox1.Text = "Activation of the metadata will process / upload the selected version into the ac" +
    "tive tool (similar to the slides on the main screen). \n\nThis allows for testing " +
    "and troubleshooting.";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(16, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1001, 602);
            this.tabControl1.TabIndex = 25;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridViewTableMetadata);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(993, 576);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Model Metadata";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dataGridViewTableMetadata
            // 
            this.dataGridViewTableMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewTableMetadata.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTableMetadata.Location = new System.Drawing.Point(2, 3);
            this.dataGridViewTableMetadata.Name = "dataGridViewTableMetadata";
            this.dataGridViewTableMetadata.Size = new System.Drawing.Size(988, 570);
            this.dataGridViewTableMetadata.TabIndex = 1;
            this.dataGridViewTableMetadata.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DataGridViewTableMetadataKeyDown);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridViewAttributeMetadata);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(993, 576);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Multi-Active Attributes (read-only)";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridViewAttributeMetadata
            // 
            this.dataGridViewAttributeMetadata.AllowUserToAddRows = false;
            this.dataGridViewAttributeMetadata.AllowUserToDeleteRows = false;
            this.dataGridViewAttributeMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewAttributeMetadata.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAttributeMetadata.ImeMode = System.Windows.Forms.ImeMode.On;
            this.dataGridViewAttributeMetadata.Location = new System.Drawing.Point(2, 3);
            this.dataGridViewAttributeMetadata.MultiSelect = false;
            this.dataGridViewAttributeMetadata.Name = "dataGridViewAttributeMetadata";
            this.dataGridViewAttributeMetadata.ReadOnly = true;
            this.dataGridViewAttributeMetadata.Size = new System.Drawing.Size(964, 570);
            this.dataGridViewAttributeMetadata.TabIndex = 1;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dataGridViewDrivingKeyMetadata);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(993, 576);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Driving Key Attributes (read-only)";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // dataGridViewDrivingKeyMetadata
            // 
            this.dataGridViewDrivingKeyMetadata.AllowUserToAddRows = false;
            this.dataGridViewDrivingKeyMetadata.AllowUserToDeleteRows = false;
            this.dataGridViewDrivingKeyMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewDrivingKeyMetadata.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewDrivingKeyMetadata.Location = new System.Drawing.Point(2, 3);
            this.dataGridViewDrivingKeyMetadata.MultiSelect = false;
            this.dataGridViewDrivingKeyMetadata.Name = "dataGridViewDrivingKeyMetadata";
            this.dataGridViewDrivingKeyMetadata.ReadOnly = true;
            this.dataGridViewDrivingKeyMetadata.Size = new System.Drawing.Size(964, 570);
            this.dataGridViewDrivingKeyMetadata.TabIndex = 2;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(1028, 157);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(120, 40);
            this.button2.TabIndex = 27;
            this.button2.Text = "Save Model Changes";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // labelResult
            // 
            this.labelResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelResult.AutoSize = true;
            this.labelResult.Location = new System.Drawing.Point(1028, 542);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(38, 13);
            this.labelResult.TabIndex = 29;
            this.labelResult.Text = "Ready";
            // 
            // buttonStart
            // 
            this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStart.Location = new System.Drawing.Point(1028, 499);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(120, 40);
            this.buttonStart.TabIndex = 28;
            this.buttonStart.Text = "Activate Metadata";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWorkMetadataActivation);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // grpTableName
            // 
            this.grpTableName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpTableName.Controls.Add(this.radioButtonIntegrationLayer);
            this.grpTableName.Controls.Add(this.radioButtonStagingLayer);
            this.grpTableName.Location = new System.Drawing.Point(1154, 51);
            this.grpTableName.Name = "grpTableName";
            this.grpTableName.Size = new System.Drawing.Size(178, 70);
            this.grpTableName.TabIndex = 66;
            this.grpTableName.TabStop = false;
            this.grpTableName.Text = "Target area";
            // 
            // radioButtonIntegrationLayer
            // 
            this.radioButtonIntegrationLayer.AutoSize = true;
            this.radioButtonIntegrationLayer.Location = new System.Drawing.Point(6, 42);
            this.radioButtonIntegrationLayer.Name = "radioButtonIntegrationLayer";
            this.radioButtonIntegrationLayer.Size = new System.Drawing.Size(104, 17);
            this.radioButtonIntegrationLayer.TabIndex = 1;
            this.radioButtonIntegrationLayer.Text = "Integration Layer";
            this.radioButtonIntegrationLayer.UseVisualStyleBackColor = true;
            // 
            // radioButtonStagingLayer
            // 
            this.radioButtonStagingLayer.AutoSize = true;
            this.radioButtonStagingLayer.Checked = true;
            this.radioButtonStagingLayer.Location = new System.Drawing.Point(6, 19);
            this.radioButtonStagingLayer.Name = "radioButtonStagingLayer";
            this.radioButtonStagingLayer.Size = new System.Drawing.Size(90, 17);
            this.radioButtonStagingLayer.TabIndex = 0;
            this.radioButtonStagingLayer.TabStop = true;
            this.radioButtonStagingLayer.Text = "Staging Layer";
            this.radioButtonStagingLayer.UseVisualStyleBackColor = true;
            // 
            // FormModelMetadata
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1337, 761);
            this.Controls.Add(this.grpTableName);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.MetadataGenerationGroupBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.outputGroupBoxVersioning);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.labelInformation);
            this.Controls.Add(this.richTextBoxInformation);
            this.Controls.Add(this.menuStrip1);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(1353, 800);
            this.Name = "FormModelMetadata";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage the model metadata";
            this.SizeChanged += new System.EventHandler(this.FormModelMetadata_SizeChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVersioning)).EndInit();
            this.outputGroupBoxVersioning.ResumeLayout(false);
            this.outputGroupBoxVersioning.PerformLayout();
            this.MetadataGenerationGroupBox.ResumeLayout(false);
            this.MetadataGenerationGroupBox.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTableMetadata)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAttributeMetadata)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDrivingKeyMetadata)).EndInit();
            this.grpTableName.ResumeLayout(false);
            this.grpTableName.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBoxInformation;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem metadataToolStripMenuItem;
        private System.Windows.Forms.Label labelInformation;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.TrackBar trackBarVersioning;
        private System.Windows.Forms.GroupBox outputGroupBoxVersioning;
        private System.Windows.Forms.RadioButton radioButtonMinorRelease;
        private System.Windows.Forms.RadioButton radiobuttonMajorRelease;
        private System.Windows.Forms.RadioButton radiobuttonNoVersionChange;
        private System.Windows.Forms.GroupBox MetadataGenerationGroupBox;
        private System.Windows.Forms.CheckBox checkBoxClearMetadata;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private CustomDataGridViewTable dataGridViewTableMetadata;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataGridViewAttributeMetadata;
        private System.Windows.Forms.TabPage tabPage3;
        private CustomDataGridViewTable dataGridViewDrivingKeyMetadata;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label labelResult;
        private System.Windows.Forms.Button buttonStart;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        internal System.Windows.Forms.GroupBox grpTableName;
        internal System.Windows.Forms.RadioButton radioButtonIntegrationLayer;
        internal System.Windows.Forms.RadioButton radioButtonStagingLayer;
        private System.Windows.Forms.CheckBox checkBoxIgnoreVersion;
    }
}