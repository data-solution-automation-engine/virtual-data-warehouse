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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Virtual_Data_Warehouse
{
    public partial class FormMain : FormBase
    {
        private StringBuilder _errorMessage;
        private StringBuilder _errorDetails;
        internal bool startUpIndicator = true;

        private List<CustomTabPage> localCustomTabPageList = new List<CustomTabPage>();

        private BindingSource _bindingSourceLoadPatternCollection = new BindingSource();
        private BindingSource _bindingSourceLoadPatternDefinition = new BindingSource();

        private DatabaseHandling databaseHandling;

        public FormMain()
        {
            databaseHandling = new DatabaseHandling();

            _errorMessage = new StringBuilder();
            _errorMessage.AppendLine("Error were detected:");
            _errorMessage.AppendLine();

            _errorDetails = new StringBuilder();
            _errorDetails.AppendLine();


            localCustomTabPageList = new List<CustomTabPage>();

            InitializeComponent();

            // Make sure the root directories exist, based on hard-coded (tool) parameters
            // Also creates the initial file with the configuration if it doesn't exist already
            EnvironmentConfiguration.InitialiseVedwRootPath();

            // Load the VEDW settings information, to be able to locate the TEAM configuration file and load it
            string loadVedwConfigurationResult = EnvironmentConfiguration.LoadVedwSettingsFile(
                GlobalParameters.VedwConfigurationPath +
                GlobalParameters.VedwConfigurationfileName +
                GlobalParameters.VedwFileExtension);

            richTextBoxInformationMain.AppendText(loadVedwConfigurationResult + "\r\n\r\n");

            // Load the TEAM configuration settings from the TEAM configuration directory
            LoadTeamConfigurationFile();

            // Make sure the retrieved variables are displayed on the form
            UpdateVedwConfigurationSettingsOnForm();

            // Start monitoring the configuration directories for file changes
            // RunFileWatcher(); DISABLED FOR NOW - FIRES 2 EVENTS!!

            richTextBoxInformationMain.AppendText(
                "Application initialised - welcome to the Virtual Data Warehouse! \r\n\r\n");

            checkBoxGenerateInDatabase.Checked = false;



            SetDatabaseConnections();

            var patternDefinition = new LoadPatternDefinitionFileHandling();
            VedwConfigurationSettings.patternDefinitionList = patternDefinition.DeserializeLoadPatternDefinition();

            // Load Pattern definition in memory
            if ((VedwConfigurationSettings.patternDefinitionList != null) &&
                (!VedwConfigurationSettings.patternDefinitionList.Any()))
            {
                SetTextMain(
                    "There are no pattern definitions / types found in the designated load pattern directory. Please verify if there is a " +
                    GlobalParameters.LoadPatternDefinitionFile + " in the " +
                    VedwConfigurationSettings.VedwInputPath +
                    " directory, and if the file contains pattern types.");
            }

            // Load Pattern metadata & update in memory
            var patternCollection = new LoadPatternCollectionFileHandling();
            VedwConfigurationSettings.patternList = patternCollection.DeserializeLoadPatternCollection();

            if ((VedwConfigurationSettings.patternList != null) && (!VedwConfigurationSettings.patternList.Any()))
            {
                SetTextMain(
                    "There are no patterns found in the designated load pattern directory. Please verify if there is a " +
                    GlobalParameters.LoadPatternListFile + " in the " + VedwConfigurationSettings.VedwInputPath +
                    " directory, and if the file contains patterns.");
            }





            // Populate the data grid
            populateLoadPatternCollectionDataGrid();
            populateLoadPatternDefinitionDataGrid();

            CreateCustomTabPages();

            foreach (CustomTabPage localCustomTabPage in localCustomTabPageList)
            {
                localCustomTabPage.setDisplayJsonFlag(false);
                localCustomTabPage.setGenerateInDatabaseFlag(false);
                localCustomTabPage.setSaveOutputFileFlag(true);
            }

            startUpIndicator = false;
        }

        public void populateLoadPatternCollectionDataGrid()
        {
            // Create a datatable 
            DataTable dt = VedwConfigurationSettings.patternList.ToDataTable();
      

            dt.AcceptChanges(); //Make sure the changes are seen as committed, so that changes can be detected later on
            dt.Columns[0].ColumnName = "Name";
            dt.Columns[1].ColumnName = "Type";
            dt.Columns[2].ColumnName = "Path";
            dt.Columns[3].ColumnName = "Notes";
            _bindingSourceLoadPatternCollection.DataSource = dt;

            if (VedwConfigurationSettings.patternList != null)
            {
                // Set the column header names.
                dataGridViewLoadPatternCollection.DataSource = _bindingSourceLoadPatternCollection;
                dataGridViewLoadPatternCollection.ColumnHeadersVisible = true;
                dataGridViewLoadPatternCollection.Columns[0].HeaderText = "Name";
                dataGridViewLoadPatternCollection.Columns[0].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.TopLeft;

                dataGridViewLoadPatternCollection.Columns[1].HeaderText = "Type";
                dataGridViewLoadPatternCollection.Columns[1].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.TopLeft;

                dataGridViewLoadPatternCollection.Columns[2].HeaderText = "Path";
                dataGridViewLoadPatternCollection.Columns[2].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.TopLeft;

                dataGridViewLoadPatternCollection.Columns[3].HeaderText = "Notes";
                dataGridViewLoadPatternCollection.Columns[3].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridViewLoadPatternCollection.Columns[3].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.TopLeft;

            }

            GridAutoLayoutLoadPatternCollection();
        }

        public void populateLoadPatternDefinitionDataGrid()
        {
            // Create a datatable 
            DataTable dt = VedwConfigurationSettings.patternDefinitionList.ToDataTable();

            dt.AcceptChanges(); //Make sure the changes are seen as committed, so that changes can be detected later on
            dt.Columns[0].ColumnName = "Key";
            dt.Columns[1].ColumnName = "Type";
            dt.Columns[2].ColumnName = "SelectionQuery";
            dt.Columns[3].ColumnName = "BaseQuery";
            dt.Columns[4].ColumnName = "AttributeQuery";
            dt.Columns[5].ColumnName = "AdditionalBusinessKeyQuery";
            dt.Columns[6].ColumnName = "Notes";
            dt.Columns[7].ColumnName = "ConnectionKey";

            _bindingSourceLoadPatternDefinition.DataSource = dt;

            if (VedwConfigurationSettings.patternList != null)
            {
                // Set the column header names.
                dataGridViewLoadPatternDefinition.DataSource = _bindingSourceLoadPatternDefinition;
                dataGridViewLoadPatternDefinition.ColumnHeadersVisible = true;
                dataGridViewLoadPatternDefinition.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dataGridViewLoadPatternDefinition.Columns[0].HeaderText = "Key";
                dataGridViewLoadPatternCollection.Columns[0].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.TopLeft;

                dataGridViewLoadPatternDefinition.Columns[1].HeaderText = "Type";
                dataGridViewLoadPatternCollection.Columns[1].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.TopLeft;

                dataGridViewLoadPatternDefinition.Columns[2].HeaderText = "Selection Query";
                dataGridViewLoadPatternDefinition.Columns[2].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridViewLoadPatternDefinition.Columns[2].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.TopLeft;

                dataGridViewLoadPatternDefinition.Columns[3].HeaderText = "Base Query";
                dataGridViewLoadPatternDefinition.Columns[3].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridViewLoadPatternDefinition.Columns[3].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.TopLeft;

                dataGridViewLoadPatternDefinition.Columns[4].HeaderText = "Attribute Query";
                dataGridViewLoadPatternDefinition.Columns[4].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridViewLoadPatternDefinition.Columns[4].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.TopLeft;

                dataGridViewLoadPatternDefinition.Columns[5].HeaderText = "Add. Business Key Query";
                dataGridViewLoadPatternDefinition.Columns[5].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridViewLoadPatternDefinition.Columns[5].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.TopLeft;

                dataGridViewLoadPatternDefinition.Columns[6].HeaderText = "Notes";
                dataGridViewLoadPatternDefinition.Columns[6].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridViewLoadPatternDefinition.Columns[6].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.TopLeft;

                dataGridViewLoadPatternDefinition.Columns[7].HeaderText = "ConnectionKey";
                dataGridViewLoadPatternDefinition.Columns[7].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridViewLoadPatternDefinition.Columns[7].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.TopLeft;
            }

            GridAutoLayoutLoadPatternDefinition();
        }

        private void GridAutoLayoutLoadPatternDefinition()
        {
            //Table Mapping metadata grid - set the auto size based on all cells for each column
            for (var i = 0; i < dataGridViewLoadPatternDefinition.Columns.Count - 1; i++)
            {
                dataGridViewLoadPatternDefinition.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            if (dataGridViewLoadPatternDefinition.Columns.Count > 0)
            {
                dataGridViewLoadPatternDefinition.Columns[dataGridViewLoadPatternDefinition.Columns.Count - 1]
                    .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            // Table Mapping metadata grid - disable the auto size again (to enable manual resizing)
            for (var i = 0; i < dataGridViewLoadPatternDefinition.Columns.Count - 1; i++)
            {
                int columnWidth = dataGridViewLoadPatternDefinition.Columns[i].Width;
                dataGridViewLoadPatternDefinition.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dataGridViewLoadPatternDefinition.Columns[i].Width = columnWidth;
            }
        }



        private void GridAutoLayoutLoadPatternCollection()
        {
            //Table Mapping metadata grid - set the autosize based on all cells for each column
            for (var i = 0; i < dataGridViewLoadPatternCollection.Columns.Count - 1; i++)
            {
                dataGridViewLoadPatternCollection.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            if (dataGridViewLoadPatternCollection.Columns.Count > 0)
            {
                dataGridViewLoadPatternCollection.Columns[dataGridViewLoadPatternCollection.Columns.Count - 1]
                    .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            // Table Mapping metadata grid - disable the auto size again (to enable manual resizing)
            for (var i = 0; i < dataGridViewLoadPatternCollection.Columns.Count - 1; i++)
            {
                int columnWidth = dataGridViewLoadPatternCollection.Columns[i].Width;
                dataGridViewLoadPatternCollection.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dataGridViewLoadPatternCollection.Columns[i].Width = columnWidth;
            }
        }

        private void SetDatabaseConnections()
        {
            #region Database connections

            var connOmd = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringOmd};
            var connStg = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringStg};
            var connPsa = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringHstg};

            // Attempt to gracefully capture connection troubles
            if (connOmd.ConnectionString != "Server=<>;Initial Catalog=<Metadata>;user id=sa;password=<>")
                try
                {
                    connOmd.Open();
                    connOmd.Close();
                    // connOmd.Dispose();
                }
                catch
                {
                    SetTextMain(
                        "There was an issue establishing a database connection to the Metadata Repository Database. These are managed via the TEAM configuration files. The reported database connection string is '" +
                        TeamConfigurationSettings.ConnectionStringOmd + "'.\r\n");
                    return;
                }
            else
            {
                SetTextMain(
                    "Metadata Repository Connection has not yet been defined yet. Please make sure TEAM is configured with the right connection details. \r\n");
                return;
            }


            if (connPsa.ConnectionString !=
                "Server=<>;Initial Catalog=<Persistent_Staging_Area>;user id = sa;password =<> ")
                try
                {
                    connPsa.Open();
                    connPsa.Close();
                    //connPsa.Dispose();
                }
                catch
                {
                    SetTextMain(
                        "There was an issue establishing a database connection to the Persistent Staging Area database. These are managed via the TEAM configuration files. The reported database connection string is '" +
                        TeamConfigurationSettings.ConnectionStringHstg + "'.\r\n");
                    return;
                }
            else
            {
                SetTextMain(
                    "The Persistent Staging Area connection has not yet been defined yet. Please make sure TEAM is configured with the right connection details. \r\n");
                return;
            }


            if (connStg.ConnectionString != "Server=<>;Initial Catalog=<Staging_Area>;user id = sa;password =<> ")
                try
                {
                    connStg.Open();
                    connStg.Close();
                    //connStg.Dispose();
                }
                catch
                {
                    SetTextMain(
                        "There was an issue establishing a database connection to the Staging Area database. These are managed via the TEAM configuration files. The reported database connection string is '" +
                        TeamConfigurationSettings.ConnectionStringStg + "'.\r\n");
                    return;
                }
            else
            {
                SetTextMain(
                    "The Staging Area connection has not yet been defined yet. Please make sure TEAM is configured with the right connection details. \r\n");
                return;
            }

            #endregion


            // Use the database connections
            try
            {
                connOmd.Open();

                InitialiseVersion();


            }
            catch (Exception ex)
            {
                SetTextMain(
                    "An issue was encountered while populating the available metadata for the selected version. The error message is: " +
                    ex);
            }
            finally
            {
                connOmd.Close();
                connOmd.Dispose();
            }




        }



        private void LoadTeamConfigurationFile()
        {
            // Load the rest of the (TEAM) configurations, from wherever they may be according to the VEDW settings (the TEAM configuration file)\
            var teamConfigurationFileName = VedwConfigurationSettings.TeamConfigurationPath +
                                            GlobalParameters.TeamConfigurationfileName + '_' +
                                            VedwConfigurationSettings.WorkingEnvironment +
                                            GlobalParameters.VedwFileExtension;

            richTextBoxInformationMain.AppendText("Retrieving TEAM configuration details from '" +
                                                  teamConfigurationFileName + "'. \r\n\r\n");

            var teamConfigResult = EnvironmentConfiguration.LoadTeamConfigurationFile(teamConfigurationFileName);

            if (teamConfigResult.Length > 0)
            {
                richTextBoxInformationMain.AppendText(
                    "Issues have been encountered while retrieving the TEAM configuration details. The following is returned: " +
                    teamConfigResult + "\r\n\r\n");
            }
        }

        /// <summary>
        /// This is the local updates on the VEDW specific configuration.
        /// </summary>
        private void UpdateVedwConfigurationSettingsOnForm()
        {
            textBoxOutputPath.Text = VedwConfigurationSettings.VedwOutputPath;
            textBoxConfigurationPath.Text = VedwConfigurationSettings.TeamConfigurationPath;
            textBoxLoadPatternPath.Text = VedwConfigurationSettings.VedwInputPath;
            textBoxSchemaName.Text = VedwConfigurationSettings.VedwSchema;


            // Environment radiobutton
            if (VedwConfigurationSettings.WorkingEnvironment == "Development")
            {
                radioButtonDevelopment.Checked = true;
            }
            else if (VedwConfigurationSettings.WorkingEnvironment == "Production")
            {
                radioButtonProduction.Checked = true;
            }
            else
            {
                richTextBoxInformationMain.AppendText(
                    "An issue was encountered updating the environment setting on the application - please verify.");
            }
        }


        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void RunFileWatcher()
        {
            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            //watcher.Path = (GlobalParameters.ConfigurationPath + GlobalParameters.ConfigfileName);

            watcher.Path = VedwConfigurationSettings.TeamConfigurationPath;

            /* Watch for changes in LastAccess and LastWrite times, and
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            // Only watch text files.
            watcher.Filter = GlobalParameters.TeamConfigurationfileName;

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

        private void InitialiseVersion()
        {
            //Initialise the versioning
            var selectedVersion = GetMaxVersionId();

            var versionMajorMinor = GetVersion(selectedVersion);
            var majorVersion = versionMajorMinor.Key;
            var minorVersion = versionMajorMinor.Value;

        }







        private void openOutputDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(VedwConfigurationSettings.VedwOutputPath);
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
        /// Save VEDW settings in the from to memory & disk
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveConfigurationFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Make sure the paths contain a backslash
            if (!textBoxOutputPath.Text.EndsWith(@"\"))
            {
                textBoxOutputPath.Text = textBoxOutputPath.Text + @"\";
            }

            if (!textBoxLoadPatternPath.Text.EndsWith(@"\"))
            {
                textBoxLoadPatternPath.Text = textBoxLoadPatternPath.Text + @"\";
            }

            if (!textBoxConfigurationPath.Text.EndsWith(@"\"))
            {
                textBoxConfigurationPath.Text = textBoxConfigurationPath.Text + @"\";
            }

            // Make the paths accessible from anywhere in the app (global parameters)
            VedwConfigurationSettings.TeamConfigurationPath = textBoxConfigurationPath.Text;
            VedwConfigurationSettings.VedwOutputPath = textBoxOutputPath.Text;
            VedwConfigurationSettings.VedwInputPath = textBoxLoadPatternPath.Text;
            VedwConfigurationSettings.VedwSchema = textBoxSchemaName.Text;

            // Working environment radiobutton
            if (radioButtonDevelopment.Checked)
            {
                VedwConfigurationSettings.WorkingEnvironment = "Development";
            }
            else if (!radioButtonDevelopment.Checked)
            {
                VedwConfigurationSettings.WorkingEnvironment = "Production";
            }
            else
            {
                richTextBoxInformationMain.AppendText(
                    "An issue was encountered saving the environment selection, can you verify the selection of the Development / Production radio button?");
            }

            // Update the root path file (from memory)
            var rootPathConfigurationFile = new StringBuilder();
            rootPathConfigurationFile.AppendLine("/* Virtual Data Warehouse Core Settings */");
            rootPathConfigurationFile.AppendLine("/* Saved at " + DateTime.Now + " */");
            rootPathConfigurationFile.AppendLine("TeamConfigurationPath|" +
                                                 VedwConfigurationSettings.TeamConfigurationPath + "");
            rootPathConfigurationFile.AppendLine("VedwOutputPath|" + VedwConfigurationSettings.VedwOutputPath + "");
            rootPathConfigurationFile.AppendLine("LoadPatternListPath|" + VedwConfigurationSettings.VedwInputPath + "");
            rootPathConfigurationFile.AppendLine("WorkingEnvironment|" + VedwConfigurationSettings.WorkingEnvironment + "");
            rootPathConfigurationFile.AppendLine("VedwSchema|" + VedwConfigurationSettings.VedwSchema + "");
            rootPathConfigurationFile.AppendLine("/* End of file */");

            // Save the VEDW core settings file to disk
            using (var outfile = new StreamWriter(GlobalParameters.VedwConfigurationPath +
                                                  GlobalParameters.VedwConfigurationfileName +
                                                  GlobalParameters.VedwFileExtension))
            {
                outfile.Write(rootPathConfigurationFile.ToString());
                outfile.Close();
            }

            // Reload the TEAM settings, as the environment may have changed
            LoadTeamConfigurationFile();

            // Reset / reload the checkbox lists
            SetDatabaseConnections();

            richTextBoxInformationMain.Text = "The global parameter file (" +
                                              GlobalParameters.VedwConfigurationfileName +
                                              GlobalParameters.VedwFileExtension + ") has been updated in: " +
                                              GlobalParameters.VedwConfigurationPath;
        }

        private void openConfigurationDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(VedwConfigurationSettings.TeamConfigurationPath);
            }
            catch (Exception ex)
            {
                richTextBoxInformationMain.Text =
                    "An error has occured while attempting to open the configuration directory. The error message is: " +
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
                Process.Start(VedwConfigurationSettings.TeamConfigurationPath +
                              GlobalParameters.TeamConfigurationfileName + '_' +
                              VedwConfigurationSettings.WorkingEnvironment +
                              GlobalParameters.VedwFileExtension);
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

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            GridAutoLayoutLoadPatternCollection();
            GridAutoLayoutLoadPatternDefinition();
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            GridAutoLayoutLoadPatternCollection();
            GridAutoLayoutLoadPatternDefinition();
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

        private void ButtonOpenLoadPatternList(object sender, EventArgs e)
        {
            var theDialog = new OpenFileDialog
            {
                Title = @"Open Load Pattern Definition File",
                Filter = @"Load Pattern Definition|*.json",
                InitialDirectory = VedwConfigurationSettings.VedwInputPath
            };

            var ret = STAShowDialog(theDialog);

            if (ret == DialogResult.OK)
            {
                try
                {
                    var chosenFile = theDialog.FileName;
                    var dataSet = new DataSet();

                    string fileExtension = Path.GetExtension(theDialog.FileName);

                    // Save the list to memory
                    VedwConfigurationSettings.patternDefinitionList =
                        JsonConvert.DeserializeObject<List<LoadPatternDefinition>>(File.ReadAllText(chosenFile));

                    // ... and populate the data grid
                    //LoadAllLoadPatternComboBoxes();
                    populateLoadPatternDefinitionDataGrid();

                    SetTextMain("The file " + chosenFile + " was loaded.\r\n");
                    GridAutoLayoutLoadPatternDefinition();
                }
                catch (Exception ex)
                {
                    richTextBoxInformationMain.AppendText("An error has been encountered! The reported error is: " +
                                                          ex);
                }

                try
                {
                    CreateCustomTabPages();
                }
                catch (Exception ex)
                {
                    richTextBoxInformationMain.AppendText(
                        "An issue was encountered when regenerating the UI (Tab Pages). The reported error is " + ex);
                }
            }
        }

        private void ButtonSaveLoadPatternList_Click(object sender, EventArgs e)
        {
            try
            {
                var theDialog = new SaveFileDialog
                {
                    Title = @"Save Load Pattern Definition File",
                    Filter = @"Load Pattern Definition|*.json",
                    InitialDirectory = textBoxLoadPatternPath.Text
                };

                var ret = STAShowDialog(theDialog);

                if (ret == DialogResult.OK)
                {
                    try
                    {
                        var chosenFile = theDialog.FileName;

                        DataTable gridDataTable = (DataTable) _bindingSourceLoadPatternDefinition.DataSource;

                        // Make sure the output is sorted
                        gridDataTable.DefaultView.Sort = "[KEY] ASC";

                        gridDataTable.TableName = "LoadPatternDefinition";

                        JArray outputFileArray = new JArray();
                        foreach (DataRow singleRow in gridDataTable.DefaultView.ToTable().Rows)
                        {
                            JObject individualRow = JObject.FromObject(new
                            {
                                loadPatternKey = singleRow[0].ToString(),
                                loadPatternType = singleRow[1].ToString(),
                                LoadPatternSelectionQuery = singleRow[2].ToString(),
                                loadPatternBaseQuery = singleRow[3].ToString(),
                                loadPatternAttributeQuery = singleRow[4].ToString(),
                                loadPatternAdditionalBusinessKeyQuery = singleRow[5].ToString(),
                                loadPatternNotes = singleRow[6].ToString(),
                                LoadPatternConnectionKey = singleRow[7].ToString()
                            });
                            outputFileArray.Add(individualRow);
                        }

                        string json = JsonConvert.SerializeObject(outputFileArray, Formatting.Indented);

                        File.WriteAllText(chosenFile, json);

                        SetTextMain("The file " + chosenFile + " was loaded.\r\n");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has been encountered! The reported error is: " + ex);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBoxInformationMain.Clear();

                var chosenFile = textBoxLoadPatternPath.Text + GlobalParameters.LoadPatternDefinitionFile;


                DataTable gridDataTable = (DataTable) _bindingSourceLoadPatternDefinition.DataSource;

                // Make sure the output is sorted
                gridDataTable.DefaultView.Sort = "[KEY] ASC";

                gridDataTable.TableName = "LoadPatternDefinition";

                JArray outputFileArray = new JArray();
                foreach (DataRow singleRow in gridDataTable.DefaultView.ToTable().Rows)
                {
                    JObject individualRow = JObject.FromObject(new
                    {
                        loadPatternKey = singleRow[0].ToString(),
                        loadPatternType = singleRow[1].ToString(),
                        LoadPatternSelectionQuery = singleRow[2].ToString(),
                        loadPatternBaseQuery = singleRow[3].ToString(),
                        loadPatternAttributeQuery = singleRow[4].ToString(),
                        loadPatternAdditionalBusinessKeyQuery = singleRow[5].ToString(),
                        loadPatternNotes = singleRow[6].ToString(),
                        LoadPatternConnectionKey = singleRow[7].ToString()
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
                    VedwConfigurationSettings.patternDefinitionList =
                        JsonConvert.DeserializeObject<List<LoadPatternDefinition>>(File.ReadAllText(chosenFile));
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

        private void openVEDWConfigurationSettingsFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(GlobalParameters.VedwConfigurationPath +
                              GlobalParameters.VedwConfigurationfileName +
                              GlobalParameters.VedwFileExtension);

            }
            catch (Exception ex)
            {
                richTextBoxInformationMain.Text =
                    "An error has occured while attempting to open the VEDW configuration file. The error message is: " +
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

        /// <summary>
        /// Generates the Custom Tab Pages using the pattern metadata. This method will remove any non-standard Tab Pages and create these using the Load Pattern Definition metadata.
        /// </summary>
        internal void CreateCustomTabPages()
        {
            // Remove any existing Custom Tab Pages
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


            if (Directory.Exists(VedwConfigurationSettings.VedwInputPath))
            {
                string[] fileEntries = Directory.GetFiles(VedwConfigurationSettings.VedwInputPath);
                foreach (string fileName in fileEntries)
                {
                    
                }
            }
            else
            {

            }

            // Add the Custom Tab Pages
            foreach (LoadPatternDefinition pattern in VedwConfigurationSettings.patternDefinitionList)
            {
                var conn = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringOmd};
                var inputItemList =
                    databaseHandling.GetItemList(pattern.LoadPatternType, pattern.LoadPatternSelectionQuery, conn);

                CustomTabPage localCustomTabPage = new CustomTabPage(pattern, inputItemList);
                localCustomTabPage.OnChangeMainText += new EventHandler<MyEventArgs>(UpdateMainInformationTextBox);
                localCustomTabPage.OnClearMainText += new EventHandler<MyClearArgs>(ClearMainInformationTextBox);

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

        private void buttonOpenLoadPatternCollection_Click(object sender, EventArgs e)
        {
            var theDialog = new OpenFileDialog
            {
                Title = @"Open Load Pattern Collection File",
                Filter = @"Load Pattern Collection|*.json",
                InitialDirectory = VedwConfigurationSettings.VedwInputPath
            };

            var ret = STAShowDialog(theDialog);

            if (ret == DialogResult.OK)
            {
                try
                {
                    var chosenFile = theDialog.FileName;
                    var dataSet = new DataSet();

                    string fileExtension = Path.GetExtension(theDialog.FileName);

                    // Save the list to memory
                    VedwConfigurationSettings.patternList =
                        JsonConvert.DeserializeObject<List<LoadPattern>>(File.ReadAllText(chosenFile));

                    // ... and populate the data grid
                    populateLoadPatternCollectionDataGrid();

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

        private void buttonSaveAsLoadPatternCollection_Click(object sender, EventArgs e)
        {
            try
            {
                var theDialog = new SaveFileDialog
                {
                    Title = @"Save Load Pattern Collection File",
                    Filter = @"Load Pattern Collection|*.json",
                    InitialDirectory = textBoxLoadPatternPath.Text
                };

                var ret = STAShowDialog(theDialog);

                if (ret == DialogResult.OK)
                {
                    try
                    {
                        var chosenFile = theDialog.FileName;

                        DataTable gridDataTable = (DataTable) _bindingSourceLoadPatternCollection.DataSource;

                        // Make sure the output is sorted
                        gridDataTable.DefaultView.Sort = "[NAME] ASC";

                        gridDataTable.TableName = "LoadPatternList";

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

                        File.WriteAllText(chosenFile, json);

                        SetTextMain("The file " + chosenFile + " was loaded.\r\n");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has been encountered! The reported error is: " + ex);
            }
        }

        private void buttonUpdateLoadPatternCollection_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBoxInformationMain.Clear();

                var chosenFile = textBoxLoadPatternPath.Text + GlobalParameters.LoadPatternListFile;

                DataTable gridDataTable = (DataTable) _bindingSourceLoadPatternCollection.DataSource;

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
                    VedwConfigurationSettings.patternList =
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

        private void button12_Click(object sender, EventArgs e)
        {
            // Get the name of the active tab so this can be refreshed
            string tabName = tabControlMain.SelectedTab.Name;

            //foreach (LoadPatternDefinition pattern in VedwConfigurationSettings.patternDefinitionList)
            //{
            //    if (pattern.LoadPatternType == tabName)
            //    {
            foreach (CustomTabPage customTabPage in localCustomTabPageList)
                //      foreach (LoadPatternDefinition in l)
            {

                if (customTabPage.Name == tabName)
                {
                    var conn = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringOmd};
                    var inputItemList = databaseHandling.GetItemList(customTabPage.input.LoadPatternType,
                        customTabPage.input.LoadPatternSelectionQuery, conn);
                    customTabPage.SetItemList(inputItemList);
                }
            }

            //}

            //}

        }

        private void tabControlSettings_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridAutoLayoutLoadPatternCollection();
            GridAutoLayoutLoadPatternDefinition();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            var fileBrowserDialog = new FolderBrowserDialog();
            fileBrowserDialog.SelectedPath = textBoxConfigurationPath.Text;

            DialogResult result = fileBrowserDialog.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fileBrowserDialog.SelectedPath))
            {
                string[] files = Directory.GetFiles(fileBrowserDialog.SelectedPath);

                int teamFileCounter = 0;
                foreach (string file in files)
                {
                    if (file.Contains("TEAM_configuration_"))
                    {
                        teamFileCounter++;
                    }
                }

                string finalPath = "";
                if (fileBrowserDialog.SelectedPath.EndsWith(@"\"))
                {
                    finalPath = fileBrowserDialog.SelectedPath;
                }
                else
                {
                    finalPath = fileBrowserDialog.SelectedPath + @"\";
                }


                textBoxConfigurationPath.Text = finalPath;

                if (teamFileCounter == 0)
                {
                    richTextBoxInformationMain.Text =
                        "The selected directory does not seem to contain TEAM configuration files. You are looking for files like TEAM_configuration_*.txt";
                }
                else
                {
                    richTextBoxInformationMain.Text =
                        "The path now points to a directory that contains TEAM configuration files.";
                }

            }

        }

        private void pictureBox4_Click(object sender, EventArgs e)
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

                string finalPath = "";
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
                    richTextBoxInformationMain.Text =
                        "The path now points to a directory that contains the loadPatternCollection.json Load Pattern Collection file.";
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

                string finalPath = "";
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
                Process.Start(GlobalParameters.VedwConfigurationPath);
            }
            catch (Exception ex)
            {
                richTextBoxInformationMain.Text =
                    "An error has occured while attempting to open the configuration directory. The error message is: " +
                    ex;
            }
        }
    }
}
