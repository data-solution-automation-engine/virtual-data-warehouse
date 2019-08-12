using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Threading;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using HandlebarsDotNet;
using Newtonsoft.Json;
using Virtual_Data_Warehouse.Classes;

namespace Virtual_EDW
{
    public partial class FormMain : FormBase
    {
        private StringBuilder _errorMessage;
        private StringBuilder _errorDetails;
        private int _errorCounter;
        internal bool startUpIndicator = true;

        public FormMain()
        {
            _errorMessage = new StringBuilder();
            _errorMessage.AppendLine("Error were detected:");
            _errorMessage.AppendLine();
           
            _errorDetails = new StringBuilder();
            _errorDetails.AppendLine();
            _errorCounter = 0;

            InitializeComponent();

            // Make sure the root directories exist, based on hard-coded (tool) parameters
            // Also creates the initial file with the configuration if it doesn't exist already
            EnvironmentConfiguration.InitialiseVedwRootPath();

            // Load the VEDW settings information, to be able to locate the TEAM configuration file and load it
            EnvironmentConfiguration.LoadVedwSettingsFile(GlobalParameters.VedwConfigurationPath +
                                                          GlobalParameters.VedwConfigurationfileName +
                                                          GlobalParameters.VedwFileExtension);

            // Load the TEAM configuration settings from the TEAM configuration directory
            LoadTeamConfigurationFile();

            // Make sure the retrieved variables are displayed on the form
            UpdateVedwConfigurationSettingsOnForm();
            UpdateHashSnippets();

            // Start monitoring the configuration directories for file changes
            // RunFileWatcher(); DISABLED FOR NOW - FIRES 2 EVENTS!!

            richTextBoxInformationMain.AppendText("Application initialised - welcome to the Virtual Data Warehouse! \r\n\r\n");

            checkBoxGenerateInDatabase.Checked = false;
            //checkBoxIfExistsStatement.Checked = true;
            //radiobuttonViews.Checked = true;
            checkBoxDisableSatZeroRecords.Checked = false;
            checkBoxDisableLsatZeroRecords.Checked = false;

            InitialiseDocumentation();

            SetDatabaseConnections();

            // Set all the checkboxes to 'yes'
            CheckAllStgValues();
            CheckAllPsaValues();
            CheckAllHubValues();
            CheckAllSatValues();
            CheckAllLinkValues();
            CheckAllLsatValues();

            // Load the patterns into the tool based on the available list
            LoadStgPatternCombobox();
            LoadPsaPatternCombobox();
            LoadHubPatternCombobox();
            LoadSatPatternCombobox();
            LoadLinkPatternCombobox();
            LoadLsatPatternCombobox();
            startUpIndicator = false;
        }

        private void SetDatabaseConnections()
        {
            // Make sure nothing is checked in the various CheckboxLists
            ClearStgCheckBoxList();
            ClearPsaCheckBoxList();
            ClearHubCheckBoxList();
            ClearLinkCheckBoxList();
            ClearSatCheckBoxList();
            ClearLsatCheckBoxList();

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
                    SetTextMain("There was an issue establishing a database connection to the Metadata Repository Database. These are managed via the TEAM configuration files. The reported database connection string is '" +
                                 TeamConfigurationSettings.ConnectionStringOmd + "'.\r\n");
                    return;
                }
            else
            {
                SetTextMain("Metadata Repository Connection has not yet been defined yet. Please make sure TEAM is configured with the right connection details. \r\n");
                return;
            }


            if (connPsa.ConnectionString != "Server=<>;Initial Catalog=<Persistent_Staging_Area>;user id = sa;password =<> ")
                try
                {
                    connPsa.Open();
                    connPsa.Close();
                    //connPsa.Dispose();
                }
                catch
                {
                    SetTextMain("There was an issue establishing a database connection to the Persistent Staging Area database. These are managed via the TEAM configuration files. The reported database connection string is '" +
                        TeamConfigurationSettings.ConnectionStringHstg + "'.\r\n");
                    return;
                }
            else
            {
                SetTextMain("The Persistent Staging Area connection has not yet been defined yet. Please make sure TEAM is configured with the right connection details. \r\n");
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
                    SetTextMain("There was an issue establishing a database connection to the Staging Area database. These are managed via the TEAM configuration files. The reported database connection string is '" +
                                 TeamConfigurationSettings.ConnectionStringStg + "'.\r\n");
                    return;
                }
            else
            {
                SetTextMain("The Staging Area connection has not yet been defined yet. Please make sure TEAM is configured with the right connection details. \r\n");
                return;
            }
            #endregion


            // Use the database connections
            try
            {
                connOmd.Open();

                InitialiseVersion();
                PopulateHubCheckboxList(connOmd);
                PopulateLnkCheckboxList(connOmd);
                PopulateSatCheckboxList(connOmd);
                PopulateLsatCheckboxList(connOmd);
            }
            catch (Exception ex)
            {
                SetTextMain("An issue was encountered while populating the available metadata for the selected version. The error message is: " + ex);
            }
            finally
            {
                connOmd.Close();
                connOmd.Dispose();
            }



            try
            {
                connStg.Open();
                PopulateStgCheckboxList(connStg);
            }
            catch
            {
                SetTextMain("The Staging Area objects could not be retrieved from metadata.");
            }
            finally
            {
                connStg.Close();
                connStg.Dispose();
            }



            try
            {
                connPsa.Open();
                PopulatePsaCheckboxList(connPsa);
            }
            catch
            {
                SetTextMain("The Persistent Staging Area objects could not be retrieved from metadata.");
            }
            finally
            {
                connPsa.Close();
                connPsa.Dispose();
            }
        }

        private void ClearStgCheckBoxList()
        {
            int count = checkedListBoxStgMetadata.Items.Count;
            for (int index = count; index > 0; index--)

            {
                if (checkedListBoxStgMetadata.CheckedItems.Contains(checkedListBoxStgMetadata.Items[index - 1]))
                {
                    checkedListBoxStgMetadata.Items.RemoveAt(index - 1);
                }
            }
        }

        private void ClearPsaCheckBoxList()
        {
            int count = checkedListBoxPsaMetadata.Items.Count;
            for (int index = count; index > 0; index--)

            {
                if (checkedListBoxPsaMetadata.CheckedItems.Contains(checkedListBoxPsaMetadata.Items[index - 1]))
                {
                    checkedListBoxPsaMetadata.Items.RemoveAt(index - 1);
                }
            }
        }

        private void ClearHubCheckBoxList()
        {
            int count = checkedListBoxHubMetadata.Items.Count;
            for (int index = count; index > 0; index--)

            {
                if (checkedListBoxHubMetadata.CheckedItems.Contains(checkedListBoxHubMetadata.Items[index - 1]))
                {
                    checkedListBoxHubMetadata.Items.RemoveAt(index - 1);
                }
            }
        }

        private void ClearLinkCheckBoxList()
        {
            int count = checkedListBoxLinkMetadata.Items.Count;
            for (int index = count; index > 0; index--)

            {
                if (checkedListBoxLinkMetadata.CheckedItems.Contains(checkedListBoxLinkMetadata.Items[index - 1]))
                {
                    checkedListBoxLinkMetadata.Items.RemoveAt(index - 1);
                }
            }
        }

        private void ClearSatCheckBoxList()
        {
            int count = checkedListBoxSatMetadata.Items.Count;
            for (int index = count; index > 0; index--)

            {
                if (checkedListBoxSatMetadata.CheckedItems.Contains(checkedListBoxSatMetadata.Items[index - 1]))
                {
                    checkedListBoxSatMetadata.Items.RemoveAt(index - 1);
                }
            }
        }

        private void ClearLsatCheckBoxList()
        {
            int count = checkedListBoxLsatMetadata.Items.Count;
            for (int index = count; index > 0; index--)

            {
                if (checkedListBoxLsatMetadata.CheckedItems.Contains(checkedListBoxLsatMetadata.Items[index - 1]))
                {
                    checkedListBoxLsatMetadata.Items.RemoveAt(index - 1);
                }
            }
        }

        private void LoadTeamConfigurationFile()
        {
            // Load the rest of the (TEAM) configurations, from wherever they may be according to the VEDW settings (the TEAM configuration file)\
            var teamConfigurationFileName = VedwConfigurationSettings.TeamConfigurationPath +
                                            GlobalParameters.TeamConfigurationfileName + '_' +
                                            VedwConfigurationSettings.WorkingEnvironment +
                                            GlobalParameters.VedwFileExtension;

            richTextBoxInformationMain.Text = "Retrieving TEAM configuration details from '" + teamConfigurationFileName + "'. \r\n\r\n";

            var teamConfigResult = EnvironmentConfiguration.LoadTeamConfigurationFile(teamConfigurationFileName);

            if (teamConfigResult.Length > 0)
            {
                richTextBoxInformationMain.AppendText(
                    "Issues have been encountered while retrieving the TEAM configuration details. The following is returned: " +
                    teamConfigResult + "\r\n\r\n");
            }
        }

        /// <summary>
        /// Updating the SQL statements used for either Binary or Character hash value calculation.
        /// </summary>
        private void UpdateHashSnippets()
        {
            if (VedwConfigurationSettings.HashKeyOutputType == "Binary")
            {
                VedwConfigurationSettings.hashingStartSnippet = "HASHBYTES('MD5',";
                VedwConfigurationSettings.hashingEndSnippet = ")";
                VedwConfigurationSettings.hashingCollation = "";
                VedwConfigurationSettings.hashingZeroKey = "0x00000000000000000000000000000000";
            }
            else if (VedwConfigurationSettings.HashKeyOutputType == "Character")
            {
                VedwConfigurationSettings.hashingStartSnippet = "CONVERT(CHAR(32),HASHBYTES('MD5',";
                VedwConfigurationSettings.hashingEndSnippet = "),2)";
                VedwConfigurationSettings.hashingCollation = "COLLATE DATABASE_DEFAULT";
                VedwConfigurationSettings.hashingZeroKey = "'00000000000000000000000000000000'";
            }
            else
            {
                richTextBoxInformationMain.AppendText("An issue was encountered updating the Hash output setting on the application - please verify.");
            }
        }

        /// <summary>
        /// This is the local updates on the VEDW specific configuration.
        /// </summary>
        private void UpdateVedwConfigurationSettingsOnForm()
        {
            textBoxOutputPath.Text = VedwConfigurationSettings.VedwOutputPath;
            textBoxConfigurationPath.Text = VedwConfigurationSettings.TeamConfigurationPath;
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
                richTextBoxInformationMain.AppendText("An issue was encountered updating the Hash outpu setting on the application - please verify.");
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




        private void CheckKeyword(string word, Color color, int startIndex)
        {
            if (richTextBoxInformationMain.Text.Contains(word))
            {
                int index = -1;
                int selectStart = richTextBoxInformationMain.SelectionStart;

                while ((index = richTextBoxInformationMain.Text.IndexOf(word, (index + 1), StringComparison.Ordinal)) != -1)
                {
                    richTextBoxInformationMain.Select((index + startIndex), word.Length);
                    richTextBoxInformationMain.SelectionColor = color;
                    richTextBoxInformationMain.Select(selectStart, 0);
                    richTextBoxInformationMain.SelectionColor = Color.Black;
                }
            }
        }

        private void InitialiseDocumentation()
        {
            richTextBoxPSA.Text =
                "The Persistent Staging Area (PSA) is the foundation of the Virtual Enterprise Data Warehouse (EDW). " +
                "The ETL effectively compares and loads the delta into the PSA tables that correspond to the Staging Area counterparts." +
                "\n\r" +
                "Because of this the logic is generated as 'SELECT INSERT' to load new data delta into this area.";

            richTextBoxStaging.Text =
                "The Staging Area component is a simple example of delta detection using a Full Outer Join (FJO) type interface." +
                "In most cases the data delta from the source into the Staging Area is handled by a dedicated ETL tool, or at the very least using different techniques (of which this is only one example). This step is mainly used for demonstration purposes.";

            richTextBoxHub.Text =
                "The Hub type entities define the business concept and integration point for the model. The generated views combine the metadata from the various source to target mappings to create a single integrated Hub query.";

            richTextBoxSat.Text =
                "The Satellite type entities capture (historical / time-variant) context about the Business Keys in the Hub entities. A Satellite is typically sourced from a single Staging Area table.";

            richTextBoxLink.Text =
                "The Link type entities record the relationships between the Business Entities (Hubs). Similar to Hubs they are subject to potentially being populated from multiple Staging Area tables. The Link views therefore present an integrated view of all relationships across these tables.";

            richTextBoxLsat.Text =
                "The Link Satellites describe the changes over time for the relationships (Links). There are two types of Link Satellites supported - normal (historical) Link Satellites and Driving Key Link Satellites.";

        }

        private void HubButtonClick(object sender, EventArgs e)
        {
            richTextBoxHub.Clear();
            richTextBoxHubOutput.Clear();
            richTextBoxInformationMain.Clear();

            var newThread = new Thread(BackgroundDoHub);
            newThread.Start();

            tabControlHub.SelectedIndex = 0;
        }

        private void BackgroundDoHub()
        {
            // Check if the schema needs to be created
            CreateSchema(TeamConfigurationSettings.ConnectionStringInt);

            GenerateHubFromPattern();
        }


        /// <summary>
        ///   Create Hub SQL using Handlebars as templating engine
        /// </summary>
        private void GenerateHubFromPattern()
        {
            int errorCounter = 0;

            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            var connPsa = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringHstg };

            if (checkedListBoxHubMetadata.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= checkedListBoxHubMetadata.CheckedItems.Count - 1; x++)
                {
                    var targetTableName = checkedListBoxHubMetadata.CheckedItems[x].ToString();
                    SetTextHub($"Processing generation for {targetTableName}\r\n");

                    // Retrieve metadata and store in a data table object
                    var metadataQuery = 
                               @"SELECT 
                                   [SOURCE_NAME]
                                  ,[SOURCE_BUSINESS_KEY_DEFINITION]
                                  ,[TARGET_NAME]
                                  ,[TARGET_BUSINESS_KEY_DEFINITION]
                                  ,[FILTER_CRITERIA]
                                  ,[SURROGATE_KEY]
                                FROM [interface].[INTERFACE_SOURCE_HUB_XREF]
                                WHERE [TARGET_NAME] = '"+targetTableName+"'";

                    var metadataDataTable = GetDataTable(ref connOmd, metadataQuery);

                    // Move the data table to the class instance
                    List<SourceToTargetMapping> sourceToTargetMappingList = new List<SourceToTargetMapping>();

                    foreach (DataRow row in metadataDataTable.Rows)
                    {
                        // Creating the Business Key, using the available components (see above)
                        List<BusinessKey> businessKeyList = new List<BusinessKey>();
                        BusinessKey businessKey =
                            new BusinessKey
                            {
                                businessKeyComponentMapping = InterfaceHandling.BusinessKeyComponentMappingList((string)row["SOURCE_BUSINESS_KEY_DEFINITION"], (string)row["TARGET_BUSINESS_KEY_DEFINITION"]),
                                surrogateKey = (string)row["SURROGATE_KEY"]
                            };

                        businessKeyList.Add(businessKey);

                        // Add the created Business Key to the source-to-target mapping
                        var sourceToTargetMapping = new SourceToTargetMapping();

                        sourceToTargetMapping.sourceTable = (string)row["SOURCE_NAME"];
                        sourceToTargetMapping.targetTable = (string)row["TARGET_NAME"];
                        sourceToTargetMapping.targetTableHashKey = (string)row["SURROGATE_KEY"];
                        sourceToTargetMapping.businessKey = businessKeyList;
                        sourceToTargetMapping.filterCriterion = (string) row["FILTER_CRITERIA"];

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
                        var template = Handlebars.Compile(VedwConfigurationSettings.activeLoadPatternHub);
                        var result = template(sourceTargetMappingList);

                        // Check if the metadata needs to be displayed also
                        DisplayJsonMetadata(sourceTargetMappingList, "Hub");

                        // Display the output of the template to the user
                        SetTextHubOutput(result);

                        // Spool the output to disk
                        errorCounter = SaveOutputToDisk(textBoxOutputPath.Text + @"\Output_" + targetTableName + ".sql", result, errorCounter);

                        // Generate in database
                        errorCounter = ExecuteOutputInDatabase(connPsa, result, errorCounter);
                    }
                    catch (Exception ex)
                    {
                        errorCounter++;
                        SetTextMain("The template could not be compiled, the error message is " + ex);
                    }
                }
            }
            else
            {
                SetTextHub("There was no metadata selected to create Hub code. Please check the metadata schema - are there any Hubs selected?");
            }
 
            connOmd.Close();
            connOmd.Dispose();

            SetTextMain($"\r\n{errorCounter} errors have been found.\r\n");
            SetTextMain($"Associated scripts have been saved in {VedwConfigurationSettings.VedwOutputPath}.\r\n");

            // Call a delegate to handle multi-threading for syntax highlighting
            SetTextHubOutputSyntax(richTextBoxHubOutput);
        }

        internal int ExecuteOutputInDatabase(SqlConnection sqlConnection, string result, int errorCounter)
        {
            if (checkBoxGenerateInDatabase.Checked)
            {
                try
                {
                    sqlConnection.ConnectionString = TeamConfigurationSettings.ConnectionStringHstg;
                    using (var connection = sqlConnection)
                    {
                        var server = new Server(new ServerConnection(connection));
                        try
                        {
                            server.ConnectionContext.ExecuteNonQuery(result);
                            SetTextMain("The statement was executed successfully.\r\n");
                        }
                        catch (Exception exception)
                        {
                            SetTextMain("Issues occurred executing the SQL statement.\r\n");
                            SetTextMain(@"SQL error: " + exception.Message + "\r\n\r\n");
                            errorCounter++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorCounter++;
                    SetTextMain(
                        "There was an issue executing the code against the database. The message is: " +
                        ex);
                }
            }

            return errorCounter;
        }

        private int SaveOutputToDisk(string targetFile, string textContent, int errorCounter)
        {
            if (checkBoxSaveToFile.Checked)
            {
                try
                {
                    //Output to file
                    using (var outfile = new StreamWriter(targetFile))
                    {
                        outfile.Write(textContent);
                        outfile.Close();
                    }
                }
                catch (Exception ex)
                {
                    errorCounter++;
                    SetTextMain("There was an issue in saving the output to disk. The message is: " + ex);
                }
            }

            return errorCounter;
        }

        private void DisplayJsonMetadata(SourceToTargetMappingList sourceTargetMappingList, string output)
        {
            if (checkBoxGenerateJsonSchema.Checked)
            {
                try
                {
                    var json = JsonConvert.SerializeObject(sourceTargetMappingList, Formatting.Indented);

                    if (output == "Hub")
                    {
                        SetTextHubOutput(json + "\r\n\r\n");
                    }
                    else if (output == "Link")
                    {
                        SetTextLinkOutput(json + "\r\n\r\n");
                    }
                    else if (output == "Satellite")
                    {
                        SetTextSatOutput(json + "\r\n\r\n");
                    }
                    else if (output == "LinkSatellite")
                    {
                        SetTextLsatOutput(json + "\r\n\r\n");
                    }
                    else if (output == "StagingArea")
                    {
                        SetTextStgOutput(json + "\r\n\r\n");
                    }
                    else if (output == "PersistentStagingArea")
                    {
                        SetTextPsaOutput(json + "\r\n\r\n");
                    }
                }
                catch (Exception ex)
                {
                    SetTextMain("An error was encountered while generating the JSON metadata. The error message is: " + ex);
                }
            }
        }

        /// <summary>
        ///   Create Satellite code using Handlebars as templating
        /// </summary>
        private void GenerateSatFromPattern()
        {
            int errorCounter = 0;

            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            var connPsa = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringHstg };

            if (checkedListBoxSatMetadata.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= checkedListBoxSatMetadata.CheckedItems.Count - 1; x++)
                {
                    var targetTableName = checkedListBoxSatMetadata.CheckedItems[x].ToString();
                    SetTextSat(@"Processing generation for " + targetTableName + "\r\n");

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
                                          FROM interface.INTERFACE_SOURCE_SATELLITE_XREF 
                                          WHERE TARGET_TYPE = 'Normal' 
                                          AND TARGET_NAME = '" + targetTableName + "'";

                    var metadataDataTable = GetDataTable(ref connOmd, metadataQuery);

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
                                                     ,[MULTI_ACTIVE_KEY_INDICATOR]
                                                   FROM [interface].[INTERFACE_SOURCE_SATELLITE_ATTRIBUTE_XREF]
                                                   WHERE TARGET_NAME = '" + targetTableName + "' AND [SOURCE_NAME]='"+(string)row["SOURCE_NAME"]+"'";

                        var columnMetadataDataTable = GetDataTable(ref connOmd, columnMetadataQuery);

                        List<ColumnMapping> columnMappingList = new List<ColumnMapping>();
                        foreach (DataRow column in columnMetadataDataTable.Rows)
                        {
                            ColumnMapping columnMapping = new ColumnMapping();
                            columnMapping.sourceColumn.columnName = (string) column["SOURCE_ATTRIBUTE_NAME"];
                            columnMapping.targetColumn.columnName = (string) column["TARGET_ATTRIBUTE_NAME"];
                            columnMappingList.Add(columnMapping);
                        }

                        // Add the created Business Key to the source-to-target mapping
                        var sourceToTargetMapping = new SourceToTargetMapping();

                        sourceToTargetMapping.sourceTable = (string)row["SOURCE_NAME"];
                        sourceToTargetMapping.targetTable = (string)row["TARGET_NAME"];
                        sourceToTargetMapping.targetTableHashKey = (string)row["SURROGATE_KEY"];
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
                        var template = Handlebars.Compile(VedwConfigurationSettings.activeLoadPatternSat);
                        var result = template(sourceTargetMappingList);

                        // Display the output of the template to the user
                        SetTextSatOutput(result);

                        // Check if the metadata needs to be displayed also
                        DisplayJsonMetadata(sourceTargetMappingList, "Satellite");

                        // Spool the output to disk
                        errorCounter = SaveOutputToDisk(textBoxOutputPath.Text + @"\Output_" + targetTableName + ".sql", result, errorCounter);

                        //Generate in database
                        errorCounter = ExecuteOutputInDatabase(connPsa, result, errorCounter);
                    }
                    catch (Exception ex)
                    {
                        errorCounter++;
                        SetTextMain("The template could not be compiled, the error message is " + ex);
                    }
                }
            }
            else
            {
                SetTextSat("There was no metadata selected to create Satellite code. Please check the metadata schema - are there any Satellites selected?");
            }
 
            connOmd.Close();
            connOmd.Dispose();

            SetTextMain($"\r\n{errorCounter} errors have been found.\r\n");
            SetTextMain($"Associated scripts have been saved in {VedwConfigurationSettings.VedwOutputPath}.\r\n");

            // Call a delegate to handle multi-threading for syntax highlighting
            SetTextSatOutputSyntax(richTextBoxSatOutput);
        }

        public DataTable GetBusinessKeyComponentList(string stagingTableName, string hubTableName, string businessKeyDefinition)
        {            

            // Retrieving the top level component to evaluate composite, concat or pivot 
            var conn = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringOmd};

            try
            {
                conn.Open();
            }
            catch (Exception exception)
            {
                SetTextMain("An error has occurred interpreting the components of the Business Key for "+ hubTableName + " due to connectivity issues (connection string " + conn.ConnectionString + "). The associated message is " + exception.Message);
   
            }

            businessKeyDefinition = businessKeyDefinition.Replace("'", "''");

            var sqlStatementForComponent = new StringBuilder();

            sqlStatementForComponent.AppendLine("SELECT");
            sqlStatementForComponent.AppendLine("  [SOURCE_ID]");
            sqlStatementForComponent.AppendLine(" ,[SOURCE_NAME]"); 
            sqlStatementForComponent.AppendLine(" ,[SOURCE_SCHEMA_NAME]");
            sqlStatementForComponent.AppendLine(" ,[HUB_ID]");
            sqlStatementForComponent.AppendLine(" ,[HUB_NAME]");
            sqlStatementForComponent.AppendLine(" ,[BUSINESS_KEY_DEFINITION]");
            sqlStatementForComponent.AppendLine(" ,[BUSINESS_KEY_COMPONENT_ID]");
            sqlStatementForComponent.AppendLine(" ,[BUSINESS_KEY_COMPONENT_ORDER]");
            sqlStatementForComponent.AppendLine(" ,[BUSINESS_KEY_COMPONENT_VALUE]");
            sqlStatementForComponent.AppendLine("FROM [interface].[INTERFACE_BUSINESS_KEY_COMPONENT]");
            sqlStatementForComponent.AppendLine("WHERE SOURCE_NAME = '" + stagingTableName + "'");
            sqlStatementForComponent.AppendLine("  AND HUB_NAME = '" + hubTableName + "'");
            sqlStatementForComponent.AppendLine("  AND BUSINESS_KEY_DEFINITION = '" + businessKeyDefinition + "'");

            var componentList = GetDataTable(ref conn, sqlStatementForComponent.ToString());

            if (componentList == null)
            {
                SetTextMain("An error has occurred interpreting the Hub Business Key (components) in the model for " + hubTableName + ". The Business Key was not found when querying the underlying metadata.");
            }

            return componentList;
        }
        
        private void openOutputDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            try
            {
                Process.Start(VedwConfigurationSettings.VedwOutputPath);
            }
            catch (Exception ex)
            {
                richTextBoxInformationMain.Text = "An error has occured while attempting to open the output directory. The error message is: "+ex;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SatelliteButtonClick (object sender, EventArgs e)
        {
            richTextBoxSat.Clear();
            richTextBoxSatOutput.Clear();
            richTextBoxInformationMain.Clear();

            var newThread = new Thread(BackgroundDoSat);
            newThread.Start();

            tabControlSat.SelectedIndex = 0;
        }

        private void BackgroundDoSat(Object obj)
        {
            // Check if the schema needs to be created
            CreateSchema(TeamConfigurationSettings.ConnectionStringInt);

            //if (radiobuttonViews.Checked)
            //{
              //  GenerateSatViews();
                GenerateSatFromPattern();
                //}
                //else if (radioButtonIntoStatement.Checked)
                //{
                //    GenerateSatInsertInto();
                //}
        }

        private void LinkButtonClick (object sender, EventArgs e)
        {
            richTextBoxLink.Clear();
            richTextBoxLinkOutput.Clear();
            richTextBoxInformationMain.Clear();

            var newThread = new Thread(BackgroundDoLink);
            newThread.Start();
            tabControlLink.SelectedIndex = 0;
        }

        private void BackgroundDoLink(Object obj)
        {
            // Check if the schema needs to be created
            CreateSchema(TeamConfigurationSettings.ConnectionStringInt);
            GenerateLinkFromPattern();
        }

        /// <summary>
        ///   Create Link SQL using Handlebars as templating engine
        /// </summary>
        private void GenerateLinkFromPattern()
        {
            int errorCounter = 0;

            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            var connPsa = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringHstg };

            if (checkedListBoxLinkMetadata.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= checkedListBoxLinkMetadata.CheckedItems.Count - 1; x++)
                {
                    var targetTableName = checkedListBoxLinkMetadata.CheckedItems[x].ToString();
                    SetTextLink($"Processing generation for {targetTableName}\r\n");

                    // Retrieve metadata and store in a data table object
                    var metadataQuery = @"
                    SELECT 
                       [SOURCE_NAME]
                      ,[SOURCE_BUSINESS_KEY_DEFINITION]
                      ,[TARGET_NAME]
                      ,[TARGET_BUSINESS_KEY_DEFINITION]
                      ,[FILTER_CRITERIA]
                      ,[SURROGATE_KEY]
                    FROM [interface].[INTERFACE_SOURCE_LINK_XREF]
                    WHERE [TARGET_NAME] = '" + targetTableName + "'";

                    var metadataDataTable = GetDataTable(ref connOmd, metadataQuery);

                    // Move the data table to the class instance
                    List<SourceToTargetMapping> sourceToTargetMappingList = new List<SourceToTargetMapping>();

                    foreach (DataRow row in metadataDataTable.Rows)
                    {
                        // Commence creating the list of Business Keys. A Link entity has multiple Business Keys. One for the Link and a few for the Hubs.
                        List<BusinessKey> businessKeyList = new List<BusinessKey>();

                        // Creating the Link Business Key definition, using the available components (see above)
                        BusinessKey businessKey =
                            new BusinessKey
                            {
                                businessKeyComponentMapping = InterfaceHandling.BusinessKeyComponentMappingList((string)row["SOURCE_BUSINESS_KEY_DEFINITION"], (string)row["TARGET_BUSINESS_KEY_DEFINITION"]),
                                surrogateKey = (string)row["SURROGATE_KEY"]
                            };

                        businessKeyList.Add(businessKey); // Adding the Link Business Key


                        // Creating the various Hub Business Keys for the Link
                        // Retrieve metadata and store in a data table object
                        var hubLinkQuery = @"
                            SELECT 
                               [SOURCE_SCHEMA_NAME]
                              ,[SOURCE_NAME]
                              ,[LINK_SCHEMA_NAME]
                              ,[LINK_NAME]
                              ,[HUB_SCHEMA_NAME]
                              ,[HUB_NAME]
                              ,[HUB_SURROGATE_KEY]
                              ,[HUB_SOURCE_BUSINESS_KEY_DEFINITION]
                              ,[HUB_TARGET_BUSINESS_KEY_DEFINITION]
                              ,[HUB_ORDER]
                            FROM [interface].[INTERFACE_HUB_LINK_XREF]
                            WHERE [LINK_NAME] = '" + targetTableName + "'" + 
                            "ORDER BY [HUB_ORDER]";

                        var hubDataTable = GetDataTable(ref connOmd, hubLinkQuery);

                        foreach (DataRow hubRow in hubDataTable.Rows)
                        {
                            var hubBusinessKey = new BusinessKey();

                            hubBusinessKey.businessKeyComponentMapping =
                                InterfaceHandling.BusinessKeyComponentMappingList(
                                    (string)hubRow["HUB_SOURCE_BUSINESS_KEY_DEFINITION"], (string)hubRow["HUB_TARGET_BUSINESS_KEY_DEFINITION"]);
                            hubBusinessKey.surrogateKey = (string)hubRow["HUB_SURROGATE_KEY"];

                            businessKeyList.Add(hubBusinessKey); // Adding the Link Business Key
                        }

                        // Defining the source-to-target mapping
                        var sourceToTargetMapping = new SourceToTargetMapping
                        {
                            sourceTable = (string) row["SOURCE_NAME"],
                            targetTable = (string) row["TARGET_NAME"],
                            targetTableHashKey = (string) row["SURROGATE_KEY"],
                            businessKey = businessKeyList,
                            filterCriterion = (string) row["FILTER_CRITERIA"]
                        };

                        sourceToTargetMappingList.Add(sourceToTargetMapping);
                    }

                    // Create an instance of the 'MappingList' class / object 
                    SourceToTargetMappingList sourceTargetMappingList = new SourceToTargetMappingList();
                    sourceTargetMappingList.individualSourceToTargetMapping = sourceToTargetMappingList;
                    sourceTargetMappingList.metadataConfiguration = new MetadataConfiguration();
                    sourceTargetMappingList.mainTable = targetTableName;

                    // Return the result to the user
                    try
                    {
                        // Compile the template, and merge it with the metadata
                        var template = Handlebars.Compile(VedwConfigurationSettings.activeLoadPatternLnk);
                        var result = template(sourceTargetMappingList);

                        // Check if the metadata needs to be displayed also
                        DisplayJsonMetadata(sourceTargetMappingList,"Link");

                        // Display the output of the template to the user
                        SetTextLinkOutput(result);

                        // Spool the output to disk
                        errorCounter = SaveOutputToDisk(textBoxOutputPath.Text + @"\Output_" + targetTableName + ".sql", result, errorCounter);

                        // Generate in database
                        errorCounter = ExecuteOutputInDatabase(connPsa, result, errorCounter);
                    }
                    catch (Exception ex)
                    {
                        errorCounter++;
                        SetTextMain("The template could not be compiled, the error message is " + ex);
                    }
                }
            }
            else
            {
                SetTextLink("There was no metadata selected to create Link views. Please check the metadata schema - are there any Links selected?");
            }

            connOmd.Close();
            connOmd.Dispose();

            SetTextMain($"\r\n{errorCounter} errors have been found.\r\n");
            if (checkBoxSaveToFile.Checked)
            {
                SetTextMain($"Associated scripts have been saved in {VedwConfigurationSettings.VedwOutputPath}.\r\n");
            }

            // Call a delegate to handle multi-threading for syntax highlighting
            SetTextLnkOutputSyntax(richTextBoxLinkOutput);
        }

        internal DataTable GetBusinessKeyElements(string stagingAreaTableName,string hubTableName, string businessKeyDefinition, int businessKeyComponentId)
        {
            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            var sqlStatementForSourceBusinessKey = new StringBuilder();

            businessKeyDefinition = businessKeyDefinition.Replace("'", "''");

            sqlStatementForSourceBusinessKey.AppendLine("SELECT * FROM interface.INTERFACE_BUSINESS_KEY_COMPONENT_PART");
            sqlStatementForSourceBusinessKey.AppendLine("WHERE SOURCE_NAME= '" + stagingAreaTableName + "'");
            sqlStatementForSourceBusinessKey.AppendLine("  AND HUB_NAME= '" + hubTableName + "'");
            sqlStatementForSourceBusinessKey.AppendLine("  AND BUSINESS_KEY_DEFINITION = '" + businessKeyDefinition + "'");
            sqlStatementForSourceBusinessKey.AppendLine("  AND BUSINESS_KEY_COMPONENT_ID = '" + businessKeyComponentId + "'");
            sqlStatementForSourceBusinessKey.AppendLine("ORDER BY BUSINESS_KEY_COMPONENT_ORDER, BUSINESS_KEY_COMPONENT_ELEMENT_ORDER");

            var elementList = GetDataTable(ref connOmd, sqlStatementForSourceBusinessKey.ToString());
            return elementList;
        }

        private void LinkSatelliteButtonClick (object sender, EventArgs e)
        {
            // Check if the schema needs to be created
            CreateSchema(TeamConfigurationSettings.ConnectionStringInt);

            richTextBoxLsat.Clear();
            richTextBoxLsatOutput.Clear();
            richTextBoxInformationMain.Clear();

            var newThread = new Thread(BackgroundDoLsat);
            newThread.Start();
            tabControlLsat.SelectedIndex = 0;
        }

        private void BackgroundDoLsat(Object obj)
        {
            CreateSchema(TeamConfigurationSettings.ConnectionStringInt);
            GenerateLinkSatelliteFromPattern();
            //if (radiobuttonViews.Checked)
            //{
           //     GenerateLsatHistoryViews();
            //    GenerateLsatDrivingKeyViews();
            //}
            //else if (radioButtonIntoStatement.Checked)
            //{
            //    GenerateLsatInsertInto();
            //}
        }

        /// <summary>
        ///   Create Link-Satellite SQL using Handlebars as templating engine
        /// </summary>
        private void GenerateLinkSatelliteFromPattern()
        {
            int errorCounter = 0;

            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            var connPsa = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringHstg };

            if (checkedListBoxLsatMetadata.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= checkedListBoxLsatMetadata.CheckedItems.Count - 1; x++)
                {
                    var targetTableName = checkedListBoxLsatMetadata.CheckedItems[x].ToString();
                    SetTextLsat($"Processing Link Satellite generation for {targetTableName}\r\n");

                    // Retrieve metadata and store in a data table object
                    var metadataQuery =
                               @"SELECT 
                                   [SOURCE_NAME]
                                  ,[SOURCE_BUSINESS_KEY_DEFINITION]
                                  ,[TARGET_NAME]
                                  ,[TARGET_BUSINESS_KEY_DEFINITION]
                                  ,[FILTER_CRITERIA]
                                  ,[SURROGATE_KEY]
                                FROM [interface].[INTERFACE_SOURCE_HUB_XREF]
                                WHERE [TARGET_NAME] = '" + targetTableName + "'";

                    var metadataDataTable = GetDataTable(ref connOmd, metadataQuery);

                    // Move the data table to the class instance
                    List<SourceToTargetMapping> sourceToTargetMappingList = new List<SourceToTargetMapping>();

                    foreach (DataRow row in metadataDataTable.Rows)
                    {
                        // Creating the Business Key, using the available components (see above)
                        List<BusinessKey> businessKeyList = new List<BusinessKey>();
                        BusinessKey businessKey =
                            new BusinessKey
                            {
                                businessKeyComponentMapping = InterfaceHandling.BusinessKeyComponentMappingList((string)row["SOURCE_BUSINESS_KEY_DEFINITION"], (string)row["TARGET_BUSINESS_KEY_DEFINITION"])
                            };

                        businessKeyList.Add(businessKey);

                        // Add the created Business Key to the source-to-target mapping
                        var sourceToTargetMapping = new SourceToTargetMapping();

                        sourceToTargetMapping.sourceTable = (string)row["SOURCE_NAME"];
                        sourceToTargetMapping.targetTable = (string)row["TARGET_NAME"];
                        sourceToTargetMapping.targetTableHashKey = (string)row["SURROGATE_KEY"];
                        sourceToTargetMapping.businessKey = businessKeyList;
                        sourceToTargetMapping.filterCriterion = (string)row["FILTER_CRITERIA"];

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
                        var template = Handlebars.Compile(VedwConfigurationSettings.activeLoadPatternLsat);
                        var result = template(sourceTargetMappingList);

                        // Display the output of the template to the user
                        SetTextLsatOutput(result);

                        // Check if the metadata needs to be displayed also
                        DisplayJsonMetadata(sourceTargetMappingList,"LinkSatellite");

                        // Spool the output to disk
                        errorCounter = SaveOutputToDisk(textBoxOutputPath.Text + @"\Output_" + targetTableName + ".sql", result, errorCounter);

                        //Generate in database
                        errorCounter = ExecuteOutputInDatabase(connPsa, result, errorCounter);
                    }
                    catch (Exception ex)
                    {
                        errorCounter++;
                        SetTextMain("The template could not be compiled, the error message is " + ex);
                    }
                }
            }
            else
            {
                SetTextLsat("There was no metadata selected to create LInk Satellite code. Please check the metadata schema - are there any Link Satellites selected?");
            }

            connOmd.Close();
            connOmd.Dispose();

            SetTextLsat($"\r\n{errorCounter} errors have been found.\r\n");

            // Call a delegate to handle multi-threading for syntax highlighting
            SetTextLsatOutputSyntax(richTextBoxLsatOutput);
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

        private void CloseDimensionalForm(object sender, FormClosedEventArgs e)
        {
            _myDimensionalForm = null;
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

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var t = new Thread(ThreadProcAbout);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is yet to be implemented.", "Upcoming!", MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
        }

        private void linksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is yet to be implemented.", "Upcoming!", MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            InitialiseDocumentation();
        }

        private void buttonGeneratePSA_Click(object sender, EventArgs e)
        {
            richTextBoxPSA.Clear();
            richTextBoxPsaOutput.Clear();
            richTextBoxInformationMain.Clear();

            var newThread = new Thread(BackgroundDoPsa);
            newThread.Start();
            tabControlPsa.SelectedIndex = 0;
        }

        private void BackgroundDoPsa(Object obj)
        {
            // Check if the schema needs to be created
            CreateSchema(TeamConfigurationSettings.ConnectionStringHstg);
            GeneratePersistentStagingAreaFromPattern();
            //if (radiobuttonViews.Checked)
            //{
            // PsaGenerateViews();
            //}
            //else if (radioButtonIntoStatement.Checked)
            //{
            //    PsaGenerateInsertInto();
            //}
        }

        /// <summary>
        ///   Create Persistent Staging Area SQL using Handlebars as templating engine
        /// </summary>
        private void GeneratePersistentStagingAreaFromPattern()
        {
            int errorCounter = 0;

            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            var connPsa = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringHstg };

            if (checkedListBoxPsaMetadata.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= checkedListBoxPsaMetadata.CheckedItems.Count - 1; x++)
                {
                    var targetTableName = checkedListBoxPsaMetadata.CheckedItems[x].ToString();
                    SetTextPsa(@"Processing generation for " + targetTableName + "\r\n");

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
                                          FROM interface.INTERFACE_SOURCE_PERSISTENT_STAGING_XREF 
                                          WHERE TARGET_TYPE = 'PersistentStagingArea' 
                                          AND TARGET_NAME = '" + targetTableName + "'";

                    var metadataDataTable = GetDataTable(ref connOmd, metadataQuery);

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
                                                   FROM [interface].[INTERFACE_SOURCE_PERSISTENT_STAGING_ATTRIBUTE_XREF]
                                                   WHERE TARGET_NAME = '" + targetTableName + "' AND [SOURCE_NAME]='" + (string)row["SOURCE_NAME"] + "'";

                        var columnMetadataDataTable = GetDataTable(ref connOmd, columnMetadataQuery);

                        List<ColumnMapping> columnMappingList = new List<ColumnMapping>();
                        foreach (DataRow column in columnMetadataDataTable.Rows)
                        {
                            ColumnMapping columnMapping = new ColumnMapping();
                            columnMapping.sourceColumn.columnName = (string)column["SOURCE_ATTRIBUTE_NAME"];
                            columnMapping.targetColumn.columnName = (string)column["TARGET_ATTRIBUTE_NAME"];
                            columnMappingList.Add(columnMapping);
                        }

                        // Add the created Business Key to the source-to-target mapping
                        var sourceToTargetMapping = new SourceToTargetMapping();

                        sourceToTargetMapping.sourceTable = (string)row["SOURCE_NAME"];
                        sourceToTargetMapping.targetTable = (string)row["TARGET_NAME"];
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
                        var template = Handlebars.Compile(VedwConfigurationSettings.activeLoadPatternPsa);
                        var result = template(sourceTargetMappingList);

                        // Display the output of the template to the user
                        SetTextPsaOutput(result);

                        // Check if the metadata needs to be displayed also
                        DisplayJsonMetadata(sourceTargetMappingList,"PersistentStagingArea");

                        // Spool the output to disk
                        errorCounter = SaveOutputToDisk(textBoxOutputPath.Text + @"\Output_" + targetTableName + ".sql", result, errorCounter);

                        //Generate in database
                        errorCounter = ExecuteOutputInDatabase(connPsa, result, errorCounter);
                    }
                    catch (Exception ex)
                    {
                        errorCounter++;
                        SetTextMain("The template could not be compiled, the error message is " + ex);
                    }
                }
            }
            else
            {
                SetTextPsa("There was no metadata selected to create Persistent Staging Area code. Please check the metadata schema - are there any Persistent Staging Area tables selected?");
            }

            connOmd.Close();
            connOmd.Dispose();

            SetTextMain($"\r\n{errorCounter} errors have been found.\r\n");
            SetTextMain($"Associated scripts have been saved in {VedwConfigurationSettings.VedwOutputPath}.\r\n");


            // Call a delegate to handle multi-threading for syntax highlighting
            SetTextPsaOutputSyntax(richTextBoxPsaOutput);
        }



        private void buttonGenerateStaging_Click(object sender, EventArgs e)
        {
            richTextBoxStaging.Clear();
            richTextBoxStgOutput.Clear();
            richTextBoxInformationMain.Clear();

            var newThread = new Thread(BackgroundDoStaging);
            newThread.Start();
            tabControlStg.SelectedIndex = 0;
        }

        private void BackgroundDoStaging(Object obj)
        {
            // Check if the schema needs to be created
            CreateSchema(TeamConfigurationSettings.ConnectionStringStg);

            GenerateStagingAreaFromPattern();
            //if (radiobuttonViews.Checked)
            //{
            // StagingGenerateViews();


            //}
            //else if (radioButtonIntoStatement.Checked)
            //{
            //    StagingGenerateInsertInto();
            //}
        }

        /// <summary>
        ///   Create Staging Area SQL using Handlebars as templating engine
        /// </summary>
        private void GenerateStagingAreaFromPattern()
        {
            int errorCounter = 0;

            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            var connPsa = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringHstg };

            if (checkedListBoxStgMetadata.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= checkedListBoxStgMetadata.CheckedItems.Count - 1; x++)
                {
                    var targetTableName = checkedListBoxStgMetadata.CheckedItems[x].ToString();
                    SetTextStg(@"Processing generation for " + targetTableName + "\r\n");

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

                    var metadataDataTable = GetDataTable(ref connOmd, metadataQuery);

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

                        var columnMetadataDataTable = GetDataTable(ref connOmd, columnMetadataQuery);

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

                        var lookupTable = (string) row["TARGET_NAME"];
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
                        DisplayJsonMetadata(sourceTargetMappingList, "StagingArea");

                        // Display the output of the template to the user
                        SetTextStgOutput(result);

                        // Spool the output to disk
                        errorCounter = SaveOutputToDisk(textBoxOutputPath.Text + @"\Output_" + targetTableName + ".sql", result, errorCounter);

                        //Generate in database
                        errorCounter = ExecuteOutputInDatabase(connPsa, result, errorCounter);
                    }
                    catch (Exception ex)
                    {
                        errorCounter++;
                        SetTextMain("The template could not be compiled, the error message is " + ex);
                    }
                }
            }
            else
            {
                SetTextStg("There was no metadata selected to generate Staging Area code. Please check the metadata schema - are there any Staging Area tables selected?");
            }

            connOmd.Close();
            connOmd.Dispose();

            SetTextMain($"\r\n{errorCounter} errors have been found.\r\n");
            SetTextMain($"Associated scripts have been saved in {VedwConfigurationSettings.VedwOutputPath}.\r\n");

            // Call a delegate to handle multi-threading for syntax highlighting
            SetTextStgOutputSyntax(richTextBoxStgOutput);
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
                    _myTestRiForm.Invoke((MethodInvoker)delegate { _myTestRiForm.Close(); });
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
                    _myTestDataForm.Invoke((MethodInvoker)delegate { _myTestDataForm.Close(); });
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

 

        private FormDimensional _myDimensionalForm;
        public void ThreadProcDimensional()
        {
            if (_myDimensionalForm == null)
            {
                _myDimensionalForm = new FormDimensional(this);
                _myDimensionalForm.Show();

                Application.Run();
            }

            else
            {
                if (_myDimensionalForm.InvokeRequired)
                {
                    // Thread Error
                    _myDimensionalForm.Invoke((MethodInvoker)delegate { _myDimensionalForm.Close(); });
                    _myDimensionalForm.FormClosed += CloseDimensionalForm;

                    _myDimensionalForm = new FormDimensional(this);
                    _myDimensionalForm.Show();
                    Application.Run();
                }
                else
                {
                    // No invoke required - same thread
                    _myDimensionalForm.FormClosed += CloseDimensionalForm;

                    _myDimensionalForm = new FormDimensional(this);
                    _myDimensionalForm.Show();
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
                    _myPitForm.Invoke((MethodInvoker)delegate { _myPitForm.Close(); });
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
                    _myAboutForm.Invoke((MethodInvoker)delegate { _myAboutForm.Close(); });
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



        /// <summary>
        /// Delegate to update Staging Area generation feedback to the main STG informational textbox.
        /// </summary>
        /// <param name="text"></param>
        delegate void SetTextCallBackStg(string text);
        private void SetTextStg(string text)
        {
            if (richTextBoxStaging.InvokeRequired)
            {
                var d = new SetTextCallBackStg(SetTextStg);
                Invoke(d, text);
            }
            else
            {
                richTextBoxStaging.AppendText(text);
            }
        }

        /// <summary>
        /// Delegate to update the Staging Area SQL output textbox.
        /// </summary>
        /// <param name="text"></param>
        delegate void SetTextCallBackStgOutput(string text);
        private void SetTextStgOutput(string text)
        {
            if (richTextBoxStgOutput.InvokeRequired)
            {
                var d = new SetTextCallBackStgOutput(SetTextStgOutput);
                Invoke(d, text);
            }
            else
            {
                richTextBoxStgOutput.AppendText(text);
            }
        }

        /// <summary>
        /// Delegate to apply syntax highlighting on the Staging Area SQL output textbox.
        /// </summary>
        /// <param name="textBox"></param>
        delegate void SetSyntaxCallbackStgSyntax(RichTextBox textBox);
        private void SetTextStgOutputSyntax(RichTextBox textBox)
        {
            if (richTextBoxStgOutput.InvokeRequired)
            {
                var d = new SetSyntaxCallbackStgSyntax(SetTextStgOutputSyntax);
                Invoke(d, textBox);
            }
            else
            {
                TextHandling.SyntaxHighlightSql(richTextBoxStgOutput, richTextBoxStgOutput.Text);
            }
        }



        /// <summary>
        /// Delegate to update Persistent Staging Area generation feedback to the main PSA informational textbox.
        /// </summary>
        /// <param name="text"></param>
        delegate void SetTextCallBackPsa(string text);
        private void SetTextPsa(string text)
        {
            if (richTextBoxPSA.InvokeRequired)
            {
                var d = new SetTextCallBackPsa(SetTextPsa);
                Invoke(d, text);
            }
            else
            {
                richTextBoxPSA.AppendText(text);
            }
        }

        /// <summary>
        /// Delegate to update the Persistent Staging Area SQL output textbox.
        /// </summary>
        /// <param name="text"></param>
        delegate void SetTextCallBackPsaOutput(string text);
        private void SetTextPsaOutput(string text)
        {
            if (richTextBoxPsaOutput.InvokeRequired)
            {
                var d = new SetTextCallBackPsaOutput(SetTextPsaOutput);
                Invoke(d, text);
            }
            else
            {
                richTextBoxPsaOutput.AppendText(text);
            }
        }

        /// <summary>
        /// Delegate to apply syntax highlighting on the Persistent Staging Area SQL output textbox.
        /// </summary>
        /// <param name="textBox"></param>
        delegate void SetSyntaxCallbackPsaSyntax(RichTextBox textBox);
        private void SetTextPsaOutputSyntax(RichTextBox textBox)
        {
            if (richTextBoxPsaOutput.InvokeRequired)
            {
                var d = new SetSyntaxCallbackPsaSyntax(SetTextPsaOutputSyntax);
                Invoke(d, textBox);
            }
            else
            {
                TextHandling.SyntaxHighlightSql(richTextBoxPsaOutput, richTextBoxPsaOutput.Text);
            }
        }




        /// <summary>
        /// Delegate to update Hub generation feedback to the main Hub informational textbox.
        /// </summary>
        /// <param name="text"></param>
        delegate void SetTextCallBackHub(string text);
        private void SetTextHub(string text)
        {
            if (richTextBoxHub.InvokeRequired)
            {
                var d = new SetTextCallBackHub(SetTextHub);
                Invoke(d, text);
            }
            else
            {
                richTextBoxHub.AppendText(text);
            }
        }

        /// <summary>
        /// Delegate to update the Hub SQL output textbox.
        /// </summary>
        /// <param name="text"></param>
        delegate void SetTextCallBackHubOutput(string text);
        private void SetTextHubOutput(string text)
        {
            if (richTextBoxHubOutput.InvokeRequired)
            {
                var d = new SetTextCallBackHubOutput(SetTextHubOutput);
                Invoke(d, text);
            }
            else
            {
                richTextBoxHubOutput.AppendText(text);
            }
        }

        /// <summary>
        /// Delegate to apply syntax highlighting on the Hub SQL output textbox.
        /// </summary>
        /// <param name="textBox"></param>
        delegate void SetSyntaxCallbackHubSyntax(RichTextBox textBox);
        private void SetTextHubOutputSyntax(RichTextBox textBox)
        {
            if (richTextBoxHubOutput.InvokeRequired)
            {
                var d = new SetSyntaxCallbackHubSyntax(SetTextHubOutputSyntax);
                Invoke(d, textBox);
            }
            else
            {
                TextHandling.SyntaxHighlightSql(richTextBoxHubOutput, richTextBoxHubOutput.Text);
            }
        }



        /// <summary>
        /// Delegate to update Satellite generation feedback to the main Satellite informational textbox.
        /// </summary>
        /// <param name="text"></param>
        delegate void SetTextCallBackSat(string text);
        private void SetTextSat(string text)
        {
            if (richTextBoxSat.InvokeRequired)
            {
                var d = new SetTextCallBackSat(SetTextSat);
                Invoke(d, text);
            }
            else
            {
                richTextBoxSat.AppendText(text);
            }
        }

        /// <summary>
        /// Delegate to update the Satellite SQL output textbox.
        /// </summary>
        /// <param name="text"></param>
        delegate void SetTextCallBackSatOutput(string text);
        private void SetTextSatOutput(string text)
        {
            if (richTextBoxSatOutput.InvokeRequired)
            {
                var d = new SetTextCallBackSatOutput(SetTextSatOutput);
                Invoke(d, text);
            }
            else
            {
                richTextBoxSatOutput.AppendText(text);
            }
        }

        /// <summary>
        /// Delegate to apply syntax highlighting on the Satellite SQL output textbox.
        /// </summary>
        /// <param name="textBox"></param>
        delegate void SetSyntaxCallbackSatSyntax(RichTextBox textBox);
        private void SetTextSatOutputSyntax(RichTextBox textBox)
        {
            if (richTextBoxSatOutput.InvokeRequired)
            {
                var d = new SetSyntaxCallbackSatSyntax(SetTextSatOutputSyntax);
                Invoke(d, textBox);
            }
            else
            {
                TextHandling.SyntaxHighlightSql(richTextBoxSatOutput, richTextBoxSatOutput.Text);
            }
        }
        


        /// <summary>
        /// Delegate to update Link generation feedback to the main LInk informational textbox.
        /// </summary>
        /// <param name="text"></param>
        delegate void SetTextCallBackLink(string text);
        private void SetTextLink(string text)
        {
            if (richTextBoxLink.InvokeRequired)
            {
                var d = new SetTextCallBackLink(SetTextLink);
                Invoke(d, text);
            }
            else
            {
                richTextBoxLink.AppendText(text);
            }
        }

        /// <summary>
        /// Delegate to update the Link SQL output textbox.
        /// </summary>
        /// <param name="text"></param>
        delegate void SetTextCallBackLinkOutput(string text);
        private void SetTextLinkOutput(string text)
        {
            if (richTextBoxLinkOutput.InvokeRequired)
            {
                var d = new SetTextCallBackLinkOutput(SetTextLinkOutput);
                Invoke(d, text);
            }
            else
            {
                richTextBoxLinkOutput.AppendText(text);
            }
        }

        /// <summary>
        /// Delegate to apply syntax highlighting on the Link SQL output textbox.
        /// </summary>
        /// <param name="textBox"></param>
        delegate void SetSyntaxCallbackLinkSyntax(RichTextBox textBox);
        private void SetTextLnkOutputSyntax(RichTextBox textBox)
        {
            if (richTextBoxLinkOutput.InvokeRequired)
            {
                var d = new SetSyntaxCallbackLinkSyntax(SetTextLnkOutputSyntax);
                Invoke(d, textBox);
            }
            else
            {
                TextHandling.SyntaxHighlightSql(richTextBoxLinkOutput, richTextBoxLinkOutput.Text);
            }
        }



        /// <summary>
        /// Delegate to update Link Satellite generation feedback to the main LSAT informational textbox.
        /// </summary>
        /// <param name="text"></param>
        delegate void SetTextCallBackLsat(string text);
        private void SetTextLsat(string text)
        {
            if (richTextBoxLsat.InvokeRequired)
            {
                var d = new SetTextCallBackLsat(SetTextLsat);
                Invoke(d, text);
            }
            else
            {
                richTextBoxLsat.AppendText(text);
            }
        }

        /// <summary>
        /// Delegate to update the Link-Satellite SQL output textbox.
        /// </summary>
        /// <param name="text"></param>
        delegate void SetTextCallBackLsatOutput(string text);
        private void SetTextLsatOutput(string text)
        {
            if (richTextBoxLsatOutput.InvokeRequired)
            {
                var d = new SetTextCallBackLsatOutput(SetTextLsatOutput);
                Invoke(d, text);
            }
            else
            {
                richTextBoxLsatOutput.AppendText(text);
            }
        }

        /// <summary>
        /// Delegate to apply syntax highlighting on the Link-Satellite SQL output textbox.
        /// </summary>
        /// <param name="textBox"></param>
        delegate void SetSyntaxCallbackLsatSyntax(RichTextBox textBox);
        private void SetTextLsatOutputSyntax(RichTextBox textBox)
        {
            if (richTextBoxLsatOutput.InvokeRequired)
            {
                var d = new SetSyntaxCallbackLsatSyntax(SetTextLsatOutputSyntax);
                Invoke(d, textBox);
            }
            else
            {
                TextHandling.SyntaxHighlightSql(richTextBoxLsatOutput, richTextBoxLsatOutput.Text);
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

        private void CheckAllHubValues()
        {
            //MessageBox.Show(checkBoxSelectAll.Checked.ToString());
            for (int x = 0; x <= checkedListBoxHubMetadata.Items.Count - 1; x++)
            {
                checkedListBoxHubMetadata.SetItemChecked(x, checkBoxSelectAllHubs.Checked);
            }
        }

        private void CheckAllSatValues()
        {
            //MessageBox.Show(checkBoxSelectAll.Checked.ToString());
            for (int x = 0; x <= checkedListBoxSatMetadata.Items.Count - 1; x++)
            {
                checkedListBoxSatMetadata.SetItemChecked(x, checkBoxSelectAllSats.Checked);
            }
        }

        private void CheckAllLinkValues()
        {
            //MessageBox.Show(checkBoxSelectAll.Checked.ToString());
            for (int x = 0; x <= checkedListBoxLinkMetadata.Items.Count - 1; x++)
            {
                checkedListBoxLinkMetadata.SetItemChecked(x, checkBoxSelectAllLinks.Checked);
            }
        }

        private void CheckAllLsatValues()
        {
            //MessageBox.Show(checkBoxSelectAll.Checked.ToString());
            for (int x = 0; x <= checkedListBoxLsatMetadata.Items.Count - 1; x++)
            {
                checkedListBoxLsatMetadata.SetItemChecked(x, checkBoxSelectAllLsats.Checked);
            }
        }

        private void CheckAllStgValues()
        {
            for (int x = 0; x <= checkedListBoxStgMetadata.Items.Count - 1; x++)
            {
                checkedListBoxStgMetadata.SetItemChecked(x, checkBoxSelectAllStg.Checked);
            }
        }

        private void CheckAllPsaValues()
        {
            //MessageBox.Show(checkBoxSelectAll.Checked.ToString());
            for (int x = 0; x <= checkedListBoxPsaMetadata.Items.Count - 1; x++)
            {
                checkedListBoxPsaMetadata.SetItemChecked(x, checkBoxSelectAllPsa.Checked);
            }
        }

        private void button_Repopulate_Hub(object sender, EventArgs e)
        {
            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            PopulateHubCheckboxList(connOmd);
        }

        private void PopulateStgCheckboxList(SqlConnection connStg)
        {
            // Clear the existing checkboxes
            checkedListBoxStgMetadata.Items.Clear();

            try
            {
                // Query the metadata for the STG and HSTG tables
                var queryMetadata = new StringBuilder();

                queryMetadata.AppendLine("SELECT TABLE_NAME");
                queryMetadata.AppendLine("FROM INFORMATION_SCHEMA.TABLES ");
                queryMetadata.AppendLine("WHERE TABLE_TYPE='BASE TABLE' ");
                queryMetadata.AppendLine("  AND TABLE_NAME LIKE '%"+textBoxFilterCriterionStg.Text+"%'");
                queryMetadata.AppendLine("  AND TABLE_NAME NOT LIKE '%USERMANAGED%'");
                if (checkBoxExcludeLanding.Checked)
                {
                    queryMetadata.AppendLine("  AND TABLE_NAME NOT LIKE '%_LANDING'");
                }
                queryMetadata.AppendLine("  AND TABLE_NAME LIKE '"+TeamConfigurationSettings.StgTablePrefixValue+"_%'");
                queryMetadata.AppendLine("ORDER BY TABLE_NAME");

                var tables = GetDataTable(ref connStg, queryMetadata.ToString());

                if (tables.Rows.Count == 0)
                {
                    SetTextStg("There was no metadata available to display Staging Area content. Please check the metadata schema (are there any Staging Area tables available?) or the database connection.");
                }

                foreach (DataRow row in tables.Rows)
                {
                    checkedListBoxStgMetadata.Items.Add(row["TABLE_NAME"]);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("There is no database connection! Please check the details in the information pane.","An issue has been encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _errorCounter++;
                //MessageBox.Show("There is no database connection! Please check the details in the information pane.","An issue has been encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _errorMessage.AppendLine("Unable to populate the Staging Area selection, there is no database connection.");
                _errorDetails.AppendLine("Error logging details: " + ex);
            }
        }

        private void PopulatePsaCheckboxList(SqlConnection connPsa)
        {
            // Clear the existing checkboxes
            checkedListBoxPsaMetadata.Items.Clear();

            try
            {

                // Query the metadata for the STG and HSTG tables
                var queryMetadata = new StringBuilder();

                queryMetadata.AppendLine(@"SELECT TABLE_SCHEMA,TABLE_NAME " +
                                         "FROM INFORMATION_SCHEMA.TABLES " +
                                         "WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME LIKE '%"+textBoxFilterCriterionPsa.Text+"%'" +
                                         "  AND TABLE_NAME LIKE '" + TeamConfigurationSettings.PsaTablePrefixValue + "_%'");


                var tables = GetDataTable(ref connPsa, queryMetadata.ToString());

                if (tables.Rows.Count == 0)
                {
                    SetTextPsa("There was no metadata available display Persisten Staging Area content. Please check the metadata schema (are there any PSA tables available?) or the database connection.");
                }

                foreach (DataRow row in tables.Rows)
                {
                    checkedListBoxPsaMetadata.Items.Add(row["TABLE_NAME"]);
                }
            }
            catch (Exception ex)
            {
                _errorCounter++;
                //MessageBox.Show("There is no database connection! Please check the details in the information pane.","An issue has been encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _errorMessage.AppendLine("Unable to populate the Persistent Staging Area selection, there is no database connection.");
                _errorDetails.AppendLine("Error logging details: " + ex);
            }
        }

        private void PopulateHubCheckboxList(SqlConnection connOmd)
        {
            // Clear the existing checkboxes
            checkedListBoxHubMetadata.Items.Clear();

            try
            {
                // Query the metadata for the STG and HSTG tables
                var queryMetadata = new StringBuilder();

                queryMetadata.AppendLine("SELECT DISTINCT HUB_NAME AS TABLE_NAME ");
                queryMetadata.AppendLine("FROM MD_HUB ");
                queryMetadata.AppendLine("WHERE HUB_NAME LIKE '%"+textBoxFilterCriterionHub.Text+"%'");
                queryMetadata.AppendLine("AND HUB_NAME != 'Not applicable'");
                queryMetadata.AppendLine("ORDER BY HUB_NAME");

                var tables = GetDataTable(ref connOmd, queryMetadata.ToString());

                if (tables.Rows.Count == 0)
                {
                    SetTextHub("There was no metadata available to display Hub content. Please check the metadata schema (are there any Hubs available?) or the database connection.");
                }

                foreach (DataRow row in tables.Rows)
                {
                    checkedListBoxHubMetadata.Items.Add(row["TABLE_NAME"]);
                }
            }
            catch (Exception ex)
            {
                _errorCounter++;
               // MessageBox.Show("There is no database connection! Please check the details in the information pane.","An issue has been encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _errorMessage.AppendLine("Unable to populate the Hub selection, there is no database connection.");
                _errorDetails.AppendLine("Verions error logging details: " + ex);
            }
        }

        private void PopulateSatCheckboxList(SqlConnection connOmd)
        {
            // Clear the existing checkboxes
            checkedListBoxSatMetadata.Items.Clear();

            try
            {

                // Query the metadata for the STG and HSTG tables
                var queryMetadata = new StringBuilder();

                queryMetadata.AppendLine("SELECT DISTINCT SATELLITE_NAME AS TABLE_NAME ");
                queryMetadata.AppendLine("FROM MD_SATELLITE ");
                queryMetadata.AppendLine("WHERE SATELLITE_TYPE='Normal' AND SATELLITE_NAME LIKE '%"+textBoxFilterCriterionSat.Text+"%'");
                queryMetadata.AppendLine("ORDER BY SATELLITE_NAME");

                var tables = GetDataTable(ref connOmd, queryMetadata.ToString());

                if (tables.Rows.Count == 0)
                {
                    SetTextSatOutput(
                        "There was no metadata available to display Satellite content. Please check the metadata schema (are there any Hubs available?) or the database connection.");
                }

                foreach (DataRow row in tables.Rows)
                {
                    checkedListBoxSatMetadata.Items.Add(row["TABLE_NAME"]);
                }
            }
            catch (Exception ex)
            {
                _errorCounter++;
                //MessageBox.Show("There is no database connection! Please check the details in the information pane.",
                // "An issue has been encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _errorMessage.AppendLine("Unable to populate the Satellite selection, there is no database connection.");
                _errorDetails.AppendLine("Error logging details: " + ex);
            }
        }

        private void PopulateLnkCheckboxList(SqlConnection connOmd)
        {
            // Clear the existing checkboxes
            checkedListBoxLinkMetadata.Items.Clear();

            try
            {
                // Query the metadata for the STG and HSTG tables
                var queryMetadata = new StringBuilder();

                queryMetadata.AppendLine("SELECT DISTINCT LINK_NAME AS TABLE_NAME ");
                queryMetadata.AppendLine("FROM MD_LINK ");
                queryMetadata.AppendLine("WHERE LINK_NAME LIKE '%"+textBoxFilterCriterionLnk.Text+"%'");
                queryMetadata.AppendLine("AND LINK_NAME != 'Not applicable'");
                queryMetadata.AppendLine("ORDER BY LINK_NAME");

                var tables = GetDataTable(ref connOmd, queryMetadata.ToString());

                if (tables.Rows.Count == 0)
                {
                    SetTextLink("There was no metadata available to display Link content. Please check the metadata schema (are there any Hubs available?) or the database connection.");
                }

                foreach (DataRow row in tables.Rows)
                {
                    checkedListBoxLinkMetadata.Items.Add(row["TABLE_NAME"]);
                }
            }
            catch (Exception ex)
            {
                _errorCounter++;
                // MessageBox.Show("There is no database connection! Please check the details in the information pane.",
                //  "An issue has been encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _errorMessage.AppendLine("Unable to populate the Link selection cannot be populated, there is no database connection.");
                _errorDetails.AppendLine("Error logging details: " + ex);
            }
        }

        private void PopulateLsatCheckboxList(SqlConnection connOmd)
        {
            // Clear the existing checkboxes
            checkedListBoxLsatMetadata.Items.Clear();

            try
            {
                // Query the metadata for the Link Satellite tables
                var queryMetadata = new StringBuilder();

                queryMetadata.AppendLine("SELECT DISTINCT SATELLITE_NAME AS TABLE_NAME ");
                queryMetadata.AppendLine("FROM MD_SATELLITE ");
                queryMetadata.AppendLine("WHERE SATELLITE_TYPE !='Normal' AND SATELLITE_NAME LIKE '%"+textBoxFilterCriterionLsat.Text+"%'");
                queryMetadata.AppendLine(" ORDER BY SATELLITE_NAME");
                
                var tables = GetDataTable(ref connOmd, queryMetadata.ToString());

                if (tables.Rows.Count == 0)
                {
                    SetTextLsat("There was no metadata available to display Link Satellite content. Please check the metadata schema (are there any Link Satellites available?) or the database connection.");
                }

                foreach (DataRow row in tables.Rows)
                {
                    checkedListBoxLsatMetadata.Items.Add(row["TABLE_NAME"]);
                }
            }
            catch (Exception exception)
            {
                _errorCounter++;
                //MessageBox.Show("There is no database connection! Please check the details in the information pane.",
                   // "An issue has been encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _errorMessage.AppendLine("Unable to populate the Link Satellite selection, there is not database connection.");
                _errorDetails.AppendLine("Error logging details: " + exception);
            }
        }

        private void checkBoxSelectAll_CheckedChanged_1(object sender, EventArgs e)
        {
            CheckAllHubValues();
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            CheckAllStgValues();
        }

        private void button_Repopulate_STG(object sender, EventArgs e)
        {
            

            var connStg = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringStg };
            _errorMessage.Clear();
            PopulateStgCheckboxList(connStg);
            if (_errorCounter > 0)
            {
                richTextBoxInformationMain.Clear();
                richTextBoxInformationMain.Text = _errorMessage.ToString();
            }
        }

        private void button_Repopulate_PSA(object sender, EventArgs e)
        {
            

            var connPsa = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringHstg };
            _errorMessage.Clear();
            PopulatePsaCheckboxList(connPsa);
            if (_errorCounter > 0)
            {
                richTextBoxInformationMain.Clear();
                richTextBoxInformationMain.Text = _errorMessage.ToString();
            }
        }

        private void button_Repopulate_Sat(object sender, EventArgs e)
        {
            

            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            _errorMessage.Clear();
            PopulateSatCheckboxList(connOmd);
            if (_errorCounter > 0)
            {
                richTextBoxInformationMain.Clear();
                richTextBoxInformationMain.Text = _errorMessage.ToString();
            }
        }

        private void button_Repopulate_Lnk(object sender, EventArgs e)
        {
            

            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            _errorMessage.Clear();
            PopulateLnkCheckboxList(connOmd);
            if (_errorCounter > 0)
            {
                richTextBoxInformationMain.Clear();
                richTextBoxInformationMain.Text = _errorMessage.ToString();
            }
        }

        private void button_Repopulate_LSAT(object sender, EventArgs e)
        {
            

            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            _errorMessage.Clear();
            PopulateLsatCheckboxList(connOmd);
            if (_errorCounter > 0)
            {
                richTextBoxInformationMain.Clear();
                richTextBoxInformationMain.Text = _errorMessage.ToString();
            }
        }

        private void checkBoxSelectAllPsa_CheckedChanged(object sender, EventArgs e)
        {
            CheckAllPsaValues();
        }

        private void checkBoxSelectAllSats_CheckedChanged(object sender, EventArgs e)
        {
            CheckAllSatValues();
        }

        private void checkBoxSelectAllLinks_CheckedChanged(object sender, EventArgs e)
        {
            CheckAllLinkValues();
        }

        private void checkBoxSelectAllLsats_CheckedChanged(object sender, EventArgs e)
        {
            CheckAllLsatValues();
        }


        private void CreateSchema(string connString)
        {
            var createStatement = new StringBuilder();

            createStatement.AppendLine("-- Creating the schema");
            createStatement.AppendLine("IF NOT EXISTS (");
            createStatement.AppendLine("SELECT SCHEMA_NAME");
            createStatement.AppendLine("FROM INFORMATION_SCHEMA.SCHEMATA");
            createStatement.AppendLine("WHERE SCHEMA_NAME = '" + VedwConfigurationSettings.VedwSchema + "')");
            createStatement.AppendLine("");
            createStatement.AppendLine("BEGIN");
            createStatement.AppendLine(" EXEC sp_executesql N'CREATE SCHEMA [" + VedwConfigurationSettings.VedwSchema + "]'");
            createStatement.AppendLine("END");

            using (var connectionVersion = new SqlConnection(connString))
            {
                var commandVersion = new SqlCommand(createStatement.ToString(), connectionVersion);

                try
                {
                    connectionVersion.Open();
                    commandVersion.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    SetTextMain("An issue occured creating the VEDW schema '" + VedwConfigurationSettings.VedwSchema + "'. The reported error is " + ex);
                }
            }
        }



        /// <summary>
        /// Save VEDW settings in the from to memory & disk
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveConfigurationFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Paths
            VedwConfigurationSettings.TeamConfigurationPath = textBoxConfigurationPath.Text;
            VedwConfigurationSettings.VedwOutputPath = textBoxOutputPath.Text;
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
                richTextBoxInformationMain.AppendText("An issue was encountered saving the Hash Disabling checkbox, can you verify the settings file in the Configuration directory?");
            }

            // Update the root path file (from memory)
            var rootPathConfigurationFile = new StringBuilder();
            rootPathConfigurationFile.AppendLine("/* Virtual Enterprise Data Warehouse (VEDW) Core Settings */");
            rootPathConfigurationFile.AppendLine("/* Saved at " + DateTime.Now + " */");
            rootPathConfigurationFile.AppendLine("TeamConfigurationPath|" + VedwConfigurationSettings.TeamConfigurationPath + "");
            rootPathConfigurationFile.AppendLine("VedwOutputPath|" + VedwConfigurationSettings.VedwOutputPath + "");
            rootPathConfigurationFile.AppendLine("EnableUnicode|"+ VedwConfigurationSettings.EnableUnicode+"");
            rootPathConfigurationFile.AppendLine("DisableHash|" + VedwConfigurationSettings.DisableHash + "");
            rootPathConfigurationFile.AppendLine("HashKeyOutputType|" + VedwConfigurationSettings.HashKeyOutputType + "");
            rootPathConfigurationFile.AppendLine("WorkingEnvironment|" + VedwConfigurationSettings.WorkingEnvironment + "");
            rootPathConfigurationFile.AppendLine("VedwSchema|" + VedwConfigurationSettings.VedwSchema + "");
            rootPathConfigurationFile.AppendLine("/* End of file */");

            // Save the VEDW core settings file to disk
            using (var outfile = new StreamWriter(GlobalParameters.VedwConfigurationPath + GlobalParameters.VedwConfigurationfileName + GlobalParameters.VedwFileExtension))
            {
                outfile.Write(rootPathConfigurationFile.ToString());
                outfile.Close();
            }

            // Reset the hash interpretation
            UpdateHashSnippets();

            // Reload the TEAM settings, as the environment may have changed
            LoadTeamConfigurationFile();

            // Reset / reload the checkbox lists
            SetDatabaseConnections();

            richTextBoxInformationMain.Text = "The global parameter file ("+GlobalParameters.VedwConfigurationfileName + GlobalParameters.VedwFileExtension+ ") has been updated in: " + GlobalParameters.VedwConfigurationPath;
        }

        private void openConfigurationDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(VedwConfigurationSettings.TeamConfigurationPath);
            }
            catch (Exception ex)
            {
                richTextBoxInformationMain.Text = "An error has occured while attempting to open the configuration directory. The error message is: " + ex;
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
                richTextBoxInformationMain.Text = "An error has occured while attempting to open the configuration directory. The error message is: " + ex;
            }
        }

        private void richTextBoxStaging_TextChanged(object sender, EventArgs e)
        {
            // Set the current caret position to the end
            richTextBoxStaging.SelectionStart = richTextBoxStaging.Text.Length;
            // Scroll automatically
            richTextBoxStaging.ScrollToCaret();
        }

        private void richTextBoxPSA_TextChanged(object sender, EventArgs e)
        {
            // Set the current caret position to the end
            richTextBoxPSA.SelectionStart = richTextBoxPSA.Text.Length;
            // Scroll automatically
            richTextBoxPSA.ScrollToCaret();
        }

        private void richTextBoxHub_TextChanged(object sender, EventArgs e)
        {
            // Set the current caret position to the end
            richTextBoxHub.SelectionStart = richTextBoxHub.Text.Length;
            // Scroll automatically
            richTextBoxHub.ScrollToCaret();
        }

        private void richTextBoxSat_TextChanged(object sender, EventArgs e)
        {
            // Set the current caret position to the end
            richTextBoxSat.SelectionStart = richTextBoxSat.Text.Length;
            // Scroll automatically
            richTextBoxSat.ScrollToCaret();
        }

        private void richTextBoxLink_TextChanged(object sender, EventArgs e)
        {
            // Set the current caret position to the end
            richTextBoxLink.SelectionStart = richTextBoxLink.Text.Length;
            // Scroll automatically
            richTextBoxLink.ScrollToCaret();
        }

        private void richTextBoxLsat_TextChanged(object sender, EventArgs e)
        {
            // Set the current caret position to the end
            richTextBoxLsat.SelectionStart = richTextBoxLsat.Text.Length;
            // Scroll automatically
            richTextBoxLsat.ScrollToCaret();
        }

        private void runEverythingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Select everything
            checkBoxSelectAllStg.Checked = true;
            checkBoxSelectAllPsa.Checked = true;
            checkBoxSelectAllHubs.Checked = true;
            checkBoxSelectAllSats.Checked = true;
            checkBoxSelectAllLsats.Checked = true;
            checkBoxSelectAllLinks.Checked = true;

            //Run Staging to Integration
            MainTabControl.SelectTab(0);
            buttonGenerateStaging.PerformClick();
            MainTabControl.SelectTab(1);
            buttonGeneratePSA.PerformClick();
            MainTabControl.SelectTab(2);
            buttonGenerateHubs.PerformClick();
            MainTabControl.SelectTab(3);
            buttonGenerateSats.PerformClick();
            MainTabControl.SelectTab(4);
            buttonGenerateLinks.PerformClick();
            MainTabControl.SelectTab(5);
            buttonGenerateLsats.PerformClick();
        }

        #region Load Combo Box events
        private void LoadStgPatternCombobox()
        {
            var patternCollection = new LoadPatternHandling();
            try
            {
                var patternList = patternCollection.DeserializeLoadPatternCollection();

                foreach (var patternDetail in patternList)
                {
                    if (patternDetail.LoadPatternType == "StagingArea")
                    {
                        comboBoxStgPattern.Items.Add(patternDetail.LoadPatternName);
                    }
                }
            }
            catch (Exception ex)
            {
                SetTextMain(ex.ToString());
            }

            comboBoxStgPattern.SelectedItem = comboBoxStgPattern.Items[0];
        }

        private void LoadPsaPatternCombobox()
        {
            var patternCollection = new LoadPatternHandling();
            try
            {
                var patternList = patternCollection.DeserializeLoadPatternCollection();

                foreach (var patternDetail in patternList)
                {
                    if (patternDetail.LoadPatternType == "PersistentStagingArea")
                    {
                        comboBoxPsaPattern.Items.Add(patternDetail.LoadPatternName);
                    }
                }
            }
            catch (Exception ex)
            {
                SetTextMain(ex.ToString());
            }

            comboBoxPsaPattern.SelectedItem = comboBoxPsaPattern.Items[0];
        }

        private void LoadHubPatternCombobox()
        {
            var patternCollection = new LoadPatternHandling();
            try
            {
                var patternList = patternCollection.DeserializeLoadPatternCollection();

                foreach (var patternDetail in patternList)
                {
                    if (patternDetail.LoadPatternType == "Hub")
                    {
                        comboBoxHubPattern.Items.Add(patternDetail.LoadPatternName);
                    }
                }
            }
            catch (Exception ex)
            {
                SetTextMain(ex.ToString());
            }

            comboBoxHubPattern.SelectedItem = comboBoxHubPattern.Items[0];
        }

        private void LoadSatPatternCombobox()
        {
            var patternCollection = new LoadPatternHandling();
            try
            {
                var patternList = patternCollection.DeserializeLoadPatternCollection();

                foreach (var patternDetail in patternList)
                {
                    if (patternDetail.LoadPatternType == "Satellite")
                    {
                        comboBoxSatPattern.Items.Add(patternDetail.LoadPatternName);
                    }
                }
            }
            catch (Exception ex)
            {
                SetTextMain(ex.ToString());
            }

            comboBoxSatPattern.SelectedItem = comboBoxSatPattern.Items[0];
        }

        private void LoadLinkPatternCombobox()
        {
            var patternCollection = new LoadPatternHandling();
            try
            {
                var patternList = patternCollection.DeserializeLoadPatternCollection();

                foreach (var patternDetail in patternList)
                {
                    if (patternDetail.LoadPatternType == "Link")
                    {
                        comboBoxLinkPattern.Items.Add(patternDetail.LoadPatternName);
                    }
                }
            }
            catch (Exception ex)
            {
                SetTextMain(ex.ToString());
            }

            comboBoxLinkPattern.SelectedItem = comboBoxLinkPattern.Items[0];
        }

        private void LoadLsatPatternCombobox()
        {
            var patternCollection = new LoadPatternHandling();
            try
            {
                var patternList = patternCollection.DeserializeLoadPatternCollection();

                foreach (var patternDetail in patternList)
                {
                    if (patternDetail.LoadPatternType == "LinkSatellite")
                    {
                        comboBoxLsatPattern.Items.Add(patternDetail.LoadPatternName);
                    }
                }
            }
            catch (Exception ex)
            {
                SetTextMain(ex.ToString());
            }

            comboBoxLsatPattern.SelectedItem = comboBoxLsatPattern.Items[0];
        }
        #endregion


        #region Tab Selected Index Changed events
        private void tabControlHub_SelectedIndexChanged(object sender, EventArgs e)
        {
            TextHandling.SyntaxHighlightHandlebars(richTextBoxHubPattern, richTextBoxHubPattern.Text.TrimEnd());
        }

        private void tabControlSat_SelectedIndexChanged(object sender, EventArgs e)
        {
            TextHandling.SyntaxHighlightHandlebars(richTextBoxSatPattern, richTextBoxSatPattern.Text.TrimEnd());
        }

        private void tabControlLink_SelectedIndexChanged(object sender, EventArgs e)
        {
            TextHandling.SyntaxHighlightHandlebars(richTextBoxLinkPattern, richTextBoxLinkPattern.Text.TrimEnd());
        }

        private void tabControlLsat_SelectedIndexChanged(object sender, EventArgs e)
        {
            TextHandling.SyntaxHighlightHandlebars(richTextBoxLsatPattern, richTextBoxLsatPattern.Text.TrimEnd());
        }

        private void tabControlPsa_SelectedIndexChanged(object sender, EventArgs e)
        {
            TextHandling.SyntaxHighlightHandlebars(richTextBoxPsaPattern, richTextBoxPsaPattern.Text.TrimEnd());
        }

        private void tabControlStg_SelectedIndexChanged(object sender, EventArgs e)
        {
            TextHandling.SyntaxHighlightHandlebars(richTextBoxStgPattern, richTextBoxStgPattern.Text.TrimEnd());
        }
        #endregion


        #region Combobox Selected Index Changed events
        private void comboBoxHubPattern_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Retrieve all the info for the pattern name from memory (from the list of patterns)
            var loadPattern = VedwConfigurationSettings.patternList.FirstOrDefault(o => o.LoadPatternName == comboBoxHubPattern.Text);

            // Set the label with the path so it's visible to the user where the file is located
            labelLoadPatternHubPath.Text = loadPattern.LoadPatternFilePath;

            // Read the file from the path
            var loadPatternTemplate = File.ReadAllText(loadPattern.LoadPatternFilePath).TrimEnd();

            // Display the pattern in the text box on the screen
            richTextBoxHubPattern.Text = loadPatternTemplate;

            // Make sure the pattern is stored in a global variable (memory) to overcome multi-threading issues
            LoadPattern.ActivateLoadPattern(loadPatternTemplate, loadPattern.LoadPatternType);

            // Only trigger changes when not in startup mode, otherwise the text will not load properly from file (too many changes)
            if (startUpIndicator == false)
            {
                // Syntax highlight for Handlebars
                TextHandling.SyntaxHighlightHandlebars(richTextBoxHubPattern, richTextBoxHubPattern.Text);
            }
        }

        private void comboBoxSatPattern_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Retrieve all the info for the pattern name from memory (from the list of patterns)
            var loadPattern = VedwConfigurationSettings.patternList.FirstOrDefault(o => o.LoadPatternName == comboBoxSatPattern.Text);

            // Set the label with the path so it's visible to the user where the file is located
            labelLoadPatternSatPath.Text = loadPattern.LoadPatternFilePath;

            // Read the file from the path
            var loadPatternTemplate = File.ReadAllText(loadPattern.LoadPatternFilePath);

            // Display the pattern in the text box on the screen
            richTextBoxSatPattern.Text = loadPatternTemplate;

            // Make sure the pattern is stored in a global variable (memory) to overcome multi-threading issues
            LoadPattern.ActivateLoadPattern(loadPatternTemplate, loadPattern.LoadPatternType);

            // Only trigger changes when not in startup mode, otherwise the text will not load properly from file (too many changes)
            if (startUpIndicator == false)
            {
                // Syntax highlight for Handlebars
                TextHandling.SyntaxHighlightHandlebars(richTextBoxSatPattern, richTextBoxSatPattern.Text);
            }
        }

        private void comboBoxLsatPattern_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Retrieve all the info for the pattern name from memory (from the list of patterns)
            var loadPattern = VedwConfigurationSettings.patternList.FirstOrDefault(o => o.LoadPatternName == comboBoxLsatPattern.Text);

            // Set the label with the path so it's visible to the user where the file is located
            labelLoadPatternLsatPath.Text = loadPattern.LoadPatternFilePath;

            // Read the file from the path
            var loadPatternTemplate = File.ReadAllText(loadPattern.LoadPatternFilePath);

            // Display the pattern in the text box on the screen
            richTextBoxLsatPattern.Text = loadPatternTemplate;

            // Make sure the pattern is stored in a global variable (memory) to overcome multithreading issues
            LoadPattern.ActivateLoadPattern(loadPatternTemplate, loadPattern.LoadPatternType);

            // Only trigger changes when not in startup mode, otherwise the text will not load properly from file (too many changes)
            if (startUpIndicator == false)
            {
                // Syntax highlight for Handlebars
                TextHandling.SyntaxHighlightHandlebars(richTextBoxLsatPattern, richTextBoxLsatPattern.Text);
            }
        }

        private void comboBoxLinkPattern_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Retrieve all the info for the pattern name from memory (from the list of patterns)
            var loadPattern = VedwConfigurationSettings.patternList.FirstOrDefault(o => o.LoadPatternName == comboBoxLinkPattern.Text);

            // Set the label with the path so it's visible to the user where the file is located
            labelLoadPatternLinkPath.Text = loadPattern.LoadPatternFilePath;

            // Read the file from the path
            var loadPatternTemplate = File.ReadAllText(loadPattern.LoadPatternFilePath);

            // Display the pattern in the text box on the screen
            richTextBoxLinkPattern.Text = loadPatternTemplate;

            // Make sure the pattern is stored in a global variable (memory) to overcome multithreading issues
            LoadPattern.ActivateLoadPattern(loadPatternTemplate, loadPattern.LoadPatternType);

            // Only trigger changes when not in startup mode, otherwise the text will not load properly from file (too many changes)
            if (startUpIndicator == false)
            {
                // Syntax highlight for Handlebars
                TextHandling.SyntaxHighlightHandlebars(richTextBoxLinkPattern, richTextBoxLinkPattern.Text);
            }
        }

        private void comboBoxPsaPattern_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Retrieve all the info for the pattern name from memory (from the list of patterns)
            var loadPattern = VedwConfigurationSettings.patternList.FirstOrDefault(o => o.LoadPatternName == comboBoxPsaPattern.Text);

            // Set the label with the path so it's visible to the user where the file is located
            labelLoadPatternPsaPath.Text = loadPattern.LoadPatternFilePath;

            // Read the file from the path
            var loadPatternTemplate = File.ReadAllText(loadPattern.LoadPatternFilePath);

            // Display the pattern in the text box on the screen
            richTextBoxPsaPattern.Text = loadPatternTemplate;

            // Make sure the pattern is stored in a global variable (memory) to overcome multithreading issues
            LoadPattern.ActivateLoadPattern(loadPatternTemplate, loadPattern.LoadPatternType);

            // Only trigger changes when not in startup mode, otherwise the text will not load properly from file (too many changes)
            if (startUpIndicator == false)
            {
                // Syntax highlight for Handlebars
                TextHandling.SyntaxHighlightHandlebars(richTextBoxPsaPattern, richTextBoxPsaPattern.Text);
            }
        }

        private void comboBoxStgPattern_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Retrieve all the info for the pattern name from memory (from the list of patterns)
            var loadPattern = VedwConfigurationSettings.patternList.FirstOrDefault(o => o.LoadPatternName == comboBoxStgPattern.Text);

            // Set the label with the path so it's visible to the user where the file is located
            labelLoadPatternStgPath.Text = loadPattern.LoadPatternFilePath;

            // Read the file from the path
            var loadPatternTemplate = File.ReadAllText(loadPattern.LoadPatternFilePath);

            // Display the pattern in the text box on the screen
            richTextBoxStgPattern.Text = loadPatternTemplate;

            // Make sure the pattern is stored in a global variable (memory) to overcome multithreading issues
            LoadPattern.ActivateLoadPattern(loadPatternTemplate, loadPattern.LoadPatternType);

            // Only trigger changes when not in startup mode, otherwise the text will not load properly from file (too many changes)
            if (startUpIndicator == false)
            {
                // Syntax highlight for Handlebars
                TextHandling.SyntaxHighlightHandlebars(richTextBoxStgPattern, richTextBoxStgPattern.Text);
            }
        }
        #endregion


        #region SaveButtons
        private void buttonHubSavePattern_Click(object sender, EventArgs e)
        {
            richTextBoxInformationMain.Clear();
            string backupResponse = LoadPattern.BackupLoadPattern(labelLoadPatternHubPath.Text);
            string saveResponse = "";

            if (backupResponse.StartsWith("A backup was created"))
            {
                SetTextMain(backupResponse);
                saveResponse = LoadPattern.SaveLoadPattern(labelLoadPatternHubPath.Text, richTextBoxHubPattern.Text);
                SetTextMain("\r\n\r\n" + saveResponse);
            }
            else
            {
                SetTextMain(backupResponse);
                SetTextMain(saveResponse);
            }
        }

        private void buttonSatSavePattern_Click(object sender, EventArgs e)
        {
            richTextBoxInformationMain.Clear();
            string backupResponse = LoadPattern.BackupLoadPattern(labelLoadPatternSatPath.Text);
            string saveResponse = "";

            if (backupResponse.StartsWith("A backup was created"))
            {
                SetTextMain(backupResponse);
                saveResponse = LoadPattern.SaveLoadPattern(labelLoadPatternSatPath.Text, richTextBoxSatPattern.Text);
                SetTextMain("\r\n\r\n" + saveResponse);
            }
            else
            {
                SetTextMain(backupResponse);
                SetTextMain(saveResponse);
            }
        }

        private void ButtonStgSavePattern_Click(object sender, EventArgs e)
        {
            richTextBoxInformationMain.Clear();
            string backupResponse = LoadPattern.BackupLoadPattern(labelLoadPatternStgPath.Text);
            string saveResponse = "";

            if (backupResponse.StartsWith("A backup was created"))
            {
                SetTextMain(backupResponse);
                saveResponse = LoadPattern.SaveLoadPattern(labelLoadPatternStgPath.Text, richTextBoxStgPattern.Text);
                SetTextMain("\r\n\r\n" + saveResponse);
            }
            else
            {
                SetTextMain(backupResponse);
                SetTextMain(saveResponse);
            }
        }

        private void buttonPsaSavePattern_Click(object sender, EventArgs e)
        {
            richTextBoxInformationMain.Clear();
            string backupResponse = LoadPattern.BackupLoadPattern(labelLoadPatternPsaPath.Text);
            string saveResponse = "";

            if (backupResponse.StartsWith("A backup was created"))
            {
                SetTextMain(backupResponse);
                saveResponse = LoadPattern.SaveLoadPattern(labelLoadPatternPsaPath.Text, richTextBoxPsaPattern.Text);
                SetTextMain("\r\n\r\n" + saveResponse);
            }
            else
            {
                SetTextMain(backupResponse);
                SetTextMain(saveResponse);
            }
        }

        private void buttonLnkSavePattern_Click(object sender, EventArgs e)
        {
            richTextBoxInformationMain.Clear();
            string backupResponse = LoadPattern.BackupLoadPattern(labelLoadPatternLinkPath.Text);
            string saveResponse = "";

            if (backupResponse.StartsWith("A backup was created"))
            {
                SetTextMain(backupResponse);
                saveResponse = LoadPattern.SaveLoadPattern(labelLoadPatternLinkPath.Text, richTextBoxLinkPattern.Text);
                SetTextMain("\r\n\r\n" + saveResponse);
            }
            else
            {
                SetTextMain(backupResponse);
                SetTextMain(saveResponse);
            }
        }

        private void buttonLsatSavePattern_Click(object sender, EventArgs e)
        {
            richTextBoxInformationMain.Clear();
            string backupResponse = LoadPattern.BackupLoadPattern(labelLoadPatternLsatPath.Text);
            string saveResponse = "";

            if (backupResponse.StartsWith("A backup was created"))
            {
                SetTextMain(backupResponse);
                saveResponse = LoadPattern.SaveLoadPattern(labelLoadPatternLsatPath.Text, richTextBoxLsatPattern.Text);
                SetTextMain("\r\n\r\n" + saveResponse);
            }
            else
            {
                SetTextMain(backupResponse);
                SetTextMain(saveResponse);
            }
        }
        #endregion
        

        #region TextChanged event
        private void richTextBoxHubPattern_TextChanged(object sender, EventArgs e)
        {
            // Make sure the pattern is stored in a global variable (memory) to overcome multi-threading issues
            LoadPattern.ActivateLoadPattern(richTextBoxHubPattern.Text.TrimEnd(), "Hub");
        }

        private void richTextBoxSatPattern_TextChanged(object sender, EventArgs e)
        {
            LoadPattern.ActivateLoadPattern(richTextBoxSatPattern.Text.TrimEnd(), "Satellite");
        }

        private void richTextBoxLinkPattern_TextChanged(object sender, EventArgs e)
        {
            // Make sure the pattern is stored in a global variable (memory) to overcome multi-threading issues
            LoadPattern.ActivateLoadPattern(richTextBoxLinkPattern.Text.TrimEnd(), "Link");
        }

        private void richTextBoxStgPattern_TextChanged(object sender, EventArgs e)
        {
            // Make sure the pattern is stored in a global variable (memory) to overcome multi-threading issues
            LoadPattern.ActivateLoadPattern(richTextBoxStgPattern.Text.TrimEnd(), "StagingArea");
        }

        private void richTextBoxPsaPattern_TextChanged(object sender, EventArgs e)
        {
            // Make sure the pattern is stored in a global variable (memory) to overcome multi-threading issues
            LoadPattern.ActivateLoadPattern(richTextBoxPsaPattern.Text.TrimEnd(), "PersistentStagingArea");
        }

        private void richTextBoxLsatPattern_TextChanged(object sender, EventArgs e)
        {
            // Make sure the pattern is stored in a global variable (memory) to overcome multi-threading issues
            LoadPattern.ActivateLoadPattern(richTextBoxLsatPattern.Text.TrimEnd(), "LinkSatellite");
        }
        #endregion

        private void richTextBoxInformationMain_TextChanged(object sender, EventArgs e)
        {
            // Set the current caret position to the end
            richTextBoxInformationMain.SelectionStart = richTextBoxStaging.Text.Length;
            // Scroll automatically
            richTextBoxInformationMain.ScrollToCaret();
        }
    }
}
