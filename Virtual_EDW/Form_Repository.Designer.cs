namespace Virtual_EDW
{
    partial class FormManageRepository
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormManageRepository));
            this.buttonTruncate = new System.Windows.Forms.Button();
            this.buttonDeploy = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.labelResult = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonTruncate
            // 
            this.buttonTruncate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTruncate.Location = new System.Drawing.Point(12, 122);
            this.buttonTruncate.Name = "buttonTruncate";
            this.buttonTruncate.Size = new System.Drawing.Size(168, 51);
            this.buttonTruncate.TabIndex = 20;
            this.buttonTruncate.Text = "Truncate All Metadata";
            this.buttonTruncate.UseVisualStyleBackColor = true;
            this.buttonTruncate.Click += new System.EventHandler(this.buttonTruncate_Click);
            // 
            // buttonDeploy
            // 
            this.buttonDeploy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDeploy.Location = new System.Drawing.Point(12, 12);
            this.buttonDeploy.Name = "buttonDeploy";
            this.buttonDeploy.Size = new System.Drawing.Size(168, 51);
            this.buttonDeploy.TabIndex = 21;
            this.buttonDeploy.Text = "Deploy Metadata Repository";
            this.buttonDeploy.UseVisualStyleBackColor = true;
            this.buttonDeploy.Click += new System.EventHandler(this.buttonDeploy_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(240, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Create a new repository in the selected database.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 182);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(244, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Truncate all information in the metadata repository.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(212, 13);
            this.label4.TabIndex = 25;
            this.label4.Text = "WARNING - will remove existing tables first.";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // labelResult
            // 
            this.labelResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelResult.AutoSize = true;
            this.labelResult.Location = new System.Drawing.Point(186, 31);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(38, 13);
            this.labelResult.TabIndex = 30;
            this.labelResult.Text = "Ready";
            // 
            // FormManageRepository
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 261);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonDeploy);
            this.Controls.Add(this.buttonTruncate);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(350, 300);
            this.MinimumSize = new System.Drawing.Size(350, 300);
            this.Name = "FormManageRepository";
            this.Text = "Create / Rebuild Repository";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonTruncate;
        private System.Windows.Forms.Button buttonDeploy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label labelResult;
    }
}