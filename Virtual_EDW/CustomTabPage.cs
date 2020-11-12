using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using HandlebarsDotNet;
using Newtonsoft.Json;
using TEAM;

namespace Virtual_Data_Warehouse
{
    // Delegate to control main form text box (append text)
    public class MyEventArgs : EventArgs
    {
        public string Value { get; set; }

        public MyEventArgs(string value)
        {
            Value = value;
        }
    }

    // Delegate to control main form text box (clear)
    public class MyClearArgs : EventArgs
    {        public MyClearArgs()
        {
        }
    }


    class CustomTabPage : TabPage
    {

        string inputNiceName;
        internal Dictionary<string, VDW_DataObjectMappingList> itemList;
        internal string connectionString;

        // Objects on main Tab Page
        Panel localPanel;
        Label localLabelProcessing;
        CheckBox localCheckBoxSelectAll;
        CheckedListBox localCheckedListBox;
        RichTextBox localRichTextBox;
        Button localButtonGenerate;
        GroupBox localGroupBoxFilter;
        TextBox localTextBoxFilter;

        // Objects on the sub Tab Control (on the main Tab Page)
        TabControl localTabControl;

        TabPage tabPageGenerationOutput;
        RichTextBox localRichTextBoxGenerationOutput;

        TabPage tabPageGenerationPattern;
        Label localLabelActivePattern;
        Label localLabelFilePath;
        Label localLabelFullFilePath;
        ComboBox localComboBoxGenerationPattern;
        RichTextBox localRichTextBoxGenerationPattern;
        Button localSavePattern;      

        #region Main Form CheckBox value handling
        // Values for the checkboxes from main form
        public bool displayJsonFlag { get; set; }
        public bool generateInDatabaseFlag { get; set; }
        public bool saveOutputFileFlag { get; set; }

        internal bool startUpIndicator = true;


        public void setDisplayJsonFlag(bool value)
        {
            displayJsonFlag = value;
        }

        public void setGenerateInDatabaseFlag(bool value)
        {
            generateInDatabaseFlag = value;
        }

        public void setSaveOutputFileFlag(bool value)
        {
            saveOutputFileFlag = value;
        }
        #endregion

        /// <summary>
        /// Constructor to instantiate a new Custom Tab Page
        /// </summary>
        public CustomTabPage(string classification, string notes, Dictionary<string, VDW_DataObjectMappingList> itemList, string connectionString)
        {
            // Register the Handlebars helpers (extensions)
            ClassHandlebarsHelpers.RegisterHandleBarsHelpers();

            this.itemList = itemList;
            this.connectionString = connectionString;

            inputNiceName = Regex.Replace(classification, "(\\B[A-Z])", " $1");

            displayJsonFlag = true;
            generateInDatabaseFlag = true;
            saveOutputFileFlag = true;

            #region Main Tab Page

            // Base properties of the custom tab page
            Name = $"{classification}";
            Text = inputNiceName;
            BackColor = Color.Transparent;
            BorderStyle = BorderStyle.None;
            UseVisualStyleBackColor = true;
            Size = new Size(1330, 601);
            AutoSizeMode = AutoSizeMode.GrowOnly;
            AutoSize = true;

            // Add Panel to facilitate docking
            localPanel = new Panel();
            Controls.Add(localPanel);
            localPanel.Dock = DockStyle.Fill;
            localPanel.AutoSize = true;

            // Add 'Generate' Button 
            localButtonGenerate = new Button();
            localPanel.Controls.Add(localButtonGenerate);
            localButtonGenerate.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            localButtonGenerate.Location = new Point(17, 542);
            localButtonGenerate.Name = $"enerate{classification}";
            localButtonGenerate.Size = new Size(109, 40);
            localButtonGenerate.Text = $"Generate {inputNiceName}";
            localButtonGenerate.Click += new EventHandler(Generate);

            // Add 'Processing' Label
            localLabelProcessing = new Label();
            localPanel.Controls.Add(localLabelProcessing);
            localLabelProcessing.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localLabelProcessing.Location = new Point(14, 12);
            localLabelProcessing.Size = new Size(280, 13);
            localLabelProcessing.Name = $"label{classification}Processing";
            localLabelProcessing.Text = $"{inputNiceName} Processing";


            // Add 'Select All' CheckBox
            localCheckBoxSelectAll = new CheckBox();
            localPanel.Controls.Add(localCheckBoxSelectAll);
            localCheckBoxSelectAll.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localCheckBoxSelectAll.Location = new Point(346, 11);
            localCheckBoxSelectAll.Size = new Size(69, 17);
            localCheckBoxSelectAll.Name = "checkBoxSelectAll";
            localCheckBoxSelectAll.Checked = true;
            localCheckBoxSelectAll.Text = "Select all";
            localCheckBoxSelectAll.CheckedChanged += new EventHandler(SelectAllCheckBoxItems);

            // Add Checked List Box
            localCheckedListBox = new CheckedListBox();
            localPanel.Controls.Add(localCheckedListBox);
            localCheckedListBox.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left);
            localCheckedListBox.Location = new Point(17, 31);
            localCheckedListBox.Size = new Size(393, 377);
            localCheckedListBox.BorderStyle = BorderStyle.FixedSingle;
            localCheckedListBox.CheckOnClick = true;
            localCheckedListBox.Name = $"checkedListBox{classification}";

            // Add User feedback RichTextBox (left hand side of form)
            localRichTextBox = new RichTextBox();
            localPanel.Controls.Add(localRichTextBox);
            localRichTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left);
            localRichTextBox.Location = new Point(17, 418);
            localRichTextBox.Size = new Size(393, 115);
            localRichTextBox.BorderStyle = BorderStyle.None;
            localRichTextBox.BackColor = SystemColors.Window;
            localRichTextBox.TextChanged += new EventHandler(ScrollToCaret);

            // Add 'Filter' Group Box
            localGroupBoxFilter = new GroupBox();
            localPanel.Controls.Add(localGroupBoxFilter);
            localGroupBoxFilter.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            localGroupBoxFilter.Location = new Point(247, 539);
            localGroupBoxFilter.Size = new Size(163, 43);
            localGroupBoxFilter.Text = "Filter Criterion";
            localGroupBoxFilter.Name = $"groupBoxFilter{inputNiceName}";

            // Add 'Filter' Text Box
            localTextBoxFilter = new TextBox();
            localGroupBoxFilter.Controls.Add(localTextBoxFilter);
            localTextBoxFilter.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            localTextBoxFilter.Location = new Point(6, 15);
            localTextBoxFilter.Size = new Size(151, 20);
            localTextBoxFilter.Name = $"textBoxFilterCriterion{inputNiceName}";
            localTextBoxFilter.TextChanged += new EventHandler(FilterItemList);

            #endregion

            #region Sub Tab Pages

            // Add Sub Tab Control
            localTabControl = new TabControl();
            localPanel.Controls.Add(localTabControl);
            localTabControl.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            localTabControl.Location = new Point(416, 9);
            localTabControl.Size = new Size(896, 573);
            localTabControl.Name = $"tabControl{classification}";
            localTabControl.BackColor = Color.White;
            localTabControl.SelectedIndexChanged += new EventHandler(SyntaxHighlightsHandlebars);

            // Add 'Generation Output' Tab Page on Sub Tab
            tabPageGenerationOutput = new TabPage($"{inputNiceName} Generation Output");
            localTabControl.TabPages.Add(tabPageGenerationOutput);
            tabPageGenerationOutput.BackColor = Color.Transparent;
            tabPageGenerationOutput.BorderStyle = BorderStyle.None;
            tabPageGenerationOutput.UseVisualStyleBackColor = true;

            // Add 'Generation Output' RichTextBox to Generation Output tab
            localRichTextBoxGenerationOutput = new RichTextBox();
            tabPageGenerationOutput.Controls.Add(localRichTextBoxGenerationOutput);
            localRichTextBoxGenerationOutput.Dock = DockStyle.Fill;
            localRichTextBoxGenerationOutput.Text = $"No {inputNiceName} logic has been generated at the moment.";
            localRichTextBoxGenerationOutput.Location = new Point(3, 6);
            localRichTextBoxGenerationOutput.Size = new Size(882, 535);
            localRichTextBoxGenerationOutput.BorderStyle = BorderStyle.None;

            // Add 'Pattern' Tab Page to on Sub Tab
            tabPageGenerationPattern = new TabPage($"{inputNiceName} Pattern");
            localTabControl.TabPages.Add(tabPageGenerationPattern);
            tabPageGenerationPattern.BackColor = Color.Transparent;
            tabPageGenerationPattern.BorderStyle = BorderStyle.None;
            tabPageGenerationPattern.UseVisualStyleBackColor = true;

            // Add 'Pattern ComboBox' to Pattern tab
            localComboBoxGenerationPattern = new ComboBox();
            tabPageGenerationPattern.Controls.Add(localComboBoxGenerationPattern);
            localComboBoxGenerationPattern.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localComboBoxGenerationPattern.Location = new Point(108, 8);
            localComboBoxGenerationPattern.Size = new Size(496, 21);
            localComboBoxGenerationPattern.Name = $"comboBox{classification}Pattern";
            localComboBoxGenerationPattern.SelectedIndexChanged += new EventHandler(DisplayPattern);

            // Add 'Active Pattern' Label
            localLabelActivePattern = new Label();
            tabPageGenerationPattern.Controls.Add(localLabelActivePattern);
            localLabelActivePattern.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localLabelActivePattern.Location = new Point(2, 11);
            localLabelActivePattern.Size = new Size(77, 13);
            localLabelActivePattern.Name = $"label{classification}ActivePattern";
            localLabelActivePattern.Text = "Active Pattern:";

            // Add 'File Path' Label
            localLabelFilePath = new Label();
            tabPageGenerationPattern.Controls.Add(localLabelFilePath);
            localLabelFilePath.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localLabelFilePath.Location = new Point(2, 34);
            localLabelFilePath.Size = new Size(60, 13);
            localLabelFilePath.Name = $"label{classification}FilePath";
            localLabelFilePath.Text = @"File Path:";

            // Add 'Full Path' Label
            localLabelFullFilePath = new Label();
            tabPageGenerationPattern.Controls.Add(localLabelFullFilePath);
            localLabelFullFilePath.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localLabelFullFilePath.Location = new Point(105, 34);
            localLabelFullFilePath.Size = new Size(650, 13);
            localLabelFullFilePath.Name = $"label{classification}FullFilePath";
            localLabelFullFilePath.Text = @"<path>";

            // Add 'Save Pattern' Button
            localSavePattern = new Button();
            tabPageGenerationPattern.Controls.Add(localSavePattern);
            localSavePattern.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localSavePattern.Location = new Point(610, 7);
            localSavePattern.Size = new Size(101, 23);
            localSavePattern.Text = $"Save updates";
            localSavePattern.Name = $"Generate{classification}";
            localSavePattern.Click += new EventHandler(SavePattern);

            // Add 'Generation Pattern' RichTextBox to Pattern tab
            localRichTextBoxGenerationPattern = new RichTextBox();
            tabPageGenerationPattern.Controls.Add(localRichTextBoxGenerationPattern);
            //localRichTextBoxGenerationPattern.Size = new Size(1, 1);
            localRichTextBoxGenerationPattern.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            localRichTextBoxGenerationPattern.Location = new Point(3, 59);
            //localRichTextBoxGenerationPattern.Size = new Size(882, 485);
            localRichTextBoxGenerationPattern.Size = new Size(195, 35);
            localRichTextBoxGenerationPattern.BorderStyle = BorderStyle.None;
            //localRichTextBoxGenerationPattern.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            #endregion

            #region Constructor Methods

            // Populate the Combo Box
            LoadTabPageComboBox(classification);

            // Populate the Checked List Box
            SetItemList(itemList);

            // Report back to the user if there is not metadata available
            if (itemList.Count == 0)
            {
                localRichTextBox.Text =
                    $"There was no metadata available to display {inputNiceName} content. Please check the associated metadata schema (are there any {inputNiceName} records available?) or the database connection.\r\n\r\n";
            }

            // Initial documentation as per the definition notes
            localRichTextBox.AppendText(notes);

            // Prevention of double hitting of some event handlers
            startUpIndicator = false;

            #endregion
        }

        /// <summary>
        /// Automatically scroll to the end of the text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ScrollToCaret(object sender, EventArgs e)
        {
            localRichTextBox.SelectionStart = localRichTextBox.Text.Length;
            localRichTextBox.ScrollToCaret();
        }

        public void SyntaxHighlightsHandlebars(object sender, EventArgs e)
        {
            TextHandling.SyntaxHighlightHandlebars(localRichTextBoxGenerationPattern, localRichTextBoxGenerationPattern.Text.TrimEnd());
        }

        public void FilterItemList(object o, EventArgs e)
        {
            SetItemList(itemList);
        }

        public void SetItemList(Dictionary<string, VDW_DataObjectMappingList> itemList)
        {
            // Copy the input variable to the local item list
            this.itemList = itemList;

            // Clear the existing checkboxes
            localCheckedListBox.Items.Clear();

            // Add items to the Checked List Box, if it satisfies the filter criterion
            foreach (string item in itemList.Keys)
            {
                if (item.Contains(localTextBoxFilter.Text))
                {
                    localCheckedListBox.Items.Add(item);
                }
            }

            // Report back to the user if there is not metadata available
            if (localCheckedListBox.Items.Count == 0)
            {
                localRichTextBox.Text =
                    $"There was no metadata available to display {inputNiceName} content. Please check the associated metadata schema (are there any {inputNiceName} records available?) or the database connection.";
            }

            // Set all the Check Boxes to 'checked'
            for (int x = 0; x <= localCheckedListBox.Items.Count - 1; x++)
            {
                CheckAllCheckBoxes();
            }
        }
        
        private void CheckAllCheckBoxes()
        {
            for (int x = 0; x <= localCheckedListBox.Items.Count - 1; x++)
            {
                localCheckedListBox.SetItemChecked(x, localCheckBoxSelectAll.Checked);
            }
        }

        private void DisplayPattern(object o, EventArgs e)
        {
            // Retrieve all the info for the pattern name from memory (from the list of patterns)
            var loadPattern = FormBase.VdwConfigurationSettings.patternList.FirstOrDefault(x => x.LoadPatternName == localComboBoxGenerationPattern.Text);
            
            // Set the label with the path so it's visible to the user where the file is located
            string localFullPath = Path.Combine(FormBase.VdwConfigurationSettings.LoadPatternPath, loadPattern.LoadPatternFilePath);

            localLabelFullFilePath.Text = localFullPath;

            //Path.Combine(Environment.CurrentDirectory, "Some\\Path.txt"));

            // Read the file from the path
            string loadPatternTemplate ="";
            try
            {
                loadPatternTemplate = File.ReadAllText(localFullPath);
            }
            catch
            {
                loadPatternTemplate = $"There was an error loading the pattern specified in the load pattern collection file.\r\n\r\nDoes '{loadPattern.LoadPatternFilePath}' exist and is the path correct?\r\n\r\nIf this is not the case please update the load pattern collection information in the 'settings' tab.";
            }

            // Display the pattern in the text box on the screen
            localRichTextBoxGenerationPattern.Text = loadPatternTemplate;

            // Syntax highlight for Handlebars
            if (startUpIndicator == false)
            {
                TextHandling.SyntaxHighlightHandlebars(localRichTextBoxGenerationPattern,
                    localRichTextBoxGenerationPattern.Text);
            }
        }

        private void SelectAllCheckBoxItems(object o, EventArgs e)
        {
            CheckAllCheckBoxes();
        }

        public event EventHandler<MyEventArgs> OnChangeMainText = delegate { };

        public event EventHandler<MyClearArgs> OnClearMainText = delegate { };

        public void RaiseOnChangeMainText(string inputText)
        {
            OnChangeMainText(this, new MyEventArgs(inputText));
        }

        public void RaiseOnClearMainText()
        {
            OnClearMainText(this, new MyClearArgs());
        }

        void SavePattern(object o, EventArgs e)
        {
            RaiseOnClearMainText();
            string backupResponse = LoadPattern.BackupLoadPattern(localLabelFullFilePath.Text);
            string saveResponse = "";

            if (backupResponse.StartsWith("A backup was created"))
            {
                RaiseOnChangeMainText(backupResponse);
                saveResponse = LoadPattern.SaveLoadPattern(localLabelFullFilePath.Text, localRichTextBoxGenerationPattern.Text);
                RaiseOnChangeMainText("\r\n\r\n" + saveResponse);
            }
            else
            {
                RaiseOnChangeMainText(backupResponse);
                RaiseOnChangeMainText(saveResponse);
            }
        }

        void Generate(object o, EventArgs e)
        {
            localTabControl.SelectedIndex = 0;
            DoWork();
        }

        void DoWork()
        {
            //VdwUtility.CreateSchema(FormBase.TeamConfigurationSettings.ConnectionStringStg);
            // TO DO: retrieve correct schema
            localRichTextBox.Clear();
            GenerateFromPattern();
        }

        /// <summary>
        ///   Create output using Handlebars as templating engine
        /// </summary>
        private void GenerateFromPattern()
        {
            EventLog eventLog = new EventLog();

            localRichTextBoxGenerationOutput.Clear();
            RaiseOnClearMainText();
            localTabControl.SelectedIndex = 0;

            // Loop through the checked items, select the right mapping and generate the pattern
            if (localCheckedListBox.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= localCheckedListBox.CheckedItems.Count - 1; x++)
                {
                    var targetTableName = localCheckedListBox.CheckedItems[x].ToString();
                    localRichTextBox.AppendText(@"Processing generation for " + targetTableName + ".\r\n");

                    // Only process the selected items in the total of available source-to-target mappings
                    VDW_DataObjectMappingList dataObjectMappingList = new VDW_DataObjectMappingList();
                    itemList.TryGetValue(targetTableName, out dataObjectMappingList);

                    // Return the result to the user
                    try
                    {
                        // Compile the template, and merge it with the metadata
                        var template = Handlebars.Compile(localRichTextBoxGenerationPattern.Text);
                        var result = template(dataObjectMappingList);

                        // Check if the metadata needs to be displayed
                        if (displayJsonFlag)
                        {
                            try
                            {
                                var json = JsonConvert.SerializeObject(dataObjectMappingList, Formatting.Indented);
                                localRichTextBoxGenerationOutput.AppendText(json + "\r\n\r\n");
                            }
                            catch (Exception ex)
                            {
                                RaiseOnChangeMainText("An error was encountered while generating the JSON metadata. The error message is: " + ex);
                            }
                        }

                        // Display the output of the template to the user
                        localRichTextBoxGenerationOutput.AppendText(result);

                        // Spool the output to disk
                        EventLog fileSaveEventLog = new EventLog();
                        if (saveOutputFileFlag)
                        {
                            fileSaveEventLog = VdwUtility.SaveOutputToDisk(FormBase.VdwConfigurationSettings.VdwOutputPath + targetTableName + ".sql", result);
                        }

                        //Generate in database
                        EventLog databaseEventLog = new EventLog();
                        if (generateInDatabaseFlag)
                        {



                            //var localConn = VdwUtility.MatchConnectionKey(connectionString);
                            var localConn = FormBase.TeamConfigurationSettings.MetadataConnection.CreateSqlServerConnectionString(false);
                            var conn = new SqlConnection { ConnectionString = localConn};

                            databaseEventLog = VdwUtility.ExecuteOutputInDatabase(conn, result);
                        }

                        eventLog.AddRange(fileSaveEventLog);
                        eventLog.AddRange(databaseEventLog);

                    }
                    catch (Exception ex)
                    {
                        eventLog.Add(Event.CreateNewEvent(EventTypes.Error, "The template could not be compiled. The message is: " + ex.Message + "\r\n\r\n"));
                    }
                }
            }
            else
            {
                localRichTextBox.AppendText($"There was no metadata selected to generate {inputNiceName} code. Please check the metadata schema - are there any {inputNiceName} objects selected?");
            }

            //connOmd.Close();
            //connOmd.Dispose();

            // Report back to the user
            int errorCounter = 0;
            foreach (Event individualEvent in eventLog)
            {
                if (individualEvent.eventCode == 1)
                {
                    errorCounter++;
                }

                RaiseOnChangeMainText(individualEvent.eventDescription);
            }

            RaiseOnChangeMainText($"\r\n{errorCounter} error(s) have been found.\r\n");
            RaiseOnChangeMainText($"\r\nAssociated scripts have been saved in {FormBase.VdwConfigurationSettings.VdwOutputPath}.\r\n");

            // Apply syntax highlighting
            SyntaxHighlight();
        }

        public void SyntaxHighlight()
        {
            TextHandling.SyntaxHighlightSql(localRichTextBoxGenerationOutput, localRichTextBoxGenerationOutput.Text);
        }        

        private void LoadTabPageComboBox(string patternClassification)
        {
            bool available = false;
            try
            {
                var patternList = FormBase.VdwConfigurationSettings.patternList;

                localComboBoxGenerationPattern.Items.Clear();
                localRichTextBoxGenerationPattern.Clear();
                localLabelFullFilePath.Text = "";

                foreach (var patternDetail in patternList)
                {
                    if (patternDetail.LoadPatternType == patternClassification)
                    {
                        localComboBoxGenerationPattern.Items.Add(patternDetail.LoadPatternName);
                        available = true;
                    }
                }
            }
            catch (Exception ex)
            {
                RaiseOnChangeMainText(ex.ToString());
            }

            if (available)
            {
                localComboBoxGenerationPattern.SelectedItem = localComboBoxGenerationPattern.Items[0];
            }
            else
            {
                localComboBoxGenerationPattern.ResetText();
                localComboBoxGenerationPattern.SelectedIndex = -1;
            }
        }

        public void ClearTextBoxes(RichTextBox inputRichTextBox, RichTextBox localRichTextBoxGenerationOutput)
        {
            inputRichTextBox.Clear();
            localRichTextBoxGenerationOutput.Clear();            
        }

        private StringBuilder LoadCheckedListBox(string patternNiceName, StringBuilder inputQuery, SqlConnection conn)
        {
            StringBuilder returnDetails = new StringBuilder();

            // Clear the existing checkboxes
            localCheckedListBox.Items.Clear();


 

            for (int x = 0; x <= localCheckedListBox.Items.Count - 1; x++)
            {
                localCheckedListBox.SetItemChecked(x, localCheckBoxSelectAll.Checked);
            }
            return returnDetails;
        }
    }
}
