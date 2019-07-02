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
            this.DebuggingTextbox = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxTestCaseAmount = new System.Windows.Forms.TextBox();
            this.TargetPlatformGroupBox = new System.Windows.Forms.GroupBox();
            this.radioButtonPSA = new System.Windows.Forms.RadioButton();
            this.radioButtonStagingArea = new System.Windows.Forms.RadioButton();
            this.radiobuttonSource = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SQLGenerationGroupBox = new System.Windows.Forms.GroupBox();
            this.checkBoxGenerateInDatabase = new System.Windows.Forms.CheckBox();
            this.checkBoxTruncate = new System.Windows.Forms.CheckBox();
            this.buttonGenerateTestcases = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.richTextBoxOutput = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TargetPlatformGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SQLGenerationGroupBox.SuspendLayout();
            this.menuStrip1.SuspendLayout();
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
            // DebuggingTextbox
            // 
            this.DebuggingTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DebuggingTextbox.Location = new System.Drawing.Point(214, 34);
            this.DebuggingTextbox.Name = "DebuggingTextbox";
            this.DebuggingTextbox.Size = new System.Drawing.Size(615, 405);
            this.DebuggingTextbox.TabIndex = 6;
            this.DebuggingTextbox.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Define Number of Test Cases";
            // 
            // textBoxTestCaseAmount
            // 
            this.textBoxTestCaseAmount.Location = new System.Drawing.Point(7, 44);
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
            this.TargetPlatformGroupBox.Size = new System.Drawing.Size(171, 123);
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBoxTestCaseAmount);
            this.groupBox1.Location = new System.Drawing.Point(25, 159);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(171, 123);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Test Cases";
            // 
            // SQLGenerationGroupBox
            // 
            this.SQLGenerationGroupBox.Controls.Add(this.checkBoxGenerateInDatabase);
            this.SQLGenerationGroupBox.Controls.Add(this.checkBoxTruncate);
            this.SQLGenerationGroupBox.Location = new System.Drawing.Point(25, 288);
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
            this.buttonGenerateTestcases.Location = new System.Drawing.Point(25, 409);
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
            // richTextBoxOutput
            // 
            this.richTextBoxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxOutput.Location = new System.Drawing.Point(214, 467);
            this.richTextBoxOutput.Name = "richTextBoxOutput";
            this.richTextBoxOutput.Size = new System.Drawing.Size(615, 48);
            this.richTextBoxOutput.TabIndex = 18;
            this.richTextBoxOutput.Text = "";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(211, 451);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Information";
            // 
            // FormTestData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(841, 527);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.richTextBoxOutput);
            this.Controls.Add(this.buttonGenerateTestcases);
            this.Controls.Add(this.SQLGenerationGroupBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.TargetPlatformGroupBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.DebuggingTextbox);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(857, 566);
            this.Name = "FormTestData";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Generate Test Data";
            this.TargetPlatformGroupBox.ResumeLayout(false);
            this.TargetPlatformGroupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.SQLGenerationGroupBox.ResumeLayout(false);
            this.SQLGenerationGroupBox.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RichTextBox DebuggingTextbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxTestCaseAmount;
        private System.Windows.Forms.GroupBox TargetPlatformGroupBox;
        private System.Windows.Forms.RadioButton radioButtonPSA;
        private System.Windows.Forms.RadioButton radioButtonStagingArea;
        private System.Windows.Forms.RadioButton radiobuttonSource;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox SQLGenerationGroupBox;
        private System.Windows.Forms.CheckBox checkBoxGenerateInDatabase;
        private System.Windows.Forms.CheckBox checkBoxTruncate;
        private System.Windows.Forms.Button buttonGenerateTestcases;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.RichTextBox richTextBoxOutput;
        private System.Windows.Forms.Label label2;
    }
}