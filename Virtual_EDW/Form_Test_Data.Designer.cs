namespace Virtual_EDW
{
    partial class FormTestData
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTestData));
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxOutput = new System.Windows.Forms.RichTextBox();
            this.textBoxTestCaseAmount = new System.Windows.Forms.TextBox();
            this.TargetPlatformGroupBox = new System.Windows.Forms.GroupBox();
            this.radioButtonPSA = new System.Windows.Forms.RadioButton();
            this.radioButtonStagingArea = new System.Windows.Forms.RadioButton();
            this.radiobuttonSource = new System.Windows.Forms.RadioButton();
            this.SQLGenerationGroupBox = new System.Windows.Forms.GroupBox();
            this.checkBoxGenerateInDatabase = new System.Windows.Forms.CheckBox();
            this.checkBoxTruncate = new System.Windows.Forms.CheckBox();
            this.buttonGenerateTestcases = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBoxhashKeyoutput = new System.Windows.Forms.GroupBox();
            this.radioButtonCharacterHash = new System.Windows.Forms.RadioButton();
            this.radioButtonBinaryHash = new System.Windows.Forms.RadioButton();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.richTextBoxInformationMain = new System.Windows.Forms.RichTextBox();
            this.TargetPlatformGroupBox.SuspendLayout();
            this.SQLGenerationGroupBox.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBoxhashKeyoutput.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(211, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Output";
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxOutput.Location = new System.Drawing.Point(214, 34);
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.Size = new System.Drawing.Size(615, 357);
            this.textBoxOutput.TabIndex = 6;
            this.textBoxOutput.Text = "";
            // 
            // textBoxTestCaseAmount
            // 
            this.textBoxTestCaseAmount.Location = new System.Drawing.Point(6, 68);
            this.textBoxTestCaseAmount.Name = "textBoxTestCaseAmount";
            this.textBoxTestCaseAmount.Size = new System.Drawing.Size(143, 20);
            this.textBoxTestCaseAmount.TabIndex = 9;
            this.textBoxTestCaseAmount.Text = "25";
            // 
            // TargetPlatformGroupBox
            // 
            this.TargetPlatformGroupBox.Controls.Add(this.radioButtonPSA);
            this.TargetPlatformGroupBox.Controls.Add(this.radioButtonStagingArea);
            this.TargetPlatformGroupBox.Controls.Add(this.radiobuttonSource);
            this.TargetPlatformGroupBox.Location = new System.Drawing.Point(25, 28);
            this.TargetPlatformGroupBox.Name = "TargetPlatformGroupBox";
            this.TargetPlatformGroupBox.Size = new System.Drawing.Size(171, 99);
            this.TargetPlatformGroupBox.TabIndex = 12;
            this.TargetPlatformGroupBox.TabStop = false;
            this.TargetPlatformGroupBox.Text = "Target Area";
            // 
            // radioButtonPSA
            // 
            this.radioButtonPSA.AutoSize = true;
            this.radioButtonPSA.Enabled = false;
            this.radioButtonPSA.Location = new System.Drawing.Point(7, 69);
            this.radioButtonPSA.Name = "radioButtonPSA";
            this.radioButtonPSA.Size = new System.Drawing.Size(135, 17);
            this.radioButtonPSA.TabIndex = 3;
            this.radioButtonPSA.Text = "Persistent Staging Area";
            this.radioButtonPSA.UseVisualStyleBackColor = true;
            // 
            // radioButtonStagingArea
            // 
            this.radioButtonStagingArea.AutoSize = true;
            this.radioButtonStagingArea.Location = new System.Drawing.Point(7, 46);
            this.radioButtonStagingArea.Name = "radioButtonStagingArea";
            this.radioButtonStagingArea.Size = new System.Drawing.Size(86, 17);
            this.radioButtonStagingArea.TabIndex = 1;
            this.radioButtonStagingArea.Text = "Staging Area";
            this.radioButtonStagingArea.UseVisualStyleBackColor = true;
            // 
            // radiobuttonSource
            // 
            this.radiobuttonSource.AutoSize = true;
            this.radiobuttonSource.Enabled = false;
            this.radiobuttonSource.Location = new System.Drawing.Point(7, 23);
            this.radiobuttonSource.Name = "radiobuttonSource";
            this.radiobuttonSource.Size = new System.Drawing.Size(59, 17);
            this.radiobuttonSource.TabIndex = 0;
            this.radiobuttonSource.Text = "Source";
            this.radiobuttonSource.UseVisualStyleBackColor = true;
            // 
            // SQLGenerationGroupBox
            // 
            this.SQLGenerationGroupBox.Controls.Add(this.checkBoxGenerateInDatabase);
            this.SQLGenerationGroupBox.Controls.Add(this.textBoxTestCaseAmount);
            this.SQLGenerationGroupBox.Controls.Add(this.checkBoxTruncate);
            this.SQLGenerationGroupBox.Location = new System.Drawing.Point(25, 133);
            this.SQLGenerationGroupBox.Name = "SQLGenerationGroupBox";
            this.SQLGenerationGroupBox.Size = new System.Drawing.Size(171, 100);
            this.SQLGenerationGroupBox.TabIndex = 15;
            this.SQLGenerationGroupBox.TabStop = false;
            this.SQLGenerationGroupBox.Text = "SQL Generation Options";
            // 
            // checkBoxGenerateInDatabase
            // 
            this.checkBoxGenerateInDatabase.AutoSize = true;
            this.checkBoxGenerateInDatabase.Location = new System.Drawing.Point(6, 22);
            this.checkBoxGenerateInDatabase.Name = "checkBoxGenerateInDatabase";
            this.checkBoxGenerateInDatabase.Size = new System.Drawing.Size(128, 17);
            this.checkBoxGenerateInDatabase.TabIndex = 7;
            this.checkBoxGenerateInDatabase.Text = "Generate in database";
            this.checkBoxGenerateInDatabase.UseVisualStyleBackColor = true;
            // 
            // checkBoxTruncate
            // 
            this.checkBoxTruncate.AutoSize = true;
            this.checkBoxTruncate.Location = new System.Drawing.Point(6, 45);
            this.checkBoxTruncate.Name = "checkBoxTruncate";
            this.checkBoxTruncate.Size = new System.Drawing.Size(142, 17);
            this.checkBoxTruncate.TabIndex = 10;
            this.checkBoxTruncate.Text = "Add Truncate Statement";
            this.checkBoxTruncate.UseVisualStyleBackColor = true;
            // 
            // buttonGenerateTestcases
            // 
            this.buttonGenerateTestcases.Location = new System.Drawing.Point(25, 467);
            this.buttonGenerateTestcases.Name = "buttonGenerateTestcases";
            this.buttonGenerateTestcases.Size = new System.Drawing.Size(109, 40);
            this.buttonGenerateTestcases.TabIndex = 16;
            this.buttonGenerateTestcases.Text = "Generate Test Cases";
            this.buttonGenerateTestcases.UseVisualStyleBackColor = true;
            this.buttonGenerateTestcases.Click += new System.EventHandler(this.buttonGenerateTestcases_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(841, 24);
            this.menuStrip1.TabIndex = 17;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Image = global::Virtual_Data_Warehouse.Properties.Resources.ExitApplication;
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // groupBoxhashKeyoutput
            // 
            this.groupBoxhashKeyoutput.Controls.Add(this.radioButtonCharacterHash);
            this.groupBoxhashKeyoutput.Controls.Add(this.radioButtonBinaryHash);
            this.groupBoxhashKeyoutput.Location = new System.Drawing.Point(25, 239);
            this.groupBoxhashKeyoutput.Name = "groupBoxhashKeyoutput";
            this.groupBoxhashKeyoutput.Size = new System.Drawing.Size(171, 70);
            this.groupBoxhashKeyoutput.TabIndex = 89;
            this.groupBoxhashKeyoutput.TabStop = false;
            this.groupBoxhashKeyoutput.Text = "Hash key output";
            // 
            // radioButtonCharacterHash
            // 
            this.radioButtonCharacterHash.AutoSize = true;
            this.radioButtonCharacterHash.Location = new System.Drawing.Point(6, 42);
            this.radioButtonCharacterHash.Name = "radioButtonCharacterHash";
            this.radioButtonCharacterHash.Size = new System.Drawing.Size(71, 17);
            this.radioButtonCharacterHash.TabIndex = 1;
            this.radioButtonCharacterHash.Text = "Character";
            this.radioButtonCharacterHash.UseVisualStyleBackColor = true;
            // 
            // radioButtonBinaryHash
            // 
            this.radioButtonBinaryHash.AutoSize = true;
            this.radioButtonBinaryHash.Checked = true;
            this.radioButtonBinaryHash.Location = new System.Drawing.Point(6, 19);
            this.radioButtonBinaryHash.Name = "radioButtonBinaryHash";
            this.radioButtonBinaryHash.Size = new System.Drawing.Size(54, 17);
            this.radioButtonBinaryHash.TabIndex = 0;
            this.radioButtonBinaryHash.TabStop = true;
            this.radioButtonBinaryHash.Text = "Binary";
            this.radioButtonBinaryHash.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            this.groupBox9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox9.Controls.Add(this.richTextBoxInformationMain);
            this.groupBox9.Location = new System.Drawing.Point(214, 409);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(615, 106);
            this.groupBox9.TabIndex = 91;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Information";
            // 
            // richTextBoxInformationMain
            // 
            this.richTextBoxInformationMain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.richTextBoxInformationMain.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBoxInformationMain.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxInformationMain.Location = new System.Drawing.Point(6, 17);
            this.richTextBoxInformationMain.Name = "richTextBoxInformationMain";
            this.richTextBoxInformationMain.Size = new System.Drawing.Size(603, 82);
            this.richTextBoxInformationMain.TabIndex = 29;
            this.richTextBoxInformationMain.Text = "";
            // 
            // FormTestData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(841, 527);
            this.Controls.Add(this.groupBox9);
            this.Controls.Add(this.groupBoxhashKeyoutput);
            this.Controls.Add(this.buttonGenerateTestcases);
            this.Controls.Add(this.SQLGenerationGroupBox);
            this.Controls.Add(this.TargetPlatformGroupBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxOutput);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(857, 566);
            this.Name = "FormTestData";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Generate Test Data";
            this.TargetPlatformGroupBox.ResumeLayout(false);
            this.TargetPlatformGroupBox.PerformLayout();
            this.SQLGenerationGroupBox.ResumeLayout(false);
            this.SQLGenerationGroupBox.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBoxhashKeyoutput.ResumeLayout(false);
            this.groupBoxhashKeyoutput.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RichTextBox textBoxOutput;
        private System.Windows.Forms.TextBox textBoxTestCaseAmount;
        private System.Windows.Forms.GroupBox TargetPlatformGroupBox;
        private System.Windows.Forms.RadioButton radioButtonPSA;
        private System.Windows.Forms.RadioButton radioButtonStagingArea;
        private System.Windows.Forms.RadioButton radiobuttonSource;
        private System.Windows.Forms.GroupBox SQLGenerationGroupBox;
        private System.Windows.Forms.CheckBox checkBoxGenerateInDatabase;
        private System.Windows.Forms.CheckBox checkBoxTruncate;
        private System.Windows.Forms.Button buttonGenerateTestcases;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        internal System.Windows.Forms.GroupBox groupBoxhashKeyoutput;
        internal System.Windows.Forms.RadioButton radioButtonCharacterHash;
        internal System.Windows.Forms.RadioButton radioButtonBinaryHash;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.RichTextBox richTextBoxInformationMain;
    }
}