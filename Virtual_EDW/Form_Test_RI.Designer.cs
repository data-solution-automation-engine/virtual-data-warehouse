namespace Virtual_Data_Warehouse
{
    partial class FormTestRi
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTestRi));
            this.buttonGenerateTestcases = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.richTextBoxOutput = new System.Windows.Forms.RichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TargetPlatformGroupBox = new System.Windows.Forms.GroupBox();
            this.radioButtonPSA = new System.Windows.Forms.RadioButton();
            this.radioButtonIntegrationLayer = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonFullValidation = new System.Windows.Forms.RadioButton();
            this.radioButtonDeltaValidation = new System.Windows.Forms.RadioButton();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.richTextBoxInformationMain = new System.Windows.Forms.RichTextBox();
            this.menuStrip1.SuspendLayout();
            this.TargetPlatformGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonGenerateTestcases
            // 
            this.buttonGenerateTestcases.Location = new System.Drawing.Point(25, 467);
            this.buttonGenerateTestcases.Name = "buttonGenerateTestcases";
            this.buttonGenerateTestcases.Size = new System.Drawing.Size(109, 40);
            this.buttonGenerateTestcases.TabIndex = 19;
            this.buttonGenerateTestcases.Text = "Generate Referential Integrity Test";
            this.buttonGenerateTestcases.UseVisualStyleBackColor = true;
            this.buttonGenerateTestcases.Click += new System.EventHandler(this.buttonGenerateTestcases_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(211, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Output";
            // 
            // richTextBoxOutput
            // 
            this.richTextBoxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxOutput.Location = new System.Drawing.Point(214, 34);
            this.richTextBoxOutput.Name = "richTextBoxOutput";
            this.richTextBoxOutput.Size = new System.Drawing.Size(615, 357);
            this.richTextBoxOutput.TabIndex = 17;
            this.richTextBoxOutput.Text = "";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(841, 24);
            this.menuStrip1.TabIndex = 20;
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
            // TargetPlatformGroupBox
            // 
            this.TargetPlatformGroupBox.Controls.Add(this.radioButtonPSA);
            this.TargetPlatformGroupBox.Controls.Add(this.radioButtonIntegrationLayer);
            this.TargetPlatformGroupBox.Location = new System.Drawing.Point(25, 28);
            this.TargetPlatformGroupBox.Name = "TargetPlatformGroupBox";
            this.TargetPlatformGroupBox.Size = new System.Drawing.Size(171, 74);
            this.TargetPlatformGroupBox.TabIndex = 21;
            this.TargetPlatformGroupBox.TabStop = false;
            this.TargetPlatformGroupBox.Text = "Target Area";
            // 
            // radioButtonPSA
            // 
            this.radioButtonPSA.AutoSize = true;
            this.radioButtonPSA.Location = new System.Drawing.Point(6, 19);
            this.radioButtonPSA.Name = "radioButtonPSA";
            this.radioButtonPSA.Size = new System.Drawing.Size(147, 17);
            this.radioButtonPSA.TabIndex = 3;
            this.radioButtonPSA.Text = "Persistent Staging (virtual)";
            this.radioButtonPSA.UseVisualStyleBackColor = true;
            // 
            // radioButtonIntegrationLayer
            // 
            this.radioButtonIntegrationLayer.AutoSize = true;
            this.radioButtonIntegrationLayer.Checked = true;
            this.radioButtonIntegrationLayer.Location = new System.Drawing.Point(6, 42);
            this.radioButtonIntegrationLayer.Name = "radioButtonIntegrationLayer";
            this.radioButtonIntegrationLayer.Size = new System.Drawing.Size(151, 17);
            this.radioButtonIntegrationLayer.TabIndex = 1;
            this.radioButtonIntegrationLayer.TabStop = true;
            this.radioButtonIntegrationLayer.Text = "Integration Layer (physical)";
            this.radioButtonIntegrationLayer.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonFullValidation);
            this.groupBox1.Controls.Add(this.radioButtonDeltaValidation);
            this.groupBox1.Location = new System.Drawing.Point(25, 119);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(171, 74);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Validation Scope";
            // 
            // radioButtonFullValidation
            // 
            this.radioButtonFullValidation.AutoSize = true;
            this.radioButtonFullValidation.Location = new System.Drawing.Point(6, 19);
            this.radioButtonFullValidation.Name = "radioButtonFullValidation";
            this.radioButtonFullValidation.Size = new System.Drawing.Size(128, 17);
            this.radioButtonFullValidation.TabIndex = 3;
            this.radioButtonFullValidation.Text = "Data Warehouse (full)";
            this.radioButtonFullValidation.UseVisualStyleBackColor = true;
            // 
            // radioButtonDeltaValidation
            // 
            this.radioButtonDeltaValidation.AutoSize = true;
            this.radioButtonDeltaValidation.Checked = true;
            this.radioButtonDeltaValidation.Location = new System.Drawing.Point(6, 42);
            this.radioButtonDeltaValidation.Name = "radioButtonDeltaValidation";
            this.radioButtonDeltaValidation.Size = new System.Drawing.Size(118, 17);
            this.radioButtonDeltaValidation.TabIndex = 1;
            this.radioButtonDeltaValidation.TabStop = true;
            this.radioButtonDeltaValidation.Text = "Staging Area (delta)";
            this.radioButtonDeltaValidation.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            this.groupBox9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox9.Controls.Add(this.richTextBoxInformationMain);
            this.groupBox9.Location = new System.Drawing.Point(214, 409);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(615, 106);
            this.groupBox9.TabIndex = 92;
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
            // FormTestRi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(841, 528);
            this.Controls.Add(this.groupBox9);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.TargetPlatformGroupBox);
            this.Controls.Add(this.buttonGenerateTestcases);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.richTextBoxOutput);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(857, 566);
            this.Name = "FormTestRi";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Validate Referential Integrity";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.TargetPlatformGroupBox.ResumeLayout(false);
            this.TargetPlatformGroupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonGenerateTestcases;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RichTextBox richTextBoxOutput;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.GroupBox TargetPlatformGroupBox;
        private System.Windows.Forms.RadioButton radioButtonPSA;
        private System.Windows.Forms.RadioButton radioButtonIntegrationLayer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonFullValidation;
        private System.Windows.Forms.RadioButton radioButtonDeltaValidation;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.RichTextBox richTextBoxInformationMain;
    }
}