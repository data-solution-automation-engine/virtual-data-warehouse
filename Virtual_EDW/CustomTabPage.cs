using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using HandlebarsDotNet;
using Newtonsoft.Json;
using DataWarehouseAutomation;
using TEAM_Library;

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
    {
        public MyClearArgs()
        {
        }
    }

    /// <summary>
    /// Custom Tab Page to generate a tab connected to a specific pattern classification..
    /// </summary>
    class CustomTabPage : TabPage
    {
        readonly string _inputNiceName;
        internal Dictionary<string, VDW_DataObjectMappingList> ItemList;

        // Objects on main Tab Page
        readonly CheckBox _localCheckBoxSelectAll;
        readonly CheckedListBox _localCheckedListBox;
        readonly TextBox _localTextBoxFilter;

        // Objects on the sub Tab Control (on the main Tab Page)
        readonly TabControl localTabControl;

        readonly TabPage tabPageGenerationOutput;
        readonly RichTextBox localRichTextBoxGenerationOutput;

        readonly TabPage tabPageGenerationPattern;
        // Active pattern
        readonly Label localLabelActivePattern;
        readonly Label localLabelFilePath;
        readonly Label localLabelFullFilePath;
        // Assigned connection
        readonly Label localLabelActiveConnectionKey;
        readonly Label localLabelActiveConnectionKeyValue;
        // Output file pattern
        readonly Label localLabelOutputFilePattern;
        readonly Label localLabelOutputFilePatternValue;

        readonly ComboBox localComboBoxGenerationPattern;
        readonly RichTextBox localRichTextBoxGenerationPattern;
        readonly Button localSavePattern;      

        #region Main Form CheckBox value handling
        // Values for the checkboxes from main form
        public bool DisplayJsonFlag { get; set; }
        public bool GenerateInDatabaseFlag { get; set; }
        public bool SaveOutputFileFlag { get; set; }

        internal bool StartUpIndicator = true;


        /// <summary>
        /// Function can be called from the main form as well this wya.
        /// </summary>
        public void ApplySyntaxHighlightingForHandlebars()
        {
            localRichTextBoxGenerationPattern.Rtf = TextHandling.SyntaxHighlightHandlebars(localRichTextBoxGenerationPattern.Text.TrimEnd()).Rtf;
        }

        public void SetDisplayJsonFlag(bool value)
        {
            DisplayJsonFlag = value;
        }

        public void SetGenerateInDatabaseFlag(bool value)
        {
            GenerateInDatabaseFlag = value;
        }

        public void SetSaveOutputFileFlag(bool value)
        {
            SaveOutputFileFlag = value;
        }
        #endregion

        /// <summary>
        /// Constructor to instantiate a new Custom Tab Page
        /// </summary>
        public CustomTabPage(string classification, string notes, Dictionary<string, VDW_DataObjectMappingList> itemList)
        {
            // Register the Handlebars helpers (extensions), these are maintained in the DataWarehouseAutomation class library.
            HandleBarsHelpers.RegisterHandleBarsHelpers();

            this.ItemList = itemList;

            _inputNiceName = Regex.Replace(classification, "(\\B[A-Z])", " $1");

            DisplayJsonFlag = true;
            GenerateInDatabaseFlag = true;
            SaveOutputFileFlag = true;

            #region Main Tab Page

            // Base properties of the custom tab page
            Name = $"{classification}";
            Text = _inputNiceName;
            BackColor = Color.Transparent;
            BorderStyle = BorderStyle.None;
            UseVisualStyleBackColor = true;
            Size = new Size(1330, 601);
            AutoSizeMode = AutoSizeMode.GrowOnly;
            AutoSize = true;

            // Add Panel to facilitate docking
            var localPanel = new Panel();
            Controls.Add(localPanel);
            localPanel.Dock = DockStyle.Fill;
            localPanel.AutoSize = true;

            // Add 'Generate' Button 
            var localButtonGenerate = new Button();
            localPanel.Controls.Add(localButtonGenerate);
            localButtonGenerate.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            localButtonGenerate.Location = new Point(17, 542);
            localButtonGenerate.Name = $"Generate{classification}";
            localButtonGenerate.Size = new Size(170, 40);
            localButtonGenerate.Text = $"Generate {_inputNiceName}";
            localButtonGenerate.Click += Generate;

            // Add 'Processing' Label
            var localLabelProcessing = new Label();
            localPanel.Controls.Add(localLabelProcessing);
            localLabelProcessing.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localLabelProcessing.Location = new Point(14, 12);
            localLabelProcessing.Size = new Size(280, 13);
            localLabelProcessing.Name = $"label{classification}Processing";
            localLabelProcessing.Text = $"{_inputNiceName} Processing";


            // Add 'Select All' CheckBox
            _localCheckBoxSelectAll = new CheckBox();
            localPanel.Controls.Add(_localCheckBoxSelectAll);
            _localCheckBoxSelectAll.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            _localCheckBoxSelectAll.Location = new Point(346, 11);
            _localCheckBoxSelectAll.Size = new Size(69, 17);
            _localCheckBoxSelectAll.Name = "checkBoxSelectAll";
            _localCheckBoxSelectAll.Checked = true;
            _localCheckBoxSelectAll.Text = "Select all";
            _localCheckBoxSelectAll.CheckedChanged += SelectAllCheckBoxItems;

            // Add Checked List Box
            _localCheckedListBox = new CheckedListBox();
            localPanel.Controls.Add(_localCheckedListBox);
            _localCheckedListBox.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left);
            _localCheckedListBox.Location = new Point(17, 31);
            _localCheckedListBox.Size = new Size(393, 510);
            _localCheckedListBox.BorderStyle = BorderStyle.FixedSingle;
            _localCheckedListBox.CheckOnClick = true;
            _localCheckedListBox.Name = $"checkedListBox{classification}";

            // Add 'Filter' Group Box
            var localGroupBoxFilter = new GroupBox();
            localPanel.Controls.Add(localGroupBoxFilter);
            localGroupBoxFilter.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            localGroupBoxFilter.Location = new Point(247, 539);
            localGroupBoxFilter.Size = new Size(163, 43);
            localGroupBoxFilter.Text = "Filter Criterion";
            localGroupBoxFilter.Name = $"groupBoxFilter{_inputNiceName}";

            // Add 'Filter' Text Box
            _localTextBoxFilter = new TextBox();
            localGroupBoxFilter.Controls.Add(_localTextBoxFilter);
            _localTextBoxFilter.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            _localTextBoxFilter.Location = new Point(6, 15);
            _localTextBoxFilter.Size = new Size(151, 20);
            _localTextBoxFilter.Name = $"textBoxFilterCriterion{_inputNiceName}";
            _localTextBoxFilter.TextChanged += FilterItemList;

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
            localTabControl.SelectedIndexChanged += SubTabClick;

            // Add 'Generation Output' Tab Page on Sub Tab
            tabPageGenerationOutput = new TabPage($"{_inputNiceName} Generation Output");
            localTabControl.TabPages.Add(tabPageGenerationOutput);
            tabPageGenerationOutput.BackColor = Color.Transparent;
            tabPageGenerationOutput.Name = $"{_inputNiceName} Generation Output";
            tabPageGenerationOutput.BorderStyle = BorderStyle.None;
            tabPageGenerationOutput.UseVisualStyleBackColor = true;

            // Add 'Generation Output' RichTextBox to Generation Output tab
            localRichTextBoxGenerationOutput = new RichTextBox();
            tabPageGenerationOutput.Controls.Add(localRichTextBoxGenerationOutput);
            localRichTextBoxGenerationOutput.Dock = DockStyle.Fill;
            localRichTextBoxGenerationOutput.Text = $"No {_inputNiceName} logic has been generated at the moment.";
            localRichTextBoxGenerationOutput.Location = new Point(3, 6);
            localRichTextBoxGenerationOutput.Size = new Size(882, 535);
            localRichTextBoxGenerationOutput.BorderStyle = BorderStyle.None;
            
            // Add 'Pattern' Tab Page to on Sub Tab
            tabPageGenerationPattern = new TabPage($"{_inputNiceName} Pattern");
            localTabControl.TabPages.Add(tabPageGenerationPattern);
            tabPageGenerationPattern.BackColor = Color.Transparent;
            tabPageGenerationPattern.Name = $"{_inputNiceName} Pattern";
            tabPageGenerationPattern.BorderStyle = BorderStyle.None;
            tabPageGenerationPattern.UseVisualStyleBackColor = true;

            // Add 'Pattern ComboBox' to Pattern tab
            localComboBoxGenerationPattern = new ComboBox();
            tabPageGenerationPattern.Controls.Add(localComboBoxGenerationPattern);
            localComboBoxGenerationPattern.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localComboBoxGenerationPattern.Location = new Point(108, 8);
            localComboBoxGenerationPattern.Size = new Size(496, 21);
            localComboBoxGenerationPattern.Name = $"comboBox{classification}Pattern";
            localComboBoxGenerationPattern.SelectedIndexChanged += DisplayPattern;

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
            localLabelFullFilePath.Size = new Size(750, 13);
            localLabelFullFilePath.Name = $"label{classification}FullFilePath";
            localLabelFullFilePath.Text = @"<path>";

            // Add 'Active Connection Key' Label
            localLabelActiveConnectionKey = new Label();
            tabPageGenerationPattern.Controls.Add(localLabelActiveConnectionKey);
            localLabelActiveConnectionKey.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localLabelActiveConnectionKey.Location = new Point(2, 57);
            localLabelActiveConnectionKey.Size = new Size(100, 13);
            localLabelActiveConnectionKey.Name = $"label{classification}ConnectionKey";
            localLabelActiveConnectionKey.Text = @"Connection Key:";

            // Add 'Active Connection Key Value' Label
            localLabelActiveConnectionKeyValue = new Label();
            tabPageGenerationPattern.Controls.Add(localLabelActiveConnectionKeyValue);
            localLabelActiveConnectionKeyValue.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localLabelActiveConnectionKeyValue.Location = new Point(105, 57);
            localLabelActiveConnectionKeyValue.Size = new Size(240, 13);
            localLabelActiveConnectionKeyValue.Name = $"label{classification}ConnectionKeyValue";
            localLabelActiveConnectionKeyValue.Text = @"<connection key>";

            // Add 'Output File Pattern' Label
            localLabelOutputFilePattern = new Label();
            tabPageGenerationPattern.Controls.Add(localLabelOutputFilePattern);
            localLabelOutputFilePattern.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localLabelOutputFilePattern.Location = new Point(350, 57);
            localLabelOutputFilePattern.Size = new Size(100, 13);
            localLabelOutputFilePattern.Name = $"label{classification}OutputFilePattern";
            localLabelOutputFilePattern.Text = @"Output file pattern:";

            // Add 'Output File Pattern Value' Label
            localLabelOutputFilePatternValue = new Label();
            tabPageGenerationPattern.Controls.Add(localLabelOutputFilePatternValue);
            localLabelOutputFilePatternValue.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localLabelOutputFilePatternValue.Location = new Point(460, 57);
            localLabelOutputFilePatternValue.Size = new Size(250, 13);
            localLabelOutputFilePatternValue.Name = $"label{classification}OutputFilePatternValue";
            localLabelOutputFilePatternValue.Text = @"<output file pattern>";

            // Add 'Save Pattern' Button
            localSavePattern = new Button();
            tabPageGenerationPattern.Controls.Add(localSavePattern);
            localSavePattern.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localSavePattern.Location = new Point(610, 7);
            localSavePattern.Size = new Size(101, 23);
            localSavePattern.Text = $"Save updates";
            localSavePattern.Name = $"Save{classification}";
            localSavePattern.Click += SavePattern;

            // Add 'Generation Pattern' RichTextBox to Pattern tab
            localRichTextBoxGenerationPattern = new RichTextBox();
            tabPageGenerationPattern.Controls.Add(localRichTextBoxGenerationPattern);
            localRichTextBoxGenerationPattern.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            localRichTextBoxGenerationPattern.Location = new Point(3, 82);
            localRichTextBoxGenerationPattern.Size = new Size(195, 30);
            localRichTextBoxGenerationPattern.BorderStyle = BorderStyle.None;
            localRichTextBoxGenerationPattern.TextChanged += CommitPatternToMemory;
            #endregion

            #region Constructor Methods

            // Populate the Combo Box
            LoadTabPageComboBox(classification);

            // Populate the Checked List Box
            SetItemList(itemList);

            // Report back to the user if there is not metadata available
            if (itemList == null || itemList.Count == 0)
            {
                RaiseOnChangeMainText($"There was no metadata available to display {_inputNiceName} content. Please check the associated metadata schema (are there any {_inputNiceName} records available?) or the database connection.\r\n\r\n");
            }

            // Initial documentation as per the definition notes
            if (notes != null)
            {
                RaiseOnChangeMainText(notes);
            }

            // Set the tab pages back to what they were before reload
            var currentSubTab = FormBase.VdwConfigurationSettings.SelectedSubTab;
            if (currentSubTab != null)
            {
                var localTabPage = localTabControl.TabPages[currentSubTab];

                if (localTabPage != null) // The control we're looking for has to exist, otherwise no need to continue
                {
                    if (localTabControl.TabPages.Contains(localTabControl.TabPages[currentSubTab]))
                    {
                        localTabControl.SelectTab(localTabControl.TabPages[currentSubTab]);
                        try
                        {
                            localTabControl.SelectedTab.Controls[7].Text =
                                FormBase.VdwConfigurationSettings.SelectedPatternText;
                        }
                        catch
                        {
                            // Do nothing
                        }

                        ApplySyntaxHighlightingForHandlebars();
                    }
                }
            }

            // Prevention of double hitting of some event handlers
            StartUpIndicator = false;

            #endregion
        }

        private void CommitPatternToMemory(object sender, EventArgs args)
        {
            if (StartUpIndicator == false)
            {
                if (localTabControl.SelectedTab != null)
                {
                    try
                    {
                        FormBase.VdwConfigurationSettings.SelectedPatternText =
                            localTabControl.SelectedTab.Controls[7].Text;
                    }
                    catch
                    {
                        // Do nothing
                    }
                }
            }
        }

        /// <summary>
        /// Called by the OnSelectedIndexChanged event on the local tab control, this method saves on-screen information to memory in case the UI is rebuilt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void SubTabClick(object sender, EventArgs args)
        {
            try
            {
                if (!localTabControl.SelectedTab.Name.EndsWith("Output"))
                {
                    //SyntaxHighlightsHandlebars(sender, args); // Needed as part of index change event
                    //TextHandling.SyntaxHighlightHandlebars(localRichTextBoxGenerationPattern, localRichTextBoxGenerationPattern.Text.TrimEnd());
                    FormBase.VdwConfigurationSettings.SelectedSubTab = localTabControl.SelectedTab.Name;
                    FormBase.VdwConfigurationSettings.SelectedPatternText = localTabControl.SelectedTab.Controls[7].Text;
                    ApplySyntaxHighlightingForHandlebars();
                }
            }
            catch
            {
                // Do nothing
            }
        }

        public void FilterItemList(object o, EventArgs e)
        {
            SetItemList(ItemList);
        }

        public void SetItemList(Dictionary<string, VDW_DataObjectMappingList> itemList)
        {
            // Copy the input variable to the local item list
            this.ItemList = itemList;

            // Clear the existing checkboxes
            _localCheckedListBox.Items.Clear();

            // Add items to the Checked List Box, if it satisfies the filter criterion
            if (itemList != null && itemList.Count > 0)
            {
                foreach (string item in itemList.Keys)
                {
                    if (item.Contains(_localTextBoxFilter.Text))
                    {
                        _localCheckedListBox.Items.Add(item);
                    }
                }
            }

            // Report back to the user if there is not metadata available
            if (_localCheckedListBox.Items.Count == 0)
            {
                RaiseOnChangeMainText($"There was no metadata available to display {_inputNiceName} content. Please check the associated metadata schema (are there any {_inputNiceName} records available?) or the database connection.");
            }

            // Set all the Check Boxes to 'checked'
            for (int x = 0; x <= _localCheckedListBox.Items.Count - 1; x++)
            {
                CheckAllCheckBoxes();
            }
        }
        
        private void CheckAllCheckBoxes()
        {
            for (int x = 0; x <= _localCheckedListBox.Items.Count - 1; x++)
            {
                _localCheckedListBox.SetItemChecked(x, _localCheckBoxSelectAll.Checked);
            }
        }

        private void DisplayPattern(object o, EventArgs e)
        {
            // Retrieve all the info for the pattern name from memory based on the combobox value (from the list of patterns).
            var loadPattern = FormBase.VdwConfigurationSettings.patternList.FirstOrDefault(x => x.LoadPatternName == localComboBoxGenerationPattern.Text);
            
            // Set the label with the path so it's visible to the user where the file is located
            if (loadPattern != null)
            {
                string localFullPath = Path.Combine(FormBase.VdwConfigurationSettings.LoadPatternPath, loadPattern.LoadPatternFilePath);

                localLabelFullFilePath.Text = localFullPath;
                localLabelActiveConnectionKeyValue.Text = loadPattern.LoadPatternConnectionKey;

                if (!string.IsNullOrEmpty(loadPattern.LoadPatternOutputFilePattern))
                {
                    localLabelOutputFilePatternValue.Text = loadPattern.LoadPatternOutputFilePattern;
                }
                else
                {
                    localLabelOutputFilePatternValue.Text = "Undefined";
                }


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
            }
            else
            {
                FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Warning, $"There was no pattern available."));
            }

            // Syntax highlight for Handlebars
            if (StartUpIndicator == false)
            {
                ApplySyntaxHighlightingForHandlebars();
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
            string backupResponse = LoadPatternFileHandling.BackupPatternCollection(localLabelFullFilePath.Text);
            string saveResponse = "";

            if (backupResponse.StartsWith("A backup was created"))
            {
                RaiseOnChangeMainText(backupResponse);
                saveResponse = LoadPatternFileHandling.SavePatternCollection(localLabelFullFilePath.Text, localRichTextBoxGenerationPattern.Text);
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
            GenerateFromPattern();
        }

        /// <summary>
        ///   Create output using Handlebars as templating engine
        /// </summary>
        private void GenerateFromPattern()
        {
            // Establish the current time at the start of generation, to display only messages related to the current generation run.
            var currentTime = DateTime.Now;

            localRichTextBoxGenerationOutput.Clear();
            RaiseOnClearMainText();
            localTabControl.SelectedIndex = 0;

            // Loop through the checked items, select the right mapping and generate the pattern.
            if (_localCheckedListBox.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= _localCheckedListBox.CheckedItems.Count - 1; x++)
                {
                    var targetTableName = _localCheckedListBox.CheckedItems[x].ToString();
                    RaiseOnChangeMainText(@"Generating code for " + targetTableName + ".\r\n");

                    // Only process the selected items in the total of available source-to-target mappings.
                    ItemList.TryGetValue(targetTableName, out var dataObjectMappingList);

                    // Return the result to the user.
                    try
                    {
                        // Compile the template, and merge it with the metadata.
                        var template = Handlebars.Compile(localRichTextBoxGenerationPattern.Text);
                        var result = template(dataObjectMappingList);

                        // Check if the metadata needs to be displayed.
                        if (DisplayJsonFlag)
                        {
                            try
                            {
                                var json = JsonConvert.SerializeObject(dataObjectMappingList, Formatting.Indented);
                                localRichTextBoxGenerationOutput.AppendText(json + "\r\n\r\n");
                            }
                            catch (Exception exception)
                            {
                                RaiseOnChangeMainText("An error was encountered while parsing the Json metadata. The error message is: " + exception.Message);
                            }
                        }

                        // Display the output of the template to the user.
                        localRichTextBoxGenerationOutput.AppendText(result);

                        // Spool the output to disk.
                        if (SaveOutputFileFlag)
                        {
                            // Assert file output pattern.
                            var outputFile = localLabelOutputFilePatternValue.Text;
                            outputFile = outputFile.Replace("{targetDataObject.name}", targetTableName);
                            VdwUtility.SaveOutputToDisk(FormBase.VdwConfigurationSettings.VdwOutputPath + outputFile, result);
                        }

                        //Generate in database.
                        if (GenerateInDatabaseFlag)
                        {
                            // Find the right connection for the pattern connection key.
                            var localConnection = TeamConfiguration.GetTeamConnectionByInternalId(localLabelActiveConnectionKeyValue.Text, FormBase.TeamConfigurationSettings.ConnectionDictionary);

                            if (localConnection != null)
                            {
                                var conn = new SqlConnection {ConnectionString = localConnection.CreateSqlServerConnectionString(false)};

                                try
                                {
                                    VdwUtility.CreateVdwSchema(conn);
                                }
                                catch
                                {
                                    var errorMessage = $"There was an issue creating the schema '{FormBase.VdwConfigurationSettings.VdwSchema}' in database '{conn.Database}'.";
                                    RaiseOnChangeMainText(errorMessage);
                                    FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Error, errorMessage));
                                }

                                try
                                {
                                    VdwUtility.ExecuteOutputInDatabase(conn, result);
                                }
                                catch
                                {
                                    var errorMessage = $"There was an issue executing the query '{result}' in database '{conn.Database}'.";
                                    RaiseOnChangeMainText(errorMessage);
                                    FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Error, errorMessage));
                                }
                            }
                            else
                            {
                                var errorMessage = $"There was an issue establishing a connection to generate the output for '{targetTableName}'. Is there a TEAM connections file in the configuration directory?";
                                RaiseOnChangeMainText(errorMessage);
                                FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Error, errorMessage));
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        var errorMessage = $"The template could not be compiled. The error message is: {exception.Message}";
                        RaiseOnChangeMainText(errorMessage);
                        FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Error, errorMessage));
                    }
                }
            }
            else
            {
                RaiseOnChangeMainText($"There was no metadata selected to generate {_inputNiceName} code. Please check the metadata schema - are there any {_inputNiceName} objects selected?");
            }

            // Report back to the user
            int errorCounter = 0;

            foreach (Event individualEvent in FormBase.VdwConfigurationSettings.VdwEventLog)
            {
                if (individualEvent.eventCode == 1 && individualEvent.eventTime>currentTime)
                {
                    errorCounter++;
                    RaiseOnChangeMainText(individualEvent.eventDescription+"\r\n");
                }
            }

            RaiseOnChangeMainText($"\r\n{errorCounter} error(s) have been found.\r\n");
            RaiseOnChangeMainText($"\r\nAssociated scripts have been saved in {FormBase.VdwConfigurationSettings.VdwOutputPath}.\r\n");

            // Apply syntax highlighting.
            localRichTextBoxGenerationOutput.Rtf = TextHandling.SyntaxHighlightSql(localRichTextBoxGenerationOutput.Text).Rtf;
        }

        /// <summary>
        /// Populate the combobox containing the pattern names associated with the classification.
        /// </summary>
        /// <param name="patternClassification"></param>
        private void LoadTabPageComboBox(string patternClassification)
        {
            bool available = false;
            try
            {
                var patternList = FormBase.VdwConfigurationSettings.patternList;

                localComboBoxGenerationPattern.Items.Clear();
                localRichTextBoxGenerationPattern.Clear();
                localLabelFullFilePath.Text = "";
                localLabelActiveConnectionKeyValue.Text = "";

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
    }
}
