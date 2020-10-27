using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
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
            GlobalParameters.TeamEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"{Text}."));
            Event localEvent;

            #region Root Paths
            // Make sure the root directories exist, based on (tool) parameters

            // Configuration Path
            localEvent = FileHandling.InitialisePath(GlobalParameters.VdwConfigurationPath);
            if (localEvent.eventDescription != null)
            {
                GlobalParameters.TeamEventLog.Add(localEvent);
            }
            GlobalParameters.TeamEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"Configuration path initialised for {GlobalParameters.VdwConfigurationPath}."));
            
            
            // Input Path
            localEvent = FileHandling.InitialisePath(VdwConfigurationSettings.VdwInputPath);
            if (localEvent.eventDescription != null)
            {
                GlobalParameters.TeamEventLog.Add(localEvent);
            }
            GlobalParameters.TeamEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"Input path set for {VdwConfigurationSettings.VdwInputPath}."));

            // Output Path
            localEvent = FileHandling.InitialisePath(VdwConfigurationSettings.VdwOutputPath);
            if (localEvent.eventDescription != null)
            {
                GlobalParameters.TeamEventLog.Add(localEvent);
            }
            GlobalParameters.TeamEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"Output path set for {VdwConfigurationSettings.VdwOutputPath}."));
            #endregion

            // Create the initial VDW configuration file, if it doesn't exist (first time use or change of directory).
            VdwUtility.CreateNewVdwConfigurationFile();

            // Load the VDW configuration from disk, commit to memory and display on the form.
            VdwUtility.LoadVdwConfigurationFile();

            // Update the values on the form.
            textBoxOutputPath.Text = VdwConfigurationSettings.VdwOutputPath;
            textBoxLoadPatternPath.Text = VdwConfigurationSettings.LoadPatternPath;
            textBoxTeamEnvironmentsPath.Text = VdwConfigurationSettings.TeamEnvironmentFilePath;
            textBoxTeamConfigurationPath.Text = VdwConfigurationSettings.TeamConfigurationPath;
            textBoxInputPath.Text = VdwConfigurationSettings.VdwInputPath;
            textBoxSchemaName.Text = VdwConfigurationSettings.VdwSchema;

            // Then load the environments file and current working environment.
            TeamEnvironmentCollection.LoadTeamEnvironmentCollection(VdwConfigurationSettings.TeamEnvironmentFilePath);

            VdwConfigurationSettings.ActiveEnvironment = TeamEnvironmentCollection.GetEnvironmentByKey(VdwConfigurationSettings.TeamSelectedEnvironment);

            // Load the configuration and connection information from file, based on the selected environment and input path.
            VdwUtility.LoadTeamConfigurations(VdwConfigurationSettings.ActiveEnvironment.environmentKey);

            PopulateEnvironmentComboBox();
            comboBoxEnvironments.SelectedIndex = comboBoxEnvironments.FindStringExact(FormBase.VdwConfigurationSettings.TeamSelectedEnvironment);

            var comboItem = comboBoxEnvironments.Items.Cast<KeyValuePair<string, TeamWorkingEnvironment>>().FirstOrDefault(item => item.Value.Equals(FormBase.VdwConfigurationSettings.ActiveEnvironment));
            comboBoxEnvironments.SelectedItem = comboItem;

            // Start monitoring the configuration directories for file changes
            // RunFileWatcher(); DISABLED FOR NOW - FIRES 2 EVENTS!!

            richTextBoxInformationMain.AppendText("Application initialised - welcome to the Virtual Data Warehouse! \r\n\r\n");

            checkBoxGenerateInDatabase.Checked = false;

            // Load Pattern definition in memory
            if ((VdwConfigurationSettings.patternDefinitionList != null) && (!VdwConfigurationSettings.patternDefinitionList.Any()))
            {
                SetTextMain("There are no pattern definitions / types found in the designated load pattern directory. Please verify if there is a " + GlobalParameters.LoadPatternDefinitionFileName + " in the " + VdwConfigurationSettings.LoadPatternPath + " directory, and if the file contains pattern types.");
            }

            // Load Pattern metadata & update in memory
            var patternCollection = new LoadPatternCollectionFileHandling();
            VdwConfigurationSettings.patternList = patternCollection.DeserializeLoadPatternCollection();

            if ((VdwConfigurationSettings.patternList != null) && (!VdwConfigurationSettings.patternList.Any()))
            {
                SetTextMain("There are no patterns found in the designated load pattern directory. Please verify if there is a " + GlobalParameters.LoadPatternListFileName + " in the " + VdwConfigurationSettings.LoadPatternPath + " directory, and if the file contains patterns.");
            }

            // Populate the data grid.
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

        public void PopulateLoadPatternCollectionDataGrid()
        {
            // Create a datatable. 
            DataTable dt = VdwConfigurationSettings.patternList.ToDataTable();

            dt.AcceptChanges(); //Make sure the changes are seen as committed, so that changes can be detected later on
            dt.Columns[0].ColumnName = "Name";
            dt.Columns[1].ColumnName = "Type";
            dt.Columns[2].ColumnName = "Path";
            dt.Columns[3].ColumnName = "Notes";
            _bindingSourceLoadPatternCollection.DataSource = dt;

            if (VdwConfigurationSettings.patternList != null)
            {
                
                // Set the column header names.
                dataGridViewLoadPatternCollection.DataSource = _bindingSourceLoadPatternCollection;
                dataGridViewLoadPatternCollection.ColumnHeadersVisible = true;
                dataGridViewLoadPatternCollection.Columns[0].HeaderText = "Name";
                dataGridViewLoadPatternCollection.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
                //dataGridViewLoadPatternCollection.Columns[0].FillWeight = 100;

                dataGridViewLoadPatternCollection.Columns[1].HeaderText = "Type";
                dataGridViewLoadPatternCollection.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
                //dataGridViewLoadPatternCollection.Columns[1].FillWeight = 400;

                dataGridViewLoadPatternCollection.Columns[2].HeaderText = "Path";
                dataGridViewLoadPatternCollection.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
                //dataGridViewLoadPatternCollection.Columns[2].FillWeight = 300;

                dataGridViewLoadPatternCollection.Columns[3].HeaderText = "Notes";
                dataGridViewLoadPatternCollection.Columns[3].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridViewLoadPatternCollection.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
                //dataGridViewLoadPatternCollection.Columns[3].FillWeight = 200;

            }
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





        private void CloseTestRiForm(object sender, FormClosedEventArgs e)
        {
            _myTestRiForm = null;
        }

        private void CloseTestDataForm(object sender, FormClosedEventArgs e)
        {
            _myTestDataForm = null;
        }

        private void CloseAboutForm(object sender, FormClosedEventArgs e)
        {
            _myAboutForm = null;
        }

        private void ClosePitForm(object sender, FormClosedEventArgs e)
        {
            _myPitForm = null;
        }

        private void pointInTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var t = new Thread(ThreadProcPit);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Process.Start(ExtensionMethod.GetDefaultBrowserPath(),
                "http://roelantvos.com/blog/articles-and-white-papers/virtualisation-software/");
        }

   

        // Threads starting for other (sub) forms
        private FormTestRi _myTestRiForm;

        public void ThreadProcTestRi()
        {
            if (_myTestRiForm == null)
            {
                _myTestRiForm = new FormTestRi(this);
                _myTestRiForm.Show();

                Application.Run();
            }

            else
            {
                if (_myTestRiForm.InvokeRequired)
                {
                    // Thread Error
                    _myTestRiForm.Invoke((MethodInvoker) delegate { _myTestRiForm.Close(); });
                    _myTestRiForm.FormClosed += CloseTestRiForm;

                    _myTestRiForm = new FormTestRi(this);
                    _myTestRiForm.Show();
                    Application.Run();
                }
                else
                {
                    // No invoke required - same thread
                    _myTestRiForm.FormClosed += CloseTestRiForm;

                    _myTestRiForm = new FormTestRi(this);
                    _myTestRiForm.Show();
                    Application.Run();
                }
            }
        }

        private FormTestData _myTestDataForm;

        public void ThreadProcTestData()
        {
            if (_myTestDataForm == null)
            {
                _myTestDataForm = new FormTestData(this);
                _myTestDataForm.Show();

                Application.Run();
            }

            else
            {
                if (_myTestDataForm.InvokeRequired)
                {
                    // Thread Error
                    _myTestDataForm.Invoke((MethodInvoker) delegate { _myTestDataForm.Close(); });
                    _myTestDataForm.FormClosed += CloseTestDataForm;

                    _myTestDataForm = new FormTestData(this);
                    _myTestDataForm.Show();
                    Application.Run();
                }
                else
                {
                    // No invoke required - same thread
                    _myTestDataForm.FormClosed += CloseTestDataForm;

                    _myTestDataForm = new FormTestData(this);
                    _myTestDataForm.Show();
                    Application.Run();
                }

            }
        }




        private FormPit _myPitForm;

        public void ThreadProcPit()
        {
            if (_myPitForm == null)
            {
                _myPitForm = new FormPit(this);
                _myPitForm.Show();

                Application.Run();
            }

            else
            {
                if (_myPitForm.InvokeRequired)
                {
                    // Thread Error
                    _myPitForm.Invoke((MethodInvoker) delegate { _myPitForm.Close(); });
                    _myPitForm.FormClosed += ClosePitForm;

                    _myPitForm = new FormPit(this);
                    _myPitForm.Show();
                    Application.Run();
                }
                else
                {
                    // No invoke required - same thread
                    _myPitForm.FormClosed += ClosePitForm;

                    _myPitForm = new FormPit(this);
                    _myPitForm.Show();
                    Application.Run();
                }
            }
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


        private void generateTestDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var t = new Thread(ThreadProcTestData);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void generateRIValidationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var t = new Thread(ThreadProcTestRi);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
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
            if (textBoxTeamEnvironmentsPath.Text.EndsWith(@"\"))
            {
                textBoxTeamEnvironmentsPath.Text = textBoxTeamEnvironmentsPath.Text.Replace(@"\", "");
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
                VdwConfigurationSettings.TeamSelectedEnvironment = selectedEnvironment.Value.environmentInternalId;
            }
            else
            {
                VdwConfigurationSettings.TeamSelectedEnvironment = null;
            }

            // Make sure that the updated paths are accessible from anywhere in the app (global parameters)
            VdwConfigurationSettings.TeamEnvironmentFilePath = textBoxTeamEnvironmentsPath.Text;
            VdwConfigurationSettings.TeamConfigurationPath = textBoxTeamConfigurationPath.Text;
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
            rootPathConfigurationFile.AppendLine("TeamSelectedEnvironment|" + VdwConfigurationSettings.TeamSelectedEnvironment + "");
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

            richTextBoxInformationMain.Text = DateTime.Now+" - the global parameter file (" +
                                              GlobalParameters.VdwConfigurationFileName + ") has been updated in: " +
                                              GlobalParameters.VdwConfigurationPath+"\r\n\r\n";
        }

        private void openConfigurationDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Path.GetDirectoryName(VdwConfigurationSettings.TeamEnvironmentFilePath));
            }
            catch (Exception ex)
            {
                richTextBoxInformationMain.Text =
                    DateTime.Now + "- an error has occured while attempting to open the configuration directory. The error message is: " +
                    ex;
            }
        }

        private void openTEAMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("Team.exe");
            }
            catch (Exception)
            {
                MessageBox.Show("The TEAM application cannot be found. Is it installed?");
            }

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
            internal int id { get; set; }
            internal string classification { get; set; }
            internal string notes { get; set; }
            internal Dictionary<string,VDW_DataObjectMappingList> itemList { get; set; }
            internal string connectionString { get; set; }
        }

        internal List<LocalPattern> Patternlist()
        {
            // Deserialise the Json files for further use
            List<VDW_DataObjectMappingList> mappingList = new List<VDW_DataObjectMappingList>();

            if (Directory.Exists(VdwConfigurationSettings.VdwInputPath))
            {
                string[] fileEntries = Directory.GetFiles(VdwConfigurationSettings.VdwInputPath, "*.json");

                // Hard-coded exclusions
                string[] excludedfiles = {"interfaceBusinessKeyComponent.json", "interfaceBusinessKeyComponentPart.json", "interfaceDrivingKey.json", "interfaceHubLinkXref.json", "interfacePhysicalModel.json", "interfaceSourceHubXref.json", "interfaceSourceLinkAttributeXref.json" };

                foreach (string fileName in fileEntries)
                {
                    if (!Array.Exists(excludedfiles, x => x == Path.GetFileName(fileName)))
                    {
                        try
                        {
                            var jsonInput = File.ReadAllText(fileName);
                            VDW_DataObjectMappingList deserialisedMapping =
                                JsonConvert.DeserializeObject<VDW_DataObjectMappingList>(jsonInput);

                            mappingList.Add(deserialisedMapping);
                        }
                        catch
                        {
                            richTextBoxInformationMain.AppendText($"The file {fileName} could not be loaded properly.");
                        }
                    }
                }
            }
            else
            {
                richTextBoxInformationMain.AppendText("There were issues accessing the directory.");
            }

            // Create base list of classification / types to become the tab pages (based on the classification + notes field)
            Dictionary<string, string> classificationDictionary = new Dictionary<string, string>();

            foreach (VDW_DataObjectMappingList dataObjectMappingList in mappingList)
            {
                foreach (DataObjectMapping dataObjectMapping in dataObjectMappingList.dataObjectMappingList)
                {
                    foreach (Classification classification in dataObjectMapping.mappingClassification)
                    {
                        if (!classificationDictionary.ContainsKey(classification.classification))
                        {
                            classificationDictionary.Add(classification.classification, classification.notes);
                        }
                    }
                }
            }

            // Now use the base list of classifications / tab pages to add the item list (individual mappings) by searching the VDW_DataObjectMappingList
            List<LocalPattern> finalMappingList = new List<LocalPattern>();

            foreach (KeyValuePair<string, string> classification in classificationDictionary)
            {
                int localclassification = 0;
                string localConnectionString = "";

                LocalPattern localPatternMapping = new LocalPattern();
                Dictionary<string, VDW_DataObjectMappingList> itemList = new Dictionary<string, VDW_DataObjectMappingList>();

                // Iterate through the various levels to find the classification
                foreach (VDW_DataObjectMappingList dataObjectMappingList in mappingList)
                {
                    foreach (DataObjectMapping dataObjectMapping in dataObjectMappingList.dataObjectMappingList)
                    {
                        foreach (Classification dataObjectMappingClassification in dataObjectMapping.mappingClassification)
                        {
                            if (dataObjectMappingClassification.classification == classification.Key)
                            {
                                localclassification = dataObjectMappingClassification.id;
                                localConnectionString = dataObjectMapping.targetDataObject.dataObjectConnection.dataConnectionString;

                                if (!itemList.ContainsKey(dataObjectMapping.mappingName))
                                {
                                    itemList.Add(dataObjectMapping.mappingName, dataObjectMappingList);
                                }
                            }
                        }
                    }
                }

                localPatternMapping.id = localclassification;
                localPatternMapping.classification = classification.Key;
                localPatternMapping.notes = classification.Value;
                localPatternMapping.itemList = itemList;
                localPatternMapping.connectionString = localConnectionString;

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

            List<LocalPattern> finalMappingList = Patternlist();
            var sortedMappingList = finalMappingList.OrderBy(x => x.id);

            // Add the Custom Tab Pages
            foreach (var pattern in sortedMappingList)
            {
                CustomTabPage localCustomTabPage = new CustomTabPage(pattern.classification, pattern.notes, pattern.itemList, pattern.connectionString);
                localCustomTabPage.OnChangeMainText += UpdateMainInformationTextBox;
                localCustomTabPage.OnClearMainText += ClearMainInformationTextBox;

                localCustomTabPageList.Add(localCustomTabPage);
                tabControlMain.TabPages.Add(localCustomTabPage);
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
            var patternList = Patternlist();

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

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            var fileBrowserDialog = new OpenFileDialog
            {
                InitialDirectory = Path.GetDirectoryName(textBoxTeamEnvironmentsPath.Text)
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
                    string userFeedback = "The selected directory does not seem to contain a TEAM environments file. You are looking for the file TEAM_environments.json";
                    richTextBoxInformationMain.Text = userFeedback;
                    GlobalParameters.TeamEventLog.Add(Event.CreateNewEvent(EventTypes.Warning, userFeedback));
                }
                else
                {
                    richTextBoxInformationMain.Clear();

                    // Ensuring the path is set in memory also and reload the configuration
                    FormBase.VdwConfigurationSettings.TeamEnvironmentFilePath = fileBrowserDialog.FileName;
                    textBoxTeamEnvironmentsPath.Text = fileBrowserDialog.FileName;

                    richTextBoxInformationMain.AppendText("\r\nThe path now points to a valid TEAM environment file. Please save this configuration to activate these settings.");
                    FormBase.GlobalParameters.TeamEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"TEAM environments file path updated to {FormBase.VdwConfigurationSettings.TeamEnvironmentFilePath}."));

                    // Load the file.
                    FormBase.TeamEnvironmentCollection.LoadTeamEnvironmentCollection(fileBrowserDialog.FileName);
                    FormBase.GlobalParameters.TeamEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"TEAM environments file {FormBase.VdwConfigurationSettings.TeamEnvironmentFilePath} has been loaded to memory."));

                    comboBoxEnvironments.Items.Clear();
                    PopulateEnvironmentComboBox();
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

        private void pictureBox6_Click(object sender, EventArgs e)
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
                gridDataTable.DefaultView.Sort = "[NAME] ASC";

                gridDataTable.TableName = "LoadPatternCollection";

                JArray outputFileArray = new JArray();
                foreach (DataRow singleRow in gridDataTable.DefaultView.ToTable().Rows)
                {
                    JObject individualRow = JObject.FromObject(new
                    {
                        loadPatternName = singleRow[0].ToString(),
                        loadPatternType = singleRow[1].ToString(),
                        loadPatternFilePath = singleRow[2].ToString(),
                        loadPatternNotes = singleRow[3].ToString()
                    });
                    outputFileArray.Add(individualRow);
                }

                string json = JsonConvert.SerializeObject(outputFileArray, Formatting.Indented);

                // Create a backup file, if enabled
                if (checkBoxBackupFiles.Checked)
                {
                    try
                    {
                        var backupFile = new ClassJsonHandling();
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
                    foreach (var individualEvent in GlobalParameters.TeamEventLog)
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

        private void pictureBox10_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            var fileBrowserDialog = new FolderBrowserDialog();

            var originalPath = textBoxTeamConfigurationPath.Text;
            fileBrowserDialog.SelectedPath = textBoxTeamConfigurationPath.Text;

            DialogResult result = fileBrowserDialog.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fileBrowserDialog.SelectedPath))
            {
                string[] files = Directory.GetFiles(fileBrowserDialog.SelectedPath);

                int fileCounter = 0;
                foreach (string file in files)
                {
                    if (Path.GetFileName(file).StartsWith("TEAM_con"))
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
                    richTextBoxInformationMain.Text = "There are no TEAM configuration files in this location. Can you check if the selected directory contains TEAM files?";
                    textBoxInputPath.Text = originalPath;
                }
                else
                {
                    richTextBoxInformationMain.Text = "The path now points to a directory that contains TEAM configuration files.\r\n";

                    // (Re)Create the tab pages based on available content.
                    VdwConfigurationSettings.TeamConfigurationPath = finalPath; 
                    CreateCustomTabPages();
                }

            }
        }
    }
}
