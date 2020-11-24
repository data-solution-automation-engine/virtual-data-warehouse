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
using TEAM;

namespace Virtual_Data_Warehouse
{
    public partial class FormMain : FormBase
    {
        internal bool startUpIndicator = true;

        private List<CustomTabPage> localCustomTabPageList = new List<CustomTabPage>();

        private BindingSource _bindingSourceLoadPatternCollection = new BindingSource();

        private DatabaseHandling databaseHandling;
        FormAlert _alertEventLog;

        public FormMain()
        {
            databaseHandling = new DatabaseHandling();

            localCustomTabPageList = new List<CustomTabPage>();

            InitializeComponent();

            // Set the version of the build for everything
            const string versionNumberForApplication = "v1.6.2";

            Text = $"Virtual Data Warehouse - {versionNumberForApplication}";
            VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"{Text}."));

            #region Root Paths
            // Make sure the root directories exist, based on (tool) parameters

            // Configuration Path
            var localEvent = FileHandling.InitialisePath(GlobalParameters.VdwConfigurationPath);
            if (localEvent.eventDescription != null)
            {
                VdwConfigurationSettings.VdwEventLog.Add(localEvent);
            }
            VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"Configuration path initialised for {GlobalParameters.VdwConfigurationPath}."));
            
            // Input Path
            localEvent = FileHandling.InitialisePath(VdwConfigurationSettings.VdwInputPath);
            if (localEvent.eventDescription != null)
            {
                VdwConfigurationSettings.VdwEventLog.Add(localEvent);
            }
            VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"Input path set for {VdwConfigurationSettings.VdwInputPath}."));

            // Output Path
            localEvent = FileHandling.InitialisePath(VdwConfigurationSettings.VdwOutputPath);
            if (localEvent.eventDescription != null)
            {
                VdwConfigurationSettings.VdwEventLog.Add(localEvent);
            }
            VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"Output path set for {VdwConfigurationSettings.VdwOutputPath}."));

            // Examples Path
            localEvent = FileHandling.InitialisePath(VdwConfigurationSettings.VdwExamplesPath);
            if (localEvent.eventDescription != null)
            {
                VdwConfigurationSettings.VdwEventLog.Add(localEvent);
            }
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
            textBoxInputPath.Text = VdwConfigurationSettings.VdwInputPath;
            textBoxSchemaName.Text = VdwConfigurationSettings.VdwSchema;

            // Then load the environments file and current working environment.
            // The TeamEnvironmentCollection contains all the environments as specified in TEAM (environments file).
            TeamEnvironmentCollection.LoadTeamEnvironmentCollection(VdwConfigurationSettings.TeamEnvironmentFilePath);

            VdwConfigurationSettings.ActiveEnvironment = TeamEnvironmentCollection.GetEnvironmentByKey(VdwConfigurationSettings.TeamSelectedEnvironmentInternalId);

            // Load the configuration and connection information from file, based on the selected environment and input path.
            VdwUtility.LoadTeamConnectionsFileForVdw(VdwConfigurationSettings.ActiveEnvironment.environmentKey);
            VdwUtility.LoadTeamConfigurationFileForVdw(VdwConfigurationSettings.ActiveEnvironment.environmentKey);

            PopulateEnvironmentComboBox();
            comboBoxEnvironments.SelectedIndex = comboBoxEnvironments.FindStringExact(VdwConfigurationSettings.TeamSelectedEnvironmentInternalId);

            var comboItem = comboBoxEnvironments.Items.Cast<KeyValuePair<string, TeamWorkingEnvironment>>().FirstOrDefault(item => item.Value.Equals(FormBase.VdwConfigurationSettings.ActiveEnvironment));
            comboBoxEnvironments.SelectedItem = comboItem;

            // Start monitoring the configuration directories for file changes
            // RunFileWatcher(); DISABLED FOR NOW - FIRES 2 EVENTS!!

            richTextBoxInformationMain.AppendText("Application initialised - welcome to the Virtual Data Warehouse! \r\n\r\n");

            checkBoxGenerateInDatabase.Checked = false;

            // Load Pattern metadata & update in memory
            VdwConfigurationSettings.patternList = LoadPatternCollectionFileHandling.DeserializeLoadPatternCollection();

            if ((VdwConfigurationSettings.patternList != null) && (!VdwConfigurationSettings.patternList.Any()))
            {
                SetTextMain("There are no patterns found in the designated load pattern directory. Please verify if there is a " + GlobalParameters.LoadPatternListFileName + " in the " + VdwConfigurationSettings.LoadPatternPath + " directory, and if the file contains patterns.");
            }

            // Populate the data grid.
            CreateLoadPatternCollectionDataGrid();
            PopulateLoadPatternCollectionDataGrid();
            GridAutoLayoutLoadPatternCollection();

            // Create the tab pages based on available content.
            CreateCustomTabPages();

            foreach (CustomTabPage localCustomTabPage in localCustomTabPageList)
            {
                localCustomTabPage.setDisplayJsonFlag(false);
                localCustomTabPage.setGenerateInDatabaseFlag(false);
                localCustomTabPage.setSaveOutputFileFlag(true);
            }

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
            DataTable dt = VdwConfigurationSettings.patternList.ToDataTable();

            // Accept changes
            dt.AcceptChanges();

            // Handle unknown combobox values, by setting them to empty.
            var localConnectionKeyList = LocalTeamConnection.TeamConnectionKeyList(TeamConfigurationSettings.ConnectionDictionary);
            List<string> userFeedbackList = new List<string>();
            foreach (DataRow row in dt.Rows)
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
            dt.AcceptChanges();

            // Tidy-up headers
            dt.Columns[0].ColumnName = "Name";
            dt.Columns[1].ColumnName = "Type";
            dt.Columns[2].ColumnName = "Connection Key";
            dt.Columns[3].ColumnName = "Path";
            dt.Columns[4].ColumnName = "Notes";

            _bindingSourceLoadPatternCollection.DataSource = dt;
            dataGridViewLoadPatternCollection.DataSource = _bindingSourceLoadPatternCollection;


        }

        /// <summary>
        /// Add the grid view for the load pattern collection to the form
        /// </summary>
        public void CreateLoadPatternCollectionDataGrid()
        {
            dataGridViewLoadPatternCollection.AutoGenerateColumns = false;
            dataGridViewLoadPatternCollection.ColumnHeadersVisible = true;
            dataGridViewLoadPatternCollection.EditMode = DataGridViewEditMode.EditOnEnter;

            DataGridViewTextBoxColumn loadPatternName = new DataGridViewTextBoxColumn
            {
                Name = "LoadPatternName",
                HeaderText = "Name",
                DataPropertyName = "Name"
            };
            dataGridViewLoadPatternCollection.Columns.Add(loadPatternName);

            DataGridViewTextBoxColumn loadPatternType = new DataGridViewTextBoxColumn
            {
                Name = "LoadPatternType",
                HeaderText = "Type",
                DataPropertyName = "Type"
            };
            dataGridViewLoadPatternCollection.Columns.Add(loadPatternType);

            DataGridViewComboBoxColumn loadPatternConnectionKey = new DataGridViewComboBoxColumn
            {
                Name = "LoadPatternConnectionKey",
                HeaderText = "Connection Key",
                DataPropertyName = "Connection Key",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DataSource = LocalTeamConnection.GetConnections(TeamConfigurationSettings.ConnectionDictionary),
                DisplayMember = "ConnectionKey",
                ValueMember = "ConnectionId",
                ValueType = typeof(string)
            };
            dataGridViewLoadPatternCollection.Columns.Add(loadPatternConnectionKey);

            DataGridViewTextBoxColumn loadPatternPath = new DataGridViewTextBoxColumn
            {
                Name = "LoadPatternPath",
                HeaderText = "Path",
                DataPropertyName = "Path"
            };
            dataGridViewLoadPatternCollection.Columns.Add(loadPatternPath);

            DataGridViewTextBoxColumn loadPatternNotes = new DataGridViewTextBoxColumn
            {
                Name = "LoadPatternNotes",
                HeaderText = "Notes",
                DataPropertyName = "Notes"
            };
            dataGridViewLoadPatternCollection.Columns.Add(loadPatternNotes);

            //// Ensure editing is committed straight away.
            //foreach (Binding item in dataGridViewLoadPatternCollection.DataBindings)
            //{
            //    item.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            //}

        }

        private void GridAutoLayoutLoadPatternCollection()
        {
            //dataGridViewLoadPatternCollection.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ////Set the auto-size based on (the contents of) all cells in each column.
            //for (var i = 0; i < dataGridViewLoadPatternCollection.Columns.Count - 1; i++)
            //{
            //    dataGridViewLoadPatternCollection.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //    int localWidth = dataGridViewLoadPatternCollection.Columns[i].Width;
            //}

            //// Choose one column to be used to fill out the grid
            //if (dataGridViewLoadPatternCollection.Columns.Count > 0)
            //{
            //    dataGridViewLoadPatternCollection.Columns[dataGridViewLoadPatternCollection.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //}

            //// Disable the auto size again (to enable manual resizing).
            //for (var i = 0; i < dataGridViewLoadPatternCollection.Columns.Count - 1; i++)
            //{
            //    int columnWidth = dataGridViewLoadPatternCollection.Columns[i].Width;
            //    dataGridViewLoadPatternCollection.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //    dataGridViewLoadPatternCollection.Columns[i].Width = columnWidth;
            //}
            dataGridViewLoadPatternCollection.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            //dataGridViewLoadPatternCollection.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewLoadPatternCollection.Columns[dataGridViewLoadPatternCollection.ColumnCount - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // Disable the auto size again (to enable manual resizing).
            for (var i = 0; i < dataGridViewLoadPatternCollection.Columns.Count - 1; i++)
            {

                dataGridViewLoadPatternCollection.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dataGridViewLoadPatternCollection.Columns[i].Width = dataGridViewLoadPatternCollection.Columns[i].GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, true);
            }
        }

        private void SetDatabaseConnections()
        {
            #region Database connections

            //var connOmd = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringOmd};
            //var connStg = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringStg};
            //var connPsa = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringHstg};

            //// Attempt to gracefully capture connection troubles
            //if (connOmd.ConnectionString != "Server=<>;Initial Catalog=<Metadata>;user id=sa;password=<>")
            //    try
            //    {
            //        connOmd.Open();
            //        connOmd.Close();
            //        // connOmd.Dispose();
            //    }
            //    catch
            //    {
            //        SetTextMain(
            //            "There was an issue establishing a database connection to the Metadata Repository Database. These are managed via the TEAM configuration files. The reported database connection string is '" +
            //            TeamConfigurationSettings.ConnectionStringOmd + "'.\r\n");
            //        return;
            //    }
            //else
            //{
            //    SetTextMain(
            //        "Metadata Repository Connection has not yet been defined yet. Please make sure TEAM is configured with the right connection details. \r\n");
            //    return;
            //}


            //if (connPsa.ConnectionString !=
            //    "Server=<>;Initial Catalog=<Persistent_Staging_Area>;user id = sa;password =<> ")
            //    try
            //    {
            //        connPsa.Open();
            //        connPsa.Close();
            //        //connPsa.Dispose();
            //    }
            //    catch
            //    {
            //        SetTextMain(
            //            "There was an issue establishing a database connection to the Persistent Staging Area database. These are managed via the TEAM configuration files. The reported database connection string is '" +
            //            TeamConfigurationSettings.ConnectionStringHstg + "'.\r\n");
            //        return;
            //    }
            //else
            //{
            //    SetTextMain(
            //        "The Persistent Staging Area connection has not yet been defined yet. Please make sure TEAM is configured with the right connection details. \r\n");
            //    return;
            //}


            //if (connStg.ConnectionString != "Server=<>;Initial Catalog=<Staging_Area>;user id = sa;password =<> ")
            //    try
            //    {
            //        connStg.Open();
            //        connStg.Close();
            //        //connStg.Dispose();
            //    }
            //    catch
            //    {
            //        SetTextMain(
            //            "There was an issue establishing a database connection to the Staging Area database. These are managed via the TEAM configuration files. The reported database connection string is '" +
            //            TeamConfigurationSettings.ConnectionStringStg + "'.\r\n");
            //        return;
            //    }
            //else
            //{
            //    SetTextMain(
            //        "The Staging Area connection has not yet been defined yet. Please make sure TEAM is configured with the right connection details. \r\n");
            //    return;
            //}

            #endregion


            // Use the database connections
            try
            {
                //connOmd.Open();
            }
            catch (Exception ex)
            {
                SetTextMain(
                    "An issue was encountered while populating the available metadata for the selected version. The error message is: " +
                    ex);
            }
            finally
            {
              //  connOmd.Close();
              //  connOmd.Dispose();
            }
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void RunFileWatcher()
        {
            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            //watcher.Path = (GlobalParameters.ConfigurationPath + GlobalParameters.ConfigfileName);

            watcher.Path = VdwConfigurationSettings.TeamEnvironmentFilePath;

            /* Watch for changes in LastAccess and LastWrite times, and
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            // Only watch text files.
            watcher.Filter = GlobalParameters.TeamConfigurationFileName;

            // Add event handlers.
            watcher.Changed += OnChanged;
            //  watcher.Created += new FileSystemEventHandler(OnChanged);
            //  watcher.Deleted += new FileSystemEventHandler(OnChanged);

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        // Define the event handlers.
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            MessageBox.Show("File changed");
        }


        private void openOutputDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(VdwConfigurationSettings.VdwOutputPath);
            }
            catch (Exception ex)
            {
                richTextBoxInformationMain.Text =
                    "An error has occured while attempting to open the output directory. The error message is: " + ex;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void CloseAboutForm(object sender, FormClosedEventArgs e)
        {
            _myAboutForm = null;
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Process.Start(ExtensionMethod.GetDefaultBrowserPath(),
                "http://roelantvos.com/blog/articles-and-white-papers/virtualisation-software/");
        }
        
        private FormAbout _myAboutForm;

        public void ThreadProcAbout()
        {
            if (_myAboutForm == null)
            {
                _myAboutForm = new FormAbout(this);
                _myAboutForm.Show();

                Application.Run();
            }

            else
            {
                if (_myAboutForm.InvokeRequired)
                {
                    // Thread Error
                    _myAboutForm.Invoke((MethodInvoker) delegate { _myAboutForm.Close(); });
                    _myAboutForm.FormClosed += CloseAboutForm;

                    _myAboutForm = new FormAbout(this);
                    _myAboutForm.Show();
                    Application.Run();
                }
                else
                {
                    // No invoke required - same thread
                    _myAboutForm.FormClosed += CloseAboutForm;

                    _myAboutForm = new FormAbout(this);
                    _myAboutForm.Show();
                    Application.Run();
                }

            }
        }

        #region Multi-threading delegates for text boxes

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
            // Make sure the paths contain a backslash
            if (textBoxTeamEnvironmentsFilePath.Text.EndsWith(@"\"))
            {
                textBoxTeamEnvironmentsFilePath.Text = textBoxTeamEnvironmentsFilePath.Text.Replace(@"\", "");
            }

            if (!textBoxTeamConfigurationPath.Text.EndsWith(@"\"))
            {
                textBoxTeamConfigurationPath.Text = textBoxTeamConfigurationPath.Text + @"\";
            }

            if (!textBoxInputPath.Text.EndsWith(@"\"))
            {
                textBoxInputPath.Text = textBoxInputPath.Text + @"\";
            }

            if (!textBoxOutputPath.Text.EndsWith(@"\"))
            {
                textBoxOutputPath.Text = textBoxOutputPath.Text + @"\";
            }

            if (!textBoxLoadPatternPath.Text.EndsWith(@"\"))
            {
                textBoxLoadPatternPath.Text = textBoxLoadPatternPath.Text + @"\";
            }

            new KeyValuePair< string, TeamWorkingEnvironment > ("", null);

            if (comboBoxEnvironments.SelectedItem != null)
            {
                var selectedEnvironment = (KeyValuePair<string, TeamWorkingEnvironment>)comboBoxEnvironments.SelectedItem;
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
            VdwConfigurationSettings.VdwInputPath = textBoxInputPath.Text;
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
            rootPathConfigurationFile.AppendLine("InputPath|" + VdwConfigurationSettings.VdwInputPath + "");
            rootPathConfigurationFile.AppendLine("OutputPath|" + VdwConfigurationSettings.VdwOutputPath + "");
            rootPathConfigurationFile.AppendLine("LoadPatternPath|" + VdwConfigurationSettings.LoadPatternPath + "");
            rootPathConfigurationFile.AppendLine("VdwSchema|" + VdwConfigurationSettings.VdwSchema + "");
            rootPathConfigurationFile.AppendLine("/* End of file */");

            // Save the VDW core settings file to disk
            using (var outfile = new StreamWriter(GlobalParameters.VdwConfigurationPath +
                                                  GlobalParameters.VdwConfigurationFileName))
            {
                outfile.Write(rootPathConfigurationFile.ToString());
                outfile.Close();
            }

            // Reload the VDW and TEAM settings, as the environment may have changed
            VdwUtility.LoadVdwConfigurationFile();
            //ApplyTeamConfigurationToMemory();

            // Reset / reload the checkbox lists
            SetDatabaseConnections();

            // Recreate the in-memory patterns (to make sure MetadataGeneration object details are also added).
            CreateCustomTabPages();

            richTextBoxInformationMain.Text = DateTime.Now+" - the global parameter file (" +
                                              GlobalParameters.VdwConfigurationFileName + ") has been updated in: " +
                                              GlobalParameters.VdwConfigurationPath+"\r\n\r\n";
        }

        private void openTEAMConfigurationSettingsFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(VdwConfigurationSettings.TeamEnvironmentFilePath);
            }
            catch (Exception ex)
            {
                richTextBoxInformationMain.Text =
                    "An error has occured while attempting to open the TEAM configuration file. The error message is: " +
                    ex;
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
                Process.Start(GlobalParameters.VdwConfigurationPath +
                              GlobalParameters.VdwConfigurationFileName);

            }
            catch (Exception ex)
            {
                richTextBoxInformationMain.Text =
                    "An error has occured while attempting to open the VDW configuration file. The error message is: " +
                    ex;
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
            richTextBoxInformationMain.AppendText(text + "\r\n");
        }

        /// <summary>
        /// Load all the metadata into a single list and associate with a pattern, based on the classification of the mapping (i.e. CoreBusinessConcept).
        /// </summary>
        /// <returns></returns>
        internal List<LocalPattern> PatternList()
        {
            #region Deserialisation

            // Deserialise the Json files into a local List of Data Object Mappings (mappingList) for further use.
            List<VDW_DataObjectMappingList> mappingList = new List<VDW_DataObjectMappingList>();

            if (Directory.Exists(VdwConfigurationSettings.VdwInputPath))
            {
                string[] fileEntries = Directory.GetFiles(VdwConfigurationSettings.VdwInputPath, "*.json");

                // Hard-coded exclusions
                string[] excludedFiles =
                {
                    "interfaceBusinessKeyComponent.json", "interfaceBusinessKeyComponentPart.json",
                    "interfaceDrivingKey.json", "interfaceHubLinkXref.json", "interfacePhysicalModel.json",
                    "interfaceSourceHubXref.json", "interfaceSourceLinkAttributeXref.json"
                };

                if (fileEntries.Length > 0)
                {
                    foreach (string fileName in fileEntries)
                    {
                        if (!Array.Exists(excludedFiles, x => x == Path.GetFileName(fileName)))
                        {
                            try
                            {
                                // Validate the file contents against the schema definition.
                                if (File.Exists(Application.StartupPath + @"\Schema\" + GlobalParameters
                                                    .JsonSchemaForDataWarehouseAutomationFileName))
                                {
                                    var result = DataWarehouseAutomation.JsonHandling.ValidateJsonFileAgainstSchema(
                                        Application.StartupPath + @"\Schema\" + GlobalParameters
                                            .JsonSchemaForDataWarehouseAutomationFileName, fileName);

                                    foreach (var error in result.Errors)
                                    {
                                        VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Error,
                                            $"An error was encountered validating the contents {fileName}.{error.Message}. This occurs at line {error.LineNumber}."));
                                    }
                                }
                                else
                                {
                                    VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Error,
                                        $"An error occurred while validating the file against the Data Warehouse Automation schema. Does the schema file exist?"));
                                }

                                // Add the deserialised file to the list of mappings.
                                VDW_DataObjectMappingList deserialisedMapping = new VDW_DataObjectMappingList();

                                var jsonInput = File.ReadAllText(fileName);
                                deserialisedMapping =
                                    JsonConvert.DeserializeObject<VDW_DataObjectMappingList>(jsonInput);
                                deserialisedMapping.metadataFileName = fileName;

                                mappingList.Add(deserialisedMapping);
                            }
                            catch
                            {
                                InformUser($"The file {fileName} could not be loaded properly.", EventTypes.Error);
                            }
                        }
                    }
                }
                else
                {
                    InformUser($"No files were detected in directory {VdwConfigurationSettings.VdwInputPath}.",
                        EventTypes.Warning);
                }
            }
            else
            {
                InformUser(
                    $"There were issues accessing the directory {VdwConfigurationSettings.VdwInputPath}. It does not seem to exist.",
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

            if (mappingList.Any() == true)
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
                    tabControlMain.Controls.Remove((customTabPage));
                }
            }

            List<LocalPattern> finalMappingList = PatternList();
            var sortedMappingList = finalMappingList.OrderBy(x => x.classification);

            // Add the Custom Tab Pages
            foreach (var pattern in sortedMappingList)
            {
                CustomTabPage localCustomTabPage = new CustomTabPage(pattern.classification, pattern.notes, pattern.itemList);
                localCustomTabPage.OnChangeMainText += UpdateMainInformationTextBox;
                localCustomTabPage.OnClearMainText += ClearMainInformationTextBox;

                localCustomTabPageList.Add(localCustomTabPage);
                tabControlMain.TabPages.Add(localCustomTabPage);
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
        }

        private void checkBoxGenerateInDatabase_CheckedChanged(object sender, EventArgs e)
        {
            foreach (CustomTabPage localTabPage in localCustomTabPageList)
            {
                if (checkBoxGenerateInDatabase.Checked)
                {
                    localTabPage.setGenerateInDatabaseFlag(true);
                }
                else
                {
                    localTabPage.setGenerateInDatabaseFlag(false);
                }
            }
        }

        private void checkBoxGenerateJsonSchema_CheckedChanged(object sender, EventArgs e)
        {
            foreach (CustomTabPage localTabPage in localCustomTabPageList)
            {
                if (checkBoxGenerateJsonSchema.Checked)
                {
                    localTabPage.setDisplayJsonFlag(true);
                }
                else
                {
                    localTabPage.setDisplayJsonFlag(false);
                }
            }
        }

        private void checkBoxSaveToFile_CheckedChanged(object sender, EventArgs e)
        {
            foreach (CustomTabPage localTabPage in localCustomTabPageList)
            {
                if (checkBoxSaveToFile.Checked)
                {
                    localTabPage.setSaveOutputFileFlag(true);
                }
                else
                {
                    localTabPage.setSaveOutputFileFlag(false);
                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            // Get the total of tab pages to create
            var patternList = PatternList();

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
                comboBoxEnvironments.Items.Add(new KeyValuePair<string, TeamWorkingEnvironment>(environment.Value.environmentKey, environment.Value));
                comboBoxEnvironments.DisplayMember = "Key";
                comboBoxEnvironments.ValueMember = "Value";
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            var fileBrowserDialog = new FolderBrowserDialog();

            var originalPath = textBoxInputPath.Text;
            fileBrowserDialog.SelectedPath = textBoxInputPath.Text;

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


                textBoxInputPath.Text = finalPath;

                if (fileCounter == 0)
                {
                    richTextBoxInformationMain.Text = "There are no Json files in this location. Can you check if the selected directory contains Json files?";
                    textBoxInputPath.Text = originalPath;
                }
                else
                {
                    richTextBoxInformationMain.Text = "The path now points to a directory that contains Json files.";

                    // (Re)Create the tab pages based on available content.
                    VdwConfigurationSettings.VdwInputPath = finalPath;
                    CreateCustomTabPages();
                }

            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            var fileBrowserDialog = new FolderBrowserDialog();
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


                    richTextBoxInformationMain.Text =
                        "The code generation output will be saved at "+finalPath+".'";
   

            }
        }

        private void openVDWConfigurationDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(GlobalParameters.VdwConfigurationPath);
            }
            catch (Exception ex)
            {
                richTextBoxInformationMain.Text =
                    "An error has occured while attempting to open the configuration directory. The error message is: " +
                    ex;
            }
        }

        private void openInputDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(VdwConfigurationSettings.VdwInputPath);
            }
            catch (Exception ex)
            {
                richTextBoxInformationMain.Text =
                    "An error has occured while attempting to open the input directory. The error message is: " +
                    ex;
            }
        }

        private void pictureBoxUpdateLoadPatternPath_Click(object sender, EventArgs e)
        {
            var fileBrowserDialog = new FolderBrowserDialog();
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
                    richTextBoxInformationMain.Text =
                        "The selected directory does not seem to contain a loadPatternCollection.json file. Did you select a correct Load Pattern directory?";
                }
                else
                {
                    richTextBoxInformationMain.Text = "The path now points to a directory that contains the loadPatternCollection.json Load Pattern Collection file.";
                }


                // Update the parameters in memory.
                VdwConfigurationSettings.LoadPatternPath = finalPath; ;
                textBoxLoadPatternPath.Text = finalPath;

                // Report back to the user.
                richTextBoxInformationMain.AppendText("\r\nThe path now points to a directory that contains load patterns. Please save this configuration to retain these settings.");
                VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"Load pattern path has been updated to {VdwConfigurationSettings.LoadPatternPath}."));

                // Reload the files into the grid
                VdwConfigurationSettings.patternList.Clear();
                VdwConfigurationSettings.patternList = LoadPatternCollectionFileHandling.DeserializeLoadPatternCollection();
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
                    VdwConfigurationSettings.patternList = JsonConvert.DeserializeObject<List<LoadPattern>>(File.ReadAllText(chosenFile));

                    // ... and populate the data grid
                    PopulateLoadPatternCollectionDataGrid();

                    SetTextMain("The file " + chosenFile + " was loaded.\r\n");
                    GridAutoLayoutLoadPatternCollection();
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

                // Make sure the output is sorted
                gridDataTable.DefaultView.Sort = "[Name] ASC";

                gridDataTable.TableName = "LoadPatternCollection";

                JArray outputFileArray = new JArray();
                foreach (DataRow singleRow in gridDataTable.DefaultView.ToTable().Rows)
                {
                    JObject individualRow = JObject.FromObject(new
                    {
                        loadPatternName = singleRow[0].ToString(),
                        loadPatternType = singleRow[1].ToString(),
                        loadPatternConnectionKey = singleRow[2].ToString(),
                        loadPatternFilePath = singleRow[3].ToString(),
                        loadPatternNotes = singleRow[4].ToString()
                    });
                    outputFileArray.Add(individualRow);
                }

                string json = JsonConvert.SerializeObject(outputFileArray, Formatting.Indented);

                // Create a backup file, if enabled
                if (checkBoxBackupFiles.Checked)
                {
                    try
                    {
                        var backupFile = new JsonHandling();
                        var targetFileName = backupFile.BackupJsonFile(chosenFile);
                        SetTextMain("A backup of the in-use JSON file was created as " + targetFileName + ".\r\n\r\n");
                    }
                    catch (Exception exception)
                    {
                        SetTextMain(
                            "An issue occured when trying to make a backup of the in-use JSON file. The error message was " +
                            exception + ".");
                    }
                }

                File.WriteAllText(chosenFile, json);

                SetTextMain("The file " + chosenFile + " was updated.\r\n");

                try
                {
                    // Quick fix, in the file again to commit changes to memory.
                    VdwConfigurationSettings.patternList =
                        JsonConvert.DeserializeObject<List<LoadPattern>>(File.ReadAllText(chosenFile));
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
                richTextBoxInformationMain.Text =
                    "An error has occured while attempting to open the load pattern directory. The error message is: " + ex;
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
                    MessageBox.Show("An issue occurred creating the sample schemas. The error message is: " + ex, "An issue has occured", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    VdwConfigurationSettings.TeamConfigurationPath = finalPath; ;
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

                    comboBoxEnvironments.Items.Clear();
                    PopulateEnvironmentComboBox();
                }
            }
        }

        private void comboBoxEnvironments_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Retrieve the updated value from the Combobox
            var selectedEnvironment = (KeyValuePair<string, TeamWorkingEnvironment>)comboBoxEnvironments.SelectedItem;
            VdwConfigurationSettings.TeamSelectedEnvironmentInternalId = selectedEnvironment.Value.environmentInternalId;
            VdwConfigurationSettings.ActiveEnvironment = selectedEnvironment.Value;

            if (startUpIndicator != true)
            {
                // Reload the configuration and connections file associated with this new environment.
                VdwUtility.LoadTeamConnectionsFileForVdw(VdwConfigurationSettings.ActiveEnvironment.environmentKey);
                VdwUtility.LoadTeamConfigurationFileForVdw(VdwConfigurationSettings.ActiveEnvironment.environmentKey);
                richTextBoxInformationMain.AppendText($"The {VdwConfigurationSettings.ActiveEnvironment.environmentKey} environment is now active (not saved yet though).\r\n");
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Specify that the link was visited. 
            linkLabelVdwGithub.LinkVisited = true;
            // Navigate to a URL.
            Process.Start("https://github.com/RoelantVos/Virtual-Data-Warehouse");
        }

        private void linkLabelWebLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Specify that the link was visited. 
            linkLabelWebLog.LinkVisited = true;
            // Navigate to a URL.
            Process.Start("http://www.roelantvos.com");
        }

        /// <summary>
        /// Set the TEAM Connections File Path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxOpenConnectionFile_Click(object sender, EventArgs e)
        {
            var fileBrowserDialog = new FolderBrowserDialog();
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
                    VdwConfigurationSettings.TeamConnectionsPath = finalPath; ;
                    textBoxTeamConnectionsPath.Text = finalPath;

                    // Report back to the user.
                    richTextBoxInformationMain.AppendText("\r\nThe path now points to a directory that contains TEAM connections files. Please save this configuration to retain these settings.");
                    VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"TEAM connections path updated to {VdwConfigurationSettings.TeamConnectionsPath}."));
                }
            }
        }

        /// <summary>
        /// Ensure changes, especially in the combobox are managed straight away and not require leaving the cell to commit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewLoadPatternCollection_CurrentCellDirtyStateChanged_1(object sender, EventArgs e)
        {
            if (dataGridViewLoadPatternCollection.IsCurrentCellDirty)
            {
                dataGridViewLoadPatternCollection.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
    }
}
