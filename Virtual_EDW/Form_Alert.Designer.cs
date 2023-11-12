namespace Virtual_Data_Warehouse
{
    partial class FormAlert
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAlert));
            labelProgressMessage = new System.Windows.Forms.Label();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            buttonCancel = new System.Windows.Forms.Button();
            buttonClose = new System.Windows.Forms.Button();
            richTextBoxMetadataLog = new System.Windows.Forms.RichTextBox();
            buttonShowLog = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // labelProgressMessage
            // 
            labelProgressMessage.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            labelProgressMessage.AutoSize = true;
            labelProgressMessage.Location = new System.Drawing.Point(10, 526);
            labelProgressMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelProgressMessage.Name = "labelProgressMessage";
            labelProgressMessage.Size = new System.Drawing.Size(51, 13);
            labelProgressMessage.TabIndex = 0;
            labelProgressMessage.Text = "Progress";
            // 
            // progressBar1
            // 
            progressBar1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            progressBar1.Location = new System.Drawing.Point(14, 496);
            progressBar1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new System.Drawing.Size(418, 27);
            progressBar1.TabIndex = 1;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonCancel.Location = new System.Drawing.Point(541, 496);
            buttonCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(127, 46);
            buttonCancel.TabIndex = 2;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // buttonClose
            // 
            buttonClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonClose.Location = new System.Drawing.Point(810, 496);
            buttonClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonClose.Name = "buttonClose";
            buttonClose.Size = new System.Drawing.Size(127, 46);
            buttonClose.TabIndex = 3;
            buttonClose.Text = "Close";
            buttonClose.UseVisualStyleBackColor = true;
            buttonClose.Click += buttonClose_Click;
            // 
            // richTextBoxMetadataLog
            // 
            richTextBoxMetadataLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            richTextBoxMetadataLog.BackColor = System.Drawing.SystemColors.Info;
            richTextBoxMetadataLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            richTextBoxMetadataLog.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            richTextBoxMetadataLog.HideSelection = false;
            richTextBoxMetadataLog.Location = new System.Drawing.Point(14, 14);
            richTextBoxMetadataLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            richTextBoxMetadataLog.Name = "richTextBoxMetadataLog";
            richTextBoxMetadataLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            richTextBoxMetadataLog.Size = new System.Drawing.Size(922, 475);
            richTextBoxMetadataLog.TabIndex = 4;
            richTextBoxMetadataLog.Text = "";
            // 
            // buttonShowLog
            // 
            buttonShowLog.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonShowLog.Location = new System.Drawing.Point(676, 496);
            buttonShowLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonShowLog.Name = "buttonShowLog";
            buttonShowLog.Size = new System.Drawing.Size(127, 46);
            buttonShowLog.TabIndex = 5;
            buttonShowLog.Text = "Show Log File";
            buttonShowLog.UseVisualStyleBackColor = true;
            buttonShowLog.Click += buttonShowLog_Click;
            // 
            // FormAlert
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(951, 556);
            Controls.Add(buttonShowLog);
            Controls.Add(richTextBoxMetadataLog);
            Controls.Add(buttonClose);
            Controls.Add(buttonCancel);
            Controls.Add(progressBar1);
            Controls.Add(labelProgressMessage);
            Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            Name = "FormAlert";
            Text = "Processing Metadata";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label labelProgressMessage;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.RichTextBox richTextBoxMetadataLog;
        private System.Windows.Forms.Button buttonShowLog;
    }
}