using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using HandlebarsDotNet;
using Newtonsoft.Json;
using DataWarehouseAutomation;
using TEAM_Library;
using System.Text.Json;
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
    {
        public MyClearArgs()
        {
        }
    }

    /// <summary>
    /// Custom Tab Page to generate a tab connected to a specific template classification.
    /// </summary>
    class CustomTabPage : TabPage
    {
        readonly string _inputNiceName;
        internal bool _fileSplit;
        internal Dictionary<string, VdwDataObjectMappingList> VdwDataObjectMappingList;

        // Objects on main Tab Page
        readonly CheckBox _localCheckBoxSelectAll;
        readonly CheckedListBox _localCheckedListBox;
        readonly TextBox _localTextBoxFilter;

        // Objects on the sub Tab Control (on the main Tab Page)
        readonly TabControl localTabControl;

        readonly TabPage tabPageGenerationOutput;
        readonly RichTextBox localRichTextBoxGenerationOutput;

        readonly TabPage tabPageGenerationTemplate;
        // Active template
        readonly Label localLabelActiveTemplate;
        readonly Label localLabelFilePath;
        readonly Label localLabelFullFilePath;
        // Assigned connection
        readonly Label localLabelActiveConnectionKey;
        readonly Label localLabelActiveConnectionKeyValue;
        // Output file
        readonly Label localLabelOutputFileTemplate;
        readonly Label localLabelOutputFileTemplateValue;

        readonly ComboBox localComboBoxGenerationTemplate;
        readonly RichTextBox localRichTextBoxGenerationTemplate;
        readonly Button localSaveTemplate;

        #region Main Form CheckBox value handling
        // Values for the checkboxes from main form
        public bool DisplayJsonFlag { get; set; }
        public bool GenerateInDatabaseFlag { get; set; }
        public bool SaveOutputFileFlag { get; set; } = false;

        internal bool StartUpIndicator = true;
        
        /// <summary>
        /// Function can be called from the main form as well this wya.
        /// </summary>
        public void ApplySyntaxHighlightingForHandlebars()
        {
            localRichTextBoxGenerationTemplate.Rtf = TextHandling.SyntaxHighlightHandlebars(localRichTextBoxGenerationTemplate.Text.TrimEnd()).Rtf;
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
        public CustomTabPage(string classification, string notes, Dictionary<string, VdwDataObjectMappingList> vdwDataObjectMappingList)
        {
            // Register the Handlebars helpers (extensions), these are maintained in the DataWarehouseAutomation class library.
            HandleBarsHelpers.RegisterHandleBarsHelpers();

            this.VdwDataObjectMappingList = vdwDataObjectMappingList;

            _inputNiceName = Regex.Replace(classification, "(\\B[A-Z])", " $1");

            DisplayJsonFlag = true;
            GenerateInDatabaseFlag = true;
            SaveOutputFileFlag = false;

            #region Main Tab Page

            // Base properties of the custom tab page
            Name = $"{classification}";
            Text = _inputNiceName;
            Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
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

            // Add 'Select All' CheckBox
            _localCheckBoxSelectAll = new CheckBox();
            localPanel.Controls.Add(_localCheckBoxSelectAll);
            _localCheckBoxSelectAll.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            _localCheckBoxSelectAll.Location = new Point(19, 12);
            _localCheckBoxSelectAll.Size = new Size(90, 17);
            _localCheckBoxSelectAll.Name = "checkBoxSelectAll";
            _localCheckBoxSelectAll.Checked = true;
            _localCheckBoxSelectAll.Text = "Select all";
            _localCheckBoxSelectAll.CheckedChanged += SelectAllCheckBoxItems;

            // Add 'Generate' Button 
            var localButtonGenerate = new Button();
            localPanel.Controls.Add(localButtonGenerate);
            localButtonGenerate.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localButtonGenerate.Location = new Point(116, 9);
            localButtonGenerate.Name = $"Generate{classification}";
            localButtonGenerate.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            localButtonGenerate.Size = new Size(120, 22);
            localButtonGenerate.Text = $"Generate";
            localButtonGenerate.Click += Generate;

            // Add 'Filter' Text Box
            _localTextBoxFilter = new TextBox();
            localPanel.Controls.Add(_localTextBoxFilter);
            _localTextBoxFilter.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            _localTextBoxFilter.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            _localTextBoxFilter.Location = new Point(266, 9);
            _localTextBoxFilter.Size = new Size(144, 9);
            _localTextBoxFilter.Name = $"textBoxFilterCriterion{_inputNiceName}";
            _localTextBoxFilter.TextChanged += FilterItemList;
            _localTextBoxFilter.PlaceholderText = "Add a filter";

            // Add Checked List Box
            _localCheckedListBox = new CheckedListBox();
            localPanel.Controls.Add(_localCheckedListBox);
            _localCheckedListBox.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left);
            _localCheckedListBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            _localCheckedListBox.Location = new Point(17, 32);
            _localCheckedListBox.Size = new Size(393, 562);
            _localCheckedListBox.BorderStyle = BorderStyle.FixedSingle;
            _localCheckedListBox.CheckOnClick = true;
            _localCheckedListBox.Name = $"checkedListBox{classification}";

            #endregion

            #region Sub Tab Pages

            // Add Sub Tab Control
            localTabControl = new TabControl();
            localPanel.Controls.Add(localTabControl);
            localTabControl.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            localTabControl.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            localTabControl.Location = new Point(416, 9);
            localTabControl.Size = new Size(896, 570);
            localTabControl.Name = $"tabControl{classification}";
            localTabControl.BackColor = Color.White;
            localTabControl.SelectedIndexChanged += SubTabClick;

            // Add 'Generation Output' Tab Page on Sub Tab
            tabPageGenerationOutput = new TabPage($"{_inputNiceName} Generation Output");
            localTabControl.TabPages.Add(tabPageGenerationOutput);
            tabPageGenerationOutput.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            tabPageGenerationOutput.BackColor = Color.Transparent;
            tabPageGenerationOutput.Name = $"{_inputNiceName} Generation Output";
            tabPageGenerationOutput.BorderStyle = BorderStyle.None;
            tabPageGenerationOutput.UseVisualStyleBackColor = true;

            // Add 'Generation Output' RichTextBox to Generation Output tab
            localRichTextBoxGenerationOutput = new RichTextBox();
            tabPageGenerationOutput.Controls.Add(localRichTextBoxGenerationOutput);
            localRichTextBoxGenerationOutput.Dock = DockStyle.Fill;
            localRichTextBoxGenerationOutput.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            localRichTextBoxGenerationOutput.Text = $"No {_inputNiceName} logic has been generated at the moment.";
            localRichTextBoxGenerationOutput.Location = new Point(3, 6);
            localRichTextBoxGenerationOutput.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            localRichTextBoxGenerationOutput.Size = new Size(882, 535);
            localRichTextBoxGenerationOutput.BorderStyle = BorderStyle.None;
            
            // Add 'Template' Tab Page to on Sub Tab
            tabPageGenerationTemplate = new TabPage($"{_inputNiceName} Template");
            localTabControl.TabPages.Add(tabPageGenerationTemplate);
            tabPageGenerationTemplate.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            tabPageGenerationTemplate.BackColor = Color.Transparent;
            tabPageGenerationTemplate.Name = $"{_inputNiceName} Template";
            tabPageGenerationTemplate.BorderStyle = BorderStyle.None;
            tabPageGenerationTemplate.UseVisualStyleBackColor = true;

            // Add 'Template ComboBox' to Template tab
            localComboBoxGenerationTemplate = new ComboBox();
            tabPageGenerationTemplate.Controls.Add(localComboBoxGenerationTemplate);
            localComboBoxGenerationTemplate.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            localComboBoxGenerationTemplate.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localComboBoxGenerationTemplate.Location = new Point(108, 8);
            localComboBoxGenerationTemplate.Size = new Size(496, 21);
            localComboBoxGenerationTemplate.Name = $"comboBox{classification}Template";
            localComboBoxGenerationTemplate.SelectedIndexChanged += DisplayTemplateOutput;

            // Add 'Active Template' Label
            localLabelActiveTemplate = new Label();
            tabPageGenerationTemplate.Controls.Add(localLabelActiveTemplate);
            localLabelActiveTemplate.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            localLabelActiveTemplate.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localLabelActiveTemplate.Location = new Point(2, 11);
            localLabelActiveTemplate.Size = new Size(77, 13);
            localLabelActiveTemplate.Name = $"label{classification}ActiveTemplate";
            localLabelActiveTemplate.Text = "Active Template:";

            // Add 'File Path' Label
            localLabelFilePath = new Label();
            tabPageGenerationTemplate.Controls.Add(localLabelFilePath);
            localLabelFilePath.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            localLabelFilePath.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localLabelFilePath.Location = new Point(2, 34);
            localLabelFilePath.Size = new Size(60, 13);
            localLabelFilePath.Name = $"label{classification}FilePath";
            localLabelFilePath.Text = @"File Path:";

            // Add 'Full Path' Label
            localLabelFullFilePath = new Label();
            tabPageGenerationTemplate.Controls.Add(localLabelFullFilePath);
            localLabelFullFilePath.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            localLabelFullFilePath.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localLabelFullFilePath.Location = new Point(105, 34);
            localLabelFullFilePath.Size = new Size(750, 13);
            localLabelFullFilePath.Name = $"label{classification}FullFilePath";
            localLabelFullFilePath.Text = @"<path>";

            // Add 'Active Connection Key' Label
            localLabelActiveConnectionKey = new Label();
            tabPageGenerationTemplate.Controls.Add(localLabelActiveConnectionKey);
            localLabelActiveConnectionKey.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localLabelActiveConnectionKey.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            localLabelActiveConnectionKey.Location = new Point(2, 57);
            localLabelActiveConnectionKey.Size = new Size(100, 13);
            localLabelActiveConnectionKey.Name = $"label{classification}ConnectionKey";
            localLabelActiveConnectionKey.Text = @"Connection Key:";

            // Add 'Active Connection Key Value' Label
            localLabelActiveConnectionKeyValue = new Label();
            tabPageGenerationTemplate.Controls.Add(localLabelActiveConnectionKeyValue);
            localLabelActiveConnectionKeyValue.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            localLabelActiveConnectionKeyValue.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localLabelActiveConnectionKeyValue.Location = new Point(105, 57);
            localLabelActiveConnectionKeyValue.Size = new Size(240, 13);
            localLabelActiveConnectionKeyValue.Name = $"label{classification}ConnectionKeyValue";
            localLabelActiveConnectionKeyValue.Text = @"<connection key>";

            // Add 'Output File Template' Label
            localLabelOutputFileTemplate = new Label();
            tabPageGenerationTemplate.Controls.Add(localLabelOutputFileTemplate);
            localLabelOutputFileTemplate.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localLabelOutputFileTemplate.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            localLabelOutputFileTemplate.Location = new Point(350, 57);
            localLabelOutputFileTemplate.Size = new Size(120, 13);
            localLabelOutputFileTemplate.Name = $"label{classification}OutputFileTemplate";
            localLabelOutputFileTemplate.Text = @"Output file template:";

            // Add 'Output File Template Value' Label
            localLabelOutputFileTemplateValue = new Label();
            tabPageGenerationTemplate.Controls.Add(localLabelOutputFileTemplateValue);
            localLabelOutputFileTemplateValue.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localLabelOutputFileTemplateValue.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            localLabelOutputFileTemplateValue.Location = new Point(480, 57);
            localLabelOutputFileTemplateValue.Size = new Size(400, 13);
            localLabelOutputFileTemplateValue.Name = $"label{classification}OutputFileTemplateValue";
            localLabelOutputFileTemplateValue.Text = @"<output file template>";

            // Add 'Save Template' Button
            localSaveTemplate = new Button();
            tabPageGenerationTemplate.Controls.Add(localSaveTemplate);
            localSaveTemplate.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            localSaveTemplate.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            localSaveTemplate.Location = new Point(610, 7);
            localSaveTemplate.Size = new Size(101, 23);
            localSaveTemplate.Text = $"Save";
            localSaveTemplate.Name = $"Save{classification}";
            localSaveTemplate.Click += SaveTemplate;

            // Add 'Generation Template' RichTextBox to Templates tab
            localRichTextBoxGenerationTemplate = new RichTextBox();
            tabPageGenerationTemplate.Controls.Add(localRichTextBoxGenerationTemplate);
            localRichTextBoxGenerationTemplate.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            localRichTextBoxGenerationTemplate.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            localRichTextBoxGenerationTemplate.Location = new Point(3, 82);
            localRichTextBoxGenerationTemplate.Size = new Size(195, 30);
            localRichTextBoxGenerationTemplate.BorderStyle = BorderStyle.None;
            localRichTextBoxGenerationTemplate.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            localRichTextBoxGenerationTemplate.AcceptsTab = true;
            localRichTextBoxGenerationTemplate.TextChanged += CommitTemplateToMemory;
            #endregion

            #region Constructor Methods

            // Populate the Combo Box
            LoadTabPageComboBox(classification);

            // Populate the Checked List Box
            SetItemList(vdwDataObjectMappingList);

            // Report back to the user if there is not metadata available
            if (vdwDataObjectMappingList == null || vdwDataObjectMappingList.Count == 0)
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
                            localTabControl.SelectedTab.Controls[7].Text = FormBase.VdwConfigurationSettings.SelectedTemplateText;
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

        private void CommitTemplateToMemory(object sender, EventArgs args)
        {
            if (StartUpIndicator == false)
            {
                if (localTabControl.SelectedTab != null)
                {
                    try
                    {
                        FormBase.VdwConfigurationSettings.SelectedTemplateText = localTabControl.SelectedTab.Controls[7].Text;
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
                    FormBase.VdwConfigurationSettings.SelectedSubTab = localTabControl.SelectedTab.Name;
                    FormBase.VdwConfigurationSettings.SelectedTemplateText = localTabControl.SelectedTab.Controls[7].Text;
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
            SetItemList(VdwDataObjectMappingList);
        }

        public void SetItemList(Dictionary<string, VdwDataObjectMappingList> vdwDataObjectMappingList)
        {
            // Copy the input variable to the local item list.
            this.VdwDataObjectMappingList = vdwDataObjectMappingList;

            // Clear the existing checkboxes.
            _localCheckedListBox.Items.Clear();

            // Add items to the Checked List Box, if it satisfies the filter criterion.
            if (vdwDataObjectMappingList != null && vdwDataObjectMappingList.Count > 0)
            {
                foreach (string item in vdwDataObjectMappingList.Keys)
                {
                    if (item.Contains(_localTextBoxFilter.Text))
                    {
                        _localCheckedListBox.Items.Add(item);
                    }
                }
            }

            // Report back to the user if there is not metadata available.
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

        private void DisplayTemplateOutput(object o, EventArgs e)
        {
            // Retrieve all the info for the template name from memory based on the combobox value (from the list of templates).
            var template = FormBase.VdwConfigurationSettings.templateList.FirstOrDefault(x => x.TemplateName == localComboBoxGenerationTemplate.Text);
            
            // Set the label with the path so it's visible to the user where the file is located
            if (template != null)
            {
                string localFullPath = Path.Combine(FormBase.VdwConfigurationSettings.TemplatePath, template.TemplateFilePath);

                localLabelFullFilePath.Text = localFullPath;
                localLabelActiveConnectionKeyValue.Text = template.TemplateConnectionKey;

                // Update the internal flag to split files, or not.
                _fileSplit = template.TemplateOutputFileSplit;

                if (!string.IsNullOrEmpty(template.TemplateOutputFileConvention))
                {
                    localLabelOutputFileTemplateValue.Text = template.TemplateOutputFileConvention;
                }
                else
                {
                    localLabelOutputFileTemplateValue.Text = "Undefined";
                }


                // Read the file from the path
                string templateFile ="";
                try
                {
                    templateFile = File.ReadAllText(localFullPath);
                }
                catch
                {
                    templateFile = $"There was an error loading the template specified in the template collection file.\r\n\r\nDoes '{template.TemplateFilePath}' exist and is the path correct?\r\n\r\nIf this is not the case please update the template collection information in the 'settings' tab.";
                }

                // Display the template in the text box on the screen
                localRichTextBoxGenerationTemplate.Text = templateFile;
            }
            else
            {
                FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Warning, $"There was no template available."));
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

        void SaveTemplate(object o, EventArgs e)
        {
            var saveResponse = TemplateHandling.SaveTemplateCollection(localLabelFullFilePath.Text, localRichTextBoxGenerationTemplate.Text);
            RaiseOnChangeMainText("\r\n" + saveResponse);

            //RaiseOnClearMainText();
            //string backupResponse = TemplateHandling.BackupTemplateCollection(localLabelFullFilePath.Text);
            //string saveResponse = "";

            //if (backupResponse.StartsWith("A backup was created"))
            //{
            //    RaiseOnChangeMainText(backupResponse);

            //}
            //else
            //{
            //    RaiseOnChangeMainText(backupResponse);
            //    RaiseOnChangeMainText(saveResponse);
            //}
        }

        void Generate(object o, EventArgs e)
        {
            localTabControl.SelectedIndex = 0;
            DoWork();
        }

        void DoWork()
        {
            GenerateFromTemplate();
        }

        /// <summary>
        /// Create output using Handlebars as templating engine
        /// </summary>
        private void GenerateFromTemplate()
        {
            var currentTime = DateTime.Now;

            localRichTextBoxGenerationOutput.Clear();
            RaiseOnClearMainText();
            localTabControl.SelectedIndex = 0;

            // Loop through the checked items, select the right mapping and generate the template output.
            if (_localCheckedListBox.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= _localCheckedListBox.CheckedItems.Count - 1; x++)
                {
                    var targetDataObjectName = _localCheckedListBox.CheckedItems[x].ToString();
                    RaiseOnChangeMainText(@"Generating code for " + targetDataObjectName + ".\r\n");

                    // Only process the selected items in the total of available source-to-target mappings.
                    VdwDataObjectMappingList.TryGetValue(targetDataObjectName, out var VDW_dataObjectMappingList);

                    // Return the result to the user.
                    try
                    {
                        // Compile the template, and merge it with the metadata.
                        var template = Handlebars.Compile(localRichTextBoxGenerationTemplate.Text);                   

                        if (!_fileSplit)
                        {
                            var outputFileName = localLabelOutputFileTemplateValue.Text;
                            outputFileName = outputFileName.Replace("{targetDataObject.name}", targetDataObjectName);
                            Generate(targetDataObjectName, VDW_dataObjectMappingList, template, outputFileName);
                        }
                        else
                        {
                            // Split out the original mapping list into separate mapping lists.
                            var separatedMappingLists = new List<DataObjectMapping>();

                            var tempMetadataConfiguration = VDW_dataObjectMappingList.metadataConfiguration;
                            var tempGenerationSpecificMetadata = VDW_dataObjectMappingList.generationSpecificMetadata;
                            var tempMetadataFileName = VDW_dataObjectMappingList.metadataFileName;

                            foreach (var vdwMapping in VDW_dataObjectMappingList.DataObjectMappings)
                            {
                                var outputFileName = localLabelOutputFileTemplateValue.Text;
                                var localVdwMappingList = new VdwDataObjectMappingList();
                                localVdwMappingList.generationSpecificMetadata = tempGenerationSpecificMetadata;
                                localVdwMappingList.metadataFileName = tempMetadataFileName;
                                localVdwMappingList.metadataConfiguration = tempMetadataConfiguration;

                                localVdwMappingList.DataObjectMappings.Add(vdwMapping);

                                outputFileName = outputFileName.Replace("{targetDataObject.name}", targetDataObjectName);

                                try
                                {
                                    var sourceDataObject = vdwMapping.SourceDataObjects.FirstOrDefault();
                                    string sourceDataObjectName = "";

                                    // Parse the JSON string to a dynamic object
                                    //JsonDocument jsonDoc = JsonDocument.Parse(sourceDataObject);
                                    //JsonElement root = jsonDoc.RootElement;

                                    // Safely extract the "name" property
                                    if (sourceDataObject.TryGetProperty("name", out JsonElement nameElement))
                                    {
                                        sourceDataObjectName = nameElement.GetString();

                                        outputFileName = outputFileName.Replace("{sourceDataObject.name}", sourceDataObjectName);
                                    }

                                    //sourceDataObjectName = sourceDataObject.Name;                         

                                    //outputFileName = outputFileName.Replace("{sourceDataObject.name}", vdwMapping.SourceDataObjects.FirstOrDefault().MappingName);
                                }
                                catch (Exception ex)
                                {
                                    //
                                }
                                
                                Generate(targetDataObjectName, localVdwMappingList, template, outputFileName);
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
            //localRichTextBoxGenerationOutput.Rtf = TextHandling.SyntaxHighlightSql(localRichTextBoxGenerationOutput.Text).Rtf;
        }

        private void Generate(string targetDataObjectName, VdwDataObjectMappingList vdwDataObjectMappingList, HandlebarsTemplate<object, object> template, string outputFileName)
        {
            var jsonInput = System.Text.Json.JsonSerializer.Serialize(vdwDataObjectMappingList);
            JsonNode deserializedMapping = System.Text.Json.JsonSerializer.Deserialize<JsonNode>(jsonInput);

            var result = template(deserializedMapping);

            // Check if the metadata needs to be displayed.
            if (DisplayJsonFlag)
            {
                try
                {
                    var json = JsonConvert.SerializeObject(vdwDataObjectMappingList, Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                            NullValueHandling = NullValueHandling.Ignore
                        });

                    localRichTextBoxGenerationOutput.AppendText(json + "\r\n\r\n");
                }
                catch (Exception exception)
                {
                    RaiseOnChangeMainText($"An error was encountered while parsing the Json metadata. The error message is: {exception.Message}.");
                }
            }

            // Display the output of the template to the user.
            localRichTextBoxGenerationOutput.AppendText(result);

            // Spool the output to disk.
            if (SaveOutputFileFlag)
            {
                VdwUtility.SaveOutputToDisk(FormBase.VdwConfigurationSettings.VdwOutputPath + outputFileName, result);
            }

            //Generate in database.
            if (GenerateInDatabaseFlag)
            {
                // Find the right connection for the template connection key.
                var localConnection = TeamConfiguration.GetTeamConnectionByInternalId(localLabelActiveConnectionKeyValue.Text, FormBase.TeamConfigurationSettings.ConnectionDictionary);

                if (localConnection != null)
                {
                    try
                    {
                        if (localConnection.TechnologyConnectionType == TechnologyConnectionType.SqlServer)
                        {
                            VdwUtility.CreateVdwSchema(new SqlConnection { ConnectionString = localConnection.CreateSqlServerConnectionString(false) });
                        }
                    }
                    catch (Exception exception)
                    {
                        var errorMessage = $"There was an issue creating the schema '{FormBase.VdwConfigurationSettings.VdwSchema}' against connection '{localConnection.ConnectionKey}'. The error message is {exception.Message}.";
                        RaiseOnChangeMainText(errorMessage);
                        FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Error, errorMessage));
                    }

                    try
                    {
                        VdwUtility.ExecuteInDatabase(localConnection, result);
                    }
                    catch (Exception exception)
                    {
                        var errorMessage = $"There was an issue executing the query '{result}' against connection '{localConnection.ConnectionKey}'. The reported error is {exception.Message}.";
                        RaiseOnChangeMainText(errorMessage);
                        FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Error, errorMessage));
                    }
                }
                else
                {
                    var errorMessage = $"There was an issue establishing a connection to generate the output for '{targetDataObjectName}'. Is there a TEAM connections file in the configuration directory?";
                    RaiseOnChangeMainText(errorMessage);
                    FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Error, errorMessage));
                }
            }
        }

        /// <summary>
        /// Populate the combobox containing the template names associated with the classification.
        /// </summary>
        /// <param name="templateClassification"></param>
        private void LoadTabPageComboBox(string templateClassification)
        {
            bool available = false;
            try
            {
                var templateList = FormBase.VdwConfigurationSettings.templateList;

                localComboBoxGenerationTemplate.Items.Clear();
                localRichTextBoxGenerationTemplate.Clear();
                localLabelFullFilePath.Text = "";
                localLabelActiveConnectionKeyValue.Text = "";

                foreach (var templateDetail in templateList)
                {
                    if (templateDetail.TemplateType == templateClassification)
                    {
                        localComboBoxGenerationTemplate.Items.Add(templateDetail.TemplateName);
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
                localComboBoxGenerationTemplate.SelectedItem = localComboBoxGenerationTemplate.Items[0];
            }
            else
            {
                localComboBoxGenerationTemplate.ResetText();
                localComboBoxGenerationTemplate.SelectedIndex = -1;
            }
        }
    }
}
