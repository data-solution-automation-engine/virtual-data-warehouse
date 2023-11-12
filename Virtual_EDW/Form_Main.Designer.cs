namespace Virtual_Data_Warehouse
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            menuStripMainMenu = new System.Windows.Forms.MenuStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openInputDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openTemplateDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openOutputDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openVDWConfigurationDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openCoreDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            openVDWConfigurationSettingsFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openTEAMConfigurationSettingsFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            openTemplateCollectionFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveTemplateCollectionFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            saveConfigurationFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            displayEventLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            checkBoxGenerateInDatabase = new System.Windows.Forms.CheckBox();
            SQLGenerationGroupBox = new System.Windows.Forms.GroupBox();
            checkBoxSaveToFile = new System.Windows.Forms.CheckBox();
            checkBoxGenerateJsonSchema = new System.Windows.Forms.CheckBox();
            backgroundWorkerActivateMetadata = new System.ComponentModel.BackgroundWorker();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            groupBox9 = new System.Windows.Forms.GroupBox();
            richTextBoxInformationMain = new System.Windows.Forms.RichTextBox();
            button12 = new System.Windows.Forms.Button();
            tabPageSettings = new System.Windows.Forms.TabPage();
            groupBox1 = new System.Windows.Forms.GroupBox();
            label10 = new System.Windows.Forms.Label();
            textBoxMetadataPath = new System.Windows.Forms.TextBox();
            pictureBox4 = new System.Windows.Forms.PictureBox();
            labelTemplatePath = new System.Windows.Forms.Label();
            textBoxTemplatePath = new System.Windows.Forms.TextBox();
            pictureBox6 = new System.Windows.Forms.PictureBox();
            groupBoxConfigurationPaths = new System.Windows.Forms.GroupBox();
            pictureBox7 = new System.Windows.Forms.PictureBox();
            textBoxTeamConfigurationPath = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            comboBoxEnvironments = new System.Windows.Forms.ComboBox();
            pictureBox3 = new System.Windows.Forms.PictureBox();
            textBoxTeamEnvironmentsFilePath = new System.Windows.Forms.TextBox();
            labelTEAMConfigurationFile = new System.Windows.Forms.Label();
            groupBoxOutputOptions = new System.Windows.Forms.GroupBox();
            pictureBox5 = new System.Windows.Forms.PictureBox();
            textBoxSchemaName = new System.Windows.Forms.TextBox();
            label12 = new System.Windows.Forms.Label();
            OutputPathLabel = new System.Windows.Forms.Label();
            textBoxOutputPath = new System.Windows.Forms.TextBox();
            tabPageHome = new System.Windows.Forms.TabPage();
            labelWelcome = new System.Windows.Forms.Label();
            pictureBox2 = new System.Windows.Forms.PictureBox();
            tabControlMain = new System.Windows.Forms.TabControl();
            backgroundWorkerEventLog = new System.ComponentModel.BackgroundWorker();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            toolTipVdw = new System.Windows.Forms.ToolTip(components);
            clearEventLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            menuStripMainMenu.SuspendLayout();
            SQLGenerationGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox9.SuspendLayout();
            tabPageSettings.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
            groupBoxConfigurationPaths.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            groupBoxOutputOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            tabPageHome.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            tabControlMain.SuspendLayout();
            SuspendLayout();
            // 
            // menuStripMainMenu
            // 
            menuStripMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, helpToolStripMenuItem });
            menuStripMainMenu.Location = new System.Drawing.Point(0, 0);
            menuStripMainMenu.Name = "menuStripMainMenu";
            menuStripMainMenu.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            menuStripMainMenu.Size = new System.Drawing.Size(1469, 24);
            menuStripMainMenu.TabIndex = 4;
            menuStripMainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { openInputDirectoryToolStripMenuItem, openTemplateDirectoryToolStripMenuItem, openOutputDirectoryToolStripMenuItem, openVDWConfigurationDirectoryToolStripMenuItem, openCoreDirectoryToolStripMenuItem, toolStripSeparator1, openVDWConfigurationSettingsFileToolStripMenuItem, openTEAMConfigurationSettingsFileToolStripMenuItem, toolStripSeparator2, openTemplateCollectionFileToolStripMenuItem, saveTemplateCollectionFileToolStripMenuItem, toolStripSeparator4, saveConfigurationFileToolStripMenuItem, toolStripSeparator3, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // openInputDirectoryToolStripMenuItem
            // 
            openInputDirectoryToolStripMenuItem.Image = Properties.Resources.OpenDirectoryIcon;
            openInputDirectoryToolStripMenuItem.Name = "openInputDirectoryToolStripMenuItem";
            openInputDirectoryToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            openInputDirectoryToolStripMenuItem.Text = "Open Metadata Directory";
            openInputDirectoryToolStripMenuItem.Click += openInputDirectoryToolStripMenuItem_Click;
            // 
            // openTemplateDirectoryToolStripMenuItem
            // 
            openTemplateDirectoryToolStripMenuItem.Image = Properties.Resources.OpenDirectoryIcon;
            openTemplateDirectoryToolStripMenuItem.Name = "openTemplateDirectoryToolStripMenuItem";
            openTemplateDirectoryToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            openTemplateDirectoryToolStripMenuItem.Text = "Open Template Directory";
            openTemplateDirectoryToolStripMenuItem.Click += OpenTemplateDirectoryToolStripMenuItem_Click;
            // 
            // openOutputDirectoryToolStripMenuItem
            // 
            openOutputDirectoryToolStripMenuItem.Image = Properties.Resources.OpenDirectoryIcon;
            openOutputDirectoryToolStripMenuItem.Name = "openOutputDirectoryToolStripMenuItem";
            openOutputDirectoryToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            openOutputDirectoryToolStripMenuItem.Text = "Open Output Directory";
            openOutputDirectoryToolStripMenuItem.Click += openOutputDirectoryToolStripMenuItem_Click;
            // 
            // openVDWConfigurationDirectoryToolStripMenuItem
            // 
            openVDWConfigurationDirectoryToolStripMenuItem.Image = Properties.Resources.OpenDirectoryIcon;
            openVDWConfigurationDirectoryToolStripMenuItem.Name = "openVDWConfigurationDirectoryToolStripMenuItem";
            openVDWConfigurationDirectoryToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            openVDWConfigurationDirectoryToolStripMenuItem.Text = "Open Configuration Directory";
            openVDWConfigurationDirectoryToolStripMenuItem.Click += openVDWConfigurationDirectoryToolStripMenuItem_Click;
            // 
            // openCoreDirectoryToolStripMenuItem
            // 
            openCoreDirectoryToolStripMenuItem.Image = Properties.Resources.OpenDirectoryIcon;
            openCoreDirectoryToolStripMenuItem.Name = "openCoreDirectoryToolStripMenuItem";
            openCoreDirectoryToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            openCoreDirectoryToolStripMenuItem.Text = "Open Core Directory";
            openCoreDirectoryToolStripMenuItem.Click += openCoreDirectoryToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(231, 6);
            // 
            // openVDWConfigurationSettingsFileToolStripMenuItem
            // 
            openVDWConfigurationSettingsFileToolStripMenuItem.Image = Properties.Resources.OpenFileIcon;
            openVDWConfigurationSettingsFileToolStripMenuItem.Name = "openVDWConfigurationSettingsFileToolStripMenuItem";
            openVDWConfigurationSettingsFileToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            openVDWConfigurationSettingsFileToolStripMenuItem.Text = "Open VDW Configuration File";
            openVDWConfigurationSettingsFileToolStripMenuItem.Click += openVDWConfigurationSettingsFileToolStripMenuItem_Click;
            // 
            // openTEAMConfigurationSettingsFileToolStripMenuItem
            // 
            openTEAMConfigurationSettingsFileToolStripMenuItem.Image = Properties.Resources.OpenFileIcon;
            openTEAMConfigurationSettingsFileToolStripMenuItem.Name = "openTEAMConfigurationSettingsFileToolStripMenuItem";
            openTEAMConfigurationSettingsFileToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            openTEAMConfigurationSettingsFileToolStripMenuItem.Text = "Open TEAM Environments File";
            openTEAMConfigurationSettingsFileToolStripMenuItem.Click += openTEAMConfigurationSettingsFileToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(231, 6);
            // 
            // openTemplateCollectionFileToolStripMenuItem
            // 
            openTemplateCollectionFileToolStripMenuItem.Image = Properties.Resources.OpenFileIcon;
            openTemplateCollectionFileToolStripMenuItem.Name = "openTemplateCollectionFileToolStripMenuItem";
            openTemplateCollectionFileToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            openTemplateCollectionFileToolStripMenuItem.Text = "Load Template Collection File";
            openTemplateCollectionFileToolStripMenuItem.Click += openTemplateCollectionFileToolStripMenuItem_Click;
            // 
            // saveTemplateCollectionFileToolStripMenuItem
            // 
            saveTemplateCollectionFileToolStripMenuItem.Image = Properties.Resources.SaveFile;
            saveTemplateCollectionFileToolStripMenuItem.Name = "saveTemplateCollectionFileToolStripMenuItem";
            saveTemplateCollectionFileToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            saveTemplateCollectionFileToolStripMenuItem.Text = "Save Template Collection Grid";
            saveTemplateCollectionFileToolStripMenuItem.Click += SaveTemplateCollectionFileToolStripMenuItem_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(231, 6);
            // 
            // saveConfigurationFileToolStripMenuItem
            // 
            saveConfigurationFileToolStripMenuItem.Image = Properties.Resources.SaveFile;
            saveConfigurationFileToolStripMenuItem.Name = "saveConfigurationFileToolStripMenuItem";
            saveConfigurationFileToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S;
            saveConfigurationFileToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            saveConfigurationFileToolStripMenuItem.Text = "Save Settings";
            saveConfigurationFileToolStripMenuItem.Click += saveConfigurationFileToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(231, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Image = Properties.Resources.ExitApplication;
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { helpToolStripMenuItem1, displayEventLogToolStripMenuItem, clearEventLogToolStripMenuItem, aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            // 
            // helpToolStripMenuItem1
            // 
            helpToolStripMenuItem1.Image = Properties.Resources.HelpIconSmall;
            helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            helpToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            helpToolStripMenuItem1.Text = "Help";
            helpToolStripMenuItem1.Click += helpToolStripMenuItem1_Click;
            // 
            // displayEventLogToolStripMenuItem
            // 
            displayEventLogToolStripMenuItem.Image = Properties.Resources.log_file;
            displayEventLogToolStripMenuItem.Name = "displayEventLogToolStripMenuItem";
            displayEventLogToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            displayEventLogToolStripMenuItem.Text = "Display Event Log";
            displayEventLogToolStripMenuItem.Click += displayEventLogToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Image = Properties.Resources.RavosLogo;
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // checkBoxGenerateInDatabase
            // 
            checkBoxGenerateInDatabase.AutoSize = true;
            checkBoxGenerateInDatabase.Location = new System.Drawing.Point(7, 23);
            checkBoxGenerateInDatabase.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxGenerateInDatabase.Name = "checkBoxGenerateInDatabase";
            checkBoxGenerateInDatabase.Size = new System.Drawing.Size(198, 17);
            checkBoxGenerateInDatabase.TabIndex = 7;
            checkBoxGenerateInDatabase.Text = "Generate in database (SQL Server)";
            checkBoxGenerateInDatabase.UseVisualStyleBackColor = true;
            checkBoxGenerateInDatabase.CheckedChanged += checkBoxGenerateInDatabase_CheckedChanged;
            // 
            // SQLGenerationGroupBox
            // 
            SQLGenerationGroupBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            SQLGenerationGroupBox.Controls.Add(checkBoxSaveToFile);
            SQLGenerationGroupBox.Controls.Add(checkBoxGenerateJsonSchema);
            SQLGenerationGroupBox.Controls.Add(checkBoxGenerateInDatabase);
            SQLGenerationGroupBox.Location = new System.Drawing.Point(19, 669);
            SQLGenerationGroupBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SQLGenerationGroupBox.Name = "SQLGenerationGroupBox";
            SQLGenerationGroupBox.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SQLGenerationGroupBox.Size = new System.Drawing.Size(233, 122);
            SQLGenerationGroupBox.TabIndex = 14;
            SQLGenerationGroupBox.TabStop = false;
            SQLGenerationGroupBox.Text = "SQL Generation Options";
            // 
            // checkBoxSaveToFile
            // 
            checkBoxSaveToFile.AutoSize = true;
            checkBoxSaveToFile.Location = new System.Drawing.Point(7, 76);
            checkBoxSaveToFile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxSaveToFile.Name = "checkBoxSaveToFile";
            checkBoxSaveToFile.Size = new System.Drawing.Size(181, 17);
            checkBoxSaveToFile.TabIndex = 9;
            checkBoxSaveToFile.Text = "Save generation output to file";
            checkBoxSaveToFile.UseVisualStyleBackColor = true;
            checkBoxSaveToFile.CheckedChanged += checkBoxSaveToFile_CheckedChanged;
            // 
            // checkBoxGenerateJsonSchema
            // 
            checkBoxGenerateJsonSchema.AutoSize = true;
            checkBoxGenerateJsonSchema.Location = new System.Drawing.Point(7, 50);
            checkBoxGenerateJsonSchema.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxGenerateJsonSchema.Name = "checkBoxGenerateJsonSchema";
            checkBoxGenerateJsonSchema.Size = new System.Drawing.Size(191, 17);
            checkBoxGenerateJsonSchema.TabIndex = 8;
            checkBoxGenerateJsonSchema.Text = "Generate Json metadata schema";
            checkBoxGenerateJsonSchema.UseVisualStyleBackColor = true;
            checkBoxGenerateJsonSchema.CheckedChanged += checkBoxGenerateJsonSchema_CheckedChanged;
            // 
            // backgroundWorkerActivateMetadata
            // 
            backgroundWorkerActivateMetadata.WorkerReportsProgress = true;
            backgroundWorkerActivateMetadata.WorkerSupportsCancellation = true;
            backgroundWorkerActivateMetadata.DoWork += backgroundWorker_DoWorkMetadataActivation;
            backgroundWorkerActivateMetadata.ProgressChanged += backgroundWorkerActivateMetadata_ProgressChanged;
            backgroundWorkerActivateMetadata.RunWorkerCompleted += backgroundWorkerActivateMetadata_RunWorkerCompleted;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            pictureBox1.Image = Properties.Resources.RavosLogo;
            pictureBox1.Location = new System.Drawing.Point(1323, 676);
            pictureBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(127, 115);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 15;
            pictureBox1.TabStop = false;
            // 
            // groupBox9
            // 
            groupBox9.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox9.Controls.Add(richTextBoxInformationMain);
            groupBox9.Location = new System.Drawing.Point(393, 669);
            groupBox9.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox9.Name = "groupBox9";
            groupBox9.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox9.Size = new System.Drawing.Size(923, 122);
            groupBox9.TabIndex = 12;
            groupBox9.TabStop = false;
            groupBox9.Text = "Information";
            // 
            // richTextBoxInformationMain
            // 
            richTextBoxInformationMain.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            richTextBoxInformationMain.BackColor = System.Drawing.SystemColors.Control;
            richTextBoxInformationMain.BorderStyle = System.Windows.Forms.BorderStyle.None;
            richTextBoxInformationMain.Location = new System.Drawing.Point(7, 20);
            richTextBoxInformationMain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            richTextBoxInformationMain.Name = "richTextBoxInformationMain";
            richTextBoxInformationMain.Size = new System.Drawing.Size(909, 95);
            richTextBoxInformationMain.TabIndex = 29;
            richTextBoxInformationMain.Text = "";
            richTextBoxInformationMain.TextChanged += richTextBoxInformationMain_TextChanged;
            // 
            // button12
            // 
            button12.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            button12.Location = new System.Drawing.Point(259, 676);
            button12.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button12.MinimumSize = new System.Drawing.Size(127, 46);
            button12.Name = "button12";
            button12.Size = new System.Drawing.Size(127, 46);
            button12.TabIndex = 23;
            button12.Text = "Refresh Metadata";
            button12.UseVisualStyleBackColor = true;
            button12.Click += buttonRefreshMetadata_Click;
            // 
            // tabPageSettings
            // 
            tabPageSettings.Controls.Add(groupBox1);
            tabPageSettings.Controls.Add(groupBoxConfigurationPaths);
            tabPageSettings.Controls.Add(groupBoxOutputOptions);
            tabPageSettings.Location = new System.Drawing.Point(4, 22);
            tabPageSettings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageSettings.Name = "tabPageSettings";
            tabPageSettings.Size = new System.Drawing.Size(1433, 586);
            tabPageSettings.TabIndex = 8;
            tabPageSettings.Text = "Settings";
            tabPageSettings.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            groupBox1.Controls.Add(label10);
            groupBox1.Controls.Add(textBoxMetadataPath);
            groupBox1.Controls.Add(pictureBox4);
            groupBox1.Controls.Add(labelTemplatePath);
            groupBox1.Controls.Add(textBoxTemplatePath);
            groupBox1.Controls.Add(pictureBox6);
            groupBox1.Location = new System.Drawing.Point(718, 3);
            groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Size = new System.Drawing.Size(704, 106);
            groupBox1.TabIndex = 117;
            groupBox1.TabStop = false;
            groupBox1.Text = "VDW input";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(7, 30);
            label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(135, 13);
            label10.TabIndex = 94;
            label10.Text = "Metadata directory (json)";
            // 
            // textBoxMetadataPath
            // 
            textBoxMetadataPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxMetadataPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            textBoxMetadataPath.Location = new System.Drawing.Point(160, 27);
            textBoxMetadataPath.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxMetadataPath.Multiline = true;
            textBoxMetadataPath.Name = "textBoxMetadataPath";
            textBoxMetadataPath.Size = new System.Drawing.Size(507, 22);
            textBoxMetadataPath.TabIndex = 93;
            // 
            // pictureBox4
            // 
            pictureBox4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            pictureBox4.Image = Properties.Resources.OpenDirectoryIcon;
            pictureBox4.Location = new System.Drawing.Point(674, 27);
            pictureBox4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new System.Drawing.Size(22, 23);
            pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pictureBox4.TabIndex = 96;
            pictureBox4.TabStop = false;
            pictureBox4.Click += pictureBox4_Click;
            // 
            // labelTemplatePath
            // 
            labelTemplatePath.AutoSize = true;
            labelTemplatePath.Location = new System.Drawing.Point(7, 60);
            labelTemplatePath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelTemplatePath.Name = "labelTemplatePath";
            labelTemplatePath.Size = new System.Drawing.Size(101, 13);
            labelTemplatePath.TabIndex = 98;
            labelTemplatePath.Text = "Template directory";
            // 
            // textBoxTemplatePath
            // 
            textBoxTemplatePath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxTemplatePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            textBoxTemplatePath.Location = new System.Drawing.Point(160, 57);
            textBoxTemplatePath.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxTemplatePath.Multiline = true;
            textBoxTemplatePath.Name = "textBoxTemplatePath";
            textBoxTemplatePath.Size = new System.Drawing.Size(507, 22);
            textBoxTemplatePath.TabIndex = 97;
            // 
            // pictureBox6
            // 
            pictureBox6.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            pictureBox6.Image = Properties.Resources.OpenDirectoryIcon;
            pictureBox6.Location = new System.Drawing.Point(674, 57);
            pictureBox6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pictureBox6.Name = "pictureBox6";
            pictureBox6.Size = new System.Drawing.Size(22, 23);
            pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pictureBox6.TabIndex = 99;
            pictureBox6.TabStop = false;
            pictureBox6.Click += PictureBoxUpdateTemplatePath_Click;
            // 
            // groupBoxConfigurationPaths
            // 
            groupBoxConfigurationPaths.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBoxConfigurationPaths.Controls.Add(pictureBox7);
            groupBoxConfigurationPaths.Controls.Add(textBoxTeamConfigurationPath);
            groupBoxConfigurationPaths.Controls.Add(label2);
            groupBoxConfigurationPaths.Controls.Add(label1);
            groupBoxConfigurationPaths.Controls.Add(comboBoxEnvironments);
            groupBoxConfigurationPaths.Controls.Add(pictureBox3);
            groupBoxConfigurationPaths.Controls.Add(textBoxTeamEnvironmentsFilePath);
            groupBoxConfigurationPaths.Controls.Add(labelTEAMConfigurationFile);
            groupBoxConfigurationPaths.Location = new System.Drawing.Point(4, 3);
            groupBoxConfigurationPaths.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxConfigurationPaths.Name = "groupBoxConfigurationPaths";
            groupBoxConfigurationPaths.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxConfigurationPaths.Size = new System.Drawing.Size(707, 210);
            groupBoxConfigurationPaths.TabIndex = 100;
            groupBoxConfigurationPaths.TabStop = false;
            groupBoxConfigurationPaths.Text = "Configuration paths";
            // 
            // pictureBox7
            // 
            pictureBox7.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            pictureBox7.Image = Properties.Resources.OpenDirectoryIcon;
            pictureBox7.Location = new System.Drawing.Point(676, 136);
            pictureBox7.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pictureBox7.Name = "pictureBox7";
            pictureBox7.Size = new System.Drawing.Size(22, 23);
            pictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pictureBox7.TabIndex = 116;
            pictureBox7.TabStop = false;
            pictureBox7.Click += pictureOpenTeamConfigurationFile_Click;
            // 
            // textBoxTeamConfigurationPath
            // 
            textBoxTeamConfigurationPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxTeamConfigurationPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            textBoxTeamConfigurationPath.Location = new System.Drawing.Point(160, 136);
            textBoxTeamConfigurationPath.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxTeamConfigurationPath.Multiline = true;
            textBoxTeamConfigurationPath.Name = "textBoxTeamConfigurationPath";
            textBoxTeamConfigurationPath.Size = new System.Drawing.Size(508, 22);
            textBoxTeamConfigurationPath.TabIndex = 114;
            toolTipVdw.SetToolTip(textBoxTeamConfigurationPath, resources.GetString("textBoxTeamConfigurationPath.ToolTip"));
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(7, 140);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(137, 13);
            label2.TabIndex = 115;
            label2.Text = "TEAM configuration path";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(7, 59);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(105, 13);
            label1.TabIndex = 113;
            label1.Text = "Active environment";
            // 
            // comboBoxEnvironments
            // 
            comboBoxEnvironments.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            comboBoxEnvironments.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxEnvironments.FormattingEnabled = true;
            comboBoxEnvironments.Location = new System.Drawing.Point(160, 55);
            comboBoxEnvironments.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            comboBoxEnvironments.Name = "comboBoxEnvironments";
            comboBoxEnvironments.Size = new System.Drawing.Size(508, 21);
            comboBoxEnvironments.TabIndex = 112;
            comboBoxEnvironments.SelectedIndexChanged += comboBoxEnvironments_SelectedIndexChanged;
            // 
            // pictureBox3
            // 
            pictureBox3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            pictureBox3.Image = Properties.Resources.OpenDirectoryIcon;
            pictureBox3.Location = new System.Drawing.Point(676, 25);
            pictureBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new System.Drawing.Size(22, 23);
            pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 95;
            pictureBox3.TabStop = false;
            pictureBox3.Click += pictureBoxOpenEnvironmentFile_Click;
            // 
            // textBoxTeamEnvironmentsFilePath
            // 
            textBoxTeamEnvironmentsFilePath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxTeamEnvironmentsFilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            textBoxTeamEnvironmentsFilePath.Location = new System.Drawing.Point(160, 25);
            textBoxTeamEnvironmentsFilePath.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxTeamEnvironmentsFilePath.Multiline = true;
            textBoxTeamEnvironmentsFilePath.Name = "textBoxTeamEnvironmentsFilePath";
            textBoxTeamEnvironmentsFilePath.Size = new System.Drawing.Size(508, 22);
            textBoxTeamEnvironmentsFilePath.TabIndex = 83;
            toolTipVdw.SetToolTip(textBoxTeamEnvironmentsFilePath, resources.GetString("textBoxTeamEnvironmentsFilePath.ToolTip"));
            // 
            // labelTEAMConfigurationFile
            // 
            labelTEAMConfigurationFile.AutoSize = true;
            labelTEAMConfigurationFile.Location = new System.Drawing.Point(7, 30);
            labelTEAMConfigurationFile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelTEAMConfigurationFile.Name = "labelTEAMConfigurationFile";
            labelTEAMConfigurationFile.Size = new System.Drawing.Size(128, 13);
            labelTEAMConfigurationFile.TabIndex = 84;
            labelTEAMConfigurationFile.Text = "TEAM environments file";
            // 
            // groupBoxOutputOptions
            // 
            groupBoxOutputOptions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            groupBoxOutputOptions.Controls.Add(pictureBox5);
            groupBoxOutputOptions.Controls.Add(textBoxSchemaName);
            groupBoxOutputOptions.Controls.Add(label12);
            groupBoxOutputOptions.Controls.Add(OutputPathLabel);
            groupBoxOutputOptions.Controls.Add(textBoxOutputPath);
            groupBoxOutputOptions.Location = new System.Drawing.Point(718, 117);
            groupBoxOutputOptions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxOutputOptions.Name = "groupBoxOutputOptions";
            groupBoxOutputOptions.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBoxOutputOptions.Size = new System.Drawing.Size(704, 97);
            groupBoxOutputOptions.TabIndex = 89;
            groupBoxOutputOptions.TabStop = false;
            groupBoxOutputOptions.Text = "VDW output";
            // 
            // pictureBox5
            // 
            pictureBox5.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            pictureBox5.Image = Properties.Resources.OpenDirectoryIcon;
            pictureBox5.Location = new System.Drawing.Point(674, 57);
            pictureBox5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new System.Drawing.Size(22, 23);
            pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pictureBox5.TabIndex = 97;
            pictureBox5.TabStop = false;
            pictureBox5.Click += pictureBox5_Click;
            // 
            // textBoxSchemaName
            // 
            textBoxSchemaName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxSchemaName.Location = new System.Drawing.Point(160, 27);
            textBoxSchemaName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxSchemaName.Name = "textBoxSchemaName";
            textBoxSchemaName.Size = new System.Drawing.Size(507, 22);
            textBoxSchemaName.TabIndex = 91;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new System.Drawing.Point(7, 30);
            label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(105, 13);
            label12.TabIndex = 92;
            label12.Text = "VDW schema name";
            // 
            // OutputPathLabel
            // 
            OutputPathLabel.AutoSize = true;
            OutputPathLabel.Location = new System.Drawing.Point(7, 60);
            OutputPathLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            OutputPathLabel.Name = "OutputPathLabel";
            OutputPathLabel.Size = new System.Drawing.Size(131, 13);
            OutputPathLabel.TabIndex = 82;
            OutputPathLabel.Text = "Output (spool) directory";
            // 
            // textBoxOutputPath
            // 
            textBoxOutputPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxOutputPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            textBoxOutputPath.Location = new System.Drawing.Point(160, 57);
            textBoxOutputPath.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxOutputPath.Multiline = true;
            textBoxOutputPath.Name = "textBoxOutputPath";
            textBoxOutputPath.Size = new System.Drawing.Size(507, 22);
            textBoxOutputPath.TabIndex = 81;
            // 
            // tabPageHome
            // 
            tabPageHome.Controls.Add(labelWelcome);
            tabPageHome.Controls.Add(pictureBox2);
            tabPageHome.Location = new System.Drawing.Point(4, 22);
            tabPageHome.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageHome.Name = "tabPageHome";
            tabPageHome.Size = new System.Drawing.Size(1433, 586);
            tabPageHome.TabIndex = 12;
            tabPageHome.Text = "Home";
            tabPageHome.UseVisualStyleBackColor = true;
            // 
            // labelWelcome
            // 
            labelWelcome.AutoSize = true;
            labelWelcome.Location = new System.Drawing.Point(152, 36);
            labelWelcome.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelWelcome.Name = "labelWelcome";
            labelWelcome.Size = new System.Drawing.Size(214, 13);
            labelWelcome.TabIndex = 85;
            labelWelcome.Text = "Welcome to the Virtual Data Warehouse";
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.RavosLogo;
            pictureBox2.Location = new System.Drawing.Point(40, 36);
            pictureBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new System.Drawing.Size(104, 100);
            pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 18;
            pictureBox2.TabStop = false;
            // 
            // tabControlMain
            // 
            tabControlMain.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tabControlMain.Controls.Add(tabPageHome);
            tabControlMain.Controls.Add(tabPageSettings);
            tabControlMain.Location = new System.Drawing.Point(14, 37);
            tabControlMain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new System.Drawing.Size(1441, 612);
            tabControlMain.TabIndex = 3;
            tabControlMain.SelectedIndexChanged += tabControlMain_SelectedIndexChanged;
            // 
            // backgroundWorkerEventLog
            // 
            backgroundWorkerEventLog.WorkerReportsProgress = true;
            backgroundWorkerEventLog.WorkerSupportsCancellation = true;
            backgroundWorkerEventLog.DoWork += backgroundWorkerEventLog_DoWork;
            backgroundWorkerEventLog.ProgressChanged += backgroundWorkerEventLog_ProgressChanged;
            backgroundWorkerEventLog.RunWorkerCompleted += backgroundWorkerEventLog_RunWorkerCompleted;
            // 
            // toolTipVdw
            // 
            toolTipVdw.AutoPopDelay = 5000;
            toolTipVdw.InitialDelay = 300;
            toolTipVdw.ReshowDelay = 100;
            // 
            // clearEventLogToolStripMenuItem
            // 
            clearEventLogToolStripMenuItem.Image = Properties.Resources.log_file;
            clearEventLogToolStripMenuItem.Name = "clearEventLogToolStripMenuItem";
            clearEventLogToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            clearEventLogToolStripMenuItem.Text = "Clear Event Log";
            clearEventLogToolStripMenuItem.Click += clearEventLogToolStripMenuItem_Click;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoScroll = true;
            BackColor = System.Drawing.SystemColors.Control;
            ClientSize = new System.Drawing.Size(1469, 808);
            Controls.Add(button12);
            Controls.Add(groupBox9);
            Controls.Add(pictureBox1);
            Controls.Add(SQLGenerationGroupBox);
            Controls.Add(tabControlMain);
            Controls.Add(menuStripMainMenu);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStripMainMenu;
            Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            MinimumSize = new System.Drawing.Size(931, 686);
            Name = "FormMain";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Virtual Data Warehouse";
            menuStripMainMenu.ResumeLayout(false);
            menuStripMainMenu.PerformLayout();
            SQLGenerationGroupBox.ResumeLayout(false);
            SQLGenerationGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox9.ResumeLayout(false);
            tabPageSettings.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
            groupBoxConfigurationPaths.ResumeLayout(false);
            groupBoxConfigurationPaths.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            groupBoxOutputOptions.ResumeLayout(false);
            groupBoxOutputOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            tabPageHome.ResumeLayout(false);
            tabPageHome.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            tabControlMain.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStripMainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openOutputDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxGenerateInDatabase;
        private System.Windows.Forms.GroupBox SQLGenerationGroupBox;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.PictureBox pictureBox1;
        public System.ComponentModel.BackgroundWorker backgroundWorkerActivateMetadata;
        private System.Windows.Forms.ToolStripMenuItem saveConfigurationFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem openTEAMConfigurationSettingsFileToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.RichTextBox richTextBoxInformationMain;
        private System.Windows.Forms.CheckBox checkBoxGenerateJsonSchema;
        private System.Windows.Forms.CheckBox checkBoxSaveToFile;
        private System.Windows.Forms.ToolStripMenuItem openVDWConfigurationSettingsFileToolStripMenuItem;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.TabPage tabPageSettings;
        internal System.Windows.Forms.TextBox textBoxTeamEnvironmentsFilePath;
        internal System.Windows.Forms.TextBox textBoxOutputPath;
        internal System.Windows.Forms.GroupBox groupBoxOutputOptions;
        internal System.Windows.Forms.TextBox textBoxSchemaName;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label labelTEAMConfigurationFile;
        private System.Windows.Forms.Label OutputPathLabel;
        private System.Windows.Forms.TabPage tabPageHome;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.GroupBox groupBoxConfigurationPaths;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.ToolStripMenuItem openVDWConfigurationDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openInputDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem openTemplateCollectionFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveTemplateCollectionFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openTemplateDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxEnvironments;
        private System.Windows.Forms.ToolStripMenuItem displayEventLogToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker backgroundWorkerEventLog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox7;
        internal System.Windows.Forms.TextBox textBoxTeamConfigurationPath;
        private System.Windows.Forms.Label label2;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label label10;
        internal System.Windows.Forms.TextBox textBoxMetadataPath;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Label labelTemplatePath;
        internal System.Windows.Forms.TextBox textBoxTemplatePath;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.ToolTip toolTipVdw;
        private System.Windows.Forms.Label labelWelcome;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openCoreDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearEventLogToolStripMenuItem;
    }
}

