namespace Virtual_Data_Warehouse
{
    partial class FormDimensional
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDimensional));
            this.richTextBoxInformation = new System.Windows.Forms.RichTextBox();
            this.butHubs = new System.Windows.Forms.Button();
            this.dataGridAttributes = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.butGenerate = new System.Windows.Forms.Button();
            this.TargetPlatformGroupBox = new System.Windows.Forms.GroupBox();
            this.radioButtonPSA = new System.Windows.Forms.RadioButton();
            this.radioButtonIntegrationLayer = new System.Windows.Forms.RadioButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBoxCheck = new System.Windows.Forms.PictureBox();
            this.labelFirstHub = new System.Windows.Forms.Label();
            this.tabGenerate = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabHub = new System.Windows.Forms.TabPage();
            this.checkBoxSelectAllHubs = new System.Windows.Forms.CheckBox();
            this.dimTablControl = new System.Windows.Forms.TabControl();
            this.tabPathways = new System.Windows.Forms.TabPage();
            this.checkedListBoxPathways = new System.Windows.Forms.CheckedListBox();
            this.labelInformation = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxHideSystemAttributes = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelPathway = new System.Windows.Forms.Label();
            this.pictureBoxPathway = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonReset = new System.Windows.Forms.Button();
            this.checkBoxAllAttributes = new System.Windows.Forms.CheckBox();
            this.TableName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IncludeAttribute = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridAttributes)).BeginInit();
            this.TargetPlatformGroupBox.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCheck)).BeginInit();
            this.tabGenerate.SuspendLayout();
            this.tabHub.SuspendLayout();
            this.dimTablControl.SuspendLayout();
            this.tabPathways.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPathway)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // richTextBoxInformation
            // 
            this.richTextBoxInformation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxInformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxInformation.Location = new System.Drawing.Point(438, 578);
            this.richTextBoxInformation.Name = "richTextBoxInformation";
            this.richTextBoxInformation.Size = new System.Drawing.Size(734, 71);
            this.richTextBoxInformation.TabIndex = 10;
            this.richTextBoxInformation.Text = "";
            // 
            // butHubs
            // 
            this.butHubs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.butHubs.Location = new System.Drawing.Point(275, 285);
            this.butHubs.Name = "butHubs";
            this.butHubs.Size = new System.Drawing.Size(109, 40);
            this.butHubs.TabIndex = 1;
            this.butHubs.Text = "Repopulate Selection";
            this.butHubs.UseVisualStyleBackColor = true;
            this.butHubs.Click += new System.EventHandler(this.ButtonHubSelection);
            // 
            // dataGridAttributes
            // 
            this.dataGridAttributes.AllowUserToDeleteRows = false;
            this.dataGridAttributes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridAttributes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TableName,
            this.ColumnName,
            this.IncludeAttribute});
            this.dataGridAttributes.Location = new System.Drawing.Point(438, 49);
            this.dataGridAttributes.Name = "dataGridAttributes";
            this.dataGridAttributes.RowHeadersVisible = false;
            this.dataGridAttributes.ShowEditingIcon = false;
            this.dataGridAttributes.Size = new System.Drawing.Size(532, 347);
            this.dataGridAttributes.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(48, 406);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(442, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Select the first Hub table that is intended to become the lowest grain (base) of " +
    "the Dimension";
            // 
            // butGenerate
            // 
            this.butGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.butGenerate.Location = new System.Drawing.Point(20, 600);
            this.butGenerate.Name = "butGenerate";
            this.butGenerate.Size = new System.Drawing.Size(109, 40);
            this.butGenerate.TabIndex = 4;
            this.butGenerate.Text = "Generate Dimension";
            this.butGenerate.UseVisualStyleBackColor = true;
            // 
            // TargetPlatformGroupBox
            // 
            this.TargetPlatformGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TargetPlatformGroupBox.Controls.Add(this.radioButtonPSA);
            this.TargetPlatformGroupBox.Controls.Add(this.radioButtonIntegrationLayer);
            this.TargetPlatformGroupBox.Location = new System.Drawing.Point(986, 42);
            this.TargetPlatformGroupBox.Name = "TargetPlatformGroupBox";
            this.TargetPlatformGroupBox.Size = new System.Drawing.Size(183, 74);
            this.TargetPlatformGroupBox.TabIndex = 22;
            this.TargetPlatformGroupBox.TabStop = false;
            this.TargetPlatformGroupBox.Text = "Source Area";
            // 
            // radioButtonPSA
            // 
            this.radioButtonPSA.AutoSize = true;
            this.radioButtonPSA.Checked = true;
            this.radioButtonPSA.Location = new System.Drawing.Point(6, 19);
            this.radioButtonPSA.Name = "radioButtonPSA";
            this.radioButtonPSA.Size = new System.Drawing.Size(172, 17);
            this.radioButtonPSA.TabIndex = 3;
            this.radioButtonPSA.TabStop = true;
            this.radioButtonPSA.Text = "Persistent Staging Area (virtual)";
            this.radioButtonPSA.UseVisualStyleBackColor = true;
            // 
            // radioButtonIntegrationLayer
            // 
            this.radioButtonIntegrationLayer.AutoSize = true;
            this.radioButtonIntegrationLayer.Location = new System.Drawing.Point(6, 42);
            this.radioButtonIntegrationLayer.Name = "radioButtonIntegrationLayer";
            this.radioButtonIntegrationLayer.Size = new System.Drawing.Size(160, 17);
            this.radioButtonIntegrationLayer.TabIndex = 1;
            this.radioButtonIntegrationLayer.Text = "Presentation Layer (physical)";
            this.radioButtonIntegrationLayer.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1184, 24);
            this.menuStrip1.TabIndex = 23;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // pictureBoxCheck
            // 
            this.pictureBoxCheck.Image = global::Virtual_Data_Warehouse.Properties.Resources.transparent_green_checkmark_hi;
            this.pictureBoxCheck.Location = new System.Drawing.Point(22, 406);
            this.pictureBoxCheck.Name = "pictureBoxCheck";
            this.pictureBoxCheck.Size = new System.Drawing.Size(20, 24);
            this.pictureBoxCheck.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCheck.TabIndex = 24;
            this.pictureBoxCheck.TabStop = false;
            this.pictureBoxCheck.Visible = false;
            // 
            // labelFirstHub
            // 
            this.labelFirstHub.AutoSize = true;
            this.labelFirstHub.Location = new System.Drawing.Point(48, 421);
            this.labelFirstHub.Name = "labelFirstHub";
            this.labelFirstHub.Size = new System.Drawing.Size(68, 13);
            this.labelFirstHub.TabIndex = 25;
            this.labelFirstHub.Text = "labelFirstHub";
            this.labelFirstHub.Visible = false;
            // 
            // tabGenerate
            // 
            this.tabGenerate.Controls.Add(this.label2);
            this.tabGenerate.Controls.Add(this.label1);
            this.tabGenerate.Location = new System.Drawing.Point(4, 22);
            this.tabGenerate.Name = "tabGenerate";
            this.tabGenerate.Size = new System.Drawing.Size(399, 341);
            this.tabGenerate.TabIndex = 3;
            this.tabGenerate.Text = "Generation Output";
            this.tabGenerate.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Join";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Driving Hub";
            // 
            // tabHub
            // 
            this.tabHub.Controls.Add(this.butHubs);
            this.tabHub.Controls.Add(this.checkBoxSelectAllHubs);
            this.tabHub.Location = new System.Drawing.Point(4, 22);
            this.tabHub.Name = "tabHub";
            this.tabHub.Padding = new System.Windows.Forms.Padding(3);
            this.tabHub.Size = new System.Drawing.Size(399, 341);
            this.tabHub.TabIndex = 0;
            this.tabHub.Text = "First Hub";
            this.tabHub.UseVisualStyleBackColor = true;
            // 
            // checkBoxSelectAllHubs
            // 
            this.checkBoxSelectAllHubs.AutoSize = true;
            this.checkBoxSelectAllHubs.Location = new System.Drawing.Point(599, 20);
            this.checkBoxSelectAllHubs.Name = "checkBoxSelectAllHubs";
            this.checkBoxSelectAllHubs.Size = new System.Drawing.Size(69, 17);
            this.checkBoxSelectAllHubs.TabIndex = 21;
            this.checkBoxSelectAllHubs.Text = "Select all";
            this.checkBoxSelectAllHubs.UseVisualStyleBackColor = true;
            // 
            // dimTablControl
            // 
            this.dimTablControl.Controls.Add(this.tabHub);
            this.dimTablControl.Controls.Add(this.tabPathways);
            this.dimTablControl.Controls.Add(this.tabGenerate);
            this.dimTablControl.Location = new System.Drawing.Point(12, 27);
            this.dimTablControl.Name = "dimTablControl";
            this.dimTablControl.SelectedIndex = 0;
            this.dimTablControl.Size = new System.Drawing.Size(407, 373);
            this.dimTablControl.TabIndex = 1;
            // 
            // tabPathways
            // 
            this.tabPathways.Controls.Add(this.button1);
            this.tabPathways.Controls.Add(this.checkedListBoxPathways);
            this.tabPathways.Location = new System.Drawing.Point(4, 22);
            this.tabPathways.Name = "tabPathways";
            this.tabPathways.Size = new System.Drawing.Size(399, 347);
            this.tabPathways.TabIndex = 4;
            this.tabPathways.Text = "Pathways";
            this.tabPathways.UseVisualStyleBackColor = true;
            // 
            // checkedListBoxPathways
            // 
            this.checkedListBoxPathways.CheckOnClick = true;
            this.checkedListBoxPathways.FormattingEnabled = true;
            this.checkedListBoxPathways.Location = new System.Drawing.Point(3, 3);
            this.checkedListBoxPathways.Name = "checkedListBoxPathways";
            this.checkedListBoxPathways.Size = new System.Drawing.Size(393, 274);
            this.checkedListBoxPathways.TabIndex = 0;
            this.checkedListBoxPathways.SelectedIndexChanged += new System.EventHandler(this.checkedListBoxPathways_SelectedIndexChanged);
            // 
            // labelInformation
            // 
            this.labelInformation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelInformation.AutoSize = true;
            this.labelInformation.Location = new System.Drawing.Point(435, 562);
            this.labelInformation.Name = "labelInformation";
            this.labelInformation.Size = new System.Drawing.Size(59, 13);
            this.labelInformation.TabIndex = 26;
            this.labelInformation.Text = "Information";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(440, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 27;
            this.label4.Text = "Attributes";
            // 
            // checkBoxHideSystemAttributes
            // 
            this.checkBoxHideSystemAttributes.AutoSize = true;
            this.checkBoxHideSystemAttributes.Checked = true;
            this.checkBoxHideSystemAttributes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxHideSystemAttributes.Location = new System.Drawing.Point(6, 19);
            this.checkBoxHideSystemAttributes.Name = "checkBoxHideSystemAttributes";
            this.checkBoxHideSystemAttributes.Size = new System.Drawing.Size(172, 17);
            this.checkBoxHideSystemAttributes.TabIndex = 28;
            this.checkBoxHideSystemAttributes.Text = "Hide Process Control Attributes";
            this.checkBoxHideSystemAttributes.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.checkBoxHideSystemAttributes);
            this.groupBox1.Location = new System.Drawing.Point(986, 131);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(183, 74);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source Area";
            // 
            // labelPathway
            // 
            this.labelPathway.AutoSize = true;
            this.labelPathway.Location = new System.Drawing.Point(48, 458);
            this.labelPathway.Name = "labelPathway";
            this.labelPathway.Size = new System.Drawing.Size(70, 13);
            this.labelPathway.TabIndex = 32;
            this.labelPathway.Text = "labelPathway";
            this.labelPathway.Visible = false;
            // 
            // pictureBoxPathway
            // 
            this.pictureBoxPathway.Image = global::Virtual_Data_Warehouse.Properties.Resources.transparent_green_checkmark_hi;
            this.pictureBoxPathway.Location = new System.Drawing.Point(22, 443);
            this.pictureBoxPathway.Name = "pictureBoxPathway";
            this.pictureBoxPathway.Size = new System.Drawing.Size(20, 24);
            this.pictureBoxPathway.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPathway.TabIndex = 31;
            this.pictureBoxPathway.TabStop = false;
            this.pictureBoxPathway.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(48, 443);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(310, 13);
            this.label6.TabIndex = 30;
            this.label6.Text = "Select any subsequent business concepts (Hubs, through Links)";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::Virtual_Data_Warehouse.Properties.Resources.transparent_green_checkmark_hi;
            this.pictureBox2.Location = new System.Drawing.Point(22, 482);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(20, 24);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 34;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(48, 482);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(342, 13);
            this.label8.TabIndex = 33;
            this.label8.Text = "Select any attributes that are intended to become part of the Dimension";
            // 
            // buttonReset
            // 
            this.buttonReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonReset.Location = new System.Drawing.Point(135, 600);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(109, 40);
            this.buttonReset.TabIndex = 35;
            this.buttonReset.Text = "Reset Selection";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // checkBoxAllAttributes
            // 
            this.checkBoxAllAttributes.AutoSize = true;
            this.checkBoxAllAttributes.Location = new System.Drawing.Point(497, 32);
            this.checkBoxAllAttributes.Name = "checkBoxAllAttributes";
            this.checkBoxAllAttributes.Size = new System.Drawing.Size(69, 17);
            this.checkBoxAllAttributes.TabIndex = 36;
            this.checkBoxAllAttributes.Text = "Select all";
            this.checkBoxAllAttributes.UseVisualStyleBackColor = true;
            this.checkBoxAllAttributes.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // TableName
            // 
            this.TableName.HeaderText = "Table Name";
            this.TableName.Name = "TableName";
            this.TableName.ReadOnly = true;
            // 
            // ColumnName
            // 
            this.ColumnName.HeaderText = "Column Name";
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.ReadOnly = true;
            // 
            // IncludeAttribute
            // 
            this.IncludeAttribute.HeaderText = "Include";
            this.IncludeAttribute.Name = "IncludeAttribute";
            this.IncludeAttribute.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.IncludeAttribute.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(287, 292);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(109, 40);
            this.button1.TabIndex = 37;
            this.button1.Text = "Next Cycle";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // FormDimensional
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 661);
            this.Controls.Add(this.checkBoxAllAttributes);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.labelPathway);
            this.Controls.Add(this.pictureBoxPathway);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelInformation);
            this.Controls.Add(this.TargetPlatformGroupBox);
            this.Controls.Add(this.dataGridAttributes);
            this.Controls.Add(this.labelFirstHub);
            this.Controls.Add(this.richTextBoxInformation);
            this.Controls.Add(this.pictureBoxCheck);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.butGenerate);
            this.Controls.Add(this.dimTablControl);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(1200, 700);
            this.Name = "FormDimensional";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage the Dimensional Model";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridAttributes)).EndInit();
            this.TargetPlatformGroupBox.ResumeLayout(false);
            this.TargetPlatformGroupBox.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCheck)).EndInit();
            this.tabGenerate.ResumeLayout(false);
            this.tabGenerate.PerformLayout();
            this.tabHub.ResumeLayout(false);
            this.tabHub.PerformLayout();
            this.dimTablControl.ResumeLayout(false);
            this.tabPathways.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPathway)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridAttributes;
        private System.Windows.Forms.Button butGenerate;
        private System.Windows.Forms.Button butHubs;
        private System.Windows.Forms.GroupBox TargetPlatformGroupBox;
        private System.Windows.Forms.RadioButton radioButtonPSA;
        private System.Windows.Forms.RadioButton radioButtonIntegrationLayer;
        private System.Windows.Forms.RichTextBox richTextBoxInformation;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBoxCheck;
        private System.Windows.Forms.Label labelFirstHub;
        private System.Windows.Forms.TabPage tabGenerate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabHub;
        private System.Windows.Forms.CheckBox checkBoxSelectAllHubs;
        private System.Windows.Forms.TabControl dimTablControl;
        private System.Windows.Forms.Label labelInformation;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBoxHideSystemAttributes;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabPage tabPathways;
        private System.Windows.Forms.Label labelPathway;
        private System.Windows.Forms.PictureBox pictureBoxPathway;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.CheckedListBox checkedListBoxPathways;
        private System.Windows.Forms.CheckBox checkBoxAllAttributes;
        private System.Windows.Forms.DataGridViewTextBoxColumn TableName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IncludeAttribute;
        private System.Windows.Forms.Button button1;
    }
}