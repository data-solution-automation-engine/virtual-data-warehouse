using HandlebarsDotNet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static Virtual_Data_Warehouse.FormBase;

namespace Virtual_Data_Warehouse
{
    public class MyEventArgs : EventArgs
    {
        public string Value { get; set; }

        public MyEventArgs(string value)
        {
            Value = value;
        }
    }

    public class MyClearArgs : EventArgs
    {
        public MyClearArgs()
        {
        }
    }



    class CustomTabpage : TabPage
    {
        string inputNiceName;
        //SqlConnection conn = new SqlConnection();
        StringBuilder inputMetadataQuery = new StringBuilder();

        List<string> itemList;

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
        /// <param name="input"></param>
        public CustomTabpage(string input, List<string> itemList)
        {
            inputNiceName = Regex.Replace(input, "(\\B[A-Z])", " $1");
            this.itemList = itemList;

            displayJsonFlag = true;
            generateInDatabaseFlag = true;
            saveOutputFileFlag = true;

            #region Main Tab Page
            // Base properties of custom tab page
            Name = $"tabPage{input}";
            Text = inputNiceName;
            BackColor = Color.Transparent;
            BorderStyle = BorderStyle.None;
            //Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left);
            UseVisualStyleBackColor = true;

            // Add Panel to faciliate docking
            //localPanel = new Panel();
            //Controls.Add(localPanel);
            ////localPanel.Dock = DockStyle.Fill;

            // Add 'Processing' Label
            localLabelProcessing = new Label();
            localLabelProcessing.Location = new Point(14, 12);
            localLabelProcessing.Size = new Size(123, 13);
            localLabelProcessing.Name = $"label{input}Processing";
            localLabelProcessing.Text = $"{inputNiceName} Processing";
            Controls.Add(localLabelProcessing);

            // Add 'Select All' CheckBox
            localCheckBoxSelectAll = new CheckBox();
            localCheckBoxSelectAll.Location = new Point(140, 11);
            localCheckBoxSelectAll.Size = new Size(69, 17);
            localCheckBoxSelectAll.Name = "checkBoxStagingAreaSelectAll";
            localCheckBoxSelectAll.Checked = true;
            localCheckBoxSelectAll.Text = "Select all";
            localCheckBoxSelectAll.CheckedChanged += new EventHandler(SelectAllCheckBoxItems);
            Controls.Add(localCheckBoxSelectAll);

            // Add Checked List Box
            localCheckedListBox = new CheckedListBox();
            localCheckedListBox.Location = new Point(17, 31);
            localCheckedListBox.Size = new Size(393, 377);
            localCheckedListBox.BorderStyle = BorderStyle.FixedSingle;
            localCheckedListBox.CheckOnClick = true;
            localCheckedListBox.Name = $"checkedListBox{input}";
            //localCheckedListBox.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left);
            Controls.Add(localCheckedListBox);

            // Add User feedback RichTextBox (left hand side of form)
            localRichTextBox = new RichTextBox();
            localRichTextBox.Location = new Point(17, 418);
            localRichTextBox.Size = new Size(393, 115);
            localRichTextBox.BorderStyle = BorderStyle.None;
            localRichTextBox.BackColor = SystemColors.Window;
            localRichTextBox.Text = $"The {inputNiceName} pattern type.";
            //localRichTextBox.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            Controls.Add(localRichTextBox);

            // Add 'Generate' Button 

            localButtonGenerate = new Button();            
            localButtonGenerate.Dock = DockStyle.None;
            //localButtonGenerate.Anchor = AnchorStyles.Left & AnchorStyles.Bottom;
            localButtonGenerate.Location = new Point(17, 542);
            localButtonGenerate.Size = new Size(109, 40);
            localButtonGenerate.Text = $"Generate {inputNiceName}";
            localButtonGenerate.Name = $"Generate{input}";
            localButtonGenerate.TabIndex = 4;
            //localButtonGenerate.UseVisualStyleBackColor = true;
            localButtonGenerate.Click += new EventHandler(Generate);
            Controls.Add(localButtonGenerate);






            // Add 'Filter' Group Box
            localGroupBoxFilter = new GroupBox();
            localGroupBoxFilter.Location = new Point(247, 539);
            localGroupBoxFilter.Size = new Size(163, 43);
            localGroupBoxFilter.Text = "Filter Criterion";
            localGroupBoxFilter.Name = $"groupBoxFilter{inputNiceName}";
            //localGroupBoxFilter.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            Controls.Add(localGroupBoxFilter);

            // Add 'Filter' Text Box
            localTextBoxFilter = new TextBox();
            localTextBoxFilter.Location = new Point(6, 15);
            localTextBoxFilter.Size = new Size(151, 20);
            localTextBoxFilter.Name = $"textBoxFilterCriterion{inputNiceName}";
            localTextBoxFilter.TextChanged += new EventHandler(FilterItemList);
            localGroupBoxFilter.Controls.Add(localTextBoxFilter);
            #endregion




            #region Sub Tab Pages
            // Add Sub Tab Control
            localTabControl = new TabControl();
            localTabControl.Location = new Point(416, 9);
            localTabControl.Size = new Size(896, 573);
            localTabControl.Name = $"tabControl{input}";
            localTabControl.BackColor = Color.White;
            //localTabControl.Anchor = (AnchorStyles.Top| AnchorStyles.Bottom |AnchorStyles.Left | AnchorStyles.Right);
            Controls.Add(localTabControl);

            // Add 'Generation Output' Tab Page on Sub Tab
            tabPageGenerationOutput = new TabPage($"{inputNiceName} Generation Output");
            tabPageGenerationOutput.BackColor = Color.Transparent;
            tabPageGenerationOutput.BorderStyle = BorderStyle.None;
            tabPageGenerationOutput.UseVisualStyleBackColor = true;
            localTabControl.TabPages.Add(tabPageGenerationOutput);

            // Add 'Generation Output' RichTextBox to Generation Output tab
            localRichTextBoxGenerationOutput = new RichTextBox();
            localRichTextBoxGenerationOutput.Text = $"No {inputNiceName} logic has been generated at the moment.";
            localRichTextBoxGenerationOutput.Location = new Point(3, 6);
            localRichTextBoxGenerationOutput.Size = new Size(882, 535);
            localRichTextBoxGenerationOutput.BorderStyle = BorderStyle.None;
            tabPageGenerationOutput.Controls.Add(localRichTextBoxGenerationOutput);

            // Add 'Pattern' Tab Page to on Sub Tab
            tabPageGenerationPattern = new TabPage($"{inputNiceName} Pattern");
            tabPageGenerationPattern.BackColor = Color.Transparent;
            tabPageGenerationPattern.BorderStyle = BorderStyle.None;
            tabPageGenerationPattern.UseVisualStyleBackColor = true;
            localTabControl.TabPages.Add(tabPageGenerationPattern);

            // Add 'Generation Pattern' RichTextBox to Pattern tab
            localRichTextBoxGenerationPattern = new RichTextBox();
            localRichTextBoxGenerationPattern.Location = new Point(3, 59);
            localRichTextBoxGenerationPattern.Size = new Size(882, 485);
            localRichTextBoxGenerationPattern.BorderStyle = BorderStyle.None;
            tabPageGenerationPattern.Controls.Add(localRichTextBoxGenerationPattern);

            // Add 'Pattern ComboBox' to Pattern tab
            localComboBoxGenerationPattern = new ComboBox();
            localComboBoxGenerationPattern.Location = new Point(108, 8);
            localComboBoxGenerationPattern.Size = new Size(496, 21);
            localComboBoxGenerationPattern.Name = $"comboBox{input}Pattern";
            localComboBoxGenerationPattern.SelectedIndexChanged += new EventHandler(DisplayPattern);
            tabPageGenerationPattern.Controls.Add(localComboBoxGenerationPattern);

            // Add 'Active Pattern' Label
            localLabelActivePattern = new Label();
            localLabelActivePattern.Location = new Point(2, 11);
            localLabelActivePattern.Size = new Size(77, 13);
            localLabelActivePattern.Name = $"label{input}ActivePattern";
            localLabelActivePattern.Text = "Active Pattern:";
            tabPageGenerationPattern.Controls.Add(localLabelActivePattern);

            // Add 'File Path' Label
            localLabelFilePath = new Label();
            localLabelFilePath.Location = new Point(2, 34);
            localLabelFilePath.Size = new Size(60, 13);
            localLabelFilePath.Name = $"label{input}FilePath";
            localLabelFilePath.Text = @"File Path:";
            tabPageGenerationPattern.Controls.Add(localLabelFilePath);

            // Add 'Full Path' Label
            localLabelFullFilePath = new Label();
            localLabelFullFilePath.Location = new Point(105, 34);
            localLabelFullFilePath.Size = new Size(496, 13);
            localLabelFullFilePath.Name = $"label{input}FullFilePath";
            localLabelFullFilePath.Text = @"<path>";
            tabPageGenerationPattern.Controls.Add(localLabelFullFilePath);

            // Add 'Save Pattern' Button
            localSavePattern = new Button();
            localSavePattern.Location = new Point(610, 7);
            localSavePattern.Size = new Size(101, 23);
            localSavePattern.Text = $"Save updates";
            localSavePattern.Name = $"Generate{input}";
            localSavePattern.Click += new EventHandler(SavePattern);
            tabPageGenerationPattern.Controls.Add(localSavePattern);
            #endregion

            #region Constructor Methods
            // Populate the Combo Box
            LoadTabPageComboBox(input);

            // Populate the Checked List Box
            SetItemList(itemList);
            #endregion
        }

        public void FilterItemList(object o, EventArgs e)
        {

        }

        public void SetItemList(List<string> itemList)
        {
            this.itemList = itemList;

            // Clear the existing checkboxes
            localCheckedListBox.Items.Clear();

            foreach (string item in itemList)
            {
                localCheckedListBox.Items.Add(item);
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
            var loadPattern = VedwConfigurationSettings.patternList.FirstOrDefault(x => x.LoadPatternName == localComboBoxGenerationPattern.Text);

            // Set the label with the path so it's visible to the user where the file is located
            localLabelFullFilePath.Text = loadPattern.LoadPatternFilePath;

            // Read the file from the path
            var loadPatternTemplate = File.ReadAllText(loadPattern.LoadPatternFilePath);

            // Display the pattern in the text box on the screen
            localRichTextBoxGenerationPattern.Text = loadPatternTemplate;

            // Make sure the pattern is stored in a global variable (memory) to overcome multithreading issues
            LoadPattern.ActivateLoadPattern(loadPatternTemplate, loadPattern.LoadPatternType);

            // Syntax highlight for Handlebars
            TextHandling.SyntaxHighlightHandlebars(localRichTextBoxGenerationPattern, localRichTextBoxGenerationPattern.Text);
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
            //var newThread = new Thread(DoWork);
            //newThread.Start();
            // localTabControl.SelectedIndex = 0;
            DoWork();
        }


        void DoWork()
        {
            Utility.CreateSchema(TeamConfigurationSettings.ConnectionStringStg);
            localRichTextBox.Clear();
            GenerateFromPattern();
        }

        /// <summary>
        ///   Create Staging Area SQL using Handlebars as templating engine
        /// </summary>
        private void GenerateFromPattern()
        {
            localRichTextBoxGenerationOutput.Clear();
            RaiseOnClearMainText();
            localTabControl.SelectedIndex = 0;

            int errorCounter = 0;

            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            var connPsa = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringHstg };

            if (localCheckedListBox.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= localCheckedListBox.CheckedItems.Count - 1; x++)
                {
                    var targetTableName = localCheckedListBox.CheckedItems[x].ToString();
                    //SetTextStg(@"Processing generation for " + targetTableName + "\r\n");

                    // Retrieve metadata and store in a data table object
                    var metadataQuery = @"SELECT 
                                             [SOURCE_SCHEMA_NAME]
                                            ,[SOURCE_NAME]
                                            ,[SOURCE_BUSINESS_KEY_DEFINITION]
                                            ,[TARGET_SCHEMA_NAME]
                                            ,[TARGET_NAME]
                                            ,[TARGET_BUSINESS_KEY_DEFINITION]
                                            ,[TARGET_TYPE]
                                            ,[SURROGATE_KEY]
                                            ,[FILTER_CRITERIA]
                                            ,[LOAD_VECTOR]
                                          FROM interface.INTERFACE_SOURCE_STAGING_XREF 
                                          WHERE TARGET_TYPE = 'StagingArea' 
                                          AND TARGET_NAME = '" + targetTableName + "'";

                    var metadataDataTable = Utility.GetDataTable(ref connOmd, metadataQuery);

                    // Move the data table to the class instance
                    List<SourceToTargetMapping> sourceToTargetMappingList = new List<SourceToTargetMapping>();

                    foreach (DataRow row in metadataDataTable.Rows)
                    {
                        // Creating the Business Key definition, using the available components (see above)
                        List<BusinessKey> businessKeyList = new List<BusinessKey>();
                        BusinessKey businessKey =
                            new BusinessKey
                            {
                                businessKeyComponentMapping = InterfaceHandling.BusinessKeyComponentMappingList((string)row["SOURCE_BUSINESS_KEY_DEFINITION"], "")
                            };

                        businessKeyList.Add(businessKey);

                        // Create the column-to-column mapping
                        var columnMetadataQuery = @"SELECT 
                                                      [SOURCE_ATTRIBUTE_NAME]
                                                     ,[TARGET_ATTRIBUTE_NAME]
                                                   FROM [interface].[INTERFACE_SOURCE_STAGING_ATTRIBUTE_XREF]
                                                   WHERE TARGET_NAME = '" + targetTableName + "' AND [SOURCE_NAME]='" + (string)row["SOURCE_NAME"] + "'";

                        var columnMetadataDataTable = Utility.GetDataTable(ref connOmd, columnMetadataQuery);

                        List<ColumnMapping> columnMappingList = new List<ColumnMapping>();
                        foreach (DataRow column in columnMetadataDataTable.Rows)
                        {
                            ColumnMapping columnMapping = new ColumnMapping();
                            Column sourceColumn = new Column();
                            Column targetColumn = new Column();

                            sourceColumn.columnName = (string)column["SOURCE_ATTRIBUTE_NAME"];
                            targetColumn.columnName = (string)column["TARGET_ATTRIBUTE_NAME"];

                            columnMapping.sourceColumn = sourceColumn;
                            columnMapping.targetColumn = targetColumn;

                            columnMappingList.Add(columnMapping);
                        }

                        var lookupTable = (string)row["TARGET_NAME"];
                        if (TeamConfigurationSettings.TableNamingLocation == "Prefix")
                        {
                            int prefixLocation = lookupTable.IndexOf(TeamConfigurationSettings.StgTablePrefixValue);

                            lookupTable = lookupTable.Remove(prefixLocation, TeamConfigurationSettings.StgTablePrefixValue.Length).Insert(prefixLocation, TeamConfigurationSettings.PsaTablePrefixValue);
                        }
                        else
                        {
                            int prefixLocation = lookupTable.LastIndexOf(TeamConfigurationSettings.StgTablePrefixValue);

                            lookupTable = lookupTable.Remove(prefixLocation, TeamConfigurationSettings.StgTablePrefixValue.Length).Insert(prefixLocation, TeamConfigurationSettings.PsaTablePrefixValue);
                        }

                        // Add the created Business Key to the source-to-target mapping
                        var sourceToTargetMapping = new SourceToTargetMapping();

                        sourceToTargetMapping.sourceTable = (string)row["SOURCE_NAME"];
                        sourceToTargetMapping.targetTable = (string)row["TARGET_NAME"];
                        sourceToTargetMapping.lookupTable = lookupTable;
                        //sourceToTargetMapping.targetTableHashKey = (string)row["SURROGATE_KEY"];
                        sourceToTargetMapping.businessKey = businessKeyList;
                        sourceToTargetMapping.filterCriterion = (string)row["FILTER_CRITERIA"];
                        sourceToTargetMapping.columnMapping = columnMappingList;

                        // Add the source-to-target mapping to the mapping list
                        sourceToTargetMappingList.Add(sourceToTargetMapping);
                    }

                    // Create an instance of the 'MappingList' class / object model 
                    SourceToTargetMappingList sourceTargetMappingList = new SourceToTargetMappingList();
                    sourceTargetMappingList.individualSourceToTargetMapping = sourceToTargetMappingList;
                    sourceTargetMappingList.metadataConfiguration = new MetadataConfiguration();
                    sourceTargetMappingList.mainTable = targetTableName;

                    // Return the result to the user
                    try
                    {
                        // Compile the template, and merge it with the metadata
                        var template = Handlebars.Compile(VedwConfigurationSettings.activeLoadPatternStg);
                        var result = template(sourceTargetMappingList);

                        // Check if the metadata needs to be displayed also

                        //DisplayJsonMetadata(sourceTargetMappingList, "StagingArea");

                        if (displayJsonFlag == true)
                        {
                            try
                            {
                                var json = JsonConvert.SerializeObject(sourceTargetMappingList, Formatting.Indented);
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
                        errorCounter = Utility.SaveOutputToDisk(saveOutputFileFlag, localRichTextBoxGenerationOutput.Text + @"\Output_" + targetTableName + ".sql", result, errorCounter);

                        //Generate in database
                        errorCounter = Utility.ExecuteOutputInDatabase(generateInDatabaseFlag, connPsa, result, errorCounter);
                    }
                    catch (Exception ex)
                    {
                        errorCounter++;
                        RaiseOnChangeMainText("The template could not be compiled, the error message is " + ex);
                        //SetTextMain("The template could not be compiled, the error message is " + ex);
                    }
                }
            }
            else
            {
                localRichTextBox.AppendText($"There was no metadata selected to generate {inputNiceName} code. Please check the metadata schema - are there any {inputNiceName} objects selected?");
            }

            connOmd.Close();
            connOmd.Dispose();

            RaiseOnChangeMainText($"\r\n{errorCounter} errors have been found.\r\n");
            RaiseOnChangeMainText($"Associated scripts have been saved in {VedwConfigurationSettings.VedwOutputPath}.\r\n");

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
                var patternList = VedwConfigurationSettings.patternList;

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

            if (available == true)
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

            //try
            //{
            //    var tables = Utility.GetDataTable(ref conn, inputQuery.ToString());

            //    if (tables.Rows.Count == 0)
            //    {
            //        localRichTextBox.AppendText($"There was no metadata available to display {patternNiceName} content. Please check the metadata schema (are there any {patternNiceName} tables available?) or the database connection.");
            //    }

            //    foreach (DataRow row in tables.Rows)
            //    {
            //        localCheckedListBox.Items.Add(row["TARGET_NAME"]);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    returnDetails.AppendLine($"Unable to populate the {patternNiceName} selection, there is no database connection.");
            //    returnDetails.AppendLine("Error logging details: " + ex);
            //}

            for (int x = 0; x <= localCheckedListBox.Items.Count - 1; x++)
            {
                localCheckedListBox.SetItemChecked(x, localCheckBoxSelectAll.Checked);
            }
            return returnDetails;
        }
    }
}
