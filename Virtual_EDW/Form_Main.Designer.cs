namespace Virtual_EDW
{
    partial class FormMain
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
        /// 
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.buttonGenerateHubs = new System.Windows.Forms.Button();
            this.richTextBoxInformation = new System.Windows.Forms.RichTextBox();
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.tabPageStaging = new System.Windows.Forms.TabPage();
            this.checkBoxExcludeLanding = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxFilterCriterionStg = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBoxSelectAllStg = new System.Windows.Forms.CheckBox();
            this.checkedListBoxStgMetadata = new System.Windows.Forms.CheckedListBox();
            this.label26 = new System.Windows.Forms.Label();
            this.richTextBoxStaging = new System.Windows.Forms.RichTextBox();
            this.buttonGenerateStaging = new System.Windows.Forms.Button();
            this.tabPagePSA = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxFilterCriterionPsa = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.checkBoxSelectAllPsa = new System.Windows.Forms.CheckBox();
            this.checkedListBoxPsaMetadata = new System.Windows.Forms.CheckedListBox();
            this.label20 = new System.Windows.Forms.Label();
            this.richTextBoxPSA = new System.Windows.Forms.RichTextBox();
            this.buttonGeneratePSA = new System.Windows.Forms.Button();
            this.tabPageHub = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBoxFilterCriterionHub = new System.Windows.Forms.TextBox();
            this.checkBoxSelectAllHubs = new System.Windows.Forms.CheckBox();
            this.buttonRepopulateHubs = new System.Windows.Forms.Button();
            this.checkedListBoxHubMetadata = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.richTextBoxHub = new System.Windows.Forms.RichTextBox();
            this.tabPageSat = new System.Windows.Forms.TabPage();
            this.checkBoxEvaluateSatDelete = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBoxFilterCriterionSat = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.checkBoxSelectAllSats = new System.Windows.Forms.CheckBox();
            this.checkedListBoxSatMetadata = new System.Windows.Forms.CheckedListBox();
            this.checkBoxDisableSatZeroRecords = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.richTextBoxSat = new System.Windows.Forms.RichTextBox();
            this.buttonGenerateSats = new System.Windows.Forms.Button();
            this.tabPageLink = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.textBoxFilterCriterionLnk = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.checkBoxSelectAllLinks = new System.Windows.Forms.CheckBox();
            this.checkedListBoxLinkMetadata = new System.Windows.Forms.CheckedListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.richTextBoxLink = new System.Windows.Forms.RichTextBox();
            this.buttonGenerateLinks = new System.Windows.Forms.Button();
            this.tabPageLinkSat = new System.Windows.Forms.TabPage();
            this.checkBoxEvaluateLsatDeletes = new System.Windows.Forms.CheckBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.textBoxFilterCriterionLsat = new System.Windows.Forms.TextBox();
            this.checkBoxDisableLsatZeroRecords = new System.Windows.Forms.CheckBox();
            this.button6 = new System.Windows.Forms.Button();
            this.checkBoxSelectAllLsats = new System.Windows.Forms.CheckBox();
            this.checkedListBoxLsatMetadata = new System.Windows.Forms.CheckedListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.richTextBoxLsat = new System.Windows.Forms.RichTextBox();
            this.buttonGenerateLsats = new System.Windows.Forms.Button();
            this.tabPageSettings = new System.Windows.Forms.TabPage();
            this.checkBoxUnicode = new System.Windows.Forms.CheckBox();
            this.groupBoxhashKeyoutput = new System.Windows.Forms.GroupBox();
            this.radioButtonCharacterHash = new System.Windows.Forms.RadioButton();
            this.radioButtonBinaryHash = new System.Windows.Forms.RadioButton();
            this.checkBoxDisableHash = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxConfigurationPath = new System.Windows.Forms.TextBox();
            this.OutputPathLabel = new System.Windows.Forms.Label();
            this.textBoxOutputPath = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.menuStripMainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openOutputDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openConfigurationDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveConfigurationFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dimensionalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rawDataMartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pointInTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unknownKeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateTestDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateRIValidationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.linksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label4 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.checkBoxGenerateInDatabase = new System.Windows.Forms.CheckBox();
            this.checkBoxSchemaBound = new System.Windows.Forms.CheckBox();
            this.checkBoxIfExistsStatement = new System.Windows.Forms.CheckBox();
            this.TargetPlatformGroupBox = new System.Windows.Forms.GroupBox();
            this.SQL2014Radiobutton = new System.Windows.Forms.RadioButton();
            this.OracleRadiobutton = new System.Windows.Forms.RadioButton();
            this.radiobuttonANSISQL = new System.Windows.Forms.RadioButton();
            this.OutputGroupBox = new System.Windows.Forms.GroupBox();
            this.radioButtonIntoStatement = new System.Windows.Forms.RadioButton();
            this.radiobuttonStoredProc = new System.Windows.Forms.RadioButton();
            this.radiobuttonViews = new System.Windows.Forms.RadioButton();
            this.SQLGenerationGroupBox = new System.Windows.Forms.GroupBox();
            this.checkBoxIgnoreVersion = new System.Windows.Forms.CheckBox();
            this.groupBoxVersionSelection = new System.Windows.Forms.GroupBox();
            this.labelVersion = new System.Windows.Forms.Label();
            this.trackBarVersioning = new System.Windows.Forms.TrackBar();
            this.backgroundWorkerActivateMetadata = new System.ComponentModel.BackgroundWorker();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.openTEAMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.MainTabControl.SuspendLayout();
            this.tabPageStaging.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPagePSA.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPageHub.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabPageSat.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabPageLink.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabPageLinkSat.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tabPageSettings.SuspendLayout();
            this.groupBoxhashKeyoutput.SuspendLayout();
            this.menuStripMainMenu.SuspendLayout();
            this.TargetPlatformGroupBox.SuspendLayout();
            this.OutputGroupBox.SuspendLayout();
            this.SQLGenerationGroupBox.SuspendLayout();
            this.groupBoxVersionSelection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVersioning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonGenerateHubs
            // 
            this.buttonGenerateHubs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonGenerateHubs.Location = new System.Drawing.Point(17, 432);
            this.buttonGenerateHubs.Name = "buttonGenerateHubs";
            this.buttonGenerateHubs.Size = new System.Drawing.Size(109, 40);
            this.buttonGenerateHubs.TabIndex = 0;
            this.buttonGenerateHubs.Text = "Generate Hubs";
            this.buttonGenerateHubs.UseVisualStyleBackColor = true;
            this.buttonGenerateHubs.Click += new System.EventHandler(this.HubButtonClick);
            // 
            // richTextBoxInformation
            // 
            this.richTextBoxInformation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxInformation.Location = new System.Drawing.Point(585, 66);
            this.richTextBoxInformation.Name = "richTextBoxInformation";
            this.richTextBoxInformation.Size = new System.Drawing.Size(765, 716);
            this.richTextBoxInformation.TabIndex = 2;
            this.richTextBoxInformation.Text = "";
            this.richTextBoxInformation.TextChanged += new System.EventHandler(this.richTextBoxInformation_TextChanged);
            // 
            // MainTabControl
            // 
            this.MainTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.MainTabControl.Controls.Add(this.tabPageStaging);
            this.MainTabControl.Controls.Add(this.tabPagePSA);
            this.MainTabControl.Controls.Add(this.tabPageHub);
            this.MainTabControl.Controls.Add(this.tabPageSat);
            this.MainTabControl.Controls.Add(this.tabPageLink);
            this.MainTabControl.Controls.Add(this.tabPageLinkSat);
            this.MainTabControl.Controls.Add(this.tabPageSettings);
            this.MainTabControl.Location = new System.Drawing.Point(12, 44);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(566, 517);
            this.MainTabControl.TabIndex = 3;
            // 
            // tabPageStaging
            // 
            this.tabPageStaging.Controls.Add(this.checkBoxExcludeLanding);
            this.tabPageStaging.Controls.Add(this.groupBox2);
            this.tabPageStaging.Controls.Add(this.button1);
            this.tabPageStaging.Controls.Add(this.checkBoxSelectAllStg);
            this.tabPageStaging.Controls.Add(this.checkedListBoxStgMetadata);
            this.tabPageStaging.Controls.Add(this.label26);
            this.tabPageStaging.Controls.Add(this.richTextBoxStaging);
            this.tabPageStaging.Controls.Add(this.buttonGenerateStaging);
            this.tabPageStaging.Location = new System.Drawing.Point(4, 22);
            this.tabPageStaging.Name = "tabPageStaging";
            this.tabPageStaging.Size = new System.Drawing.Size(558, 491);
            this.tabPageStaging.TabIndex = 11;
            this.tabPageStaging.Text = "Staging Area";
            this.tabPageStaging.UseVisualStyleBackColor = true;
            // 
            // checkBoxExcludeLanding
            // 
            this.checkBoxExcludeLanding.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxExcludeLanding.AutoSize = true;
            this.checkBoxExcludeLanding.Checked = true;
            this.checkBoxExcludeLanding.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxExcludeLanding.Location = new System.Drawing.Point(416, 432);
            this.checkBoxExcludeLanding.Name = "checkBoxExcludeLanding";
            this.checkBoxExcludeLanding.Size = new System.Drawing.Size(140, 17);
            this.checkBoxExcludeLanding.TabIndex = 25;
            this.checkBoxExcludeLanding.Text = "Exclude Landing Tables";
            this.checkBoxExcludeLanding.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.textBoxFilterCriterionStg);
            this.groupBox2.Location = new System.Drawing.Point(247, 429);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(163, 43);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Filter Criterion";
            // 
            // textBoxFilterCriterionStg
            // 
            this.textBoxFilterCriterionStg.Location = new System.Drawing.Point(6, 16);
            this.textBoxFilterCriterionStg.Name = "textBoxFilterCriterionStg";
            this.textBoxFilterCriterionStg.Size = new System.Drawing.Size(151, 20);
            this.textBoxFilterCriterionStg.TabIndex = 23;
            this.textBoxFilterCriterionStg.TextChanged += new System.EventHandler(this.button_Repopulate_STG);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(132, 432);
            this.button1.MinimumSize = new System.Drawing.Size(109, 40);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(109, 40);
            this.button1.TabIndex = 22;
            this.button1.Text = "Repopulate Selection";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button_Repopulate_STG);
            // 
            // checkBoxSelectAllStg
            // 
            this.checkBoxSelectAllStg.AutoSize = true;
            this.checkBoxSelectAllStg.Location = new System.Drawing.Point(140, 11);
            this.checkBoxSelectAllStg.Name = "checkBoxSelectAllStg";
            this.checkBoxSelectAllStg.Size = new System.Drawing.Size(69, 17);
            this.checkBoxSelectAllStg.TabIndex = 21;
            this.checkBoxSelectAllStg.Text = "Select all";
            this.checkBoxSelectAllStg.UseVisualStyleBackColor = true;
            this.checkBoxSelectAllStg.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged_1);
            // 
            // checkedListBoxStgMetadata
            // 
            this.checkedListBoxStgMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.checkedListBoxStgMetadata.CheckOnClick = true;
            this.checkedListBoxStgMetadata.ColumnWidth = 261;
            this.checkedListBoxStgMetadata.FormattingEnabled = true;
            this.checkedListBoxStgMetadata.Location = new System.Drawing.Point(17, 31);
            this.checkedListBoxStgMetadata.MultiColumn = true;
            this.checkedListBoxStgMetadata.Name = "checkedListBoxStgMetadata";
            this.checkedListBoxStgMetadata.Size = new System.Drawing.Size(523, 274);
            this.checkedListBoxStgMetadata.TabIndex = 10;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(14, 12);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(123, 13);
            this.label26.TabIndex = 6;
            this.label26.Text = "Staging Area Processing";
            // 
            // richTextBoxStaging
            // 
            this.richTextBoxStaging.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.richTextBoxStaging.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxStaging.Location = new System.Drawing.Point(17, 308);
            this.richTextBoxStaging.Name = "richTextBoxStaging";
            this.richTextBoxStaging.Size = new System.Drawing.Size(523, 115);
            this.richTextBoxStaging.TabIndex = 4;
            this.richTextBoxStaging.Text = "";
            // 
            // buttonGenerateStaging
            // 
            this.buttonGenerateStaging.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonGenerateStaging.Location = new System.Drawing.Point(17, 432);
            this.buttonGenerateStaging.Name = "buttonGenerateStaging";
            this.buttonGenerateStaging.Size = new System.Drawing.Size(109, 40);
            this.buttonGenerateStaging.TabIndex = 5;
            this.buttonGenerateStaging.Text = "Generate Staging";
            this.buttonGenerateStaging.UseVisualStyleBackColor = true;
            this.buttonGenerateStaging.Click += new System.EventHandler(this.buttonGenerateStaging_Click);
            // 
            // tabPagePSA
            // 
            this.tabPagePSA.Controls.Add(this.groupBox3);
            this.tabPagePSA.Controls.Add(this.button2);
            this.tabPagePSA.Controls.Add(this.checkBoxSelectAllPsa);
            this.tabPagePSA.Controls.Add(this.checkedListBoxPsaMetadata);
            this.tabPagePSA.Controls.Add(this.label20);
            this.tabPagePSA.Controls.Add(this.richTextBoxPSA);
            this.tabPagePSA.Controls.Add(this.buttonGeneratePSA);
            this.tabPagePSA.Location = new System.Drawing.Point(4, 22);
            this.tabPagePSA.Name = "tabPagePSA";
            this.tabPagePSA.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePSA.Size = new System.Drawing.Size(558, 491);
            this.tabPagePSA.TabIndex = 9;
            this.tabPagePSA.Text = "PSA";
            this.tabPagePSA.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.textBoxFilterCriterionPsa);
            this.groupBox3.Location = new System.Drawing.Point(247, 429);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(163, 43);
            this.groupBox3.TabIndex = 25;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Filter Criterion";
            // 
            // textBoxFilterCriterionPsa
            // 
            this.textBoxFilterCriterionPsa.Location = new System.Drawing.Point(6, 16);
            this.textBoxFilterCriterionPsa.Name = "textBoxFilterCriterionPsa";
            this.textBoxFilterCriterionPsa.Size = new System.Drawing.Size(151, 20);
            this.textBoxFilterCriterionPsa.TabIndex = 23;
            this.textBoxFilterCriterionPsa.TextChanged += new System.EventHandler(this.button_Repopulate_PSA);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(132, 432);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(109, 40);
            this.button2.TabIndex = 22;
            this.button2.Text = "Repopulate Selection";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button_Repopulate_PSA);
            // 
            // checkBoxSelectAllPsa
            // 
            this.checkBoxSelectAllPsa.AutoSize = true;
            this.checkBoxSelectAllPsa.Location = new System.Drawing.Point(219, 11);
            this.checkBoxSelectAllPsa.Name = "checkBoxSelectAllPsa";
            this.checkBoxSelectAllPsa.Size = new System.Drawing.Size(69, 17);
            this.checkBoxSelectAllPsa.TabIndex = 21;
            this.checkBoxSelectAllPsa.Text = "Select all";
            this.checkBoxSelectAllPsa.UseVisualStyleBackColor = true;
            this.checkBoxSelectAllPsa.CheckedChanged += new System.EventHandler(this.checkBoxSelectAllPsa_CheckedChanged);
            // 
            // checkedListBoxPsaMetadata
            // 
            this.checkedListBoxPsaMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.checkedListBoxPsaMetadata.CheckOnClick = true;
            this.checkedListBoxPsaMetadata.ColumnWidth = 261;
            this.checkedListBoxPsaMetadata.FormattingEnabled = true;
            this.checkedListBoxPsaMetadata.Location = new System.Drawing.Point(17, 31);
            this.checkedListBoxPsaMetadata.MultiColumn = true;
            this.checkedListBoxPsaMetadata.Name = "checkedListBoxPsaMetadata";
            this.checkedListBoxPsaMetadata.Size = new System.Drawing.Size(523, 274);
            this.checkedListBoxPsaMetadata.TabIndex = 10;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(14, 12);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(202, 13);
            this.label20.TabIndex = 4;
            this.label20.Text = "Persistent Staging Area (PSA) Processing";
            // 
            // richTextBoxPSA
            // 
            this.richTextBoxPSA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.richTextBoxPSA.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxPSA.Location = new System.Drawing.Point(17, 308);
            this.richTextBoxPSA.Name = "richTextBoxPSA";
            this.richTextBoxPSA.Size = new System.Drawing.Size(523, 115);
            this.richTextBoxPSA.TabIndex = 2;
            this.richTextBoxPSA.Text = "";
            // 
            // buttonGeneratePSA
            // 
            this.buttonGeneratePSA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonGeneratePSA.Location = new System.Drawing.Point(17, 432);
            this.buttonGeneratePSA.Name = "buttonGeneratePSA";
            this.buttonGeneratePSA.Size = new System.Drawing.Size(109, 40);
            this.buttonGeneratePSA.TabIndex = 3;
            this.buttonGeneratePSA.Text = "Generate PSA";
            this.buttonGeneratePSA.UseVisualStyleBackColor = true;
            this.buttonGeneratePSA.Click += new System.EventHandler(this.buttonGeneratePSA_Click);
            // 
            // tabPageHub
            // 
            this.tabPageHub.Controls.Add(this.groupBox4);
            this.tabPageHub.Controls.Add(this.checkBoxSelectAllHubs);
            this.tabPageHub.Controls.Add(this.buttonRepopulateHubs);
            this.tabPageHub.Controls.Add(this.checkedListBoxHubMetadata);
            this.tabPageHub.Controls.Add(this.label1);
            this.tabPageHub.Controls.Add(this.richTextBoxHub);
            this.tabPageHub.Controls.Add(this.buttonGenerateHubs);
            this.tabPageHub.Location = new System.Drawing.Point(4, 22);
            this.tabPageHub.Name = "tabPageHub";
            this.tabPageHub.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageHub.Size = new System.Drawing.Size(558, 491);
            this.tabPageHub.TabIndex = 0;
            this.tabPageHub.Text = "Hubs";
            this.tabPageHub.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox4.Controls.Add(this.textBoxFilterCriterionHub);
            this.groupBox4.Location = new System.Drawing.Point(247, 429);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(163, 43);
            this.groupBox4.TabIndex = 25;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Filter Criterion";
            // 
            // textBoxFilterCriterionHub
            // 
            this.textBoxFilterCriterionHub.Location = new System.Drawing.Point(6, 16);
            this.textBoxFilterCriterionHub.Name = "textBoxFilterCriterionHub";
            this.textBoxFilterCriterionHub.Size = new System.Drawing.Size(151, 20);
            this.textBoxFilterCriterionHub.TabIndex = 23;
            this.textBoxFilterCriterionHub.TextChanged += new System.EventHandler(this.button_Repopulate_Hub);
            // 
            // checkBoxSelectAllHubs
            // 
            this.checkBoxSelectAllHubs.AutoSize = true;
            this.checkBoxSelectAllHubs.Location = new System.Drawing.Point(102, 11);
            this.checkBoxSelectAllHubs.Name = "checkBoxSelectAllHubs";
            this.checkBoxSelectAllHubs.Size = new System.Drawing.Size(69, 17);
            this.checkBoxSelectAllHubs.TabIndex = 20;
            this.checkBoxSelectAllHubs.Text = "Select all";
            this.checkBoxSelectAllHubs.UseVisualStyleBackColor = true;
            this.checkBoxSelectAllHubs.CheckedChanged += new System.EventHandler(this.checkBoxSelectAll_CheckedChanged_1);
            // 
            // buttonRepopulateHubs
            // 
            this.buttonRepopulateHubs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRepopulateHubs.Location = new System.Drawing.Point(132, 432);
            this.buttonRepopulateHubs.Name = "buttonRepopulateHubs";
            this.buttonRepopulateHubs.Size = new System.Drawing.Size(109, 40);
            this.buttonRepopulateHubs.TabIndex = 9;
            this.buttonRepopulateHubs.Text = "Repopulate Selection";
            this.buttonRepopulateHubs.UseVisualStyleBackColor = true;
            this.buttonRepopulateHubs.Click += new System.EventHandler(this.button_Repopulate_Hub);
            // 
            // checkedListBoxHubMetadata
            // 
            this.checkedListBoxHubMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.checkedListBoxHubMetadata.CheckOnClick = true;
            this.checkedListBoxHubMetadata.ColumnWidth = 261;
            this.checkedListBoxHubMetadata.FormattingEnabled = true;
            this.checkedListBoxHubMetadata.Location = new System.Drawing.Point(17, 31);
            this.checkedListBoxHubMetadata.MultiColumn = true;
            this.checkedListBoxHubMetadata.Name = "checkedListBoxHubMetadata";
            this.checkedListBoxHubMetadata.Size = new System.Drawing.Size(523, 274);
            this.checkedListBoxHubMetadata.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Hub Processing";
            // 
            // richTextBoxHub
            // 
            this.richTextBoxHub.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.richTextBoxHub.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxHub.Location = new System.Drawing.Point(17, 308);
            this.richTextBoxHub.Name = "richTextBoxHub";
            this.richTextBoxHub.Size = new System.Drawing.Size(523, 115);
            this.richTextBoxHub.TabIndex = 0;
            this.richTextBoxHub.Text = "";
            // 
            // tabPageSat
            // 
            this.tabPageSat.Controls.Add(this.checkBoxEvaluateSatDelete);
            this.tabPageSat.Controls.Add(this.groupBox5);
            this.tabPageSat.Controls.Add(this.button4);
            this.tabPageSat.Controls.Add(this.checkBoxSelectAllSats);
            this.tabPageSat.Controls.Add(this.checkedListBoxSatMetadata);
            this.tabPageSat.Controls.Add(this.checkBoxDisableSatZeroRecords);
            this.tabPageSat.Controls.Add(this.label2);
            this.tabPageSat.Controls.Add(this.richTextBoxSat);
            this.tabPageSat.Controls.Add(this.buttonGenerateSats);
            this.tabPageSat.Location = new System.Drawing.Point(4, 22);
            this.tabPageSat.Name = "tabPageSat";
            this.tabPageSat.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSat.Size = new System.Drawing.Size(558, 491);
            this.tabPageSat.TabIndex = 1;
            this.tabPageSat.Text = "Satellites";
            this.tabPageSat.UseVisualStyleBackColor = true;
            // 
            // checkBoxEvaluateSatDelete
            // 
            this.checkBoxEvaluateSatDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxEvaluateSatDelete.AutoSize = true;
            this.checkBoxEvaluateSatDelete.Checked = true;
            this.checkBoxEvaluateSatDelete.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxEvaluateSatDelete.Location = new System.Drawing.Point(416, 455);
            this.checkBoxEvaluateSatDelete.Name = "checkBoxEvaluateSatDelete";
            this.checkBoxEvaluateSatDelete.Size = new System.Drawing.Size(144, 17);
            this.checkBoxEvaluateSatDelete.TabIndex = 26;
            this.checkBoxEvaluateSatDelete.Text = "Evaluate Logical Deletes";
            this.checkBoxEvaluateSatDelete.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox5.Controls.Add(this.textBoxFilterCriterionSat);
            this.groupBox5.Location = new System.Drawing.Point(247, 429);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(163, 43);
            this.groupBox5.TabIndex = 25;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Filter Criterion";
            // 
            // textBoxFilterCriterionSat
            // 
            this.textBoxFilterCriterionSat.Location = new System.Drawing.Point(6, 16);
            this.textBoxFilterCriterionSat.Name = "textBoxFilterCriterionSat";
            this.textBoxFilterCriterionSat.Size = new System.Drawing.Size(151, 20);
            this.textBoxFilterCriterionSat.TabIndex = 23;
            this.textBoxFilterCriterionSat.TextChanged += new System.EventHandler(this.button_Repopulate_Sat);
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button4.Location = new System.Drawing.Point(132, 432);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(109, 40);
            this.button4.TabIndex = 22;
            this.button4.Text = "Repopulate Selection";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button_Repopulate_Sat);
            // 
            // checkBoxSelectAllSats
            // 
            this.checkBoxSelectAllSats.AutoSize = true;
            this.checkBoxSelectAllSats.Location = new System.Drawing.Point(115, 11);
            this.checkBoxSelectAllSats.Name = "checkBoxSelectAllSats";
            this.checkBoxSelectAllSats.Size = new System.Drawing.Size(69, 17);
            this.checkBoxSelectAllSats.TabIndex = 21;
            this.checkBoxSelectAllSats.Text = "Select all";
            this.checkBoxSelectAllSats.UseVisualStyleBackColor = true;
            this.checkBoxSelectAllSats.CheckedChanged += new System.EventHandler(this.checkBoxSelectAllSats_CheckedChanged);
            // 
            // checkedListBoxSatMetadata
            // 
            this.checkedListBoxSatMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.checkedListBoxSatMetadata.CheckOnClick = true;
            this.checkedListBoxSatMetadata.ColumnWidth = 261;
            this.checkedListBoxSatMetadata.FormattingEnabled = true;
            this.checkedListBoxSatMetadata.Location = new System.Drawing.Point(17, 31);
            this.checkedListBoxSatMetadata.MultiColumn = true;
            this.checkedListBoxSatMetadata.Name = "checkedListBoxSatMetadata";
            this.checkedListBoxSatMetadata.Size = new System.Drawing.Size(523, 274);
            this.checkedListBoxSatMetadata.TabIndex = 9;
            // 
            // checkBoxDisableSatZeroRecords
            // 
            this.checkBoxDisableSatZeroRecords.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxDisableSatZeroRecords.AutoSize = true;
            this.checkBoxDisableSatZeroRecords.Location = new System.Drawing.Point(416, 432);
            this.checkBoxDisableSatZeroRecords.Name = "checkBoxDisableSatZeroRecords";
            this.checkBoxDisableSatZeroRecords.Size = new System.Drawing.Size(122, 17);
            this.checkBoxDisableSatZeroRecords.TabIndex = 8;
            this.checkBoxDisableSatZeroRecords.Text = "Disable zero-records";
            this.checkBoxDisableSatZeroRecords.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Satellite Processing";
            // 
            // richTextBoxSat
            // 
            this.richTextBoxSat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.richTextBoxSat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxSat.Location = new System.Drawing.Point(17, 308);
            this.richTextBoxSat.Name = "richTextBoxSat";
            this.richTextBoxSat.Size = new System.Drawing.Size(523, 115);
            this.richTextBoxSat.TabIndex = 2;
            this.richTextBoxSat.Text = "";
            // 
            // buttonGenerateSats
            // 
            this.buttonGenerateSats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonGenerateSats.Location = new System.Drawing.Point(17, 432);
            this.buttonGenerateSats.Name = "buttonGenerateSats";
            this.buttonGenerateSats.Size = new System.Drawing.Size(109, 40);
            this.buttonGenerateSats.TabIndex = 3;
            this.buttonGenerateSats.Text = "Generate Satellites";
            this.buttonGenerateSats.UseVisualStyleBackColor = true;
            this.buttonGenerateSats.Click += new System.EventHandler(this.SatelliteButtonClick);
            // 
            // tabPageLink
            // 
            this.tabPageLink.Controls.Add(this.groupBox6);
            this.tabPageLink.Controls.Add(this.button5);
            this.tabPageLink.Controls.Add(this.checkBoxSelectAllLinks);
            this.tabPageLink.Controls.Add(this.checkedListBoxLinkMetadata);
            this.tabPageLink.Controls.Add(this.label3);
            this.tabPageLink.Controls.Add(this.richTextBoxLink);
            this.tabPageLink.Controls.Add(this.buttonGenerateLinks);
            this.tabPageLink.Location = new System.Drawing.Point(4, 22);
            this.tabPageLink.Name = "tabPageLink";
            this.tabPageLink.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLink.Size = new System.Drawing.Size(558, 491);
            this.tabPageLink.TabIndex = 2;
            this.tabPageLink.Text = "Links";
            this.tabPageLink.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox6.Controls.Add(this.textBoxFilterCriterionLnk);
            this.groupBox6.Location = new System.Drawing.Point(247, 429);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(163, 43);
            this.groupBox6.TabIndex = 26;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Filter Criterion";
            // 
            // textBoxFilterCriterionLnk
            // 
            this.textBoxFilterCriterionLnk.Location = new System.Drawing.Point(6, 16);
            this.textBoxFilterCriterionLnk.Name = "textBoxFilterCriterionLnk";
            this.textBoxFilterCriterionLnk.Size = new System.Drawing.Size(151, 20);
            this.textBoxFilterCriterionLnk.TabIndex = 23;
            this.textBoxFilterCriterionLnk.TextChanged += new System.EventHandler(this.button_Repopulate_Lnk);
            // 
            // button5
            // 
            this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button5.Location = new System.Drawing.Point(132, 432);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(109, 40);
            this.button5.TabIndex = 22;
            this.button5.Text = "Repopulate Selection";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button_Repopulate_Lnk);
            // 
            // checkBoxSelectAllLinks
            // 
            this.checkBoxSelectAllLinks.AutoSize = true;
            this.checkBoxSelectAllLinks.Location = new System.Drawing.Point(102, 11);
            this.checkBoxSelectAllLinks.Name = "checkBoxSelectAllLinks";
            this.checkBoxSelectAllLinks.Size = new System.Drawing.Size(69, 17);
            this.checkBoxSelectAllLinks.TabIndex = 21;
            this.checkBoxSelectAllLinks.Text = "Select all";
            this.checkBoxSelectAllLinks.UseVisualStyleBackColor = true;
            this.checkBoxSelectAllLinks.CheckedChanged += new System.EventHandler(this.checkBoxSelectAllLinks_CheckedChanged);
            // 
            // checkedListBoxLinkMetadata
            // 
            this.checkedListBoxLinkMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.checkedListBoxLinkMetadata.CheckOnClick = true;
            this.checkedListBoxLinkMetadata.ColumnWidth = 261;
            this.checkedListBoxLinkMetadata.FormattingEnabled = true;
            this.checkedListBoxLinkMetadata.Location = new System.Drawing.Point(17, 31);
            this.checkedListBoxLinkMetadata.MultiColumn = true;
            this.checkedListBoxLinkMetadata.Name = "checkedListBoxLinkMetadata";
            this.checkedListBoxLinkMetadata.Size = new System.Drawing.Size(523, 274);
            this.checkedListBoxLinkMetadata.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Link Processing";
            // 
            // richTextBoxLink
            // 
            this.richTextBoxLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.richTextBoxLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxLink.Location = new System.Drawing.Point(17, 308);
            this.richTextBoxLink.Name = "richTextBoxLink";
            this.richTextBoxLink.Size = new System.Drawing.Size(523, 115);
            this.richTextBoxLink.TabIndex = 5;
            this.richTextBoxLink.Text = "";
            // 
            // buttonGenerateLinks
            // 
            this.buttonGenerateLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonGenerateLinks.Location = new System.Drawing.Point(17, 432);
            this.buttonGenerateLinks.Name = "buttonGenerateLinks";
            this.buttonGenerateLinks.Size = new System.Drawing.Size(109, 40);
            this.buttonGenerateLinks.TabIndex = 6;
            this.buttonGenerateLinks.Text = "Generate Links";
            this.buttonGenerateLinks.UseVisualStyleBackColor = true;
            this.buttonGenerateLinks.Click += new System.EventHandler(this.LinkButtonClick);
            // 
            // tabPageLinkSat
            // 
            this.tabPageLinkSat.Controls.Add(this.checkBoxEvaluateLsatDeletes);
            this.tabPageLinkSat.Controls.Add(this.groupBox7);
            this.tabPageLinkSat.Controls.Add(this.checkBoxDisableLsatZeroRecords);
            this.tabPageLinkSat.Controls.Add(this.button6);
            this.tabPageLinkSat.Controls.Add(this.checkBoxSelectAllLsats);
            this.tabPageLinkSat.Controls.Add(this.checkedListBoxLsatMetadata);
            this.tabPageLinkSat.Controls.Add(this.label5);
            this.tabPageLinkSat.Controls.Add(this.richTextBoxLsat);
            this.tabPageLinkSat.Controls.Add(this.buttonGenerateLsats);
            this.tabPageLinkSat.Location = new System.Drawing.Point(4, 22);
            this.tabPageLinkSat.Name = "tabPageLinkSat";
            this.tabPageLinkSat.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLinkSat.Size = new System.Drawing.Size(558, 491);
            this.tabPageLinkSat.TabIndex = 4;
            this.tabPageLinkSat.Text = "Link-Satellites";
            this.tabPageLinkSat.UseVisualStyleBackColor = true;
            // 
            // checkBoxEvaluateLsatDeletes
            // 
            this.checkBoxEvaluateLsatDeletes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxEvaluateLsatDeletes.AutoSize = true;
            this.checkBoxEvaluateLsatDeletes.Checked = true;
            this.checkBoxEvaluateLsatDeletes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxEvaluateLsatDeletes.Location = new System.Drawing.Point(416, 455);
            this.checkBoxEvaluateLsatDeletes.Name = "checkBoxEvaluateLsatDeletes";
            this.checkBoxEvaluateLsatDeletes.Size = new System.Drawing.Size(144, 17);
            this.checkBoxEvaluateLsatDeletes.TabIndex = 27;
            this.checkBoxEvaluateLsatDeletes.Text = "Evaluate Logical Deletes";
            this.checkBoxEvaluateLsatDeletes.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox7.Controls.Add(this.textBoxFilterCriterionLsat);
            this.groupBox7.Location = new System.Drawing.Point(247, 429);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(163, 43);
            this.groupBox7.TabIndex = 26;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Filter Criterion";
            // 
            // textBoxFilterCriterionLsat
            // 
            this.textBoxFilterCriterionLsat.Location = new System.Drawing.Point(6, 16);
            this.textBoxFilterCriterionLsat.Name = "textBoxFilterCriterionLsat";
            this.textBoxFilterCriterionLsat.Size = new System.Drawing.Size(151, 20);
            this.textBoxFilterCriterionLsat.TabIndex = 23;
            this.textBoxFilterCriterionLsat.TextChanged += new System.EventHandler(this.button_Repopulate_LSAT);
            // 
            // checkBoxDisableLsatZeroRecords
            // 
            this.checkBoxDisableLsatZeroRecords.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxDisableLsatZeroRecords.AutoSize = true;
            this.checkBoxDisableLsatZeroRecords.Location = new System.Drawing.Point(416, 432);
            this.checkBoxDisableLsatZeroRecords.Name = "checkBoxDisableLsatZeroRecords";
            this.checkBoxDisableLsatZeroRecords.Size = new System.Drawing.Size(122, 17);
            this.checkBoxDisableLsatZeroRecords.TabIndex = 23;
            this.checkBoxDisableLsatZeroRecords.Text = "Disable zero-records";
            this.checkBoxDisableLsatZeroRecords.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button6.Location = new System.Drawing.Point(132, 432);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(109, 40);
            this.button6.TabIndex = 22;
            this.button6.Text = "Repopulate Selection";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button_Repopulate_LSAT);
            // 
            // checkBoxSelectAllLsats
            // 
            this.checkBoxSelectAllLsats.AutoSize = true;
            this.checkBoxSelectAllLsats.Location = new System.Drawing.Point(139, 11);
            this.checkBoxSelectAllLsats.Name = "checkBoxSelectAllLsats";
            this.checkBoxSelectAllLsats.Size = new System.Drawing.Size(69, 17);
            this.checkBoxSelectAllLsats.TabIndex = 21;
            this.checkBoxSelectAllLsats.Text = "Select all";
            this.checkBoxSelectAllLsats.UseVisualStyleBackColor = true;
            this.checkBoxSelectAllLsats.CheckedChanged += new System.EventHandler(this.checkBoxSelectAllLsats_CheckedChanged);
            // 
            // checkedListBoxLsatMetadata
            // 
            this.checkedListBoxLsatMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.checkedListBoxLsatMetadata.CheckOnClick = true;
            this.checkedListBoxLsatMetadata.ColumnWidth = 261;
            this.checkedListBoxLsatMetadata.FormattingEnabled = true;
            this.checkedListBoxLsatMetadata.Location = new System.Drawing.Point(17, 31);
            this.checkedListBoxLsatMetadata.MultiColumn = true;
            this.checkedListBoxLsatMetadata.Name = "checkedListBoxLsatMetadata";
            this.checkedListBoxLsatMetadata.Size = new System.Drawing.Size(523, 274);
            this.checkedListBoxLsatMetadata.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(122, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Link-Satellite Processing";
            // 
            // richTextBoxLsat
            // 
            this.richTextBoxLsat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.richTextBoxLsat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxLsat.Location = new System.Drawing.Point(17, 308);
            this.richTextBoxLsat.Name = "richTextBoxLsat";
            this.richTextBoxLsat.Size = new System.Drawing.Size(523, 71);
            this.richTextBoxLsat.TabIndex = 8;
            this.richTextBoxLsat.Text = "";
            // 
            // buttonGenerateLsats
            // 
            this.buttonGenerateLsats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonGenerateLsats.Location = new System.Drawing.Point(17, 432);
            this.buttonGenerateLsats.Name = "buttonGenerateLsats";
            this.buttonGenerateLsats.Size = new System.Drawing.Size(109, 40);
            this.buttonGenerateLsats.TabIndex = 9;
            this.buttonGenerateLsats.Text = "Generate Link Satellites";
            this.buttonGenerateLsats.UseVisualStyleBackColor = true;
            this.buttonGenerateLsats.Click += new System.EventHandler(this.LinkSatelliteButtonClick);
            // 
            // tabPageSettings
            // 
            this.tabPageSettings.Controls.Add(this.checkBoxUnicode);
            this.tabPageSettings.Controls.Add(this.groupBoxhashKeyoutput);
            this.tabPageSettings.Controls.Add(this.checkBoxDisableHash);
            this.tabPageSettings.Controls.Add(this.label6);
            this.tabPageSettings.Controls.Add(this.textBoxConfigurationPath);
            this.tabPageSettings.Controls.Add(this.OutputPathLabel);
            this.tabPageSettings.Controls.Add(this.textBoxOutputPath);
            this.tabPageSettings.Controls.Add(this.label31);
            this.tabPageSettings.Location = new System.Drawing.Point(4, 22);
            this.tabPageSettings.Name = "tabPageSettings";
            this.tabPageSettings.Size = new System.Drawing.Size(558, 491);
            this.tabPageSettings.TabIndex = 8;
            this.tabPageSettings.Text = "Settings";
            this.tabPageSettings.UseVisualStyleBackColor = true;
            this.tabPageSettings.Click += new System.EventHandler(this.tabPageDefaultSettings_Click);
            // 
            // checkBoxUnicode
            // 
            this.checkBoxUnicode.AutoSize = true;
            this.checkBoxUnicode.Location = new System.Drawing.Point(17, 268);
            this.checkBoxUnicode.Name = "checkBoxUnicode";
            this.checkBoxUnicode.Size = new System.Drawing.Size(112, 17);
            this.checkBoxUnicode.TabIndex = 89;
            this.checkBoxUnicode.Text = "Output in Unicode";
            this.checkBoxUnicode.UseVisualStyleBackColor = true;
            // 
            // groupBoxhashKeyoutput
            // 
            this.groupBoxhashKeyoutput.Controls.Add(this.radioButtonCharacterHash);
            this.groupBoxhashKeyoutput.Controls.Add(this.radioButtonBinaryHash);
            this.groupBoxhashKeyoutput.Location = new System.Drawing.Point(17, 155);
            this.groupBoxhashKeyoutput.Name = "groupBoxhashKeyoutput";
            this.groupBoxhashKeyoutput.Size = new System.Drawing.Size(126, 70);
            this.groupBoxhashKeyoutput.TabIndex = 88;
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
            this.radioButtonBinaryHash.Location = new System.Drawing.Point(6, 19);
            this.radioButtonBinaryHash.Name = "radioButtonBinaryHash";
            this.radioButtonBinaryHash.Size = new System.Drawing.Size(54, 17);
            this.radioButtonBinaryHash.TabIndex = 0;
            this.radioButtonBinaryHash.Text = "Binary";
            this.radioButtonBinaryHash.UseVisualStyleBackColor = true;
            // 
            // checkBoxDisableHash
            // 
            this.checkBoxDisableHash.AutoSize = true;
            this.checkBoxDisableHash.Location = new System.Drawing.Point(17, 245);
            this.checkBoxDisableHash.Name = "checkBoxDisableHash";
            this.checkBoxDisableHash.Size = new System.Drawing.Size(183, 17);
            this.checkBoxDisableHash.TabIndex = 87;
            this.checkBoxDisableHash.Text = "Disable Hash Keys (experimental)";
            this.checkBoxDisableHash.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 91);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(235, 13);
            this.label6.TabIndex = 84;
            this.label6.Text = "Configuration path (TEAM metadata file location)";
            // 
            // textBoxConfigurationPath
            // 
            this.textBoxConfigurationPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxConfigurationPath.Location = new System.Drawing.Point(17, 109);
            this.textBoxConfigurationPath.Multiline = true;
            this.textBoxConfigurationPath.Name = "textBoxConfigurationPath";
            this.textBoxConfigurationPath.Size = new System.Drawing.Size(493, 27);
            this.textBoxConfigurationPath.TabIndex = 83;
            // 
            // OutputPathLabel
            // 
            this.OutputPathLabel.AutoSize = true;
            this.OutputPathLabel.Location = new System.Drawing.Point(14, 43);
            this.OutputPathLabel.Name = "OutputPathLabel";
            this.OutputPathLabel.Size = new System.Drawing.Size(63, 13);
            this.OutputPathLabel.TabIndex = 82;
            this.OutputPathLabel.Text = "Output path";
            // 
            // textBoxOutputPath
            // 
            this.textBoxOutputPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxOutputPath.Location = new System.Drawing.Point(17, 61);
            this.textBoxOutputPath.Multiline = true;
            this.textBoxOutputPath.Name = "textBoxOutputPath";
            this.textBoxOutputPath.Size = new System.Drawing.Size(493, 27);
            this.textBoxOutputPath.TabIndex = 81;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(14, 12);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(199, 13);
            this.label31.TabIndex = 53;
            this.label31.Text = "Configuration settings for ETL generation";
            // 
            // menuStripMainMenu
            // 
            this.menuStripMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.dimensionalToolStripMenuItem,
            this.testingToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStripMainMenu.Location = new System.Drawing.Point(0, 0);
            this.menuStripMainMenu.Name = "menuStripMainMenu";
            this.menuStripMainMenu.Size = new System.Drawing.Size(1362, 24);
            this.menuStripMainMenu.TabIndex = 4;
            this.menuStripMainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openOutputDirectoryToolStripMenuItem,
            this.openConfigurationDirectoryToolStripMenuItem,
            this.saveConfigurationFileToolStripMenuItem,
            this.toolStripSeparator2,
            this.openTEAMToolStripMenuItem,
            this.toolStripSeparator3,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openOutputDirectoryToolStripMenuItem
            // 
            this.openOutputDirectoryToolStripMenuItem.Image = global::Virtual_EDW.Properties.Resources.OpenDirectoryIcon;
            this.openOutputDirectoryToolStripMenuItem.Name = "openOutputDirectoryToolStripMenuItem";
            this.openOutputDirectoryToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.openOutputDirectoryToolStripMenuItem.Text = "Open Output Directory";
            this.openOutputDirectoryToolStripMenuItem.Click += new System.EventHandler(this.openOutputDirectoryToolStripMenuItem_Click);
            // 
            // openConfigurationDirectoryToolStripMenuItem
            // 
            this.openConfigurationDirectoryToolStripMenuItem.Image = global::Virtual_EDW.Properties.Resources.OpenDirectoryIcon;
            this.openConfigurationDirectoryToolStripMenuItem.Name = "openConfigurationDirectoryToolStripMenuItem";
            this.openConfigurationDirectoryToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.openConfigurationDirectoryToolStripMenuItem.Text = "Open Configuration Directory";
            this.openConfigurationDirectoryToolStripMenuItem.Click += new System.EventHandler(this.openConfigurationDirectoryToolStripMenuItem_Click);
            // 
            // saveConfigurationFileToolStripMenuItem
            // 
            this.saveConfigurationFileToolStripMenuItem.Image = global::Virtual_EDW.Properties.Resources.SaveFile;
            this.saveConfigurationFileToolStripMenuItem.Name = "saveConfigurationFileToolStripMenuItem";
            this.saveConfigurationFileToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.saveConfigurationFileToolStripMenuItem.Text = "Save VEDW Settings";
            this.saveConfigurationFileToolStripMenuItem.Click += new System.EventHandler(this.saveConfigurationFileToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = global::Virtual_EDW.Properties.Resources.ExitApplication;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // dimensionalToolStripMenuItem
            // 
            this.dimensionalToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rawDataMartToolStripMenuItem,
            this.pointInTimeToolStripMenuItem,
            this.unknownKeysToolStripMenuItem});
            this.dimensionalToolStripMenuItem.Name = "dimensionalToolStripMenuItem";
            this.dimensionalToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.dimensionalToolStripMenuItem.Text = "Delivery";
            // 
            // rawDataMartToolStripMenuItem
            // 
            this.rawDataMartToolStripMenuItem.Image = global::Virtual_EDW.Properties.Resources.CubeIcon;
            this.rawDataMartToolStripMenuItem.Name = "rawDataMartToolStripMenuItem";
            this.rawDataMartToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.rawDataMartToolStripMenuItem.Text = "Raw Data Mart";
            this.rawDataMartToolStripMenuItem.Click += new System.EventHandler(this.rawDataMartToolStripMenuItem_Click);
            // 
            // pointInTimeToolStripMenuItem
            // 
            this.pointInTimeToolStripMenuItem.Image = global::Virtual_EDW.Properties.Resources.Time;
            this.pointInTimeToolStripMenuItem.Name = "pointInTimeToolStripMenuItem";
            this.pointInTimeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.pointInTimeToolStripMenuItem.Text = "Point In Time";
            this.pointInTimeToolStripMenuItem.Click += new System.EventHandler(this.pointInTimeToolStripMenuItem_Click);
            // 
            // unknownKeysToolStripMenuItem
            // 
            this.unknownKeysToolStripMenuItem.Image = global::Virtual_EDW.Properties.Resources.ghost_icon;
            this.unknownKeysToolStripMenuItem.Name = "unknownKeysToolStripMenuItem";
            this.unknownKeysToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.unknownKeysToolStripMenuItem.Text = "Unknown Keys";
            this.unknownKeysToolStripMenuItem.Click += new System.EventHandler(this.unknownKeysToolStripMenuItem_Click);
            // 
            // testingToolStripMenuItem
            // 
            this.testingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateTestDataToolStripMenuItem,
            this.generateRIValidationToolStripMenuItem});
            this.testingToolStripMenuItem.Name = "testingToolStripMenuItem";
            this.testingToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.testingToolStripMenuItem.Text = "Testing";
            // 
            // generateTestDataToolStripMenuItem
            // 
            this.generateTestDataToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("generateTestDataToolStripMenuItem.Image")));
            this.generateTestDataToolStripMenuItem.Name = "generateTestDataToolStripMenuItem";
            this.generateTestDataToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
            this.generateTestDataToolStripMenuItem.Text = "Generate Test Data";
            this.generateTestDataToolStripMenuItem.Click += new System.EventHandler(this.generateTestDataToolStripMenuItem_Click);
            // 
            // generateRIValidationToolStripMenuItem
            // 
            this.generateRIValidationToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("generateRIValidationToolStripMenuItem.Image")));
            this.generateRIValidationToolStripMenuItem.Name = "generateRIValidationToolStripMenuItem";
            this.generateRIValidationToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
            this.generateRIValidationToolStripMenuItem.Text = "Generate Referential Integrity Validation";
            this.generateRIValidationToolStripMenuItem.Click += new System.EventHandler(this.generateRIValidationToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem1,
            this.toolStripMenuItem1,
            this.linksToolStripMenuItem,
            this.toolStripSeparator1,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Image = global::Virtual_EDW.Properties.Resources.HelpIconSmall;
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(188, 22);
            this.helpToolStripMenuItem1.Text = "Help";
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.helpToolStripMenuItem1_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Image = global::Virtual_EDW.Properties.Resources.DocumentationIcon;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(188, 22);
            this.toolStripMenuItem1.Text = "Reset Documentation";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // linksToolStripMenuItem
            // 
            this.linksToolStripMenuItem.Image = global::Virtual_EDW.Properties.Resources.LinkIcon;
            this.linksToolStripMenuItem.Name = "linksToolStripMenuItem";
            this.linksToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.linksToolStripMenuItem.Text = "Links";
            this.linksToolStripMenuItem.Click += new System.EventHandler(this.linksToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(185, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.ToolTipText = "Information about Virtual Enterprise Data Warehouse";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(584, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Output";
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button3.BackColor = System.Drawing.SystemColors.ControlDark;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button3.Location = new System.Drawing.Point(33, 568);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(109, 40);
            this.button3.TabIndex = 6;
            this.button3.Text = "Do everything (Integration Layer)";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.DoEverythingButtonClick);
            // 
            // checkBoxGenerateInDatabase
            // 
            this.checkBoxGenerateInDatabase.AutoSize = true;
            this.checkBoxGenerateInDatabase.Location = new System.Drawing.Point(6, 20);
            this.checkBoxGenerateInDatabase.Name = "checkBoxGenerateInDatabase";
            this.checkBoxGenerateInDatabase.Size = new System.Drawing.Size(128, 17);
            this.checkBoxGenerateInDatabase.TabIndex = 7;
            this.checkBoxGenerateInDatabase.Text = "Generate in database";
            this.checkBoxGenerateInDatabase.UseVisualStyleBackColor = true;
            // 
            // checkBoxSchemaBound
            // 
            this.checkBoxSchemaBound.AutoSize = true;
            this.checkBoxSchemaBound.Enabled = false;
            this.checkBoxSchemaBound.Location = new System.Drawing.Point(6, 41);
            this.checkBoxSchemaBound.Name = "checkBoxSchemaBound";
            this.checkBoxSchemaBound.Size = new System.Drawing.Size(186, 17);
            this.checkBoxSchemaBound.TabIndex = 9;
            this.checkBoxSchemaBound.Text = "Schemabound option (Views only)";
            this.checkBoxSchemaBound.UseVisualStyleBackColor = true;
            this.checkBoxSchemaBound.CheckedChanged += new System.EventHandler(this.SchemaboundCheckbox_CheckedChanged);
            // 
            // checkBoxIfExistsStatement
            // 
            this.checkBoxIfExistsStatement.AutoSize = true;
            this.checkBoxIfExistsStatement.Location = new System.Drawing.Point(6, 63);
            this.checkBoxIfExistsStatement.Name = "checkBoxIfExistsStatement";
            this.checkBoxIfExistsStatement.Size = new System.Drawing.Size(172, 17);
            this.checkBoxIfExistsStatement.TabIndex = 10;
            this.checkBoxIfExistsStatement.Text = "Add If Exists / Drop / Truncate";
            this.checkBoxIfExistsStatement.UseVisualStyleBackColor = true;
            // 
            // TargetPlatformGroupBox
            // 
            this.TargetPlatformGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TargetPlatformGroupBox.Controls.Add(this.SQL2014Radiobutton);
            this.TargetPlatformGroupBox.Controls.Add(this.OracleRadiobutton);
            this.TargetPlatformGroupBox.Controls.Add(this.radiobuttonANSISQL);
            this.TargetPlatformGroupBox.Location = new System.Drawing.Point(148, 567);
            this.TargetPlatformGroupBox.Name = "TargetPlatformGroupBox";
            this.TargetPlatformGroupBox.Size = new System.Drawing.Size(178, 109);
            this.TargetPlatformGroupBox.TabIndex = 11;
            this.TargetPlatformGroupBox.TabStop = false;
            this.TargetPlatformGroupBox.Text = "Target Platform";
            // 
            // SQL2014Radiobutton
            // 
            this.SQL2014Radiobutton.AutoSize = true;
            this.SQL2014Radiobutton.Location = new System.Drawing.Point(7, 42);
            this.SQL2014Radiobutton.Name = "SQL2014Radiobutton";
            this.SQL2014Radiobutton.Size = new System.Drawing.Size(107, 17);
            this.SQL2014Radiobutton.TabIndex = 3;
            this.SQL2014Radiobutton.Text = "SQL Server 2014";
            this.SQL2014Radiobutton.UseVisualStyleBackColor = true;
            // 
            // OracleRadiobutton
            // 
            this.OracleRadiobutton.AutoSize = true;
            this.OracleRadiobutton.Enabled = false;
            this.OracleRadiobutton.Location = new System.Drawing.Point(7, 64);
            this.OracleRadiobutton.Name = "OracleRadiobutton";
            this.OracleRadiobutton.Size = new System.Drawing.Size(77, 17);
            this.OracleRadiobutton.TabIndex = 2;
            this.OracleRadiobutton.Text = "Oracle 12c";
            this.OracleRadiobutton.UseVisualStyleBackColor = true;
            // 
            // radiobuttonANSISQL
            // 
            this.radiobuttonANSISQL.AutoSize = true;
            this.radiobuttonANSISQL.Enabled = false;
            this.radiobuttonANSISQL.Location = new System.Drawing.Point(7, 21);
            this.radiobuttonANSISQL.Name = "radiobuttonANSISQL";
            this.radiobuttonANSISQL.Size = new System.Drawing.Size(74, 17);
            this.radiobuttonANSISQL.TabIndex = 0;
            this.radiobuttonANSISQL.Text = "ANSI SQL";
            this.radiobuttonANSISQL.UseVisualStyleBackColor = true;
            // 
            // OutputGroupBox
            // 
            this.OutputGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.OutputGroupBox.Controls.Add(this.radioButtonIntoStatement);
            this.OutputGroupBox.Controls.Add(this.radiobuttonStoredProc);
            this.OutputGroupBox.Controls.Add(this.radiobuttonViews);
            this.OutputGroupBox.Location = new System.Drawing.Point(332, 682);
            this.OutputGroupBox.Name = "OutputGroupBox";
            this.OutputGroupBox.Size = new System.Drawing.Size(242, 101);
            this.OutputGroupBox.TabIndex = 13;
            this.OutputGroupBox.TabStop = false;
            this.OutputGroupBox.Text = "Output Type";
            // 
            // radioButtonIntoStatement
            // 
            this.radioButtonIntoStatement.AutoSize = true;
            this.radioButtonIntoStatement.Location = new System.Drawing.Point(7, 61);
            this.radioButtonIntoStatement.Name = "radioButtonIntoStatement";
            this.radioButtonIntoStatement.Size = new System.Drawing.Size(156, 17);
            this.radioButtonIntoStatement.TabIndex = 2;
            this.radioButtonIntoStatement.Text = "Into statements (Views only)";
            this.radioButtonIntoStatement.UseVisualStyleBackColor = true;
            // 
            // radiobuttonStoredProc
            // 
            this.radiobuttonStoredProc.AutoSize = true;
            this.radiobuttonStoredProc.Enabled = false;
            this.radiobuttonStoredProc.Location = new System.Drawing.Point(7, 40);
            this.radiobuttonStoredProc.Name = "radiobuttonStoredProc";
            this.radiobuttonStoredProc.Size = new System.Drawing.Size(113, 17);
            this.radiobuttonStoredProc.TabIndex = 1;
            this.radiobuttonStoredProc.Text = "Stored Procedures";
            this.radiobuttonStoredProc.UseVisualStyleBackColor = true;
            // 
            // radiobuttonViews
            // 
            this.radiobuttonViews.AutoSize = true;
            this.radiobuttonViews.Location = new System.Drawing.Point(7, 19);
            this.radiobuttonViews.Name = "radiobuttonViews";
            this.radiobuttonViews.Size = new System.Drawing.Size(53, 17);
            this.radiobuttonViews.TabIndex = 0;
            this.radiobuttonViews.Text = "Views";
            this.radiobuttonViews.UseVisualStyleBackColor = true;
            // 
            // SQLGenerationGroupBox
            // 
            this.SQLGenerationGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SQLGenerationGroupBox.Controls.Add(this.checkBoxIgnoreVersion);
            this.SQLGenerationGroupBox.Controls.Add(this.checkBoxGenerateInDatabase);
            this.SQLGenerationGroupBox.Controls.Add(this.checkBoxSchemaBound);
            this.SQLGenerationGroupBox.Controls.Add(this.checkBoxIfExistsStatement);
            this.SQLGenerationGroupBox.Location = new System.Drawing.Point(332, 568);
            this.SQLGenerationGroupBox.Name = "SQLGenerationGroupBox";
            this.SQLGenerationGroupBox.Size = new System.Drawing.Size(242, 108);
            this.SQLGenerationGroupBox.TabIndex = 14;
            this.SQLGenerationGroupBox.TabStop = false;
            this.SQLGenerationGroupBox.Text = "SQL Generation Options";
            // 
            // checkBoxIgnoreVersion
            // 
            this.checkBoxIgnoreVersion.AutoSize = true;
            this.checkBoxIgnoreVersion.Checked = true;
            this.checkBoxIgnoreVersion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxIgnoreVersion.Location = new System.Drawing.Point(6, 86);
            this.checkBoxIgnoreVersion.Name = "checkBoxIgnoreVersion";
            this.checkBoxIgnoreVersion.Size = new System.Drawing.Size(219, 17);
            this.checkBoxIgnoreVersion.TabIndex = 11;
            this.checkBoxIgnoreVersion.Text = "Use live database / ignore model version";
            this.checkBoxIgnoreVersion.UseVisualStyleBackColor = true;
            this.checkBoxIgnoreVersion.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // groupBoxVersionSelection
            // 
            this.groupBoxVersionSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxVersionSelection.Controls.Add(this.labelVersion);
            this.groupBoxVersionSelection.Controls.Add(this.trackBarVersioning);
            this.groupBoxVersionSelection.Location = new System.Drawing.Point(148, 682);
            this.groupBoxVersionSelection.Name = "groupBoxVersionSelection";
            this.groupBoxVersionSelection.Size = new System.Drawing.Size(178, 101);
            this.groupBoxVersionSelection.TabIndex = 19;
            this.groupBoxVersionSelection.TabStop = false;
            this.groupBoxVersionSelection.Text = "Version Selection";
            this.groupBoxVersionSelection.Enter += new System.EventHandler(this.groupBoxVersionSelection_Enter);
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(6, 63);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(42, 13);
            this.labelVersion.TabIndex = 18;
            this.labelVersion.Text = "Version";
            // 
            // trackBarVersioning
            // 
            this.trackBarVersioning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarVersioning.Location = new System.Drawing.Point(4, 24);
            this.trackBarVersioning.Name = "trackBarVersioning";
            this.trackBarVersioning.Size = new System.Drawing.Size(163, 45);
            this.trackBarVersioning.TabIndex = 17;
            this.trackBarVersioning.Scroll += new System.EventHandler(this.trackBarVersioning_Scroll);
            // 
            // backgroundWorkerActivateMetadata
            // 
            this.backgroundWorkerActivateMetadata.WorkerReportsProgress = true;
            this.backgroundWorkerActivateMetadata.WorkerSupportsCancellation = true;
            this.backgroundWorkerActivateMetadata.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWorkMetadataActivation);
            this.backgroundWorkerActivateMetadata.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerActivateMetadata_ProgressChanged);
            this.backgroundWorkerActivateMetadata.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerActivateMetadata_RunWorkerCompleted);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox1.Image = global::Virtual_EDW.Properties.Resources.RavosLogo;
            this.pictureBox1.Location = new System.Drawing.Point(33, 683);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(109, 100);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 15;
            this.pictureBox1.TabStop = false;
            // 
            // openTEAMToolStripMenuItem
            // 
            this.openTEAMToolStripMenuItem.Image = global::Virtual_EDW.Properties.Resources.RavosLogo;
            this.openTEAMToolStripMenuItem.Name = "openTEAMToolStripMenuItem";
            this.openTEAMToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.openTEAMToolStripMenuItem.Text = "Open TEAM";
            this.openTEAMToolStripMenuItem.Click += new System.EventHandler(this.openTEAMToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(228, 6);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(228, 6);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1362, 797);
            this.Controls.Add(this.groupBoxVersionSelection);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.SQLGenerationGroupBox);
            this.Controls.Add(this.OutputGroupBox);
            this.Controls.Add(this.TargetPlatformGroupBox);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.MainTabControl);
            this.Controls.Add(this.richTextBoxInformation);
            this.Controls.Add(this.menuStripMainMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripMainMenu;
            this.MinimumSize = new System.Drawing.Size(1378, 835);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Virtual Enterprise Data Warehouse - v1.4";
            this.MainTabControl.ResumeLayout(false);
            this.tabPageStaging.ResumeLayout(false);
            this.tabPageStaging.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPagePSA.ResumeLayout(false);
            this.tabPagePSA.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPageHub.ResumeLayout(false);
            this.tabPageHub.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabPageSat.ResumeLayout(false);
            this.tabPageSat.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabPageLink.ResumeLayout(false);
            this.tabPageLink.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tabPageLinkSat.ResumeLayout(false);
            this.tabPageLinkSat.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.tabPageSettings.ResumeLayout(false);
            this.tabPageSettings.PerformLayout();
            this.groupBoxhashKeyoutput.ResumeLayout(false);
            this.groupBoxhashKeyoutput.PerformLayout();
            this.menuStripMainMenu.ResumeLayout(false);
            this.menuStripMainMenu.PerformLayout();
            this.TargetPlatformGroupBox.ResumeLayout(false);
            this.TargetPlatformGroupBox.PerformLayout();
            this.OutputGroupBox.ResumeLayout(false);
            this.OutputGroupBox.PerformLayout();
            this.SQLGenerationGroupBox.ResumeLayout(false);
            this.SQLGenerationGroupBox.PerformLayout();
            this.groupBoxVersionSelection.ResumeLayout(false);
            this.groupBoxVersionSelection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVersioning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonGenerateHubs;
        private System.Windows.Forms.RichTextBox richTextBoxInformation;
        private System.Windows.Forms.TabControl MainTabControl;
        private System.Windows.Forms.TabPage tabPageHub;
        private System.Windows.Forms.TabPage tabPageSat;
        private System.Windows.Forms.RichTextBox richTextBoxHub;
        private System.Windows.Forms.TabPage tabPageLink;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MenuStrip menuStripMainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox richTextBoxSat;
        private System.Windows.Forms.Button buttonGenerateSats;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox richTextBoxLink;
        private System.Windows.Forms.Button buttonGenerateLinks;
        private System.Windows.Forms.ToolStripMenuItem openOutputDirectoryToolStripMenuItem;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.CheckBox checkBoxGenerateInDatabase;
        private System.Windows.Forms.TabPage tabPageLinkSat;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RichTextBox richTextBoxLsat;
        private System.Windows.Forms.Button buttonGenerateLsats;
        private System.Windows.Forms.CheckBox checkBoxSchemaBound;
        private System.Windows.Forms.CheckBox checkBoxIfExistsStatement;
        private System.Windows.Forms.TabPage tabPageSettings;
        private System.Windows.Forms.GroupBox TargetPlatformGroupBox;
        private System.Windows.Forms.RadioButton SQL2014Radiobutton;
        private System.Windows.Forms.RadioButton OracleRadiobutton;
        private System.Windows.Forms.RadioButton radiobuttonANSISQL;
        private System.Windows.Forms.GroupBox OutputGroupBox;
        private System.Windows.Forms.RadioButton radiobuttonStoredProc;
        private System.Windows.Forms.RadioButton radiobuttonViews;
        private System.Windows.Forms.GroupBox SQLGenerationGroupBox;
        private System.Windows.Forms.ToolStripMenuItem dimensionalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rawDataMartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem linksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TabPage tabPagePSA;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.RichTextBox richTextBoxPSA;
        private System.Windows.Forms.Button buttonGeneratePSA;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.RadioButton radioButtonIntoStatement;
        private System.Windows.Forms.TabPage tabPageStaging;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.RichTextBox richTextBoxStaging;
        private System.Windows.Forms.Button buttonGenerateStaging;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.ToolStripMenuItem testingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateTestDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateRIValidationToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBoxVersionSelection;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.CheckBox checkBoxIgnoreVersion;
        internal System.Windows.Forms.TrackBar trackBarVersioning;
        private System.Windows.Forms.CheckBox checkBoxDisableSatZeroRecords;
        private System.Windows.Forms.Button buttonRepopulateHubs;
        private System.Windows.Forms.CheckedListBox checkedListBoxHubMetadata;
        private System.Windows.Forms.CheckBox checkBoxSelectAllHubs;
        private System.Windows.Forms.CheckBox checkBoxSelectAllStg;
        private System.Windows.Forms.CheckedListBox checkedListBoxStgMetadata;
        private System.Windows.Forms.CheckBox checkBoxSelectAllPsa;
        private System.Windows.Forms.CheckedListBox checkedListBoxPsaMetadata;
        private System.Windows.Forms.CheckBox checkBoxSelectAllSats;
        private System.Windows.Forms.CheckedListBox checkedListBoxSatMetadata;
        private System.Windows.Forms.CheckBox checkBoxSelectAllLinks;
        private System.Windows.Forms.CheckedListBox checkedListBoxLinkMetadata;
        private System.Windows.Forms.CheckBox checkBoxSelectAllLsats;
        private System.Windows.Forms.CheckedListBox checkedListBoxLsatMetadata;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.CheckBox checkBoxDisableLsatZeroRecords;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxFilterCriterionStg;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBoxFilterCriterionPsa;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox textBoxFilterCriterionHub;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox textBoxFilterCriterionSat;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox textBoxFilterCriterionLnk;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.TextBox textBoxFilterCriterionLsat;
        private System.Windows.Forms.ToolStripMenuItem pointInTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unknownKeysToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxEvaluateSatDelete;
        private System.Windows.Forms.CheckBox checkBoxEvaluateLsatDeletes;
        public System.ComponentModel.BackgroundWorker backgroundWorkerActivateMetadata;
        private System.Windows.Forms.CheckBox checkBoxExcludeLanding;
        private System.Windows.Forms.ToolStripMenuItem saveConfigurationFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openConfigurationDirectoryToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxUnicode;
        internal System.Windows.Forms.GroupBox groupBoxhashKeyoutput;
        internal System.Windows.Forms.RadioButton radioButtonCharacterHash;
        internal System.Windows.Forms.RadioButton radioButtonBinaryHash;
        private System.Windows.Forms.CheckBox checkBoxDisableHash;
        private System.Windows.Forms.Label label6;
        internal System.Windows.Forms.TextBox textBoxConfigurationPath;
        private System.Windows.Forms.Label OutputPathLabel;
        internal System.Windows.Forms.TextBox textBoxOutputPath;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem openTEAMToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    }
}

