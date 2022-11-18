using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DataWarehouseAutomation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TEAM_Library;

namespace Virtual_Data_Warehouse
{
    public partial class FormMain : FormBase
    {
        internal bool startUpIndicator;
        private readonly List<CustomTabPage> localCustomTabPageList;
        private readonly BindingSource _bindingSourceLoadPatternCollection = new BindingSource();

        FormAlert _alertEventLog;

        public FormMain()
        {
            localCustomTabPageList = new List<CustomTabPage>();

            InitializeComponent();

            // Set the version of the build for everything
            const string versionNumberForApplication = "v1.6.8";

            Text = $"Virtual Data Warehouse - {versionNumberForApplication}";
            labelWelcome.Text = $"{labelWelcome.Text} - {versionNumberForApplication}";

            VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"{Text}."));

            #region Root Paths

            // Make sure the root directories exist, based on (tool) parameters.

            // Core Path
            FileHandling.InitialisePath(GlobalParameters.CorePath, TeamPathTypes.CorePath, VdwConfigurationSettings.VdwEventLog);
            VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"Core path set as {GlobalParameters.CorePath}."));

            // Configuration Path
            FileHandling.InitialisePath(GlobalParameters.VdwConfigurationPath, TeamPathTypes.ConfigurationPath, VdwConfigurationSettings.VdwEventLog);
            VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"Configuration path initialised for {GlobalParameters.VdwConfigurationPath}."));

            // Input Path.
            FileHandling.InitialisePath(VdwConfigurationSettings.VdwMetadatPath, TeamPathTypes.InputPath, VdwConfigurationSettings.VdwEventLog);
            VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"Input path set for {VdwConfigurationSettings.VdwMetadatPath}."));

            // Output Path.
            FileHandling.InitialisePath(VdwConfigurationSettings.VdwOutputPath, TeamPathTypes.OutputPath, VdwConfigurationSettings.VdwEventLog);
            VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"Output path set for {VdwConfigurationSettings.VdwOutputPath}."));

            // Examples Path.
            FileHandling.InitialisePath(VdwConfigurationSettings.VdwExamplesPath, TeamPathTypes.MetadataPath, VdwConfigurationSettings.VdwEventLog);
            VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"Examples path set for {VdwConfigurationSettings.VdwOutputPath}."));

            #endregion

            // Create the initial VDW configuration file, if it doesn't exist (first time use or change of directory).
            VdwUtility.CreateNewVdwConfigurationFile();

            // Load the VDW configuration from disk, commit to memory and display on the form.
            VdwUtility.LoadVdwConfigurationFile();

            // Update the values on the form.
            textBoxOutputPath.Text = VdwConfigurationSettings.VdwOutputPath;
            textBoxLoadPatternPath.Text = VdwConfigurationSettings.LoadPatternPath;
            textBoxTeamEnvironmentsFilePath.Text = VdwConfigurationSettings.TeamEnvironmentFilePath;
            textBoxTeamConfigurationPath.Text = VdwConfigurationSettings.TeamConfigurationPath;
            textBoxTeamConnectionsPath.Text = VdwConfigurationSettings.TeamConnectionsPath;
            textBoxMetadataPath.Text = VdwConfigurationSettings.VdwMetadatPath;
            textBoxSchemaName.Text = VdwConfigurationSettings.VdwSchema;

            // Then load the environments file and current working environment.
            // The TeamEnvironmentCollection contains all the environments as specified in TEAM (environments file).
            try
            {
                TeamEnvironmentCollection.LoadTeamEnvironmentCollection(VdwConfigurationSettings.TeamEnvironmentFilePath);
            }
            catch
            {
                richTextBoxInformationMain.AppendText("The root environment configuration file was not found. The following file was expected: " + VdwConfigurationSettings.TeamEnvironmentFilePath + '.');
            }

            VdwConfigurationSettings.ActiveEnvironment = TeamEnvironmentCollection.GetEnvironmentById(VdwConfigurationSettings.TeamSelectedEnvironmentInternalId);

            // Load the configuration and connection information from file, based on the selected environment and input path.
            VdwUtility.LoadTeamConnectionsFileForVdw(VdwConfigurationSettings.ActiveEnvironment.environmentKey);
            VdwUtility.LoadTeamConfigurationFileForVdw(VdwConfigurationSettings.ActiveEnvironment.environmentKey);

            PopulateEnvironmentComboBox();
            comboBoxEnvironments.SelectedIndex = comboBoxEnvironments.FindStringExact(VdwConfigurationSettings.TeamSelectedEnvironmentInternalId);

            var comboItem = comboBoxEnvironments.Items.Cast<KeyValuePair<string, TeamEnvironment>>().FirstOrDefault(item => item.Value.Equals(VdwConfigurationSettings.ActiveEnvironment));
            comboBoxEnvironments.SelectedItem = comboItem;

            richTextBoxInformationMain.AppendText("Application initialised - welcome to the Virtual Data Warehouse! \r\n\r\n");

            checkBoxGenerateInDatabase.Checked = false;

            #region Load Pattern Grid

            // Load the pattern collection into memory.
            VdwConfigurationSettings.patternList = LoadPatternFileHandling.LoadPatternCollection();

            if ((VdwConfigurationSettings.patternList != null) && (!VdwConfigurationSettings.patternList.Any()))
            {
                SetTextMain("There are no patterns found in the designated load pattern directory. Please verify if there is a " + GlobalParameters.LoadPatternListFileName + " in the " + VdwConfigurationSettings.LoadPatternPath + " directory, and if the file contains patterns.");
            }

            _loadPatternGridView = new LoadPatternGridView(TeamConfigurationSettings);

            // Define and populate the data grid.
            ((ISupportInitialize)(_loadPatternGridView)).BeginInit();
            _loadPatternGridView.DoubleBuffered(true);
            tabPageSettings.Controls.Add(_loadPatternGridView);
            ((ISupportInitialize)(_loadPatternGridView)).EndInit();

            PopulateLoadPatternCollectionDataGrid();

            _loadPatternGridView.AutoLayout();

            #endregion

            #region Pattern Tab Pages

            // Create the tab pages based on available content.
            CreateCustomTabPages();

            foreach (CustomTabPage localCustomTabPage in localCustomTabPageList)
            {
                localCustomTabPage.SetDisplayJsonFlag(true);
                localCustomTabPage.SetDisplayJsonFlag(false);

                localCustomTabPage.SetGenerateInDatabaseFlag(true);
                localCustomTabPage.SetGenerateInDatabaseFlag(false);

                localCustomTabPage.SetSaveOutputFileFlag(false);
                localCustomTabPage.SetSaveOutputFileFlag(true);
            }

            #endregion

            // Start monitoring the configuration directories for file changes
            RunFileWatcher();

            startUpIndicator = false;
        }

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        public void PopulateLoadPatternCollectionDataGrid()
        {
            // Convert into data table
            DataTable patternDataTable = VdwConfigurationSettings.patternList.ToDataTable();

            // Accept changes
            patternDataTable.AcceptChanges();

            // Handle unknown combobox values, by setting them to empty.
            var localConnectionKeyList = LocalTeamConnection.TeamConnectionKeyList(TeamConfigurationSettings.ConnectionDictionary);
            List<string> userFeedbackList = new List<string>();

            foreach (DataRow row in patternDataTable.Rows)
            {
                var comboBoxValueConnectionKey = row["LoadPatternConnectionKey"].ToString();

                if (!localConnectionKeyList.Contains(comboBoxValueConnectionKey))
                {
                    if (!userFeedbackList.Contains(comboBoxValueConnectionKey) && comboBoxValueConnectionKey!="")
                    {
                        userFeedbackList.Add(comboBoxValueConnectionKey);
                    }

                    row["LoadPatternConnectionKey"] = DBNull.Value;
                }
            }

            // Provide user feedback is any of the connections have been invalidated.
            if (userFeedbackList.Count > 0)
            {
                foreach (string issue in userFeedbackList)
                {
                    richTextBoxInformationMain.AppendText($"The connection '{issue}' found in the metadata file does not seem to exist in TEAM. The value has been defaulted in the grid, but not saved yet.\r\n");
                }
            }

            //Make sure the changes are seen as committed, so that changes can be detected later on.
            patternDataTable.AcceptChanges();

            _bindingSourceLoadPatternCollection.DataSource = patternDataTable;
            _loadPatternGridView.DataSource = _bindingSourceLoadPatternCollection;
        }
        
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void RunFileWatcher()
        {
            try
            {
                // Create a new FileSystemWatcher and set its properties.
                FileSystemWatcher watcher = new FileSystemWatcher
                {
                    Path = VdwConfigurationSettings.VdwMetadatPath,
                    Filter = "*.json",
                };

                // Add event handlers.
                watcher.Changed += FileWatcherOnChanged;
                watcher.Created += FileWatcherOnChanged;
                watcher.Deleted += FileWatcherOnChanged;
                watcher.Error += FileWatcherOnError;

                watcher.SynchronizingObject = this;
                // Begin watching.
                watcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                richTextBoxInformationMain.AppendText($"There was en error starting the file watcher: {ex}.");
            }
        }

        private void FileWatcherOnError(object source, ErrorEventArgs e)
        {
            if (e.GetException().GetType() == typeof(InternalBufferOverflowException))
            {
                InformUser("File System Watcher internal buffer overflow.",EventTypes.Error);
            }
            else
            {
                InformUser($"Watched directory {VdwConfigurationSettings.VdwMetadatPath} not accessible by the system.", EventTypes.Error);
            }
        }

        // Define the event handlers.
        private void FileWatcherOnChanged(object source, FileSystemEventArgs e)
        {
            // Make sure the damn thing only fires once.
            var localFileSystemWatcher = (FileSystemWatcher) source;
            try
            {

                localFileSystemWatcher.EnableRaisingEvents = false;

                InformUser($"File {e.Name} was modified in {VdwConfigurationSettings.VdwMetadatPath}.", EventTypes.Information);

                CreateCustomTabPages();

            }

            finally
            {
                localFileSystemWatcher.EnableRaisingEvents = true;
            }
        }


        private void openOutputDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(VdwConfigurationSettings.VdwOutputPath);
            }
            catch (Exception ex)
            {
                richTextBoxInformationMain.Text = "An error has occurred while attempting to open the output directory. The error message is: " + ex.Message;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Process.Start(ExtensionMethod.GetDefaultBrowserPath(),"http://roelantvos.com/blog/articles-and-white-papers/virtualisation-software/");
        }
        
 
        #region Multi-threading delegates

        /// <summary>
        /// Delegate to update the main information textbox.
        /// </summary>
        /// <param name="text"></param>
        delegate void SetTextCallBackDebug(string text);

        private void SetTextMain(string text)
        {
            if (richTextBoxInformationMain.InvokeRequired)
            {
                var d = new SetTextCallBackDebug(SetTextMain);
                Invoke(d, text);
            }
            else
            {
                richTextBoxInformationMain.AppendText(text);
            }
        }


        delegate void CallBackAddCustomTabPage(TabPage tabPage);

        private void AddCustomTabPage(TabPage tabPage)
        {
            if (tabControlMain.InvokeRequired)
            {
                var d = new CallBackAddCustomTabPage(AddCustomTabPage);
                Invoke(d, tabPage);
            }
            else
            {
                tabControlMain.TabPages.Add(tabPage);

            }
        }

        delegate void CallBackRemoveCustomTabPage(TabPage tabPage);

        private void RemoveCustomTabPage(TabPage tabPage)
        {
            if (tabControlMain.InvokeRequired)
            {
                var d = new CallBackRemoveCustomTabPage(RemoveCustomTabPage);
                Invoke(d, tabPage);
            }
            else
            {
                tabControlMain.TabPages.Remove(tabPage);
            }
        }

        #endregion

        #region Background worker

        // This event handler deals with the results of the background operation.
        private void backgroundWorkerActivateMetadata_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // labelResult.Text = "Cancelled!";
                richTextBoxInformationMain.AppendText("Cancelled!");
            }
            else if (e.Error != null)
            {
                richTextBoxInformationMain.AppendText("Error: " + e.Error.Message);
            }
            else
            {
                richTextBoxInformationMain.AppendText("Done. The metadata was processed succesfully!\r\n");
                //SetVersion(trackBarVersioning.Value);
            }

            // Close the AlertForm
            //alert.Close();
        }

        // This event handler updates the progress.
        private void backgroundWorkerActivateMetadata_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Show the progress in main form (GUI)
            //labelResult.Text = (e.ProgressPercentage + "%");

            // Pass the progress to AlertForm label and progressbar


            // Manage the logging
        }

        # endregion

        // This event handler is where the time-consuming work is done.
        private void backgroundWorker_DoWorkMetadataActivation(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            // Handling multithreading
            if (worker != null && worker.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Save VDW settings in the from to memory & disk
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveConfigurationFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Make sure the paths contain a backslash.
            if (textBoxTeamEnvironmentsFilePath.Text.EndsWith(@"\"))
            {
                textBoxTeamEnvironmentsFilePath.Text = textBoxTeamEnvironmentsFilePath.Text.Replace(@"\", "");
            }

            if (!textBoxTeamConfigurationPath.Text.EndsWith(@"\"))
            {
                textBoxTeamConfigurationPath.Text += @"\";
            }

            if (!textBoxMetadataPath.Text.EndsWith(@"\"))
            {
                textBoxMetadataPath.Text += @"\";
            }

            if (!textBoxOutputPath.Text.EndsWith(@"\"))
            {
                textBoxOutputPath.Text += @"\";
            }

            if (!textBoxLoadPatternPath.Text.EndsWith(@"\"))
            {
                textBoxLoadPatternPath.Text += @"\";
            }

            new KeyValuePair< string, TeamEnvironment > ("", null);

            if (comboBoxEnvironments.SelectedItem != null)
            {
                var selectedEnvironment = (KeyValuePair<string, TeamEnvironment>)comboBoxEnvironments.SelectedItem;
                VdwConfigurationSettings.TeamSelectedEnvironmentInternalId = selectedEnvironment.Value.environmentInternalId;
            }
            else
            {
                VdwConfigurationSettings.TeamSelectedEnvironmentInternalId = null;
            }

            // Make sure that the updated paths are accessible from anywhere in the app (global parameters)
            VdwConfigurationSettings.TeamEnvironmentFilePath = textBoxTeamEnvironmentsFilePath.Text;
            VdwConfigurationSettings.TeamConfigurationPath = textBoxTeamConfigurationPath.Text;
            VdwConfigurationSettings.TeamConnectionsPath = textBoxTeamConnectionsPath.Text;
            VdwConfigurationSettings.LoadPatternPath = textBoxLoadPatternPath.Text;
            VdwConfigurationSettings.VdwMetadatPath = textBoxMetadataPath.Text;
            VdwConfigurationSettings.VdwOutputPath = textBoxOutputPath.Text;
            VdwConfigurationSettings.VdwSchema = textBoxSchemaName.Text;

            // Update the root path file (from memory)
            var rootPathConfigurationFile = new StringBuilder();
            rootPathConfigurationFile.AppendLine("/* Virtual Data Warehouse Core Settings */");
            rootPathConfigurationFile.AppendLine("/* Saved at " + DateTime.Now + " */");
            rootPathConfigurationFile.AppendLine("TeamEnvironmentFilePath|" + VdwConfigurationSettings.TeamEnvironmentFilePath + "");
            rootPathConfigurationFile.AppendLine("TeamConfigurationPath|" + VdwConfigurationSettings.TeamConfigurationPath + "");
            rootPathConfigurationFile.AppendLine("TeamConnectionsPath|" + VdwConfigurationSettings.TeamConnectionsPath + "");
            rootPathConfigurationFile.AppendLine("TeamSelectedEnvironment|" + VdwConfigurationSettings.TeamSelectedEnvironmentInternalId + "");
            rootPathConfigurationFile.AppendLine("InputPath|" + VdwConfigurationSettings.VdwMetadatPath + "");
            rootPathConfigurationFile.AppendLine("OutputPath|" + VdwConfigurationSettings.VdwOutputPath + "");
            rootPathConfigurationFile.AppendLine("LoadPatternPath|" + VdwConfigurationSettings.LoadPatternPath + "");
            rootPathConfigurationFile.AppendLine("VdwSchema|" + VdwConfigurationSettings.VdwSchema + "");
            rootPathConfigurationFile.AppendLine("/* End of file */");

            // Save the VDW core settings file to disk
            using (var outfile = new StreamWriter(GlobalParameters.CorePath + GlobalParameters.VdwConfigurationFileName))
            {
                outfile.Write(rootPathConfigurationFile.ToString());
                outfile.Close();
            }

            // Reload the VDW and TEAM settings, as the environment may have changed
            VdwUtility.LoadVdwConfigurationFile();

            // Recreate the in-memory patterns (to make sure MetadataGeneration object details are also added).
            CreateCustomTabPages();

            richTextBoxInformationMain.Text = DateTime.Now+" - the global parameter file (" + GlobalParameters.VdwConfigurationFileName + ") has been updated in: " + GlobalParameters.CorePath+"\r\n\r\n";
        }

        private void openTEAMConfigurationSettingsFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(VdwConfigurationSettings.TeamEnvironmentFilePath);
            }
            catch (Exception ex)
            {
                richTextBoxInformationMain.Text = "An error has occurred while attempting to open the TEAM configuration file. The error message is: " + ex.Message;
            }
        }

        private void richTextBoxInformationMain_TextChanged(object sender, EventArgs e)
        {
            // Set the current caret position to the end
            richTextBoxInformationMain.SelectionStart = richTextBoxInformationMain.Text.Length;
            // Scroll automatically
            richTextBoxInformationMain.ScrollToCaret();
        }

        private DialogResult STAShowDialog(FileDialog dialog)
        {
            var state = new DialogState {FileDialog = dialog};
            var t = new Thread(state.ThreadProcShowDialog);
            t.SetApartmentState(ApartmentState.STA);

            t.Start();
            t.Join();

            return state.DialogResult;
        }

        public class DialogState
        {
            public DialogResult DialogResult;
            public FileDialog FileDialog;

            public void ThreadProcShowDialog()
            {
                DialogResult = FileDialog.ShowDialog();
            }
        }

        private void openVDWConfigurationSettingsFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(GlobalParameters.VdwConfigurationPath + GlobalParameters.VdwConfigurationFileName);

            }
            catch (Exception ex)
            {
                richTextBoxInformationMain.Text = "An error has occurred while attempting to open the VDW configuration file. The error message is: " + ex.Message;
            }
        }

        private void UpdateMainInformationTextBox(Object o, MyEventArgs e)
        {
            richTextBoxInformationMain.AppendText(e.Value);
        }

        private void ClearMainInformationTextBox(Object o, MyClearArgs e)
        {
            richTextBoxInformationMain.Clear();
        }


        internal class LocalPattern
        {
            internal string classification { get; set; }
            internal string notes { get; set; }
            internal Dictionary<string,VDW_DataObjectMappingList> itemList { get; set; }
        }


        internal void InformUser(string text, EventTypes eventType)
        {
            VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(eventType, $"{text}"));
            SetTextMain(text + "\r\n");
        }

        /// <summary>
        /// Load all the metadata into a single list and associate with a pattern, based on the classification of the mapping (i.e. CoreBusinessConcept).
        /// </summary>
        /// <returns></returns>
        internal List<LocalPattern> GetMetadata()
        {
            #region Deserialisation

            // Deserialise the Json files into a local List of Data Object Mappings (mappingList) for further use.
            List<VDW_DataObjectMappingList> mappingList = new List<VDW_DataObjectMappingList>();

            if (Directory.Exists(VdwConfigurationSettings.VdwMetadatPath))
            {
                string[] fileEntries = Directory.GetFiles(VdwConfigurationSettings.VdwMetadatPath, "*.json");

                // Hard-coded exclusions
                string[] excludedFiles =
                {
                    "interfaceBusinessKeyComponent.json", 
                    "interfaceBusinessKeyComponentPart.json",
                    "interfaceDrivingKey.json", 
                    "interfaceHubLinkXref.json", 
                    "interfacePhysicalModel.json",
                    "interfaceSourceHubXref.json", 
                    "interfaceSourceLinkAttributeXref.json",
                    "Development_TEAM_Model_Metadata.json",
                    "Development_TEAM_Attribute_Mapping.json",
                    "Development_TEAM_Table_Mapping.json",
                    "Production_TEAM_Model_Metadata.json",
                    "Production_TEAM_Attribute_Mapping.json",
                    "Production_TEAM_Table_Mapping.json"
                };

                if (fileEntries.Length > 0)
                {
                    foreach (var fileName in fileEntries)
                    {
                        if (!Array.Exists(excludedFiles, x => x == Path.GetFileName(fileName)) && 
                            !fileName.EndsWith("TEAM_Table_Mapping.json") && 
                            !fileName.EndsWith("TEAM_Attribute_Mapping.json") && 
                            !fileName.EndsWith("TEAM_Model_Metadata.json"))
                        {
                            try
                            {
                                // Validate the file contents against the schema definition.
                                if (File.Exists(Application.StartupPath + @"\Schema\" + GlobalParameters.JsonSchemaForDataWarehouseAutomationFileName))
                                {
                                    var result = DataWarehouseAutomation.JsonHandling.ValidateJsonFileAgainstSchema(Application.StartupPath + @"\Schema\" + GlobalParameters.JsonSchemaForDataWarehouseAutomationFileName, fileName);

                                    foreach (var error in result.Errors)
                                    {
                                        VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Error, $"An error was encountered validating the contents {fileName}.{error.Message}. This occurs at line {error.LineNumber}."));
                                    }
                                }
                                else
                                {
                                    VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Error, $"An error occurred while validating the file against the Data Warehouse Automation schema. Does the schema file exist?"));
                                }

                                // Add the deserialised file to the list of mappings.
                                VDW_DataObjectMappingList deserialisedMapping;

                                var jsonInput = File.ReadAllText(fileName);
                                deserialisedMapping = JsonConvert.DeserializeObject<VDW_DataObjectMappingList>(jsonInput);

                                if (deserialisedMapping != null)
                                {
                                    deserialisedMapping.metadataFileName = fileName;

                                    mappingList.Add(deserialisedMapping);
                                }
                            }
                            catch (Exception exception)
                            {
                                InformUser($"The file '{fileName}' could not be loaded properly. The reported error is {exception.Message}", EventTypes.Error);
                            }
                        }
                    }
                }
                else
                {
                    InformUser($"No files were detected in directory {VdwConfigurationSettings.VdwMetadatPath}.",
                        EventTypes.Warning);
                }
            }
            else
            {
                InformUser(
                    $"There were issues accessing the directory {VdwConfigurationSettings.VdwMetadatPath}. It does not seem to exist.",
                    EventTypes.Warning);
            }

            #endregion

            // The intended outcome is to have a list of patterns (LocalPattern class) to return back for further processing
            // This list of patterns is based on the classifications, each pattern is mapped to a classification and so the classifications are unique.
            // Each classification is bound to the 'item list' of the pattern, which is the name of the mapping and the list of associated Data Object Mappings.
            // In a way, it's bringing the classification to a higher level - from the Data Object mapping to the Data Object Mapping List level.

            #region Flattening

            // First step, re-ordering and flattening.
            // In the Tuple, Item1 is the classification, Item2 is the mapping name and Item 3 is notes.
            Dictionary<VDW_DataObjectMappingList, Tuple<string, string, string>> objectDictionary =
                new Dictionary<VDW_DataObjectMappingList, Tuple<string, string, string>>();

            if (mappingList.Any())
            {
                foreach (VDW_DataObjectMappingList dataObjectMappings in mappingList)
                {
                    if (dataObjectMappings.dataObjectMappings != null)
                    {
                        foreach (DataObjectMapping dataObjectMapping in dataObjectMappings.dataObjectMappings)
                        {
                            if (dataObjectMapping.mappingName == null)
                            {
                                dataObjectMapping.mappingName = dataObjectMapping.targetDataObject.name;
                                InformUser(
                                    $"The Data Object Mapping for target {dataObjectMapping.targetDataObject.name} does not have a mapping name, so the target name is used.",
                                    EventTypes.Warning);
                            }
                            // Check if there are classifications, as these are used to create the tabs.
                            if (dataObjectMapping.mappingClassifications != null)
                            {
                                foreach (Classification classification in dataObjectMapping.mappingClassifications)
                                {
                                    if (!objectDictionary.ContainsKey(dataObjectMappings))
                                    {
                                        objectDictionary.Add(dataObjectMappings,
                                            new Tuple<string, string, string>(classification.classification,
                                                dataObjectMapping.mappingName, classification.notes));
                                    }
                                }
                            }
                            else
                            {
                                if (!objectDictionary.ContainsKey(dataObjectMappings))
                                {
                                    objectDictionary.Add(dataObjectMappings,
                                        new Tuple<string, string, string>("Miscellaneous",
                                            dataObjectMapping.mappingName, ""));
                                }

                                InformUser(
                                    $"The Data Object Mapping {dataObjectMapping.mappingName} does not have a classification, and therefore will be placed under 'Miscellaneous'",
                                    EventTypes.Warning);
                            }
                        }
                    }
                    else
                    {
                        InformUser(
                            $"There are no valid Data Object Mappings found in the file {dataObjectMappings.metadataFileName}. Please check the Event Log, and if this file has valid Json contents.",
                            EventTypes.Warning);
                    }
                }
            }
            else
            {
                InformUser($"The list of Data Object Mappings is empty.", EventTypes.Warning);
            }

            #endregion

            // Now use the full validated and re-organised set to create a list of Data Object Mapping lists (list of DataObjectMappings) for each classification.
            // This will generate the tab pages.

            // Create base list of classification / types to become the tab pages (key: classification, value: notes).
            Dictionary<string, string> classificationDictionary = new Dictionary<string, string>();

            foreach (var objectRow in objectDictionary)
            {
                if (!classificationDictionary.ContainsKey(objectRow.Value.Item1))
                {
                    classificationDictionary.Add(objectRow.Value.Item1, objectRow.Value.Item3);
                }
            }

            // Create the final list
            List<LocalPattern> finalMappingList = new List<LocalPattern>();

            foreach (var classification in classificationDictionary)
            {
                LocalPattern localPatternMapping = new LocalPattern();
                Dictionary<string, VDW_DataObjectMappingList> itemList =
                    new Dictionary<string, VDW_DataObjectMappingList>();

                foreach (var objectRow in objectDictionary)
                {
                    if (objectRow.Value.Item1 == classification.Key)
                    {
                        if (!itemList.ContainsKey(objectRow.Value.Item2))
                        {
                            itemList.Add(objectRow.Value.Item2, objectRow.Key);
                        }
                    }
                }

                localPatternMapping.classification = classification.Key;
                localPatternMapping.notes = classification.Value;
                localPatternMapping.itemList = itemList;

                finalMappingList.Add(localPatternMapping);
            }

            return finalMappingList;
        }

        /// <summary>
        /// Generates the Custom Tab Pages using the pattern metadata. This method will remove any non-standard Tab Pages and create these using the Load Pattern Definition metadata.
        /// </summary>
        internal void CreateCustomTabPages()
        {
            // Save all work when reloading
            if (startUpIndicator == false)
            {
                // TO DO, save the pattern before reload to not lose work.
            }

            // Remove any existing Custom Tab Pages before rebuild
            localCustomTabPageList.Clear();
            foreach (TabPage customTabPage in tabControlMain.TabPages)
            {
                if ((customTabPage.Name == "tabPageHome") || (customTabPage.Name == "tabPageSettings"))
                {
                    // Do nothing, as only the two standard Tab Pages exist.
                }
                else
                {
                    // Remove the Tab Page from the Tab Control
                    RemoveCustomTabPage(customTabPage);
                }
            }

            List<LocalPattern> finalMappingList = GetMetadata();
            var sortedMappingList = finalMappingList.OrderBy(x => x.classification);

            // Add the Custom Tab Pages
            foreach (var pattern in sortedMappingList)
            {
                CustomTabPage localCustomTabPage = new CustomTabPage(pattern.classification, pattern.notes, pattern.itemList);
                localCustomTabPage.OnChangeMainText += UpdateMainInformationTextBox;
                localCustomTabPage.OnClearMainText += ClearMainInformationTextBox;

                localCustomTabPageList.Add(localCustomTabPage);

                AddCustomTabPage(localCustomTabPage);
            }

            // Work around issue related to incorrectly enabled metadata extract
            if (checkBoxGenerateJsonSchema.Checked)
            {
                checkBoxGenerateJsonSchema.Checked = false;
                checkBoxGenerateJsonSchema.Checked = true;
            }
            else
            {
                checkBoxGenerateJsonSchema.Checked = true;
                checkBoxGenerateJsonSchema.Checked = false;
            }

            // Work around issue related to incorrectly enabled metadata extract
            if (checkBoxGenerateInDatabase.Checked)
            {
                checkBoxGenerateInDatabase.Checked = false;
                checkBoxGenerateInDatabase.Checked = true;
            }
            else
            {
                checkBoxGenerateInDatabase.Checked = true;
                checkBoxGenerateInDatabase.Checked = false;
            }


            // Set the tabpages back to what they were before reload
            if (startUpIndicator == false)
            {
                var currentMainTab = VdwConfigurationSettings.SelectedMainTab;

                if (currentMainTab != null && currentMainTab != "")
                {
                    try
                    {
                        tabControlMain.SelectTab(tabControlMain.TabPages[currentMainTab]);
                    }
                    catch (Exception ex)
                    {
                        VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Warning, $"An exception was encountered creating a new tab page. The exception is {ex}."));
                    }
                }
            }
        }



        private void checkBoxGenerateInDatabase_CheckedChanged(object sender, EventArgs e)
        {
            foreach (CustomTabPage localTabPage in localCustomTabPageList)
            {
                if (checkBoxGenerateInDatabase.Checked)
                {
                    localTabPage.SetGenerateInDatabaseFlag(true);
                }
                else
                {
                    localTabPage.SetGenerateInDatabaseFlag(false);
                }
            }
        }

        private void checkBoxGenerateJsonSchema_CheckedChanged(object sender, EventArgs e)
        {
            foreach (CustomTabPage localTabPage in localCustomTabPageList)
            {
                if (checkBoxGenerateJsonSchema.Checked)
                {
                    localTabPage.SetDisplayJsonFlag(true);
                }
                else
                {
                    localTabPage.SetDisplayJsonFlag(false);
                }
            }
        }

        private void checkBoxSaveToFile_CheckedChanged(object sender, EventArgs e)
        {
            foreach (CustomTabPage localTabPage in localCustomTabPageList)
            {
                if (checkBoxSaveToFile.Checked)
                {
                    localTabPage.SetSaveOutputFileFlag(true);
                }
                else
                {
                    localTabPage.SetSaveOutputFileFlag(false);
                }
            }
        }

        private void buttonRefreshMetadata_Click(object sender, EventArgs e)
        {
            // Get the total of tab pages to create
            var patternList = GetMetadata();

            // Get the name of the active tab so this can be refreshed
            string tabName = tabControlMain.SelectedTab.Name;

            foreach (CustomTabPage customTabPage in localCustomTabPageList)
            {
                if (customTabPage.Name == tabName)
                {
                    foreach (LocalPattern localPattern in patternList)
                    {
                        if (localPattern.classification == tabName)
                        {
                            customTabPage.SetItemList(localPattern.itemList);
                        }
                    }
                }
            }
        }

        public void PopulateEnvironmentComboBox()
        {
            foreach (var environment in FormBase.TeamEnvironmentCollection.EnvironmentDictionary)
            {
                // Adding items in the drop down list
                comboBoxEnvironments.Items.Add(new KeyValuePair<string, TeamEnvironment>(environment.Value.environmentKey, environment.Value));
                comboBoxEnvironments.DisplayMember = "Key";
                comboBoxEnvironments.ValueMember = "Value";
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            var fileBrowserDialog = new FolderBrowserDialog();

            fileBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            var originalPath = textBoxMetadataPath.Text;
            fileBrowserDialog.SelectedPath = textBoxMetadataPath.Text;

            DialogResult result = fileBrowserDialog.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fileBrowserDialog.SelectedPath))
            {
                string[] files = Directory.GetFiles(fileBrowserDialog.SelectedPath);

                int fileCounter = 0;
                foreach (string file in files)
                {
                    if (file.EndsWith(".json"))
                    {
                        fileCounter++;
                    }
                }

                string finalPath;
                if (fileBrowserDialog.SelectedPath.EndsWith(@"\"))
                {
                    finalPath = fileBrowserDialog.SelectedPath;
                }
                else
                {
                    finalPath = fileBrowserDialog.SelectedPath + @"\";
                }


                textBoxMetadataPath.Text = finalPath;

                if (fileCounter == 0)
                {
                    richTextBoxInformationMain.Text = "There are no Json files in this location. Can you check if the selected directory contains Json files?";
                    textBoxMetadataPath.Text = originalPath;
                }
                else
                {
                    richTextBoxInformationMain.Text = "The path now points to a directory that contains Json files.";

                    // (Re)Create the tab pages based on available content.
                    VdwConfigurationSettings.VdwMetadatPath = finalPath;
                    CreateCustomTabPages();
                }

            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            var fileBrowserDialog = new FolderBrowserDialog();

            fileBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            fileBrowserDialog.SelectedPath = textBoxOutputPath.Text;

            DialogResult result = fileBrowserDialog.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fileBrowserDialog.SelectedPath))
            {
                string finalPath;

                if (fileBrowserDialog.SelectedPath.EndsWith(@"\"))
                {
                    finalPath = fileBrowserDialog.SelectedPath;
                }
                else
                {
                    finalPath = fileBrowserDialog.SelectedPath + @"\";
                }

                textBoxOutputPath.Text = finalPath;

                richTextBoxInformationMain.Text = "The code generation output will be saved at "+finalPath+".'";  

            }
        }

        private void openVDWConfigurationDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(VdwConfigurationSettings.TeamConfigurationPath);
            }
            catch (Exception exception)
            {
                richTextBoxInformationMain.Text = "An error has occurred while attempting to open the configuration directory. The error message is: " + exception.Message;
            }
        }

        private void openInputDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(textBoxMetadataPath.Text);
            }
            catch (Exception ex)
            {
                richTextBoxInformationMain.Text = "An error has occurred while attempting to open the input directory. The error message is: " + ex;
            }
        }

        private void pictureBoxUpdateLoadPatternPath_Click(object sender, EventArgs e)
        {
            var fileBrowserDialog = new FolderBrowserDialog();

            fileBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            fileBrowserDialog.SelectedPath = textBoxLoadPatternPath.Text;

            DialogResult result = fileBrowserDialog.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fileBrowserDialog.SelectedPath))
            {
                string[] files = Directory.GetFiles(fileBrowserDialog.SelectedPath);

                int fileCounter = 0;
                foreach (string file in files)
                {
                    if (file.Contains("loadPatternCollection"))
                    {
                        fileCounter++;
                    }
                }

                string finalPath;
                if (fileBrowserDialog.SelectedPath.EndsWith(@"\"))
                {
                    finalPath = fileBrowserDialog.SelectedPath;
                }
                else
                {
                    finalPath = fileBrowserDialog.SelectedPath + @"\";
                }


                textBoxLoadPatternPath.Text = finalPath;

                if (fileCounter == 0)
                {
                    richTextBoxInformationMain.Text = "The selected directory does not seem to contain a loadPatternCollection.json file. Did you select a correct Load Pattern directory?";
                }
                else
                {
                    richTextBoxInformationMain.Text = "The path now points to a directory that contains the loadPatternCollection.json Load Pattern Collection file.";
                }

                // Update the parameters in memory.
                VdwConfigurationSettings.LoadPatternPath = finalPath;
                textBoxLoadPatternPath.Text = finalPath;

                // Report back to the user.
                richTextBoxInformationMain.AppendText("\r\nThe path now points to a directory that contains load patterns. Please save this configuration to retain these settings.");
                VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"Load pattern path has been updated to {VdwConfigurationSettings.LoadPatternPath}."));

                // Reload the files into the grid
                VdwConfigurationSettings.patternList.Clear();
                VdwConfigurationSettings.patternList = LoadPatternFileHandling.LoadPatternCollection();
                PopulateLoadPatternCollectionDataGrid();
            }
        }

         private void openPatternCollectionFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var theDialog = new OpenFileDialog
            {
                Title = @"Open Load Pattern Collection File",
                Filter = @"Load Pattern Collection|*.json",
                InitialDirectory = VdwConfigurationSettings.LoadPatternPath
            };

            var ret = STAShowDialog(theDialog);

            if (ret == DialogResult.OK)
            {
                try
                {
                    var chosenFile = theDialog.FileName;

                    // Save the list to memory
                    VdwConfigurationSettings.patternList = JsonConvert.DeserializeObject<List<LoadPatternFileHandling>>(File.ReadAllText(chosenFile));

                    // ... and populate the data grid
                    PopulateLoadPatternCollectionDataGrid();

                    SetTextMain("The file " + chosenFile + " was loaded.\r\n");
                    _loadPatternGridView.AutoLayout();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error has been encountered! The reported error is: " + ex);
                }

                try
                {
                    // Quick fix, in the file again to commit changes to memory.
                    CreateCustomTabPages();
                }
                catch (Exception ex)
                {
                    richTextBoxInformationMain.AppendText(
                        "An issue was encountered when regenerating the UI (Tab Pages). The reported error is " + ex);
                }
            }
        }

        private void savePatternCollectionFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBoxInformationMain.Clear();

                var chosenFile = textBoxLoadPatternPath.Text + GlobalParameters.LoadPatternListFileName;

                DataTable gridDataTable = (DataTable)_bindingSourceLoadPatternCollection.DataSource;

                // Make sure the output is sorted.
                gridDataTable.DefaultView.Sort = $"[{LoadPatternGridColumns.LoadPatternName}] ASC";
                gridDataTable.TableName = "LoadPatternCollection";

                JArray outputFileArray = new JArray();
                foreach (DataRow singleRow in gridDataTable.DefaultView.ToTable().Rows)
                {
                    JObject individualRow = JObject.FromObject(new
                    {
                        loadPatternName = singleRow[0].ToString(),
                        loadPatternType = singleRow[1].ToString(),
                        loadPatternConnectionKey = singleRow[2].ToString(),
                        loadPatternOutputFilePattern = singleRow[3].ToString(),
                        loadPatternFilePath = singleRow[4].ToString(),
                        loadPatternNotes = singleRow[5].ToString()
                    });
                    outputFileArray.Add(individualRow);
                }

                string json = JsonConvert.SerializeObject(outputFileArray, Formatting.Indented);


                File.WriteAllText(chosenFile, json);

                SetTextMain("The file " + chosenFile + " was updated.\r\n");

                try
                {
                    // Quick fix, in the file again to commit changes to memory.
                    VdwConfigurationSettings.patternList =
                        JsonConvert.DeserializeObject<List<LoadPatternFileHandling>>(File.ReadAllText(chosenFile));
                    CreateCustomTabPages();
                }
                catch (Exception ex)
                {
                    richTextBoxInformationMain.AppendText(
                        "An issue was encountered when regenerating the UI (Tab Pages). The reported error is " + ex);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void openLoadPatternDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(VdwConfigurationSettings.LoadPatternPath);
            }
            catch (Exception ex)
            {
                richTextBoxInformationMain.Text = "An error has occurred while attempting to open the load pattern directory. The error message is: " + ex;
            }
        }

        private void displayEventLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (backgroundWorkerEventLog.IsBusy != true)
            {
                // create a new instance of the alert form
                _alertEventLog = new FormAlert();

                _alertEventLog.ShowLogButton(false);
                _alertEventLog.ShowCancelButton(false);
                _alertEventLog.ShowProgressBar(false);
                _alertEventLog.ShowProgressLabel(false);
                // event handler for the Cancel button in AlertForm
                _alertEventLog.Canceled += buttonCancelEventLogForm_Click;
                _alertEventLog.Show();
                // Start the asynchronous operation.

                backgroundWorkerEventLog.RunWorkerAsync();
            }
        }

        private void buttonCancelEventLogForm_Click(object sender, EventArgs e)
        {
            if (backgroundWorkerEventLog.WorkerSupportsCancellation)
            {
                // Cancel the asynchronous operation.
                backgroundWorkerEventLog.CancelAsync();
                // Close the AlertForm
                _alertEventLog.Close();
            }
        }

        private void backgroundWorkerEventLog_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            // Handle multi-threading
            if (worker != null && worker.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                backgroundWorkerEventLog.ReportProgress(0);

                _alertEventLog.SetTextLogging("Event Log.\r\n\r\n");

                try
                {
                    foreach (var individualEvent in VdwConfigurationSettings.VdwEventLog)
                    {
                        _alertEventLog.SetTextLogging($"{individualEvent.eventTime} - {(EventTypes)individualEvent.eventCode}: {individualEvent.eventDescription}\r\n");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An issue occurred creating the sample schemas. The error message is: " + ex, "An issue has occurred", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }


                backgroundWorkerEventLog.ReportProgress(100);
            }
        }

        private void backgroundWorkerEventLog_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _alertEventLog.Message = "In progress, please wait... " + e.ProgressPercentage + "%";
            _alertEventLog.ProgressValue = e.ProgressPercentage;
        }

        private void backgroundWorkerEventLog_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // Do nothing
            }
            else if (e.Error != null)
            {
                // Do nothing
            }
            else
            {
                // Do nothing
            }
        }

        /// <summary>
        /// Set the path to the TEAM configuration file and load it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureOpenTeamConfigurationFile_Click(object sender, EventArgs e)
        {
            var fileBrowserDialog = new FolderBrowserDialog();

            fileBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            fileBrowserDialog.SelectedPath = textBoxTeamConfigurationPath.Text;

            DialogResult result = fileBrowserDialog.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fileBrowserDialog.SelectedPath))
            {
                string[] files = Directory.GetFiles(fileBrowserDialog.SelectedPath);

                int fileCounter = 0;
                foreach (string file in files)
                {
                    if (Path.GetFileName(file).StartsWith(GlobalParameters.TeamConfigurationFileName))
                    {
                        fileCounter++;
                    }
                }

                if (fileCounter == 0)
                {
                    string userFeedback = "The selected directory does not seem to contain a TEAM configuration file (TEAM_configuration.txt).";
                    richTextBoxInformationMain.Text = userFeedback;
                    VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Warning, userFeedback));
                }
                else
                {
                    string finalPath;
                    if (fileBrowserDialog.SelectedPath.EndsWith(@"\"))
                    {
                        finalPath = fileBrowserDialog.SelectedPath;
                    }
                    else
                    {
                        finalPath = fileBrowserDialog.SelectedPath + @"\";
                    }

                    // Update the parameters in memory.
                    VdwConfigurationSettings.TeamConfigurationPath = finalPath;
                    textBoxTeamConfigurationPath.Text = finalPath;

                    // Report back to the user.
                    richTextBoxInformationMain.AppendText("\r\nThe path now points to a directory that contains TEAM configuration files. Please save this configuration to retain these settings.");
                    VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"TEAM configuration path updated to {VdwConfigurationSettings.TeamConfigurationPath}."));
                }
            }
        }

        /// <summary>
        /// Set a new path for the TEAM environments file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxOpenEnvironmentFile_Click(object sender, EventArgs e)
        {
            try
            {
                var fileBrowserDialog = new OpenFileDialog
                {
                    InitialDirectory = Path.GetDirectoryName(textBoxTeamEnvironmentsFilePath.Text)
                };

                DialogResult result = fileBrowserDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fileBrowserDialog.FileName))
                {
                    string[] files = Directory.GetFiles(Path.GetDirectoryName(fileBrowserDialog.FileName));

                    int teamFileCounter = 0;
                    foreach (string file in files)
                    {
                        if (file.Contains(GlobalParameters.JsonEnvironmentFileName))
                        {
                            teamFileCounter++;
                        }
                    }

                    if (teamFileCounter == 0)
                    {
                        string userFeedback = "The selected directory does not seem to contain a TEAM environments file (TEAM_environments.json)";
                        richTextBoxInformationMain.Text = userFeedback;
                        VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Warning, userFeedback));
                    }
                    else
                    {
                        richTextBoxInformationMain.Clear();

                        // Ensuring the path is set in memory also and reload the configuration
                        VdwConfigurationSettings.TeamEnvironmentFilePath = fileBrowserDialog.FileName;
                        textBoxTeamEnvironmentsFilePath.Text = fileBrowserDialog.FileName;

                        richTextBoxInformationMain.AppendText("\r\nThe path now points to a valid TEAM environment file. Please save this configuration to activate these settings.");
                        VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"TEAM environments file path updated to {VdwConfigurationSettings.TeamEnvironmentFilePath}."));

                        // Load the file.
                        TeamEnvironmentCollection.LoadTeamEnvironmentCollection(fileBrowserDialog.FileName);
                        VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"TEAM environments file {VdwConfigurationSettings.TeamEnvironmentFilePath} has been loaded to memory."));

                        // Reload the environments combobox
                        comboBoxEnvironments.Items.Clear();
                        PopulateEnvironmentComboBox();
                        comboBoxEnvironments.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception exception)
            {
                richTextBoxInformationMain.Text += $"An error was encountered: {exception.Message}";
            }
        }

        private void comboBoxEnvironments_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Retrieve the updated value from the Combobox.
            var selectedEnvironment = (KeyValuePair<string, TeamEnvironment>)comboBoxEnvironments.SelectedItem;

            VdwConfigurationSettings.TeamSelectedEnvironmentInternalId = selectedEnvironment.Value.environmentInternalId;
            VdwConfigurationSettings.ActiveEnvironment = selectedEnvironment.Value;

            if (startUpIndicator != true)
            {
                // Reload the configuration and connections file associated with this new environment.
                VdwUtility.LoadTeamConnectionsFileForVdw(VdwConfigurationSettings.ActiveEnvironment.environmentKey);
                VdwUtility.LoadTeamConfigurationFileForVdw(VdwConfigurationSettings.ActiveEnvironment.environmentKey);

                richTextBoxInformationMain.AppendText($"The '{VdwConfigurationSettings.ActiveEnvironment.environmentKey}' environment is now active.\r\n");
            }

            // Set any screen controls with the correct value.
            textBoxTeamConfigurationPath.Text = VdwConfigurationSettings.ActiveEnvironment.configurationPath;
            textBoxTeamConnectionsPath.Text = VdwConfigurationSettings.ActiveEnvironment.configurationPath;
            textBoxMetadataPath.Text = VdwConfigurationSettings.ActiveEnvironment.metadataPath;



        }

        /// <summary>
        /// Set the TEAM Connections File Path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxOpenConnectionFile_Click(object sender, EventArgs e)
        {
            var fileBrowserDialog = new FolderBrowserDialog();

            fileBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            fileBrowserDialog.SelectedPath = textBoxTeamConnectionsPath.Text;

            DialogResult result = fileBrowserDialog.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fileBrowserDialog.SelectedPath))
            {
                string[] files = Directory.GetFiles(fileBrowserDialog.SelectedPath);

                int fileCounter = 0;
                foreach (string file in files)
                {
                    if (Path.GetFileName(file).StartsWith(GlobalParameters.JsonConnectionFileName))
                    {
                        fileCounter++;
                    }
                }

                if (fileCounter == 0)
                {
                    string userFeedback = "The selected directory does not seem to contain a TEAM connections file (TEAM_connections.json).";
                    richTextBoxInformationMain.Text = userFeedback;
                    VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Warning, userFeedback));
                }
                else
                {
                    string finalPath;
                    if (fileBrowserDialog.SelectedPath.EndsWith(@"\"))
                    {
                        finalPath = fileBrowserDialog.SelectedPath;
                    }
                    else
                    {
                        finalPath = fileBrowserDialog.SelectedPath + @"\";
                    }


                    // Update the parameters in memory.
                    VdwConfigurationSettings.TeamConnectionsPath = finalPath;
                    textBoxTeamConnectionsPath.Text = finalPath;

                    // Report back to the user.
                    richTextBoxInformationMain.AppendText("\r\nThe path now points to a directory that contains TEAM connections files. Please save this configuration to retain these settings.");
                    VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"TEAM connections path updated to {VdwConfigurationSettings.TeamConnectionsPath}."));
                }
            }
        }

        private void tabControlMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab.Name != tabControlMain.TabPages[0].Name)
            {
                VdwConfigurationSettings.SelectedMainTab = tabControlMain.SelectedTab.Name;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var t = new Thread(ThreadProcAbout);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private FormAbout _myAboutForm;
        public void ThreadProcAbout()
        {
            if (_myAboutForm == null)
            {
                _myAboutForm = new FormAbout();
                _myAboutForm.Show();

                Application.Run();
            }

            else
            {
                if (_myAboutForm.InvokeRequired)
                {
                    // Thread Error
                    _myAboutForm.Invoke((MethodInvoker)delegate { _myAboutForm.Close(); });
                    _myAboutForm.FormClosed += CloseAboutForm;

                    _myAboutForm = new FormAbout();
                    _myAboutForm.Show();
                    Application.Run();
                }
                else
                {
                    // No invoke required - same thread
                    _myAboutForm.FormClosed += CloseAboutForm;

                    _myAboutForm = new FormAbout();
                    _myAboutForm.Show();
                    Application.Run();
                }
            }
        }

        private void CloseAboutForm(object sender, FormClosedEventArgs e)
        {
            _myAboutForm = null;
        }

        private void openCoreDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(GlobalParameters.CorePath);
            }
            catch (Exception exception)
            {
                richTextBoxInformationMain.Text = "An error has occurred while attempting to open the core directory. The error message is: " + exception.Message;
            }
        }
    }
}
