namespace Virtual_Data_Warehouse
{
    partial class FormAbout
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAbout));
            linkLabel1 = new System.Windows.Forms.LinkLabel();
            textBoxVDW = new System.Windows.Forms.TextBox();
            buttonClose = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new System.Drawing.Point(13, 32);
            linkLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new System.Drawing.Size(406, 13);
            linkLabel1.TabIndex = 25;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "https://github.com/data-solution-automation-engine/virtual-data-warehouse";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked_1;
            // 
            // textBoxVDW
            // 
            textBoxVDW.BorderStyle = System.Windows.Forms.BorderStyle.None;
            textBoxVDW.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            textBoxVDW.Location = new System.Drawing.Point(13, 12);
            textBoxVDW.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxVDW.Name = "textBoxVDW";
            textBoxVDW.Size = new System.Drawing.Size(308, 13);
            textBoxVDW.TabIndex = 23;
            textBoxVDW.TabStop = false;
            textBoxVDW.Text = "Virtual Data Warehouse";
            // 
            // buttonClose
            // 
            buttonClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonClose.Location = new System.Drawing.Point(344, 259);
            buttonClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonClose.Name = "buttonClose";
            buttonClose.Size = new System.Drawing.Size(127, 46);
            buttonClose.TabIndex = 22;
            buttonClose.Text = "Close";
            buttonClose.UseVisualStyleBackColor = true;
            buttonClose.Click += buttonClose_Click;
            // 
            // FormAbout
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.SystemColors.ControlLightLight;
            BackgroundImage = (System.Drawing.Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            ClientSize = new System.Drawing.Size(484, 317);
            ControlBox = false;
            Controls.Add(linkLabel1);
            Controls.Add(textBoxVDW);
            Controls.Add(buttonClose);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(500, 356);
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(500, 356);
            Name = "FormAbout";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "About";
            ResumeLayout(false);
            PerformLayout();
        }

        private void richTextBox1_LinkClicked(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.LinkLabel linkLabelTeam;
        private System.Windows.Forms.TextBox textBoxVDW;
        private System.Windows.Forms.Button buttonClose;
    }
}