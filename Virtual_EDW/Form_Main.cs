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
using System.Security.Permissions;

namespace Virtual_EDW
{
    public partial class FormMain : FormBase
    {
        Form_Alert _alert;
        private StringBuilder _errorMessage;
        private StringBuilder _errorDetails;
        private int _errorCounter;

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

            richTextBoxInformation.AppendText("Application initialised - welcome to the Virtual Data Warehouse! \r\n\r\n");

            checkBoxGenerateInDatabase.Checked = false;
            checkBoxIfExistsStatement.Checked = true;
            radiobuttonViews.Checked = true;
            SQL2014Radiobutton.Checked = true;
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
        }

        private void SetDatabaseConnections()
        {
            var connOmd = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringOmd};
            var connStg = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringStg};
            var connPsa = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringHstg};

            ClearStgCheckBoxList();
            ClearPsaCheckBoxList();
            ClearHubCheckBoxList();
            ClearLinkCheckBoxList();
            ClearSatCheckBoxList();
            ClearLsatCheckBoxList();

            try
            {
                connOmd.Open();

                InitialiseVersion();
                PopulateHubCheckboxList(connOmd);
                PopulateLnkCheckboxList(connOmd);
                PopulateSatCheckboxList(connOmd);
                PopulateLsatCheckboxList(connOmd);
            }
            catch
            {
                richTextBoxInformation.AppendText(
                    "There was an issue establishing a database connection to the Metadata Repository Database. These are managed via the TEAM configuration files. The reported database connection string is '" +
                    TeamConfigurationSettings.ConnectionStringOmd + "'.\r\n");
            }

            try
            {
                connStg.Open();
                PopulateStgCheckboxList(connStg);
            }
            catch
            {
                richTextBoxInformation.AppendText(
                    "There was an issue establishing a database connection to the Staging Area Database. These are managed via the TEAM configuration files. The reported database connection string is '" +
                    TeamConfigurationSettings.ConnectionStringOmd + "'.\r\n");
            }

            try
            {
                connPsa.Open();
                PopulatePsaCheckboxList(connPsa);
            }
            catch
            {
                richTextBoxInformation.AppendText(
                    "There was an issue establishing a database connection to the Persistent Staging Area (PSA) Database. These are managed via the TEAM configuration files. The reported database connection string is '" +
                    TeamConfigurationSettings.ConnectionStringOmd + "'.\r\n");
            }

            if (_errorCounter > 0)
            {
                richTextBoxInformation.AppendText(_errorMessage.ToString());
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

            richTextBoxInformation.Text = "Retrieving TEAM configuration details from '" + teamConfigurationFileName + "'. \r\n\r\n";

            var teamConfigResult = EnvironmentConfiguration.LoadTeamConfigurationFile(teamConfigurationFileName);

            if (teamConfigResult.Length > 0)
            {
                richTextBoxInformation.AppendText(
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
                richTextBoxInformation.AppendText("An issue was encountered updating the Hash output setting on the application - please verify.");
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

            // Unicode checkbox
            if (VedwConfigurationSettings.EnableUnicode == "True")
            {
                checkBoxUnicode.Checked = true;
            }
            else if (VedwConfigurationSettings.DisableHash == "False")
            {
                checkBoxDisableHash.Checked = false;
            }
            else
            {
                richTextBoxInformation.AppendText("An issue was encountered updating the Unicode setting on the application - please verify.");
            }

            // Hash key vs natural key checkbox
            if (VedwConfigurationSettings.DisableHash == "True")
            {
                checkBoxDisableHash.Checked = true;
            }
            else if (VedwConfigurationSettings.DisableHash == "False")
            {
                checkBoxDisableHash.Checked = false;
            }
            else
            {
                richTextBoxInformation.AppendText("An issue was encountered updating the Unicode setting on the application - please verify.");
            }

            // Hash key output radiobutton
            if (VedwConfigurationSettings.HashKeyOutputType == "Binary")
            {
                radioButtonBinaryHash.Checked = true;
            }
            else if (VedwConfigurationSettings.HashKeyOutputType == "Character")
            {
                radioButtonCharacterHash.Checked = true;
            }
            else
            {
                richTextBoxInformation.AppendText(
                    "An issue was encountered updating the Hash output setting on the application - please verify.");
            }

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
                richTextBoxInformation.AppendText("An issue was encountered updating the Hash outpu setting on the application - please verify.");
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
            if (richTextBoxInformation.Text.Contains(word))
            {
                int index = -1;
                int selectStart = richTextBoxInformation.SelectionStart;

                while ((index = richTextBoxInformation.Text.IndexOf(word, (index + 1), StringComparison.Ordinal)) != -1)
                {
                    richTextBoxInformation.Select((index + startIndex), word.Length);
                    richTextBoxInformation.SelectionColor = color;
                    richTextBoxInformation.Select(selectStart, 0);
                    richTextBoxInformation.SelectionColor = Color.Black;
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
            richTextBoxInformation.Clear();

            var newThread = new Thread(BackgroundDoHub);
            newThread.Start();
        }

        private void BackgroundDoHub()
        {
            // Check if the schema needs to be created
            CreateSchema(TeamConfigurationSettings.ConnectionStringInt);

            if (radiobuttonViews.Checked) // Views
            {
                GenerateHubViews();
            }
            else if (radiobuttonStoredProc.Checked)
            {

            }
            else if (radioButtonIntoStatement.Checked) // Insert into
            {
                GenerateHubInsertInto();
            }
        }

        private void GenerateHubInsertInto()
        {
            int errorCounter = 0;

            if (checkBoxIfExistsStatement.Checked)
            {
                SetTextHub("The Drop If Exists checkbox has been checked, but this feature is not relevant for this specific operation and will be ignored. \n\r");
            }

            // Determine metadata retrieval connection (dependent on option selected)
            if (checkedListBoxHubMetadata.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= checkedListBoxHubMetadata.CheckedItems.Count - 1; x++)
                {
                    var connHstg = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringHstg };
                    var conn = new SqlConnection
                    {
                        ConnectionString = checkBoxIgnoreVersion.Checked ? TeamConfigurationSettings.ConnectionStringInt : TeamConfigurationSettings.ConnectionStringOmd
                    };

                    var hubTableName = checkedListBoxHubMetadata.CheckedItems[x].ToString();
                    var hubSk = hubTableName.Substring(4) + "_" + TeamConfigurationSettings.DwhKeyIdentifier;

                    // Build the main attribute list of the Hub table for selection
                    var sourceTableStructure = GetTableStructure(hubTableName, ref conn, "HUB");

                    // Initial SQL
                    var insertIntoStatement = new StringBuilder();

                    insertIntoStatement.AppendLine("--");
                    insertIntoStatement.AppendLine("-- Hub Insert Into statement for " + hubTableName);
                    insertIntoStatement.AppendLine("-- Generated at " + DateTime.Now);
                    insertIntoStatement.AppendLine("--");
                    insertIntoStatement.AppendLine();
                    insertIntoStatement.AppendLine("USE [" + TeamConfigurationSettings.PsaDatabaseName + "]");
                    insertIntoStatement.AppendLine("GO");
                    insertIntoStatement.AppendLine();
                    insertIntoStatement.AppendLine("INSERT INTO " + TeamConfigurationSettings.IntegrationDatabaseName + "." +TeamConfigurationSettings.SchemaName + "." + hubTableName);
                    insertIntoStatement.AppendLine("(");
                    
                    foreach (DataRow attribute in sourceTableStructure.Rows)
                    {
                        insertIntoStatement.AppendLine("   [" + attribute["COLUMN_NAME"] + "],");
                    }

                    insertIntoStatement.Remove(insertIntoStatement.Length - 3, 3);
                    insertIntoStatement.AppendLine();
                    insertIntoStatement.AppendLine(")");
                    insertIntoStatement.AppendLine("SELECT ");

                    foreach (DataRow attribute in sourceTableStructure.Rows)
                    {
                        if ((string)attribute["COLUMN_NAME"] == TeamConfigurationSettings.EtlProcessAttribute)
                        {
                            insertIntoStatement.AppendLine("   -1 AS " + attribute["COLUMN_NAME"] + ",");
                        }
                        else if ((string)attribute["COLUMN_NAME"] == TeamConfigurationSettings.AlternativeRecordSourceAttribute)
                        {
                            insertIntoStatement.AppendLine("   CHECKSUM(hub_view." + TeamConfigurationSettings.AlternativeRecordSourceAttribute + ") AS " + attribute["COLUMN_NAME"] + ",");
                        }
                        else
                        {
                            insertIntoStatement.AppendLine("   hub_view.[" + attribute["COLUMN_NAME"] + "],");
                        }


                    }

                    insertIntoStatement.Remove(insertIntoStatement.Length - 3, 3);
                    insertIntoStatement.AppendLine();
                    insertIntoStatement.AppendLine("FROM [" + VedwConfigurationSettings.VedwSchema + "]." + hubTableName + " hub_view");
                    insertIntoStatement.AppendLine("LEFT OUTER JOIN ");
                    insertIntoStatement.AppendLine(" " + TeamConfigurationSettings.IntegrationDatabaseName + "." +TeamConfigurationSettings.SchemaName +
                                                    "." + hubTableName + " hub_table");
                    insertIntoStatement.AppendLine(" ON hub_view." + hubSk + " = hub_table." + hubSk);
                    insertIntoStatement.AppendLine("WHERE hub_table." + hubSk + " IS NULL");

                    using (var outfile = new StreamWriter(textBoxOutputPath.Text + @"\INSERT_STATEMENT_" + hubTableName +".sql"))
                    {
                        outfile.Write(insertIntoStatement.ToString());
                        outfile.Close();
                    }

                    if (checkBoxGenerateInDatabase.Checked)
                    {
                        int insertError = GenerateInDatabase(connHstg, insertIntoStatement.ToString());
                        errorCounter = errorCounter + insertError;

                    }

                    SetTextDebug(insertIntoStatement.ToString());
                    SetTextDebug("\n");

                    SetTextHub($"Processing Hub Insert Into statement for {hubTableName}\r\n");

                    connHstg.Close();
                    conn.Close();
                }
            }
            else
            {
                SetTextHub("There was no metadata selected to create Hub insert statements. Please check the metadata schema - are there any Hubs selected?");
            }

            SetTextHub($"\r\n{errorCounter} errors have been found.\r\n");
            SetTextHub($"SQL Scripts have been successfully saved in {VedwConfigurationSettings.VedwOutputPath}.\r\n");
        }

        internal List<String> GetHubClauses(string stagingAreaTableName, string hubTableName, string businessKeyDefinition, string groupCounter)
        {
            var fieldList = new StringBuilder();
            var compositeKey = new StringBuilder();
            var fieldDict = new Dictionary<string, string>();
            var fieldOrderedList = new List<string>();
            // Retrieving the business key attributes for the Hub                 
            var hubKeyList = GetHubTargetBusinessKeyList(hubTableName);

            var hubQuerySelect = new StringBuilder();
            var hubQueryWhere = new StringBuilder();
            var hubQueryGroupBy = new StringBuilder();

            string firstKey;
            var sqlStatementForSourceQuery = new StringBuilder();

            // Depending on the ignore version the connection is set.
            var conn = checkBoxIgnoreVersion.Checked ? new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringStg } : new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };

            var stringDataType = checkBoxUnicode.Checked ? "NVARCHAR" : "VARCHAR";

            // For every STG / Hub relationship, the business key needs to be defined - starting with the components of the key
            var componentList = GetBusinessKeyComponentList(stagingAreaTableName, hubTableName, businessKeyDefinition);

            foreach (DataRow component in componentList.Rows)
            {
                var componentId = (int)component["BUSINESS_KEY_COMPONENT_ID"] - 1;

                // Retrieve the elements of each business key component
                // This only concerns concatenated keys as they are single component keys comprising of multiple elements.
                var elementList = GetBusinessKeyElements(stagingAreaTableName, hubTableName, businessKeyDefinition, (int)component["BUSINESS_KEY_COMPONENT_ID"]);

                if (elementList == null)
                {
                    SetTextDebug("\n");
                    SetTextHub($"An error occurred for the Hub Insert Into statement for {hubTableName}. The collection of Business Keys is empty.\r\n");
                }
                else
                {
                    if (elementList.Rows.Count > 1) // Build a concatenated key if the count of elements is greater than 1 for a component (key part)
                    {
                        fieldList.Clear();
                        fieldDict.Clear();

                        foreach (DataRow element in elementList.Rows)
                        {
                            var elementType = element["BUSINESS_KEY_COMPONENT_ELEMENT_TYPE"].ToString();

                            if (elementType == "Attribute")
                            {
                                fieldList.Append("'" + element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] + "',");

                                if (checkBoxIgnoreVersion.Checked)
                                {
                                    // Make sure the live database is hit when the checkbox is ticked
                                    sqlStatementForSourceQuery.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION");
                                    sqlStatementForSourceQuery.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS");
                                    sqlStatementForSourceQuery.AppendLine("WHERE TABLE_NAME= '" + stagingAreaTableName + "'");
                                    sqlStatementForSourceQuery.AppendLine("AND TABLE_SCHEMA = '" + TeamConfigurationSettings.SchemaName + "'");
                                    sqlStatementForSourceQuery.AppendLine("AND COLUMN_NAME IN (" + fieldList.ToString().Substring(0, fieldList.ToString().Length - 1) + ")");
                                }
                                else
                                {
                                    //Ignore version is not checked, so versioning is used based on the activated metadata
                                    sqlStatementForSourceQuery.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION");
                                    sqlStatementForSourceQuery.AppendLine("FROM [interface].[INTERFACE_PHYSICAL_MODEL]");
                                    sqlStatementForSourceQuery.AppendLine("WHERE [TABLE_NAME] = '" + stagingAreaTableName + "'");
                                    sqlStatementForSourceQuery.AppendLine("AND [SCHEMA_NAME] = '" + TeamConfigurationSettings.SchemaName + "'");
                                    sqlStatementForSourceQuery.AppendLine("AND COLUMN_NAME IN (" + fieldList.ToString().Substring(0, fieldList.ToString().Length - 1) + ")");
                                }

                                var elementDataTypes = GetDataTable(ref conn, sqlStatementForSourceQuery.ToString()) ;

                                foreach (DataRow attribute in elementDataTypes.Rows)
                                {
                                    fieldDict.Add(attribute["COLUMN_NAME"].ToString(), attribute["DATA_TYPE"].ToString());
                                }
                            }
                            else if (elementType == "User Defined Value")
                            {
                                fieldList.Append("''" + element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] + "'',");
                            }

                            fieldOrderedList.Add(element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"].ToString());
                        }


                        // Build the concatenated key
                        foreach (var busKey in fieldOrderedList)
                        {
                            if (fieldDict.ContainsKey(busKey))
                            {
                                var key = "ISNULL([" + busKey + "], '')";

                                if ((fieldDict[busKey] == "datetime2") || (fieldDict[busKey] == "datetime"))
                                {
                                    key = "CASE WHEN [" + busKey + "] IS NOT NULL THEN CONVERT(" + stringDataType + "(100), [" + busKey + "], 112) ELSE '' END";
                                }
                                else if ((fieldDict[busKey] == "numeric") || (fieldDict[busKey] == "integer") || (fieldDict[busKey] == "int") || (fieldDict[busKey] == "tinyint") || (fieldDict[busKey] == "decimal"))
                                {
                                    key = "CASE WHEN [" + busKey + "] IS NOT NULL THEN CAST([" + busKey + "] AS " + stringDataType + "(100)) ELSE '' END";
                                }

                                compositeKey.Append(key).Append(" + ");
                            }
                            else
                            {
                                var key = " " + busKey;
                                compositeKey.Append(key).Append(" + ");
                            }
                        }

                        hubQuerySelect.AppendLine(compositeKey.ToString().Substring(0, compositeKey.ToString().Length - 2) + " AS [" + hubKeyList.Rows[componentId]["COLUMN_NAME"] + groupCounter + "],");
                        hubQueryWhere.AppendLine(compositeKey.ToString().Substring(0, compositeKey.ToString().Length - 2) + " != '' AND");
                        hubQueryGroupBy.AppendLine(compositeKey.ToString().Substring(0, compositeKey.ToString().Length - 2) + ",");
                    }
                    else // Handle a component of a single or composite key 
                    {
                        foreach (DataRow element in elementList.Rows) // Only a single element...
                        {
                            if (element["BUSINESS_KEY_COMPONENT_ELEMENT_TYPE"].ToString() == "User Defined Value")
                            {
                                firstKey = element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"].ToString();
                                hubQuerySelect.AppendLine("    " + firstKey + " AS [" + hubKeyList.Rows[componentId]["COLUMN_NAME"] + groupCounter + "],");
                            }
                            else // It's a normal attribute
                            {
                                // We need the data type again

                                if (checkBoxIgnoreVersion.Checked)
                                {
                                    sqlStatementForSourceQuery.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION");
                                    sqlStatementForSourceQuery.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS");
                                    sqlStatementForSourceQuery.AppendLine("WHERE TABLE_NAME= '" + stagingAreaTableName + "'");
                                    sqlStatementForSourceQuery.AppendLine("AND TABLE_SCHEMA = '" + TeamConfigurationSettings.SchemaName + "'");
                                    sqlStatementForSourceQuery.AppendLine("AND COLUMN_NAME = ('" + element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] + "')");
                                }
                                else
                                {
                                    //Ignore version is not checked, so versioning is used based on the activated metadata
                                    sqlStatementForSourceQuery.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION");
                                    sqlStatementForSourceQuery.AppendLine("FROM [interface].[INTERFACE_PHYSICAL_MODEL]");
                                    sqlStatementForSourceQuery.AppendLine("WHERE TABLE_NAME= '" + stagingAreaTableName + "'");
                                    sqlStatementForSourceQuery.AppendLine("AND SCHEMA_NAME = '" + TeamConfigurationSettings.SchemaName + "'");
                                    sqlStatementForSourceQuery.AppendLine("AND COLUMN_NAME = ('" + element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] + "')");
                              }



                                firstKey = "[" + element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] + "]";

                                var elementDataTypes = GetDataTable(ref conn, sqlStatementForSourceQuery.ToString());

                                foreach (DataRow attribute in elementDataTypes.Rows)
                                {
                                    if (attribute["DATA_TYPE"].ToString() == "numeric" || attribute["DATA_TYPE"].ToString() == "int")
                                    {
                                        hubQuerySelect.AppendLine("    CAST(" + firstKey + " AS " + stringDataType + "(100)) AS [" + hubKeyList.Rows[componentId]["COLUMN_NAME"] + groupCounter + "],");
                                    }
                                    else
                                    {
                                        hubQuerySelect.AppendLine("      " + firstKey + " AS [" + hubKeyList.Rows[componentId]["COLUMN_NAME"] + groupCounter + "],");
                                    }
                                }

                                hubQueryWhere.AppendLine(" " + firstKey + " IS NOT NULL AND");
                                hubQueryGroupBy.AppendLine("    " + firstKey + ",");
                            }

                        } // End of element loop (for single element)
                    }
                }
            } // End of component elements

            // Return the results
            var outputList = new List<String>();

            outputList.Add(hubQuerySelect.ToString());
            outputList.Add(hubQueryWhere.ToString());
            outputList.Add(hubQueryGroupBy.ToString());

            return outputList;
        }

        private void GenerateHubViews()
        {
            int errorCounter = 0;

            // Create the Hub views - representing the Hub entity
            var connOmd = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringOmd};
            var connStg = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringStg};
            var connPsa = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringHstg};
            var connInt = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringInt};

            if (checkedListBoxHubMetadata.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= checkedListBoxHubMetadata.CheckedItems.Count - 1; x++)
                {
                    var hubView = new StringBuilder();
                    var hubTableName = checkedListBoxHubMetadata.CheckedItems[x].ToString();
                    var hubSk = hubTableName.Substring(4) + "_" + TeamConfigurationSettings.DwhKeyIdentifier;

                    var stringDataType = checkBoxUnicode.Checked ? "NVARCHAR" : "VARCHAR";

                    // Retrieving the business key attributes for the Hub                 
                    var hubKeyList = GetHubTargetBusinessKeyList(hubTableName);

                    // Initial SQL
                    hubView.AppendLine("--");
                    hubView.AppendLine("-- Hub View definition for " + hubTableName);
                    hubView.AppendLine("-- Generated at " + DateTime.Now);
                    hubView.AppendLine("--");
                    hubView.AppendLine();
                    hubView.AppendLine("USE [" + TeamConfigurationSettings.PsaDatabaseName + "]");
                    hubView.AppendLine("GO");
                    hubView.AppendLine();
                    hubView.AppendLine("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[" + VedwConfigurationSettings.VedwSchema + "].[" + hubTableName +"]') AND type in (N'V'))");
                    hubView.AppendLine("DROP VIEW [" + VedwConfigurationSettings.VedwSchema + "].[" + hubTableName + "]");
                    hubView.AppendLine("GO");
                    hubView.AppendLine("CREATE VIEW [" + VedwConfigurationSettings.VedwSchema + "].[" + hubTableName + "] AS  ");

                    // START OF MAIN QUERY
                    hubView.AppendLine("SELECT hub.*");
                    hubView.AppendLine("FROM(");
                    hubView.AppendLine("SELECT");

                    //Replace hash value with concatenated business key value (experimental)
                    if (!checkBoxDisableHash.Checked)
                    {
                        //Regular Hash
                        hubView.AppendLine("  "+VedwConfigurationSettings.hashingStartSnippet);

                        foreach (DataRow hubKey in hubKeyList.Rows)
                        {
                            hubView.AppendLine("    ISNULL(RTRIM(CONVERT(" + stringDataType + "(100),[" + (string)hubKey["COLUMN_NAME"] + "])),'NA')+'|'+");
                        }
                        hubView.Remove(hubView.Length - 3, 3);
                        hubView.AppendLine();

                        hubView.AppendLine("  "+VedwConfigurationSettings.hashingEndSnippet+" AS " + hubSk + ",");

                    }
                    else
                    {
                        //BK as DWH key
                        foreach (DataRow hubKey in hubKeyList.Rows)
                        {
                            hubView.Append("  ISNULL(RTRIM(CONVERT(" + stringDataType + "(100),[" + (string)hubKey["COLUMN_NAME"] + "])),'NA')+'|'+");
                        }
                        hubView.Remove(hubView.Length - 5, 5);
                        hubView.Append("  AS " + hubSk + ",");
                        hubView.AppendLine();
                    }

                    hubView.AppendLine("  -1 AS " + TeamConfigurationSettings.EtlProcessAttribute + ",");

                    if (TeamConfigurationSettings.EnableAlternativeLoadDateTimeAttribute == "True")
                    {
                        hubView.AppendLine("  MIN(" + TeamConfigurationSettings.LoadDateTimeAttribute + ") AS " + TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + ",");
                    }
                    else
                    {
                        hubView.AppendLine("  MIN(" + TeamConfigurationSettings.LoadDateTimeAttribute + ") AS " + TeamConfigurationSettings.LoadDateTimeAttribute + ",");
                    }


                    if (TeamConfigurationSettings.EnableAlternativeRecordSourceAttribute == "True")
                    {
                        hubView.AppendLine("  " + TeamConfigurationSettings.RecordSourceAttribute + " AS " + TeamConfigurationSettings.AlternativeRecordSourceAttribute + ",");
                    }
                    else
                    {
                        hubView.AppendLine("  " + TeamConfigurationSettings.RecordSourceAttribute + ",");
                    }

                    foreach (DataRow hubKey in hubKeyList.Rows)
                    {
                        hubView.AppendLine("    CONVERT(" + stringDataType + "(100),[" + (string) hubKey["COLUMN_NAME"] + "]) AS "+ (string)hubKey["COLUMN_NAME"] + ",");
                    }

                    // Row number for record condensing
                    hubView.AppendLine("  ROW_NUMBER() OVER (PARTITION  BY");
                    foreach (DataRow hubKey in hubKeyList.Rows)
                    {
                        hubView.AppendLine("      [" + (string)hubKey["COLUMN_NAME"] + "],");
                    }
                    hubView.Remove(hubView.Length - 3, 3);
                    hubView.AppendLine();
                    hubView.AppendLine("  ORDER BY ");

                    if (TeamConfigurationSettings.EnableAlternativeLoadDateTimeAttribute == "True")
                    {
                        hubView.AppendLine("      MIN(" + TeamConfigurationSettings.LoadDateTimeAttribute + ")");
                    }
                    else
                    {
                        hubView.AppendLine("      MIN(" + TeamConfigurationSettings.LoadDateTimeAttribute + ")");
                    }
                    hubView.AppendLine("  ) AS ROW_NR");

                    hubView.AppendLine("FROM");
                    hubView.AppendLine("(");

                    // Determine if there are many Staging / Hubs relationships to map to this setup
                    // These will be treated as individual 'ETLs' which are to be unioned in the query
                    var queryHubGen = "SELECT * FROM [interface].[INTERFACE_SOURCE_HUB_XREF] WHERE HUB_NAME = '" + hubTableName + "'";
                    var hubTables = GetDataTable(ref connOmd, queryHubGen);


                    // This loop runs through the various STG / Hub relationships to create the union statements
                    if (hubTables != null)
                    {
                        var rowcounter = 1;

                        foreach (DataRow hubDetailRow in hubTables.Rows)
                        {
                            var sqlSourceStatement = new StringBuilder();
                           
                            var stagingAreaTableName = (string)hubDetailRow["SOURCE_NAME"];
                            var psaTableName = TeamConfigurationSettings.PsaTablePrefixValue + stagingAreaTableName.Replace(TeamConfigurationSettings.StgTablePrefixValue, "");
                            var businessKeyDefinition = (string) hubDetailRow["SOURCE_BUSINESS_KEY_DEFINITION"];
                            var filterCriteria = (string)hubDetailRow["FILTER_CRITERIA"];


                            // Construct the join clauses, where clauses etc. for the Hubs
                            var queryClauses = GetHubClauses(stagingAreaTableName, hubTableName, businessKeyDefinition, "");

                            var hubQuerySelect = queryClauses[0];
                            var hubQueryWhere = queryClauses[1];
                            var hubQueryGroupBy = queryClauses[2];

                            //hubQuerySelect.Remove(hubQuerySelect.Length - 3, 3);
                            hubQueryWhere.Remove(hubQueryWhere.Length - 6, 6);
                            //hubQueryGroupBy.Remove(hubQueryGroupBy.Length - 3, 3);

                            //Troubleshooting
                            if (hubQuerySelect == "")
                            {
                                SetTextLink("Keys missing, please check the metadata for table " + hubTableName + "\r\n");
                            }

                            sqlSourceStatement.AppendLine("  SELECT ");
                            sqlSourceStatement.AppendLine("    " + hubQuerySelect);
                            sqlSourceStatement.Remove(sqlSourceStatement.Length - 3, 3);
                            sqlSourceStatement.AppendLine("       " + TeamConfigurationSettings.RecordSourceAttribute + ",");
                            sqlSourceStatement.AppendLine("       MIN(" + TeamConfigurationSettings.LoadDateTimeAttribute + ") AS " + TeamConfigurationSettings.LoadDateTimeAttribute +"");
                            sqlSourceStatement.AppendLine("  FROM "+TeamConfigurationSettings.SchemaName+"." + psaTableName);
                            sqlSourceStatement.AppendLine("  WHERE");
                            sqlSourceStatement.AppendLine("    " + hubQueryWhere);
                            sqlSourceStatement.Remove(sqlSourceStatement.Length - 7, 7);
                            sqlSourceStatement.AppendLine();

                            if (string.IsNullOrEmpty(filterCriteria))
                            {}
                            else
                            {
                                sqlSourceStatement.AppendLine("    AND " + filterCriteria);
                            }

                            sqlSourceStatement.AppendLine("  GROUP BY ");
                            sqlSourceStatement.AppendLine("    " + hubQueryGroupBy);
                            sqlSourceStatement.Remove(sqlSourceStatement.Length - 3, 3);

                            sqlSourceStatement.AppendLine("    " + TeamConfigurationSettings.RecordSourceAttribute + "");

                            hubView.Append(sqlSourceStatement);

                            if (rowcounter < hubTables.Rows.Count)
                            {
                                hubView.AppendLine("UNION");
                            }

                            rowcounter++;
                        }

                    } // end of if datatable not empty

                    hubView.AppendLine(") HUB_selection");
                    hubView.AppendLine("GROUP BY");

                    foreach (DataRow hubKey in hubKeyList.Rows)
                    {
                        hubView.AppendLine("  [" + (string) hubKey["COLUMN_NAME"] + "],");
                    }

                    hubView.AppendLine("  " + TeamConfigurationSettings.RecordSourceAttribute);

                    hubView.AppendLine(") hub");
                    hubView.AppendLine("WHERE ROW_NR = 1");

                    // Zero record insert
                    hubView.AppendLine("UNION");

                    //Regular Hash
                    if (VedwConfigurationSettings.HashKeyOutputType == "Character")
                    {
                        hubView.AppendLine("SELECT "+VedwConfigurationSettings.hashingZeroKey+",");
                    }
                    else if (VedwConfigurationSettings.HashKeyOutputType == "Binary")
                    {
                        hubView.AppendLine("SELECT "+VedwConfigurationSettings.hashingZeroKey+",");
                    }
                    else // Throw error
                    {
                        MessageBox.Show("Error defining key output type " + VedwConfigurationSettings.HashKeyOutputType);
                    }

                    hubView.AppendLine("- 1,");
                    hubView.AppendLine("'1900-01-01',");
                    hubView.AppendLine("'Data Warehouse',");
                    foreach (DataRow hubKey in hubKeyList.Rows) // Supporting composite and concatenate
                    {
                        hubView.AppendLine("'Unknown',");
                    }
                    hubView.AppendLine("1 AS ROW_NR");

                    hubView.AppendLine();
                    hubView.AppendLine("GO");
                    // END OF MAIN QUERY

                    //Output to file
                    using (var outfile = new StreamWriter(textBoxOutputPath.Text + @"\VIEW_" + hubTableName + ".sql"))
                    {
                        outfile.Write(hubView.ToString());
                        outfile.Close();
                    }

                    //Generate in database
                    if (checkBoxGenerateInDatabase.Checked)
                    {
                        connPsa.ConnectionString = TeamConfigurationSettings.ConnectionStringHstg;
                        int insertError = GenerateInDatabase(connPsa, hubView.ToString());
                        errorCounter = errorCounter + insertError;
                    }

                    //Present in front-end
                    SetTextDebug(hubView.ToString());
                    SetTextDebug("\n");

                    SetTextHub($"Processing Hub entity view for {hubTableName}\r\n");
                }
            }
            else
            {
                SetTextHub("There was no metadata selected to create Hub views. Please check the metadata schema - are there any Hubs selected?");
            }

            connOmd.Close();
            connStg.Close();
            connPsa.Close();
            connInt.Close();

            SetTextHub($"\r\n{errorCounter} errors have been found.\r\n");
            SetTextHub($"SQL Scripts have been successfully saved in {VedwConfigurationSettings.VedwOutputPath}.\r\n");

        }

        //Retrieve the table structure for a given table, in a given version
        private DataTable GetTableStructure(string targetTableName, ref SqlConnection sqlConnection, string tableType)
        {
            var sqlStatementForSourceQuery = new StringBuilder();

            if (!checkBoxIgnoreVersion.Checked && tableType != "PSA")
            {
                sqlStatementForSourceQuery.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION");
                sqlStatementForSourceQuery.AppendLine("FROM [interface].[INTERFACE_PHYSICAL_MODEL]");
                sqlStatementForSourceQuery.AppendLine("WHERE TABLE_NAME= '" + targetTableName + "'");
            }

            if (checkBoxIgnoreVersion.Checked || tableType == "PSA")
            {
                sqlStatementForSourceQuery.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION");
                sqlStatementForSourceQuery.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS");
                sqlStatementForSourceQuery.AppendLine("WHERE TABLE_NAME= '" + targetTableName + "'");
                sqlStatementForSourceQuery.AppendLine("AND TABLE_SCHEMA = '" + TeamConfigurationSettings.SchemaName + "'");
                sqlStatementForSourceQuery.AppendLine("ORDER BY ORDINAL_POSITION");
            }

            var sourceStructure = GetDataTable(ref sqlConnection, sqlStatementForSourceQuery.ToString());

            if (sourceStructure == null)
            {
                SetTextDebug("An error has occurred retrieving the table structure for table "+targetTableName+". If the 'ignore version' option is checked this information is retrieved from the data dictionary. If unchecked the metadata ('manage model metadata') will be used.");
            }

            return sourceStructure;
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
                SetTextDebug("An error has occurred interpreting the components of the Business Key for "+ hubTableName + " due to connectivity issues (connection string " + conn.ConnectionString + "). The associated message is " + exception.Message);
   
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
                SetTextDebug("An error has occurred interpreting the Hub Business Key (components) in the model for " + hubTableName + ". The Business Key was not found when querying the underlying metadata.");
            }

            return componentList;
        }

        //  Executing a SQL object against the databasa (SQL Server SMO API)
        public int GenerateInDatabase(SqlConnection sqlConnection, string viewStatement)
        {
            int errorCounter = 0;
            using (var connection = sqlConnection)
            {
                var server = new Server(new ServerConnection(connection));
                try
                {
                    server.ConnectionContext.ExecuteNonQuery(viewStatement);
                    SetTextDebug("The statement was executed successfully.\r\n");
                }
                catch (Exception exception)
                {
                    SetTextDebug("Issues occurred executing the SQL statement.\r\n");
                    SetTextDebug(@"SQL error: " + exception.Message + "\r\n\r\n");
                    errorCounter++;
                }
            }

            return errorCounter;
        }

        private void openOutputDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            try
            {
                Process.Start(VedwConfigurationSettings.VedwOutputPath);
            }
            catch (Exception ex)
            {
                richTextBoxInformation.Text = "An error has occured while attempting to open the output directory. The error message is: "+ex;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SatelliteButtonClick (object sender, EventArgs e)
        {
            richTextBoxSat.Clear();
            richTextBoxInformation.Clear();

            var newThread = new Thread(BackgroundDoSat);
            newThread.Start();
        }

        private void BackgroundDoSat(Object obj)
        {
            // Check if the schema needs to be created
            CreateSchema(TeamConfigurationSettings.ConnectionStringInt);

            if (radiobuttonViews.Checked)
            {
                GenerateSatViews();
            }
            else if (radiobuttonStoredProc.Checked)
            {
                MessageBox.Show("This feature is yet to be implemented.", "Upcoming!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (radioButtonIntoStatement.Checked)
            {
                GenerateSatInsertInto();
            }
        }

        // Generate the Insert Into statement for the Satellites
        private void GenerateSatInsertInto()
        {
            int errorCounter = 0;

            if (checkBoxIfExistsStatement.Checked)
            {
                SetTextSat("The Drop If Exists checkbox has been checked, but this feature is not relevant for this specific operation and will be ignored. \n\r");
            }

            if (checkedListBoxSatMetadata.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= checkedListBoxSatMetadata.CheckedItems.Count - 1; x++)
                {
                    var connHstg = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringHstg };
                    var connOmd = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringOmd};
                    var conn = new SqlConnection
                    {
                        ConnectionString = checkBoxIgnoreVersion.Checked ? TeamConfigurationSettings.ConnectionStringInt : TeamConfigurationSettings.ConnectionStringOmd
                    };

                    var targetTableName = checkedListBoxSatMetadata.CheckedItems[x].ToString();

                    var queryTableArray = "SELECT * FROM interface.INTERFACE_SOURCE_SATELLITE_XREF " +
                                          "WHERE SATELLITE_TYPE = 'Normal' " +
                                          " AND SATELLITE_NAME = '" + targetTableName + "'";


                    var tables = GetDataTable(ref connOmd, queryTableArray);

                    foreach (DataRow row in tables.Rows)
                    {
                        var hubSk = row["HUB_NAME"].ToString().Substring(4) + "_"+TeamConfigurationSettings.DwhKeyIdentifier;

                        // Build the main attribute list of the Satellite table for selection
                        var sourceTableStructure = GetTableStructure(targetTableName, ref conn, "SAT");

                        // Query to detect multi-active attributes
                        var multiActiveAttributes = GetMultiActiveAttributes((int)row["SATELLITE_ID"]);

                        // Initial SQL
                        var insertIntoStatement = new StringBuilder();

                        insertIntoStatement.AppendLine("--");
                        insertIntoStatement.AppendLine("-- Satellite Insert Into statement for " + targetTableName);
                        insertIntoStatement.AppendLine("-- Generated at " + DateTime.Now);
                        insertIntoStatement.AppendLine("--");
                        insertIntoStatement.AppendLine();
                        insertIntoStatement.AppendLine("USE [" + TeamConfigurationSettings.PsaDatabaseName + "]");
                        insertIntoStatement.AppendLine("GO");
                        insertIntoStatement.AppendLine();
                        insertIntoStatement.AppendLine("INSERT INTO [" + TeamConfigurationSettings.IntegrationDatabaseName + "].[" +
                                                       TeamConfigurationSettings.SchemaName + "].[" + targetTableName + "]");
                        insertIntoStatement.AppendLine("   (");

                        foreach (DataRow attribute in sourceTableStructure.Rows)
                        {
                            var sourceAttribute = attribute["COLUMN_NAME"];

                            insertIntoStatement.Append("   [" + sourceAttribute + "],");
                            insertIntoStatement.AppendLine();
                        }

                        insertIntoStatement.Remove(insertIntoStatement.Length - 3, 3);
                        insertIntoStatement.AppendLine();
                        insertIntoStatement.AppendLine("   )");
                        insertIntoStatement.AppendLine("SELECT ");
                        

                        foreach (DataRow attribute in sourceTableStructure.Rows)
                        {
                            var sourceAttribute = attribute["COLUMN_NAME"];

                            if ((string)sourceAttribute == TeamConfigurationSettings.EtlProcessAttribute || (string)sourceAttribute == TeamConfigurationSettings.EtlProcessUpdateAttribute)
                            {
                                insertIntoStatement.Append("   -1 AS [" + sourceAttribute + "],");
                                insertIntoStatement.AppendLine();
                            }
                            else if ((string)attribute["COLUMN_NAME"] == TeamConfigurationSettings.AlternativeRecordSourceAttribute)
                            {
                                insertIntoStatement.AppendLine("   CHECKSUM(sat_view." + TeamConfigurationSettings.AlternativeRecordSourceAttribute + ") AS " + attribute["COLUMN_NAME"] + ",");
                            }
                            else
                            {
                                insertIntoStatement.Append("   sat_view.[" + sourceAttribute + "],");
                                insertIntoStatement.AppendLine();
                            }
                        }

                        insertIntoStatement.Remove(insertIntoStatement.Length - 3, 3);
                        insertIntoStatement.AppendLine();
                        insertIntoStatement.AppendLine("FROM [" + VedwConfigurationSettings.VedwSchema + "]." + targetTableName + " sat_view");
                        insertIntoStatement.AppendLine("LEFT OUTER JOIN");
                        insertIntoStatement.AppendLine("   [" + TeamConfigurationSettings.IntegrationDatabaseName + "].[" + TeamConfigurationSettings.SchemaName + "].[" + targetTableName + "] sat_table");
                        insertIntoStatement.AppendLine(" ON sat_view.[" + hubSk + "] = sat_table.[" + hubSk+"]");



                        if (TeamConfigurationSettings.EnableAlternativeSatelliteLoadDateTimeAttribute == "True")
                        {
                            insertIntoStatement.AppendLine("AND sat_view.[" + TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "] = sat_table.[" +
                                                           TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "]");
                        }
                        else
                        {
                            insertIntoStatement.AppendLine("AND sat_view.[" + TeamConfigurationSettings.LoadDateTimeAttribute + "] = sat_table.[" +
                                                         TeamConfigurationSettings.LoadDateTimeAttribute + "]");                          
                        }

                        foreach (DataRow attribute in multiActiveAttributes.Rows)
                        {
                            insertIntoStatement.AppendLine("AND sat_view.[" + (string)attribute["SATELLITE_ATTRIBUTE_NAME"] + "] = sat_table.["+(string)attribute["SATELLITE_ATTRIBUTE_NAME"] +"]");
                        }

                        insertIntoStatement.AppendLine("WHERE sat_table." + hubSk + " IS NULL");

                        using (var outfile = new StreamWriter(textBoxOutputPath.Text + @"\INSERT_STATEMENT_" + targetTableName + ".sql"))
                        {
                            outfile.Write(insertIntoStatement.ToString());
                            outfile.Close();
                        }

                        if (checkBoxGenerateInDatabase.Checked)
                        {
                            connHstg.ConnectionString = TeamConfigurationSettings.ConnectionStringHstg;
                            int insertError = GenerateInDatabase(connHstg, insertIntoStatement.ToString());
                            errorCounter = errorCounter + insertError;
                        }

                        SetTextDebug(insertIntoStatement.ToString());
                        SetTextDebug("\n");

                        SetTextSat(string.Format("Processing Satellite Insert Into statement for {0}\r\n",targetTableName));
                    }

                    connOmd.Close();
                    connHstg.Close();
                    conn.Close();
                }
            }
            else
            {
                SetTextSat("There was no metadata selected to create Satellite insert statements. Please check the metadata schema - are there any Satellites selected?");
            }

            SetTextSat($"\r\n{errorCounter} errors have been found.\r\n");
            SetTextSat($"SQL Scripts have been successfully saved in {VedwConfigurationSettings.VedwOutputPath}.\r\n");
        }

        private void GenerateSatViews() //  Generate Satellite Views
        {
            int errorCounter = 0;

            if (checkedListBoxSatMetadata.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= checkedListBoxSatMetadata.CheckedItems.Count - 1; x++)
                {
                    var targetTableName = checkedListBoxSatMetadata.CheckedItems[x].ToString();

                    var stringDataType = checkBoxUnicode.Checked ? "NVARCHAR" : "VARCHAR";

                    var sqlStatementForTablesToImport = "SELECT * FROM interface.INTERFACE_SOURCE_SATELLITE_XREF " +
                                                        "WHERE SATELLITE_TYPE = 'Normal' " +
                                                        " AND SATELLITE_NAME = '" + targetTableName + "'";
                  
                    var connOmd = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringOmd};
                    var connHstg = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringHstg};
                    var connStg = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringStg };

                    var conn = new SqlConnection
                    {
                        ConnectionString =
                            checkBoxIgnoreVersion.Checked
                                ? TeamConfigurationSettings.ConnectionStringStg
                                : TeamConfigurationSettings.ConnectionStringOmd
                    };

                    // Start logic handling
                    var tables = GetDataTable(ref connOmd, sqlStatementForTablesToImport);

                    if (tables.Rows.Count == 0)
                    {
                        SetTextSat("There was no metadata available to create Satellites. Please check the metadata schema (are there any Link Satellites available?) or the database connection.");
                    }

                    foreach (DataRow row in tables.Rows)
                    {
                        // Declare variabels and arrays
                        var targetTableId = (int) row["SATELLITE_ID"];
                        var stagingAreaTableId = (int) row["SOURCE_ID"];
                        var stagingAreaTableName = (string) row["SOURCE_NAME"];
                        var psaTableName = TeamConfigurationSettings.PsaTablePrefixValue + stagingAreaTableName.Replace(TeamConfigurationSettings.StgTablePrefixValue, "");
                        var hubTableName = (string) row["HUB_NAME"];
                        var filterCriteria = (string)row["FILTER_CRITERIA"];
                        var businessKeyDefinition = (string)row["SOURCE_BUSINESS_KEY_DEFINITION"];

                        var hubSk = hubTableName.Substring(4) + "_" + TeamConfigurationSettings.DwhKeyIdentifier;

                        // The name of the Hub hash key as it may be available in the Staging Area (if added here)
                        var stgHubSk = TeamConfigurationSettings.DwhKeyIdentifier + "_" + hubTableName;

                        string hubQuerySelect = "";
                        string hubQueryWhere = "";
                        string hubQueryGroupBy = "";

                        string multiActiveAttributeFromName;

                        var satView = new StringBuilder();

                        // Initial SQL
                        satView.AppendLine("-- ");
                        satView.AppendLine("-- Satellite View definition for " + targetTableName);
                        satView.AppendLine("-- Generated at " + DateTime.Now);
                        satView.AppendLine("-- ");
                        satView.AppendLine();
                        satView.AppendLine("USE [" + TeamConfigurationSettings.PsaDatabaseName + "]");
                        satView.AppendLine("GO");
                        satView.AppendLine();
                        satView.AppendLine("IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[" + VedwConfigurationSettings.VedwSchema + "].[" + targetTableName + "]') AND type in (N'V'))");
                        satView.AppendLine("DROP VIEW [" + VedwConfigurationSettings.VedwSchema + "].[" + targetTableName + "]");
                        satView.AppendLine("go");
                        satView.AppendLine("CREATE VIEW [" + VedwConfigurationSettings.VedwSchema + "].[" + targetTableName + "] AS  ");

                        // Query the Staging Area to retrieve the attributes and datatypes, precisions and length
                        var sqlStatementForSourceAttribute = new StringBuilder();

                        var localKey = TeamConfigurationSettings.DwhKeyIdentifier;
                        var localkeyLength = localKey.Length;
                        var localkeySubstring = localkeyLength + 1;

                        if (checkBoxIgnoreVersion.Checked)
                        {
                            sqlStatementForSourceAttribute.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION");
                            sqlStatementForSourceAttribute.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS");
                            sqlStatementForSourceAttribute.AppendLine("WHERE TABLE_NAME= '" + psaTableName + "'");
                            sqlStatementForSourceAttribute.AppendLine("AND TABLE_SCHEMA = '" + TeamConfigurationSettings.SchemaName + "'");
                            sqlStatementForSourceAttribute.AppendLine("  AND SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" +localkeyLength + "," + localkeySubstring + ")!='_" +TeamConfigurationSettings.DwhKeyIdentifier + "'");
                            sqlStatementForSourceAttribute.AppendLine("  AND COLUMN_NAME NOT IN ('" +TeamConfigurationSettings.RecordSourceAttribute + "','" +TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" +TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" +TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute +"','" +TeamConfigurationSettings.EtlProcessAttribute + "','" + TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" +TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" +TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute +"','" +TeamConfigurationSettings.LoadDateTimeAttribute + "')");
                            sqlStatementForSourceAttribute.AppendLine("ORDER BY ORDINAL_POSITION");
                        }
                        else
                        {
                            sqlStatementForSourceAttribute.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION, ORDINAL_POSITION");
                            sqlStatementForSourceAttribute.AppendLine("FROM [interface].[INTERFACE_PHYSICAL_MODEL]");
                            //sqlStatementForSourceAttribute.AppendLine("WHERE VERSION_ID = " + versionId);
                            sqlStatementForSourceAttribute.AppendLine("  WHERE TABLE_NAME= '" + psaTableName + "'");
                            sqlStatementForSourceAttribute.AppendLine("  AND SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" +localkeyLength + "," + localkeySubstring + ")!='_" +TeamConfigurationSettings.DwhKeyIdentifier + "'");
                            sqlStatementForSourceAttribute.AppendLine("  AND COLUMN_NAME NOT IN ('" +TeamConfigurationSettings.RecordSourceAttribute + "','" +TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" +TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" +TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute +"','" +TeamConfigurationSettings.EtlProcessAttribute + "','" +TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" +TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" +TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute +"','" +TeamConfigurationSettings.LoadDateTimeAttribute + "')");
                            sqlStatementForSourceAttribute.AppendLine("ORDER BY ORDINAL_POSITION");
                        }

                        var stgStructure = GetDataTable(ref conn, sqlStatementForSourceAttribute.ToString());
                        stgStructure.PrimaryKey = new[] {stgStructure.Columns["COLUMN_NAME"]};


                        // For every STG / Hub relationship, the business key needs to be defined - starting with the components of the key
                        var componentElementList = GetBusinessKeyElementsBase(stagingAreaTableName, hubTableName, businessKeyDefinition);
                        componentElementList.PrimaryKey = new[]{componentElementList.Columns["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"]};

                        // Query to detect multi-active attributes
                        var multiActiveAttributes = GetMultiActiveAttributes(targetTableId);

                        // Retrieve the Source-To-Target mapping for Satellites
                        var sourceStructure = GetStagingToSatelliteAttributeMapping(targetTableId, stagingAreaTableId);

                        // Checking if a hash value already exists in the source (the Staging Area, so it doesn't need to be calculated again)
                        var foundRow = stgStructure.Rows.Find(stgHubSk);

                        // Retrieving the business key attributes for the Hub                 
                        var hubKeyList = GetHubTargetBusinessKeyList(hubTableName);

                        // Construct the join clauses, where clauses etc. for the Hubs
                        var queryClauses = GetHubClauses(stagingAreaTableName, hubTableName, businessKeyDefinition, "");
                        hubQuerySelect = queryClauses[0];

                        //Troubleshooting
                        if (hubQuerySelect == "")
                        {
                            SetTextLink("Keys missing, please check the metadata for table " + hubTableName + "\r\n");
                        }


                        // Creating the query
                        satView.AppendLine("SELECT");

                        if (!checkBoxDisableHash.Checked)
                        {
                            // Regular hash calculation
                            // Satellite / Hub key
                            if (foundRow != null)
                            {
                                // Hash can be selected from STG
                                satView.AppendLine("   " + stgHubSk + " AS " + hubSk + ",");
                            }
                            else
                            {
                                // Hash needs to be calculated
                                satView.AppendLine("   " + VedwConfigurationSettings.hashingStartSnippet);

                                foreach (DataRow attribute in hubKeyList.Rows)
                                {
                                    satView.AppendLine("     ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + attribute["COLUMN_NAME"] + ")),'NA')+'|'+");
                                }

                                satView.Remove(satView.Length - 3, 3);
                                satView.AppendLine();
                                satView.AppendLine("   "+VedwConfigurationSettings.hashingEndSnippet+" AS " + hubSk + ",");
                            }
                        }
                        else
                        {
                            //BK as DWH key
                            foreach (DataRow attribute in hubKeyList.Rows)
                            {
                                satView.Append("  ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + attribute["COLUMN_NAME"] + ")),'NA')+'|'+");
                            }
                            satView.Remove(satView.Length - 5, 5);
                            //hubView.AppendLine();
                            satView.Append("  AS " + hubSk + ",");
                            satView.AppendLine();
                        }


                        // Effective Date / LDTS
                        if (TeamConfigurationSettings.EnableAlternativeSatelliteLoadDateTimeAttribute == "True")
                        {
                            satView.AppendLine("   DATEADD(mcs,[" + TeamConfigurationSettings.RowIdAttribute + "]," + TeamConfigurationSettings.LoadDateTimeAttribute + ") AS " + TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + ",");
                        }
                        else
                        {
                            satView.AppendLine("   DATEADD(mcs,["+TeamConfigurationSettings.RowIdAttribute+"]," + TeamConfigurationSettings.LoadDateTimeAttribute + ") AS " + TeamConfigurationSettings.LoadDateTimeAttribute + ",");
                        }


                        // Expiry datetime
                        satView.AppendLine("   COALESCE ( LEAD ( DATEADD(mcs,[" + TeamConfigurationSettings.RowIdAttribute + "]," + TeamConfigurationSettings.LoadDateTimeAttribute + ") ) OVER");
                        satView.AppendLine("   		     (PARTITION BY ");


                        foreach (DataRow hubKey in hubKeyList.Rows)
                        {
                            satView.AppendLine("   		          [" + (string)hubKey["COLUMN_NAME"] + "],");
                        }

                        satView.Remove(satView.Length - 2, 2);
                        satView.AppendLine();
                        foreach (DataRow attribute in multiActiveAttributes.Rows)
                        {
                            multiActiveAttributeFromName = (string) attribute["SOURCE_ATTRIBUTE_NAME"];
                            satView.AppendLine("              " + multiActiveAttributeFromName + ",");
                        }


              

                        satView.Remove(satView.Length - 3, 3);
                        satView.AppendLine();
                        satView.AppendLine("   		      ORDER BY " + TeamConfigurationSettings.LoadDateTimeAttribute + "),");
                        satView.AppendLine("   CAST( '9999-12-31' AS DATETIME)) AS " + TeamConfigurationSettings.ExpiryDateTimeAttribute +
                                           ",");
                        satView.AppendLine("   CASE");
                        satView.AppendLine("      WHEN ( RANK() OVER (PARTITION BY ");


                        foreach (DataRow hubKey in hubKeyList.Rows)
                        {
                            satView.AppendLine("           [" + (string)hubKey["COLUMN_NAME"] + "],");
                        }

                        satView.Remove(satView.Length - 2, 2);
                        satView.AppendLine();
                        foreach (DataRow attribute in multiActiveAttributes.Rows)
                        {
                            multiActiveAttributeFromName = (string) attribute["SOURCE_ATTRIBUTE_NAME"];
                            satView.AppendLine("         " + multiActiveAttributeFromName + ",");
                        }

                        satView.Remove(satView.Length - 3, 3);
                        satView.AppendLine();
                        satView.AppendLine("          ORDER BY " + TeamConfigurationSettings.LoadDateTimeAttribute + " desc )) = 1");
                        satView.AppendLine("      THEN 'Y'");
                        satView.AppendLine("      ELSE 'N'");
                        satView.AppendLine("   END AS " + TeamConfigurationSettings.CurrentRowAttribute + ",");

                        satView.AppendLine("   -1 AS " + TeamConfigurationSettings.EtlProcessAttribute + ",");
                        satView.AppendLine("   -1 AS " + TeamConfigurationSettings.EtlProcessUpdateAttribute + ",");
                        satView.AppendLine("   " + TeamConfigurationSettings.ChangeDataCaptureAttribute + ",");
                        satView.AppendLine("   " + TeamConfigurationSettings.RowIdAttribute + ",");

                        if (TeamConfigurationSettings.EnableAlternativeRecordSourceAttribute == "True")
                        {
                            satView.AppendLine("   " + TeamConfigurationSettings.RecordSourceAttribute + " AS " + TeamConfigurationSettings.AlternativeRecordSourceAttribute + ",");
                        }
                        else
                        {
                            satView.AppendLine("   " + TeamConfigurationSettings.RecordSourceAttribute + ",");
                        }

                        //Logical deletes
                        if (checkBoxEvaluateSatDelete.Checked)
                        {
                            satView.AppendLine("    CASE");
                            satView.AppendLine("      WHEN [" + TeamConfigurationSettings.ChangeDataCaptureAttribute +"] = 'Delete' THEN 'Y'");
                            satView.AppendLine("      ELSE 'N'");
                            satView.AppendLine("    END AS [" + TeamConfigurationSettings.LogicalDeleteAttribute+"],");
                        }

         

                        //Hash key generation
                        satView.AppendLine("   "+VedwConfigurationSettings.hashingStartSnippet);
                        satView.AppendLine("      ISNULL(RTRIM(CONVERT("+ stringDataType + "(100)," + TeamConfigurationSettings.ChangeDataCaptureAttribute + ")),'NA')+'|'+");

                        foreach (DataRow attribute in sourceStructure.Rows)
                        {
                            var localAttribute = attribute["SOURCE_ATTRIBUTE_NAME"];
                            var foundBusinessKeyAttribute = componentElementList.Rows.Find(localAttribute);

                            if (foundBusinessKeyAttribute == null)
                            {
                                satView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + localAttribute +")),'NA')+'|'+");
                            }
                        }
                        satView.Remove(satView.Length - 3, 3);
                        satView.AppendLine();
                        satView.AppendLine("   " +VedwConfigurationSettings.hashingEndSnippet+ " AS " + TeamConfigurationSettings.RecordChecksumAttribute + ",");

                        // Regular attributes
                        foreach (DataRow attribute in sourceStructure.Rows)
                        {
                            var localAttribute = attribute["SOURCE_ATTRIBUTE_NAME"];
                            var localAttributeTarget = attribute["SATELLITE_ATTRIBUTE_NAME"];
                            var foundBusinessKeyAttribute = componentElementList.Rows.Find(localAttribute);

                            if (foundBusinessKeyAttribute == null)
                            {
                                satView.AppendLine("   " + localAttribute + " AS " + localAttributeTarget + ",");
                            }
                        }

                        // Optional Row Number (just for fun)
                        satView.AppendLine("   CAST(");
                        satView.AppendLine("      ROW_NUMBER() OVER (PARTITION  BY ");


                        // Hash needs to be calculated

                        foreach (DataRow hubKey in hubKeyList.Rows)
                        {
                            satView.AppendLine("         [" + (string)hubKey["COLUMN_NAME"] + "],");
                        }
                        satView.Remove(satView.Length - 2, 2);
                        satView.AppendLine();

                        foreach (DataRow attribute in multiActiveAttributes.Rows)
                        {
                            multiActiveAttributeFromName = (string) attribute["SOURCE_ATTRIBUTE_NAME"];
                            satView.AppendLine("         " + multiActiveAttributeFromName + ",");
                        }

                        satView.Remove(satView.Length - 3, 3);
                        satView.AppendLine("      ORDER BY ");


                        foreach (DataRow hubKey in hubKeyList.Rows)
                        {
                            satView.AppendLine("         [" + (string)hubKey["COLUMN_NAME"] + "],");
                        }

                        satView.Remove(satView.Length - 2, 2);
                        satView.AppendLine();

     

                        foreach (DataRow attribute in multiActiveAttributes.Rows)
                        {
                            multiActiveAttributeFromName = (string) attribute["SOURCE_ATTRIBUTE_NAME"];
                            satView.AppendLine("         " + multiActiveAttributeFromName + ",");
                        }
                        satView.AppendLine("         [" + TeamConfigurationSettings.LoadDateTimeAttribute + "]) AS INT)");

                        satView.AppendLine("   AS ROW_NUMBER");
                        // End of initial selection

                        // Inner selection
                        satView.AppendLine("FROM ");
                        satView.AppendLine("   (");
                        satView.AppendLine("      SELECT ");
                        satView.AppendLine("         [" + TeamConfigurationSettings.LoadDateTimeAttribute + "],");
                        satView.AppendLine("         [" + TeamConfigurationSettings.EventDateTimeAttribute + "],");
                        satView.AppendLine("         [" + TeamConfigurationSettings.RecordSourceAttribute + "],");
                        satView.AppendLine("         [" + TeamConfigurationSettings.RowIdAttribute + "],");
                        satView.AppendLine("         [" + TeamConfigurationSettings.ChangeDataCaptureAttribute + "],");

                        // Satellite / Hub key
                        if (foundRow != null)
                        {
                            // Hash can be selected from STG
                            satView.AppendLine("            " + stgHubSk + " AS " + hubSk + ",");
                        }
                        else
                        {
                            foreach (DataRow hubKey in hubKeyList.Rows)
                            {
                                satView.AppendLine("         [" + (string)hubKey["COLUMN_NAME"] + "],");
                            }
                        }

                        // Regular attributes
                        foreach (DataRow attribute in sourceStructure.Rows)
                        {
                            var localAttribute = attribute["SOURCE_ATTRIBUTE_NAME"];
                            var foundBusinessKeyAttribute = componentElementList.Rows.Find(localAttribute);

                            if (foundBusinessKeyAttribute == null)
                            {
                                satView.AppendLine("         [" + localAttribute + "],");
                            }
                        }

                        // Record condensing
                        satView.AppendLine("         COMBINED_VALUE,");
                        satView.AppendLine("         CASE ");
                        satView.AppendLine("           WHEN LAG(COMBINED_VALUE,1,"+ VedwConfigurationSettings.hashingZeroKey + ") OVER (PARTITION BY ");


                        // Satellite / Hub key
                        if (foundRow != null)
                        {
                            // Hash can be selected from STG
                            satView.AppendLine("             " + stgHubSk + " AS " + hubSk + ",");
                        }
                        else
                        {
                            foreach (DataRow hubKey in hubKeyList.Rows)
                            {
                                satView.AppendLine("             [" + (string)hubKey["COLUMN_NAME"] + "],");
                            }
                        }
                        // Handle Multi-Active
                        foreach (DataRow attribute in multiActiveAttributes.Rows)
                        {
                            satView.AppendLine("             [" + (string)attribute["SOURCE_ATTRIBUTE_NAME"] + "],");
                        }
                        satView.Remove(satView.Length - 3, 3);
                        satView.AppendLine();

                        satView.AppendLine("             ORDER BY [" + TeamConfigurationSettings.LoadDateTimeAttribute + "] ASC, [" + TeamConfigurationSettings.EventDateTimeAttribute + "] ASC, [" + TeamConfigurationSettings.ChangeDataCaptureAttribute + "] DESC) = COMBINED_VALUE");
                        satView.AppendLine("           THEN 'Same'");
                        satView.AppendLine("           ELSE 'Different'");
                        satView.AppendLine("         END AS VALUE_CHANGE_INDICATOR,");
                        satView.AppendLine("         CASE ");

                        // CDC Change Indicator
                        satView.AppendLine("           WHEN LAG([" + TeamConfigurationSettings.ChangeDataCaptureAttribute + "],1,'') OVER (PARTITION BY ");
                        if (foundRow != null)
                        {
                            // Hash can be selected from STG
                            satView.AppendLine("             " + stgHubSk + " AS " + hubSk + ",");
                        }
                        else
                        {
                            foreach (DataRow hubKey in hubKeyList.Rows)
                            {
                                satView.AppendLine("             [" + (string)hubKey["COLUMN_NAME"] + "],");
                            }
                        }
                        // Handle Multi-Active
                        foreach (DataRow attribute in multiActiveAttributes.Rows)
                        {
                            satView.AppendLine("             [" + (string)attribute["SOURCE_ATTRIBUTE_NAME"] + "],");
                        }
                        satView.Remove(satView.Length - 3, 3);
                        satView.AppendLine();

                        satView.AppendLine("             ORDER BY [" + TeamConfigurationSettings.LoadDateTimeAttribute + "] ASC, [" + TeamConfigurationSettings.EventDateTimeAttribute + "] ASC, [" + TeamConfigurationSettings.ChangeDataCaptureAttribute + "] ASC) = [" + TeamConfigurationSettings.ChangeDataCaptureAttribute + "]");
                        satView.AppendLine("           THEN 'Same'");
                        satView.AppendLine("           ELSE 'Different'");
                        satView.AppendLine("         END AS CDC_CHANGE_INDICATOR,");

                        // Time Change Indicator
                        satView.AppendLine("         CASE ");
                        satView.AppendLine("           WHEN LEAD([" + TeamConfigurationSettings.LoadDateTimeAttribute + "],1,'9999-12-31') OVER (PARTITION BY ");
                        
                        // Satellite / Hub key
                        if (foundRow != null)
                        {
                            // Hash can be selected from STG
                            satView.AppendLine("             " + stgHubSk + " AS " + hubSk + ",");
                        }
                        else
                        {
                            foreach (DataRow hubKey in hubKeyList.Rows)
                            {
                                satView.AppendLine("             [" + (string)hubKey["COLUMN_NAME"] + "],");
                            }
                        }
                        // Handle Multi-Active
                        foreach (DataRow attribute in multiActiveAttributes.Rows)
                        {
                            satView.AppendLine("             [" + (string)attribute["SOURCE_ATTRIBUTE_NAME"] + "],");
                        }
                        satView.Remove(satView.Length - 3, 3);
                        satView.AppendLine();


                        satView.AppendLine("             ORDER BY [" + TeamConfigurationSettings.LoadDateTimeAttribute + "] ASC, [" + TeamConfigurationSettings.EventDateTimeAttribute + "] ASC, [" + TeamConfigurationSettings.ChangeDataCaptureAttribute + "] ASC) = [" + TeamConfigurationSettings.LoadDateTimeAttribute + "]");
                        satView.AppendLine("           THEN 'Same'");
                        satView.AppendLine("           ELSE 'Different'");
                        satView.AppendLine("         END AS TIME_CHANGE_INDICATOR");

                        satView.AppendLine("      FROM");


                        // Combined Value selection (inner most query)
                        satView.AppendLine("      (");
                        satView.AppendLine("        SELECT");
                        satView.AppendLine("          [" + TeamConfigurationSettings.LoadDateTimeAttribute + "],");
                        satView.AppendLine("          [" + TeamConfigurationSettings.EventDateTimeAttribute + "],");
                        satView.AppendLine("          [" + TeamConfigurationSettings.RecordSourceAttribute + "],");
                        satView.AppendLine("          [" + TeamConfigurationSettings.RowIdAttribute + "],");
                        satView.AppendLine("          [" + TeamConfigurationSettings.ChangeDataCaptureAttribute + "],");

                        // Business keys 
                        if (foundRow != null)
                        {
                            // Hash can be selected from STG
                            satView.AppendLine("          [" + stgHubSk + "],");
                        }
                        else
                        {
                            satView.AppendLine("          " + hubQuerySelect);
                            satView.Remove(satView.Length - 3, 3);
                        }

                        // Regular attributes
                        foreach (DataRow attribute in sourceStructure.Rows)
                        {
                            var localAttribute = attribute["SOURCE_ATTRIBUTE_NAME"];
                            var foundBusinessKeyAttribute = componentElementList.Rows.Find(localAttribute);

                            if (foundBusinessKeyAttribute == null)
                            {
                                satView.AppendLine("          [" + localAttribute + "],");
                            }
                        }

                        // Hash needs to be calculated for Combined Value
                        satView.AppendLine("         "+VedwConfigurationSettings.hashingStartSnippet);

                        foreach (DataRow attribute in sourceStructure.Rows)
                        {
                            satView.AppendLine("             ISNULL(RTRIM(CONVERT(" + stringDataType + "(100),[" + attribute["SOURCE_ATTRIBUTE_NAME"] + "])),'NA')+'|'+");
                        }

                        satView.Remove(satView.Length - 3, 3);
                        satView.AppendLine();
                        satView.AppendLine("         "+VedwConfigurationSettings.hashingEndSnippet+" AS COMBINED_VALUE");
                        satView.AppendLine("        FROM " + TeamConfigurationSettings.SchemaName + "." + psaTableName);

                        // Filter criteria
                        if (string.IsNullOrEmpty(filterCriteria))
                        {
                        }
                        else
                        {
                            satView.AppendLine("      WHERE " + filterCriteria);
                        }

                        if (checkBoxDisableSatZeroRecords.Checked==false)
                        {
                            // Start of zero record
                            satView.AppendLine("        UNION");
                            satView.AppendLine("        SELECT DISTINCT");
                            satView.AppendLine("          '1900-01-01' AS [" + TeamConfigurationSettings.LoadDateTimeAttribute + "],");
                            satView.AppendLine("          '1900-01-01' AS [" + TeamConfigurationSettings.EventDateTimeAttribute + "],");
                            satView.AppendLine("          'Data Warehouse' AS [" + TeamConfigurationSettings.RecordSourceAttribute + "],");
                            satView.AppendLine("          0 AS [" + TeamConfigurationSettings.RowIdAttribute + "],");
                            satView.AppendLine("          'N/A' AS [" + TeamConfigurationSettings.ChangeDataCaptureAttribute + "],");

                            // Business keys 
                            if (foundRow != null)
                            {
                                // Hash can be selected from STG
                                satView.AppendLine("          [" + stgHubSk + "],");
                            }
                            else
                            {
                                satView.AppendLine("          " + hubQuerySelect);
                                satView.Remove(satView.Length - 3, 3);
                            }

                            // Multi-active attributes
                            foreach (DataRow attribute in multiActiveAttributes.Rows)
                            {
                                satView.AppendLine("          "+ attribute["SOURCE_ATTRIBUTE_NAME"] + ",");
                            }


                            // Adding regular attributes as NULLs, skipping the key and any multi-active attributes
                            foreach (DataRow attribute in sourceStructure.Rows)
                            {
                                var localAttribute = attribute["SOURCE_ATTRIBUTE_NAME"]; // Get the key
                                var foundBusinessKeyAttribute = componentElementList.Rows.Find(localAttribute); // Get the multi-active attribute
                                var foundMultiActiveAttribute = multiActiveAttributes.Rows.Find(localAttribute);

                                if (foundBusinessKeyAttribute == null && foundMultiActiveAttribute == null)
                                {
                                    satView.AppendLine("          NULL AS " + localAttribute + ",");
                                }
                            }

                            satView.AppendLine("          "+ VedwConfigurationSettings.hashingZeroKey + " AS COMBINED_VALUE");
                            satView.AppendLine("        FROM " + TeamConfigurationSettings.SchemaName + "." + psaTableName);
                            // End of zero record
                        }

                        satView.AppendLine("   ) sub");
                        satView.AppendLine(") combined_value");

                        satView.AppendLine("WHERE ");
                        satView.AppendLine("  (VALUE_CHANGE_INDICATOR ='Different' and [" + TeamConfigurationSettings.ChangeDataCaptureAttribute + "] in ('Insert', 'Change')) ");
                        satView.AppendLine("  OR");
                        satView.AppendLine("  (CDC_CHANGE_INDICATOR = 'Different' and TIME_CHANGE_INDICATOR = 'Different')");

                        // Zero record insert
                        if (checkBoxDisableSatZeroRecords.Checked == false)
                        {
                            satView.AppendLine("UNION");
                            satView.AppendLine("SELECT " + VedwConfigurationSettings.hashingZeroKey + ",");
                            satView.AppendLine("'1900-01-01',");
                            satView.AppendLine("'9999-12-31',");
                            satView.AppendLine("'Y',"); // Current Row Indicator
                            satView.AppendLine("- 1,"); // ETL insert ID
                            satView.AppendLine("- 1,"); // ETL update ID
                            satView.AppendLine("'N/A',"); // CDC operation
                            satView.AppendLine("1,"); // Row Nr
                            satView.AppendLine("'Data Warehouse',");
                            if (checkBoxEvaluateSatDelete.Checked)
                            {
                                satView.AppendLine("'N',"); // Logical Delete evaluation, if checked
                            }

                            satView.AppendLine(VedwConfigurationSettings.hashingZeroKey + ","); //Full Row Hash

                            foreach (DataRow attribute in multiActiveAttributes.Rows)
                            {
                                // Requires handling of data types
                                if (attribute["SATELLITE_ATTRIBUTE_NAME"].ToString().Contains("DATE"))
                                {
                                    satView.AppendLine("CAST('1900-01-01' AS DATE),");
                                }
                                else
                                {
                                    satView.AppendLine(VedwConfigurationSettings.hashingZeroKey + ",");
                                }
                            }

                            foreach (DataRow attribute in sourceStructure.Rows)
                            {
                                var localAttribute = attribute["SOURCE_ATTRIBUTE_NAME"];
                                var foundBusinessKeyAttribute = componentElementList.Rows.Find(localAttribute);
                                var foundMultiActiveAttribute = multiActiveAttributes.Rows.Find(localAttribute);

                                if (foundBusinessKeyAttribute == null && foundMultiActiveAttribute == null)
                                {
                                    satView.AppendLine("NULL,");
                                }
                            }

                            satView.AppendLine("1"); // Row Nr
                        }
                        // END OF ZERO RECORD CREATION


                        satView.AppendLine();
                        satView.AppendLine("GO");

                        using (var outfile = new StreamWriter(textBoxOutputPath.Text + @"\VIEW_" + targetTableName + ".sql", false))
                        {
                            outfile.Write(satView.ToString());
                            outfile.Close();
                        }

                        if (checkBoxGenerateInDatabase.Checked)
                        {
                            connHstg.ConnectionString = TeamConfigurationSettings.ConnectionStringHstg;
                            int insertError = GenerateInDatabase(connHstg, satView.ToString());
                            errorCounter = errorCounter + insertError;
                        }

                        SetTextDebug(satView.ToString());
                        SetTextDebug("\n");
                        SetTextSat(@"Processing Sat entity view for " + targetTableName + "\r\n");
                    }
                }
            }
            else
            {
                SetTextSat("There was no metadata selected to create Satellite views. Please check the metadata schema - are there any Satellites selected?");
            }

            SetTextSat($"\r\n{errorCounter} errors have been found.\r\n");
            SetTextSat($"SQL Scripts have been successfully saved in {VedwConfigurationSettings.VedwOutputPath}.\r\n");
        }

        private void DoEverythingButtonClick (object sender, EventArgs e)
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

        private void LinkButtonClick (object sender, EventArgs e)
        {
            richTextBoxLink.Clear();
            richTextBoxInformation.Clear();

            var newThread = new Thread(BackgroundDoLink);
            newThread.Start();
        }

        private void BackgroundDoLink(Object obj)
        {
            // Check if the schema needs to be created
            CreateSchema(TeamConfigurationSettings.ConnectionStringInt);

            if (radiobuttonViews.Checked)
            {
                GenerateLinkViews();
            }
            else if (radiobuttonStoredProc.Checked)
            {
                MessageBox.Show("This feature is yet to be implemented.", "Upcoming!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (radioButtonIntoStatement.Checked)
            {
                GenerateLinkInsertInto();
            }
        }

        private void GenerateLinkInsertInto()
        {
            int errorCounter = 0;

            if (checkBoxIfExistsStatement.Checked)
            {
                SetTextLink("The Drop If Exists checkbox has been checked, but this feature is not relevant for this specific operation and will be ignored. \n\r");
            }

            if (checkedListBoxLinkMetadata.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= checkedListBoxLinkMetadata.CheckedItems.Count - 1; x++)
                {
                    var connStg = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringStg};
                    var connHstg = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringHstg};
                    var conn = new SqlConnection
                    {
                        ConnectionString =
                            checkBoxIgnoreVersion.Checked
                                ? TeamConfigurationSettings.ConnectionStringInt
                                : TeamConfigurationSettings.ConnectionStringOmd
                    };

                    var targetTableName = checkedListBoxLinkMetadata.CheckedItems[x].ToString();
                    var linkSk = targetTableName.Substring(4) + "_" + TeamConfigurationSettings.DwhKeyIdentifier;

                    // Initial SQL for Link tables
                    var insertIntoStatement = new StringBuilder();

                    insertIntoStatement.AppendLine("--");
                    insertIntoStatement.AppendLine("-- Link Insert Into statement for " + targetTableName);
                    insertIntoStatement.AppendLine("-- Generated at " + DateTime.Now);
                    insertIntoStatement.AppendLine("--");
                    insertIntoStatement.AppendLine();
                    insertIntoStatement.AppendLine("USE [" + TeamConfigurationSettings.PsaDatabaseName + "]");
                    insertIntoStatement.AppendLine("GO");
                    insertIntoStatement.AppendLine();
                    insertIntoStatement.AppendLine("INSERT INTO " + TeamConfigurationSettings.IntegrationDatabaseName + "." +TeamConfigurationSettings.SchemaName + "." + targetTableName);
                    insertIntoStatement.AppendLine("   (");

                    // Build the main attribute list of the Hub table for selection
                    var sourceTableStructure = GetTableStructure(targetTableName, ref conn, "LNK");

                    foreach (DataRow attribute in sourceTableStructure.Rows)
                    {
                        insertIntoStatement.AppendLine("   [" + attribute["COLUMN_NAME"] + "],");
                    }

                    insertIntoStatement.Remove(insertIntoStatement.Length - 3, 3);
                    insertIntoStatement.AppendLine();
                    insertIntoStatement.AppendLine("   )");
                    insertIntoStatement.AppendLine("SELECT ");

                    foreach (DataRow attribute in sourceTableStructure.Rows)
                    {
                        var sourceAttribute = attribute["COLUMN_NAME"];

                        if ((string) sourceAttribute == TeamConfigurationSettings.EtlProcessAttribute)
                        {
                            insertIntoStatement.Append("   -1 AS " + sourceAttribute + ",");
                            insertIntoStatement.AppendLine();
                        }
                        else if ((string)attribute["COLUMN_NAME"] == TeamConfigurationSettings.AlternativeRecordSourceAttribute)
                        {
                            insertIntoStatement.AppendLine("   CHECKSUM(link_view." + TeamConfigurationSettings.AlternativeRecordSourceAttribute + ") AS " + attribute["COLUMN_NAME"] + ",");
                        }
                        else
                        {
                            insertIntoStatement.Append("   link_view.[" + sourceAttribute + "],");
                            insertIntoStatement.AppendLine();
                        }
                    }

                    insertIntoStatement.Remove(insertIntoStatement.Length - 3, 3);
                    insertIntoStatement.AppendLine();
                    insertIntoStatement.AppendLine("FROM [" + VedwConfigurationSettings.VedwSchema + "]." + targetTableName + " link_view");
                    insertIntoStatement.AppendLine("LEFT OUTER JOIN ");
                    insertIntoStatement.AppendLine(" " + TeamConfigurationSettings.IntegrationDatabaseName + "." + TeamConfigurationSettings.SchemaName +
                                                   "." + targetTableName + " link_table");
                    insertIntoStatement.AppendLine(" ON link_view." + linkSk + " = link_table." + linkSk);
                    insertIntoStatement.AppendLine("WHERE link_table." + linkSk + " IS NULL");

                    using (
                        var outfile =
                            new StreamWriter(textBoxOutputPath.Text + @"\INSERT_STATEMENT_" + targetTableName + ".sql"))
                    {
                        outfile.Write(insertIntoStatement.ToString());
                        outfile.Close();
                    }

                    if (checkBoxGenerateInDatabase.Checked)
                    {
                        int insertError = GenerateInDatabase(connHstg, insertIntoStatement.ToString());
                        errorCounter = errorCounter + insertError;
                    }

                    SetTextDebug(insertIntoStatement.ToString());
                    SetTextDebug("\n");
                    SetTextLink(string.Format("Processing Link Insert Into statement for {0}\r\n", targetTableName));

                    connStg.Close();
                    connHstg.Close();
                    conn.Close();
                }
            }
            else
            {
                SetTextLink("There was no metadata selected to create Link insert statements. Please check the metadata schema - are there any Links selected?");
            }

            SetTextLink($"\r\n{errorCounter} errors have been found.\r\n");
            SetTextLink($"SQL Scripts have been successfully saved in {VedwConfigurationSettings.VedwOutputPath}.\r\n");
        }

        private void GenerateLinkViews()
        {
            int errorCounter = 0;

            var connOmd = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringOmd};
            var connStg = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringStg};
            var connHstg = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringHstg};
            var connInt = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringInt};

            if (checkedListBoxLinkMetadata.CheckedItems.Count != 0)
            {
                for (var x = 0; x <= checkedListBoxLinkMetadata.CheckedItems.Count - 1; x++)
                {
                    int hubKeycounter;
                    var stringDataType = checkBoxUnicode.Checked ? "NVARCHAR" : "VARCHAR";

                    var linkTableName = checkedListBoxLinkMetadata.CheckedItems[x].ToString();
                    var linkSk = linkTableName.Substring(4) + "_" + TeamConfigurationSettings.DwhKeyIdentifier;

                    var linkView = new StringBuilder();

                    // Get the associated Hub tables and its target business key attributes for the Link
                    var hubBusinessKeyList = GetHubTablesForLink(linkTableName);
                    var hubFullBusinessKeyList = GetAllHubTablesForLink(linkTableName);
                    

                    // Get the target business key names as they are in the Link (may be aliased compared to the Hub)
                    var linkBusinessKeyList = GetLinkTargetBusinessKeyList(linkTableName);

                    var degenerateLinkAttributes = GetDegenerateLinkAttributes(linkTableName);

                    // Initial view SQL
                    linkView.AppendLine("--");
                    linkView.AppendLine("-- Link View definition for " + linkTableName);
                    linkView.AppendLine("-- Generated at " + DateTime.Now);
                    linkView.AppendLine("--");
                    linkView.AppendLine();
                    linkView.AppendLine("USE [" + TeamConfigurationSettings.PsaDatabaseName + "]");
                    linkView.AppendLine("GO");
                    linkView.AppendLine();
                    linkView.AppendLine("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[" + VedwConfigurationSettings.VedwSchema + "].[" +linkTableName + "]') AND type in (N'V'))");
                    linkView.AppendLine("DROP VIEW [" + VedwConfigurationSettings.VedwSchema + "].[" + linkTableName + "]");
                    linkView.AppendLine("GO");
                    linkView.AppendLine();
                    linkView.AppendLine("CREATE VIEW [" + VedwConfigurationSettings.VedwSchema + "].[" + linkTableName +"] AS  ");
                    linkView.AppendLine("SELECT link.*");
                    linkView.AppendLine("FROM");
                    linkView.AppendLine("(");
                                                       
                    linkView.AppendLine("SELECT");

                    if (!checkBoxDisableHash.Checked)
                    {
                        // Create Link Hash Key
                        linkView.AppendLine("  "+VedwConfigurationSettings.hashingStartSnippet);

                        // Key fields (Hubs)
                        hubKeycounter = 1; // This is to make sure the orders between source and target keys are in sync
                        foreach (var hubTargetBusinessKey in linkBusinessKeyList)
                        {
                            foreach (var hubArray in hubFullBusinessKeyList)
                            {
                                var hubBusinessKeyName = hubArray[1];
                                var hubGroupCounter = hubArray[3];

                                if (hubGroupCounter == hubKeycounter.ToString())
                                {
                                    linkView.AppendLine("    ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + hubArray[1] + ")),'NA')+'|'+");
                                }
                            }
                            hubKeycounter++;
                        }

                        // Degenerate fields (Sats)
                        if (degenerateLinkAttributes != null && degenerateLinkAttributes.Rows.Count > 0)
                        {
                            foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                            {
                                linkView.AppendLine("    ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," +
                                                    (string) attribute["LINK_ATTRIBUTE_NAME"] + ")),'NA')+'|'+");
                            }
                        }

                        linkView.Remove(linkView.Length - 3, 3);
                        linkView.AppendLine();
                        linkView.AppendLine("  "+VedwConfigurationSettings.hashingEndSnippet+" AS " + linkSk + ",");
                    }
                    else
                    {
                        //Business Key as DWH key
                        hubKeycounter = 1; // This is to make sure the orders between source and target keys are in sync
                        foreach (var hubTargetBusinessKey in linkBusinessKeyList)
                        {
                            foreach (var hubArray in hubFullBusinessKeyList)
                            {
                                var hubBusinessKeyName = hubArray[1];
                                var hubGroupCounter = hubArray[3];

                                if (hubGroupCounter == hubKeycounter.ToString())
                                {
                                    linkView.Append("  ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + hubArray[1] + ")),'NA')+'|'+");
                                }
                            }
                            hubKeycounter++;
                        }

                        if (degenerateLinkAttributes != null && degenerateLinkAttributes.Rows.Count > 0)
                        {
                            foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                            {
                                linkView.Append("    ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," +
                                                (string) attribute["LINK_ATTRIBUTE_NAME"] + ")),'NA')+'|'+");
                            }
                        }

                        linkView.Remove(linkView.Length - 5, 5);
                        linkView.Append("  AS " + linkSk + ",");
                        linkView.AppendLine();
                    }

                    linkView.AppendLine("  -1 AS " + TeamConfigurationSettings.EtlProcessAttribute + ",");

                    if (TeamConfigurationSettings.EnableAlternativeLoadDateTimeAttribute == "True")
                    {
                        linkView.AppendLine("  MIN(" + TeamConfigurationSettings.LoadDateTimeAttribute + ") AS " +TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + ",");
                    }
                    else
                    {
                        linkView.AppendLine("  MIN(" + TeamConfigurationSettings.LoadDateTimeAttribute + ") AS " + TeamConfigurationSettings.LoadDateTimeAttribute + ",");
                    }

                    if (TeamConfigurationSettings.EnableAlternativeRecordSourceAttribute == "True")
                    {
                        linkView.AppendLine("  " + TeamConfigurationSettings.RecordSourceAttribute + " AS " + TeamConfigurationSettings.AlternativeRecordSourceAttribute + ",");
                    }
                    else
                    {
                        linkView.AppendLine("  " + TeamConfigurationSettings.RecordSourceAttribute + ",");
                    }

                    hubKeycounter = 1; // This is to make sure the orders between source and target keys are in sync
                    
                    // Add individual Hub Hash keys
                    foreach (var hubTargetBusinessKey in linkBusinessKeyList)
                    {
                        var hubTargetBusinessKeyName = hubTargetBusinessKey[0]; // The Hub SK as known in the target link table

                        if (!checkBoxDisableHash.Checked)
                        {
                            linkView.AppendLine("  "+VedwConfigurationSettings.hashingStartSnippet);
                        }

                        foreach (var hubArray in hubFullBusinessKeyList)
                        {
                            //var hubNameSk = hubArray[0].Substring(4) + "_" + ConfigurationSettings.DwhKeyIdentifier;
                            var hubBusinessKeyName = hubArray[1];
                            var hubGroupCounter = hubArray[3];

                            if (hubGroupCounter == hubKeycounter.ToString())
                            {
                                linkView.AppendLine("    ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," +hubBusinessKeyName + ")),'NA')+'|'+");
                            }
                        }

                        if (!checkBoxDisableHash.Checked)
                        {
                            linkView.Remove(linkView.Length - 3, 3);
                            linkView.AppendLine();
                            linkView.AppendLine("  "+VedwConfigurationSettings.hashingEndSnippet+" AS " + hubTargetBusinessKeyName + ",");
                        }
                        else
                        {
                            linkView.Remove(linkView.Length - 3, 3);
                            linkView.Append("  AS " + hubTargetBusinessKeyName + ",");
                            linkView.AppendLine();
                        }
                        hubKeycounter++;
                    }

                    // Add the degenerate attributes
                    if (degenerateLinkAttributes != null && degenerateLinkAttributes.Rows.Count > 0)
                    {
                        foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                        {
                            linkView.AppendLine("  " + (string) attribute["LINK_ATTRIBUTE_NAME"] + ",");
                        }
                    }

                    // Row number for condensing
                    linkView.AppendLine("  ROW_NUMBER() OVER (PARTITION  BY");

                    hubKeycounter = 1; // This is to make sure the orders between source and target keys are in sync
                    foreach (var hubTargetBusinessKey in linkBusinessKeyList)
                    {
                        foreach (var hubArray in hubFullBusinessKeyList)
                        {
                            var hubBusinessKeyName = hubArray[1];
                            var hubGroupCounter = hubArray[3];

                            if (hubGroupCounter == hubKeycounter.ToString())
                            {
                                linkView.AppendLine("      [" + hubArray[1] + "],");
                            }
                        }
                        hubKeycounter++;
                    }

                    // Add the degenerate attributes
                    if (degenerateLinkAttributes != null && degenerateLinkAttributes.Rows.Count > 0)
                    {
                        foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                        {
                            linkView.AppendLine("      [" + (string) attribute["LINK_ATTRIBUTE_NAME"] + "],");
                        }
                    }

                    linkView.Remove(linkView.Length - 3, 3);
                    linkView.AppendLine();
                    linkView.AppendLine("  ORDER BY ");
                    if (TeamConfigurationSettings.EnableAlternativeLoadDateTimeAttribute == "True")
                    {
                        linkView.AppendLine("      MIN(" + TeamConfigurationSettings.LoadDateTimeAttribute + ")");
                    }
                    else
                    {
                        linkView.AppendLine("      MIN(" + TeamConfigurationSettings.LoadDateTimeAttribute + ")");
                    }
                    linkView.AppendLine("  ) AS ROW_NR");

                    linkView.AppendLine("FROM");
                    linkView.AppendLine("(");

                    // Get all relationships between the Link table and the underlying Staging source(s)
                    // This will drive the number of UNION statements (or separate ETL jobs)
                    var queryLinkStagingTables = new StringBuilder();
                    queryLinkStagingTables.AppendLine("SELECT");
                    queryLinkStagingTables.AppendLine(" [SOURCE_ID]");
                    queryLinkStagingTables.AppendLine(",[SOURCE_NAME]");
                    queryLinkStagingTables.AppendLine(",[LINK_ID]");
                    queryLinkStagingTables.AppendLine(",[LINK_NAME]");
                    queryLinkStagingTables.AppendLine(",[FILTER_CRITERIA]");
                    queryLinkStagingTables.AppendLine("FROM [interface].[INTERFACE_SOURCE_LINK_XREF]");
                    queryLinkStagingTables.AppendLine("WHERE LINK_NAME = '" + linkTableName + "'");

                    var linkStagingTables = GetDataTable(ref connOmd, queryLinkStagingTables.ToString());

                    if (linkStagingTables != null)
                    {
                        var rowcounter = 1;

                        // Loop through the mappings to add a union
                        foreach (DataRow linkDetailRow in linkStagingTables.Rows)
                        {
                            var sqlStatementForComponent = new StringBuilder();
                            var sqlSourceStatement = new StringBuilder();

                            var stagingAreaTableName = (string) linkDetailRow["SOURCE_NAME"];
                            var filterCriteria = (string) linkDetailRow["FILTER_CRITERIA"];

                            var currentTableName = TeamConfigurationSettings.PsaTablePrefixValue +stagingAreaTableName.Replace(TeamConfigurationSettings.StgTablePrefixValue, "");

                            // Get the Hubs for each Link/STG combination - both need to be represented in the query
                            var hubTables = GetHubLinkCombination(stagingAreaTableName, linkTableName);

                            var hubQuerySelect = new StringBuilder();
                            var hubQueryWhere = new StringBuilder();
                            var hubQueryGroupBy = new StringBuilder();

                            // Creating the business keys, multiple times because it's a Link
                            // Assign groups (counter) to allow for SAL
                            int groupCounter = 1;

                            foreach (DataRow hubDetailRow in hubTables.Rows)
                            {
                                sqlStatementForComponent.Clear();

                                var hubTableName = (string)hubDetailRow["HUB_NAME"];
                                var businessKeyDefinition = (string) hubDetailRow["BUSINESS_KEY_DEFINITION"];

                                // Construct the join clauses, where clauses etc. for the Hubs
                                var queryClauses = GetHubClauses(stagingAreaTableName, hubTableName, businessKeyDefinition, groupCounter.ToString());

                                hubQuerySelect.AppendLine(queryClauses[0]);
                                hubQueryWhere.AppendLine(queryClauses[1]);
                                hubQueryGroupBy.AppendLine(queryClauses[2]);

                                hubQuerySelect.Remove(hubQuerySelect.Length - 3, 3);
                                hubQueryWhere.Remove(hubQueryWhere.Length - 3, 3);
                                hubQueryGroupBy.Remove(hubQueryGroupBy.Length - 3, 3);
                                
                                groupCounter++;
                            } // End of business key creation

                            //Troubleshooting
                            if (hubQuerySelect.ToString() == "")
                            {
                                SetTextLink("Keys missing, please check the metadata for table " + currentTableName + "\r\n");
                            }

                            // Initiating select statement for subquery
                            sqlSourceStatement.AppendLine("  SELECT ");
                            sqlSourceStatement.AppendLine("   " + hubQuerySelect);
                            sqlSourceStatement.AppendLine("    " + TeamConfigurationSettings.RecordSourceAttribute + ",");

                            if (degenerateLinkAttributes != null && degenerateLinkAttributes.Rows.Count > 0)
                            {
                                foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                                {
                                    sqlSourceStatement.AppendLine(
                                        "    [" + (string) attribute["SOURCE_ATTRIBUTE_NAME"] + "] AS [" +
                                        (string) attribute["LINK_ATTRIBUTE_NAME"] + "],");
                                }
                            }

                            sqlSourceStatement.AppendLine("    MIN(" + TeamConfigurationSettings.LoadDateTimeAttribute + ") AS " + TeamConfigurationSettings.LoadDateTimeAttribute +"");
                            sqlSourceStatement.AppendLine("  FROM [" + TeamConfigurationSettings.PsaDatabaseName + "].[" + TeamConfigurationSettings.SchemaName + "].[" + currentTableName + "]");
                            sqlSourceStatement.AppendLine("  WHERE");
                            sqlSourceStatement.AppendLine(" " + hubQueryWhere);
                            sqlSourceStatement.Remove(sqlSourceStatement.Length - 6, 6);
                            sqlSourceStatement.AppendLine();

                            if (string.IsNullOrEmpty(filterCriteria))
                            {
                            }
                            else
                            {
                                sqlSourceStatement.AppendLine("    AND " + filterCriteria);
                            }
                            sqlSourceStatement.AppendLine("  GROUP BY ");
                            sqlSourceStatement.AppendLine("" + hubQueryGroupBy);

                            // Add degenerate attributes
                            if (degenerateLinkAttributes != null && degenerateLinkAttributes.Rows.Count > 0)
                            {
                                foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                                {
                                    sqlSourceStatement.AppendLine(
                                        "    [" + (string) attribute["SOURCE_ATTRIBUTE_NAME"] + "],");
                                }
                            }

                            sqlSourceStatement.AppendLine("    " + TeamConfigurationSettings.RecordSourceAttribute);

                            linkView.AppendLine(sqlSourceStatement.ToString());
                            linkView.Remove(linkView.Length -4, 4);
                            linkView.AppendLine();

                            // Add a union if there's more to add
                            if (rowcounter < linkStagingTables.Rows.Count)
                            {                            
                                linkView.AppendLine("UNION");
                            }
                            rowcounter++;
                        }

                        // End of subqueries / UNION statement
                        linkView.AppendLine(") LINK_selection");
                        linkView.AppendLine("GROUP BY");

                        hubKeycounter = 1; // This is to make sure the orders between source and target keys are in sync
                        foreach (var hubTargetBusinessKey in linkBusinessKeyList)
                        {
                            foreach (var hubArray in hubFullBusinessKeyList)
                            {
                                var hubBusinessKeyName = hubArray[1];
                                var hubGroupCounter = hubArray[3];

                                if (hubGroupCounter == hubKeycounter.ToString())
                                {
                                    linkView.AppendLine("  [" + hubBusinessKeyName + "],");
                                }
                            }
                            hubKeycounter++;
                        }

                        if (degenerateLinkAttributes != null && degenerateLinkAttributes.Rows.Count > 0)
                        {
                            foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                            {
                                linkView.AppendLine("  [" + (string) attribute["LINK_ATTRIBUTE_NAME"] + "],");
                            }
                        }

                        linkView.AppendLine("  [" + TeamConfigurationSettings.RecordSourceAttribute + "]");
                        linkView.AppendLine(") link");
                        linkView.AppendLine("WHERE ROW_NR=1");

                        
                        SetTextLink(@"Processing Link entity view for " + linkTableName + "\r\n");
       
                        // Create the file logging
                        using (
                            var outfile =
                                new StreamWriter(textBoxOutputPath.Text + @"\VIEW_" + linkTableName + ".sql"))
                        {
                            outfile.Write(linkView.ToString());
                            outfile.Close();
                        }

                        // Generate into the database
                        if (checkBoxGenerateInDatabase.Checked)
                        {
                            connHstg.ConnectionString = TeamConfigurationSettings.ConnectionStringHstg;
                            int insertError = GenerateInDatabase(connHstg, linkView.ToString());
                            errorCounter = errorCounter + insertError;
                        }

                        SetTextDebug(linkView.ToString());
                        SetTextDebug("\n");

                    }

                    connOmd.Close();
                    connStg.Close();
                    connInt.Close();
                }
            }
            else
            {
                SetTextLink("There was no metadata selected to create Link views. Please check the metadata schema - are there any Links selected?");
            }

            SetTextLink($"\r\n{errorCounter} errors have been found.\r\n");
            SetTextLink($"SQL Scripts have been successfully saved in {VedwConfigurationSettings.VedwOutputPath}.\r\n");
        }

        private DataTable GetBusinessKeyElementsBase (string stagingAreaTableName, string hubTableName, string businessKeyDefinition)
        {
            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            var sqlStatementForSourceBusinessKey = new StringBuilder();

            businessKeyDefinition = businessKeyDefinition.Replace("'", "''");

            sqlStatementForSourceBusinessKey.AppendLine("SELECT * FROM interface.INTERFACE_BUSINESS_KEY_COMPONENT_PART");
            sqlStatementForSourceBusinessKey.AppendLine("WHERE SOURCE_NAME = '" + stagingAreaTableName + "'");
            sqlStatementForSourceBusinessKey.AppendLine("  AND HUB_NAME= '" + hubTableName + "'");
            sqlStatementForSourceBusinessKey.AppendLine("  AND BUSINESS_KEY_DEFINITION = '" + businessKeyDefinition + "'");
            sqlStatementForSourceBusinessKey.AppendLine("ORDER BY BUSINESS_KEY_COMPONENT_ORDER, BUSINESS_KEY_COMPONENT_ELEMENT_ORDER");

            var elementListBase = GetDataTable(ref connOmd, sqlStatementForSourceBusinessKey.ToString());
            return elementListBase;
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

        private DataTable GetDegenerateLinkAttributes(string targetTableName)
        {
            var conn = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };

            try
            {
                conn.Open();
            }
            catch (Exception exception)
            {
                SetTextDebug("An error has occurred selecting Link table degenerate attributes: " + conn.ConnectionString + "). The associated message is " + exception.Message);

            }

            // Query to select degenerate attributes
            var degenerateLinkAttributeQuery = new StringBuilder();

            degenerateLinkAttributeQuery.AppendLine("SELECT ");
            degenerateLinkAttributeQuery.AppendLine("  SOURCE_ATTRIBUTE_NAME,");
            degenerateLinkAttributeQuery.AppendLine("  LINK_ATTRIBUTE_NAME");
            degenerateLinkAttributeQuery.AppendLine("FROM [interface].[INTERFACE_SOURCE_LINK_ATTRIBUTE_XREF]");
            degenerateLinkAttributeQuery.AppendLine("WHERE LINK_NAME = '" + targetTableName + "'");

            var degenerateLinkAttributes = GetDataTable(ref conn, degenerateLinkAttributeQuery.ToString());
            return degenerateLinkAttributes;
        }

        private DataTable GetHubLinkCombination(string stagingAreaTableName, string linkTableName)
        {
            

            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };

            // Get the Hubs for each Link/STG combination - both need to be represented in the query
            var queryHubGen = "SELECT * FROM [interface].[INTERFACE_HUB_LINK_XREF] " +
                              "WHERE SOURCE_NAME = '" + stagingAreaTableName + "'" +
                              "  AND LINK_NAME = '" + linkTableName + "'" +
                              " ORDER BY HUB_ORDER, BUSINESS_KEY_DEFINITION";

            var hubTables = GetDataTable(ref connOmd, queryHubGen);
            return hubTables;
        }


        public DataTable GetHubTargetBusinessKeyList(string hubTableName)
        {
            // Obtain the business key as it is known in the target Hub table. Can be multiple due to composite keys
            var conn = new SqlConnection
            {
                ConnectionString = checkBoxIgnoreVersion.Checked ? TeamConfigurationSettings.ConnectionStringInt : TeamConfigurationSettings.ConnectionStringOmd
            };

            try
            {
                conn.Open();
            }
            catch (Exception exception)
            {
                SetTextDebug("An error has occurred defining the Hub Business Key in the model due to connectivity issues (connection string " + conn.ConnectionString + "). The associated message is " + exception.Message);
            }

            var sqlStatementForHubBusinessKeys = new StringBuilder();

            var keyText = TeamConfigurationSettings.DwhKeyIdentifier;
            var localkeyLength = keyText.Length;
            var localkeySubstring = localkeyLength + 1;

            if (checkBoxIgnoreVersion.Checked)
            {
                // Make sure the live database is hit when the checkbox is ticked
                sqlStatementForHubBusinessKeys.AppendLine("SELECT COLUMN_NAME");
                sqlStatementForHubBusinessKeys.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS");
                sqlStatementForHubBusinessKeys.AppendLine("WHERE SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" + localkeyLength + "," + localkeySubstring + ")!='_" + TeamConfigurationSettings.DwhKeyIdentifier +"'");
                sqlStatementForHubBusinessKeys.AppendLine("AND TABLE_SCHEMA = '" + TeamConfigurationSettings.SchemaName + "'");
                sqlStatementForHubBusinessKeys.AppendLine("  AND TABLE_NAME= '" + hubTableName + "'");
                sqlStatementForHubBusinessKeys.AppendLine("  AND COLUMN_NAME NOT IN ('" + TeamConfigurationSettings.RecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" +
                                                          TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "','" + TeamConfigurationSettings.EtlProcessAttribute + "','" + TeamConfigurationSettings.LoadDateTimeAttribute + "')");
            }
            else
            {
                //Ignore version is not checked, so versioning is used - meaning the business key metadata is sourced from the version history metadata.
                sqlStatementForHubBusinessKeys.AppendLine("SELECT COLUMN_NAME");
                sqlStatementForHubBusinessKeys.AppendLine("FROM [interface].[INTERFACE_PHYSICAL_MODEL]");
                sqlStatementForHubBusinessKeys.AppendLine("WHERE SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" + localkeyLength + "," + localkeySubstring + ")!='_" + TeamConfigurationSettings.DwhKeyIdentifier + "'");
                sqlStatementForHubBusinessKeys.AppendLine("  AND TABLE_NAME= '" + hubTableName + "'");
                sqlStatementForHubBusinessKeys.AppendLine("  AND COLUMN_NAME NOT IN ('" + TeamConfigurationSettings.RecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" + TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "','" +
                                                          TeamConfigurationSettings.EtlProcessAttribute + "','" + TeamConfigurationSettings.LoadDateTimeAttribute + "')");
                //sqlStatementForHubBusinessKeys.AppendLine("  AND VERSION_ID = " + versionId + "");
            }


            var hubKeyList = GetDataTable(ref conn, sqlStatementForHubBusinessKeys.ToString());

            if (hubKeyList == null)
            {
                SetTextDebug("An error has occurred defining the Hub Business Key in the model for " + hubTableName + ". The Business Key was not found when querying the underlying metadata. This can be either that the attribute is missing in the metadata or in the table (depending if versioning is used). If the 'ignore versioning' option is checked, then the metadata will be retrieved directly from the data dictionary. Otherwise the metadata needs to be available in the repository (manage model metadata).");
            }

            return hubKeyList;
        }

        public LinkedList<string[]> GetLinkTargetBusinessKeyList(string linkTableName)
        {
            // Obtain the business keys are they are known in the target Link table. Can be different due to same-as links etc.
            var conn = new SqlConnection
            {
                ConnectionString = checkBoxIgnoreVersion.Checked ? TeamConfigurationSettings.ConnectionStringInt : TeamConfigurationSettings.ConnectionStringOmd
            };

            try
            {
                conn.Open();
            }
            catch (Exception exception)
            {
                SetTextDebug("An error has occurred defining the Hub Business Key in the model due to connectivity issues (connection string " + conn.ConnectionString + "). The associated message is " + exception.Message);
            }

            var sqlStatementForLinkBusinessKeys = new StringBuilder();

            if (checkBoxIgnoreVersion.Checked)
            {
                // Make sure the live database is hit when the checkbox is ticked
                sqlStatementForLinkBusinessKeys.AppendLine("SELECT COLUMN_NAME");
                sqlStatementForLinkBusinessKeys.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS");
                sqlStatementForLinkBusinessKeys.AppendLine("WHERE TABLE_NAME= '" + linkTableName + "'");
                sqlStatementForLinkBusinessKeys.AppendLine("AND TABLE_SCHEMA = '" + TeamConfigurationSettings.SchemaName + "'");
                sqlStatementForLinkBusinessKeys.AppendLine("  AND COLUMN_NAME NOT IN ('" + TeamConfigurationSettings.RecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" +
                                                          TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "','" + TeamConfigurationSettings.EtlProcessAttribute + "','" + TeamConfigurationSettings.LoadDateTimeAttribute + "')");
                sqlStatementForLinkBusinessKeys.AppendLine("ORDER BY ORDINAL_POSITION");
            }
            else
            {
                //Ignore version is not checked, so versioning is used - meaning the business key metadata is sourced from the version history metadata.
                sqlStatementForLinkBusinessKeys.AppendLine("SELECT COLUMN_NAME");
                sqlStatementForLinkBusinessKeys.AppendLine("FROM [interface].[INTERFACE_PHYSICAL_MODEL]");
                sqlStatementForLinkBusinessKeys.AppendLine("WHERE TABLE_NAME= '" + linkTableName + "'");
                sqlStatementForLinkBusinessKeys.AppendLine("  AND COLUMN_NAME NOT IN ('" + TeamConfigurationSettings.RecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" + TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "','" +
                                                          TeamConfigurationSettings.EtlProcessAttribute + "','" + TeamConfigurationSettings.LoadDateTimeAttribute + "')");
                //sqlStatementForLinkBusinessKeys.AppendLine("  AND VERSION_ID = " + versionId + "");
                sqlStatementForLinkBusinessKeys.AppendLine("ORDER BY ORDINAL_POSITION");
            }


            var linkKeyListDataTable = GetDataTable(ref conn, sqlStatementForLinkBusinessKeys.ToString());

            if (linkKeyListDataTable == null)
            {
                SetTextDebug("An error has occurred defining the Business Keys in the model for " + linkTableName +
                             ". The Business Keys were not found when querying the underlying metadata. This can be either that the attribute is missing in the metadata or in the table (depending if versioning is used). If the 'ignore versioning' option is checked, then the metadata will be retrieved directly from the data dictionary. Otherwise the metadata needs to be available in the repository (manage model metadata).");
            }
            else
            {

                var linkKeyList = new LinkedList<string[]>();
                var shortlinkTableKeyName = linkTableName.Replace("LNK_", "")+ "_" +TeamConfigurationSettings.DwhKeyIdentifier;

                foreach (DataRow linkKey in linkKeyListDataTable.Rows)
                {
                    if (!linkKey["COLUMN_NAME"].ToString().Contains(shortlinkTableKeyName) && linkKey["COLUMN_NAME"].ToString().Contains(TeamConfigurationSettings.DwhKeyIdentifier)) // Removing Link SK and degenerate fields
                    {
                        linkKeyList.AddLast
                            (
                                new[]
                                {
                                    linkKey["COLUMN_NAME"].ToString()
                                }
                            );
                    }
                }

                return linkKeyList;
            }

            return null;
        }

        private LinkedList<string[]> GetHubTablesForLink(string targetTableName)
        {
            

            // First, get the associated Hub tables for the Link
            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            var queryLinkHubTables = new StringBuilder();
            int groupCounter = 1;

            queryLinkHubTables.AppendLine("SELECT DISTINCT ");
            queryLinkHubTables.AppendLine(" [LINK_ID]");
            queryLinkHubTables.AppendLine(",[LINK_NAME]");
            queryLinkHubTables.AppendLine(",[HUB_ID]");
            queryLinkHubTables.AppendLine(",[HUB_NAME]");
            queryLinkHubTables.AppendLine(",[HUB_ORDER]");
            queryLinkHubTables.AppendLine("FROM [interface].[INTERFACE_HUB_LINK_XREF]");
            queryLinkHubTables.AppendLine("WHERE LINK_NAME = '" + targetTableName + "'");
            queryLinkHubTables.AppendLine("ORDER BY HUB_ORDER");

            var linkHubTables = GetDataTable(ref connOmd, queryLinkHubTables.ToString());

            // Now, retrieve the target business key attributes for the Hubs associated with the Link
            // This is either from the catalog (ignore version) or metadata (use version)
            var conn = new SqlConnection
            {
                ConnectionString =
                    checkBoxIgnoreVersion.Checked ? TeamConfigurationSettings.ConnectionStringInt : TeamConfigurationSettings.ConnectionStringOmd
            };

            var hubTargetBusinessKeyListForLink = new LinkedList<string[]>();

            foreach (DataRow hubTable in linkHubTables.Rows)
            {
                var queryHubBusinessKeys = new StringBuilder();

                var localKey = TeamConfigurationSettings.DwhKeyIdentifier;
                var localHubPrefix = TeamConfigurationSettings.HubTablePrefixValue;
                var localkeyLength = localKey.Length;
                var localkeySubstring = localkeyLength + 1;
                var localHubPrefixLength = localHubPrefix.Length + 1;

                if (checkBoxIgnoreVersion.Checked)
                {
                    queryHubBusinessKeys.AppendLine("SELECT a.TABLE_NAME, a.COLUMN_NAME, b.TOTALROWS");
                    queryHubBusinessKeys.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS a");
                    queryHubBusinessKeys.AppendLine("JOIN ");
                    queryHubBusinessKeys.AppendLine(" (SELECT TABLE_NAME, COUNT(*) AS TOTALROWS");
                    queryHubBusinessKeys.AppendLine("  FROM   INFORMATION_SCHEMA.COLUMNS");
                    queryHubBusinessKeys.AppendLine("  WHERE SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" + localkeyLength + "," + localkeySubstring + ")!='_" + TeamConfigurationSettings.DwhKeyIdentifier + "'");
                    queryHubBusinessKeys.AppendLine("   AND SUBSTRING(TABLE_NAME,1," + localHubPrefixLength + ")='" + TeamConfigurationSettings.HubTablePrefixValue + "_'");
                    queryHubBusinessKeys.AppendLine("   AND COLUMN_NAME NOT IN ('" + TeamConfigurationSettings.RecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" +TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" +TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "','" +TeamConfigurationSettings.EtlProcessAttribute + "','" + TeamConfigurationSettings.LoadDateTimeAttribute + "')");
                    queryHubBusinessKeys.AppendLine("   AND TABLE_SCHEMA = '" + TeamConfigurationSettings.SchemaName + "'");
                    queryHubBusinessKeys.AppendLine("	 GROUP BY TABLE_NAME");
                    queryHubBusinessKeys.AppendLine("	) b");
                    queryHubBusinessKeys.AppendLine("ON a.TABLE_NAME=b.TABLE_NAME");
                    queryHubBusinessKeys.AppendLine("WHERE SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" + localkeyLength +"," +localkeySubstring + ")!='_" + TeamConfigurationSettings.DwhKeyIdentifier + "'");queryHubBusinessKeys.AppendLine("  AND a.TABLE_NAME= '" + (string)hubTable["HUB_NAME"] + "'");
                    queryHubBusinessKeys.AppendLine("  AND COLUMN_NAME NOT IN ('" + TeamConfigurationSettings.RecordSourceAttribute + "','" +TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" +TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" +TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "','" +TeamConfigurationSettings.EtlProcessAttribute + "','" + TeamConfigurationSettings.LoadDateTimeAttribute + "')");
                    queryHubBusinessKeys.AppendLine("  AND TABLE_SCHEMA = '" + TeamConfigurationSettings.SchemaName + "'");
                }
                else //Get the key details from the metadata
                {
                    queryHubBusinessKeys.AppendLine("SELECT a.TABLE_NAME, a.COLUMN_NAME, b.TOTALROWS");
                    queryHubBusinessKeys.AppendLine("FROM [interface].[INTERFACE_PHYSICAL_MODEL] a");
                    queryHubBusinessKeys.AppendLine("JOIN ");
                    queryHubBusinessKeys.AppendLine("	(SELECT TABLE_NAME, COUNT(*) AS TOTALROWS");
                    queryHubBusinessKeys.AppendLine("	 FROM [interface].[INTERFACE_PHYSICAL_MODEL]");
                    //queryHubBusinessKeys.AppendLine("	 WHERE VERSION_ID = " + versionId);
                    queryHubBusinessKeys.AppendLine("      WHERE SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" + localkeyLength +"," +localkeySubstring + ")!='_" + TeamConfigurationSettings.DwhKeyIdentifier + "'");queryHubBusinessKeys.AppendLine("	   AND SUBSTRING(TABLE_NAME,1," + localHubPrefixLength + ")='" +TeamConfigurationSettings.HubTablePrefixValue + "_'");
                    queryHubBusinessKeys.AppendLine("      AND COLUMN_NAME NOT IN ('" + TeamConfigurationSettings.RecordSourceAttribute + "','" +TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" +TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" +TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "','" +TeamConfigurationSettings.EtlProcessAttribute + "','" + TeamConfigurationSettings.LoadDateTimeAttribute + "')");
                    queryHubBusinessKeys.AppendLine("	 GROUP BY TABLE_NAME");
                    queryHubBusinessKeys.AppendLine("	) b");
                    queryHubBusinessKeys.AppendLine("ON a.TABLE_NAME=b.TABLE_NAME");
                    //queryHubBusinessKeys.AppendLine("WHERE VERSION_ID = " + versionId);
                    queryHubBusinessKeys.AppendLine("WHERE SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" + localkeyLength +"," +localkeySubstring + ")!='_" + TeamConfigurationSettings.DwhKeyIdentifier + "'");
                    queryHubBusinessKeys.AppendLine("  AND a.TABLE_NAME= '" + (string)hubTable["HUB_NAME"] + "'");
                    queryHubBusinessKeys.AppendLine("  AND COLUMN_NAME NOT IN ('" + TeamConfigurationSettings.RecordSourceAttribute + "','" +TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" +TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" +TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "','" +TeamConfigurationSettings.EtlProcessAttribute + "','" + TeamConfigurationSettings.LoadDateTimeAttribute + "')");
                }

                var hubKeyList = GetDataTable(ref conn, queryHubBusinessKeys.ToString());


                foreach (DataRow hubBusinessKeyAttribute in hubKeyList.Rows)
                {
                    hubTargetBusinessKeyListForLink.AddLast
                        (
                            new[]
                            {
                                hubBusinessKeyAttribute["TABLE_NAME"].ToString(),
                                hubBusinessKeyAttribute["COLUMN_NAME"]+groupCounter.ToString(),
                                hubBusinessKeyAttribute["TOTALROWS"].ToString(),
                                groupCounter.ToString()
                            }
                        );
                    
                }

                groupCounter++;
            }
            return hubTargetBusinessKeyListForLink;
        }

        private LinkedList<string[]> GetAllHubTablesForLink(string targetTableName)
        {
            

            // First, get the associated Hub tables for the Link
            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            var queryLinkHubTables = new StringBuilder();
            int groupCounter = 1;

            queryLinkHubTables.AppendLine("SELECT DISTINCT ");
            queryLinkHubTables.AppendLine(" [LINK_ID]");
            queryLinkHubTables.AppendLine(",[LINK_NAME]");
            queryLinkHubTables.AppendLine(",[SOURCE_ID]");
            queryLinkHubTables.AppendLine(",[SOURCE_NAME]");
            queryLinkHubTables.AppendLine(",[SOURCE_SCHEMA_NAME]");
            queryLinkHubTables.AppendLine(",[HUB_ID]");
            queryLinkHubTables.AppendLine(",[HUB_NAME]");
            queryLinkHubTables.AppendLine(",[HUB_ORDER]");
            queryLinkHubTables.AppendLine("FROM [interface].[INTERFACE_HUB_LINK_XREF]");
            queryLinkHubTables.AppendLine("WHERE LINK_NAME = '" + targetTableName + "'");
            queryLinkHubTables.AppendLine("ORDER BY [HUB_ORDER]");

            var linkHubTables = GetDataTable(ref connOmd, queryLinkHubTables.ToString());

            // Now, retrieve the target business key attributes for the Hubs associated with the Link
            // This is either from the catalog (ignore version) or metadata (use version)
            var conn = new SqlConnection
            {
                ConnectionString =
                    checkBoxIgnoreVersion.Checked ? TeamConfigurationSettings.ConnectionStringInt : TeamConfigurationSettings.ConnectionStringOmd
            };

            var hubTargetBusinessKeyListForLink = new LinkedList<string[]>();

            foreach (DataRow hubTable in linkHubTables.Rows)
            {
                var queryHubBusinessKeys = new StringBuilder();

                var localKey = TeamConfigurationSettings.DwhKeyIdentifier;
                var localHubPrefix = TeamConfigurationSettings.HubTablePrefixValue;
                var localkeyLength = localKey.Length;
                var localkeySubstring = localkeyLength + 1;
                var localHubPrefixLength = localHubPrefix.Length + 1;

                if (checkBoxIgnoreVersion.Checked)
                {
                    queryHubBusinessKeys.AppendLine("SELECT a.TABLE_NAME, a.COLUMN_NAME, b.TOTALROWS");
                    queryHubBusinessKeys.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS a");
                    queryHubBusinessKeys.AppendLine("JOIN ");
                    queryHubBusinessKeys.AppendLine(" (SELECT TABLE_NAME, COUNT(*) AS TOTALROWS");
                    queryHubBusinessKeys.AppendLine("  FROM   INFORMATION_SCHEMA.COLUMNS");
                    queryHubBusinessKeys.AppendLine("  WHERE SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" + localkeyLength + "," + localkeySubstring + ")!='_" + TeamConfigurationSettings.DwhKeyIdentifier + "'");
                    queryHubBusinessKeys.AppendLine("   AND SUBSTRING(TABLE_NAME,1," + localHubPrefixLength + ")='" + TeamConfigurationSettings.HubTablePrefixValue + "_'");
                    queryHubBusinessKeys.AppendLine("   AND COLUMN_NAME NOT IN ('" + TeamConfigurationSettings.RecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" + TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "','" + TeamConfigurationSettings.EtlProcessAttribute + "','" + TeamConfigurationSettings.LoadDateTimeAttribute + "')");
                    queryHubBusinessKeys.AppendLine("   AND TABLE_SCHEMA = '" + TeamConfigurationSettings.SchemaName + "'");
                    queryHubBusinessKeys.AppendLine("	 GROUP BY TABLE_NAME");
                    queryHubBusinessKeys.AppendLine("	) b");
                    queryHubBusinessKeys.AppendLine("ON a.TABLE_NAME=b.TABLE_NAME");
                    queryHubBusinessKeys.AppendLine("WHERE SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" + localkeyLength + "," + localkeySubstring + ")!='_" + TeamConfigurationSettings.DwhKeyIdentifier + "'"); queryHubBusinessKeys.AppendLine("  AND a.TABLE_NAME= '" + (string)hubTable["HUB_NAME"] + "'");
                    queryHubBusinessKeys.AppendLine("  AND COLUMN_NAME NOT IN ('" + TeamConfigurationSettings.RecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" + TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "','" + TeamConfigurationSettings.EtlProcessAttribute + "','" + TeamConfigurationSettings.LoadDateTimeAttribute + "')");
                    queryHubBusinessKeys.AppendLine("AND TABLE_SCHEMA = '" + TeamConfigurationSettings.SchemaName + "'");
                }
                else //Get the key details from the metadata
                {
                    queryHubBusinessKeys.AppendLine("SELECT a.TABLE_NAME, a.COLUMN_NAME, b.TOTALROWS");
                    queryHubBusinessKeys.AppendLine("FROM [interface].[INTERFACE_PHYSICAL_MODEL] a");
                    queryHubBusinessKeys.AppendLine("JOIN ");
                    queryHubBusinessKeys.AppendLine("	(SELECT TABLE_NAME, COUNT(*) AS TOTALROWS");
                    queryHubBusinessKeys.AppendLine("	 FROM   [interface].[INTERFACE_PHYSICAL_MODEL]");
                    //queryHubBusinessKeys.AppendLine("	 WHERE VERSION_ID = " + versionId);
                    queryHubBusinessKeys.AppendLine("      WHERE SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" + localkeyLength + "," + localkeySubstring + ")!='_" + TeamConfigurationSettings.DwhKeyIdentifier + "'"); queryHubBusinessKeys.AppendLine("	   AND SUBSTRING(TABLE_NAME,1," + localHubPrefixLength + ")='" + TeamConfigurationSettings.HubTablePrefixValue + "_'");
                    queryHubBusinessKeys.AppendLine("      AND COLUMN_NAME NOT IN ('" + TeamConfigurationSettings.RecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" + TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "','" + TeamConfigurationSettings.EtlProcessAttribute + "','" + TeamConfigurationSettings.LoadDateTimeAttribute + "')");
                    queryHubBusinessKeys.AppendLine("	 GROUP BY TABLE_NAME");
                    queryHubBusinessKeys.AppendLine("	) b");
                    queryHubBusinessKeys.AppendLine("ON a.TABLE_NAME=b.TABLE_NAME");
                    //queryHubBusinessKeys.AppendLine("WHERE VERSION_ID = " + versionId);
                    queryHubBusinessKeys.AppendLine("WHERE SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" + localkeyLength + "," + localkeySubstring + ")!='_" + TeamConfigurationSettings.DwhKeyIdentifier + "'");
                    queryHubBusinessKeys.AppendLine("  AND a.TABLE_NAME= '" + (string)hubTable["HUB_NAME"] + "'");
                    queryHubBusinessKeys.AppendLine("  AND COLUMN_NAME NOT IN ('" + TeamConfigurationSettings.RecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" + TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "','" + TeamConfigurationSettings.EtlProcessAttribute + "','" + TeamConfigurationSettings.LoadDateTimeAttribute + "')");
                }

                var hubKeyList = GetDataTable(ref conn, queryHubBusinessKeys.ToString());


                foreach (DataRow hubBusinessKeyAttribute in hubKeyList.Rows)
                {
                    hubTargetBusinessKeyListForLink.AddLast
                        (
                            new[]
                            {
                                hubBusinessKeyAttribute["TABLE_NAME"].ToString(),
                                hubBusinessKeyAttribute["COLUMN_NAME"]+groupCounter.ToString(),
                                hubBusinessKeyAttribute["TOTALROWS"].ToString(),
                                groupCounter.ToString()
                            }
                        );

                }

                groupCounter++;
            }
            return hubTargetBusinessKeyListForLink;
        }

        private void LinkSatelliteButtonClick (object sender, EventArgs e)
        {
            // Check if the schema needs to be created
            CreateSchema(TeamConfigurationSettings.ConnectionStringInt);

            richTextBoxLsat.Clear();
            richTextBoxInformation.Clear();

            var newThread = new Thread(BackgroundDoLsat);
            newThread.Start();
        }

        private void BackgroundDoLsat(Object obj)
        {
          
            if (radiobuttonViews.Checked)
            {
                GenerateLsatHistoryViews();
                GenerateLsatDrivingKeyViews();
            }
            else if (radiobuttonStoredProc.Checked)
            {
                MessageBox.Show("This feature is yet to be implemented.", "Upcoming!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (radioButtonIntoStatement.Checked)
            {
                GenerateLsatInsertInto();
            }
        }

        private void GenerateLsatInsertInto()
        {
            int errorCounter = 0;

            if (checkBoxIfExistsStatement.Checked)
            {
                SetTextLsat("The Drop If Exists checkbox has been checked, but this feature is not relevant for this specific operation and will be ignored. \n\r");
            }

            // Determine metadata retrieval connection (dependant on option selected)
            if (checkedListBoxLsatMetadata.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= checkedListBoxLsatMetadata.CheckedItems.Count - 1; x++)
                {
                    var connHstg = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringHstg };
                    var connOmd = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringOmd};

                    var conn = new SqlConnection
                    {
                        ConnectionString = checkBoxIgnoreVersion.Checked ? TeamConfigurationSettings.ConnectionStringInt : TeamConfigurationSettings.ConnectionStringOmd
                    };

                    var targetTableName = checkedListBoxLsatMetadata.CheckedItems[x].ToString();

                    //string queryTableArray = "SELECT " +
                    //                               "   a.SOURCE_ID, " +
                    //                               "   c.SOURCE_NAME, " +
                    //                               "   SATELLITE_ID, " +
                    //                               "   SATELLITE_NAME, " +
                    //                               "   b.LINK_ID, " +
                    //                               "   LINK_NAME " +
                    //                               "FROM MD_SAT a " +
                    //                               "JOIN MD_LINK b on a.LINK_ID=b.LINK_ID " +
                    //                               "JOIN MD_STG c on a.SOURCE_ID=c.SOURCE_ID " +
                    //                               "WHERE SATELLITE_TYPE IN ('Link Satellite', 'Link Satellite - Without Attributes')" +
                    //                               " AND SATELLITE_NAME = '" + targetTableName + "'";

                    var sqlStatementForTablesToImport = new StringBuilder();
                    sqlStatementForTablesToImport.AppendLine("SELECT * FROM interface.INTERFACE_SOURCE_SATELLITE_XREF base");
                    sqlStatementForTablesToImport.AppendLine("WHERE SATELLITE_TYPE = 'Link Satellite' ");
                    sqlStatementForTablesToImport.AppendLine(" AND SATELLITE_NAME = '" + targetTableName + "'");

                    var tables = GetDataTable(ref connOmd, sqlStatementForTablesToImport.ToString());

                    foreach (DataRow row in tables.Rows)
                    {
                        var linkSk = row["LINK_NAME"].ToString().Substring(4) + "_" + TeamConfigurationSettings.DwhKeyIdentifier;

                        // Build the main attribute list of the Satellite table for selection
                        var sourceTableStructure = GetTableStructure(targetTableName, ref conn, "LSAT");

                        // Query to detect multi-active attributes
                        var multiActiveAttributes = GetMultiActiveAttributes((int) row["SATELLITE_ID"]);

                        // Initial SQL
                        var insertIntoStatement = new StringBuilder();

                        insertIntoStatement.AppendLine("--");
                        insertIntoStatement.AppendLine("-- Link Satellite Insert Into statement for " + targetTableName);
                        insertIntoStatement.AppendLine("-- Generated at " + DateTime.Now);
                        insertIntoStatement.AppendLine("--");
                        insertIntoStatement.AppendLine();
                        insertIntoStatement.AppendLine("USE [" + TeamConfigurationSettings.PsaDatabaseName + "]");
                        insertIntoStatement.AppendLine("GO");
                        insertIntoStatement.AppendLine();
                        insertIntoStatement.AppendLine("INSERT INTO [" + TeamConfigurationSettings.IntegrationDatabaseName + "].[" + TeamConfigurationSettings.SchemaName + "].[" + targetTableName + "]");
                        insertIntoStatement.AppendLine("   (");

                        foreach (DataRow attribute in sourceTableStructure.Rows)
                        {
                            insertIntoStatement.AppendLine("   [" + attribute["COLUMN_NAME"] + "],");
                        }

                        insertIntoStatement.Remove(insertIntoStatement.Length - 3, 3);
                        insertIntoStatement.AppendLine();
                        insertIntoStatement.AppendLine("   )");
                        insertIntoStatement.AppendLine("SELECT ");

                        foreach (DataRow attribute in sourceTableStructure.Rows)
                        {
                            var sourceAttribute = attribute["COLUMN_NAME"];

                            if ((string) sourceAttribute == TeamConfigurationSettings.EtlProcessAttribute ||
                                (string) sourceAttribute == TeamConfigurationSettings.EtlProcessUpdateAttribute)
                            {
                                insertIntoStatement.Append("   -1 AS [" + sourceAttribute + "],");
                                insertIntoStatement.AppendLine();
                            }
                            else if ((string)attribute["COLUMN_NAME"] == TeamConfigurationSettings.AlternativeRecordSourceAttribute)
                            {
                                insertIntoStatement.AppendLine("   CHECKSUM(lsat_view." + TeamConfigurationSettings.AlternativeRecordSourceAttribute + ") AS " + attribute["COLUMN_NAME"] + ",");
                            }
                            else
                            {
                                insertIntoStatement.Append("   lsat_view.[" + sourceAttribute + "],");
                                insertIntoStatement.AppendLine();
                            }
                        }

                        insertIntoStatement.Remove(insertIntoStatement.Length - 3, 3);
                        insertIntoStatement.AppendLine();
                        insertIntoStatement.AppendLine("FROM [" + VedwConfigurationSettings.VedwSchema + "]." + targetTableName + " lsat_view");
                        insertIntoStatement.AppendLine("LEFT OUTER JOIN");
                        insertIntoStatement.AppendLine("   [" + TeamConfigurationSettings.IntegrationDatabaseName + "].[" +TeamConfigurationSettings.SchemaName + "].[" + targetTableName + "] lsat_table");
                        insertIntoStatement.AppendLine(" ON lsat_view.[" + linkSk + "] = lsat_table.[" + linkSk + "]");

                        if (TeamConfigurationSettings.EnableAlternativeSatelliteLoadDateTimeAttribute == "True")
                        {
                            insertIntoStatement.AppendLine("AND lsat_view.[" + TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "] = lsat_table.[" + TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "]");
                        }
                        else
                        {
                            insertIntoStatement.AppendLine("AND lsat_view.[" + TeamConfigurationSettings.LoadDateTimeAttribute + "] = lsat_table.[" + TeamConfigurationSettings.LoadDateTimeAttribute + "]");
                        }

                        // Multi-active
                        foreach (DataRow attribute in multiActiveAttributes.Rows)
                        {
                            insertIntoStatement.AppendLine("AND lsat_view.[" + (string) attribute["SATELLITE_ATTRIBUTE_NAME"] +"] = lsat_table.[" + (string) attribute["SATELLITE_ATTRIBUTE_NAME"] +"]");
                        }

                        insertIntoStatement.AppendLine("WHERE lsat_table." + linkSk + " IS NULL");

                        using (
                            var outfile = new StreamWriter(textBoxOutputPath.Text + @"\INSERT_STATEMENT_" + targetTableName +".sql"))
                        {
                            outfile.Write(insertIntoStatement.ToString());
                            outfile.Close();
                        }

                        if (checkBoxGenerateInDatabase.Checked)
                        {
                            int insertError = GenerateInDatabase(connHstg, insertIntoStatement.ToString());
                            errorCounter = errorCounter + insertError;
                        }

                        SetTextDebug(insertIntoStatement.ToString());
                        SetTextDebug("\n");

                        SetTextLsat($"Processing Link Satellite Insert Into statement for {targetTableName}\r\n");
                    }
                    connHstg.Close();
                    conn.Close();
                }
            }
            else
            {
                SetTextLsat("There was no metadata selected to create Link Satellite insert statements. Please check the metadata schema - are there any Link Satellites selected?");
            }

            SetTextLsat($"\r\n{errorCounter} errors have been found.\r\n");
            SetTextLsat($"SQL Scripts have been successfully saved in {VedwConfigurationSettings.VedwOutputPath}.\r\n");
        }

        // Link Satellite generation - driving key based
        private void GenerateLsatDrivingKeyViews()
        {
            int errorCounter = 0;

            if (checkedListBoxLsatMetadata.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= checkedListBoxLsatMetadata.CheckedItems.Count - 1; x++)
                {
                    var stringDataType = checkBoxUnicode.Checked ? "NVARCHAR" : "VARCHAR";

                    var connOmd = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringOmd};
                    var connStg = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringStg};
                    var connHstg = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringHstg};

                    var targetTableName = checkedListBoxLsatMetadata.CheckedItems[x].ToString();

                    // Check if it's actually a driving 
                    var sqlStatementForTablesToImport = new StringBuilder();
                    sqlStatementForTablesToImport.AppendLine("SELECT * FROM interface.INTERFACE_SOURCE_SATELLITE_XREF base");
                    sqlStatementForTablesToImport.AppendLine("WHERE SATELLITE_TYPE = 'Link Satellite' ");
                    sqlStatementForTablesToImport.AppendLine(" AND SATELLITE_NAME = '" + targetTableName + "'");
                    sqlStatementForTablesToImport.AppendLine(" AND EXISTS (SELECT * FROM [interface].[INTERFACE_DRIVING_KEY] sub WHERE sub.SATELLITE_ID = base.SATELLITE_ID)");

                    var tables = GetDataTable(ref connOmd, sqlStatementForTablesToImport.ToString());

                    foreach (DataRow row in tables.Rows)
                        {
                            var linkSatView = new StringBuilder();

                            string multiActiveAttributeFromName;

                            var psaTableName = TeamConfigurationSettings.PsaTablePrefixValue + row["SOURCE_NAME"].ToString().Replace(TeamConfigurationSettings.StgTablePrefixValue, "");


                            var stagingAreaTableName = (string)row["SOURCE_NAME"];

                            var filterCriteria = (string) row["FILTER_CRITERIA"];

                            var targetTableId = (int) row["SATELLITE_ID"];

                            var linkTableName = (string) row["LINK_NAME"];

                            var linkSk = linkTableName.Substring(4) + "_" + TeamConfigurationSettings.DwhKeyIdentifier;

                            // Query to detect multi-active attributes
                            var multiActiveAttributes = GetMultiActiveAttributes(targetTableId);

                            // Get the associated Hub tables for the Link
                            var hubBusinessKeyList = GetHubTablesForLink(linkTableName);

                            // Retrieving the business key attributes for the Hubs associated with the Link
                            //var hubBusinessKeyList = GetHubBusinessKeysForLink(linkHubTables, versionId);

                            var degenerateLinkAttributes = GetDegenerateLinkAttributes(linkTableName);

                            // Create a list of Driving Key(s) for the Link table
                            var queryDrivingKeys = new StringBuilder();

                            queryDrivingKeys.AppendLine("SELECT ");
                            queryDrivingKeys.AppendLine("  [SATELLITE_ID]");
                            queryDrivingKeys.AppendLine(" ,[SATELLITE_NAME]");
                            queryDrivingKeys.AppendLine(" ,[HUB_ID]");
                            queryDrivingKeys.AppendLine(" ,[HUB_NAME]");
                            queryDrivingKeys.AppendLine("FROM [interface].[INTERFACE_DRIVING_KEY]");
                            queryDrivingKeys.AppendLine("WHERE SATELLITE_NAME = '"+ targetTableName+"'");

                            var drivingKeysDataTable = GetDataTable(ref connOmd, queryDrivingKeys.ToString());
                            drivingKeysDataTable.PrimaryKey = new[] { drivingKeysDataTable.Columns["HUB_NAME"]};

                            // **************************************************************************************		
                            // Creating the source query
                            // **************************************************************************************

                            // Create View
                            linkSatView.AppendLine("--");
                            linkSatView.AppendLine("-- Link Satellite View definition - Driving Key for " + targetTableName);
                            linkSatView.AppendLine("-- Generated at " + DateTime.Now);
                            linkSatView.AppendLine("--");
                            linkSatView.AppendLine();
                            linkSatView.AppendLine("USE [" + TeamConfigurationSettings.PsaDatabaseName + "]");
                            linkSatView.AppendLine("GO");
                            linkSatView.AppendLine();
                            linkSatView.AppendLine("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[" + VedwConfigurationSettings.VedwSchema + "].[" +targetTableName + "]') AND type in (N'V'))");
                            linkSatView.AppendLine("DROP VIEW [" + VedwConfigurationSettings.VedwSchema + "].[" + targetTableName + "]");
                            linkSatView.AppendLine("go");
                            linkSatView.AppendLine("CREATE VIEW [" + VedwConfigurationSettings.VedwSchema + "].[" + targetTableName +"] AS  ");
                            linkSatView.AppendLine("SELECT");

                            if (!checkBoxDisableHash.Checked)
                            {
                                linkSatView.AppendLine("   "+VedwConfigurationSettings.hashingStartSnippet);

                                foreach (var hubArray in hubBusinessKeyList)
                                {
                                    linkSatView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100),[" + hubArray[1] + "])),'NA')+'|'+");
                                }

                            // Add the degenerate attributes
                                if (degenerateLinkAttributes != null && degenerateLinkAttributes.Rows.Count > 0)
                                {
                                    foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                                    {
                                        linkSatView.AppendLine(
                                            "    ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," +
                                            (string) attribute["LINK_ATTRIBUTE_NAME"] + ")),'NA')+'|'+");
                                    }
                                }

                                linkSatView.Remove(linkSatView.Length - 3, 3);
                                linkSatView.AppendLine();
                                linkSatView.AppendLine("   "+VedwConfigurationSettings.hashingEndSnippet+" AS " + linkSk + ",");
                            }
                            else
                            {
                                //BK as DWH key
                                foreach (var hubArray in hubBusinessKeyList)
                                {
                                    linkSatView.Append("  ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + hubArray[1] + ")),'NA')+'|'+");
                                }

                                if (degenerateLinkAttributes != null && degenerateLinkAttributes.Rows.Count > 0)
                                {
                                    foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                                    {
                                        linkSatView.Append("    ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," +
                                                           (string) attribute["LINK_ATTRIBUTE_NAME"] + ")),'NA')+'|'+");
                                    }
                                }

                                linkSatView.Remove(linkSatView.Length - 5, 5);
                                linkSatView.Append("  AS " + linkSk + ",");
                                linkSatView.AppendLine();
                            }

                        // Add commented-out original business keys for easy debugging
                        foreach (var hubArray in hubBusinessKeyList)
                            {
                                linkSatView.AppendLine("   --[" + hubArray[1] + "],");
                            }

                            // Effective Datetime
                            if (TeamConfigurationSettings.EnableAlternativeSatelliteLoadDateTimeAttribute == "True")
                            {
                                linkSatView.AppendLine("   " + TeamConfigurationSettings.LoadDateTimeAttribute + " AS " +TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + ",");
                            }
                            else
                            {
                                linkSatView.AppendLine("   " + TeamConfigurationSettings.LoadDateTimeAttribute + " AS " + TeamConfigurationSettings.LoadDateTimeAttribute + ",");
                            }

                            //Multi-Active attributes
                            foreach (DataRow attribute in multiActiveAttributes.Rows)
                            {
                                linkSatView.AppendLine("   [" + (string)attribute["SATELLITE_ATTRIBUTE_NAME"] + "],");
                            }

                            // Expiry Datetime
                            linkSatView.AppendLine("   COALESCE ( LEAD ( " + TeamConfigurationSettings.LoadDateTimeAttribute + " ) OVER");
                            linkSatView.AppendLine("      (PARTITION BY ");

                            // Business Key attributes
                            foreach (var hubArray in hubBusinessKeyList)
                            {
                                var hubTableName = hubArray[0];
                                var hubBusinessKeyName = hubArray[1];
                                var foundRow = drivingKeysDataTable.Rows.Find(hubTableName);

                                if (foundRow != null)
                                {
                                    linkSatView.AppendLine("       [" + hubBusinessKeyName + "],");
                                }
                            }

                        // Driving Key attributes
                            if (degenerateLinkAttributes != null && degenerateLinkAttributes.Rows.Count > 0)
                            {
                                foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                                {
                                    linkSatView.AppendLine("    ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," +
                                                           (string) attribute["LINK_ATTRIBUTE_NAME"] + ")),'NA')+'|'+");
                                }
                            }

                            // Multi-active attributes
                            foreach (DataRow attribute in multiActiveAttributes.Rows)
                            {
                                multiActiveAttributeFromName = (string) attribute["SOURCE_ATTRIBUTE_NAME"];
                                linkSatView.AppendLine("       [" + multiActiveAttributeFromName + "],");
                            }

                            linkSatView.Remove(linkSatView.Length - 3, 3);
                            linkSatView.AppendLine();
                            linkSatView.AppendLine("   	  ORDER BY " + TeamConfigurationSettings.LoadDateTimeAttribute + "),");
                            linkSatView.AppendLine("      CAST( '9999-12-31' AS DATETIME)");
                            linkSatView.AppendLine("   ) AS " + TeamConfigurationSettings.ExpiryDateTimeAttribute + ",");

                            // Current record indicator
                            linkSatView.AppendLine("   CASE ");
                            linkSatView.AppendLine("     WHEN ( LEAD ( " + TeamConfigurationSettings.LoadDateTimeAttribute + " ) OVER");
                            linkSatView.AppendLine("      (PARTITION BY ");

                            foreach (var hubArray in hubBusinessKeyList)
                            {
                                var hubTableName = hubArray[0];
                                var hubBusinessKeyName = hubArray[1];
                                var foundRow = drivingKeysDataTable.Rows.Find(hubTableName);

                                if (foundRow != null)
                                {
                                    linkSatView.AppendLine("       [" + hubBusinessKeyName + "],");
                                }
                            }

                            foreach (DataRow attribute in multiActiveAttributes.Rows)
                            {
                                linkSatView.AppendLine("       [" + (string)attribute["SOURCE_ATTRIBUTE_NAME"] + "],");
                            }

                            linkSatView.Remove(linkSatView.Length - 3, 3);
                            linkSatView.AppendLine();
                            linkSatView.AppendLine("   	  ORDER BY " + TeamConfigurationSettings.LoadDateTimeAttribute + ")");
                            linkSatView.AppendLine("      ) IS NULL");
                            linkSatView.AppendLine("     THEN 'Y' ELSE 'N'");
                            linkSatView.AppendLine("   END AS " + TeamConfigurationSettings.CurrentRowAttribute + ",");

                            // Other process metadata attributes
                            if (TeamConfigurationSettings.EnableAlternativeRecordSourceAttribute == "True")
                            {
                                linkSatView.AppendLine("   [" + TeamConfigurationSettings.RecordSourceAttribute + "] AS ["+ TeamConfigurationSettings.AlternativeRecordSourceAttribute + "],");
                            }
                            else
                            {
                                linkSatView.AppendLine("   [" + TeamConfigurationSettings.RecordSourceAttribute + "],");
                            }

                            //Logical deletes
                            if (checkBoxEvaluateSatDelete.Checked)
                            {
                                linkSatView.AppendLine("    CASE");
                                linkSatView.AppendLine("      WHEN [" + TeamConfigurationSettings.ChangeDataCaptureAttribute + "] = 'Delete' THEN 'Y'");
                                linkSatView.AppendLine("      ELSE 'N'");
                                linkSatView.AppendLine("    END AS [" + TeamConfigurationSettings.LogicalDeleteAttribute + "],");
                            }

                            linkSatView.AppendLine("   [" + TeamConfigurationSettings.RowIdAttribute + "],");
                            linkSatView.AppendLine("   [" + TeamConfigurationSettings.ChangeDataCaptureAttribute + "],");

                            // Row number
                            linkSatView.AppendLine("   CAST(");
                            linkSatView.AppendLine("      ROW_NUMBER() OVER (PARTITION  BY ");

                            foreach (var hubArray in hubBusinessKeyList)
                            {
                                var hubTableName = hubArray[0];
                                var hubBusinessKeyName = hubArray[1];
                                var foundRow = drivingKeysDataTable.Rows.Find(hubTableName);

                                if (foundRow != null)
                                {
                                    linkSatView.AppendLine("          [" + hubBusinessKeyName + "],");
                                }
                            }

                            foreach (DataRow attribute in multiActiveAttributes.Rows)
                            {
                                multiActiveAttributeFromName = (string) attribute["SOURCE_ATTRIBUTE_NAME"];
                                linkSatView.AppendLine("          [" + multiActiveAttributeFromName + "],");
                            }

                            linkSatView.Remove(linkSatView.Length - 3, 3);
                            linkSatView.AppendLine();
                            linkSatView.AppendLine("         ORDER BY ");

                            foreach (var hubArray in hubBusinessKeyList)
                            {
                                var hubTableName = hubArray[0];
                                var hubBusinessKeyName = hubArray[1];
                                var foundRow = drivingKeysDataTable.Rows.Find(hubTableName);

                                if (foundRow != null)
                                {
                                    linkSatView.AppendLine("          [" + hubBusinessKeyName + "],");
                                }
                            }

                            foreach (DataRow attribute in multiActiveAttributes.Rows)
                            {
                                linkSatView.AppendLine("          [" + (string)attribute["SOURCE_ATTRIBUTE_NAME"] + "],");
                            }

                            linkSatView.AppendLine("          [" + TeamConfigurationSettings.LoadDateTimeAttribute + "]) AS INT)");
                            linkSatView.AppendLine("   AS ROW_NUMBER,");

                            // Checksum
                            linkSatView.AppendLine("   "+VedwConfigurationSettings.hashingStartSnippet);
                            linkSatView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100),[" +
                                                   TeamConfigurationSettings.ChangeDataCaptureAttribute + "])),'NA')+'|'+");

                            foreach (var hubArray in hubBusinessKeyList)
                            {
                                linkSatView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100),[" + hubArray[1] +"])),'NA')+'|'+");
                            }

                            foreach (DataRow attribute in multiActiveAttributes.Rows)
                            {
                                linkSatView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100),[" + (string)attribute["SOURCE_ATTRIBUTE_NAME"] + "])),'NA')+'|'+");
                            }

                            if (degenerateLinkAttributes != null && degenerateLinkAttributes.Rows.Count > 0)
                            {
                                foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                                {
                                    linkSatView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100),[" +
                                                           (string) attribute["LINK_ATTRIBUTE_NAME"] + "])),'NA')+'|'+");
                                }
                            }

                            linkSatView.Remove(linkSatView.Length - 3, 3);
                            linkSatView.AppendLine();
                            linkSatView.AppendLine("   "+VedwConfigurationSettings.hashingEndSnippet+" AS " + TeamConfigurationSettings.RecordChecksumAttribute + "");

                            // From statement
                            linkSatView.AppendLine("FROM ");
                            linkSatView.AppendLine("(");
                            linkSatView.AppendLine("  SELECT ");
                            linkSatView.AppendLine("    [" + TeamConfigurationSettings.LoadDateTimeAttribute + "],");
                            linkSatView.AppendLine("    [" + TeamConfigurationSettings.RecordSourceAttribute + "],");
                            linkSatView.AppendLine("    [" + TeamConfigurationSettings.RowIdAttribute + "],");
                            linkSatView.AppendLine("    [" + TeamConfigurationSettings.ChangeDataCaptureAttribute + "],");

                            var sqlStatementForComponent = new StringBuilder();
                            var sqlSourceStatement = new StringBuilder();
                            var sqlStatementForHubBusinessKeys = new StringBuilder();
                            var sqlStatementForSourceQuery = new StringBuilder();

                            var querySourceTableName = psaTableName;

                            // Get the Hubs for each Link/STG combination - both need to be represented in the query
                            var hubTables = GetHubLinkCombination(stagingAreaTableName, linkTableName);

                            var hubQuerySelect = new StringBuilder();

                            // Creating the business keys
                            var hubDrivingKeyPair = new LinkedList<string[]>();

                            // Assign groups (counter) to allow for SAL
                            int groupCounter = 1;
                            foreach (DataRow hubDetailRow in hubTables.Rows)
                            {
                                sqlStatementForComponent.Clear();
                                sqlStatementForHubBusinessKeys.Clear();

                                var fieldList = new StringBuilder();
                                var compositeKey = new StringBuilder();
                                var fieldDict = new Dictionary<string, string>();
                                var fieldOrderedList = new List<string>();
                                var firstKey = string.Empty;

                                var hubTableName = (string) hubDetailRow["HUB_NAME"];
                                var businessKeyDefinition = (string) hubDetailRow["BUSINESS_KEY_DEFINITION"];

                                var hubKeyList = GetHubTargetBusinessKeyList(hubTableName);

                                // Retrieving the top level component to evaluate composite, concat or pivot 
                                var componentList = GetBusinessKeyComponentList(stagingAreaTableName, hubTableName, businessKeyDefinition);

                                // Retrieving the attributes for the source business key
                                // Components are key parts, such as a composite key (2 or more components) or regular and concatenated keys (1 component)
                                foreach (DataRow component in componentList.Rows)
                                {
                                    var componentId = (int) component["BUSINESS_KEY_COMPONENT_ID"] - 1;

                                    // Retrieve the elements of each business key component
                                    // This only concerns concatenated keys as they are single component keys comprising of multiple elements.
                                    var elementList = GetBusinessKeyElements(stagingAreaTableName, hubTableName, businessKeyDefinition, (int) component["BUSINESS_KEY_COMPONENT_ID"]);

                                    // Composite key
                                    if (elementList.Rows.Count > 1) // Build a concatinated key if the count of elements is greater than 1 for a component (key part)
                                    {
                                        var keyType = "Normal";
                                        fieldList.Clear();

                                        foreach (DataRow element in elementList.Rows)
                                        {
                                            var elementType = element["BUSINESS_KEY_COMPONENT_ELEMENT_TYPE"].ToString();

                                            if (elementType == "Attribute")
                                            {
                                                fieldList.Append("'" + element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] +"',");
                                            }
                                            else if (elementType == "User Defined Value")
                                            {
                                                fieldList.Append("''" + element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] +"'',");
                                                keyType = "User Defined Value";
                                            }

                                            fieldOrderedList.Add(element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"].ToString());
                                        }

                                        sqlStatementForSourceQuery.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION");
                                        sqlStatementForSourceQuery.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS");
                                        sqlStatementForSourceQuery.AppendLine("WHERE TABLE_NAME= '" +querySourceTableName + "'");
                                        sqlStatementForSourceQuery.AppendLine("AND TABLE_SCHEMA = '" + TeamConfigurationSettings.SchemaName + "'");
                                        sqlStatementForSourceQuery.AppendLine("AND COLUMN_NAME IN (" +fieldList.ToString().Substring(0,fieldList.ToString().Length - 1) +")");

                                        var elementDataTypes = GetDataTable(ref connStg,sqlStatementForSourceQuery.ToString());

                                        fieldDict.Clear();

                                        foreach (DataRow attribute in elementDataTypes.Rows)
                                        {
                                            fieldDict.Add(attribute["COLUMN_NAME"].ToString(),attribute["DATA_TYPE"].ToString());
                                        }

                                        // Build the concatenated key
                                        foreach (var busKey in fieldOrderedList)
                                        {
                                            if (fieldDict.ContainsKey(busKey))
                                            {
                                                var key = "ISNULL([" + busKey + "], '')";

                                                if ((fieldDict[busKey] == "datetime2") ||(fieldDict[busKey] == "datetime"))
                                                {
                                                    key = "CASE WHEN [" + busKey + "] IS NOT NULL THEN CONVERT" +stringDataType + "(100), [" + busKey + "], 112) ELSE '' END";
                                                }
                                                else if ((fieldDict[busKey] == "numeric") ||(fieldDict[busKey] == "integer") ||(fieldDict[busKey] == "int") ||(fieldDict[busKey] == "tinyint") ||(fieldDict[busKey] == "decimal"))
                                                {
                                                    key = "CASE WHEN [" + busKey + "] IS NOT NULL THEN CAST([" +busKey + "] AS " + stringDataType + "(100)) ELSE '' END";
                                                }

                                                compositeKey.Append(key).Append(" + ");
                                            }
                                            else
                                            {
                                                var key = " " + busKey;
                                                compositeKey.Append(key).Append(" + ");
                                            }
                                        }

                                        hubDrivingKeyPair.AddLast
                                            (
                                                new[]
                                                {
                                                    hubTableName, // Hub table name
                                                    compositeKey.ToString().Substring(0, compositeKey.ToString().Length - 2),
                                                    // Source attribute name
                                                    hubKeyList.Rows[componentId]["COLUMN_NAME"].ToString() +
                                                    groupCounter, //  Target Attribute Name
                                                    keyType
                                                }
                                            );

                                        hubQuerySelect.AppendLine(compositeKey.ToString().Substring(0, compositeKey.ToString().Length - 2) +" AS [" + hubKeyList.Rows[componentId]["COLUMN_NAME"] + groupCounter + "],");
                                    }
                                    else // Handle a component of a single or composite key 
                                    {
                                        var keyType = "Normal";
                                        if (elementList != null)
                                            foreach (DataRow element in elementList.Rows)
                                            {
                                                if (element["BUSINESS_KEY_COMPONENT_ELEMENT_TYPE"].ToString() =="User Defined Value")
                                                {
                                                    firstKey =element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"].ToString();
                                                    keyType = "User Defined Value";
                                                }
                                                else
                                                {
                                                    firstKey = "[" + element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] +"]";
                                                }
                                            }

                                        hubQuerySelect.AppendLine("    " + firstKey + " AS [" +hubKeyList.Rows[componentId]["COLUMN_NAME"] +groupCounter + "],");
                                        hubDrivingKeyPair.AddLast
                                            (
                                                new[]
                                                {
                                                    hubTableName, // Hub table name
                                                    firstKey, // Source attribute name
                                                    hubKeyList.Rows[componentId]["COLUMN_NAME"].ToString() +
                                                    groupCounter, //  Target Attribute Name
                                                    keyType // Type (to detect user defined value)
                                                }
                                            );
                                    }
                                }

                            groupCounter++;
                            } // End of business key creation

                            // Initiating select statement for subquery
                            linkSatView.AppendLine("    " + hubQuerySelect);
                            linkSatView.Remove(linkSatView.Length - 4, 4);

                            linkSatView.AppendLine(sqlSourceStatement.ToString());
                        // End of Business Key calculation

                            if (degenerateLinkAttributes != null && degenerateLinkAttributes.Rows.Count > 0)
                            {
                                foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                                {
                                    linkSatView.AppendLine("    [" + (string) attribute["LINK_ATTRIBUTE_NAME"] + "],");
                                }
                            }

                            // Look for driving keys
                            var drivingKeys = "";
                            var columnOrdinal = 0;
                            var wherePredicate = "";

                            // The Driving Key Hub is queried here and added for a local variable
                            foreach (var hubArray in hubDrivingKeyPair)
                            {
                                var hubTableName = hubArray[0];
                                var hubSourceBusinessKeyName = hubArray[1];
                                var hubKeyType = hubArray[3];
                                var foundRow = drivingKeysDataTable.Rows.Find(hubTableName);

                                //if (foundRow != null && hubKeyType != "User Defined Value")
                                if (foundRow != null)
                                {
                                    drivingKeys += hubSourceBusinessKeyName + ", ";
                                }
                            }

                            if (drivingKeys.Length > 0)
                            {
                                drivingKeys = drivingKeys.Remove(drivingKeys.Length - 2, 2);  // remove trailing comma

                                // Add previous entry lookup for each alternate business key (= follower key)
                                foreach (var hubArray in hubDrivingKeyPair)
                                {
                                    var hubTableName = hubArray[0];
                                    var hubSourceBusinessKeyName = hubArray[1];
                                    var hubTargetBusinessKeyName = hubArray[2];

                                    // Check if the Hub drivingKeysDataTable in the array is a Driving Key (found in the DrivingKeys list). If so skip.
                                    var foundRow = drivingKeysDataTable.Rows.Find(hubTableName);

                                    // Get any other business keys, if no hit in foundrow (no driving key)
                                    if (foundRow == null)
                                    {
                                        // Add columns to the SELECT clause
                                        columnOrdinal += 1;
                                        if (columnOrdinal > 1)
                                            linkSatView.AppendLine(",");
                                        linkSatView.Append("    LAG(" + hubSourceBusinessKeyName + ", 1, '0')");
                                        linkSatView.Append(" OVER (PARTITION BY " + drivingKeys + " ORDER BY " + TeamConfigurationSettings.LoadDateTimeAttribute+")");
                                        linkSatView.Append(" AS PREVIOUS_FOLLOWER_KEY" + columnOrdinal);

                                        // Construct associated to the WHERE clause
                                        if (columnOrdinal > 1)
                                            wherePredicate += "\n    OR ";
                                        wherePredicate += hubTargetBusinessKeyName + " != PREVIOUS_FOLLOWER_KEY" + columnOrdinal;
                                    }
                                }
                            }

                            linkSatView.AppendLine();
                            linkSatView.AppendLine("  FROM " + TeamConfigurationSettings.SchemaName + "." + psaTableName);

                            // Filter criteria
                            if (String.IsNullOrEmpty(filterCriteria))
                            {
                                // Remove 3NF deletion issue

                                linkSatView.AppendLine("  WHERE NOT (" + TeamConfigurationSettings.RowIdAttribute + ">1 AND " + TeamConfigurationSettings.ChangeDataCaptureAttribute + " = 'Delete')");
                            }
                            else
                            {
                                linkSatView.AppendLine("  WHERE " + filterCriteria);
                                // Remove 3NF deletion issue

                                linkSatView.AppendLine("  AND NOT (" + TeamConfigurationSettings.RowIdAttribute + ">1 AND " + TeamConfigurationSettings.ChangeDataCaptureAttribute + " = 'Delete')");
                            }

                            if (checkBoxDisableLsatZeroRecords.Checked == false)
                            {
                                // Start of zero record
                                linkSatView.AppendLine("  UNION");

                                linkSatView.AppendLine("  SELECT ");
                                linkSatView.AppendLine("    [" + TeamConfigurationSettings.LoadDateTimeAttribute + "],");
                                linkSatView.AppendLine("    [" + TeamConfigurationSettings.RecordSourceAttribute + "],");
                                linkSatView.AppendLine("    [" + TeamConfigurationSettings.RowIdAttribute + "],");
                                linkSatView.AppendLine("    [" + TeamConfigurationSettings.ChangeDataCaptureAttribute + "],");

                                foreach (var hubArray in hubBusinessKeyList)
                                {
                                    var hubBusinessKeyName = hubArray[1];
                                    linkSatView.AppendLine("    [" + hubBusinessKeyName + "],");
                                }

                                foreach (DataRow attribute in multiActiveAttributes.Rows)
                                {
                                    multiActiveAttributeFromName = (string) attribute["SOURCE_ATTRIBUTE_NAME"];
                                    linkSatView.AppendLine("    " + multiActiveAttributeFromName + ",");
                                }

                                var counter = 0;
                                foreach (var hubArray in hubDrivingKeyPair)
                                {
                                    var hubTableName = hubArray[0];
                                    var foundRow = drivingKeysDataTable.Rows.Find(hubTableName);

                                    if (foundRow == null)
                                    {
                                        linkSatView.AppendLine("    '0' AS PREVIOUS_FOLLOWER_KEY" + counter + ",");
                                        counter++;
                                    }
                                }

                                linkSatView.Remove(linkSatView.Length - 3, 3);
                                linkSatView.AppendLine();
                                linkSatView.AppendLine("  FROM (");
                                linkSatView.AppendLine("    SELECT");
                                linkSatView.AppendLine("    '1900-01-01' AS [" + TeamConfigurationSettings.LoadDateTimeAttribute + "],");
                                linkSatView.AppendLine("    'Data Warehouse' AS [" + TeamConfigurationSettings.RecordSourceAttribute + "],");
                                linkSatView.AppendLine("    0 AS [" + TeamConfigurationSettings.RowIdAttribute + "],");
                                linkSatView.AppendLine("    'N/A' AS [" + TeamConfigurationSettings.ChangeDataCaptureAttribute + "],");

                                linkSatView.AppendLine("    " + hubQuerySelect);
                                linkSatView.Remove(linkSatView.Length - 3, 3);

                                foreach (DataRow attribute in multiActiveAttributes.Rows)
                                {
                                    multiActiveAttributeFromName = (string) attribute["SOURCE_ATTRIBUTE_NAME"];
                                    linkSatView.AppendLine("   " + multiActiveAttributeFromName + ",");
                                }

                                linkSatView.AppendLine("    DENSE_RANK() OVER (PARTITION  BY ");
                                // Dense rank needs to be over the Driving Key
                                foreach (var hubArray in hubDrivingKeyPair)
                                {
                                    var hubTableName = hubArray[0];
                                    var hubSourceBusinessKeyName = hubArray[1];
                                    var foundRow = drivingKeysDataTable.Rows.Find(hubTableName);

                                    if (foundRow != null)
                                    {
                                        linkSatView.AppendLine("        " + hubSourceBusinessKeyName + ",");
                                    }
                                }

                                foreach (DataRow attribute in multiActiveAttributes.Rows)
                                {
                                    multiActiveAttributeFromName = (string) attribute["SOURCE_ATTRIBUTE_NAME"];
                                    linkSatView.AppendLine("        [" + multiActiveAttributeFromName + "],");
                                }

                                linkSatView.Remove(linkSatView.Length - 3, 3);
                                linkSatView.AppendLine();
                                linkSatView.Append("    ORDER BY [" + TeamConfigurationSettings.LoadDateTimeAttribute + "], ");

                                foreach (var hubArray in hubDrivingKeyPair)
                                {
                                    var hubTableName = hubArray[0];
                                    var hubSourceBusinessKeyName = hubArray[1];
                                    var foundRow = drivingKeysDataTable.Rows.Find(hubTableName);

                                    if (foundRow != null)
                                    {
                                        linkSatView.Append(hubSourceBusinessKeyName).Append(",");
                                    }
                                }

                                foreach (DataRow attribute in multiActiveAttributes.Rows)
                                {
                                    multiActiveAttributeFromName = (string) attribute["SOURCE_ATTRIBUTE_NAME"];
                                    linkSatView.Append("         " + multiActiveAttributeFromName + ",");
                                }
                                linkSatView.Remove(linkSatView.Length - 1, 1);
                                linkSatView.AppendLine(" ASC) ROWVERSION");
                                linkSatView.AppendLine("  FROM " + TeamConfigurationSettings.SchemaName + "." + psaTableName);
                                linkSatView.Append("  ) dummysub");
                                linkSatView.AppendLine();
                                linkSatView.AppendLine("  WHERE ROWVERSION=1");
                            // End of zero record
                            }

                            linkSatView.AppendLine(") sub");
                            linkSatView.AppendLine("WHERE " + wherePredicate);

                            // linkSatView.AppendLine();
                            linkSatView.AppendLine("-- ORDER BY");

                            foreach (var hubArray in hubBusinessKeyList)
                            {
                                var hubTableName = hubArray[0];
                                var hubBusinessKeyName = hubArray[1];
                                var foundRow = drivingKeysDataTable.Rows.Find(hubTableName);

                                if (foundRow != null)
                                {
                                    linkSatView.AppendLine("--  " + hubBusinessKeyName + ",");
                                }
                            }

                            linkSatView.AppendLine("--  [" + TeamConfigurationSettings.LoadDateTimeAttribute + "]");
                            // End of source subuery

                            SetTextLsat("Processing driving key Link Satellite entity view for " + targetTableName + "\r\n");


                            using (var outfile = new StreamWriter(textBoxOutputPath.Text + @"\VIEW_" + targetTableName + ".sql", false))
                            {
                                outfile.Write(linkSatView.ToString());
                                outfile.Close();
                            }

                            if (checkBoxGenerateInDatabase.Checked)
                            {
                                connHstg.ConnectionString = TeamConfigurationSettings.ConnectionStringHstg;
                                int insertError = GenerateInDatabase(connHstg, linkSatView.ToString());
                                errorCounter = errorCounter + insertError;
                            }

                            SetTextDebug(linkSatView.ToString());
                            SetTextDebug("\n");
                           

                        }
                    }
                 
            }
            else
            {
                SetTextLsat("There was no metadata selected to create Driving Key Link Satellite views. Please check the metadata schema - are there any Link Satellites selected?");
            }

            SetTextLsat($"\r\n{errorCounter} errors have been found.\r\n");
            SetTextLsat($"SQL Scripts have been successfully saved in {VedwConfigurationSettings.VedwOutputPath}.\r\n");
        }

        private DataTable GetMultiActiveAttributes(int targetTableId)
        {
            // Query to detect multi-active attributes
            

            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            var multiActiveAttributeQuery = new StringBuilder();

            multiActiveAttributeQuery.AppendLine("SELECT ");
            multiActiveAttributeQuery.AppendLine("c.ATTRIBUTE_NAME AS SOURCE_ATTRIBUTE_NAME,");
            multiActiveAttributeQuery.AppendLine("b.ATTRIBUTE_NAME AS SATELLITE_ATTRIBUTE_NAME");
            multiActiveAttributeQuery.AppendLine("FROM MD_SOURCE_SATELLITE_ATTRIBUTE_XREF a");
            multiActiveAttributeQuery.AppendLine("JOIN MD_ATTRIBUTE b ON a.ATTRIBUTE_ID_TO=b.ATTRIBUTE_ID");
            multiActiveAttributeQuery.AppendLine("	JOIN MD_ATTRIBUTE c ON a.ATTRIBUTE_ID_FROM=c.ATTRIBUTE_ID");
            multiActiveAttributeQuery.AppendLine("WHERE");
            multiActiveAttributeQuery.AppendLine("     MULTI_ACTIVE_KEY_INDICATOR='Y'");
            multiActiveAttributeQuery.AppendLine(" AND SATELLITE_ID=" + targetTableId);

            var multiActiveAttributes = GetDataTable(ref connOmd, multiActiveAttributeQuery.ToString());
            multiActiveAttributes.PrimaryKey = new[] { multiActiveAttributes.Columns["SOURCE_ATTRIBUTE_NAME"] };

            return multiActiveAttributes;
        }

        // Link Satellite generation - historical
        private void GenerateLsatHistoryViews()
        {
            int errorCounter = 0;

            if (checkedListBoxLsatMetadata.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= checkedListBoxLsatMetadata.CheckedItems.Count - 1; x++)
                {
                    var stringDataType = checkBoxUnicode.Checked ? "NVARCHAR" : "VARCHAR";

                    var connOmd = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringOmd};
                    var connStg = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringStg};
                    var connHstg = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringHstg};

                    var targetTableName = checkedListBoxLsatMetadata.CheckedItems[x].ToString();

                    var sqlStatementForTablesToImport = new StringBuilder();
                    sqlStatementForTablesToImport.AppendLine("SELECT * FROM interface.INTERFACE_SOURCE_SATELLITE_XREF base");
                    sqlStatementForTablesToImport.AppendLine("WHERE SATELLITE_TYPE = 'Link Satellite' ");
                    sqlStatementForTablesToImport.AppendLine(" AND SATELLITE_NAME = '" + targetTableName + "'");
                    sqlStatementForTablesToImport.AppendLine(" AND NOT EXISTS (SELECT * FROM [interface].[INTERFACE_DRIVING_KEY] sub WHERE sub.SATELLITE_ID = base.SATELLITE_ID)");

                    var tables = GetDataTable(ref connOmd, sqlStatementForTablesToImport.ToString());

                        foreach (DataRow row in tables.Rows)
                        {
                            var linkSatView = new StringBuilder();

                            var stagingAreaTableId = (int) row["SOURCE_ID"];
                            var stagingAreaTableName = (string) row["SOURCE_NAME"];
                            var filterCriteria = (string) row["FILTER_CRITERIA"];
                            var targetTableId = (int) row["SATELLITE_ID"];
                            var linkTableName = (string) row["LINK_NAME"];

                            var linkSk = linkTableName.Substring(4) + "_" + TeamConfigurationSettings.DwhKeyIdentifier;
                            var currentTableName = (string) row["SOURCE_NAME"];
                            currentTableName = TeamConfigurationSettings.PsaTablePrefixValue +currentTableName.Replace(TeamConfigurationSettings.StgTablePrefixValue, "");


                            // Query to detect multi-active attributes
                            var multiActiveAttributes = GetMultiActiveAttributes(targetTableId);

                            // Retrieve the Source-To-Target mapping for Satellites
                            var sourceStructure = GetStagingToSatelliteAttributeMapping(targetTableId, stagingAreaTableId);

                            // Get the associated Hub tables and its target business key attributes for the Link
                            var hubBusinessKeyList = GetHubTablesForLink(linkTableName);

                            // Understand the degenerate fields, so these can be added later
                            var degenerateLinkAttributes = GetDegenerateLinkAttributes(linkTableName);

                            // **************************************************************************************		
                            // Creating the source query
                            // **************************************************************************************

                            // Create View
                            linkSatView.AppendLine("--");
                            linkSatView.AppendLine("-- Link Satellite View definition - regular history for " +targetTableName);
                            linkSatView.AppendLine("-- Generated at " + DateTime.Now);
                            linkSatView.AppendLine("--");
                            linkSatView.AppendLine();
                            linkSatView.AppendLine("USE [" + TeamConfigurationSettings.PsaDatabaseName + "]");
                            linkSatView.AppendLine("GO");
                            linkSatView.AppendLine();
                            linkSatView.AppendLine("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[" + VedwConfigurationSettings.VedwSchema + "].[" +targetTableName + "]') AND type in (N'V'))");
                            linkSatView.AppendLine("DROP VIEW [" + VedwConfigurationSettings.VedwSchema + "].[" + targetTableName +"]");
                            linkSatView.AppendLine("go");
                            linkSatView.AppendLine("CREATE VIEW [" + VedwConfigurationSettings.VedwSchema + "].[" + targetTableName +"] AS  ");
                            linkSatView.AppendLine("SELECT");


                            if (!checkBoxDisableHash.Checked)
                            {
                                // Link SK - combined hash key
                                linkSatView.AppendLine("   "+VedwConfigurationSettings.hashingStartSnippet);

                                foreach (var hubArray in hubBusinessKeyList)
                                {
                                    linkSatView.AppendLine("    ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + hubArray[1] + ")),'NA')+'|'+");
                                }

                                // Add the degenerate attributes to be part of the key
                                if (degenerateLinkAttributes != null && degenerateLinkAttributes.Rows.Count > 0)
                                {
                                    foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                                    {
                                        linkSatView.AppendLine(
                                            "    ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," +
                                            (string) attribute["LINK_ATTRIBUTE_NAME"] + ")),'NA')+'|'+");
                                    }
                                }

                                linkSatView.Remove(linkSatView.Length - 3, 3);
                                linkSatView.AppendLine();
                                linkSatView.AppendLine("  "+VedwConfigurationSettings.hashingEndSnippet+" AS " + linkSk + ",");
                            }
                            else
                            {
                                //BK as DWH key
                                foreach (var hubArray in hubBusinessKeyList)
                                {
                                    linkSatView.Append("  ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + hubArray[1] + ")),'NA')+'|'+");
                                }

                                if (degenerateLinkAttributes != null && degenerateLinkAttributes.Rows.Count > 0)
                                {
                                    foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                                    {
                                        linkSatView.Append("    ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," +
                                                           (string) attribute["LINK_ATTRIBUTE_NAME"] + ")),'NA')+'|'+");
                                    }
                                }

                                linkSatView.Remove(linkSatView.Length - 5, 5);
                                linkSatView.Append("  AS " + linkSk + ",");
                                linkSatView.AppendLine();
                            }

                            // Effective Datetime
                            if (TeamConfigurationSettings.EnableAlternativeSatelliteLoadDateTimeAttribute == "True")
                            {
                                linkSatView.AppendLine("   " + TeamConfigurationSettings.LoadDateTimeAttribute + " AS " +TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + ",");
                            }
                            else
                            {
                                linkSatView.AppendLine("   " + TeamConfigurationSettings.LoadDateTimeAttribute + " AS " + TeamConfigurationSettings.LoadDateTimeAttribute + ",");
                            }

                            //Multi-Active attributes
                            foreach (DataRow attribute in multiActiveAttributes.Rows)
                            {
                                var multiActiveAttributeFromName = (string) attribute["SATELLITE_ATTRIBUTE_NAME"];
                                linkSatView.AppendLine("          [" + multiActiveAttributeFromName + "],");
                            }

                            // Expiry Datetime
                            linkSatView.AppendLine("   COALESCE ( LEAD ( [" + TeamConfigurationSettings.LoadDateTimeAttribute + "] ) OVER");
                            linkSatView.AppendLine("   		     (PARTITION BY ");

                            // Add the Business Kyes
                            foreach (var hubArray in hubBusinessKeyList)
                            {
                                var hubBusinessKeyName = hubArray[1];
                                linkSatView.AppendLine("              " + hubBusinessKeyName + ",");
                            }

                        // Add the degenerate attributes
                            if (degenerateLinkAttributes != null && degenerateLinkAttributes.Rows.Count > 0)
                            {
                                foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                                {
                                    linkSatView.AppendLine("              " + (string) attribute["LINK_ATTRIBUTE_NAME"] +
                                                           ",");
                                }
                            }

                            // Add the multi-active attributes
                            foreach (DataRow attribute in multiActiveAttributes.Rows)
                            {
                                linkSatView.AppendLine("              " + (string)attribute["SATELLITE_ATTRIBUTE_NAME"] + ",");
                            }

                            linkSatView.Remove(linkSatView.Length - 3, 3);
                            linkSatView.AppendLine();
                            linkSatView.AppendLine("   		     ORDER BY [" + TeamConfigurationSettings.LoadDateTimeAttribute + "]),");
                            linkSatView.AppendLine("   CAST( '9999-12-31' AS DATETIME)) AS " + TeamConfigurationSettings.ExpiryDateTimeAttribute + ",");

                            // Current record indicator
                            linkSatView.AppendLine("   CASE");
                            linkSatView.AppendLine("      WHEN ( RANK() OVER (PARTITION BY ");

                            // Business Key
                            foreach (var hubArray in hubBusinessKeyList)
                            {
                                var hubBusinessKeyName = hubArray[1];
                                linkSatView.AppendLine("            " + hubBusinessKeyName + ",");
                            }

                        // Add the degenerate attributes
                            if (degenerateLinkAttributes != null && degenerateLinkAttributes.Rows.Count > 0)
                            {
                                foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                                {
                                    linkSatView.AppendLine("            " + (string) attribute["LINK_ATTRIBUTE_NAME"] +
                                                           ",");
                                }
                            }

                            // Multi-Active
                            foreach (DataRow attribute in multiActiveAttributes.Rows)
                            {
                                linkSatView.AppendLine("         " + (string)attribute["SATELLITE_ATTRIBUTE_NAME"] + ",");
                            }

                            linkSatView.Remove(linkSatView.Length - 3, 3);
                            linkSatView.AppendLine();

                            linkSatView.AppendLine("          ORDER BY [" + TeamConfigurationSettings.LoadDateTimeAttribute + "] desc )) = 1");
                            linkSatView.AppendLine("      THEN 'Y'");
                            linkSatView.AppendLine("      ELSE 'N'");
                            linkSatView.AppendLine("   END AS " + TeamConfigurationSettings.CurrentRowAttribute + ",");

                            if (TeamConfigurationSettings.EnableAlternativeRecordSourceAttribute == "True")
                            {
                                linkSatView.AppendLine("   [" + TeamConfigurationSettings.RecordSourceAttribute + "] AS [" + TeamConfigurationSettings.AlternativeRecordSourceAttribute + "],");
                            }
                            else
                            {
                                linkSatView.AppendLine("   [" + TeamConfigurationSettings.RecordSourceAttribute + "],");
                            }

                            //Logical deletes
                            if (checkBoxEvaluateSatDelete.Checked)
                            {
                                linkSatView.AppendLine("    CASE");
                                linkSatView.AppendLine("      WHEN [" + TeamConfigurationSettings.ChangeDataCaptureAttribute + "] = 'Delete' THEN 'Y'");
                                linkSatView.AppendLine("      ELSE 'N'");
                                linkSatView.AppendLine("    END AS [" + TeamConfigurationSettings.LogicalDeleteAttribute + "],");
                            }


                            linkSatView.AppendLine("   -1 AS " + TeamConfigurationSettings.EtlProcessAttribute + ",");
                            linkSatView.AppendLine("   -1 AS " + TeamConfigurationSettings.EtlProcessUpdateAttribute + ",");
                            linkSatView.AppendLine("   [" + TeamConfigurationSettings.RowIdAttribute + "],");
                            linkSatView.AppendLine("   [" + TeamConfigurationSettings.ChangeDataCaptureAttribute + "],");

                            // All the attibutes (except the multi-active ones)
                            foreach (DataRow attribute in sourceStructure.Rows)
                            {
                                if (attribute["MULTI_ACTIVE_KEY_INDICATOR"].ToString() == "N")
                                {
                                    linkSatView.Append("   " + attribute["SATELLITE_ATTRIBUTE_NAME"] + ",");
                                    linkSatView.AppendLine();
                                }
                            }

                            // Row number
                            linkSatView.AppendLine("   CAST(");
                            linkSatView.AppendLine("      ROW_NUMBER() OVER (PARTITION  BY ");

                            // Business Key
                            foreach (var hubArray in hubBusinessKeyList)
                            {
                                var hubBusinessKeyName = hubArray[1];
                                linkSatView.AppendLine("         " + hubBusinessKeyName + ",");
                            }

                        // Add the degenerate attributes
                            if (degenerateLinkAttributes != null && degenerateLinkAttributes.Rows.Count > 0)
                            {
                                foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                                {
                                    linkSatView.AppendLine("         " + (string) attribute["LINK_ATTRIBUTE_NAME"] + ",");
                                }
                            }

                            // Multi-active
                            foreach (DataRow attribute in multiActiveAttributes.Rows)
                            {
                                linkSatView.AppendLine("         " + (string)attribute["SATELLITE_ATTRIBUTE_NAME"] + ",");
                            }

                            linkSatView.Remove(linkSatView.Length - 3, 3);
                            linkSatView.AppendLine();
                            linkSatView.AppendLine("      ORDER BY ");

                            // Business Key
                            foreach (var hubArray in hubBusinessKeyList)
                            {
                                var hubBusinessKeyName = hubArray[1];
                                linkSatView.AppendLine("         " + hubBusinessKeyName + ",");
                            }

                        // Add the degenerate attributes
                            if (degenerateLinkAttributes != null && degenerateLinkAttributes.Rows.Count > 0)
                            {
                                foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                                {
                                    linkSatView.AppendLine("         " + (string) attribute["LINK_ATTRIBUTE_NAME"] + ",");
                                }
                            }

                            // Multi-active
                            foreach (DataRow attribute in multiActiveAttributes.Rows)
                            {
                                linkSatView.AppendLine("         " + (string)attribute["SATELLITE_ATTRIBUTE_NAME"] + ",");
                            }

                            linkSatView.AppendLine("         [" + TeamConfigurationSettings.LoadDateTimeAttribute + "]) AS INT)");
                            linkSatView.AppendLine("   AS ROW_NUMBER,");

                            // Checksum
                            linkSatView.AppendLine("   "+VedwConfigurationSettings.hashingStartSnippet);
                            linkSatView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100),[" +TeamConfigurationSettings.ChangeDataCaptureAttribute + "])),'NA')+'|'+");

                            foreach (var hubArray in hubBusinessKeyList)
                            {
                                var hubBusinessKeyName = hubArray[1];
                                linkSatView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + hubBusinessKeyName +")),'NA')+'|'+");
                            }

                            foreach (DataRow attribute in sourceStructure.Rows)
                            {
                                linkSatView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," +attribute["SATELLITE_ATTRIBUTE_NAME"] + ")),'NA')+'|'+");
                            }
                            // End of checksum

                            linkSatView.Remove(linkSatView.Length - 3, 3);
                            linkSatView.AppendLine();
                            linkSatView.AppendLine("   "+VedwConfigurationSettings.hashingEndSnippet+" AS " + TeamConfigurationSettings.RecordChecksumAttribute + "");

                            // From statement
                            linkSatView.AppendLine("FROM ");
                            linkSatView.AppendLine("(");
                            linkSatView.AppendLine("  SELECT DISTINCT");
                            linkSatView.AppendLine("    [" + TeamConfigurationSettings.LoadDateTimeAttribute + "],");
                            linkSatView.AppendLine("    [" + TeamConfigurationSettings.RecordSourceAttribute + "],");
                            linkSatView.AppendLine("    [" + TeamConfigurationSettings.RowIdAttribute + "],");
                            linkSatView.AppendLine("    [" + TeamConfigurationSettings.ChangeDataCaptureAttribute + "],");

                            var sqlStatementForComponent = new StringBuilder();
                            var sqlStatementForHubBusinessKeys = new StringBuilder();

                            // Get the Hubs for each Link/STG combination - both need to be represented in the query
                            var hubTables = GetHubLinkCombination(stagingAreaTableName, linkTableName);

                            var hubQuerySelect = new StringBuilder();
                            var hubQueryWhere = new StringBuilder();
                            var hubQueryGroupBy = new StringBuilder();

                            // Creating the business keys
                            // Assign groups (counter) to allow for SAL
                            int groupCounter = 1;
                            foreach (DataRow hubDetailRow in hubTables.Rows)
                            {
                                sqlStatementForComponent.Clear();
                                sqlStatementForHubBusinessKeys.Clear();

                                var hubTableName = (string) hubDetailRow["HUB_NAME"];
                                var businessKeyDefinition = (string) hubDetailRow["BUSINESS_KEY_DEFINITION"];

                                // Construct the join clauses, where clauses etc. for the Hubs
                                var queryClauses = GetHubClauses(stagingAreaTableName, hubTableName, businessKeyDefinition, groupCounter.ToString());

                                hubQuerySelect.AppendLine(queryClauses[0]);
                                hubQueryWhere.AppendLine(queryClauses[1]);
                                hubQueryGroupBy.AppendLine(queryClauses[2]);

                                hubQuerySelect.Remove(hubQuerySelect.Length - 3, 3);
                                hubQueryWhere.Remove(hubQueryWhere.Length - 3, 3);
                                hubQueryGroupBy.Remove(hubQueryGroupBy.Length - 3, 3);

                            groupCounter++;

                            } // End of business key creation

                            // Initiating select statement for subquery
                            linkSatView.AppendLine("" + hubQuerySelect);
                            linkSatView.Remove(linkSatView.Length - 4, 4);
                            linkSatView.AppendLine();

                        // Add the degenerate attributes
                            if (degenerateLinkAttributes != null && degenerateLinkAttributes.Rows.Count > 0)
                            {
                                foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                                {
                                    linkSatView.AppendLine("    ,[" + (string) attribute["SOURCE_ATTRIBUTE_NAME"] +
                                                           "] AS [" + attribute["LINK_ATTRIBUTE_NAME"] + "],");
                                }
                            }

                            // Add the multi-active attributes
                            foreach (DataRow attribute in multiActiveAttributes.Rows)
                            {
                                linkSatView.AppendLine("    ,[" + (string)attribute["SOURCE_ATTRIBUTE_NAME"] + "] AS [" + attribute["SATELLITE_ATTRIBUTE_NAME"] + "],");
                            }

                            // Add all the attributes
                            foreach (DataRow attribute in sourceStructure.Rows)
                            {
                                if (attribute["MULTI_ACTIVE_KEY_INDICATOR"].ToString() == "N")
                                {
                                    linkSatView.AppendLine("    " + attribute["SOURCE_ATTRIBUTE_NAME"] + " AS " + attribute["SATELLITE_ATTRIBUTE_NAME"] + ",");
                                }
                            }

                            linkSatView.Remove(linkSatView.Length - 3, 3);
                            linkSatView.AppendLine();
                            linkSatView.AppendLine("  FROM " + TeamConfigurationSettings.SchemaName + "." + currentTableName);

                            // Filter criteria
                            if (string.IsNullOrEmpty(filterCriteria))
                            {
                            }
                            else
                            {
                                linkSatView.AppendLine("  WHERE " + filterCriteria);
                            }

                            if (checkBoxDisableLsatZeroRecords.Checked == false)
                            {
                                // Start of zero record
                                linkSatView.AppendLine("  UNION");

                                linkSatView.AppendLine("  SELECT DISTINCT");
                                linkSatView.AppendLine("    '1900-01-01' AS [" + TeamConfigurationSettings.LoadDateTimeAttribute + "],");
                                linkSatView.AppendLine("    'Data Warehouse' AS [" + TeamConfigurationSettings.RecordSourceAttribute + "],");
                                linkSatView.AppendLine("     0 AS [" + TeamConfigurationSettings.RowIdAttribute + "],");
                                linkSatView.AppendLine("    'N/A' AS [" + TeamConfigurationSettings.ChangeDataCaptureAttribute + "],");

                                linkSatView.AppendLine("" + hubQuerySelect);
                                linkSatView.Remove(linkSatView.Length - 2, 2);

                            // Add the degenerate attributes
                                if (degenerateLinkAttributes != null && degenerateLinkAttributes.Rows.Count > 0)
                                {
                                    foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                                    {
                                        linkSatView.AppendLine(
                                            "    [" + (string) attribute["SOURCE_ATTRIBUTE_NAME"] + "] AS [" +
                                            attribute["LINK_ATTRIBUTE_NAME"] + "],");
                                    }
                                }

                                // Add the multi-active attribute
                                foreach (DataRow attribute in multiActiveAttributes.Rows)
                                {
                                    linkSatView.AppendLine("    " + (string)attribute["SOURCE_ATTRIBUTE_NAME"] + " AS [" + attribute["SATELLITE_ATTRIBUTE_NAME"] + "],");
                                }

                                // Add the rest of the attributes as NULL values (nothing was known at this stage)
                                foreach (DataRow attribute in sourceStructure.Rows)
                                {
                                    if (attribute["MULTI_ACTIVE_KEY_INDICATOR"].ToString() == "N")
                                    {
                                        linkSatView.AppendLine("    NULL AS [" + attribute["SATELLITE_ATTRIBUTE_NAME"] +"],");
                                    }
                                }

                                linkSatView.Remove(linkSatView.Length - 3, 3);
                                linkSatView.AppendLine();
                                linkSatView.AppendLine("  FROM " + TeamConfigurationSettings.SchemaName + "." + currentTableName);
                            // End of zero record
                            }

                            linkSatView.AppendLine(") sub");

                            // End of source subuery


                            SetTextLsat("Processing regular history Link Satellite entity view for " + targetTableName + "\r\n");

                            using (var outfile = new StreamWriter(textBoxOutputPath.Text + @"\VIEW_" + targetTableName + ".sql", false))
                            {
                                outfile.Write(linkSatView.ToString());
                                outfile.Close();
                            }

                            if (checkBoxGenerateInDatabase.Checked)
                            {
                                int insertError = GenerateInDatabase(connHstg, linkSatView.ToString());
                                errorCounter = errorCounter + insertError;
                            }


                            SetTextDebug(linkSatView.ToString());
                            SetTextDebug("\n");

                            

                        
                        }
                }
            }
            else
            {
                SetTextLsat("There was no metadata selected to create regular Link Satellite views. Please check the metadata schema - are there any Link Satellites selected?");
            }

            SetTextLsat($"\r\n{errorCounter} errors have been found.\r\n");
            SetTextLsat($"SQL Scripts have been successfully saved in {VedwConfigurationSettings.VedwOutputPath}.\r\n");
        }

        private DataTable GetStagingToSatelliteAttributeMapping(int targetTableId, int stagingAreaTableId)
        {
            var sqlStatementForAttributes = new StringBuilder();
            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };

            // Query the metadata to retrieve the STG and INT attributes and their relationship
            sqlStatementForAttributes.AppendLine("SELECT ");
            sqlStatementForAttributes.AppendLine(" [SOURCE_ID]");
            sqlStatementForAttributes.AppendLine(",[SOURCE_NAME]");
            sqlStatementForAttributes.AppendLine(",[SOURCE_SCHEMA_NAME]");
            sqlStatementForAttributes.AppendLine(",[SATELLITE_ID]");
            sqlStatementForAttributes.AppendLine(",[SATELLITE_NAME]");
            sqlStatementForAttributes.AppendLine(",[SOURCE_ATTRIBUTE_ID]");
            sqlStatementForAttributes.AppendLine(",[SOURCE_ATTRIBUTE_NAME]");
            sqlStatementForAttributes.AppendLine(",[SATELLITE_ATTRIBUTE_ID]");
            sqlStatementForAttributes.AppendLine(",[SATELLITE_ATTRIBUTE_NAME]");
            sqlStatementForAttributes.AppendLine(",[MULTI_ACTIVE_KEY_INDICATOR]");
            sqlStatementForAttributes.AppendLine("FROM [interface].[INTERFACE_SOURCE_SATELLITE_ATTRIBUTE_XREF]");
            sqlStatementForAttributes.AppendLine("WHERE [SATELLITE_ID] = " + targetTableId);
            sqlStatementForAttributes.AppendLine("  AND [SOURCE_ID] = " + stagingAreaTableId);
            sqlStatementForAttributes.AppendLine("  AND [SATELLITE_ATTRIBUTE_NAME] NOT IN ('" +
                                                 TeamConfigurationSettings.RecordSourceAttribute + "','" +
                                                 TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" +
                                                 TeamConfigurationSettings.RowIdAttribute + "','" +
                                                 TeamConfigurationSettings.RecordChecksumAttribute + "','" +
                                                 TeamConfigurationSettings.ChangeDataCaptureAttribute + "','" +
                                                 TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" +
                                                 TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "','" +
                                                 TeamConfigurationSettings.EtlProcessAttribute + "','" +
                                                 TeamConfigurationSettings.LoadDateTimeAttribute + "')");

            var sourceStructure = GetDataTable(ref connOmd, sqlStatementForAttributes.ToString());

            return sourceStructure;
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

        private void rawDataMartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("This feature is yet to be implemented.", "Upcoming!",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //var t = new Thread(ThreadProcDimensional);
            //t.Start();
            MessageBox.Show("This feature is yet to be implemented.", "Upcoming!", MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
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
            richTextBoxInformation.Clear();

            var newThread = new Thread(BackgroundDoPsa);
            newThread.Start();
        }

        private void BackgroundDoPsa(Object obj)
        {
            // Check if the schema needs to be created
            CreateSchema(TeamConfigurationSettings.ConnectionStringHstg);

            if (radiobuttonViews.Checked)
            {
                PsaGenerateViews();
            }
            else if (radiobuttonStoredProc.Checked)
            {
                MessageBox.Show("This feature is yet to be implemented.", "Upcoming!", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
            else if (radioButtonIntoStatement.Checked)
            {
                PsaGenerateInsertInto();
            }
        }

        // Create the Insert statement for the Persisten Staging Area (PSA)
        private void PsaGenerateInsertInto()
        {
            int errorCounter = 0;

            if (checkBoxIfExistsStatement.Checked)
            {
                SetTextPsa("The Drop If Exists checkbox has been checked, but this feature is not relevant for this specific operation and will be ignored. \n\r");
            }

            if (checkedListBoxPsaMetadata.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= checkedListBoxPsaMetadata.CheckedItems.Count - 1; x++)
                {
                    var connStg = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringStg};
                    var connHstg = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringHstg};

                    var targetTableName = checkedListBoxPsaMetadata.CheckedItems[x].ToString();
                
                    // Build the main attribute list of the PSA table for selection
                    var sourceTableStructure = GetTableStructure(targetTableName, ref connHstg, "PSA");

                    // Initial SQL
                    var psaInsertIntoStatement = new StringBuilder();

                    psaInsertIntoStatement.AppendLine("--");
                    psaInsertIntoStatement.AppendLine("-- PSA Insert Into statement for " + targetTableName);
                    psaInsertIntoStatement.AppendLine("-- Generated at " + DateTime.Now);
                    psaInsertIntoStatement.AppendLine("--");
                    psaInsertIntoStatement.AppendLine();
                    psaInsertIntoStatement.AppendLine("USE [" + TeamConfigurationSettings.StagingDatabaseName + "]");
                    psaInsertIntoStatement.AppendLine("GO");
                    psaInsertIntoStatement.AppendLine();
                    psaInsertIntoStatement.AppendLine("INSERT INTO ["+TeamConfigurationSettings.PsaDatabaseName+"].["+TeamConfigurationSettings.SchemaName+"].[" + targetTableName+"]");
                    psaInsertIntoStatement.AppendLine("   (");

                    foreach (DataRow attribute in sourceTableStructure.Rows)
                    {
                        psaInsertIntoStatement.AppendLine("   [" + attribute["COLUMN_NAME"] + "],");
                    }

                    psaInsertIntoStatement.Remove(psaInsertIntoStatement.Length - 3, 3);
                    psaInsertIntoStatement.AppendLine();
                    psaInsertIntoStatement.AppendLine("   )");
                    psaInsertIntoStatement.AppendLine("SELECT ");

                    foreach (DataRow attribute in sourceTableStructure.Rows)
                    {
                        var sourceAttribute = attribute["COLUMN_NAME"];

                        if ((string)sourceAttribute == TeamConfigurationSettings.EtlProcessAttribute)
                        {
                            psaInsertIntoStatement.AppendLine("   -1 AS " + attribute["COLUMN_NAME"] + ",");
                        }
                        else
                        {
                            psaInsertIntoStatement.AppendLine("   [" + attribute["COLUMN_NAME"] + "],");
                        }
                    }

                    psaInsertIntoStatement.Remove(psaInsertIntoStatement.Length - 3, 3);
                    psaInsertIntoStatement.AppendLine();
                    psaInsertIntoStatement.AppendLine("FROM ["+VedwConfigurationSettings.VedwSchema+"]." +targetTableName);

                    using (var outfile = new StreamWriter(textBoxOutputPath.Text + @"\INSERT_STATEMENT_" + targetTableName + ".sql"))
                    {
                        outfile.Write(psaInsertIntoStatement.ToString());
                        outfile.Close();
                    }

                    if (checkBoxGenerateInDatabase.Checked)
                    {
                        connHstg.ConnectionString = TeamConfigurationSettings.ConnectionStringHstg;

                        int insertError = GenerateInDatabase(connHstg, psaInsertIntoStatement.ToString());
                        errorCounter = errorCounter + insertError;
                    }

                    SetTextDebug(psaInsertIntoStatement.ToString());
                    SetTextDebug("\n");

                    connStg.Close();
                    connHstg.Close();

                    SetTextPsa($"Processing PSA Insert Into statement for {targetTableName}\r\n");
                }
            }
            else
            {
                SetTextPsa("There was no metadata selected to create Persistent Staging Area insert statements. Please check the metadata schema - are there any Staging Area tables selected?");
            }

            SetTextPsa($"\r\n{errorCounter} errors have been found.\r\n");
            SetTextPsa($"SQL Scripts have been successfully saved in {VedwConfigurationSettings.VedwOutputPath}.\r\n");
        }

        private void PsaGenerateViews()
        {
            // Setup error handling to report back to the user
            int errorCounter = 0;

            if (checkedListBoxPsaMetadata.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= checkedListBoxPsaMetadata.CheckedItems.Count - 1; x++)
                {
                    var connStg = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringStg};
                    var connHstg = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringHstg};

                    var recordSourceAttribute = TeamConfigurationSettings.RecordSourceAttribute;
                    var loadDateTimeAttribute = TeamConfigurationSettings.LoadDateTimeAttribute;
                    var etlProcessIdAttribute = TeamConfigurationSettings.EtlProcessAttribute;
                    var cdcOperationAttribute = TeamConfigurationSettings.ChangeDataCaptureAttribute;

                    var targetTableName = checkedListBoxPsaMetadata.CheckedItems[x].ToString();
                    var sourceTableName = targetTableName.Replace(TeamConfigurationSettings.PsaTablePrefixValue,TeamConfigurationSettings.StgTablePrefixValue);

                    SetTextPsa($"Processing PSA entity view for {targetTableName}\r\n");

                    string targetTableIndexName;
                    if (TeamConfigurationSettings.PsaKeyLocation == "PrimaryKey")//radioButtonPSABusinessKeyIndex.Checked
                    {
                        targetTableIndexName = "PK_" + targetTableName;
                    }
                    else
                    {
                        targetTableIndexName = "IX_" + targetTableName;
                    }

                    var mainBusinessKey = "";

                    var stringDataType = checkBoxUnicode.Checked ? "NVARCHAR" : "VARCHAR";

                    // Query the natural key(s)
                    var sqlStatementForPartitioningQuery = new StringBuilder();

                    sqlStatementForPartitioningQuery.AppendLine("SELECT C.name AS ATTRIBUTE_NAME");
                    sqlStatementForPartitioningQuery.AppendLine("FROM sys.index_columns A");
                    sqlStatementForPartitioningQuery.AppendLine("JOIN sys.indexes B");
                    sqlStatementForPartitioningQuery.AppendLine("ON A.object_id=B.object_id");
                    sqlStatementForPartitioningQuery.AppendLine("  AND A.index_id=B.index_id");
                    sqlStatementForPartitioningQuery.AppendLine("JOIN sys.columns C");
                    sqlStatementForPartitioningQuery.AppendLine("ON A.column_id=C.column_id");
                    sqlStatementForPartitioningQuery.AppendLine("  AND A.object_id=C.object_id");
                    sqlStatementForPartitioningQuery.AppendLine("WHERE B.name='" + targetTableIndexName + "'");
                    sqlStatementForPartitioningQuery.AppendLine("  AND C.name<>'"+TeamConfigurationSettings.LoadDateTimeAttribute+"'");
                    sqlStatementForPartitioningQuery.AppendLine("  AND C.name NOT IN ('" + recordSourceAttribute + "','" + etlProcessIdAttribute + "','" + loadDateTimeAttribute + "')");
                    sqlStatementForPartitioningQuery.AppendLine();

                    var partitionAttributes = GetDataTable(ref connHstg, sqlStatementForPartitioningQuery.ToString());

                    if (partitionAttributes.Rows.Count==0)
                    {
                        SetTextDebug("There is an issue generating the PSA logic for "+targetTableName+". This is because no natural key can be derived from the available indices on the PSA table. Please check if either a Primary Key (with naming convention PK_<table name>) is available, or a Unique Index (with naming conventions IX_<table name>) is available. The VEDW tool will use the PK or IX depending on the PSA Key Location configuration in the 'settings' tab. With the current setting it would expect to derive the key from the "+ targetTableIndexName + " index.");
                        SetTextDebug("\n\n");
                    }
                    else
                    {

                        var keycounter = 1;
                        foreach (DataRow businessKey in partitionAttributes.Rows)
                        {
                            if (keycounter == 1)
                            {
                                mainBusinessKey = businessKey["ATTRIBUTE_NAME"].ToString();
                            }

                            keycounter++;
                        }

                        // Add the main attribute list of the STG table for selection
                        var sqlStatementForSourceQuery = new StringBuilder();
                        sqlStatementForSourceQuery.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION");
                        sqlStatementForSourceQuery.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS");
                        sqlStatementForSourceQuery.AppendLine("WHERE TABLE_NAME= '" + sourceTableName + "'");
                        sqlStatementForSourceQuery.AppendLine("AND TABLE_SCHEMA = '" + TeamConfigurationSettings.SchemaName + "'");
                        sqlStatementForSourceQuery.AppendLine("  AND COLUMN_NAME !='" + TeamConfigurationSettings.EtlProcessAttribute + "'");
                        sqlStatementForSourceQuery.AppendLine("  AND COLUMN_NAME NOT LIKE '%HASH_HUB%'");
                        sqlStatementForSourceQuery.AppendLine("  AND COLUMN_NAME NOT LIKE '%HASH_LNK%'");
                        sqlStatementForSourceQuery.AppendLine("  AND COLUMN_NAME NOT LIKE '%HASH_SAT%'");
                        sqlStatementForSourceQuery.AppendLine("  AND COLUMN_NAME NOT IN ('" + etlProcessIdAttribute + "')");
                        sqlStatementForSourceQuery.AppendLine("ORDER BY ORDINAL_POSITION");

                        var sourceStructure = GetDataTable(ref connStg, sqlStatementForSourceQuery.ToString());


                        // Creating the  selection query
                        var psaView = new StringBuilder();

                        psaView.AppendLine("--");
                        psaView.AppendLine("-- PSA View definition for " + targetTableName);
                        psaView.AppendLine("-- Generated at 11/21/2014 11:52:15 AM");
                        psaView.AppendLine("--");
                        psaView.AppendLine();
                        psaView.AppendLine("USE [" + TeamConfigurationSettings.StagingDatabaseName + "]");
                        psaView.AppendLine("GO");

                        if (checkBoxIfExistsStatement.Checked)
                        {
                            psaView.AppendLine(
                                "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[" + VedwConfigurationSettings.VedwSchema + "].[" +
                                targetTableName + "]') AND type in (N'V'))");
                            psaView.AppendLine("DROP VIEW [" + VedwConfigurationSettings.VedwSchema + "].[" +
                                               targetTableName + "];");
                            psaView.AppendLine("GO");
                        }

                        psaView.AppendLine("CREATE VIEW [" + VedwConfigurationSettings.VedwSchema + "].[" +
                                           targetTableName + "] AS ");

                        psaView.AppendLine("SELECT ");
                        psaView.AppendLine("  " + targetTableName + "_" + TeamConfigurationSettings.DwhKeyIdentifier +
                                           ",");

                        foreach (DataRow attribute in sourceStructure.Rows)
                        {
                            psaView.AppendLine("  " + attribute["COLUMN_NAME"] + ",");
                        }

                        psaView.AppendLine("  LKP_" + TeamConfigurationSettings.RecordChecksumAttribute + ",");
                        psaView.AppendLine("  LKP_" + TeamConfigurationSettings.ChangeDataCaptureAttribute + ",");
                        psaView.AppendLine("  KEY_ROW_NUMBER");
                        psaView.AppendLine("FROM");
                        psaView.AppendLine("(");
                        psaView.AppendLine("  SELECT");
                        psaView.AppendLine("    "+VedwConfigurationSettings.hashingStartSnippet);

                        foreach (DataRow businessKey in partitionAttributes.Rows)
                        {
                            psaView.AppendLine("       ISNULL(RTRIM(CONVERT(" + stringDataType + "(100),STG." +
                                               (string) businessKey["ATTRIBUTE_NAME"] + ")),'NA')+'|'+");

                        }

                        psaView.AppendLine("       CONVERT(" + stringDataType + "(100),STG.[" +
                                           TeamConfigurationSettings.LoadDateTimeAttribute + "],126)+'|'");
                        psaView.AppendLine("    "+VedwConfigurationSettings.hashingEndSnippet+" AS " + targetTableName + "_" +
                                           TeamConfigurationSettings.DwhKeyIdentifier + ",");

                        foreach (DataRow attribute in sourceStructure.Rows)
                        {
                            psaView.AppendLine("    STG." + attribute["COLUMN_NAME"] + ",");
                        }

                        psaView.AppendLine("    COALESCE(maxsub.LKP_" +
                                           TeamConfigurationSettings.RecordChecksumAttribute + ",'N/A') AS LKP_" +
                                           TeamConfigurationSettings.RecordChecksumAttribute + ",");
                        psaView.AppendLine("    COALESCE(maxsub.LKP_" + cdcOperationAttribute + ",'N/A') AS LKP_" +
                                           cdcOperationAttribute + ",");

                        // ROW_NUMBER over Partition By query element
                        string currentBusinessKey;

                        psaView.AppendLine("    CAST(ROW_NUMBER() OVER (PARTITION  BY ");

                        foreach (DataRow businessKey in partitionAttributes.Rows)
                        {
                            currentBusinessKey = (string) businessKey["ATTRIBUTE_NAME"];
                            psaView.Append("       STG." + currentBusinessKey);
                            psaView.Append(",");
                        }

                        psaView.Remove(psaView.Length - 1, 1);
                        psaView.AppendLine();
                        psaView.AppendLine("    ORDER BY ");

                        foreach (DataRow businessKey in partitionAttributes.Rows)
                        {
                            currentBusinessKey = (string) businessKey["ATTRIBUTE_NAME"];
                            psaView.Append("       STG." + currentBusinessKey);
                            psaView.Append(", ");
                        }

                        psaView.Append("STG." + loadDateTimeAttribute + ") AS INT) AS KEY_ROW_NUMBER");

                        // End of the query			
                        psaView.AppendLine();
                        psaView.AppendLine("  FROM ["+TeamConfigurationSettings.SchemaName+"]." + sourceTableName + " STG");
                        psaView.AppendLine("  LEFT OUTER JOIN -- Prevent reprocessing");
                        psaView.AppendLine("    " + TeamConfigurationSettings.PsaDatabaseName + "." +
                                           TeamConfigurationSettings.SchemaName + "." + targetTableName + " HSTG");
                        psaView.AppendLine("    ON");

                        foreach (DataRow businessKey in partitionAttributes.Rows)
                        {
                            currentBusinessKey = (string) businessKey["ATTRIBUTE_NAME"];
                            psaView.AppendLine("       HSTG." + currentBusinessKey + " = STG." + currentBusinessKey +
                                               " AND");
                        }

                        psaView.AppendLine(
                            "       HSTG." + loadDateTimeAttribute + "=STG." + loadDateTimeAttribute + "");

                        psaView.AppendLine("  LEFT OUTER JOIN -- max HSTG checksum");
                        psaView.AppendLine("  (");

                        // Max subquery
                        psaView.AppendLine("    SELECT");

                        foreach (DataRow businessKey in partitionAttributes.Rows)
                        {
                            psaView.Append("      A.");
                            psaView.Append(businessKey["ATTRIBUTE_NAME"]);
                            psaView.AppendLine(",");
                        }

                        psaView.AppendLine("      A." + TeamConfigurationSettings.RecordChecksumAttribute + " AS LKP_" +
                                           TeamConfigurationSettings.RecordChecksumAttribute + ",");
                        psaView.AppendLine("      A." + cdcOperationAttribute + " AS LKP_" + cdcOperationAttribute +
                                           "");
                        psaView.AppendLine("    FROM " + TeamConfigurationSettings.PsaDatabaseName + "." +
                                           TeamConfigurationSettings.SchemaName + "." + targetTableName + " A");
                        psaView.AppendLine("    JOIN (");
                        psaView.AppendLine("      SELECT ");

                        foreach (DataRow businessKey in partitionAttributes.Rows)
                        {
                            psaView.Append("        B.");
                            psaView.Append(businessKey["ATTRIBUTE_NAME"]);
                            psaView.AppendLine(",");
                        }

                        psaView.AppendLine("        MAX(" + loadDateTimeAttribute + ") AS MAX_" +
                                           loadDateTimeAttribute + " ");
                        psaView.AppendLine("      FROM " + TeamConfigurationSettings.PsaDatabaseName + "." +
                                           TeamConfigurationSettings.SchemaName + "." + targetTableName + " B");
                        psaView.AppendLine("      GROUP BY ");

                        foreach (DataRow businessKey in partitionAttributes.Rows)
                        {
                            psaView.Append("       B.");
                            psaView.Append(businessKey["ATTRIBUTE_NAME"]);
                            psaView.AppendLine(",");
                        }

                        psaView.Remove(psaView.Length - 3, 3);

                        psaView.AppendLine();
                        psaView.AppendLine("      ) C ON");

                        foreach (DataRow businessKey in partitionAttributes.Rows)
                        {
                            psaView.AppendLine("        A." + businessKey["ATTRIBUTE_NAME"] + " = C." +
                                               businessKey["ATTRIBUTE_NAME"] + " AND");
                        }

                        psaView.Remove(psaView.Length - 1, 1);

                        psaView.AppendLine("        A." + loadDateTimeAttribute + " = C.MAX_" + loadDateTimeAttribute +
                                           "");
                        psaView.AppendLine("  ) maxsub ON");

                        foreach (DataRow businessKey in partitionAttributes.Rows)
                        {
                            psaView.AppendLine("     STG." + (string) businessKey["ATTRIBUTE_NAME"] + " = maxsub." +
                                               (string) businessKey["ATTRIBUTE_NAME"] + " AND");
                        }

                        psaView.Remove(psaView.Length - 5, 5);

                        psaView.AppendLine();
                        psaView.AppendLine("  WHERE");
                        psaView.AppendLine("    HSTG." + mainBusinessKey + " IS NULL -- prevent reprocessing");
                        psaView.AppendLine(") sub");
                        psaView.AppendLine("WHERE");
                        psaView.AppendLine("(");
                        psaView.AppendLine("  KEY_ROW_NUMBER=1");
                        psaView.AppendLine("  AND");
                        psaView.AppendLine("  (");
                        psaView.AppendLine("    (" + TeamConfigurationSettings.RecordChecksumAttribute + "!=LKP_" +
                                           TeamConfigurationSettings.RecordChecksumAttribute + ")");
                        psaView.AppendLine("    -- The checksums are different");
                        psaView.AppendLine("    OR");
                        psaView.AppendLine("    (" + TeamConfigurationSettings.RecordChecksumAttribute + "=LKP_" +
                                           TeamConfigurationSettings.RecordChecksumAttribute + " AND " +
                                           TeamConfigurationSettings.ChangeDataCaptureAttribute + "!=LKP_" +
                                           TeamConfigurationSettings.ChangeDataCaptureAttribute + ")");
                        psaView.AppendLine("    -- The checksums are the same but the CDC is different");
                        psaView.AppendLine(
                            "    -- In other words, if the hash is the same AND the CDC operation is the same then there is no change");
                        psaView.AppendLine("  )");
                        psaView.AppendLine(")");
                        psaView.AppendLine("OR");
                        psaView.AppendLine("(");
                        psaView.AppendLine(
                            "  -- It's not the most recent change in the set, so the record can be inserted as-is");
                        psaView.AppendLine("  KEY_ROW_NUMBER!=1");
                        psaView.AppendLine(")");
                        psaView.AppendLine();
                        psaView.AppendLine("GO");

                        using (var outfile =
                            new StreamWriter(VedwConfigurationSettings.VedwOutputPath + @"\VIEW_" + targetTableName +
                                             ".sql"))
                        {
                            outfile.Write(psaView.ToString());
                            outfile.Close();
                        }

                        if (checkBoxGenerateInDatabase.Checked)
                        {
                            connHstg.ConnectionString = TeamConfigurationSettings.ConnectionStringHstg;
                            int localError = GenerateInDatabase(connHstg, psaView.ToString());
                            errorCounter = errorCounter + localError;
                        }

                        SetTextDebug(psaView.ToString());
                        SetTextDebug("\n");
                    }
                }
            }
            else
            {
                SetTextPsa("There was no metadata selected to create Persistent Staging Area views. Please check the metadata schema - are there any Staging Area tables selected?");
            }

            SetTextPsa($"\r\n{errorCounter} errors have been found.\r\n");
            SetTextPsa($"SQL Scripts have been successfully saved in {VedwConfigurationSettings.VedwOutputPath}.\r\n");
        }

        private void SchemaboundCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is yet to be implemented.", "Upcoming!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void buttonGenerateStaging_Click(object sender, EventArgs e)
        {
            richTextBoxStaging.Clear();
            richTextBoxInformation.Clear();

            var newThread = new Thread(BackgroundDoStaging);
            newThread.Start();
        }

        private void BackgroundDoStaging(Object obj)
        {
            // Check if the schema needs to be created
            CreateSchema(TeamConfigurationSettings.ConnectionStringStg);

            if (radiobuttonViews.Checked)
            {
                StagingGenerateViews();
            }
            else if (radiobuttonStoredProc.Checked)
            {
                MessageBox.Show("This feature is yet to be implemented.", "Upcoming!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (radioButtonIntoStatement.Checked)
            {
                StagingGenerateInsertInto();
            }
        }

        private void StagingGenerateInsertInto()
        {
            // Setup error handling to report back to the user
            int errorCounter = 0;

            if (checkedListBoxStgMetadata.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= checkedListBoxStgMetadata.CheckedItems.Count - 1; x++)
                {
                    var targetTableName = checkedListBoxStgMetadata.CheckedItems[x].ToString();
                    var connStg = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringStg };

                    SetTextStaging($"Processing Staging Area insert statement for {targetTableName}\r\n");

                    // Build the main attribute list of the STG table for selection
                    var sqlStatementForSourceQuery = new StringBuilder();
                    sqlStatementForSourceQuery.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION");
                    sqlStatementForSourceQuery.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS");
                    sqlStatementForSourceQuery.AppendLine("WHERE TABLE_NAME= '" + targetTableName + "'");
                    sqlStatementForSourceQuery.AppendLine("AND TABLE_SCHEMA = '" + TeamConfigurationSettings.SchemaName + "'");
                    sqlStatementForSourceQuery.AppendLine("  AND COLUMN_NAME NOT IN ('" + TeamConfigurationSettings.LoadDateTimeAttribute + "','" +TeamConfigurationSettings.RowIdAttribute + "')");
                    sqlStatementForSourceQuery.AppendLine("ORDER BY ORDINAL_POSITION");

                    var sourceStructure = GetDataTable(ref connStg, sqlStatementForSourceQuery.ToString());

                    // Initial SQL
                    var stgInsertIntoStatement = new StringBuilder();
                    stgInsertIntoStatement.AppendLine("--");
                    stgInsertIntoStatement.AppendLine("-- Staging Area Insert Into statement for " + targetTableName);
                    stgInsertIntoStatement.AppendLine("-- Generated at " + DateTime.Now);
                    stgInsertIntoStatement.AppendLine("-- The Row ID and Load Date/Time will be created upon insert as default values");
                    stgInsertIntoStatement.AppendLine("--");
                    stgInsertIntoStatement.AppendLine();
                    stgInsertIntoStatement.AppendLine("USE [" + TeamConfigurationSettings.StagingDatabaseName + "]");
                    stgInsertIntoStatement.AppendLine("GO");
                    stgInsertIntoStatement.AppendLine();

                    if (checkBoxIfExistsStatement.Checked)
                    {
                        stgInsertIntoStatement.AppendLine("TRUNCATE TABLE [" + TeamConfigurationSettings.StagingDatabaseName + "].[" + TeamConfigurationSettings.SchemaName + "].[" + targetTableName + "]");
                        stgInsertIntoStatement.AppendLine("GO");
                        stgInsertIntoStatement.AppendLine();
                    }

                    stgInsertIntoStatement.AppendLine("INSERT INTO [" + TeamConfigurationSettings.StagingDatabaseName + "].[" +TeamConfigurationSettings.SchemaName + "].[" + targetTableName + "]");
                    stgInsertIntoStatement.AppendLine("   (");

                    foreach (DataRow attribute in sourceStructure.Rows)
                    {
                        stgInsertIntoStatement.AppendLine("   [" + attribute["COLUMN_NAME"] + "],");
                    }

                    stgInsertIntoStatement.Remove(stgInsertIntoStatement.Length - 3, 3);
                    stgInsertIntoStatement.AppendLine();
                    stgInsertIntoStatement.AppendLine("   )");
                    stgInsertIntoStatement.AppendLine("SELECT ");

                    foreach (DataRow attribute in sourceStructure.Rows)
                    {
                        if ((string)attribute["COLUMN_NAME"] == TeamConfigurationSettings.EtlProcessAttribute)
                        {
                            stgInsertIntoStatement.AppendLine("   -1 AS [" + attribute["COLUMN_NAME"] + "],");
                        }
                        else
                        {
                            stgInsertIntoStatement.AppendLine("   [" + attribute["COLUMN_NAME"] + "],");
                        }
                    }

                    stgInsertIntoStatement.Remove(stgInsertIntoStatement.Length - 3, 3);
                    stgInsertIntoStatement.AppendLine();
                    stgInsertIntoStatement.AppendLine("FROM ["+ VedwConfigurationSettings.VedwSchema + "].[" + targetTableName + "]");

                    using (var outfile = new StreamWriter(textBoxOutputPath.Text + @"\INSERT_STATEMENT_" + targetTableName + ".sql"))
                    {
                        outfile.Write(stgInsertIntoStatement.ToString());
                        outfile.Close();
                    }

                    if (checkBoxGenerateInDatabase.Checked)
                    {
                        connStg.ConnectionString = TeamConfigurationSettings.ConnectionStringHstg;
                        int insertError = GenerateInDatabase(connStg, stgInsertIntoStatement.ToString());
                        errorCounter = errorCounter + insertError;
                    }

                    SetTextDebug(stgInsertIntoStatement.ToString());
                    SetTextDebug("\n");
                }
            }
            else
            {
                SetTextStaging("There was no metadata selected to create Staging Area insert statements. Please check the metadata schema - are there any Staging Area tables selected?");
            }

            SetTextStaging($"\r\n{errorCounter} errors have been found.\r\n");
            SetTextStaging($"SQL Scripts have been successfully saved in {VedwConfigurationSettings.VedwOutputPath}.\r\n");
        }

        private void StagingGenerateViews()
        {
            // Setup error handling to report back to the user
            int errorCounter = 0;

            var connStg = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringStg};
            var connOmd = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringOmd};

            var etlProcessIdAttribute = TeamConfigurationSettings.EtlProcessAttribute;
            var cdcOperationAttribute = TeamConfigurationSettings.ChangeDataCaptureAttribute;
            var eventDateTimeAttribute = TeamConfigurationSettings.EventDateTimeAttribute;
            var sourceRowIdAttribute = TeamConfigurationSettings.RowIdAttribute;

            if (checkedListBoxStgMetadata.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= checkedListBoxStgMetadata.CheckedItems.Count - 1; x++)
                {
                    // Variables and parameters
                    var stagingAreaTableId = 0;
                    var hubTableId = 0;
                    var targetTableName = checkedListBoxStgMetadata.CheckedItems[x].ToString();
                    var parts = targetTableName.Split('_');
                    var sourceSystemName = parts[1];
                    var sourceTableName = targetTableName.Replace("STG_" + sourceSystemName + "_", "");
                    var historyAreaTableName = TeamConfigurationSettings.PsaTablePrefixValue + "_" + sourceSystemName + "_" + sourceTableName;
                    var mainBusinessKey = "";

                    var errorCapture = new StringBuilder();
                    var sqlStatementForSourceAttribute = new StringBuilder();
                    var sourceSqlStatement = new StringBuilder();
                    var sqlStatementForNaturalKey = new StringBuilder();
                    var sqlStatementForBusinessKeyHub = new StringBuilder();
                    var sqlStatementForBusinessKeyAttribute = new StringBuilder();
                    var sqlStatementForLnkBusinessKeyAttribute = new StringBuilder();

                    var stringDataType = checkBoxUnicode.Checked ? "NVARCHAR" : "VARCHAR";

                    connStg.ConnectionString = TeamConfigurationSettings.ConnectionStringStg;

                    SetTextStaging($"Processing Staging Area entity view for {targetTableName}\r\n");


                    // Creating the  selection query
                    var stgView = new StringBuilder();

                    stgView.AppendLine("--");
                    stgView.AppendLine("-- Staging Area View definition for " + targetTableName);
                    stgView.AppendLine("-- Generated at " + DateTime.Now);
                    stgView.AppendLine("--");
                    stgView.AppendLine();
                    stgView.AppendLine("USE [" + TeamConfigurationSettings.StagingDatabaseName + "]");
                    stgView.AppendLine("GO");

                    if (checkBoxIfExistsStatement.Checked)
                    {
                        stgView.AppendLine("IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[" + VedwConfigurationSettings.VedwSchema + "].[" +targetTableName + "]') AND type in (N'V'))");
                        stgView.AppendLine("DROP VIEW ["+ VedwConfigurationSettings.VedwSchema + "].[" + targetTableName + "]");
                        stgView.AppendLine("GO");
                    }

                    stgView.AppendLine("CREATE VIEW [" + VedwConfigurationSettings.VedwSchema + "].[" + targetTableName + "] AS ");

                    // Retrieving the Natural Key for the Staging Area table
                    sqlStatementForNaturalKey.Clear();
                    sqlStatementForNaturalKey.AppendLine("SELECT");
                    sqlStatementForNaturalKey.AppendLine("  st.name SOURCE_NAME,");
                    sqlStatementForNaturalKey.AppendLine("  sc.name AS STAGING_AREA_ATTRIBUTE_NAME,");
                    sqlStatementForNaturalKey.AppendLine("COALESCE(sep.value,'Error - no indicator present') AS NATURAL_KEY_INDICATOR");
                    sqlStatementForNaturalKey.AppendLine("FROM sys.tables st");
                    sqlStatementForNaturalKey.AppendLine("INNER JOIN sys.columns sc ");
                    sqlStatementForNaturalKey.AppendLine("  ON st.object_id = sc.object_id");
                    sqlStatementForNaturalKey.AppendLine("LEFT JOIN sys.extended_properties sep ");
                    sqlStatementForNaturalKey.AppendLine("  ON st.object_id = sep.major_id");
                    sqlStatementForNaturalKey.AppendLine(" AND sc.column_id = sep.minor_id");
                    sqlStatementForNaturalKey.AppendLine("WHERE COALESCE(sep.value,'Error - no indicator present')='Yes'");
                    sqlStatementForNaturalKey.AppendLine(" AND st.name = '" + targetTableName + "'");

                    var naturalKeyDataTable = GetDataTable(ref connStg, sqlStatementForNaturalKey.ToString());
                    if (naturalKeyDataTable != null)
                    {
                        naturalKeyDataTable.PrimaryKey = new[]
                        {naturalKeyDataTable.Columns["STAGING_AREA_ATTRIBUTE_NAME"]};
                    }

                    int keycounter = 1;
                    if (naturalKeyDataTable != null)
                    {
                        foreach (DataRow businessKey in naturalKeyDataTable.Rows)
                        {
                            if (keycounter == 1)
                            {
                                mainBusinessKey = businessKey["STAGING_AREA_ATTRIBUTE_NAME"].ToString();
                            }
                            keycounter++;
                        }
			
                        // Query the Staging Area to retrieve the attributes and datatypes, precisions and length
                        sqlStatementForSourceAttribute.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION, NUMERIC_SCALE");
                        sqlStatementForSourceAttribute.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS");
                        sqlStatementForSourceAttribute.AppendLine("WHERE TABLE_NAME= '" + targetTableName + "'");
                        sqlStatementForSourceAttribute.AppendLine("AND TABLE_SCHEMA = '" + TeamConfigurationSettings.SchemaName + "'");
                        sqlStatementForSourceAttribute.AppendLine("  AND COLUMN_NAME NOT IN ('" + eventDateTimeAttribute + "','" + sourceRowIdAttribute + "','" + cdcOperationAttribute + "', '"+TeamConfigurationSettings.AlternativeRecordSourceAttribute+"', '" + TeamConfigurationSettings.RecordSourceAttribute + "','" + etlProcessIdAttribute + "','" + TeamConfigurationSettings.LoadDateTimeAttribute + "','"+TeamConfigurationSettings.RecordChecksumAttribute+"')");
                        sqlStatementForSourceAttribute.AppendLine("ORDER BY ORDINAL_POSITION");

                        var sourceStructure = GetDataTable(ref connStg, sqlStatementForSourceAttribute.ToString());

                        if (sourceStructure != null)
                        {
                            sourceStructure.PrimaryKey = new[] { sourceStructure.Columns["COLUMN_NAME"] };
                        }
				
                        // Query the Staging Area to retrieve what keys need to be pre-hashed
                        var hashListQuery = new StringBuilder();
                        hashListQuery.AppendLine("SELECT ");
                        hashListQuery.AppendLine(" COLUMN_NAME, ");
                        hashListQuery.AppendLine(" CASE ");
                        hashListQuery.AppendLine("   WHEN SUBSTRING(COLUMN_NAME,6,3)='HUB' THEN 'HUB' ");
                        hashListQuery.AppendLine("   WHEN SUBSTRING(COLUMN_NAME,6,3)='LNK' THEN 'LNK' ");
                        hashListQuery.AppendLine(" ELSE 'UNKNOWN' ");
                        hashListQuery.AppendLine(" END AS TABLE_TYPE, ");
                        hashListQuery.AppendLine(" SUBSTRING(COLUMN_NAME, 10,LEN(COLUMN_NAME)) AS TABLE_NAME ");
                        hashListQuery.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS");
                        hashListQuery.AppendLine("WHERE TABLE_NAME= '" + targetTableName + "'");
                        hashListQuery.AppendLine("AND TABLE_SCHEMA = '" + TeamConfigurationSettings.SchemaName + "'");
                        hashListQuery.AppendLine("  AND SUBSTRING(COLUMN_NAME,1,5)='HASH_'");
                        hashListQuery.AppendLine("  AND COLUMN_NAME!='"+TeamConfigurationSettings.RecordChecksumAttribute+"'");
                        hashListQuery.AppendLine("ORDER BY ORDINAL_POSITION");

                        var hashList = GetDataTable(ref connStg, hashListQuery.ToString());
                        if (hashList != null)
                        {
                            hashList.PrimaryKey = new[] {hashList.Columns["COLUMN_NAME"]};
                        }

                        // Creating the source query
                        stgView.AppendLine("WITH STG_CTE AS ");
                        stgView.AppendLine("(");
                        stgView.AppendLine("SELECT");

                        // Adding the attributes to the main query against the source system
                        if (sourceStructure != null)
                        {
                            foreach (DataRow attribute in sourceStructure.Rows)
                            {
                                var attDataType = attribute["DATA_TYPE"].ToString();
                                var attName = attribute["COLUMN_NAME"].ToString();
                                var attScale = attribute["NUMERIC_SCALE"].ToString();

                                {
                                    var foundRow = naturalKeyDataTable.Rows.Find(attName);
                                    // Lookup the primary key(s) as they need to be specifically converted to support the MJO

                                    if (foundRow != null)
                                    {
                                        switch (attDataType)
                                        {
                                            case "bit":
                                                sourceSqlStatement.AppendLine("   CONVERT(INT,[" + attName + "]) AS [" + attName +"],");
                                                break;
                                            case "float":
                                            case "real":
                                            case "money":
                                            case "numeric":
                                            {
                                                if (attScale == "20")
                                                {
                                                    stgView.AppendLine("   CONVERT(NUMERIC(38,20),[" + attName + "]) AS [" +attName + "],");
                                                }
                                                else
                                                {
                                                    stgView.AppendLine("   CONVERT(NUMERIC(38,0),[" + attName + "]) AS [" +attName + "],");
                                                }

                                                break;
                                            }
                                            case "text":
                                            case "ntext":
                                                stgView.AppendLine("   CONVERT(" + stringDataType + "(4000),[" + attName + "]) AS [" + attName +"],");
                                                break;
                                            case "datetime":
                                            case "datetime2":
                                                stgView.AppendLine("   CONVERT(DATETIME2(7),[" + attName + "]) AS [" + attName +"],");
                                                break;
                                            default:
                                                stgView.AppendLine("   [" + attName + "] AS [" + attName + "],");
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        // This concerns all non-key attributes
                                        stgView.AppendLine("   [" + attName + "] AS [" + attName + "],");
                                    }
                                }
                            }
                        }

                        // Hash on full record
                        stgView.AppendLine("   "+VedwConfigurationSettings.hashingStartSnippet);

                        if (sourceStructure != null)
                        {
                            foreach (DataRow attribute in sourceStructure.Rows)
                            {
                                stgView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100),[" + attribute["COLUMN_NAME"] + "])),'N/A')+'|'+");
                            }
                        }

                        stgView.Remove(stgView.Length - 3, 3);
                        stgView.AppendLine();
                        stgView.AppendLine("   "+VedwConfigurationSettings.hashingEndSnippet+" AS ["+TeamConfigurationSettings.RecordChecksumAttribute+"],");

                        // Hash on Business Keys
                        if (hashList != null)
                        {
                            foreach (DataRow hub in hashList.Rows)
                            {
                                var hubTableName = TeamConfigurationSettings.HubTablePrefixValue + '_' + hub["TABLE_NAME"];

                                if (hub["TABLE_TYPE"].ToString() == "HUB")
                                {
                                    sqlStatementForBusinessKeyHub.Clear();
                                    sqlStatementForBusinessKeyAttribute.Clear();

                                    // Retrieving the Business Keys for future parallel DV2.0 processing
                                    sqlStatementForBusinessKeyHub.AppendLine("SELECT");
                                    sqlStatementForBusinessKeyHub.AppendLine("  c.SOURCE_ID,");
                                    sqlStatementForBusinessKeyHub.AppendLine("  c.SOURCE_NAME,");
                                    sqlStatementForBusinessKeyHub.AppendLine("  b.HUB_ID,");                                
                                    sqlStatementForBusinessKeyHub.AppendLine("  b.HUB_NAME,");
                                    sqlStatementForBusinessKeyHub.AppendLine("FROM MD_STG_HUB_XREF a ");
                                    sqlStatementForBusinessKeyHub.AppendLine("JOIN MD_HUB b ON a.HUB_ID = b.HUB_ID ");
                                    sqlStatementForBusinessKeyHub.AppendLine("JOIN MD_STG c on a.SOURCE_ID = c.SOURCE_ID ");
                                    sqlStatementForBusinessKeyHub.AppendLine("WHERE SOURCE_NAME = '" +targetTableName + "'");
                                    sqlStatementForBusinessKeyHub.AppendLine("AND b.HUB_NAME = '" + hubTableName + "'");

                                    var businessKeyDataTable = GetDataTable(ref connOmd,sqlStatementForBusinessKeyHub.ToString());

                                    foreach (DataRow businessKey in businessKeyDataTable.Rows)
                                    {
                                        stagingAreaTableId = (int) businessKey["SOURCE_ID"];
                                        hubTableId = (int)businessKey["HUB_ID"];
                                    }


                                    // Retrieving the Business Keys attribute(s) as represented in the source for hashing
                                    sqlStatementForBusinessKeyAttribute.AppendLine("SELECT comp.SOURCE_ID, comp.HUB_ID, comp.COMPONENT_ID, COMPONENT_TYPE, COMPONENT_ELEMENT_TYPE, COMPONENT_ELEMENT_VALUE AS ATTRIBUTE_NAME");
                                    sqlStatementForBusinessKeyAttribute.AppendLine("FROM MD_BUSINESS_KEY_COMPONENT comp");
                                    sqlStatementForBusinessKeyAttribute.AppendLine("JOIN MD_BUSINESS_KEY_COMPONENT_PART elem ");
                                    sqlStatementForBusinessKeyAttribute.AppendLine("  ON comp.COMPONENT_ID=elem.COMPONENT_ID");
                                    sqlStatementForBusinessKeyAttribute.AppendLine(" AND elem.SOURCE_ID = comp.SOURCE_ID");
                                    sqlStatementForBusinessKeyAttribute.AppendLine(" AND elem.HUB_ID = comp.HUB_ID");
                                    sqlStatementForBusinessKeyAttribute.AppendLine("WHERE comp.SOURCE_ID= '" + stagingAreaTableId + "'");
                                    sqlStatementForBusinessKeyAttribute.AppendLine("AND comp.HUB_ID= '" + hubTableId + "'");
                                    sqlStatementForBusinessKeyAttribute.AppendLine("ORDER BY comp.COMPONENT_ORDER,COMPONENT_ELEMENT_ORDER");

                                    var businessKeyAttributeDataTable = GetDataTable(ref connOmd,sqlStatementForBusinessKeyAttribute.ToString());

                                    stgView.AppendLine("   "+VedwConfigurationSettings.hashingStartSnippet);

                                    foreach (DataRow attribute in businessKeyAttributeDataTable.Rows)
                                    {
                                        stgView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + attribute["ATTRIBUTE_NAME"] + ")),'NA')+'|'+");
                                    }

                                    stgView.Remove(stgView.Length - 3, 3);
                                    stgView.AppendLine();
                                    stgView.AppendLine("   "+VedwConfigurationSettings.hashingEndSnippet+" AS HASH_" + hubTableName + ",");
                                }
                                else
                                {
                                    errorCapture.AppendLine("There is no Hub detected: processing " + hubTableName + " as " + hub["TABLE_TYPE"]);
                                }
                            }
                        }

                        // Hash on Link Keys
                        if (hashList != null)
                        {
                            foreach (DataRow lnk in hashList.Rows)
                            {
                                var lnkTableName = TeamConfigurationSettings.LinkTablePrefixValue + '_' + lnk["TABLE_NAME"];

                                if (lnk["TABLE_TYPE"].ToString() == "LNK")
                                {
                                    // Retrieving the Business Keys attribute(s) as represented in the source for hashing
                                    sqlStatementForLnkBusinessKeyAttribute.Clear();
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("SELECT ");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("  c.SOURCE_ID,");                                
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("  c.SOURCE_NAME,");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("  b.HUB_ID,");                                
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("  b.HUB_NAME,");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("  f.LINK_ID,");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("  f.LINK_NAME,");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("  COMPONENT_ELEMENT_VALUE");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("FROM MD_STG_HUB_XREF a ");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("JOIN MD_HUB b ON a.HUB_ID=b.HUB_ID ");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("JOIN MD_STG c on a.SOURCE_ID = c.SOURCE_ID ");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("JOIN MD_BUSINESS_KEY_COMPONENT d on d.SOURCE_ID = a.SOURCE_ID ");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("AND d.HUB_ID = a.HUB_ID ");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("JOIN MD_HUB_LINK_XREF e on b.HUB_ID=e.HUB_ID");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("JOIN MD_LINK f on e.LINK_ID=f.LINK_ID");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("JOIN MD_BUSINESS_KEY_COMPONENT_PART elem ON comp.COMPONENT_ID=elem.COMPONENT_ID");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("AND elem.SOURCE_ID = comp.SOURCE_ID");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("AND elem.HUB_ID = comp.HUB_ID");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("WHERE ");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("   SOURCE_NAME= '" +targetTableName + "'");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("AND f.LINK_NAME= '" +lnkTableName + "'");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("ORDER BY HUB_NAME, comp.COMPONENT_ID, COMPONENT_ELEMENT_ORDER");

                                    var lnkBusinessKeyAttributeDataTable = GetDataTable(ref connOmd,sqlStatementForLnkBusinessKeyAttribute.ToString());

                                    stgView.AppendLine("   "+VedwConfigurationSettings.hashingStartSnippet);

                                    foreach (DataRow attribute in lnkBusinessKeyAttributeDataTable.Rows)
                                    {
                                        stgView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + attribute["COMPONENT_ELEMENT_VALUE"] + ")),'NA')+'|'+");
                                    }

                                    stgView.Remove(stgView.Length - 3, 3);
                                    stgView.AppendLine();
                                    stgView.AppendLine("   "+VedwConfigurationSettings.hashingEndSnippet+" AS HASH_" + lnkTableName + ",");
                                }
                                else
                                {
                                    errorCapture.AppendLine("There is no Link detected: processing " + lnkTableName + " as " + lnk["TABLE_TYPE"] + " with source as " + targetTableName);
                                }
                            }
                        }

                        stgView.Remove(stgView.Length - 3, 3);
                        stgView.AppendLine();
                        stgView.AppendLine("FROM [" + TeamConfigurationSettings.SourceDatabaseName + "].[" + TeamConfigurationSettings.SchemaName + "].[" +sourceTableName+"]");
                        stgView.AppendLine("),");
	
                        // Creating the History Area query
                        stgView.AppendLine("PSA_CTE AS");
                        stgView.AppendLine("(");
                        stgView.AppendLine("SELECT");
                        stgView.AppendLine("   A." + TeamConfigurationSettings.RecordChecksumAttribute + " AS " + TeamConfigurationSettings.RecordChecksumAttribute + ",");

                        // Adding the attributes to the main query against the source system
                        if (sourceStructure != null)
                        {
                            foreach (DataRow attribute in sourceStructure.Rows)
                            {
                                stgView.AppendLine("   A." + attribute["COLUMN_NAME"] + " AS " +attribute["COLUMN_NAME"] + ",");
                            }
                        }

                        stgView.Remove(stgView.Length - 3, 3);
                        stgView.AppendLine();
                        stgView.AppendLine("FROM " + TeamConfigurationSettings.PsaDatabaseName+"."+TeamConfigurationSettings.SchemaName+"."+historyAreaTableName + " A");

                        stgView.AppendLine("   JOIN (");
                        stgView.AppendLine("        SELECT");

                        foreach (DataRow businessKey in naturalKeyDataTable.Rows)
                        {
                            stgView.Append("            [" + businessKey["STAGING_AREA_ATTRIBUTE_NAME"]+"]");
                            stgView.Append(",");
                        }

                        stgView.AppendLine();
                        stgView.AppendLine("            MAX(" + TeamConfigurationSettings.LoadDateTimeAttribute + ") AS MAX_" + TeamConfigurationSettings.LoadDateTimeAttribute + "");
                        stgView.AppendLine("        FROM " + TeamConfigurationSettings.PsaDatabaseName+"."+TeamConfigurationSettings.SchemaName+"."+historyAreaTableName);
                        stgView.AppendLine("        GROUP BY");

                        foreach (DataRow businessKey in naturalKeyDataTable.Rows)
                        {
                            stgView.Append("         " + businessKey["STAGING_AREA_ATTRIBUTE_NAME"]);
                            stgView.Append(",");
                        }
                        stgView.Remove(stgView.Length - 1, 1);

                        stgView.AppendLine();
                        stgView.AppendLine("        ) B ON");

                        foreach (DataRow businessKey in naturalKeyDataTable.Rows)
                        {
                            stgView.Append("   A." + businessKey["STAGING_AREA_ATTRIBUTE_NAME"] +
                                           " = B." + businessKey["STAGING_AREA_ATTRIBUTE_NAME"]);
                            stgView.AppendLine("   AND");
                        }

                        stgView.Remove(stgView.Length - 7, 7);
                        stgView.AppendLine();
                        stgView.AppendLine("   AND");
                        stgView.AppendLine("   A." + TeamConfigurationSettings.LoadDateTimeAttribute + " = B.MAX_" + TeamConfigurationSettings.LoadDateTimeAttribute + "");
                        stgView.AppendLine("WHERE "+TeamConfigurationSettings.ChangeDataCaptureAttribute+" != 'Delete'");
                        stgView.AppendLine(")");

                        // Putting together the CTE join
                        stgView.AppendLine("SELECT");

                        if (sourceStructure != null)
                        {
                            foreach (DataRow attribute in sourceStructure.Rows)
                            {
                                var attName = attribute["COLUMN_NAME"].ToString();
                                var attDataType = attribute["DATA_TYPE"].ToString();

                                if (attDataType.ToUpper() == "VARCHAR" || attDataType.ToUpper() == "NVARCHAR")
                                {
                                    stgView.AppendLine("  CASE WHEN STG_CTE.[" + mainBusinessKey + "] IS NULL THEN PSA_CTE.["+attName+"] ELSE STG_CTE.["+attName+"] COLLATE DATABASE_DEFAULT END AS ["+attName+"], ");
                                }
                                else
                                {
                                    stgView.AppendLine("  CASE WHEN STG_CTE.[" + mainBusinessKey + "] IS NULL THEN PSA_CTE.[" + attName + "] ELSE STG_CTE.[" + attName + "] END AS [" + attName + "], ");

                                }
                            }

                            stgView.AppendLine("  CASE WHEN STG_CTE.[" + mainBusinessKey + "] IS NULL THEN PSA_CTE.[" + TeamConfigurationSettings.RecordChecksumAttribute + "] ELSE STG_CTE.[" + TeamConfigurationSettings.RecordChecksumAttribute + "] "+VedwConfigurationSettings.hashingCollation+" END AS [" + TeamConfigurationSettings.RecordChecksumAttribute + "], ");

                            if (hashList != null)
                                foreach (DataRow hashkey in hashList.Rows)
                                {
                                    var intTableName="";

                                    if (hashkey["TABLE_TYPE"].ToString() == "HUB")
                                    {
                                        intTableName = TeamConfigurationSettings.HubTablePrefixValue + '_' + hashkey["TABLE_NAME"];
                                    }
                                    else if (hashkey["TABLE_TYPE"].ToString() == "LNK")
                                    {
                                        intTableName = TeamConfigurationSettings.LinkTablePrefixValue + '_' + hashkey["TABLE_NAME"];
                                    }

                                 

                                    stgView.AppendLine("  HASH_" + intTableName+",");
                                }

                            stgView.AppendLine("  CASE " +
                                               "WHEN STG_CTE.[" + mainBusinessKey + "] IS NULL THEN 'Delete' " +
                                               "WHEN PSA_CTE.[" + mainBusinessKey + "] IS NULL THEN 'Insert' " +
                                               "WHEN STG_CTE." + mainBusinessKey + " IS NOT NULL AND PSA_CTE." + mainBusinessKey + " IS NOT NULL AND STG_CTE." + TeamConfigurationSettings.RecordChecksumAttribute + " != PSA_CTE."+TeamConfigurationSettings.RecordChecksumAttribute+" THEN 'Change' " +
                                               "ELSE 'No Change' " +
                                               "END AS [" + TeamConfigurationSettings.ChangeDataCaptureAttribute + "],");
                            stgView.AppendLine("  '" + TeamConfigurationSettings.SourceSystemPrefix + "' AS " + TeamConfigurationSettings.RecordSourceAttribute+ ",");

                            stgView.AppendLine("  ROW_NUMBER() OVER ");
                            stgView.AppendLine("    (ORDER BY ");

                            foreach (DataRow businessKey in naturalKeyDataTable.Rows)
                            {
                                var keyDataType = sourceStructure.Rows.Find(businessKey["STAGING_AREA_ATTRIBUTE_NAME"]);
                                var keyDataTypeAttribute = keyDataType[1];

                                if (keyDataTypeAttribute.ToString().ToUpper() == "VARCHAR" || keyDataTypeAttribute.ToString().ToUpper() == "NVARCHAR")
                                {
                                    stgView.AppendLine("      CASE WHEN STG_CTE.[" + businessKey["STAGING_AREA_ATTRIBUTE_NAME"] + "] COLLATE DATABASE_DEFAULT IS NULL THEN PSA_CTE.[" + businessKey["STAGING_AREA_ATTRIBUTE_NAME"] + "] ELSE STG_CTE.[" + businessKey["STAGING_AREA_ATTRIBUTE_NAME"] + "] COLLATE DATABASE_DEFAULT END,");
                                }
                                else
                                {
                                    stgView.AppendLine("      CASE WHEN STG_CTE.[" + businessKey["STAGING_AREA_ATTRIBUTE_NAME"] + "] IS NULL THEN PSA_CTE.[" + businessKey["STAGING_AREA_ATTRIBUTE_NAME"] + "] ELSE STG_CTE.[" + businessKey["STAGING_AREA_ATTRIBUTE_NAME"] + "] END,");                
                                }
            
                            }
                            stgView.Remove(stgView.Length - 3, 3);
                            stgView.AppendLine();
                            stgView.AppendLine("    ) AS "+TeamConfigurationSettings.RowIdAttribute+",");
                            stgView.AppendLine("  GETDATE() AS " + TeamConfigurationSettings.EventDateTimeAttribute);

                            stgView.AppendLine("FROM STG_CTE");
                            stgView.AppendLine("FULL OUTER JOIN PSA_CTE ON ");

                            foreach (DataRow businessKey in naturalKeyDataTable.Rows)
                            {
                                var keyDataType = sourceStructure.Rows.Find(businessKey["STAGING_AREA_ATTRIBUTE_NAME"]);
                                var keyDataTypeAttribute = keyDataType[1];

                                if (keyDataTypeAttribute.ToString().ToUpper() == "VARCHAR" || keyDataTypeAttribute.ToString().ToUpper() == "NVARCHAR")
                                {
                                    stgView.AppendLine("PSA_CTE." + businessKey["STAGING_AREA_ATTRIBUTE_NAME"] + " = STG_CTE." + businessKey["STAGING_AREA_ATTRIBUTE_NAME"] +" COLLATE DATABASE_DEFAULT");
                                }
                                else
                                {
                                    stgView.AppendLine("PSA_CTE." + businessKey["STAGING_AREA_ATTRIBUTE_NAME"] + " = STG_CTE." + businessKey["STAGING_AREA_ATTRIBUTE_NAME"]);
                                }
                                stgView.AppendLine("AND");
                            }

                            stgView.Remove(stgView.Length - 6, 6);
                            stgView.AppendLine("WHERE ");
                            stgView.AppendLine("(");

                            try
                            {
                                var mainBusinessKeyLookup = sourceStructure.Rows.Find(mainBusinessKey);
                                var mainBusinessKeyDataType = mainBusinessKeyLookup[1];


                                stgView.AppendLine("  CASE ");
                                stgView.AppendLine("     WHEN STG_CTE.[" + mainBusinessKey + "] IS NULL THEN 'Delete' ");
                                stgView.AppendLine("     WHEN PSA_CTE.[" + mainBusinessKey + "] IS NULL THEN 'Insert' ");

                                if (mainBusinessKeyDataType.ToString().ToUpper() == "VARCHAR")
                                {
                                    stgView.AppendLine("     WHEN STG_CTE." + mainBusinessKey + " IS NOT NULL AND PSA_CTE." +
                                                       mainBusinessKey + " IS NOT NULL AND STG_CTE." +
                                                       TeamConfigurationSettings.RecordChecksumAttribute + " "+VedwConfigurationSettings.hashingCollation+" != PSA_CTE." +
                                                       TeamConfigurationSettings.RecordChecksumAttribute + " THEN 'Change' ");
                                }
                                else
                                {
                                    stgView.AppendLine("     WHEN STG_CTE." + mainBusinessKey + " IS NOT NULL AND PSA_CTE." +
                                                       mainBusinessKey + " IS NOT NULL AND STG_CTE." +
                                                       TeamConfigurationSettings.RecordChecksumAttribute + " != PSA_CTE." + TeamConfigurationSettings.RecordChecksumAttribute +
                                                       " THEN 'Change' ");
                                }
                            }
                            catch
                            {
                                SetTextDebug("Issues occurred while interpreting the table key for table " + targetTableName +
                                             ".\r\n");
                            }
                        }
                    }

                    stgView.AppendLine("     ELSE 'No Change' ");
                    stgView.AppendLine("     END ");
                    stgView.AppendLine(") != 'No Change'");
                    stgView.AppendLine();
                    stgView.AppendLine("GO");
                    stgView.AppendLine();
                    
                    using (var outfile = new StreamWriter(VedwConfigurationSettings.VedwOutputPath + @"\VIEW_" + targetTableName + ".sql"))
                    {
                        outfile.Write(stgView.ToString());
                        outfile.Close();
                    }

                    if (checkBoxGenerateInDatabase.Checked)
                    {
                        connStg.ConnectionString = TeamConfigurationSettings.ConnectionStringStg;
                        var localError = GenerateInDatabase(connStg, stgView.ToString());
                        errorCounter = errorCounter + localError;
                    }

                    SetTextDebug(stgView.ToString());
                    SetTextDebug("\n");
                }
            }
            else
            {
                SetTextStaging("There was no metadata selected to create Staging Area views. Please check the metadata schema - are there any Staging Area tables selected?");
            }

            SetTextStaging($"\r\n{errorCounter} errors have been found.\r\n");
            SetTextStaging($"SQL Scripts have been successfully saved in {VedwConfigurationSettings.VedwOutputPath}.\r\n");
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

  
        // Multithreading for updating the user (debugging form)
        delegate void SetTextCallBackDebug(string text);
        private void SetTextDebug(string text)
        {
            if (richTextBoxInformation.InvokeRequired)
            {
                var d = new SetTextCallBackDebug(SetTextDebug);
                Invoke(d, text);
            }
            else
            {
                richTextBoxInformation.AppendText(text);
            }
        }

        // Multithreading for updating the user (Staging Area form)
        delegate void SetTextCallBackStaging(string text);
        private void SetTextStaging(string text)
        {
            if (richTextBoxStaging.InvokeRequired)
            {
                var d = new SetTextCallBackStaging(SetTextStaging);
                Invoke(d, text);
            }
            else
            {
                richTextBoxStaging.AppendText(text);
            }
        }

        // Multithreading for updating the user (Persistent Staging Area form)
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

        // Multithreading for updating the user (Hub form)
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

        //// Multithreading for changing version stuff
        //delegate void SetVersionCallBack(int versionId);
        //internal void SetVersion(int versionId)
        //{
        //    if (trackBarVersioning.InvokeRequired)
        //    {
        //        var d = new SetVersionCallBack(SetVersion);
        //        Invoke(d, versionId);
        //    }
        //    else
        //    {
        //        InitialiseVersion();
        //        trackBarVersioning.Value = versionId;
        //    }
        //}

        // Multithreading for updating the user (Sat form)
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

        // Multithreading for updating the user (Link form)
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

        // Multithreading for updating the user (Link Satellite form)
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

        # region Background worker

        // This event handler deals with the results of the background operation.
        private void backgroundWorkerActivateMetadata_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
               // labelResult.Text = "Cancelled!";
                richTextBoxInformation.AppendText("Cancelled!");
            }
            else if (e.Error != null)
            {
                richTextBoxInformation.AppendText("Error: " + e.Error.Message);
            }
            else
            {
                richTextBoxInformation.AppendText("Done. The metadata was processed succesfully!\r\n");
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
            _alert.Message = "In progress, please wait... " + e.ProgressPercentage + "%";
            _alert.ProgressValue = e.ProgressPercentage;

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
                    SetTextStaging("There was no metadata available to display Staging Area content. Please check the metadata schema (are there any Staging Area tables available?) or the database connection.");
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
                    SetTextSat(
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
                richTextBoxInformation.Clear();
                richTextBoxInformation.Text = _errorMessage.ToString();
            }
        }

        private void button_Repopulate_PSA(object sender, EventArgs e)
        {
            

            var connPsa = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringHstg };
            _errorMessage.Clear();
            PopulatePsaCheckboxList(connPsa);
            if (_errorCounter > 0)
            {
                richTextBoxInformation.Clear();
                richTextBoxInformation.Text = _errorMessage.ToString();
            }
        }

        private void button_Repopulate_Sat(object sender, EventArgs e)
        {
            

            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            _errorMessage.Clear();
            PopulateSatCheckboxList(connOmd);
            if (_errorCounter > 0)
            {
                richTextBoxInformation.Clear();
                richTextBoxInformation.Text = _errorMessage.ToString();
            }
        }

        private void button_Repopulate_Lnk(object sender, EventArgs e)
        {
            

            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            _errorMessage.Clear();
            PopulateLnkCheckboxList(connOmd);
            if (_errorCounter > 0)
            {
                richTextBoxInformation.Clear();
                richTextBoxInformation.Text = _errorMessage.ToString();
            }
        }

        private void button_Repopulate_LSAT(object sender, EventArgs e)
        {
            

            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            _errorMessage.Clear();
            PopulateLsatCheckboxList(connOmd);
            if (_errorCounter > 0)
            {
                richTextBoxInformation.Clear();
                richTextBoxInformation.Text = _errorMessage.ToString();
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

        private void richTextBoxInformation_TextChanged(object sender, EventArgs e)
        {
            CheckKeyword("Issues occurred", Color.Red, 0);
            CheckKeyword("The statement was executed succesfully.", Color.GreenYellow, 0);
            // this.CheckKeyword("if", Color.Green, 0);
        }


        private void unknownKeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is yet to be implemented.", "Upcoming!", MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
        }


        private void tabPageDefaultSettings_Click(object sender, EventArgs e)
        {

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
                    SetTextDebug("An issue occured creating the VEDW schema '" + VedwConfigurationSettings.VedwSchema + "'. The reported error is " + ex);
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

            // Unicode checkbox
            if (checkBoxUnicode.Checked)
            {
                VedwConfigurationSettings.EnableUnicode = "True";
            }
            else if (!checkBoxUnicode.Checked)
            {
                VedwConfigurationSettings.EnableUnicode = "False";
            }
            else
            {
                richTextBoxInformation.AppendText("An issue was encountered saving the Unicode checkbox, can you verify the settings file in the Configuration directory?");
            }

            // Hashing vs natural BK checkbox
            if (checkBoxDisableHash.Checked)
            {
                VedwConfigurationSettings.DisableHash = "True";
            }
            else if (!checkBoxDisableHash.Checked)
            {
                VedwConfigurationSettings.DisableHash = "False";
            }
            else
            {
                richTextBoxInformation.AppendText("An issue was encountered saving the Hash Disabling checkbox, can you verify the settings file in the Configuration directory?");
            }
            
            // Hash type radiobutton
            if (radioButtonBinaryHash.Checked)
            {
                VedwConfigurationSettings.HashKeyOutputType = "Binary";
            }
            else if (!radioButtonBinaryHash.Checked)
            {
                VedwConfigurationSettings.HashKeyOutputType = "Character";
            }
            else
            {
                richTextBoxInformation.AppendText("An issue was encountered saving the Hash Disabling checkbox, can you verify the settings file in the Configuration directory?");
            }

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
                richTextBoxInformation.AppendText("An issue was encountered saving the Hash Disabling checkbox, can you verify the settings file in the Configuration directory?");
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

            richTextBoxInformation.Text = "The global parameter file ("+GlobalParameters.VedwConfigurationfileName + GlobalParameters.VedwFileExtension+ ") has been updated in: " + GlobalParameters.VedwConfigurationPath;
        }

        private void openConfigurationDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(VedwConfigurationSettings.TeamConfigurationPath);
            }
            catch (Exception ex)
            {
                richTextBoxInformation.Text = "An error has occured while attempting to open the configuration directory. The error message is: " + ex;
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
                richTextBoxInformation.Text = "An error has occured while attempting to open the configuration directory. The error message is: " + ex;
            }
        }

        private void radioButtonBinaryHash_CheckedChanged(object sender, EventArgs e)
        {
            UpdateHashSnippets();
        }

        private void radioButtonDevelopment_CheckedChanged(object sender, EventArgs e)
        {

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
    }
}
