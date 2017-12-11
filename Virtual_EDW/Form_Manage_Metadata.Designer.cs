using System;

namespace Virtual_EDW
{
    partial class FormManageMetadata
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormManageMetadata));
            this.richTextBoxInformation = new System.Windows.Forms.RichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.metadataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsDirectionalGraphMarkupLanguageDGMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.businessKeyMetadataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMetadataFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMetadataFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attributeMappingMetadataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.validationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageValidationRulesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.labelInformation = new System.Windows.Forms.Label();
            this.MetadataGenerationGroupBox = new System.Windows.Forms.GroupBox();
            this.checkBoxIgnoreVersion = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBoxClearMetadata = new System.Windows.Forms.CheckBox();
            this.outputGroupBoxVersioning = new System.Windows.Forms.GroupBox();
            this.radioButtonMinorRelease = new System.Windows.Forms.RadioButton();
            this.radiobuttonMajorRelease = new System.Windows.Forms.RadioButton();
            this.radiobuttonNoVersionChange = new System.Windows.Forms.RadioButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dataGridViewTableMetadata = new Virtual_EDW.CustomDataGridViewTable();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dataGridViewAttributeMetadata = new Virtual_EDW.CustomDataGridViewAttribute();
            this.groupBoxMetadataCounts = new System.Windows.Forms.GroupBox();
            this.labelLsatCount = new System.Windows.Forms.Label();
            this.labelLnkCount = new System.Windows.Forms.Label();
            this.labelSatCount = new System.Windows.Forms.Label();
            this.labelHubCount = new System.Windows.Forms.Label();
            this.trackBarVersioning = new System.Windows.Forms.TrackBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelVersion = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.buttonStart = new System.Windows.Forms.Button();
            this.labelResult = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.buttonValidation = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxFilterCriterion = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.MetadataGenerationGroupBox.SuspendLayout();
            this.outputGroupBoxVersioning.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTableMetadata)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAttributeMetadata)).BeginInit();
            this.groupBoxMetadataCounts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVersioning)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBoxInformation
            // 
            this.richTextBoxInformation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.richTextBoxInformation.Location = new System.Drawing.Point(16, 629);
            this.richTextBoxInformation.Name = "richTextBoxInformation";
            this.richTextBoxInformation.Size = new System.Drawing.Size(489, 101);
            this.richTextBoxInformation.TabIndex = 2;
            this.richTextBoxInformation.Text = "";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.metadataToolStripMenuItem,
            this.businessKeyMetadataToolStripMenuItem,
            this.attributeMappingMetadataToolStripMenuItem,
            this.validationToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1337, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // metadataToolStripMenuItem
            // 
            this.metadataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveAsDirectionalGraphMarkupLanguageDGMLToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.metadataToolStripMenuItem.Name = "metadataToolStripMenuItem";
            this.metadataToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.metadataToolStripMenuItem.Text = "File";
            // 
            // saveAsDirectionalGraphMarkupLanguageDGMLToolStripMenuItem
            // 
            this.saveAsDirectionalGraphMarkupLanguageDGMLToolStripMenuItem.Image = global::Virtual_EDW.Properties.Resources.SaveFile;
            this.saveAsDirectionalGraphMarkupLanguageDGMLToolStripMenuItem.Name = "saveAsDirectionalGraphMarkupLanguageDGMLToolStripMenuItem";
            this.saveAsDirectionalGraphMarkupLanguageDGMLToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.saveAsDirectionalGraphMarkupLanguageDGMLToolStripMenuItem.Text = "Save as Directional Graph Markup Language (DGML)";
            this.saveAsDirectionalGraphMarkupLanguageDGMLToolStripMenuItem.Click += new System.EventHandler(this.saveAsDirectionalGraphMarkupLanguageDGMLToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Image = global::Virtual_EDW.Properties.Resources.ExitApplication;
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(350, 22);
            this.closeToolStripMenuItem.Text = "Close Window";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // businessKeyMetadataToolStripMenuItem
            // 
            this.businessKeyMetadataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openMetadataFileToolStripMenuItem,
            this.saveMetadataFileToolStripMenuItem});
            this.businessKeyMetadataToolStripMenuItem.Name = "businessKeyMetadataToolStripMenuItem";
            this.businessKeyMetadataToolStripMenuItem.Size = new System.Drawing.Size(151, 20);
            this.businessKeyMetadataToolStripMenuItem.Text = "Table Mapping Metadata";
            // 
            // openMetadataFileToolStripMenuItem
            // 
            this.openMetadataFileToolStripMenuItem.Image = global::Virtual_EDW.Properties.Resources.OpenFileIcon;
            this.openMetadataFileToolStripMenuItem.Name = "openMetadataFileToolStripMenuItem";
            this.openMetadataFileToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
            this.openMetadataFileToolStripMenuItem.Text = "Open Business Key Metadata File";
            this.openMetadataFileToolStripMenuItem.Click += new System.EventHandler(this.openMetadataFileToolStripMenuItem_Click_1);
            // 
            // saveMetadataFileToolStripMenuItem
            // 
            this.saveMetadataFileToolStripMenuItem.Image = global::Virtual_EDW.Properties.Resources.SaveFile;
            this.saveMetadataFileToolStripMenuItem.Name = "saveMetadataFileToolStripMenuItem";
            this.saveMetadataFileToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
            this.saveMetadataFileToolStripMenuItem.Text = "Save Business Key Metadata File";
            this.saveMetadataFileToolStripMenuItem.Click += new System.EventHandler(this.saveBusinessKeyMetadataFileToolStripMenuItem_Click);
            // 
            // attributeMappingMetadataToolStripMenuItem
            // 
            this.attributeMappingMetadataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
            this.attributeMappingMetadataToolStripMenuItem.Name = "attributeMappingMetadataToolStripMenuItem";
            this.attributeMappingMetadataToolStripMenuItem.Size = new System.Drawing.Size(170, 20);
            this.attributeMappingMetadataToolStripMenuItem.Text = "Attribute Mapping Metadata";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Image = global::Virtual_EDW.Properties.Resources.OpenFileIcon;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(225, 22);
            this.toolStripMenuItem1.Text = "Open Attribute Mapping File";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.OpenAttributeFileMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Image = global::Virtual_EDW.Properties.Resources.SaveFile;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(225, 22);
            this.toolStripMenuItem2.Text = "Save Attribute Mapping File";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.saveAttributeMetadataMenuItem_Click);
            // 
            // validationToolStripMenuItem
            // 
            this.validationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manageValidationRulesToolStripMenuItem});
            this.validationToolStripMenuItem.Name = "validationToolStripMenuItem";
            this.validationToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.validationToolStripMenuItem.Text = "Validation";
            // 
            // manageValidationRulesToolStripMenuItem
            // 
            this.manageValidationRulesToolStripMenuItem.Image = global::Virtual_EDW.Properties.Resources.DocumentationIcon;
            this.manageValidationRulesToolStripMenuItem.Name = "manageValidationRulesToolStripMenuItem";
            this.manageValidationRulesToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.manageValidationRulesToolStripMenuItem.Text = "Manage validation rules";
            this.manageValidationRulesToolStripMenuItem.Click += new System.EventHandler(this.manageValidationRulesToolStripMenuItem_Click);
            // 
            // labelInformation
            // 
            this.labelInformation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelInformation.AutoSize = true;
            this.labelInformation.Location = new System.Drawing.Point(13, 613);
            this.labelInformation.Name = "labelInformation";
            this.labelInformation.Size = new System.Drawing.Size(59, 13);
            this.labelInformation.TabIndex = 5;
            this.labelInformation.Text = "Information";
            // 
            // MetadataGenerationGroupBox
            // 
            this.MetadataGenerationGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MetadataGenerationGroupBox.Controls.Add(this.checkBoxIgnoreVersion);
            this.MetadataGenerationGroupBox.Controls.Add(this.checkBox1);
            this.MetadataGenerationGroupBox.Controls.Add(this.checkBoxClearMetadata);
            this.MetadataGenerationGroupBox.Location = new System.Drawing.Point(1023, 215);
            this.MetadataGenerationGroupBox.Name = "MetadataGenerationGroupBox";
            this.MetadataGenerationGroupBox.Size = new System.Drawing.Size(226, 92);
            this.MetadataGenerationGroupBox.TabIndex = 3;
            this.MetadataGenerationGroupBox.TabStop = false;
            this.MetadataGenerationGroupBox.Text = "Metadata generation options";
            // 
            // checkBoxIgnoreVersion
            // 
            this.checkBoxIgnoreVersion.AutoSize = true;
            this.checkBoxIgnoreVersion.Checked = true;
            this.checkBoxIgnoreVersion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxIgnoreVersion.Location = new System.Drawing.Point(6, 65);
            this.checkBoxIgnoreVersion.Name = "checkBoxIgnoreVersion";
            this.checkBoxIgnoreVersion.Size = new System.Drawing.Size(219, 17);
            this.checkBoxIgnoreVersion.TabIndex = 25;
            this.checkBoxIgnoreVersion.Text = "Use live database / ignore model version";
            this.checkBoxIgnoreVersion.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(6, 42);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(164, 17);
            this.checkBox1.TabIndex = 10;
            this.checkBox1.Text = "Validate generation metadata";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBoxClearMetadata
            // 
            this.checkBoxClearMetadata.AutoSize = true;
            this.checkBoxClearMetadata.Location = new System.Drawing.Point(6, 19);
            this.checkBoxClearMetadata.Name = "checkBoxClearMetadata";
            this.checkBoxClearMetadata.Size = new System.Drawing.Size(150, 17);
            this.checkBoxClearMetadata.TabIndex = 9;
            this.checkBoxClearMetadata.Text = "Clear generation metadata";
            this.checkBoxClearMetadata.UseVisualStyleBackColor = true;
            this.checkBoxClearMetadata.CheckedChanged += new System.EventHandler(this.checkBoxClearMetadata_CheckedChanged);
            // 
            // outputGroupBoxVersioning
            // 
            this.outputGroupBoxVersioning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputGroupBoxVersioning.Controls.Add(this.radioButtonMinorRelease);
            this.outputGroupBoxVersioning.Controls.Add(this.radiobuttonMajorRelease);
            this.outputGroupBoxVersioning.Controls.Add(this.radiobuttonNoVersionChange);
            this.outputGroupBoxVersioning.Location = new System.Drawing.Point(1149, 49);
            this.outputGroupBoxVersioning.Name = "outputGroupBoxVersioning";
            this.outputGroupBoxVersioning.Size = new System.Drawing.Size(176, 95);
            this.outputGroupBoxVersioning.TabIndex = 2;
            this.outputGroupBoxVersioning.TabStop = false;
            this.outputGroupBoxVersioning.Text = "Versioning";
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
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(16, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1001, 583);
            this.tabControl1.TabIndex = 15;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridViewTableMetadata);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(993, 557);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Table Mappings";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dataGridViewTableMetadata
            // 
            this.dataGridViewTableMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewTableMetadata.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTableMetadata.Location = new System.Drawing.Point(2, 3);
            this.dataGridViewTableMetadata.MinimumSize = new System.Drawing.Size(964, 511);
            this.dataGridViewTableMetadata.Name = "dataGridViewTableMetadata";
            this.dataGridViewTableMetadata.Size = new System.Drawing.Size(988, 551);
            this.dataGridViewTableMetadata.TabIndex = 1;
            this.dataGridViewTableMetadata.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridViewTableMetadata_CellValidating);
            this.dataGridViewTableMetadata.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DataGridViewTableMetadataKeyDown);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridViewAttributeMetadata);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(993, 557);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Attribute Mappings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridViewAttributeMetadata
            // 
            this.dataGridViewAttributeMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewAttributeMetadata.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAttributeMetadata.Location = new System.Drawing.Point(2, 3);
            this.dataGridViewAttributeMetadata.Name = "dataGridViewAttributeMetadata";
            this.dataGridViewAttributeMetadata.Size = new System.Drawing.Size(964, 557);
            this.dataGridViewAttributeMetadata.TabIndex = 1;
            this.dataGridViewAttributeMetadata.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DataGridViewAttributeMetadataKeyDown);
            // 
            // groupBoxMetadataCounts
            // 
            this.groupBoxMetadataCounts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxMetadataCounts.Controls.Add(this.labelLsatCount);
            this.groupBoxMetadataCounts.Controls.Add(this.labelLnkCount);
            this.groupBoxMetadataCounts.Controls.Add(this.labelSatCount);
            this.groupBoxMetadataCounts.Controls.Add(this.labelHubCount);
            this.groupBoxMetadataCounts.Location = new System.Drawing.Point(788, 622);
            this.groupBoxMetadataCounts.Name = "groupBoxMetadataCounts";
            this.groupBoxMetadataCounts.Size = new System.Drawing.Size(225, 82);
            this.groupBoxMetadataCounts.TabIndex = 16;
            this.groupBoxMetadataCounts.TabStop = false;
            this.groupBoxMetadataCounts.Text = "This metadata contains:";
            // 
            // labelLsatCount
            // 
            this.labelLsatCount.AutoSize = true;
            this.labelLsatCount.Location = new System.Drawing.Point(6, 59);
            this.labelLsatCount.Name = "labelLsatCount";
            this.labelLsatCount.Size = new System.Drawing.Size(77, 13);
            this.labelLsatCount.TabIndex = 3;
            this.labelLsatCount.Text = "labelLsatCount";
            // 
            // labelLnkCount
            // 
            this.labelLnkCount.AutoSize = true;
            this.labelLnkCount.Location = new System.Drawing.Point(6, 46);
            this.labelLnkCount.Name = "labelLnkCount";
            this.labelLnkCount.Size = new System.Drawing.Size(75, 13);
            this.labelLnkCount.TabIndex = 2;
            this.labelLnkCount.Text = "labelLnkCount";
            // 
            // labelSatCount
            // 
            this.labelSatCount.AutoSize = true;
            this.labelSatCount.Location = new System.Drawing.Point(6, 33);
            this.labelSatCount.Name = "labelSatCount";
            this.labelSatCount.Size = new System.Drawing.Size(73, 13);
            this.labelSatCount.TabIndex = 1;
            this.labelSatCount.Text = "labelSatCount";
            // 
            // labelHubCount
            // 
            this.labelHubCount.AutoSize = true;
            this.labelHubCount.Location = new System.Drawing.Point(6, 20);
            this.labelHubCount.Name = "labelHubCount";
            this.labelHubCount.Size = new System.Drawing.Size(77, 13);
            this.labelHubCount.TabIndex = 0;
            this.labelHubCount.Text = "labelHubCount";
            // 
            // trackBarVersioning
            // 
            this.trackBarVersioning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarVersioning.Location = new System.Drawing.Point(7, 23);
            this.trackBarVersioning.Name = "trackBarVersioning";
            this.trackBarVersioning.Size = new System.Drawing.Size(163, 45);
            this.trackBarVersioning.TabIndex = 4;
            this.trackBarVersioning.ValueChanged += new System.EventHandler(this.trackBarVersioning_ValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.labelVersion);
            this.groupBox1.Controls.Add(this.trackBarVersioning);
            this.groupBox1.Location = new System.Drawing.Point(1023, 431);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(178, 76);
            this.groupBox1.TabIndex = 18;
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
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(1023, 49);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 40);
            this.button1.TabIndex = 1;
            this.button1.Text = "Save Metadata Changes";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.buttonSubmitVersion_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Location = new System.Drawing.Point(1156, 529);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(178, 96);
            this.richTextBox1.TabIndex = 21;
            this.richTextBox1.Text = "Activation of the metadata will process / upload the selected version into the ac" +
    "tive tool (similar to the slides on the main screen). \n\nThis allows for testing " +
    "and troubleshooting.";
            // 
            // buttonStart
            // 
            this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStart.Location = new System.Drawing.Point(1023, 529);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(120, 40);
            this.buttonStart.TabIndex = 22;
            this.buttonStart.Text = "Activate Metadata";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // labelResult
            // 
            this.labelResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelResult.AutoSize = true;
            this.labelResult.Location = new System.Drawing.Point(1023, 572);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(38, 13);
            this.labelResult.TabIndex = 23;
            this.labelResult.Text = "Ready";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWorkMetadataActivation);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // buttonValidation
            // 
            this.buttonValidation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonValidation.Enabled = false;
            this.buttonValidation.Location = new System.Drawing.Point(1023, 95);
            this.buttonValidation.Name = "buttonValidation";
            this.buttonValidation.Size = new System.Drawing.Size(120, 40);
            this.buttonValidation.TabIndex = 24;
            this.buttonValidation.Text = "Validate Only";
            this.buttonValidation.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.textBoxFilterCriterion);
            this.groupBox2.Location = new System.Drawing.Point(531, 622);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(225, 43);
            this.groupBox2.TabIndex = 25;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Filter Criterion";
            // 
            // textBoxFilterCriterion
            // 
            this.textBoxFilterCriterion.Location = new System.Drawing.Point(6, 16);
            this.textBoxFilterCriterion.Name = "textBoxFilterCriterion";
            this.textBoxFilterCriterion.Size = new System.Drawing.Size(213, 20);
            this.textBoxFilterCriterion.TabIndex = 23;
            this.textBoxFilterCriterion.TextChanged += new System.EventHandler(this.textBoxFilterCriterion_TextChanged);
            // 
            // FormManageMetadata
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1337, 742);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonValidation);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBoxMetadataCounts);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.outputGroupBoxVersioning);
            this.Controls.Add(this.MetadataGenerationGroupBox);
            this.Controls.Add(this.labelInformation);
            this.Controls.Add(this.richTextBoxInformation);
            this.Controls.Add(this.menuStrip1);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(1353, 726);
            this.Name = "FormManageMetadata";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage the automation metadata";
            this.SizeChanged += new System.EventHandler(this.FormManageMetadata_SizeChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.MetadataGenerationGroupBox.ResumeLayout(false);
            this.MetadataGenerationGroupBox.PerformLayout();
            this.outputGroupBoxVersioning.ResumeLayout(false);
            this.outputGroupBoxVersioning.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTableMetadata)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAttributeMetadata)).EndInit();
            this.groupBoxMetadataCounts.ResumeLayout(false);
            this.groupBoxMetadataCounts.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVersioning)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBoxInformation;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem metadataToolStripMenuItem;
        private System.Windows.Forms.Label labelInformation;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.GroupBox MetadataGenerationGroupBox;
        private System.Windows.Forms.CheckBox checkBoxClearMetadata;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ToolStripMenuItem validationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageValidationRulesToolStripMenuItem;
        private System.Windows.Forms.GroupBox outputGroupBoxVersioning;
        private System.Windows.Forms.RadioButton radioButtonMinorRelease;
        private System.Windows.Forms.RadioButton radiobuttonMajorRelease;
        private System.Windows.Forms.RadioButton radiobuttonNoVersionChange;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private CustomDataGridViewAttribute dataGridViewAttributeMetadata;
        private CustomDataGridViewTable dataGridViewTableMetadata;
        private System.Windows.Forms.GroupBox groupBoxMetadataCounts;
        private System.Windows.Forms.Label labelHubCount;
        private System.Windows.Forms.Label labelLsatCount;
        private System.Windows.Forms.Label labelLnkCount;
        private System.Windows.Forms.Label labelSatCount;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripMenuItem businessKeyMetadataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMetadataFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMetadataFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem attributeMappingMetadataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Label labelResult;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        public System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.TrackBar trackBarVersioning;
        private System.Windows.Forms.Button buttonValidation;
        private System.Windows.Forms.CheckBox checkBoxIgnoreVersion;
        private System.Windows.Forms.ToolStripMenuItem saveAsDirectionalGraphMarkupLanguageDGMLToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxFilterCriterion;
    }
}