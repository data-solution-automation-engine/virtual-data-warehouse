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
using Virtual_EDW.Classes;

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

            // Load the rest of the (TEAM) configurations, from wherever they may be according to the VEDW settings (the TEAM configuration file)\
            var teamConfigurationFileName = VedwConfigurationSettings.TeamConfigurationPath +
                                                           GlobalParameters.TeamConfigurationfileName + '_' +
                                                           VedwConfigurationSettings.WorkingEnvironment +
                                                           GlobalParameters.VedwFileExtension;

            richTextBoxInformation.Text = "Retrieving TEAM configuration details from '" + teamConfigurationFileName + "'. \r\n\r\n";

            StringBuilder teamConfigResult = EnvironmentConfiguration.LoadTeamConfigurationFile(teamConfigurationFileName);
        
            if (teamConfigResult.Length>0)
            {
                richTextBoxInformation.AppendText("Issues have been encountered while retrieving the TEAM configuration details. The following is returned: " + teamConfigResult.ToString() + "\r\n\r\n");
            }
            // Make sure the retrieved variables are displayed on the form
            UpdateVedwConfigurationSettingsOnForm();

            // Start monitoring the configuration directories for file changes
            // RunFileWatcher(); DISABLED FOR NOW - FIRES 2 EVENTS!!

            richTextBoxInformation.AppendText("Application initialised - welcome to Enterprise Data Warehouse Virtualisation. \r\n\r\n");

            checkBoxGenerateInDatabase.Checked = false;
            checkBoxIfExistsStatement.Checked = true;
            radiobuttonViews.Checked = true;
            SQL2014Radiobutton.Checked = true;
            checkBoxDisableSatZeroRecords.Checked = false;
            checkBoxDisableLsatZeroRecords.Checked = false;

            InitialiseDocumentation();

            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            var connStg = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringStg };
            var connPsa = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringHstg };

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
                richTextBoxInformation.AppendText("There was an issue establishing a database connection to the Metadata Repository Database. These are managed via the TEAM configuration files. The reported database connection string is '"+ FormBase.TeamConfigurationSettings.ConnectionStringOmd + "'.\r\n");
            }

            try
            {
                connStg.Open();
                PopulateStgCheckboxList(connStg);
            }
            catch
            {
                richTextBoxInformation.AppendText("There was an issue establishing a database connection to the Staging Area Database. These are managed via the TEAM configuration files. The reported database connection string is '" + FormBase.TeamConfigurationSettings.ConnectionStringOmd + "'.\r\n");
            }

            try
            {
                connPsa.Open();
                PopulatePsaCheckboxList(connPsa);
            }
            catch
            {
                richTextBoxInformation.AppendText("There was an issue establishing a database connection to the Persistent Staging Area (PSA) Database. These are managed via the TEAM configuration files. The reported database connection string is '" + FormBase.TeamConfigurationSettings.ConnectionStringOmd + "'.\r\n");
            }

            if (_errorCounter > 0)
            {
                richTextBoxInformation.AppendText(_errorMessage.ToString());
            }

        }

        /// <summary>
        /// This is the local updates on the VEDW specific configuration 
        /// </summary>
        private void UpdateVedwConfigurationSettingsOnForm()
        {
            textBoxOutputPath.Text = VedwConfigurationSettings.VedwOutputPath;
            textBoxConfigurationPath.Text = VedwConfigurationSettings.TeamConfigurationPath;

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
                richTextBoxInformation.AppendText(
                    "An issue was encountered updating the Unicode setting on the application - please verify.");
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
                richTextBoxInformation.AppendText(
                    "An issue was encountered updating the Unicode setting on the application - please verify.");
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
                    "An issue was encountered updating the Hash outpu setting on the application - please verify.");
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

            trackBarVersioning.Maximum = selectedVersion;
            trackBarVersioning.TickFrequency = GetVersionCount();
            trackBarVersioning.Value = selectedVersion;

            var versionMajorMinor = GetVersion(selectedVersion);
            var majorVersion = versionMajorMinor.Key;
            var minorVersion = versionMajorMinor.Value;

            labelVersion.Text = majorVersion + "." + minorVersion;
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
            newThread.Start(trackBarVersioning.Value);
        }

        private void BackgroundDoHub(Object obj)
        {
            var versionId = (int)obj;

            if (radiobuttonViews.Checked)
            {
                GenerateHubViews(versionId);
            }
            else if (radiobuttonStoredProc.Checked)
            {

            }
            else if (radioButtonIntoStatement.Checked)
            {
                GenerateHubInsertInto(versionId);
            }
        }

        private void GenerateHubInsertInto(int versionId)
        {
            

            if (checkBoxIfExistsStatement.Checked)
            {
                SetTextHub("The Drop If Exists checkbox has been checked, but this feature is not relevant for this specific operation and will be ignored. \n\r");
            }

            // Determine metadata retrieval connection (dependant on option selected)
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
                    var sourceTableStructure = GetTableStructure(hubTableName, ref conn, versionId, "HUB");

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
                        else
                        {
                            insertIntoStatement.AppendLine("   hub_view.[" + attribute["COLUMN_NAME"] + "],");
                        }
                    }

                    insertIntoStatement.Remove(insertIntoStatement.Length - 3, 3);
                    insertIntoStatement.AppendLine();
                    insertIntoStatement.AppendLine("FROM " + hubTableName + " hub_view");
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
                        GenerateInDatabase(connHstg, insertIntoStatement.ToString());
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
        }

        private void GenerateHubViews(int versionId)
        {
            

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
                    var hubKeyList = GetHubTargetBusinessKeyList(hubTableName, versionId);

                    // Initial SQL
                    hubView.AppendLine("--");
                    hubView.AppendLine("-- Hub View definition for " + hubTableName);
                    hubView.AppendLine("-- Generated at " + DateTime.Now);
                    hubView.AppendLine("--");
                    hubView.AppendLine();
                    hubView.AppendLine("USE [" + TeamConfigurationSettings.PsaDatabaseName + "]");
                    hubView.AppendLine("GO");
                    hubView.AppendLine();
                    hubView.AppendLine("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + hubTableName +"]') AND type in (N'V'))");
                    hubView.AppendLine("DROP VIEW [" + TeamConfigurationSettings.SchemaName + "].[" + hubTableName + "]");
                    hubView.AppendLine("GO");
                    hubView.AppendLine("CREATE VIEW [" + TeamConfigurationSettings.SchemaName + "].[" + hubTableName + "] AS  ");
                    // START OF MAIN QUERY
                    hubView.AppendLine("SELECT hub.*");
                    hubView.AppendLine("FROM(");
                    hubView.AppendLine("SELECT");

                    //Replace hash value with concatenated business key value (experimental)
                    if (!checkBoxDisableHash.Checked)
                    {
                        //Regular Hash
                        hubView.AppendLine("  CONVERT(CHAR(32),HASHBYTES('MD5',");

                        foreach (DataRow hubKey in hubKeyList.Rows)
                        {
                            hubView.AppendLine("    ISNULL(RTRIM(CONVERT(" + stringDataType + "(100),[" + (string)hubKey["COLUMN_NAME"] + "])),'NA')+'|'+");
                        }
                        hubView.Remove(hubView.Length - 3, 3);
                        hubView.AppendLine();
                        hubView.AppendLine("  ),2) AS " + hubSk + ",");
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
                    var queryHubGen = "SELECT * FROM [interface].[INTERFACE_STAGING_HUB_XREF] WHERE HUB_TABLE_NAME = '" + hubTableName + "'";
                    var hubTables = GetDataTable(ref connOmd, queryHubGen);


                    // This loop runs throught the various STG / Hub relationships to create the union statements
                    if (hubTables != null)
                    {
                        var rowcounter = 1;

                        foreach (DataRow hubDetailRow in hubTables.Rows)
                        {
                            var fieldList = new StringBuilder();
                            var compositeKey = new StringBuilder();
                            var sqlSourceStatement = new StringBuilder();
                            var fieldDict = new Dictionary<string, string>();
                            var fieldOrderedList = new List<string>();
                            string firstKey;
                            var sqlStatementForSourceQuery = new StringBuilder();
                            var hubQuerySelect = new StringBuilder();
                            var hubQueryWhere = new StringBuilder();
                            var hubQueryGroupBy = new StringBuilder();
                            
                            var stagingAreaTableName = (string)hubDetailRow["STAGING_AREA_TABLE_NAME"];
                            var psaTableName = TeamConfigurationSettings.PsaTablePrefixValue + stagingAreaTableName.Replace(TeamConfigurationSettings.StgTablePrefixValue, "");
                            var businessKeyDefinition = (string) hubDetailRow["BUSINESS_KEY_DEFINITION"];
                            var filterCriteria = (string)hubDetailRow["FILTER_CRITERIA"];


                            // For every STG / Hub relationship, the business key needs to be defined - starting with the components of the key
                            var componentList = GetBusinessKeyComponentList(stagingAreaTableName, hubTableName, businessKeyDefinition);

                            // Components are key parts, such as a composite key (2 or more components) or regular and concatenated keys (1 component)
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
                                    if (elementList.Rows.Count > 1) // Build a concatinated key if the count of elements is greater than 1 for a component (key part)
                                    {
                                        fieldList.Clear();
                                        fieldDict.Clear();

                                        foreach (DataRow element in elementList.Rows)
                                        {
                                            var elementType = element["BUSINESS_KEY_COMPONENT_ELEMENT_TYPE"].ToString();

                                            if (elementType == "Attribute")
                                            {
                                                fieldList.Append("'" + element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] +"',");

                                                sqlStatementForSourceQuery.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION");
                                                sqlStatementForSourceQuery.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS");
                                                sqlStatementForSourceQuery.AppendLine("WHERE TABLE_NAME= '" + stagingAreaTableName + "'");
                                                sqlStatementForSourceQuery.AppendLine("AND COLUMN_NAME IN (" + fieldList.ToString().Substring(0, fieldList.ToString().Length - 1) + ")");

                                                var elementDataTypes = GetDataTable(ref connStg, sqlStatementForSourceQuery.ToString());

                                                foreach (DataRow attribute in elementDataTypes.Rows)
                                                {
                                                    fieldDict.Add(attribute["COLUMN_NAME"].ToString(), attribute["DATA_TYPE"].ToString());
                                                }
                                            }
                                            else if (elementType == "User Defined Value")
                                            {
                                                fieldList.Append("''" + element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] +"'',");
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
                                                else if ((fieldDict[busKey] == "numeric") ||(fieldDict[busKey] == "integer") || (fieldDict[busKey] == "int") ||(fieldDict[busKey] == "tinyint") ||(fieldDict[busKey] == "decimal"))
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

                                        hubQuerySelect.AppendLine(compositeKey.ToString().Substring(0, compositeKey.ToString().Length - 2) +" AS [" + hubKeyList.Rows[componentId]["COLUMN_NAME"] + "],");
                                        hubQueryWhere.AppendLine(compositeKey.ToString().Substring(0, compositeKey.ToString().Length - 2) +" != '' AND");
                                        hubQueryGroupBy.AppendLine(compositeKey.ToString().Substring(0, compositeKey.ToString().Length - 2) +",");
                                    }
                                    else // Handle a component of a single or composite key 
                                    {
                                        foreach (DataRow element in elementList.Rows) // Only a single element...
                                        {
                                            if (element["BUSINESS_KEY_COMPONENT_ELEMENT_TYPE"].ToString() =="User Defined Value")
                                            {
                                                firstKey = element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"].ToString();
                                                hubQuerySelect.AppendLine("    " + firstKey + " AS [" + hubKeyList.Rows[componentId]["COLUMN_NAME"] + "],");
                                            }
                                            else // It's a normal attribute
                                            {
                                                // We need the data type again
                                                sqlStatementForSourceQuery.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION");
                                                sqlStatementForSourceQuery.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS");
                                                sqlStatementForSourceQuery.AppendLine("WHERE TABLE_NAME= '" +stagingAreaTableName + "'");
                                                sqlStatementForSourceQuery.AppendLine("AND COLUMN_NAME = ('" +element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] + "')");

                                                firstKey = "[" + element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] +"]";

                                                var elementDataTypes = GetDataTable(ref connStg, sqlStatementForSourceQuery.ToString());

                                                foreach (DataRow attribute in elementDataTypes.Rows)
                                                {
                                                    if (attribute["DATA_TYPE"].ToString() == "numeric" || attribute["DATA_TYPE"].ToString() == "int")
                                                    {
                                                        hubQuerySelect.AppendLine("    CAST(" + firstKey + " AS " + stringDataType + "(100)) AS [" + hubKeyList.Rows[componentId]["COLUMN_NAME"] + "],");
                                                    }
                                                    else
                                                    {
                                                        hubQuerySelect.AppendLine("      " + firstKey + " AS [" + hubKeyList.Rows[componentId]["COLUMN_NAME"] + "],");
                                                    }
                                                }

                                                hubQueryWhere.AppendLine(" " + firstKey + " IS NOT NULL AND");
                                                hubQueryGroupBy.AppendLine("    " + firstKey + ",");
                                            }

                                        } // End of element loop (for single element)
                                    } 
                                }
                            } // End of component elements

                            hubQueryWhere.Remove(hubQueryWhere.Length - 6, 6);

                            //Troubleshooting
                            if (hubQuerySelect.ToString() == "")
                            {
                                SetTextLink("Keys missing, please check the metadata for table " + hubTableName + "\r\n");
                            }

                            sqlSourceStatement.AppendLine("  SELECT ");
                            sqlSourceStatement.AppendLine("    " + hubQuerySelect);
                            sqlSourceStatement.Remove(sqlSourceStatement.Length - 3, 3);
                            sqlSourceStatement.AppendLine("    " + TeamConfigurationSettings.RecordSourceAttribute + ",");
                            sqlSourceStatement.AppendLine("    MIN(" + TeamConfigurationSettings.LoadDateTimeAttribute + ") AS " + TeamConfigurationSettings.LoadDateTimeAttribute +"");
                            sqlSourceStatement.AppendLine("  FROM " + psaTableName);
                            sqlSourceStatement.AppendLine("  WHERE");
                            sqlSourceStatement.AppendLine("    " + hubQueryWhere);
                            sqlSourceStatement.Remove(sqlSourceStatement.Length - 1, 1);

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
                    hubView.AppendLine("SELECT '00000000000000000000000000000000',");
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
                        GenerateInDatabase(connPsa, hubView.ToString());
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

        }

        //Retrieve the table structure for a given table, in a given version
        private DataTable GetTableStructure(string targetTableName, ref SqlConnection sqlConnection, int selectedVersion, string tableType)
        {
            var sqlStatementForSourceQuery = new StringBuilder();

            if (!checkBoxIgnoreVersion.Checked && tableType != "PSA")
            {
                sqlStatementForSourceQuery.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION");
                sqlStatementForSourceQuery.AppendLine("FROM MD_VERSION_ATTRIBUTE");
                sqlStatementForSourceQuery.AppendLine("WHERE TABLE_NAME= '" + targetTableName + "'");
                sqlStatementForSourceQuery.AppendLine("AND VERSION_ID= " + selectedVersion + "");
            }

            if (checkBoxIgnoreVersion.Checked || tableType == "PSA")
            {
                sqlStatementForSourceQuery.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION");
                sqlStatementForSourceQuery.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS");
                sqlStatementForSourceQuery.AppendLine("WHERE TABLE_NAME= '" + targetTableName + "'");
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
                SetTextDebug("An error has occurred intepreting the components of the Business Key for "+ hubTableName + " due to connectivity issues (connection string " + conn.ConnectionString + "). The associated message is " + exception.Message);
   
            }

            businessKeyDefinition = businessKeyDefinition.Replace("'", "''");

            var sqlStatementForComponent = new StringBuilder();

            sqlStatementForComponent.AppendLine("SELECT");
            sqlStatementForComponent.AppendLine("  [STAGING_AREA_TABLE_ID]");
            sqlStatementForComponent.AppendLine(" ,[STAGING_AREA_TABLE_NAME]"); 
            sqlStatementForComponent.AppendLine(" ,[STAGING_AREA_SCHEMA_NAME]");
            sqlStatementForComponent.AppendLine(" ,[HUB_TABLE_ID]");
            sqlStatementForComponent.AppendLine(" ,[HUB_TABLE_NAME]");
            sqlStatementForComponent.AppendLine(" ,[BUSINESS_KEY_DEFINITION]");
            sqlStatementForComponent.AppendLine(" ,[BUSINESS_KEY_COMPONENT_ID]");
            sqlStatementForComponent.AppendLine(" ,[BUSINESS_KEY_COMPONENT_ORDER]");
            sqlStatementForComponent.AppendLine(" ,[BUSINESS_KEY_COMPONENT_VALUE]");
            sqlStatementForComponent.AppendLine("FROM [interface].[INTERFACE_BUSINESS_KEY_COMPONENT]");
            sqlStatementForComponent.AppendLine("WHERE STAGING_AREA_TABLE_NAME = '" + stagingTableName + "'");
            sqlStatementForComponent.AppendLine("  AND HUB_TABLE_NAME = '" + hubTableName + "'");
            sqlStatementForComponent.AppendLine("  AND BUSINESS_KEY_DEFINITION = '" + businessKeyDefinition + "'");

            var componentList = GetDataTable(ref conn, sqlStatementForComponent.ToString());

            if (componentList == null)
            {
                SetTextDebug("An error has occurred interpreting the Hub Business Key (components) in the model for " + hubTableName + ". The Business Key was not found when querying the underlying metadata.");
            }

            return componentList;
        }

        //  Executing a SQL object against the databasa (SQL Server SMO API)
        public void GenerateInDatabase(SqlConnection sqlConnection, string viewStatement)
        {
            using (var connection = sqlConnection)
            {
                var server = new Server(new ServerConnection(connection));
                try
                {
                    server.ConnectionContext.ExecuteNonQuery(viewStatement);
                    SetTextDebug("The statement was executed succesfully.\r\n");
                }
                catch (Exception exception)
                {
                    SetTextDebug("Issues occurred executing the SQL statement.\r\n");
                    SetTextDebug(@"SQL error: " + exception.Message + "\r\n\r\n");
                 // SetTextDebug(@"The executed query was: " + viewStatement);
                }
            }               
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
            newThread.Start(trackBarVersioning.Value);
        }

        private void BackgroundDoSat(Object obj)
        {
            var versionId = (int)obj;

            if (radiobuttonViews.Checked)
            {
                GenerateSatViews(versionId);
            }
            else if (radiobuttonStoredProc.Checked)
            {
                MessageBox.Show("This feature is yet to be implemented.", "Upcoming!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (radioButtonIntoStatement.Checked)
            {
                GenerateSatInsertInto(versionId);
            }
        }

        // Generate the Insert Into statement for the Satellites
        private void GenerateSatInsertInto(int versionId)
        {
            

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

                    var queryTableArray = "SELECT * FROM interface.INTERFACE_STAGING_SATELLITE_XREF " +
                                          "WHERE SATELLITE_TYPE = 'Normal' " +
                                          " AND SATELLITE_TABLE_NAME = '" + targetTableName + "'";

                    if (checkBoxIfExistsStatement.Checked)
                    {
                        SetTextSat("The Drop If Exists checkbox has been checked, but this feature is not relevant for this specific operation and will be ignored. \n\r");
                    }

                    var tables = GetDataTable(ref connOmd, queryTableArray);

                    foreach (DataRow row in tables.Rows)
                    {
                        var hubSk = row["HUB_TABLE_NAME"].ToString().Substring(4) + "_"+TeamConfigurationSettings.DwhKeyIdentifier;

                        // Build the main attribute list of the Satellite table for selection
                        var sourceTableStructure = GetTableStructure(targetTableName, ref conn, versionId, "SAT");

                        // Query to detect multi-active attributes
                        var multiActiveAttributes = GetMultiActiveAttributes((int)row["SATELLITE_TABLE_ID"]);

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
                            else
                            {
                                insertIntoStatement.Append("   sat_view.[" + sourceAttribute + "],");
                                insertIntoStatement.AppendLine();
                            }
                        }

                        insertIntoStatement.Remove(insertIntoStatement.Length - 3, 3);
                        insertIntoStatement.AppendLine();
                        insertIntoStatement.AppendLine("FROM " + targetTableName + " sat_view");
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
                            insertIntoStatement.AppendLine("AND sat_view.[" + (string)attribute["ATTRIBUTE_NAME_TO"] + "] = sat_table.["+(string)attribute["ATTRIBUTE_NAME_TO"]+"]");
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
                            GenerateInDatabase(connHstg, insertIntoStatement.ToString());
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
        }

        private void GenerateSatViews(int versionId) //  Generate Satellite Views
        {
            

            if (checkedListBoxSatMetadata.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= checkedListBoxSatMetadata.CheckedItems.Count - 1; x++)
                {
                    var targetTableName = checkedListBoxSatMetadata.CheckedItems[x].ToString();

                    var stringDataType = checkBoxUnicode.Checked ? "NVARCHAR" : "VARCHAR";

                    var sqlStatementForTablesToImport = "SELECT * FROM interface.INTERFACE_STAGING_SATELLITE_XREF " +
                                                        "WHERE SATELLITE_TYPE = 'Normal' " +
                                                        " AND SATELLITE_TABLE_NAME = '" + targetTableName + "'";
                  
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
                        var targetTableId = (int) row["SATELLITE_TABLE_ID"];
                        var stagingAreaTableId = (int) row["STAGING_AREA_TABLE_ID"];
                        var stagingAreaTableName = (string) row["STAGING_AREA_TABLE_NAME"];
                        var psaTableName = TeamConfigurationSettings.PsaTablePrefixValue + stagingAreaTableName.Replace(TeamConfigurationSettings.StgTablePrefixValue, "");
                        var hubTableName = (string) row["HUB_TABLE_NAME"];
                        var filterCriteria = (string)row["FILTER_CRITERIA"];
                        var businessKeyDefinition = (string)row["BUSINESS_KEY_DEFINITION"];

                        var hubSk = hubTableName.Substring(4) + "_" + TeamConfigurationSettings.DwhKeyIdentifier;

                        // The name of the Hub hash key as it may be available in the Staging Area (if added here)
                        var stgHubSk = TeamConfigurationSettings.DwhKeyIdentifier + "_" + hubTableName;
                        var fieldList = new StringBuilder();
                        var compositeKey = new StringBuilder();

                        var fieldDict = new Dictionary<string, string>();
                        var fieldOrderedList = new List<string>();
                        string firstKey;
                        var sqlStatementForSourceQuery = new StringBuilder();
                        var hubQuerySelect = new StringBuilder();
                        var hubQueryWhere = new StringBuilder();
                        var hubQueryGroupBy = new StringBuilder();

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
                        satView.AppendLine("IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" +targetTableName + "]') AND type in (N'V'))");
                        satView.AppendLine("DROP VIEW [" + TeamConfigurationSettings.SchemaName + "].[" + targetTableName + "]");
                        satView.AppendLine("go");
                        satView.AppendLine("CREATE VIEW [" + TeamConfigurationSettings.SchemaName + "].[" + targetTableName + "] AS  ");

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
                            sqlStatementForSourceAttribute.AppendLine("  AND SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" +localkeyLength + "," + localkeySubstring + ")!='_" +TeamConfigurationSettings.DwhKeyIdentifier + "'");
                            sqlStatementForSourceAttribute.AppendLine("  AND COLUMN_NAME NOT IN ('" +TeamConfigurationSettings.RecordSourceAttribute + "','" +TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" +TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" +TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute +"','" +TeamConfigurationSettings.EtlProcessAttribute + "','" + TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" +TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" +TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute +"','" +TeamConfigurationSettings.LoadDateTimeAttribute + "')");
                            sqlStatementForSourceAttribute.AppendLine("ORDER BY ORDINAL_POSITION");
                        }
                        else
                        {
                            sqlStatementForSourceAttribute.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION, ORDINAL_POSITION");
                            sqlStatementForSourceAttribute.AppendLine("FROM MD_VERSION_ATTRIBUTE");
                            sqlStatementForSourceAttribute.AppendLine("WHERE VERSION_ID = " + versionId);
                            sqlStatementForSourceAttribute.AppendLine("  AND TABLE_NAME= '" + psaTableName + "'");
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
                        var hubKeyList = GetHubTargetBusinessKeyList(hubTableName, versionId);

                        // For every STG / Hub relationship, the business key needs to be defined - starting with the components of the key
                        var componentList = GetBusinessKeyComponentList(stagingAreaTableName, hubTableName, businessKeyDefinition);

                        // Components are key parts, such as a composite key (2 or more components) or regular and concatenated keys (1 component)
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
                                if (elementList.Rows.Count > 1) // Build a concatinated key if the count of elements is greater than 1 for a component (key part)
                                {
                                    fieldList.Clear();
                                    fieldDict.Clear();

                                    foreach (DataRow element in elementList.Rows)
                                    {
                                        var elementType = element["BUSINESS_KEY_COMPONENT_ELEMENT_TYPE"].ToString();

                                        if (elementType == "Attribute")
                                        {
                                            fieldList.Append("'" + element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] + "',");

                                            sqlStatementForSourceQuery.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION");
                                            sqlStatementForSourceQuery.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS");
                                            sqlStatementForSourceQuery.AppendLine("WHERE TABLE_NAME= '" + stagingAreaTableName + "'");
                                            sqlStatementForSourceQuery.AppendLine("AND COLUMN_NAME IN (" + fieldList.ToString().Substring(0, fieldList.ToString().Length - 1) + ")");

                                            var elementDataTypes = GetDataTable(ref connStg, sqlStatementForSourceQuery.ToString());

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

                                    hubQuerySelect.AppendLine(compositeKey.ToString().Substring(0, compositeKey.ToString().Length - 2) + " AS [" + hubKeyList.Rows[componentId]["COLUMN_NAME"] + "],");
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
                                            hubQuerySelect.AppendLine("    " + firstKey + " AS [" + hubKeyList.Rows[componentId]["COLUMN_NAME"] + "],");
                                        }
                                        else // It's a normal attribute
                                        {
                                            // We need the data type again
                                            sqlStatementForSourceQuery.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION");
                                            sqlStatementForSourceQuery.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS");
                                            sqlStatementForSourceQuery.AppendLine("WHERE TABLE_NAME= '" + stagingAreaTableName + "'");
                                            sqlStatementForSourceQuery.AppendLine("AND COLUMN_NAME = ('" + element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] + "')");

                                            firstKey = "[" + element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] + "]";

                                            var elementDataTypes = GetDataTable(ref connStg, sqlStatementForSourceQuery.ToString());

                                            foreach (DataRow attribute in elementDataTypes.Rows)
                                            {
                                                if (attribute["DATA_TYPE"].ToString() == "numeric" || attribute["DATA_TYPE"].ToString() == "int")
                                                {
                                                    hubQuerySelect.AppendLine("    CAST(" + firstKey + " AS " + stringDataType + "(100)) AS [" + hubKeyList.Rows[componentId]["COLUMN_NAME"] + "],");
                                                }
                                                else
                                                {
                                                    hubQuerySelect.AppendLine("      " + firstKey + " AS [" + hubKeyList.Rows[componentId]["COLUMN_NAME"] + "],");
                                                }
                                            }

                                            hubQueryWhere.AppendLine(" " + firstKey + " IS NOT NULL AND");
                                            hubQueryGroupBy.AppendLine("    " + firstKey + ",");
                                        }

                                    } // End of element loop (for single element)
                                }
                            }
                        } // End of component elements

                        hubQueryWhere.Remove(hubQueryWhere.Length - 6, 6);

                        //Troubleshooting
                        if (hubQuerySelect.ToString() == "")
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
                                satView.AppendLine("   CONVERT(CHAR(32),HASHBYTES('MD5',");

                                foreach (DataRow attribute in hubKeyList.Rows)
                                {
                                    satView.AppendLine("     ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + attribute["COLUMN_NAME"] + ")),'NA')+'|'+");
                                }

                                satView.Remove(satView.Length - 3, 3);
                                satView.AppendLine();
                                satView.AppendLine("   ),2) AS " + hubSk + ",");
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
                            multiActiveAttributeFromName = (string) attribute["ATTRIBUTE_NAME_FROM"];
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
                            multiActiveAttributeFromName = (string) attribute["ATTRIBUTE_NAME_FROM"];
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
                        satView.AppendLine("   CONVERT(CHAR(32),HASHBYTES('MD5',");
                        satView.AppendLine("      ISNULL(RTRIM(CONVERT("+ stringDataType + "(100)," + TeamConfigurationSettings.ChangeDataCaptureAttribute + ")),'NA')+'|'+");

                        foreach (DataRow attribute in sourceStructure.Rows)
                        {
                            var localAttribute = attribute["ATTRIBUTE_NAME_FROM"];
                            var foundBusinessKeyAttribute = componentElementList.Rows.Find(localAttribute);

                            if (foundBusinessKeyAttribute == null)
                            {
                                satView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + localAttribute +")),'NA')+'|'+");
                            }
                        }
                        satView.Remove(satView.Length - 3, 3);
                        satView.AppendLine();
                        satView.AppendLine("   ),2) AS " + TeamConfigurationSettings.RecordChecksumAttribute + ",");

                        // Regular attributes
                        foreach (DataRow attribute in sourceStructure.Rows)
                        {
                            var localAttribute = attribute["ATTRIBUTE_NAME_FROM"];
                            var localAttributeTarget = attribute["ATTRIBUTE_NAME_TO"];
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
                            multiActiveAttributeFromName = (string) attribute["ATTRIBUTE_NAME_FROM"];
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
                            multiActiveAttributeFromName = (string) attribute["ATTRIBUTE_NAME_FROM"];
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
                            var localAttribute = attribute["ATTRIBUTE_NAME_FROM"];
                            var foundBusinessKeyAttribute = componentElementList.Rows.Find(localAttribute);

                            if (foundBusinessKeyAttribute == null)
                            {
                                satView.AppendLine("         [" + localAttribute + "],");
                            }
                        }

                        // Record condensing
                        satView.AppendLine("         COMBINED_VALUE,");
                        satView.AppendLine("         CASE ");
                        satView.AppendLine("           WHEN LAG(COMBINED_VALUE,1,'N/A') OVER (PARTITION BY ");


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
                            satView.AppendLine("             [" + (string)attribute["ATTRIBUTE_NAME_FROM"] + "],");
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
                            satView.AppendLine("             [" + (string)attribute["ATTRIBUTE_NAME_FROM"] + "],");
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
                            satView.AppendLine("             [" + (string)attribute["ATTRIBUTE_NAME_FROM"] + "],");
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
                            var localAttribute = attribute["ATTRIBUTE_NAME_FROM"];
                            var foundBusinessKeyAttribute = componentElementList.Rows.Find(localAttribute);

                            if (foundBusinessKeyAttribute == null)
                            {
                                satView.AppendLine("          [" + localAttribute + "],");
                            }
                        }

                        // Hash needs to be calculated for Combined Value
                        satView.AppendLine("         CONVERT(CHAR(32),HASHBYTES('MD5',");

                        foreach (DataRow attribute in sourceStructure.Rows)
                        {
                            satView.AppendLine("             ISNULL(RTRIM(CONVERT(" + stringDataType + "(100),[" + attribute["ATTRIBUTE_NAME_FROM"] + "])),'NA')+'|'+");
                        }

                        satView.Remove(satView.Length - 3, 3);
                        satView.AppendLine();
                        satView.AppendLine("         ),2) AS COMBINED_VALUE");
                        satView.AppendLine("        FROM " + psaTableName);

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
                                satView.AppendLine("          "+ attribute["ATTRIBUTE_NAME_FROM"]+ ",");
                            }


                            // Adding regular attributes as NULLs, skipping the key and any multi-active attributes
                            foreach (DataRow attribute in sourceStructure.Rows)
                            {
                                var localAttribute = attribute["ATTRIBUTE_NAME_FROM"]; // Get the key
                                var foundBusinessKeyAttribute = componentElementList.Rows.Find(localAttribute); // Get the multi-active attribute
                                var foundMultiActiveAttribute = multiActiveAttributes.Rows.Find(localAttribute);

                                if (foundBusinessKeyAttribute == null && foundMultiActiveAttribute == null)
                                {
                                    satView.AppendLine("          NULL AS " + localAttribute + ",");
                                }
                            }

                            satView.AppendLine("          'N/A' AS COMBINED_VALUE");
                            satView.AppendLine("        FROM " + psaTableName);
                            // End of zero record
                        }

                        satView.AppendLine("   ) sub");
                        satView.AppendLine(") combined_value");

                        satView.AppendLine("WHERE ");
                        satView.AppendLine("  (VALUE_CHANGE_INDICATOR ='Different' and [" + TeamConfigurationSettings.ChangeDataCaptureAttribute + "] in ('Insert', 'Change')) ");
                        satView.AppendLine("  OR");
                        satView.AppendLine("  (CDC_CHANGE_INDICATOR = 'Different' and TIME_CHANGE_INDICATOR = 'Different')");

                        // Zero record insert
                        satView.AppendLine("UNION");
                        satView.AppendLine("SELECT '00000000000000000000000000000000',");
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
                        satView.AppendLine("'00000000000000000000000000000000',"); //Full Row Hash

                        foreach (DataRow attribute in multiActiveAttributes.Rows)
                        {
                            // Requires handling of data types
                            if ( attribute["ATTRIBUTE_NAME_TO"].ToString().Contains("DATE"))
                            {
                                satView.AppendLine("CAST('1900-01-01' AS DATE),");
                            }
                            else
                            {
                                satView.AppendLine("'00000000000000000000000000000000',");
                            }
                        }

                        foreach (DataRow attribute in sourceStructure.Rows)
                        {
                            var localAttribute = attribute["ATTRIBUTE_NAME_FROM"];
                            var foundBusinessKeyAttribute = componentElementList.Rows.Find(localAttribute);
                            var foundMultiActiveAttribute = multiActiveAttributes.Rows.Find(localAttribute);

                            if (foundBusinessKeyAttribute == null && foundMultiActiveAttribute == null)
                            {
                                satView.AppendLine("NULL,");
                            }
                        }
                        satView.AppendLine("1"); // Row Nr

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
                            GenerateInDatabase(connHstg, satView.ToString());
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
            newThread.Start(trackBarVersioning.Value);
        }

        private void BackgroundDoLink(Object obj)
        {
            var versionId = (int)obj;

            if (radiobuttonViews.Checked)
            {
                GenerateLinkViews(versionId);
            }
            else if (radiobuttonStoredProc.Checked)
            {
                MessageBox.Show("This feature is yet to be implemented.", "Upcoming!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (radioButtonIntoStatement.Checked)
            {
                GenerateLinkInsertInto(versionId);
            }
        }

        private void GenerateLinkInsertInto(int versionId)
        {
            

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
                    var sourceTableStructure = GetTableStructure(targetTableName, ref conn, versionId, "LNK");

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
                        else
                        {
                            insertIntoStatement.Append("   link_view.[" + sourceAttribute + "],");
                            insertIntoStatement.AppendLine();
                        }
                    }

                    insertIntoStatement.Remove(insertIntoStatement.Length - 3, 3);
                    insertIntoStatement.AppendLine();
                    insertIntoStatement.AppendLine("FROM " + targetTableName + " link_view");
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
                        GenerateInDatabase(connHstg, insertIntoStatement.ToString());
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
        }

        private void GenerateLinkViews(int versionId)
        {
            

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
                    var hubBusinessKeyList = GetHubTablesForLink(linkTableName, versionId);
                    var hubFullBusinessKeyList = GetAllHubTablesForLink(linkTableName, versionId);
                    

                    // Get the target business key names as they are in the Link (may be aliased compared to the Hub)
                    var linkBusinessKeyList = GetLinkTargetBusinessKeyList(linkTableName, versionId);

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
                    linkView.AppendLine("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" +linkTableName + "]') AND type in (N'V'))");
                    linkView.AppendLine("DROP VIEW [" + TeamConfigurationSettings.SchemaName + "].[" + linkTableName + "]");
                    linkView.AppendLine("GO");
                    linkView.AppendLine();
                    linkView.AppendLine("CREATE VIEW [" + TeamConfigurationSettings.SchemaName + "].[" + linkTableName +"] AS  ");
                    linkView.AppendLine("SELECT link.*");
                    linkView.AppendLine("FROM");
                    linkView.AppendLine("(");
                                                       
                    linkView.AppendLine("SELECT");

                    if (!checkBoxDisableHash.Checked)
                    {
                        // Create Link Hash Key
                        linkView.AppendLine("  CONVERT(CHAR(32),HASHBYTES('MD5',");

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
                        foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                        {
                            linkView.AppendLine("    ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + (string)attribute["ATTRIBUTE_NAME_TO"] + ")),'NA')+'|'+");
                        }

                        linkView.Remove(linkView.Length - 3, 3);
                        linkView.AppendLine();
                        linkView.AppendLine("  ),2) AS " + linkSk + ",");
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

                        foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                        {
                            linkView.Append("    ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + (string)attribute["ATTRIBUTE_NAME_TO"] + ")),'NA')+'|'+");
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
                            linkView.AppendLine("  CONVERT(CHAR(32),HASHBYTES('MD5',");
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
                            linkView.AppendLine("  ),2) AS " + hubTargetBusinessKeyName + ",");
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
                    foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                    {
                        linkView.AppendLine("  " + (string)attribute["ATTRIBUTE_NAME_TO"] + ",");
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
                    foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                    {
                        linkView.AppendLine("      [" + (string)attribute["ATTRIBUTE_NAME_TO"] + "],");
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
                    queryLinkStagingTables.AppendLine(" [STAGING_AREA_TABLE_ID]");
                    queryLinkStagingTables.AppendLine(",[STAGING_AREA_TABLE_NAME]");
                    queryLinkStagingTables.AppendLine(",[LINK_TABLE_ID]");
                    queryLinkStagingTables.AppendLine(",[LINK_TABLE_NAME]");
                    queryLinkStagingTables.AppendLine(",[FILTER_CRITERIA]");
                    queryLinkStagingTables.AppendLine("FROM [interface].[INTERFACE_STAGING_LINK_XREF]");
                    queryLinkStagingTables.AppendLine("WHERE LINK_TABLE_NAME = '" + linkTableName + "'");

                    var linkStagingTables = GetDataTable(ref connOmd, queryLinkStagingTables.ToString());

                    if (linkStagingTables != null)
                    {
                        var rowcounter = 1;

                        foreach (DataRow linkDetailRow in linkStagingTables.Rows)
                        {
                            var sqlStatementForComponent = new StringBuilder();
                            var sqlSourceStatement = new StringBuilder();
                            var sqlStatementForSourceQuery = new StringBuilder();

                            var stagingAreaTableName = (string) linkDetailRow["STAGING_AREA_TABLE_NAME"];
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

                                var fieldList = new StringBuilder();
                                var compositeKey = new StringBuilder();
                                var fieldDict = new Dictionary<string, string>();
                                var fieldOrderedList = new List<string>();

                                var hubTableName = (string)hubDetailRow["HUB_TABLE_NAME"];
                                var businessKeyDefinition = (string) hubDetailRow["BUSINESS_KEY_DEFINITION"];

                                // Getting the target Business Key name(s) for the Hub. This may be composite, so more than 1 which is matched to the source definition
                                var hubKeyList = GetHubTargetBusinessKeyList(hubTableName, versionId);

                                // Retrieving the top level component to evaluate composite, concat or pivot 
                                var componentList = GetBusinessKeyComponentList(stagingAreaTableName, hubTableName, businessKeyDefinition);

                                // Components are key parts, such as a composite key (2 or more components) or regular and concatenated keys (1 component)
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
                                        if (elementList.Rows.Count > 1) // Build a concatinated key if the count of elements is greater than 1 for a component (key part)
                                        {
                                            fieldList.Clear();
                                            fieldDict.Clear();

                                            foreach (DataRow element in elementList.Rows)
                                            {
                                                var elementType = element["BUSINESS_KEY_COMPONENT_ELEMENT_TYPE"].ToString();

                                                if (elementType == "Attribute")
                                                {
                                                    fieldList.Append("'" + element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] + "',");

                                                    sqlStatementForSourceQuery.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION");
                                                    sqlStatementForSourceQuery.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS");
                                                    sqlStatementForSourceQuery.AppendLine("WHERE TABLE_NAME= '" + stagingAreaTableName + "'");
                                                    sqlStatementForSourceQuery.AppendLine("AND COLUMN_NAME IN (" + fieldList.ToString().Substring(0, fieldList.ToString().Length - 1) + ")");

                                                    var elementDataTypes = GetDataTable(ref connStg, sqlStatementForSourceQuery.ToString());

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
                                                string firstKey;
                                                if (element["BUSINESS_KEY_COMPONENT_ELEMENT_TYPE"].ToString() == "User Defined Value")
                                                {
                                                    firstKey = element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"].ToString();
                                                    hubQuerySelect.AppendLine("    " + firstKey + " AS [" + hubKeyList.Rows[componentId]["COLUMN_NAME"] + groupCounter + "],");
                                                }
                                                else // It's a normal attribute
                                                {
                                                    // We need the data type again
                                                    sqlStatementForSourceQuery.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION");
                                                    sqlStatementForSourceQuery.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS");
                                                    sqlStatementForSourceQuery.AppendLine("WHERE TABLE_NAME= '" + stagingAreaTableName + "'");
                                                    sqlStatementForSourceQuery.AppendLine("AND COLUMN_NAME = ('" + element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] + "')");

                                                    firstKey = "[" + element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] + "]";

                                                    var elementDataTypes = GetDataTable(ref connStg, sqlStatementForSourceQuery.ToString());

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

                                groupCounter++;
                            } // End of business key creation

                            hubQueryWhere.Remove(hubQueryWhere.Length - 6, 6);

                            //Troubleshooting
                            if (hubQuerySelect.ToString() == "")
                            {
                                SetTextLink("Keys missing, please check the metadata for table " + currentTableName + "\r\n");
                            }

                            // Initiating select statement for subquery
                            sqlSourceStatement.AppendLine("  SELECT ");
                            sqlSourceStatement.AppendLine("   " + hubQuerySelect);
                            sqlSourceStatement.Remove(sqlSourceStatement.Length - 3, 3);
                            sqlSourceStatement.AppendLine("    " + TeamConfigurationSettings.RecordSourceAttribute + ",");

                            foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                            {
                                sqlSourceStatement.AppendLine("    [" + (string)attribute["ATTRIBUTE_NAME_FROM"] + "] AS [" + (string)attribute["ATTRIBUTE_NAME_TO"] + "],");
                            }
                            sqlSourceStatement.AppendLine("    MIN(" + TeamConfigurationSettings.LoadDateTimeAttribute + ") AS " + TeamConfigurationSettings.LoadDateTimeAttribute +"");
                            sqlSourceStatement.AppendLine("  FROM [" + TeamConfigurationSettings.PsaDatabaseName + "].[" + TeamConfigurationSettings.SchemaName + "].[" + currentTableName + "]");
                            sqlSourceStatement.AppendLine("  WHERE");
                            sqlSourceStatement.AppendLine(" " + hubQueryWhere);
                            sqlSourceStatement.Remove(sqlSourceStatement.Length - 1, 1);
                            if (string.IsNullOrEmpty(filterCriteria))
                            {
                            }
                            else
                            {
                                sqlSourceStatement.AppendLine("    AND " + filterCriteria);
                            }
                            sqlSourceStatement.AppendLine("  GROUP BY ");
                            sqlSourceStatement.AppendLine("" + hubQueryGroupBy);
                            sqlSourceStatement.Remove(sqlSourceStatement.Length - 3, 3);

                            // Add degenerate attributes
                            foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                            {
                                sqlSourceStatement.AppendLine("    [" + (string)attribute["ATTRIBUTE_NAME_FROM"] + "],");
                            }
                            sqlSourceStatement.AppendLine("    " + TeamConfigurationSettings.RecordSourceAttribute);

                            linkView.AppendLine(sqlSourceStatement.ToString());
                            linkView.Remove(linkView.Length - 3, 3);

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

                        foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                        {
                            linkView.AppendLine("  [" + (string)attribute["ATTRIBUTE_NAME_TO"] + "],");
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
                            GenerateInDatabase(connHstg, linkView.ToString());
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
        }

        private DataTable GetBusinessKeyElementsBase (string stagingAreaTableName, string hubTableName, string businessKeyDefinition)
        {
            

            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            var sqlStatementForSourceBusinessKey = new StringBuilder();

            businessKeyDefinition = businessKeyDefinition.Replace("'", "''");

            sqlStatementForSourceBusinessKey.AppendLine("SELECT * FROM interface.INTERFACE_BUSINESS_KEY_COMPONENT_PART");
            sqlStatementForSourceBusinessKey.AppendLine("WHERE STAGING_AREA_TABLE_NAME= '" + stagingAreaTableName + "'");
            sqlStatementForSourceBusinessKey.AppendLine("  AND HUB_TABLE_NAME= '" + hubTableName + "'");
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
            sqlStatementForSourceBusinessKey.AppendLine("WHERE STAGING_AREA_TABLE_NAME= '" + stagingAreaTableName + "'");
            sqlStatementForSourceBusinessKey.AppendLine("  AND HUB_TABLE_NAME= '" + hubTableName + "'");
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
                SetTextDebug("An error has occurred selecting Link tabl degenerate attributes: " + conn.ConnectionString + "). The associated message is " + exception.Message);

            }

            // Query to select degenerate attributes
            var degenerateLinkAttributeQuery = new StringBuilder();

            degenerateLinkAttributeQuery.AppendLine("SELECT ");
            degenerateLinkAttributeQuery.AppendLine("  c.ATTRIBUTE_NAME AS ATTRIBUTE_NAME_FROM,");
            degenerateLinkAttributeQuery.AppendLine("  b.ATTRIBUTE_NAME AS ATTRIBUTE_NAME_TO");
            degenerateLinkAttributeQuery.AppendLine("FROM MD_STG_LINK_ATT_XREF a");
            degenerateLinkAttributeQuery.AppendLine("JOIN MD_ATT b ON a.ATTRIBUTE_ID_TO=b.ATTRIBUTE_ID");
            degenerateLinkAttributeQuery.AppendLine("JOIN MD_ATT c ON a.ATTRIBUTE_ID_FROM=c.ATTRIBUTE_ID");
            degenerateLinkAttributeQuery.AppendLine("JOIN MD_LINK d ON a.LINK_TABLE_ID=d.LINK_TABLE_ID");
            degenerateLinkAttributeQuery.AppendLine("WHERE d.LINK_TABLE_NAME = '" + targetTableName + "'");

            var degenerateLinkAttributes = GetDataTable(ref conn, degenerateLinkAttributeQuery.ToString());
            return degenerateLinkAttributes;
        }

        private DataTable GetHubLinkCombination(string stagingAreaTableName, string linkTableName)
        {
            

            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };

            // Get the Hubs for each Link/STG combination - both need to be represented in the query
            var queryHubGen = "SELECT * FROM [interface].[INTERFACE_HUB_LINK_XREF] " +
                              "WHERE STAGING_AREA_TABLE_NAME = '" + stagingAreaTableName + "'" +
                              "  AND LINK_TABLE_NAME = '" + linkTableName + "'" +
                              " ORDER BY HUB_ORDER, BUSINESS_KEY_DEFINITION";

            var hubTables = GetDataTable(ref connOmd, queryHubGen);
            return hubTables;
        }


        public DataTable GetHubTargetBusinessKeyList(string hubTableName, int versionId)
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
                sqlStatementForHubBusinessKeys.AppendLine("  AND TABLE_NAME= '" + hubTableName + "'");
                sqlStatementForHubBusinessKeys.AppendLine("  AND COLUMN_NAME NOT IN ('" + TeamConfigurationSettings.RecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" +
                                                          TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "','" + TeamConfigurationSettings.EtlProcessAttribute + "','" + TeamConfigurationSettings.LoadDateTimeAttribute + "')");
            }
            else
            {
                //Ignore version is not checked, so versioning is used - meaning the business key metadata is sourced from the version history metadata.
                sqlStatementForHubBusinessKeys.AppendLine("SELECT COLUMN_NAME");
                sqlStatementForHubBusinessKeys.AppendLine("FROM MD_VERSION_ATTRIBUTE");
                sqlStatementForHubBusinessKeys.AppendLine("WHERE SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" + localkeyLength + "," + localkeySubstring + ")!='_" + TeamConfigurationSettings.DwhKeyIdentifier + "'");
                sqlStatementForHubBusinessKeys.AppendLine("  AND TABLE_NAME= '" + hubTableName + "'");
                sqlStatementForHubBusinessKeys.AppendLine("  AND COLUMN_NAME NOT IN ('" + TeamConfigurationSettings.RecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" + TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "','" +
                                                          TeamConfigurationSettings.EtlProcessAttribute + "','" + TeamConfigurationSettings.LoadDateTimeAttribute + "')");
                sqlStatementForHubBusinessKeys.AppendLine("  AND VERSION_ID = " + versionId + "");
            }


            var hubKeyList = GetDataTable(ref conn, sqlStatementForHubBusinessKeys.ToString());

            if (hubKeyList == null)
            {
                SetTextDebug("An error has occurred defining the Hub Business Key in the model for " + hubTableName + ". The Business Key was not found when querying the underlying metadata. This can be either that the attribute is missing in the metadata or in the table (depending if versioning is used). If the 'ignore versioning' option is checked, then the metadata will be retrieved directly from the data dictionary. Otherwise the metadata needs to be available in the repository (manage model metadata).");
            }

            return hubKeyList;
        }

        public LinkedList<string[]> GetLinkTargetBusinessKeyList(string linkTableName, int versionId)
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
                //sqlStatementForLinkBusinessKeys.AppendLine("WHERE SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" + localkeyLength + "," + localkeySubstring + ")!='_" + ConfigurationSettings.DwhKeyIdentifier + "'");
                sqlStatementForLinkBusinessKeys.AppendLine("WHERE TABLE_NAME= '" + linkTableName + "'");
                sqlStatementForLinkBusinessKeys.AppendLine("  AND COLUMN_NAME NOT IN ('" + TeamConfigurationSettings.RecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" +
                                                          TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "','" + TeamConfigurationSettings.EtlProcessAttribute + "','" + TeamConfigurationSettings.LoadDateTimeAttribute + "')");
                sqlStatementForLinkBusinessKeys.AppendLine("ORDER BY ORDINAL_POSITION");
            }
            else
            {
                //Ignore version is not checked, so versioning is used - meaning the business key metadata is sourced from the version history metadata.
                sqlStatementForLinkBusinessKeys.AppendLine("SELECT COLUMN_NAME");
                sqlStatementForLinkBusinessKeys.AppendLine("FROM MD_VERSION_ATTRIBUTE");
                //sqlStatementForLinkBusinessKeys.AppendLine("WHERE SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" + localkeyLength + "," + localkeySubstring + ")!='_" + ConfigurationSettings.DwhKeyIdentifier + "'");
                sqlStatementForLinkBusinessKeys.AppendLine("WHERE TABLE_NAME= '" + linkTableName + "'");
                sqlStatementForLinkBusinessKeys.AppendLine("  AND COLUMN_NAME NOT IN ('" + TeamConfigurationSettings.RecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" + TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "','" +
                                                          TeamConfigurationSettings.EtlProcessAttribute + "','" + TeamConfigurationSettings.LoadDateTimeAttribute + "')");
                sqlStatementForLinkBusinessKeys.AppendLine("  AND VERSION_ID = " + versionId + "");
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

        private LinkedList<string[]> GetHubTablesForLink(string targetTableName, int versionId)
        {
            

            // First, get the associated Hub tables for the Link
            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            var queryLinkHubTables = new StringBuilder();
            int groupCounter = 1;

            queryLinkHubTables.AppendLine("SELECT DISTINCT ");
            queryLinkHubTables.AppendLine(" [LINK_TABLE_ID]");
            queryLinkHubTables.AppendLine(",[LINK_TABLE_NAME]");
            // queryLinkHubTables.AppendLine(",[STAGING_AREA_TABLE_ID]");
            // queryLinkHubTables.AppendLine(",[STAGING_AREA_TABLE_NAME]");
            // queryLinkHubTables.AppendLine(",[STAGING_AREA_SCHEMA_NAME]");
            queryLinkHubTables.AppendLine(",[HUB_TABLE_ID]");
            queryLinkHubTables.AppendLine(",[HUB_TABLE_NAME]");
            queryLinkHubTables.AppendLine(",[HUB_ORDER]");
            //queryLinkHubTables.AppendLine(",[BUSINESS_KEY_DEFINITION]");
            queryLinkHubTables.AppendLine("FROM [interface].[INTERFACE_HUB_LINK_XREF]");
            queryLinkHubTables.AppendLine("WHERE LINK_TABLE_NAME = '" + targetTableName + "'");
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
                    queryHubBusinessKeys.AppendLine("	 GROUP BY TABLE_NAME");
                    queryHubBusinessKeys.AppendLine("	) b");
                    queryHubBusinessKeys.AppendLine("ON a.TABLE_NAME=b.TABLE_NAME");
                    queryHubBusinessKeys.AppendLine("WHERE SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" + localkeyLength +"," +localkeySubstring + ")!='_" + TeamConfigurationSettings.DwhKeyIdentifier + "'");queryHubBusinessKeys.AppendLine("  AND a.TABLE_NAME= '" + (string)hubTable["HUB_TABLE_NAME"] + "'");
                    queryHubBusinessKeys.AppendLine("  AND COLUMN_NAME NOT IN ('" + TeamConfigurationSettings.RecordSourceAttribute + "','" +TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" +TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" +TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "','" +TeamConfigurationSettings.EtlProcessAttribute + "','" + TeamConfigurationSettings.LoadDateTimeAttribute + "')");
                }
                else //Get the key details from the metadata
                {
                    queryHubBusinessKeys.AppendLine("SELECT a.TABLE_NAME, a.COLUMN_NAME, b.TOTALROWS");
                    queryHubBusinessKeys.AppendLine("FROM MD_VERSION_ATTRIBUTE a");
                    queryHubBusinessKeys.AppendLine("JOIN ");
                    queryHubBusinessKeys.AppendLine("	(SELECT TABLE_NAME, COUNT(*) AS TOTALROWS");
                    queryHubBusinessKeys.AppendLine("	 FROM   MD_VERSION_ATTRIBUTE");
                    queryHubBusinessKeys.AppendLine("	 WHERE VERSION_ID = " + versionId);
                    queryHubBusinessKeys.AppendLine("      AND SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" + localkeyLength +"," +localkeySubstring + ")!='_" + TeamConfigurationSettings.DwhKeyIdentifier + "'");queryHubBusinessKeys.AppendLine("	   AND SUBSTRING(TABLE_NAME,1," + localHubPrefixLength + ")='" +TeamConfigurationSettings.HubTablePrefixValue + "_'");
                    queryHubBusinessKeys.AppendLine("      AND COLUMN_NAME NOT IN ('" + TeamConfigurationSettings.RecordSourceAttribute + "','" +TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" +TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" +TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "','" +TeamConfigurationSettings.EtlProcessAttribute + "','" + TeamConfigurationSettings.LoadDateTimeAttribute + "')");
                    queryHubBusinessKeys.AppendLine("	 GROUP BY TABLE_NAME");
                    queryHubBusinessKeys.AppendLine("	) b");
                    queryHubBusinessKeys.AppendLine("ON a.TABLE_NAME=b.TABLE_NAME");
                    queryHubBusinessKeys.AppendLine("WHERE VERSION_ID = " + versionId);
                    queryHubBusinessKeys.AppendLine("AND SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" + localkeyLength +"," +localkeySubstring + ")!='_" + TeamConfigurationSettings.DwhKeyIdentifier + "'");
                    queryHubBusinessKeys.AppendLine("  AND a.TABLE_NAME= '" + (string)hubTable["HUB_TABLE_NAME"] + "'");
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

        private LinkedList<string[]> GetAllHubTablesForLink(string targetTableName, int versionId)
        {
            

            // First, get the associated Hub tables for the Link
            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            var queryLinkHubTables = new StringBuilder();
            int groupCounter = 1;

            queryLinkHubTables.AppendLine("SELECT DISTINCT ");
            queryLinkHubTables.AppendLine(" [LINK_TABLE_ID]");
            queryLinkHubTables.AppendLine(",[LINK_TABLE_NAME]");
            queryLinkHubTables.AppendLine(",[STAGING_AREA_TABLE_ID]");
            queryLinkHubTables.AppendLine(",[STAGING_AREA_TABLE_NAME]");
            queryLinkHubTables.AppendLine(",[STAGING_AREA_SCHEMA_NAME]");
            queryLinkHubTables.AppendLine(",[HUB_TABLE_ID]");
            queryLinkHubTables.AppendLine(",[HUB_TABLE_NAME]");
            queryLinkHubTables.AppendLine(",[HUB_ORDER]");
            queryLinkHubTables.AppendLine("FROM [interface].[INTERFACE_HUB_LINK_XREF]");
            queryLinkHubTables.AppendLine("WHERE LINK_TABLE_NAME = '" + targetTableName + "'");
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
                    queryHubBusinessKeys.AppendLine("	 GROUP BY TABLE_NAME");
                    queryHubBusinessKeys.AppendLine("	) b");
                    queryHubBusinessKeys.AppendLine("ON a.TABLE_NAME=b.TABLE_NAME");
                    queryHubBusinessKeys.AppendLine("WHERE SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" + localkeyLength + "," + localkeySubstring + ")!='_" + TeamConfigurationSettings.DwhKeyIdentifier + "'"); queryHubBusinessKeys.AppendLine("  AND a.TABLE_NAME= '" + (string)hubTable["HUB_TABLE_NAME"] + "'");
                    queryHubBusinessKeys.AppendLine("  AND COLUMN_NAME NOT IN ('" + TeamConfigurationSettings.RecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" + TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "','" + TeamConfigurationSettings.EtlProcessAttribute + "','" + TeamConfigurationSettings.LoadDateTimeAttribute + "')");
                }
                else //Get the key details from the metadata
                {
                    queryHubBusinessKeys.AppendLine("SELECT a.TABLE_NAME, a.COLUMN_NAME, b.TOTALROWS");
                    queryHubBusinessKeys.AppendLine("FROM MD_VERSION_ATTRIBUTE a");
                    queryHubBusinessKeys.AppendLine("JOIN ");
                    queryHubBusinessKeys.AppendLine("	(SELECT TABLE_NAME, COUNT(*) AS TOTALROWS");
                    queryHubBusinessKeys.AppendLine("	 FROM   MD_VERSION_ATTRIBUTE");
                    queryHubBusinessKeys.AppendLine("	 WHERE VERSION_ID = " + versionId);
                    queryHubBusinessKeys.AppendLine("      AND SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" + localkeyLength + "," + localkeySubstring + ")!='_" + TeamConfigurationSettings.DwhKeyIdentifier + "'"); queryHubBusinessKeys.AppendLine("	   AND SUBSTRING(TABLE_NAME,1," + localHubPrefixLength + ")='" + TeamConfigurationSettings.HubTablePrefixValue + "_'");
                    queryHubBusinessKeys.AppendLine("      AND COLUMN_NAME NOT IN ('" + TeamConfigurationSettings.RecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" + TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" + TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "','" + TeamConfigurationSettings.EtlProcessAttribute + "','" + TeamConfigurationSettings.LoadDateTimeAttribute + "')");
                    queryHubBusinessKeys.AppendLine("	 GROUP BY TABLE_NAME");
                    queryHubBusinessKeys.AppendLine("	) b");
                    queryHubBusinessKeys.AppendLine("ON a.TABLE_NAME=b.TABLE_NAME");
                    queryHubBusinessKeys.AppendLine("WHERE VERSION_ID = " + versionId);
                    queryHubBusinessKeys.AppendLine("AND SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" + localkeyLength + "," + localkeySubstring + ")!='_" + TeamConfigurationSettings.DwhKeyIdentifier + "'");
                    queryHubBusinessKeys.AppendLine("  AND a.TABLE_NAME= '" + (string)hubTable["HUB_TABLE_NAME"] + "'");
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
            richTextBoxLsat.Clear();
            richTextBoxInformation.Clear();

            var newThread = new Thread(BackgroundDoLsat);
            newThread.Start(trackBarVersioning.Value);
        }

        private void BackgroundDoLsat(Object obj)
        {
            var versionId = (int)obj;

            if (radiobuttonViews.Checked)
            {
                GenerateLsatHistoryViews(versionId);
                GenerateLsatDrivingKeyViews(versionId);
            }
            else if (radiobuttonStoredProc.Checked)
            {
                MessageBox.Show("This feature is yet to be implemented.", "Upcoming!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (radioButtonIntoStatement.Checked)
            {
                GenerateLsatInsertInto(versionId);
            }
        }

        private void GenerateLsatInsertInto(int versionId)
        {
            

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
                    //                               "   a.STAGING_AREA_TABLE_ID, " +
                    //                               "   c.STAGING_AREA_TABLE_NAME, " +
                    //                               "   SATELLITE_TABLE_ID, " +
                    //                               "   SATELLITE_TABLE_NAME, " +
                    //                               "   b.LINK_TABLE_ID, " +
                    //                               "   LINK_TABLE_NAME " +
                    //                               "FROM MD_SAT a " +
                    //                               "JOIN MD_LINK b on a.LINK_TABLE_ID=b.LINK_TABLE_ID " +
                    //                               "JOIN MD_STG c on a.STAGING_AREA_TABLE_ID=c.STAGING_AREA_TABLE_ID " +
                    //                               "WHERE SATELLITE_TYPE IN ('Link Satellite', 'Link Satellite - Without Attributes')" +
                    //                               " AND SATELLITE_TABLE_NAME = '" + targetTableName + "'";

                    var sqlStatementForTablesToImport = new StringBuilder();
                    sqlStatementForTablesToImport.AppendLine("SELECT * FROM interface.INTERFACE_STAGING_SATELLITE_XREF base");
                    sqlStatementForTablesToImport.AppendLine("WHERE SATELLITE_TYPE = 'Link Satellite' ");
                    sqlStatementForTablesToImport.AppendLine(" AND SATELLITE_TABLE_NAME = '" + targetTableName + "'");

                    var tables = GetDataTable(ref connOmd, sqlStatementForTablesToImport.ToString());

                    foreach (DataRow row in tables.Rows)
                    {
                        var linkSk = row["LINK_TABLE_NAME"].ToString().Substring(4) + "_" + TeamConfigurationSettings.DwhKeyIdentifier;

                        // Build the main attribute list of the Satellite table for selection
                        var sourceTableStructure = GetTableStructure(targetTableName, ref conn, versionId, "LSAT");

                        // Query to detect multi-active attributes
                        var multiActiveAttributes = GetMultiActiveAttributes((int) row["SATELLITE_TABLE_ID"]);

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
                            else
                            {
                                insertIntoStatement.Append("   lsat_view.[" + sourceAttribute + "],");
                                insertIntoStatement.AppendLine();
                            }
                        }

                        insertIntoStatement.Remove(insertIntoStatement.Length - 3, 3);
                        insertIntoStatement.AppendLine();
                        insertIntoStatement.AppendLine("FROM " + targetTableName + " lsat_view");
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
                            insertIntoStatement.AppendLine("AND lsat_view.[" + (string) attribute["ATTRIBUTE_NAME_TO"] +"] = lsat_table.[" + (string) attribute["ATTRIBUTE_NAME_TO"] +"]");
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
                            GenerateInDatabase(connHstg, insertIntoStatement.ToString());
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
        }

        // Link Satellite generation - driving key based
        private void GenerateLsatDrivingKeyViews(int versionId)
        {
            

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
                    sqlStatementForTablesToImport.AppendLine("SELECT * FROM interface.INTERFACE_STAGING_SATELLITE_XREF base");
                    sqlStatementForTablesToImport.AppendLine("WHERE SATELLITE_TYPE = 'Link Satellite' ");
                    sqlStatementForTablesToImport.AppendLine(" AND SATELLITE_TABLE_NAME = '" + targetTableName + "'");
                    sqlStatementForTablesToImport.AppendLine(" AND EXISTS (SELECT * FROM[interface].[INTERFACE_DRIVING_KEY] sub WHERE sub.SATELLITE_TABLE_ID = base.SATELLITE_TABLE_ID)");

                    var tables = GetDataTable(ref connOmd, sqlStatementForTablesToImport.ToString());

                    foreach (DataRow row in tables.Rows)
                        {
                            var linkSatView = new StringBuilder();

                            string multiActiveAttributeFromName;

                            var psaTableName = TeamConfigurationSettings.PsaTablePrefixValue + row["STAGING_AREA_TABLE_NAME"].ToString().Replace(TeamConfigurationSettings.StgTablePrefixValue, "");


                            var stagingAreaTableName = (string)row["STAGING_AREA_TABLE_NAME"];

                            var filterCriteria = (string) row["FILTER_CRITERIA"];

                            var targetTableId = (int) row["SATELLITE_TABLE_ID"];

                            var linkTableName = (string) row["LINK_TABLE_NAME"];

                            var linkSk = linkTableName.Substring(4) + "_" + TeamConfigurationSettings.DwhKeyIdentifier;

                            // Query to detect multi-active attributes
                            var multiActiveAttributes = GetMultiActiveAttributes(targetTableId);

                            // Get the associated Hub tables for the Link
                            var hubBusinessKeyList = GetHubTablesForLink(linkTableName, versionId);

                            // Retrieving the business key attributes for the Hubs associated with the Link
                            //var hubBusinessKeyList = GetHubBusinessKeysForLink(linkHubTables, versionId);

                            var degenerateLinkAttributes = GetDegenerateLinkAttributes(linkTableName);

                            // Create a list of Driving Key(s) for the Link table
                            var queryDrivingKeys = new StringBuilder();

                            //queryLinkDrivingKeys.AppendLine("SELECT ");
                            //queryLinkDrivingKeys.AppendLine(" a.HUB_TABLE_ID, ");
                            //queryLinkDrivingKeys.AppendLine(" c.HUB_TABLE_NAME, ");
                            //queryLinkDrivingKeys.AppendLine(" a.LINK_TABLE_ID, ");
                            //queryLinkDrivingKeys.AppendLine(" b.LINK_TABLE_NAME, ");
                            //queryLinkDrivingKeys.AppendLine(" a.DRIVING_KEY_INDICATOR ");
                            //queryLinkDrivingKeys.AppendLine("FROM MD_HUB_LINK_XREF a ");
                            //queryLinkDrivingKeys.AppendLine("JOIN MD_LINK b ON a.LINK_TABLE_ID=b.LINK_TABLE_ID ");
                            //queryLinkDrivingKeys.AppendLine("JOIN MD_HUB c ON a.HUB_TABLE_ID=c.HUB_TABLE_ID ");
                            //queryLinkDrivingKeys.AppendLine("WHERE b.LINK_TABLE_ID = '" + linkTableId + "'");
                            //queryLinkDrivingKeys.AppendLine("AND DRIVING_KEY_INDICATOR='Y'");

                            queryDrivingKeys.AppendLine("SELECT ");
                            queryDrivingKeys.AppendLine("  [SATELLITE_TABLE_ID]");
                            queryDrivingKeys.AppendLine(" ,[SATELLITE_TABLE_NAME]");
                            queryDrivingKeys.AppendLine(" ,[HUB_TABLE_ID]");
                            queryDrivingKeys.AppendLine(" ,[HUB_TABLE_NAME]");
                            queryDrivingKeys.AppendLine("FROM [interface].[INTERFACE_DRIVING_KEY]");
                            queryDrivingKeys.AppendLine("WHERE SATELLITE_TABLE_NAME = '"+ targetTableName+"'");

                            var drivingKeysDataTable = GetDataTable(ref connOmd, queryDrivingKeys.ToString());
                            drivingKeysDataTable.PrimaryKey = new[] { drivingKeysDataTable.Columns["HUB_TABLE_NAME"]};

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
                            linkSatView.AppendLine("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" +targetTableName + "]') AND type in (N'V'))");
                            linkSatView.AppendLine("DROP VIEW [" + TeamConfigurationSettings.SchemaName + "].[" + targetTableName + "]");
                            linkSatView.AppendLine("go");
                            linkSatView.AppendLine("CREATE VIEW [" + TeamConfigurationSettings.SchemaName + "].[" + targetTableName +"] AS  ");
                            linkSatView.AppendLine("SELECT");

                            if (!checkBoxDisableHash.Checked)
                            {
                                linkSatView.AppendLine("   CONVERT(CHAR(32),HASHBYTES('MD5',");

                                foreach (var hubArray in hubBusinessKeyList)
                                {
                                    linkSatView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100),[" + hubArray[1] + "])),'NA')+'|'+");
                                }

                                // Add the degenerate attributes
                                foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                                {
                                    linkSatView.AppendLine("    ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + (string)attribute["ATTRIBUTE_NAME_TO"] + ")),'NA')+'|'+");
                                }

                                linkSatView.Remove(linkSatView.Length - 3, 3);
                                linkSatView.AppendLine();
                                linkSatView.AppendLine("   ),2) AS " + linkSk + ",");
                            }
                            else
                            {
                                //BK as DWH key
                                foreach (var hubArray in hubBusinessKeyList)
                                {
                                    linkSatView.Append("  ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + hubArray[1] + ")),'NA')+'|'+");
                                }

                                foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                                {
                                    linkSatView.Append("    ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + (string)attribute["ATTRIBUTE_NAME_TO"] + ")),'NA')+'|'+");
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
                                linkSatView.AppendLine("   [" + (string)attribute["ATTRIBUTE_NAME_TO"] + "],");
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
                            foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                            {
                                linkSatView.AppendLine("    ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + (string)attribute["ATTRIBUTE_NAME_TO"] + ")),'NA')+'|'+");
                            }

                            // Multi-active attributes
                            foreach (DataRow attribute in multiActiveAttributes.Rows)
                            {
                                multiActiveAttributeFromName = (string) attribute["ATTRIBUTE_NAME_FROM"];
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
                                linkSatView.AppendLine("       [" + (string)attribute["ATTRIBUTE_NAME_FROM"] + "],");
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
                                multiActiveAttributeFromName = (string) attribute["ATTRIBUTE_NAME_FROM"];
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
                                linkSatView.AppendLine("          [" + (string)attribute["ATTRIBUTE_NAME_FROM"] + "],");
                            }

                            linkSatView.AppendLine("          [" + TeamConfigurationSettings.LoadDateTimeAttribute + "]) AS INT)");
                            linkSatView.AppendLine("   AS ROW_NUMBER,");

                            // Checksum
                            linkSatView.AppendLine("   CONVERT(CHAR(32),HASHBYTES('MD5',");
                            linkSatView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100),[" +
                                                   TeamConfigurationSettings.ChangeDataCaptureAttribute + "])),'NA')+'|'+");

                            foreach (var hubArray in hubBusinessKeyList)
                            {
                                linkSatView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100),[" + hubArray[1] +"])),'NA')+'|'+");
                            }

                            foreach (DataRow attribute in multiActiveAttributes.Rows)
                            {
                                linkSatView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100),[" + (string)attribute["ATTRIBUTE_NAME_FROM"] + "])),'NA')+'|'+");
                            }

                            foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                            {
                                linkSatView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100),[" + (string)attribute["ATTRIBUTE_NAME_TO"] + "])),'NA')+'|'+");
                            }

                            linkSatView.Remove(linkSatView.Length - 3, 3);
                            linkSatView.AppendLine();
                            linkSatView.AppendLine("   ),2) AS " + TeamConfigurationSettings.RecordChecksumAttribute + "");

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

                                var hubTableName = (string) hubDetailRow["HUB_TABLE_NAME"];
                                var businessKeyDefinition = (string) hubDetailRow["BUSINESS_KEY_DEFINITION"];

                                var hubKeyList = GetHubTargetBusinessKeyList(hubTableName, versionId);

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

                            foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                            {
                                linkSatView.AppendLine("    [" + (string)attribute["ATTRIBUTE_NAME_TO"] + "],");
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
                            linkSatView.AppendLine("  FROM " + psaTableName);

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
                                    multiActiveAttributeFromName = (string) attribute["ATTRIBUTE_NAME_FROM"];
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
                                    multiActiveAttributeFromName = (string) attribute["ATTRIBUTE_NAME_FROM"];
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
                                    multiActiveAttributeFromName = (string) attribute["ATTRIBUTE_NAME_FROM"];
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
                                    multiActiveAttributeFromName = (string) attribute["ATTRIBUTE_NAME_FROM"];
                                    linkSatView.Append("         " + multiActiveAttributeFromName + ",");
                                }
                                linkSatView.Remove(linkSatView.Length - 1, 1);
                                linkSatView.AppendLine(" ASC) ROWVERSION");
                                linkSatView.AppendLine("  FROM " + psaTableName);
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
                                GenerateInDatabase(connHstg, linkSatView.ToString());
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
        }

        private DataTable GetMultiActiveAttributes(int targetTableId)
        {
            // Query to detect multi-active attributes
            

            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            var multiActiveAttributeQuery = new StringBuilder();

            multiActiveAttributeQuery.AppendLine("SELECT ");
            multiActiveAttributeQuery.AppendLine("c.ATTRIBUTE_NAME AS ATTRIBUTE_NAME_FROM,");
            multiActiveAttributeQuery.AppendLine("b.ATTRIBUTE_NAME AS ATTRIBUTE_NAME_TO");
            multiActiveAttributeQuery.AppendLine("FROM MD_STG_SAT_ATT_XREF a");
            multiActiveAttributeQuery.AppendLine("JOIN MD_ATT b ON a.ATTRIBUTE_ID_TO=b.ATTRIBUTE_ID");
            multiActiveAttributeQuery.AppendLine("	JOIN MD_ATT c ON a.ATTRIBUTE_ID_FROM=c.ATTRIBUTE_ID");
            multiActiveAttributeQuery.AppendLine("WHERE");
            multiActiveAttributeQuery.AppendLine("     MULTI_ACTIVE_KEY_INDICATOR='Y'");
            multiActiveAttributeQuery.AppendLine(" AND SATELLITE_TABLE_ID=" + targetTableId);

            var multiActiveAttributes = GetDataTable(ref connOmd, multiActiveAttributeQuery.ToString());
            multiActiveAttributes.PrimaryKey = new[] { multiActiveAttributes.Columns["ATTRIBUTE_NAME_FROM"] };

            return multiActiveAttributes;
        }

        // Link Satellite generation - historical
        private void GenerateLsatHistoryViews(int versionId)
        {
            

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
                    sqlStatementForTablesToImport.AppendLine("SELECT * FROM interface.INTERFACE_STAGING_SATELLITE_XREF base");
                    sqlStatementForTablesToImport.AppendLine("WHERE SATELLITE_TYPE = 'Link Satellite' ");
                    sqlStatementForTablesToImport.AppendLine(" AND SATELLITE_TABLE_NAME = '" + targetTableName + "'");
                    sqlStatementForTablesToImport.AppendLine(" AND NOT EXISTS (SELECT * FROM[interface].[INTERFACE_DRIVING_KEY] sub WHERE sub.SATELLITE_TABLE_ID = base.SATELLITE_TABLE_ID)");

                    var tables = GetDataTable(ref connOmd, sqlStatementForTablesToImport.ToString());

                        foreach (DataRow row in tables.Rows)
                        {
                            var linkSatView = new StringBuilder();

                            var stagingAreaTableId = (int) row["STAGING_AREA_TABLE_ID"];
                            var stagingAreaTableName = (string) row["STAGING_AREA_TABLE_NAME"];
                            var filterCriteria = (string) row["FILTER_CRITERIA"];
                            var targetTableId = (int) row["SATELLITE_TABLE_ID"];
                            var linkTableName = (string) row["LINK_TABLE_NAME"];

                            var linkSk = linkTableName.Substring(4) + "_" + TeamConfigurationSettings.DwhKeyIdentifier;
                            var currentTableName = (string) row["STAGING_AREA_TABLE_NAME"];
                            currentTableName = TeamConfigurationSettings.PsaTablePrefixValue +currentTableName.Replace(TeamConfigurationSettings.StgTablePrefixValue, "");


                            // Query to detect multi-active attributes
                            var multiActiveAttributes = GetMultiActiveAttributes(targetTableId);

                            // Retrieve the Source-To-Target mapping for Satellites
                            var sourceStructure = GetStagingToSatelliteAttributeMapping(targetTableId, stagingAreaTableId);

                            // Get the associated Hub tables and its target business key attributes for the Link
                            var hubBusinessKeyList = GetHubTablesForLink(linkTableName, versionId);

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
                            linkSatView.AppendLine("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" +targetTableName + "]') AND type in (N'V'))");
                            linkSatView.AppendLine("DROP VIEW [" + TeamConfigurationSettings.SchemaName + "].[" + targetTableName +"]");
                            linkSatView.AppendLine("go");
                            linkSatView.AppendLine("CREATE VIEW [" + TeamConfigurationSettings.SchemaName + "].[" + targetTableName +"] AS  ");
                            linkSatView.AppendLine("SELECT");


                            if (!checkBoxDisableHash.Checked)
                            {
                                // Link SK - combined hash key
                                linkSatView.AppendLine("   CONVERT(CHAR(32),HASHBYTES('MD5',");

                                foreach (var hubArray in hubBusinessKeyList)
                                {
                                    linkSatView.AppendLine("    ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + hubArray[1] + ")),'NA')+'|'+");
                                }

                                // Add the degenerate attributes to be part of the key
                                foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                                {
                                    linkSatView.AppendLine("    ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + (string)attribute["ATTRIBUTE_NAME_TO"] + ")),'NA')+'|'+");
                                }

                                linkSatView.Remove(linkSatView.Length - 3, 3);
                                linkSatView.AppendLine();
                                linkSatView.AppendLine("  ),2) AS " + linkSk + ",");
                            }
                            else
                            {
                                //BK as DWH key
                                foreach (var hubArray in hubBusinessKeyList)
                                {
                                    linkSatView.Append("  ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + hubArray[1] + ")),'NA')+'|'+");
                                }

                                foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                                {
                                    linkSatView.Append("    ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + (string)attribute["ATTRIBUTE_NAME_TO"] + ")),'NA')+'|'+");
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
                                var multiActiveAttributeFromName = (string) attribute["ATTRIBUTE_NAME_TO"];
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
                            foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                            {
                                linkSatView.AppendLine("              " + (string)attribute["ATTRIBUTE_NAME_TO"] + ",");
                            }

                            // Add the multi-active attributes
                            foreach (DataRow attribute in multiActiveAttributes.Rows)
                            {
                                linkSatView.AppendLine("              " + (string)attribute["ATTRIBUTE_NAME_TO"] + ",");
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
                            foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                            {
                                linkSatView.AppendLine("            " + (string)attribute["ATTRIBUTE_NAME_TO"] + ",");
                            }

                            // Multi-Active
                            foreach (DataRow attribute in multiActiveAttributes.Rows)
                            {
                                linkSatView.AppendLine("         " + (string)attribute["ATTRIBUTE_NAME_TO"] + ",");
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
                                    linkSatView.Append("   " + attribute["ATTRIBUTE_NAME_TO"] + ",");
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
                            foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                            {
                                linkSatView.AppendLine("         " + (string)attribute["ATTRIBUTE_NAME_TO"] + ",");
                            }

                            // Multi-active
                            foreach (DataRow attribute in multiActiveAttributes.Rows)
                            {
                                linkSatView.AppendLine("         " + (string)attribute["ATTRIBUTE_NAME_TO"] + ",");
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
                            foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                            {
                                linkSatView.AppendLine("         " + (string)attribute["ATTRIBUTE_NAME_TO"] + ",");
                            }

                            // Multi-active
                            foreach (DataRow attribute in multiActiveAttributes.Rows)
                            {
                                linkSatView.AppendLine("         " + (string)attribute["ATTRIBUTE_NAME_TO"] + ",");
                            }

                            linkSatView.AppendLine("         [" + TeamConfigurationSettings.LoadDateTimeAttribute + "]) AS INT)");
                            linkSatView.AppendLine("   AS ROW_NUMBER,");

                            // Checksum
                            linkSatView.AppendLine("   CONVERT(CHAR(32),HASHBYTES('MD5',");
                            linkSatView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100),[" +TeamConfigurationSettings.ChangeDataCaptureAttribute + "])),'NA')+'|'+");

                            foreach (var hubArray in hubBusinessKeyList)
                            {
                                var hubBusinessKeyName = hubArray[1];
                                linkSatView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + hubBusinessKeyName +")),'NA')+'|'+");
                            }

                            foreach (DataRow attribute in sourceStructure.Rows)
                            {
                                linkSatView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," +attribute["ATTRIBUTE_NAME_TO"] + ")),'NA')+'|'+");
                            }
                            // End of checksum

                            linkSatView.Remove(linkSatView.Length - 3, 3);
                            linkSatView.AppendLine();
                            linkSatView.AppendLine("   ),2) AS " + TeamConfigurationSettings.RecordChecksumAttribute + "");

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
                            var sqlStatementForSourceQuery = new StringBuilder();


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
                                var fieldList = new StringBuilder();
                                var compositeKey = new StringBuilder();
                                var fieldDict = new Dictionary<string, string>();
                                var fieldOrderedList = new List<string>();
                                string firstKey;

                                var hubTableName = (string) hubDetailRow["HUB_TABLE_NAME"];
                                var businessKeyDefinition = (string) hubDetailRow["BUSINESS_KEY_DEFINITION"];
                                var hubKeyList = GetHubTargetBusinessKeyList(hubTableName, versionId);



                                // For every STG / Hub relationship, the business key needs to be defined - starting with the components of the key
                                var componentList = GetBusinessKeyComponentList(stagingAreaTableName, hubTableName, businessKeyDefinition);

                                // Components are key parts, such as a composite key (2 or more components) or regular and concatenated keys (1 component)
                                foreach (DataRow component in componentList.Rows)
                                {
                                    var componentId = (int) component["BUSINESS_KEY_COMPONENT_ID"] - 1;

                                    // Retrieve the elements of each business key component
                                    // This only concerns concatenated keys as they are single component keys comprising of multiple elements.
                                    var elementList = GetBusinessKeyElements(stagingAreaTableName, hubTableName,
                                        businessKeyDefinition, (int) component["BUSINESS_KEY_COMPONENT_ID"]);

                                    if (elementList == null)
                                    {
                                        SetTextDebug("\n");
                                        SetTextHub(
                                            $"An error occurred for the Hub Insert Into statement for {hubTableName}. The collection of Business Keys is empty.\r\n");
                                    }
                                    else
                                    {
                                        if (elementList.Rows.Count > 1)
                                            // Build a concatinated key if the count of elements is greater than 1 for a component (key part)
                                        {
                                            fieldList.Clear();
                                            fieldDict.Clear();

                                            foreach (DataRow element in elementList.Rows)
                                            {
                                                var elementType =
                                                    element["BUSINESS_KEY_COMPONENT_ELEMENT_TYPE"].ToString();

                                                if (elementType == "Attribute")
                                                {
                                                    fieldList.Append("'" +
                                                                     element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] +
                                                                     "',");

                                                    sqlStatementForSourceQuery.AppendLine(
                                                        "SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION");
                                                    sqlStatementForSourceQuery.AppendLine(
                                                        "FROM INFORMATION_SCHEMA.COLUMNS");
                                                    sqlStatementForSourceQuery.AppendLine("WHERE TABLE_NAME= '" +
                                                                                          stagingAreaTableName + "'");
                                                    sqlStatementForSourceQuery.AppendLine("AND COLUMN_NAME IN (" +
                                                                                          fieldList.ToString()
                                                                                              .Substring(0,
                                                                                                  fieldList.ToString()
                                                                                                      .Length - 1) + ")");

                                                    var elementDataTypes = GetDataTable(ref connStg,
                                                        sqlStatementForSourceQuery.ToString());

                                                    foreach (DataRow attribute in elementDataTypes.Rows)
                                                    {
                                                        fieldDict.Add(attribute["COLUMN_NAME"].ToString(),
                                                            attribute["DATA_TYPE"].ToString());
                                                    }
                                                }
                                                else if (elementType == "User Defined Value")
                                                {
                                                    fieldList.Append("''" +
                                                                     element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] +
                                                                     "'',");
                                                }

                                                fieldOrderedList.Add(
                                                    element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"].ToString());
                                            }


                                            // Build the concatenated key
                                            foreach (var busKey in fieldOrderedList)
                                            {
                                                if (fieldDict.ContainsKey(busKey))
                                                {
                                                    var key = "ISNULL([" + busKey + "], '')";

                                                    if ((fieldDict[busKey] == "datetime2") ||
                                                        (fieldDict[busKey] == "datetime"))
                                                    {
                                                        key = "CASE WHEN [" + busKey + "] IS NOT NULL THEN CONVERT(" +
                                                              stringDataType + "(100), [" + busKey +
                                                              "], 112) ELSE '' END";
                                                    }
                                                    else if ((fieldDict[busKey] == "numeric") ||
                                                             (fieldDict[busKey] == "integer") ||
                                                             (fieldDict[busKey] == "int") ||
                                                             (fieldDict[busKey] == "tinyint") ||
                                                             (fieldDict[busKey] == "decimal"))
                                                    {
                                                        key = "CASE WHEN [" + busKey + "] IS NOT NULL THEN CAST([" +
                                                              busKey + "] AS " + stringDataType +
                                                              "(100)) ELSE '' END";
                                                    }

                                                    compositeKey.Append(key).Append(" + ");
                                                }
                                                else
                                                {
                                                    var key = " " + busKey;
                                                    compositeKey.Append(key).Append(" + ");
                                                }
                                            }

                                            hubQuerySelect.AppendLine(
                                                compositeKey.ToString().Substring(0, compositeKey.ToString().Length - 2) +
                                                " AS [" + hubKeyList.Rows[componentId]["COLUMN_NAME"] + groupCounter +
                                                "],");
                                            hubQueryWhere.AppendLine(
                                                compositeKey.ToString().Substring(0, compositeKey.ToString().Length - 2) +
                                                " != '' AND");
                                            hubQueryGroupBy.AppendLine(
                                                compositeKey.ToString().Substring(0, compositeKey.ToString().Length - 2) +
                                                ",");
                                        }
                                        else // Handle a component of a single or composite key 
                                        {
                                            foreach (DataRow element in elementList.Rows) // Only a single element...
                                            {
                                                if (element["BUSINESS_KEY_COMPONENT_ELEMENT_TYPE"].ToString() ==
                                                    "User Defined Value")
                                                {
                                                    firstKey =
                                                        element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"].ToString();
                                                    hubQuerySelect.AppendLine("    " + firstKey + " AS [" +
                                                                              hubKeyList.Rows[componentId]["COLUMN_NAME"
                                                                                  ] + groupCounter + "],");
                                                }
                                                else // It's a normal attribute
                                                {
                                                    // We need the data type again
                                                    sqlStatementForSourceQuery.AppendLine(
                                                        "SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION");
                                                    sqlStatementForSourceQuery.AppendLine(
                                                        "FROM INFORMATION_SCHEMA.COLUMNS");
                                                    sqlStatementForSourceQuery.AppendLine("WHERE TABLE_NAME= '" +
                                                                                          stagingAreaTableName + "'");
                                                    sqlStatementForSourceQuery.AppendLine("AND COLUMN_NAME = ('" +
                                                                                          element[
                                                                                              "BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"
                                                                                              ] + "')");

                                                    firstKey = "[" + element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] +
                                                               "]";

                                                    var elementDataTypes = GetDataTable(ref connStg,
                                                        sqlStatementForSourceQuery.ToString());

                                                    foreach (DataRow attribute in elementDataTypes.Rows)
                                                    {
                                                        if (attribute["DATA_TYPE"].ToString() == "numeric" ||
                                                            attribute["DATA_TYPE"].ToString() == "int")
                                                        {
                                                            hubQuerySelect.AppendLine("    CAST(" + firstKey + " AS " +
                                                                                      stringDataType + "(100)) AS [" +
                                                                                      hubKeyList.Rows[componentId][
                                                                                          "COLUMN_NAME"] + groupCounter +
                                                                                      "],");
                                                        }
                                                        else
                                                        {
                                                            hubQuerySelect.AppendLine("      " + firstKey + " AS [" +
                                                                                      hubKeyList.Rows[componentId][
                                                                                          "COLUMN_NAME"] + groupCounter +
                                                                                      "],");
                                                        }
                                                    }

                                                    hubQueryWhere.AppendLine(" " + firstKey + " IS NOT NULL AND");
                                                    hubQueryGroupBy.AppendLine("    " + firstKey + ",");
                                                }

                                            } // End of element loop (for single element)
                                        }
                                    }
                                } // End of component elements

                                groupCounter++;

                            } // End of business key creation

                            // Initiating select statement for subquery
                            linkSatView.AppendLine("" + hubQuerySelect);
                            linkSatView.Remove(linkSatView.Length - 4, 4);
                            linkSatView.AppendLine();

                            // Add the degenerate attributes
                            foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                            {
                                linkSatView.AppendLine("    [" + (string)attribute["ATTRIBUTE_NAME_FROM"] + "] AS [" + attribute["ATTRIBUTE_NAME_TO"] + "],");
                            }

                            // Add the multi-active attributes
                            foreach (DataRow attribute in multiActiveAttributes.Rows)
                            {
                                linkSatView.AppendLine("    [" + (string)attribute["ATTRIBUTE_NAME_FROM"] + "] AS [" + attribute["ATTRIBUTE_NAME_TO"] + "],");
                            }

                            // Add all the attributes
                            foreach (DataRow attribute in sourceStructure.Rows)
                            {
                                if (attribute["MULTI_ACTIVE_KEY_INDICATOR"].ToString() == "N")
                                {
                                    linkSatView.AppendLine("    " + attribute["ATTRIBUTE_NAME_FROM"] + " AS " + attribute["ATTRIBUTE_NAME_TO"] + ",");
                                }
                            }

                            linkSatView.Remove(linkSatView.Length - 3, 3);
                            linkSatView.AppendLine();
                            linkSatView.AppendLine("  FROM " + currentTableName);

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
                                foreach (DataRow attribute in degenerateLinkAttributes.Rows)
                                {
                                    linkSatView.AppendLine("    [" + (string)attribute["ATTRIBUTE_NAME_FROM"] + "] AS [" + attribute["ATTRIBUTE_NAME_TO"] + "],");
                                }

                                // Add the multi-active attribute
                                foreach (DataRow attribute in multiActiveAttributes.Rows)
                                {
                                    linkSatView.AppendLine("    " + (string)attribute["ATTRIBUTE_NAME_FROM"] + " AS [" + attribute["ATTRIBUTE_NAME_TO"] + "],");
                                }

                                // Add the rest of the attributes as NULL values (nothing was known at this stage)
                                foreach (DataRow attribute in sourceStructure.Rows)
                                {
                                    if (attribute["MULTI_ACTIVE_KEY_INDICATOR"].ToString() == "N")
                                    {
                                        linkSatView.AppendLine("    NULL AS [" + attribute["ATTRIBUTE_NAME_TO"] +"],");
                                    }
                                }

                                linkSatView.Remove(linkSatView.Length - 3, 3);
                                linkSatView.AppendLine();
                                linkSatView.AppendLine("  FROM " + currentTableName);
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
                                GenerateInDatabase(connHstg, linkSatView.ToString());
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
        }

        private DataTable GetStagingToSatelliteAttributeMapping(int targetTableId, int stagingAreaTableId)
        {
            

            var sqlStatementForAttributes = new StringBuilder();
            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };

            // Query the metadata to retrieve the STG and INT attributes and their relationship
            sqlStatementForAttributes.AppendLine("SELECT ");
            sqlStatementForAttributes.AppendLine(" [STAGING_AREA_TABLE_ID]");
            sqlStatementForAttributes.AppendLine(",[STAGING_AREA_TABLE_NAME]");
            sqlStatementForAttributes.AppendLine(",[STAGING_AREA_SCHEMA_NAME]");
            sqlStatementForAttributes.AppendLine(",[SATELLITE_TABLE_ID]");
            sqlStatementForAttributes.AppendLine(",[SATELLITE_TABLE_NAME]");
            sqlStatementForAttributes.AppendLine(",[ATTRIBUTE_ID_FROM]");
            sqlStatementForAttributes.AppendLine(",[ATTRIBUTE_NAME_FROM]");
            sqlStatementForAttributes.AppendLine(",[ATTRIBUTE_ID_TO]");
            sqlStatementForAttributes.AppendLine(",[ATTRIBUTE_NAME_TO]");
            sqlStatementForAttributes.AppendLine(",[MULTI_ACTIVE_KEY_INDICATOR]");
            sqlStatementForAttributes.AppendLine("FROM[interface].[INTERFACE_STAGING_SATELLITE_ATTRIBUTE_XREF]");
            sqlStatementForAttributes.AppendLine("WHERE SATELLITE_TABLE_ID = " + targetTableId);
            sqlStatementForAttributes.AppendLine("  AND STAGING_AREA_TABLE_ID = " + stagingAreaTableId);
            sqlStatementForAttributes.AppendLine("  AND ATTRIBUTE_NAME_TO NOT IN ('" +
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
            newThread.Start(trackBarVersioning.Value);
        }

        private void BackgroundDoPsa(Object obj)
        {
            var versionId = (int)obj;

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
                PsaGenerateInsertInto(versionId);
            }
        }

        // Create the Insert statement for the Persisten Staging Area (PSA)
        private void PsaGenerateInsertInto(int versionId)
        {
            

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
                    var sourceTableStructure = GetTableStructure(targetTableName, ref connHstg, versionId, "PSA");

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
                    psaInsertIntoStatement.AppendLine("FROM "+targetTableName);

                    using (var outfile = new StreamWriter(textBoxOutputPath.Text + @"\INSERT_STATEMENT_" + targetTableName + ".sql"))
                    {
                        outfile.Write(psaInsertIntoStatement.ToString());
                        outfile.Close();
                    }

                    if (checkBoxGenerateInDatabase.Checked)
                    {
                        connHstg.ConnectionString = TeamConfigurationSettings.ConnectionStringHstg;
                        GenerateInDatabase(connHstg, psaInsertIntoStatement.ToString());
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
        }

        private void PsaGenerateViews()
        {
            

            var mydocpath = textBoxOutputPath.Text;

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
                    else {

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
                    sqlStatementForSourceQuery.AppendLine("  AND COLUMN_NAME !='"+TeamConfigurationSettings.EtlProcessAttribute+"'");
                    sqlStatementForSourceQuery.AppendLine("  AND COLUMN_NAME NOT LIKE '%HASH_HUB%'");
                    sqlStatementForSourceQuery.AppendLine("  AND COLUMN_NAME NOT LIKE '%HASH_LNK%'");
                    sqlStatementForSourceQuery.AppendLine("  AND COLUMN_NAME NOT LIKE '%HASH_SAT%'");
                    sqlStatementForSourceQuery.AppendLine("  AND COLUMN_NAME NOT IN ('" + etlProcessIdAttribute + "')");
                    sqlStatementForSourceQuery.AppendLine("ORDER BY ORDINAL_POSITION");

                    var sourceStructure = GetDataTable(ref connStg, sqlStatementForSourceQuery.ToString());

                    
                    // Creating the  selection query
                    var psaView = new StringBuilder();

                    psaView.AppendLine("--");
                    psaView.AppendLine("-- PSA View definition for "+targetTableName);
                    psaView.AppendLine("-- Generated at 11/21/2014 11:52:15 AM");
                    psaView.AppendLine("--");
                    psaView.AppendLine();
                    psaView.AppendLine("USE ["+TeamConfigurationSettings.StagingDatabaseName+"]");
                    psaView.AppendLine("GO");

                    if (checkBoxIfExistsStatement.Checked)
                    {
                        psaView.AppendLine("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + targetTableName + "]') AND type in (N'V'))");
                        psaView.AppendLine("DROP VIEW [" + TeamConfigurationSettings.SchemaName + "].[" + targetTableName + "];");
                        psaView.AppendLine("GO");
                    }

                    psaView.AppendLine("CREATE VIEW [" + TeamConfigurationSettings.SchemaName + "].[" + targetTableName + "] AS "); 

                    psaView.AppendLine("SELECT ");
                    psaView.AppendLine("  " + targetTableName + "_" + TeamConfigurationSettings.DwhKeyIdentifier + ",");

                    foreach (DataRow attribute in sourceStructure.Rows)
                    {
                        psaView.AppendLine("  " + attribute["COLUMN_NAME"] + ",");
                    }
                    psaView.AppendLine("  LKP_"+TeamConfigurationSettings.RecordChecksumAttribute+",");
                    psaView.AppendLine("  LKP_"+TeamConfigurationSettings.ChangeDataCaptureAttribute+",");
                    psaView.AppendLine("  KEY_ROW_NUMBER");
                    psaView.AppendLine("FROM");
                    psaView.AppendLine("(");
                    psaView.AppendLine("  SELECT");
                    psaView.AppendLine("    CONVERT(CHAR(32),HASHBYTES('MD5',");

                    foreach (DataRow businessKey in partitionAttributes.Rows)
                    {
                        psaView.AppendLine("       ISNULL(RTRIM(CONVERT(" + stringDataType + "(100),STG." + (string)businessKey["ATTRIBUTE_NAME"] + ")),'NA')+'|'+");

                    }

                    psaView.AppendLine("       CONVERT(" + stringDataType + "(100),STG.[" + TeamConfigurationSettings.LoadDateTimeAttribute + "],126)+'|'");
                    psaView.AppendLine("    ),2) AS " + targetTableName + "_" + TeamConfigurationSettings.DwhKeyIdentifier + ",");
                    
                    foreach (DataRow attribute in sourceStructure.Rows)
                    {
                        psaView.AppendLine("    STG." + attribute["COLUMN_NAME"] + ",");
                    }

                    psaView.AppendLine("    COALESCE(maxsub.LKP_"+TeamConfigurationSettings.RecordChecksumAttribute+",'N/A') AS LKP_"+TeamConfigurationSettings.RecordChecksumAttribute+",");
                    psaView.AppendLine("    COALESCE(maxsub.LKP_"+cdcOperationAttribute+",'N/A') AS LKP_"+cdcOperationAttribute+",");

                    // ROW_NUMBER over Partition By query element
                    string currentBusinessKey;

                    psaView.AppendLine("    CAST(ROW_NUMBER() OVER (PARTITION  BY ");

                    foreach (DataRow businessKey in partitionAttributes.Rows)
                    {
                        currentBusinessKey = (string)businessKey["ATTRIBUTE_NAME"];
                        psaView.Append("       STG." + currentBusinessKey);
                        psaView.Append(",");
                    }

                    psaView.Remove(psaView.Length - 1, 1);
                    psaView.AppendLine();
                    psaView.AppendLine("    ORDER BY ");

                    foreach (DataRow businessKey in partitionAttributes.Rows)
                    {
                        currentBusinessKey = (string)businessKey["ATTRIBUTE_NAME"];
                        psaView.Append("       STG." + currentBusinessKey);
                        psaView.Append(", ");
                    }

                    psaView.Append("STG."+loadDateTimeAttribute+") AS INT) AS KEY_ROW_NUMBER");

                    // End of the query			
                    psaView.AppendLine();
                    psaView.AppendLine("  FROM "+sourceTableName + " STG");
                    psaView.AppendLine("  LEFT OUTER JOIN -- Prevent reprocessing");
                    psaView.AppendLine("    " + TeamConfigurationSettings.PsaDatabaseName + "." + TeamConfigurationSettings.SchemaName + "." + targetTableName + " HSTG");
                    psaView.AppendLine("    ON");

                    foreach (DataRow businessKey in partitionAttributes.Rows)
                    {
                        currentBusinessKey = (string)businessKey["ATTRIBUTE_NAME"];
                        psaView.AppendLine("       HSTG." + currentBusinessKey + " = STG." + currentBusinessKey + " AND");
                    }

                    psaView.AppendLine("       HSTG." + loadDateTimeAttribute + "=STG."+loadDateTimeAttribute+"");

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

                    psaView.AppendLine("      A."+TeamConfigurationSettings.RecordChecksumAttribute+" AS LKP_"+TeamConfigurationSettings.RecordChecksumAttribute+",");
                    psaView.AppendLine("      A."+cdcOperationAttribute+" AS LKP_"+cdcOperationAttribute+"");
                    psaView.AppendLine("    FROM " + TeamConfigurationSettings.PsaDatabaseName + "." + TeamConfigurationSettings.SchemaName + "." + targetTableName + " A");
                    psaView.AppendLine("    JOIN (");
                    psaView.AppendLine("      SELECT ");

                    foreach (DataRow businessKey in partitionAttributes.Rows)
                    {
                        psaView.Append("        B.");
                        psaView.Append(businessKey["ATTRIBUTE_NAME"]);
                        psaView.AppendLine(",");
                    }

                    psaView.AppendLine("        MAX(" + loadDateTimeAttribute + ") AS MAX_" + loadDateTimeAttribute + " ");
                    psaView.AppendLine("      FROM " + TeamConfigurationSettings.PsaDatabaseName + "." + TeamConfigurationSettings.SchemaName + "." + targetTableName + " B");
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
                        psaView.AppendLine("        A." + businessKey["ATTRIBUTE_NAME"] + " = C." + businessKey["ATTRIBUTE_NAME"] + " AND");
                    }

                    psaView.Remove(psaView.Length - 1, 1);

                    psaView.AppendLine("        A." + loadDateTimeAttribute + " = C.MAX_" + loadDateTimeAttribute + "");
                    psaView.AppendLine("  ) maxsub ON");

                    foreach (DataRow businessKey in partitionAttributes.Rows)
                    {
                        psaView.AppendLine("     STG." + (string)businessKey["ATTRIBUTE_NAME"] + " = maxsub." + (string)businessKey["ATTRIBUTE_NAME"] + " AND");
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
                    psaView.AppendLine("    (" + TeamConfigurationSettings.RecordChecksumAttribute + "!=LKP_"+TeamConfigurationSettings.RecordChecksumAttribute+")");
                    psaView.AppendLine("    -- The checksums are different");
                    psaView.AppendLine("    OR");
                    psaView.AppendLine("    (" + TeamConfigurationSettings.RecordChecksumAttribute + "=LKP_"+TeamConfigurationSettings.RecordChecksumAttribute+" AND "+TeamConfigurationSettings.ChangeDataCaptureAttribute+"!=LKP_"+TeamConfigurationSettings.ChangeDataCaptureAttribute+")");
                    psaView.AppendLine("    -- The checksums are the same but the CDC is different");
                    psaView.AppendLine("    -- In other words, if the hash is the same AND the CDC operation is the same then there is no change");
                    psaView.AppendLine("  )");
                    psaView.AppendLine(")");
                    psaView.AppendLine("OR");
                    psaView.AppendLine("(");
                    psaView.AppendLine("  -- It's not the most recent change in the set, so the record can be inserted as-is");
                    psaView.AppendLine("  KEY_ROW_NUMBER!=1");
                    psaView.AppendLine(")");
                    psaView.AppendLine();
                    psaView.AppendLine("GO");

                    using (var outfile = new StreamWriter(mydocpath + @"\VIEW_" + targetTableName + ".sql"))
                    {
                        outfile.Write(psaView.ToString());
                        outfile.Close();
                    }

                    if (checkBoxGenerateInDatabase.Checked)
                    {
                        connHstg.ConnectionString = TeamConfigurationSettings.ConnectionStringHstg;
                        GenerateInDatabase(connHstg, psaView.ToString());
                    }

                    SetTextDebug(psaView.ToString());
                    SetTextDebug("\n");

                    SetTextPsa($"Processing PSA entity view for {targetTableName}\r\n");
                }
                }
            }
            else
            {
                SetTextPsa("There was no metadata selected to create Persistent Staging Area views. Please check the metadata schema - are there any Staging Area tables selected?");
            }
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
            newThread.Start(trackBarVersioning.Value);
        }

        private void BackgroundDoStaging(Object obj)
        {
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
            

            var mydocpath = textBoxOutputPath.Text;

            if (checkedListBoxStgMetadata.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= checkedListBoxStgMetadata.CheckedItems.Count - 1; x++)
                {
                    var targetTableName = checkedListBoxStgMetadata.CheckedItems[x].ToString();
                    var connStg = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringStg }; 

                    // Build the main attribute list of the STG table for selection
                    var sqlStatementForSourceQuery = new StringBuilder();
                    sqlStatementForSourceQuery.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION");
                    sqlStatementForSourceQuery.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS");
                    sqlStatementForSourceQuery.AppendLine("WHERE TABLE_NAME= '" + targetTableName + "'");
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
                        stgInsertIntoStatement.AppendLine("TRUNCATE TABLE [" + TeamConfigurationSettings.StagingDatabaseName + "].[" +TeamConfigurationSettings.SchemaName + "].[" + targetTableName + "]");
                        stgInsertIntoStatement.AppendLine("GO");
                        stgInsertIntoStatement.AppendLine();
                    }

                    stgInsertIntoStatement.AppendLine("INSERT INTO [" + TeamConfigurationSettings.StagingDatabaseName + "].[" +TeamConfigurationSettings.SchemaName + "].[" + targetTableName + "]");
                    stgInsertIntoStatement.AppendLine("   (");

                    foreach (DataRow attribute in sourceStructure.Rows)
                    {
                        stgInsertIntoStatement.AppendLine("   " + attribute["COLUMN_NAME"] + ",");
                    }

                    stgInsertIntoStatement.Remove(stgInsertIntoStatement.Length - 3, 3);
                    stgInsertIntoStatement.AppendLine();
                    stgInsertIntoStatement.AppendLine("   )");
                    stgInsertIntoStatement.AppendLine("SELECT ");

                    foreach (DataRow attribute in sourceStructure.Rows)
                    {
                        if ((string)attribute["COLUMN_NAME"] == TeamConfigurationSettings.EtlProcessAttribute)
                        {
                            stgInsertIntoStatement.AppendLine("   -1 AS " + attribute["COLUMN_NAME"] + ",");
                        }
                        else
                        {
                            stgInsertIntoStatement.AppendLine("   " + attribute["COLUMN_NAME"] + ",");
                        }
                    }

                    stgInsertIntoStatement.Remove(stgInsertIntoStatement.Length - 3, 3);
                    stgInsertIntoStatement.AppendLine();
                    stgInsertIntoStatement.AppendLine("FROM VW_" + targetTableName);

                    using (var outfile = new StreamWriter(mydocpath + @"\INSERT_STATEMENT_" + targetTableName + ".sql"))
                    {
                        outfile.Write(stgInsertIntoStatement.ToString());
                        outfile.Close();
                    }

                    if (checkBoxGenerateInDatabase.Checked)
                    {
                        connStg.ConnectionString = TeamConfigurationSettings.ConnectionStringHstg;
                        GenerateInDatabase(connStg, stgInsertIntoStatement.ToString());
                    }

                    SetTextDebug(stgInsertIntoStatement.ToString());
                    SetTextDebug("\n");
                }
            }
            else
            {
                SetTextStaging("There was no metadata selected to create Staging Area insert statements. Please check the metadata schema - are there any Staging Area tables selected?");
            }
        }

        private void StagingGenerateViews()
        {
            

            var connStg = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringStg};
            var connOmd = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringOmd};

            var recordSourceAttribute = TeamConfigurationSettings.RecordSourceAttribute;
            var loadDateTimeAttribute = TeamConfigurationSettings.LoadDateTimeAttribute;
            var etlProcessIdAttribute = TeamConfigurationSettings.EtlProcessAttribute;
            var cdcOperationAttribute = TeamConfigurationSettings.ChangeDataCaptureAttribute;
            var eventDateTimeAttribute = TeamConfigurationSettings.EventDateTimeAttribute;
            var sourceRowIdAttribute = TeamConfigurationSettings.RowIdAttribute;
            var mydocpath = textBoxOutputPath.Text;

            if (checkedListBoxStgMetadata.CheckedItems.Count != 0)
            {
                for (int x = 0; x <= checkedListBoxStgMetadata.CheckedItems.Count - 1; x++)
                {
                    // **************************************************************************************
                    // Variables and parameters
                    // **************************************************************************************

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

                    // Creating the  selection query
                    var stgView = new StringBuilder();

                    stgView.AppendLine("--");
                    stgView.AppendLine("-- Staging Area View definition for " + targetTableName);
                    stgView.AppendLine("-- Generated at 11/21/2014 11:52:15 AM");
                    stgView.AppendLine("--");
                    stgView.AppendLine();
                    stgView.AppendLine("USE [" + TeamConfigurationSettings.StagingDatabaseName + "]");
                    stgView.AppendLine("GO");

                    if (checkBoxIfExistsStatement.Checked)
                    {
                        stgView.AppendLine("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VW_" +targetTableName + "]') AND type in (N'V'))");
                        stgView.AppendLine("DROP VIEW ["+TeamConfigurationSettings.SchemaName+"].[VW_" + targetTableName + "]");
                        stgView.AppendLine("GO");
                    }

                    stgView.AppendLine("CREATE VIEW [" + TeamConfigurationSettings.SchemaName + "].[VW_" + targetTableName + "] AS ");

                    // Retrieving the Natural Key for the Staging Area table
                    sqlStatementForNaturalKey.Clear();
                    sqlStatementForNaturalKey.AppendLine("SELECT");
                    sqlStatementForNaturalKey.AppendLine("st.name STAGING_AREA_TABLE_NAME,");
                    sqlStatementForNaturalKey.AppendLine("sc.name AS STAGING_AREA_ATTRIBUTE_NAME,");
                    sqlStatementForNaturalKey.AppendLine("COALESCE(sep.value,'Error - no indicator present') AS NATURAL_KEY_INDICATOR");
                    sqlStatementForNaturalKey.AppendLine("FROM sys.tables st");
                    sqlStatementForNaturalKey.AppendLine("INNER JOIN sys.columns sc ");
                    sqlStatementForNaturalKey.AppendLine("ON st.object_id = sc.object_id");
                    sqlStatementForNaturalKey.AppendLine("LEFT JOIN sys.extended_properties sep ");
                    sqlStatementForNaturalKey.AppendLine("ON st.object_id = sep.major_id");
                    sqlStatementForNaturalKey.AppendLine("AND sc.column_id = sep.minor_id");
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
                        sqlStatementForSourceAttribute.AppendLine("  AND COLUMN_NAME NOT IN ('" + eventDateTimeAttribute + "','" + sourceRowIdAttribute + "','" + cdcOperationAttribute + "','" + recordSourceAttribute + "','" + etlProcessIdAttribute + "','" + loadDateTimeAttribute + "','"+TeamConfigurationSettings.RecordChecksumAttribute+"')");
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
                                        if (attDataType == "bit")
                                        {
                                            sourceSqlStatement.AppendLine("   CONVERT(INT," + attName + ") AS " + attName +
                                                                          ",");
                                        }
                                        else if (attDataType == "float" || attDataType == "real" || attDataType == "money" ||
                                                 attDataType == "numeric")
                                        {
                                            if (attScale == "20")
                                            {
                                                stgView.AppendLine("   CONVERT(NUMERIC(38,20)," + attName + ") AS " +
                                                                   attName + ",");
                                            }
                                            else
                                            {
                                                stgView.AppendLine("   CONVERT(NUMERIC(38,0)," + attName + ") AS " +
                                                                   attName + ",");
                                            }
                                        }
                                        else if (attDataType == "text" || attDataType == "ntext")
                                        {
                                            stgView.AppendLine("   CONVERT(" + stringDataType + "(4000)," + attName + ") AS " + attName +
                                                               ",");
                                        }
                                        else if (attDataType == "datetime" || attDataType == "datetime2")
                                        {
                                            stgView.AppendLine("   CONVERT(DATETIME2(7)," + attName + ") AS " + attName +
                                                               ",");
                                        }
                                        else
                                        {
                                            stgView.AppendLine("   " + attName + " AS " + attName + ",");
                                        }
                                    }
                                    else
                                    {
                                        // This concerns all non-key attributes
                                        stgView.AppendLine("   " + attName + " AS " + attName + ",");
                                    }
                                }
                            }
                        }

                        // Hash on full record
                        stgView.AppendLine("   CONVERT(CHAR(32),HASHBYTES('MD5',");

                        if (sourceStructure != null)
                        {
                            foreach (DataRow attribute in sourceStructure.Rows)
                            {
                                stgView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + attribute["COLUMN_NAME"] +
                                                   ")),'NA')+'|'+");
                            }
                        }

                        stgView.Remove(stgView.Length - 3, 3);
                        stgView.AppendLine();
                        stgView.AppendLine("   ),2) AS "+TeamConfigurationSettings.RecordChecksumAttribute+",");

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
                                    sqlStatementForBusinessKeyHub.AppendLine(" c.STAGING_AREA_TABLE_ID,");
                                    sqlStatementForBusinessKeyHub.AppendLine(" c.STAGING_AREA_TABLE_NAME,");
                                    sqlStatementForBusinessKeyHub.AppendLine(" b.HUB_TABLE_ID,");                                
                                    sqlStatementForBusinessKeyHub.AppendLine(" b.HUB_TABLE_NAME,");
                                    sqlStatementForBusinessKeyHub.AppendLine("FROM MD_STG_HUB_XREF a ");
                                    sqlStatementForBusinessKeyHub.AppendLine("JOIN MD_HUB b ON a.HUB_TABLE_ID = b.HUB_TABLE_ID ");
                                    sqlStatementForBusinessKeyHub.AppendLine("JOIN MD_STG c on a.STAGING_AREA_TABLE_ID = c.STAGING_AREA_TABLE_ID ");
                                    sqlStatementForBusinessKeyHub.AppendLine("WHERE STAGING_AREA_TABLE_NAME = '" +targetTableName + "'");
                                    sqlStatementForBusinessKeyHub.AppendLine("AND b.HUB_TABLE_NAME = '" + hubTableName + "'");

                                    var businessKeyDataTable = GetDataTable(ref connOmd,sqlStatementForBusinessKeyHub.ToString());

                                    foreach (DataRow businessKey in businessKeyDataTable.Rows)
                                    {
                                        stagingAreaTableId = (int) businessKey["STAGING_AREA_TABLE_ID"];
                                        hubTableId = (int)businessKey["HUB_TABLE_ID"];
                                    }

                                    // **************************************************************************************
                                    // Retrieving the Business Keys attribute(s) as represented in the source for hashing
                                    // **************************************************************************************

                                    sqlStatementForBusinessKeyAttribute.AppendLine("SELECT comp.STAGING_AREA_TABLE_ID, comp.HUB_TABLE_ID, comp.COMPONENT_ID, COMPONENT_TYPE, COMPONENT_ELEMENT_TYPE, COMPONENT_ELEMENT_VALUE AS ATTRIBUTE_NAME");
                                    sqlStatementForBusinessKeyAttribute.AppendLine("FROM");
                                    sqlStatementForBusinessKeyAttribute.AppendLine("MD_BUSINESS_KEY_COMPONENT comp");
                                    sqlStatementForBusinessKeyAttribute.AppendLine("JOIN MD_BUSINESS_KEY_COMPONENT_PART elem ");
                                    sqlStatementForBusinessKeyAttribute.AppendLine("ON comp.COMPONENT_ID=elem.COMPONENT_ID");
                                    sqlStatementForBusinessKeyAttribute.AppendLine("AND elem.STAGING_AREA_TABLE_ID = comp.STAGING_AREA_TABLE_ID");
                                    sqlStatementForBusinessKeyAttribute.AppendLine("AND elem.HUB_TABLE_ID = comp.HUB_TABLE_ID");
                                    sqlStatementForBusinessKeyAttribute.AppendLine("WHERE comp.STAGING_AREA_TABLE_ID= '" + stagingAreaTableId + "'");
                                    sqlStatementForBusinessKeyAttribute.AppendLine("WHERE comp.HUB_TABLE_ID= '" + hubTableId + "'");
                                    sqlStatementForBusinessKeyAttribute.AppendLine("ORDER BY comp.COMPONENT_ORDER,COMPONENT_ELEMENT_ORDER");

                                    var businessKeyAttributeDataTable = GetDataTable(ref connOmd,sqlStatementForBusinessKeyAttribute.ToString());

                                    stgView.AppendLine("   CONVERT(CHAR(32),HASHBYTES('MD5',");

                                    foreach (DataRow attribute in businessKeyAttributeDataTable.Rows)
                                    {
                                        stgView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + attribute["ATTRIBUTE_NAME"] + ")),'NA')+'|'+");
                                    }

                                    stgView.Remove(stgView.Length - 3, 3);
                                    stgView.AppendLine();
                                    stgView.AppendLine("   ),2) AS HASH_" + hubTableName + ",");
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
                                    // **************************************************************************************
                                    // Retrieving the Business Keys attribute(s) as represented in the source for hashing
                                    // *************************************************************************************

                                    sqlStatementForLnkBusinessKeyAttribute.Clear();
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("SELECT ");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("  c.STAGING_AREA_TABLE_ID,");                                
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("  c.STAGING_AREA_TABLE_NAME,");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("  b.HUB_TABLE_ID,");                                
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("  b.HUB_TABLE_NAME,");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("  f.LINK_TABLE_ID,");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("  f.LINK_TABLE_NAME,");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("  COMPONENT_ELEMENT_VALUE");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("FROM MD_STG_HUB_XREF a ");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("JOIN MD_HUB b ON a.HUB_TABLE_ID=b.HUB_TABLE_ID ");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("JOIN MD_STG c on a.STAGING_AREA_TABLE_ID = c.STAGING_AREA_TABLE_ID ");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("JOIN MD_BUSINESS_KEY_COMPONENT d on d.STAGING_AREA_TABLE_ID = a.STAGING_AREA_TABLE_ID ");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("AND d.HUB_TABLE_ID = a.HUB_TABLE_ID ");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("JOIN MD_HUB_LINK_XREF e on b.HUB_TABLE_ID=e.HUB_TABLE_ID");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("JOIN MD_LINK f on e.LINK_TABLE_ID=f.LINK_TABLE_ID");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("JOIN MD_BUSINESS_KEY_COMPONENT_PART elem ON comp.COMPONENT_ID=elem.COMPONENT_ID");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("AND elem.STAGING_AREA_TABLE_ID = comp.STAGING_AREA_TABLE_ID");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("AND elem.HUB_TABLE_ID = comp.HUB_TABLE_ID");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("WHERE ");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("   STAGING_AREA_TABLE_NAME= '" +targetTableName + "'");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("AND f.LINK_TABLE_NAME= '" +lnkTableName + "'");
                                    sqlStatementForLnkBusinessKeyAttribute.AppendLine("ORDER BY HUB_TABLE_NAME, comp.COMPONENT_ID, COMPONENT_ELEMENT_ORDER");

                                    var lnkBusinessKeyAttributeDataTable = GetDataTable(ref connOmd,sqlStatementForLnkBusinessKeyAttribute.ToString());

                                    stgView.AppendLine("   CONVERT(CHAR(32),HASHBYTES('MD5',");

                                    foreach (DataRow attribute in lnkBusinessKeyAttributeDataTable.Rows)
                                    {
                                        stgView.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100)," + attribute["COMPONENT_ELEMENT_VALUE"] + ")),'NA')+'|'+");
                                    }

                                    stgView.Remove(stgView.Length - 3, 3);
                                    stgView.AppendLine();
                                    stgView.AppendLine("   ),2) AS HASH_" + lnkTableName + ",");
                                }
                                else
                                {
                                    errorCapture.AppendLine("There is no Link detected: processing " + lnkTableName + " as " +
                                                            lnk["TABLE_TYPE"] + " with source as " + targetTableName);
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
                            stgView.Append("            " + businessKey["STAGING_AREA_ATTRIBUTE_NAME"]);
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

                            stgView.AppendLine("  CASE WHEN STG_CTE.[" + mainBusinessKey + "] IS NULL THEN PSA_CTE.[" + TeamConfigurationSettings.RecordChecksumAttribute + "] ELSE STG_CTE.[" + TeamConfigurationSettings.RecordChecksumAttribute + "] COLLATE DATABASE_DEFAULT END AS [" + TeamConfigurationSettings.RecordChecksumAttribute + "], ");

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
                                                       TeamConfigurationSettings.RecordChecksumAttribute + " COLLATE DATABASE_DEFAULT != PSA_CTE." +
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
                    
                    using (var outfile = new StreamWriter(mydocpath + @"\VIEW_" + targetTableName + ".sql"))
                    {
                        outfile.Write(stgView.ToString());
                        outfile.Close();
                    }

                    if (checkBoxGenerateInDatabase.Checked)
                    {
                        connStg.ConnectionString = TeamConfigurationSettings.ConnectionStringStg;
                        GenerateInDatabase(connStg, stgView.ToString());
                    }

                    SetTextDebug(stgView.ToString());
                    SetTextDebug("\n");

                    SetTextStaging($"Processing Staging Area entity view for {targetTableName}\r\n");

                }
            }
            else
            {
                SetTextStaging("There was no metadata selected to create Staging Area views. Please check the metadata schema - are there any Staging Area tables selected?");
            }
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

        // Multithreading for changing version stuff
        delegate void SetVersionCallBack(int versionId);
        internal void SetVersion(int versionId)
        {
            if (trackBarVersioning.InvokeRequired)
            {
                var d = new SetVersionCallBack(SetVersion);
                Invoke(d, versionId);
            }
            else
            {
                InitialiseVersion();
                trackBarVersioning.Value = versionId;
            }
        }

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

        // This event handler cancels the backgroundworker, fired from Cancel button in AlertForm.
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (backgroundWorkerActivateMetadata.WorkerSupportsCancellation)
            {
                // Cancel the asynchronous operation.
                backgroundWorkerActivateMetadata.CancelAsync();
                // Close the AlertForm
                _alert.Close();
            }
        }

        // Multithreading for updating the user (Link Satellite form)
        delegate int GetVersionFromTrackBarCallBack();
        private int GetVersionFromTrackBar()
        {
            if (trackBarVersioning.InvokeRequired)
            {
                var d = new GetVersionFromTrackBarCallBack(GetVersionFromTrackBar);
                return Int32.Parse(Invoke(d).ToString());
            }
            else
            {
                return trackBarVersioning.Value;
            }
        }

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

            

            var errorLog = new StringBuilder();
            var errorCounter = new int();

            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
            var metaDataConnection = TeamConfigurationSettings.ConnectionStringOmd;

            // Get everything as local variables to reduce multithreading issues
            var stagingDatabase = '[' + TeamConfigurationSettings.StagingDatabaseName + ']';
            var integrationDatabase = '[' + TeamConfigurationSettings.IntegrationDatabaseName + ']';

            var linkedServer = TeamConfigurationSettings.LinkedServer;
            if (linkedServer != "")
            {
                linkedServer = '[' + linkedServer + "].";
            }

            var effectiveDateTimeAttribute = TeamConfigurationSettings.EnableAlternativeSatelliteLoadDateTimeAttribute == "True" ? TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute : TeamConfigurationSettings.LoadDateTimeAttribute;
            var currentRecordAttribute = TeamConfigurationSettings.CurrentRowAttribute;
            var eventDateTimeAtttribute = TeamConfigurationSettings.EventDateTimeAttribute;
            var recordSource = TeamConfigurationSettings.RecordSourceAttribute;
            var alternativeRecordSource = TeamConfigurationSettings.AlternativeRecordSourceAttribute;
            var sourceRowId = TeamConfigurationSettings.RowIdAttribute;
            var recordChecksum = TeamConfigurationSettings.RecordChecksumAttribute;
            var changeDataCaptureIndicator = TeamConfigurationSettings.ChangeDataCaptureAttribute;
            var hubAlternativeLdts = TeamConfigurationSettings.AlternativeLoadDateTimeAttribute;
            var etlProcessId = TeamConfigurationSettings.EtlProcessAttribute;
            var loadDateTimeStamp = TeamConfigurationSettings.LoadDateTimeAttribute;

            var stagingPrefix = TeamConfigurationSettings.StgTablePrefixValue;
            var hubTablePrefix = TeamConfigurationSettings.HubTablePrefixValue;
            var lnkTablePrefix = TeamConfigurationSettings.LinkTablePrefixValue;

            var satTablePrefix = TeamConfigurationSettings.SatTablePrefixValue;
            var lsatTablePrefix = TeamConfigurationSettings.LinkTablePrefixValue;

           
            if (TeamConfigurationSettings.TableNamingLocation == "Prefix") //tablePrefixLocation
            {
                stagingPrefix = stagingPrefix + '%';
                hubTablePrefix = hubTablePrefix + '%';
                lnkTablePrefix = lnkTablePrefix + '%';
                satTablePrefix = satTablePrefix + '%';
                lsatTablePrefix = lsatTablePrefix + '%';
            }
            else
            {
                stagingPrefix = '%' + stagingPrefix;
                hubTablePrefix = '%' + hubTablePrefix;
                lnkTablePrefix = '%' + lnkTablePrefix;
                satTablePrefix = '%' + satTablePrefix;
                lsatTablePrefix = '%' + lsatTablePrefix;
            }

            var dwhKeyIdentifier = TeamConfigurationSettings.DwhKeyIdentifier;

            //var keyPrefixLocation = keyPrefixRadiobutton.Checked;
            if (TeamConfigurationSettings.KeyNamingLocation == "Prefix") //keyPrefixLocation
            {
                dwhKeyIdentifier = dwhKeyIdentifier + '%';
            }
            else
            {
                dwhKeyIdentifier = '%' + dwhKeyIdentifier;
            }




            // Handling multithreading
            if (worker != null && worker.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                // Determine the version
                var versionId = GetVersionFromTrackBar();

                var versionMajorMinor = GetVersion(versionId);
                var majorVersion = versionMajorMinor.Key;
                var minorVersion = versionMajorMinor.Value;

                _alert.SetTextLogging("Commencing metadata preparation / activation for version " + majorVersion + "." + minorVersion + ".\r\n\r\n");

                // Alerting the user what kind of metadata is prepared
                _alert.SetTextLogging(checkBoxIgnoreVersion.Checked
                    ? "The 'ignore model version' option is selected. This means when possible the live database (tables and attributes) will be used in conjunction with the Data Vault metadata. In other words, the model versioning is ignored.\r\n\r\n"
                    : "Metadata is prepared using the selected version for both the Data Vault metadata as well as the model metadata.\r\n\r\n");

                # region Delete Metadata - 5%
                // 1. Deleting metadata
                _alert.SetTextLogging("Commencing removal of existing metadata.\r\n");
                var deleteStatement = new StringBuilder();

                deleteStatement.AppendLine("DELETE FROM [MD_STG_SAT_ATT_XREF];");
                deleteStatement.AppendLine("DELETE FROM dbo.MD_STG_LINK_ATT_XREF;");
                deleteStatement.AppendLine("DELETE FROM dbo.MD_STG_SAT_ATT_XREF;");
                deleteStatement.AppendLine("DELETE FROM dbo.MD_STG_LINK_XREF;");
                deleteStatement.AppendLine("DELETE FROM dbo.MD_STG_SAT_XREF;");
                deleteStatement.AppendLine("DELETE FROM dbo.MD_DRIVING_KEY_XREF");
                deleteStatement.AppendLine("DELETE FROM dbo.MD_HUB_LINK_XREF;");
                deleteStatement.AppendLine("DELETE FROM dbo.MD_SAT;");
                deleteStatement.AppendLine("DELETE FROM dbo.MD_BUSINESS_KEY_COMPONENT_PART;");
                deleteStatement.AppendLine("DELETE FROM dbo.MD_BUSINESS_KEY_COMPONENT;");
                deleteStatement.AppendLine("DELETE FROM dbo.MD_STG_HUB_XREF;");
                deleteStatement.AppendLine("DELETE FROM dbo.MD_ATT;");
                deleteStatement.AppendLine("DELETE FROM dbo.MD_STG;");
                deleteStatement.AppendLine("DELETE FROM dbo.MD_HUB;");
                deleteStatement.AppendLine("DELETE FROM dbo.MD_LINK;");

                using (var connectionVersion = new SqlConnection(metaDataConnection))
                {
                    var commandVersion = new SqlCommand(deleteStatement.ToString(), connectionVersion);

                    try
                    {
                        connectionVersion.Open();
                        commandVersion.ExecuteNonQuery();

                        worker?.ReportProgress(5);
                        _alert.SetTextLogging("Removal of existing metadata completed.\r\n");
                    }
                    catch (Exception ex)
                    {
                        errorCounter++;
                        _alert.SetTextLogging("An issue has occured during removal of old metadata. Please check the Error Log for more details.\r\n");
                        errorLog.AppendLine("\r\nAn issue has occured during removal of old metadata: \r\n\r\n" + ex);
                    }
                }
                # endregion



                # region Prepare Staging Area - 10%
                // 2. Prepare STG
                _alert.SetTextLogging("\r\n");
                _alert.SetTextLogging("Commencing preparing the Staging Area metadata.\r\n");

                try
                {
                    var prepareStgStatement = new StringBuilder();
                    var stgCounter = 1;

                    prepareStgStatement.AppendLine("SELECT DISTINCT STAGING_AREA_TABLE");
                    prepareStgStatement.AppendLine("FROM MD_TABLE_MAPPING");
                    prepareStgStatement.AppendLine("WHERE STAGING_AREA_TABLE LIKE '" + stagingPrefix + "'");
                    prepareStgStatement.AppendLine("AND VERSION_ID = " + versionId);
                    prepareStgStatement.AppendLine("ORDER BY STAGING_AREA_TABLE");

                    var listStaging = GetDataTable(ref connOmd, prepareStgStatement.ToString());

                    foreach (DataRow tableName in listStaging.Rows)
                    {
                        using (var connection = new SqlConnection(metaDataConnection))
                        {
                            _alert.SetTextLogging("--> " + tableName["STAGING_AREA_TABLE"] + "\r\n");

                            var insertStgStatemeent = new StringBuilder();

                            insertStgStatemeent.AppendLine("INSERT INTO [MD_STG]");
                            insertStgStatemeent.AppendLine("([STAGING_AREA_TABLE_NAME],[STAGING_AREA_TABLE_ID])");
                            insertStgStatemeent.AppendLine("VALUES ('" + tableName["STAGING_AREA_TABLE"] + "'," + stgCounter + ")");

                            var command = new SqlCommand(insertStgStatemeent.ToString(), connection);

                            try
                            {
                                connection.Open();
                                command.ExecuteNonQuery();
                                stgCounter++;
                            }
                            catch (Exception ex)
                            {
                                errorCounter++;
                                _alert.SetTextLogging("An issue has occured during preparation of the Staging Area. Please check the Error Log for more details.\r\n");
                                errorLog.AppendLine("\r\nAn issue has occured during preparation of the Staging Area: \r\n\r\n" + ex);
                            }
                        }
                    }

                    worker?.ReportProgress(10);
                    _alert.SetTextLogging("Preparation of the Staging Area completed.\r\n");
                }
                catch (Exception ex)
                {
                    errorCounter++;
                    _alert.SetTextLogging("An issue has occured during preparation of the Staging Area. Please check the Error Log for more details.\r\n");
                    errorLog.AppendLine("\r\nAn issue has occured during preparation of the Staging Area: \r\n\r\n" + ex);
                }

                #endregion

                # region Prepare Hubs - 15%
                //3. Prepare Hubs
                _alert.SetTextLogging("\r\n");
                _alert.SetTextLogging("Commencing preparing the Hub metadata.\r\n");

                try
                {
                    var prepareHubStatement = new StringBuilder();
                    var hubCounter = 1;

                    prepareHubStatement.AppendLine("SELECT 'Not applicable' AS HUB_TABLE_NAME");
                    prepareHubStatement.AppendLine("UNION");
                    prepareHubStatement.AppendLine("SELECT DISTINCT INTEGRATION_AREA_TABLE AS HUB_TABLE_NAME");
                    prepareHubStatement.AppendLine("FROM MD_TABLE_MAPPING");
                    prepareHubStatement.AppendLine("WHERE INTEGRATION_AREA_TABLE LIKE '" + hubTablePrefix + "'");
                    prepareHubStatement.AppendLine("AND VERSION_ID = " + versionId);

                    var listHub = GetDataTable(ref connOmd, prepareHubStatement.ToString());

                    foreach (DataRow tableName in listHub.Rows)
                    {
                        using (var connection = new SqlConnection(metaDataConnection))
                        {
                            _alert.SetTextLogging("--> " + tableName["HUB_TABLE_NAME"] + "\r\n");

                            var insertHubStatemeent = new StringBuilder();

                            insertHubStatemeent.AppendLine("INSERT INTO [MD_HUB]");
                            insertHubStatemeent.AppendLine("([HUB_TABLE_NAME],[HUB_TABLE_ID])");
                            insertHubStatemeent.AppendLine("VALUES ('" + tableName["HUB_TABLE_NAME"] + "'," + hubCounter + ")");

                            var command = new SqlCommand(insertHubStatemeent.ToString(), connection);

                            try
                            {
                                connection.Open();
                                command.ExecuteNonQuery();
                                hubCounter++;
                            }
                            catch (Exception ex)
                            {
                                errorCounter++;
                                _alert.SetTextLogging("An issue has occured during preparation of the Hubs. Please check the Error Log for more details.\r\n");
                                errorLog.AppendLine("\r\nAn issue has occured during preparation of the Hubs: \r\n\r\n" + ex);
                            }
                        }
                    }

                    worker?.ReportProgress(15);
                    _alert.SetTextLogging("Preparation of the Hub metadata completed.\r\n");
                }
                catch (Exception ex)
                {
                    errorCounter++;
                    _alert.SetTextLogging("An issue has occured during preparation of the Hubs. Please check the Error Log for more details.\r\n");
                    errorLog.AppendLine("\r\nAn issue has occured during preparation of the Hubs: \r\n\r\n" + ex);
                }
                #endregion



                #region Prepare Links - 20%
                //4. Prepare links
                _alert.SetTextLogging("\r\n");
                _alert.SetTextLogging("Commencing preparing the Link metadata.\r\n");

                try
                {
                    var prepareLinkStatement = new StringBuilder();
                    var linkCounter = 1;

                    prepareLinkStatement.AppendLine("SELECT 'Not applicable' AS LINK_TABLE_NAME");
                    prepareLinkStatement.AppendLine("UNION");
                    prepareLinkStatement.AppendLine("SELECT DISTINCT INTEGRATION_AREA_TABLE AS LINK_TABLE_NAME");
                    prepareLinkStatement.AppendLine("FROM MD_TABLE_MAPPING");
                    prepareLinkStatement.AppendLine("WHERE INTEGRATION_AREA_TABLE LIKE '" + lnkTablePrefix + "'");
                    prepareLinkStatement.AppendLine("AND VERSION_ID = " + versionId);

                    var listLink = GetDataTable(ref connOmd, prepareLinkStatement.ToString());

                    foreach (DataRow tableName in listLink.Rows)
                    {
                        using (var connection = new SqlConnection(metaDataConnection))
                        {
                            _alert.SetTextLogging("--> " + tableName["LINK_TABLE_NAME"] + "\r\n");

                            var insertLinkStatement = new StringBuilder();

                            insertLinkStatement.AppendLine("INSERT INTO [MD_LINK]");
                            insertLinkStatement.AppendLine("([LINK_TABLE_NAME],[LINK_TABLE_ID])");
                            insertLinkStatement.AppendLine("VALUES ('" + tableName["LINK_TABLE_NAME"] + "'," + linkCounter + ")");

                            var command = new SqlCommand(insertLinkStatement.ToString(), connection);

                            try
                            {
                                connection.Open();
                                command.ExecuteNonQuery();
                                linkCounter++;
                            }
                            catch (Exception ex)
                            {
                                errorCounter++;
                                _alert.SetTextLogging("An issue has occured during preparation of the Links. Please check the Error Log for more details.\r\n");
                                errorLog.AppendLine("\r\nAn issue has occured during preparation of the Links: \r\n\r\n" + ex);
                            }
                        }
                    }

                    if (worker != null) worker.ReportProgress(20);
                    _alert.SetTextLogging("Preparation of the Link metadata completed.\r\n");
                }
                catch (Exception ex)
                {
                    errorCounter++;
                    _alert.SetTextLogging("An issue has occured during preparation of the Links. Please check the Error Log for more details.\r\n");
                    errorLog.AppendLine("\r\nAn issue has occured during preparation of the Links: \r\n\r\n" + ex);
                }
                #endregion



                #region Prepare Satellites - 24%
                //5.1 Prepare Satellites
                _alert.SetTextLogging("\r\n");
                _alert.SetTextLogging("Commencing preparing the Satellite metadata.\r\n");
                var satCounter = 1;

                try
                {
                    var prepareSatStatement = new StringBuilder();

                    prepareSatStatement.AppendLine("SELECT DISTINCT");
                    prepareSatStatement.AppendLine("       spec.INTEGRATION_AREA_TABLE AS SATELLITE_TABLE_NAME,");
                    prepareSatStatement.AppendLine("       hubkeysub.HUB_TABLE_ID,");
                    prepareSatStatement.AppendLine("       'Normal' AS SATELLITE_TYPE,");
                    prepareSatStatement.AppendLine("       (SELECT LINK_TABLE_ID FROM MD_LINK WHERE LINK_TABLE_NAME='Not applicable') AS LINK_TABLE_ID -- No link for normal Satellites ");
                    prepareSatStatement.AppendLine("FROM MD_TABLE_MAPPING spec ");
                    prepareSatStatement.AppendLine("LEFT OUTER JOIN ");
                    prepareSatStatement.AppendLine("( ");
                    prepareSatStatement.AppendLine("       SELECT DISTINCT INTEGRATION_AREA_TABLE, hub.HUB_TABLE_ID, STAGING_AREA_TABLE, BUSINESS_KEY_ATTRIBUTE ");
                    prepareSatStatement.AppendLine("       FROM MD_TABLE_MAPPING spec2 ");
                    prepareSatStatement.AppendLine("       LEFT OUTER JOIN -- Join in the Hub ID from the MD table ");
                    prepareSatStatement.AppendLine("             MD_HUB hub ON hub.HUB_TABLE_NAME=spec2.INTEGRATION_AREA_TABLE ");
                    prepareSatStatement.AppendLine("    WHERE INTEGRATION_AREA_TABLE LIKE '" + hubTablePrefix + "'");
                    prepareSatStatement.AppendLine("    AND VERSION_ID = " + versionId);
                    prepareSatStatement.AppendLine(") hubkeysub ");
                    prepareSatStatement.AppendLine("       ON spec.STAGING_AREA_TABLE=hubkeysub.STAGING_AREA_TABLE ");
                    prepareSatStatement.AppendLine("       AND replace(spec.BUSINESS_KEY_ATTRIBUTE,' ','')=replace(hubkeysub.BUSINESS_KEY_ATTRIBUTE,' ','') ");
                    prepareSatStatement.AppendLine("WHERE spec.INTEGRATION_AREA_TABLE LIKE '" + satTablePrefix + "'");
                    prepareSatStatement.AppendLine("AND VERSION_ID = " + versionId);

                    var listSat = GetDataTable(ref connOmd, prepareSatStatement.ToString());

                    foreach (DataRow tableName in listSat.Rows)
                    {
                        using (var connection = new SqlConnection(metaDataConnection))
                        {
                            _alert.SetTextLogging("--> " + tableName["SATELLITE_TABLE_NAME"] + "\r\n");

                            var insertSatStatement = new StringBuilder();

                            insertSatStatement.AppendLine("INSERT INTO [MD_SAT]");
                            insertSatStatement.AppendLine("([SATELLITE_TABLE_NAME],[SATELLITE_TABLE_ID], [SATELLITE_TYPE], [HUB_TABLE_ID], [LINK_TABLE_ID])");
                            insertSatStatement.AppendLine("VALUES ('" + tableName["SATELLITE_TABLE_NAME"] + "'," + satCounter + ",'" + tableName["SATELLITE_TYPE"] + "'," + tableName["HUB_TABLE_ID"] + "," + tableName["LINK_TABLE_ID"] + ")");

                            var command = new SqlCommand(insertSatStatement.ToString(), connection);

                            try
                            {
                                connection.Open();
                                command.ExecuteNonQuery();
                                satCounter++;
                            }
                            catch (Exception ex)
                            {
                                errorCounter++;
                                _alert.SetTextLogging("An issue has occured during preparation of the Satellites. Please check the Error Log for more details.\r\n");
                                errorLog.AppendLine("\r\nAn issue has occured during preparation of the Satellites: \r\n\r\n" + ex);
                            }
                        }
                    }

                    if (worker != null) worker.ReportProgress(24);
                    _alert.SetTextLogging("Preparation of the Satellite metadata completed.\r\n");
                }
                catch (Exception ex)
                {
                    errorCounter++;
                    _alert.SetTextLogging("An issue has occured during preparation of the Satellites. Please check the Error Log for more details.\r\n");
                    errorLog.AppendLine("\r\nAn issue has occured during preparation of the Satellites: \r\n\r\n" + ex);
                }
                #endregion


                #region Prepare Link Satellites - 28%
                //5.2 Prepare Satellites
                _alert.SetTextLogging("\r\n");
                _alert.SetTextLogging("Commencing preparing the Link Satellite metadata.\r\n");
                //satCounter = 1;

                try
                {
                    var prepareSatStatement = new StringBuilder();

                    prepareSatStatement.AppendLine("SELECT DISTINCT");
                    prepareSatStatement.AppendLine("       spec.INTEGRATION_AREA_TABLE AS SATELLITE_TABLE_NAME, ");
                    prepareSatStatement.AppendLine("       (SELECT HUB_TABLE_ID FROM MD_HUB WHERE HUB_TABLE_NAME='Not applicable') AS HUB_TABLE_ID, -- No Hub for Link Satellites");
                    prepareSatStatement.AppendLine("       'Link Satellite' AS SATELLITE_TYPE,");
                    prepareSatStatement.AppendLine("       lnkkeysub.LINK_TABLE_ID");
                    prepareSatStatement.AppendLine("FROM MD_TABLE_MAPPING spec");
                    prepareSatStatement.AppendLine("LEFT OUTER JOIN  -- Get the Link ID that belongs to this LSAT");
                    prepareSatStatement.AppendLine("(");
                    prepareSatStatement.AppendLine("       SELECT DISTINCT ");
                    prepareSatStatement.AppendLine("             INTEGRATION_AREA_TABLE AS LINK_TABLE_NAME,");
                    prepareSatStatement.AppendLine("             STAGING_AREA_TABLE,");
                    prepareSatStatement.AppendLine("             BUSINESS_KEY_ATTRIBUTE,");
                    prepareSatStatement.AppendLine("       lnk.LINK_TABLE_ID");
                    prepareSatStatement.AppendLine("       FROM MD_TABLE_MAPPING spec2");
                    prepareSatStatement.AppendLine("       LEFT OUTER JOIN -- Join in the Link ID from the MD table");
                    prepareSatStatement.AppendLine("             MD_LINK lnk ON lnk.LINK_TABLE_NAME=spec2.INTEGRATION_AREA_TABLE");
                    prepareSatStatement.AppendLine("       WHERE INTEGRATION_AREA_TABLE LIKE '" + lnkTablePrefix + "' ");
                    prepareSatStatement.AppendLine("       AND VERSION_ID = " + versionId);
                    prepareSatStatement.AppendLine(") lnkkeysub");
                    prepareSatStatement.AppendLine("    ON spec.STAGING_AREA_TABLE=lnkkeysub.STAGING_AREA_TABLE -- Only the combination of Link table and Business key can belong to the LSAT");
                    prepareSatStatement.AppendLine("   AND spec.BUSINESS_KEY_ATTRIBUTE=lnkkeysub.BUSINESS_KEY_ATTRIBUTE");
                    prepareSatStatement.AppendLine("");
                    prepareSatStatement.AppendLine("-- Only select Link Satellites as the base / driving table (spec alias)");
                    prepareSatStatement.AppendLine("WHERE spec.INTEGRATION_AREA_TABLE LIKE '" + lsatTablePrefix + "'");


                    var listSat = GetDataTable(ref connOmd, prepareSatStatement.ToString());

                    foreach (DataRow tableName in listSat.Rows)
                    {
                        using (var connection = new SqlConnection(metaDataConnection))
                        {
                            _alert.SetTextLogging("--> " + tableName["SATELLITE_TABLE_NAME"] + "\r\n");

                            var insertSatStatement = new StringBuilder();

                            insertSatStatement.AppendLine("INSERT INTO [MD_SAT]");
                            insertSatStatement.AppendLine("([SATELLITE_TABLE_NAME],[SATELLITE_TABLE_ID], [SATELLITE_TYPE], [HUB_TABLE_ID], [LINK_TABLE_ID])");
                            insertSatStatement.AppendLine("VALUES ('" + tableName["SATELLITE_TABLE_NAME"] + "'," + satCounter + ",'" + tableName["SATELLITE_TYPE"] + "'," + tableName["HUB_TABLE_ID"] + "," + tableName["LINK_TABLE_ID"] + ")");

                            var command = new SqlCommand(insertSatStatement.ToString(), connection);

                            try
                            {
                                connection.Open();
                                command.ExecuteNonQuery();
                                satCounter++;
                            }
                            catch (Exception ex)
                            {
                                errorCounter++;
                                _alert.SetTextLogging("An issue has occured during preparation of the Link Satellites. Please check the Error Log for more details.\r\n");
                                errorLog.AppendLine("\r\nAn issue has occured during preparation of the Link Satellites: \r\n\r\n" + ex);
                            }
                        }
                    }

                    worker.ReportProgress(28);
                    _alert.SetTextLogging("Preparation of the Link Satellite metadata completed.\r\n");
                }
                catch (Exception ex)
                {
                    errorCounter++;
                    _alert.SetTextLogging("An issue has occured during preparation of the Link Satellites. Please check the Error Log for more details.\r\n");
                    errorLog.AppendLine("\r\nAn issue has occured during preparation of the Link Satellites: \r\n\r\n" + ex);
                }
                #endregion



                #region Prepare STG / SAT Xref - 28%
                //5.3 Prepare STG / Sat XREF
                _alert.SetTextLogging("\r\n");
                _alert.SetTextLogging("Commencing preparing the relationship between (Link) Satellites and the Staging Area tables.\r\n");
                satCounter = 1;

                try
                {
                    var prepareSatXrefStatement = new StringBuilder();

                    prepareSatXrefStatement.AppendLine("SELECT");
                    prepareSatXrefStatement.AppendLine("       sat.SATELLITE_TABLE_ID,");
                    prepareSatXrefStatement.AppendLine("	   sat.SATELLITE_TABLE_NAME,");
                    prepareSatXrefStatement.AppendLine("       stg.STAGING_AREA_TABLE_ID, ");
                    prepareSatXrefStatement.AppendLine("	   stg.STAGING_AREA_TABLE_NAME,");
                    prepareSatXrefStatement.AppendLine("	   spec.BUSINESS_KEY_ATTRIBUTE,");
                    prepareSatXrefStatement.AppendLine("       spec.FILTER_CRITERIA");
                    prepareSatXrefStatement.AppendLine("FROM MD_TABLE_MAPPING spec");
                    prepareSatXrefStatement.AppendLine("LEFT OUTER JOIN -- Join in the Staging_Area_ID from the MD_STG table");
                    prepareSatXrefStatement.AppendLine("       MD_STG stg ON stg.STAGING_AREA_TABLE_NAME=spec.STAGING_AREA_TABLE");
                    prepareSatXrefStatement.AppendLine("LEFT OUTER JOIN -- Join in the Satellite_ID from the MD_SAT table");
                    prepareSatXrefStatement.AppendLine("       MD_SAT sat ON sat.SATELLITE_TABLE_NAME=spec.INTEGRATION_AREA_TABLE");
                    prepareSatXrefStatement.AppendLine("WHERE spec.INTEGRATION_AREA_TABLE LIKE '" + satTablePrefix + "' ");
                    prepareSatXrefStatement.AppendLine("AND VERSION_ID = " + versionId);
                    prepareSatXrefStatement.AppendLine("UNION");
                    prepareSatXrefStatement.AppendLine("SELECT");
                    prepareSatXrefStatement.AppendLine("       sat.SATELLITE_TABLE_ID,");
                    prepareSatXrefStatement.AppendLine("	   sat.SATELLITE_TABLE_NAME,");
                    prepareSatXrefStatement.AppendLine("       stg.STAGING_AREA_TABLE_ID, ");
                    prepareSatXrefStatement.AppendLine("	   stg.STAGING_AREA_TABLE_NAME,");
                    prepareSatXrefStatement.AppendLine("	   spec.BUSINESS_KEY_ATTRIBUTE,");
                    prepareSatXrefStatement.AppendLine("       spec.FILTER_CRITERIA");
                    prepareSatXrefStatement.AppendLine("FROM MD_TABLE_MAPPING spec");
                    prepareSatXrefStatement.AppendLine("LEFT OUTER JOIN -- Join in the Staging_Area_ID from the MD_STG table");
                    prepareSatXrefStatement.AppendLine("       MD_STG stg ON stg.STAGING_AREA_TABLE_NAME=spec.STAGING_AREA_TABLE");
                    prepareSatXrefStatement.AppendLine("LEFT OUTER JOIN -- Join in the Satellite_ID from the MD_SAT table");
                    prepareSatXrefStatement.AppendLine("       MD_SAT sat ON sat.SATELLITE_TABLE_NAME=spec.INTEGRATION_AREA_TABLE");
                    prepareSatXrefStatement.AppendLine("WHERE spec.INTEGRATION_AREA_TABLE LIKE '" + lsatTablePrefix + "' ");
                    prepareSatXrefStatement.AppendLine("AND VERSION_ID = " + versionId);


                    var listSat = GetDataTable(ref connOmd, prepareSatXrefStatement.ToString());

                    foreach (DataRow tableName in listSat.Rows)
                    {
                        using (var connection = new SqlConnection(metaDataConnection))
                        {
                            _alert.SetTextLogging("-->  Processing the " + tableName["STAGING_AREA_TABLE_NAME"] + " / " + tableName["SATELLITE_TABLE_NAME"] + " relationship\r\n");

                            var insertSatStatement = new StringBuilder();
                            var filterCriterion = tableName["FILTER_CRITERIA"].ToString();
                            filterCriterion = filterCriterion.Replace("'", "''");

                            var businessKeyDefinition = tableName["BUSINESS_KEY_ATTRIBUTE"].ToString();
                            businessKeyDefinition = businessKeyDefinition.Replace("'", "''");

                            insertSatStatement.AppendLine("INSERT INTO [MD_STG_SAT_XREF]");
                            insertSatStatement.AppendLine("([SATELLITE_TABLE_ID], [STAGING_AREA_TABLE_ID], [BUSINESS_KEY_DEFINITION], [FILTER_CRITERIA])");
                            insertSatStatement.AppendLine("VALUES ('" + tableName["SATELLITE_TABLE_ID"] + "','" + tableName["STAGING_AREA_TABLE_ID"] + "','" + businessKeyDefinition + "','" + filterCriterion + "')");

                            var command = new SqlCommand(insertSatStatement.ToString(), connection);

                            try
                            {
                                connection.Open();
                                command.ExecuteNonQuery();
                                satCounter++;
                            }
                            catch (Exception ex)
                            {
                                errorCounter++;
                                _alert.SetTextLogging("An issue has occured during preparation of the relationship between the Staging Area and the Satellite. Please check the Error Log for more details.\r\n");
                                errorLog.AppendLine("\r\nAn issue has occured during preparation of the Staging / Satellite XREF: \r\n\r\n" + ex);
                            }
                        }
                    }

                    worker.ReportProgress(28);
                    _alert.SetTextLogging("Preparation of the Staging / Satellite XREF metadata completed.\r\n");
                }
                catch (Exception ex)
                {
                    errorCounter++;
                    _alert.SetTextLogging("An issue has occured during preparation of the relationship between the Staging Area and the Satellite. Please check the Error Log for more details.\r\n");
                    errorLog.AppendLine("\r\nAn issue has occured during preparation of the Staging / Satellite XREF: \r\n\r\n" + ex);
                }

                #endregion



                #region Staging / Hub relationship - 30%
                //6. Prepare STG / HUB xref
                _alert.SetTextLogging("\r\n");
                _alert.SetTextLogging("Commencing preparing the relationship between Staging Area and Hubs.\r\n");

                try
                {
                    var prepareStgHubXrefStatement = new StringBuilder();

                    prepareStgHubXrefStatement.AppendLine("SELECT");
                    prepareStgHubXrefStatement.AppendLine("    HUB_TABLE_ID,");
                    prepareStgHubXrefStatement.AppendLine("	   HUB_TABLE_NAME,");
                    prepareStgHubXrefStatement.AppendLine("    STAGING_AREA_TABLE_ID,");
                    prepareStgHubXrefStatement.AppendLine("	   STAGING_AREA_TABLE_NAME,");
                    prepareStgHubXrefStatement.AppendLine("	   BUSINESS_KEY_ATTRIBUTE,");
                    prepareStgHubXrefStatement.AppendLine("    FILTER_CRITERIA");
                    prepareStgHubXrefStatement.AppendLine("FROM");
                    prepareStgHubXrefStatement.AppendLine("       (      ");
                    prepareStgHubXrefStatement.AppendLine("              SELECT DISTINCT ");
                    prepareStgHubXrefStatement.AppendLine("                     STAGING_AREA_TABLE,");
                    prepareStgHubXrefStatement.AppendLine("                     INTEGRATION_AREA_TABLE,");
                    prepareStgHubXrefStatement.AppendLine("					    BUSINESS_KEY_ATTRIBUTE,");
                    prepareStgHubXrefStatement.AppendLine("                     FILTER_CRITERIA");
                    prepareStgHubXrefStatement.AppendLine("              FROM   MD_TABLE_MAPPING");
                    prepareStgHubXrefStatement.AppendLine("              WHERE ");
                    prepareStgHubXrefStatement.AppendLine("                     INTEGRATION_AREA_TABLE LIKE '" + hubTablePrefix + "'");
                    prepareStgHubXrefStatement.AppendLine("              AND VERSION_ID = " + versionId);
                    prepareStgHubXrefStatement.AppendLine("       ) hub");
                    prepareStgHubXrefStatement.AppendLine("LEFT OUTER JOIN");
                    prepareStgHubXrefStatement.AppendLine("       ( ");
                    prepareStgHubXrefStatement.AppendLine("              SELECT STAGING_AREA_TABLE_ID, STAGING_AREA_TABLE_NAME");
                    prepareStgHubXrefStatement.AppendLine("              FROM MD_STG");
                    prepareStgHubXrefStatement.AppendLine("       ) stgsub");
                    prepareStgHubXrefStatement.AppendLine("ON hub.STAGING_AREA_TABLE=stgsub.STAGING_AREA_TABLE_NAME");
                    prepareStgHubXrefStatement.AppendLine("LEFT OUTER JOIN");
                    prepareStgHubXrefStatement.AppendLine("       ( ");
                    prepareStgHubXrefStatement.AppendLine("              SELECT HUB_TABLE_ID, HUB_TABLE_NAME");
                    prepareStgHubXrefStatement.AppendLine("              FROM MD_HUB");
                    prepareStgHubXrefStatement.AppendLine("       ) hubsub");
                    prepareStgHubXrefStatement.AppendLine("ON hub.INTEGRATION_AREA_TABLE=hubsub.HUB_TABLE_NAME");


                    var listXref = GetDataTable(ref connOmd, prepareStgHubXrefStatement.ToString());

                    foreach (DataRow tableName in listXref.Rows)
                    {
                        using (var connection = new SqlConnection(metaDataConnection))
                        {
                            _alert.SetTextLogging("-->  Processing the " + tableName["STAGING_AREA_TABLE_NAME"] + " / " + tableName["HUB_TABLE_NAME"] + " relationship\r\n");

                            var insertXrefStatement = new StringBuilder();
                            var filterCriterion = tableName["FILTER_CRITERIA"].ToString();
                            filterCriterion = filterCriterion.Replace("'", "''");

                            var businessKeyDefinition = tableName["BUSINESS_KEY_ATTRIBUTE"].ToString();
                            businessKeyDefinition = businessKeyDefinition.Replace("'", "''");

                            insertXrefStatement.AppendLine("INSERT INTO [MD_STG_HUB_XREF]");
                            insertXrefStatement.AppendLine("([HUB_TABLE_ID], [STAGING_AREA_TABLE_ID], [BUSINESS_KEY_DEFINITION], [FILTER_CRITERIA])");
                            insertXrefStatement.AppendLine("VALUES ('" + tableName["HUB_TABLE_ID"] + "','" + tableName["STAGING_AREA_TABLE_ID"] + "','" + businessKeyDefinition + "','" + filterCriterion + "')");

                            var command = new SqlCommand(insertXrefStatement.ToString(), connection);

                            try
                            {
                                connection.Open();
                                command.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                errorCounter++;
                                _alert.SetTextLogging("An issue has occured during preparation of the relationship between the Staging Area and the Hubs. Please check the Error Log for more details.\r\n");
                                errorLog.AppendLine("\r\nAn issue has occured during preparation of the Staging / Hub XREF: \r\n\r\n" + ex);
                            }
                        }
                    }

                    worker.ReportProgress(30);
                    _alert.SetTextLogging("Preparation of the relationship between Staging Area and Hubs completed.\r\n");
                }
                catch (Exception ex)
                {
                    errorCounter++;
                    _alert.SetTextLogging("An issue has occured during preparation of the relationship between the Staging Area and the Hubs. Please check the Error Log for more details.\r\n");
                    errorLog.AppendLine("\r\nAn issue has occured during preparation of the Staging / Hub XREF: \r\n\r\n" + ex);
                }
                #endregion



                #region Prepare attributes - 40%
                //7. Prepare Attributes
                _alert.SetTextLogging("\r\n");

                try
                {
                    var prepareAttStatement = new StringBuilder();
                    var attCounter = 1;

                    // Insert Not Applicable attribute for FKs
                    using (var connection = new SqlConnection(metaDataConnection))
                    {
                        var insertAttDummyStatement = new StringBuilder();

                        insertAttDummyStatement.AppendLine("INSERT INTO [MD_ATT]");
                        insertAttDummyStatement.AppendLine("([ATTRIBUTE_ID], [ATTRIBUTE_NAME])");
                        insertAttDummyStatement.AppendLine("VALUES ('-1','Not applicable')");

                        var command = new SqlCommand(insertAttDummyStatement.ToString(), connection);

                        try
                        {
                            connection.Open();
                            command.ExecuteNonQuery();
                            attCounter++;
                        }
                        catch (Exception ex)
                        {
                            errorCounter++;
                            _alert.SetTextLogging(
                                "An issue has occured during preparation of the attribute metadata. Please check the Error Log for more details.\r\n");
                            errorLog.AppendLine("\r\nAn issue has occured during preparation of attribute metadata: \r\n\r\n" + ex);
                        }
                    }

                    // Regular processing
                    if (checkBoxIgnoreVersion.Checked) // Read from live databasse
                    {
                        _alert.SetTextLogging("Commencing preparing the attributes directly from the database.\r\n");
                        prepareAttStatement.AppendLine("SELECT DISTINCT(COLUMN_NAME) AS COLUMN_NAME FROM");
                        prepareAttStatement.AppendLine("(");
                        prepareAttStatement.AppendLine("	SELECT COLUMN_NAME FROM " + linkedServer + stagingDatabase + ".INFORMATION_SCHEMA.COLUMNS");
                        prepareAttStatement.AppendLine("	UNION");
                        prepareAttStatement.AppendLine("	SELECT COLUMN_NAME FROM " + linkedServer + integrationDatabase + ".INFORMATION_SCHEMA.COLUMNS");
                        prepareAttStatement.AppendLine(") sub1");
                    }
                    else
                    {
                        _alert.SetTextLogging("Commencing preparing the attributes from the metadata.\r\n");
                        prepareAttStatement.AppendLine("SELECT DISTINCT COLUMN_NAME FROM MD_VERSION_ATTRIBUTE");
                        prepareAttStatement.AppendLine("WHERE VERSION_ID = " + versionId);
                    }

                    var listAtt = GetDataTable(ref connOmd, prepareAttStatement.ToString());

                    if (listAtt.Rows.Count == 0)
                    {
                        _alert.SetTextLogging("-->  No attributes were found in the metadata, did you reverse-engineer the model?\r\n");
                    }
                    else
                    {
                        foreach (DataRow tableName in listAtt.Rows)
                        {
                            using (var connection = new SqlConnection(metaDataConnection))
                            {
                                //_alert.SetTextLogging("-->  Processing " + tableName["COLUMN_NAME"] + ".\r\n");

                                var insertAttStatement = new StringBuilder();

                                insertAttStatement.AppendLine("INSERT INTO [MD_ATT]");
                                insertAttStatement.AppendLine("([ATTRIBUTE_ID], [ATTRIBUTE_NAME])");
                                insertAttStatement.AppendLine("VALUES (" + attCounter + ",'" + tableName["COLUMN_NAME"] + "')");

                                var command = new SqlCommand(insertAttStatement.ToString(), connection);

                                try
                                {
                                    connection.Open();
                                    command.ExecuteNonQuery();
                                    attCounter++;
                                }
                                catch (Exception ex)
                                {
                                    errorCounter++;
                                    _alert.SetTextLogging("An issue has occured during preparation of the attribute metadata. Please check the Error Log for more details.\r\n");
                                    errorLog.AppendLine("\r\nAn issue has occured during preparation of attribute metadata: \r\n\r\n" + ex);
                                }
                            }

                        }
                        _alert.SetTextLogging("-->  Processing " + attCounter + " attributes.\r\n");
                    }
                    worker.ReportProgress(40);

                    _alert.SetTextLogging("Preparation of the attributes completed.\r\n");
                }
                catch (Exception ex)
                {
                    errorCounter++;
                    _alert.SetTextLogging("An issue has occured during preparation of the attribute metadata. Please check the Error Log for more details.\r\n");
                    errorLog.AppendLine("\r\nAn issue has occured during preparation of attribute metadata: \r\n\r\n" + ex);
                }

                #endregion

                #region Business Key - 50%
                //8. Understanding the Business Key (MD_BUSINESS_KEY_COMPONENT)

                _alert.SetTextLogging("\r\n");
                _alert.SetTextLogging("Commencing the definition of the Business Key.\r\n");

                try
                {
                    var prepareKeyStatement = new StringBuilder();

                    prepareKeyStatement.AppendLine("SELECT ");
                    prepareKeyStatement.AppendLine("  STAGING_AREA_TABLE_ID,");
                    prepareKeyStatement.AppendLine("  STAGING_AREA_TABLE_NAME,");
                    prepareKeyStatement.AppendLine("  HUB_TABLE_ID,");
                    prepareKeyStatement.AppendLine("  HUB_TABLE_NAME,");
                    prepareKeyStatement.AppendLine("  BUSINESS_KEY_ATTRIBUTE,");
                    prepareKeyStatement.AppendLine("  ROW_NUMBER() OVER(PARTITION BY STAGING_AREA_TABLE_ID, HUB_TABLE_ID, BUSINESS_KEY_ATTRIBUTE ORDER BY STAGING_AREA_TABLE_ID, HUB_TABLE_ID, COMPONENT_ORDER ASC) AS COMPONENT_ID,");
                    prepareKeyStatement.AppendLine("  COMPONENT_ORDER,");
                    prepareKeyStatement.AppendLine("  REPLACE(COMPONENT_VALUE,'COMPOSITE(', '') AS COMPONENT_VALUE,");
                    prepareKeyStatement.AppendLine("    CASE");
                    prepareKeyStatement.AppendLine("            WHEN SUBSTRING(BUSINESS_KEY_ATTRIBUTE,1, 11)= 'CONCATENATE' THEN 'CONCATENATE()'");
                    prepareKeyStatement.AppendLine("            WHEN SUBSTRING(BUSINESS_KEY_ATTRIBUTE,1, 6)= 'PIVOT' THEN 'PIVOT()'");
                    prepareKeyStatement.AppendLine("            WHEN SUBSTRING(BUSINESS_KEY_ATTRIBUTE,1, 9)= 'COMPOSITE' THEN 'COMPOSITE()'");
                    prepareKeyStatement.AppendLine("            ELSE 'NORMAL'");
                    prepareKeyStatement.AppendLine("    END AS COMPONENT_TYPE");
                    prepareKeyStatement.AppendLine("FROM");
                    prepareKeyStatement.AppendLine("(");
                    prepareKeyStatement.AppendLine("    SELECT DISTINCT");
                    prepareKeyStatement.AppendLine("        A.STAGING_AREA_TABLE,");
                    prepareKeyStatement.AppendLine("        A.BUSINESS_KEY_ATTRIBUTE,");
                    prepareKeyStatement.AppendLine("        A.INTEGRATION_AREA_TABLE,");
                    prepareKeyStatement.AppendLine("        CASE");
                    prepareKeyStatement.AppendLine("            WHEN CHARINDEX('(', RTRIM(LTRIM(Split.a.value('.', 'VARCHAR(MAX)')))) > 0");
                    prepareKeyStatement.AppendLine("            THEN RTRIM(LTRIM(Split.a.value('.', 'VARCHAR(MAX)')))");
                    prepareKeyStatement.AppendLine("            ELSE REPLACE(RTRIM(LTRIM(Split.a.value('.', 'VARCHAR(MAX)'))), ')', '')");
                    prepareKeyStatement.AppendLine("        END AS COMPONENT_VALUE,");
                    prepareKeyStatement.AppendLine("        ROW_NUMBER() OVER(PARTITION BY STAGING_AREA_TABLE, INTEGRATION_AREA_TABLE, BUSINESS_KEY_ATTRIBUTE ORDER BY STAGING_AREA_TABLE, INTEGRATION_AREA_TABLE, BUSINESS_KEY_ATTRIBUTE ASC) AS COMPONENT_ORDER");
                    prepareKeyStatement.AppendLine("    FROM");
                    prepareKeyStatement.AppendLine("    (");

                    // Change to move from comma separate to semicolon separation for composite keys
                    prepareKeyStatement.AppendLine("      SELECT");
                    prepareKeyStatement.AppendLine("          STAGING_AREA_TABLE, ");
                    prepareKeyStatement.AppendLine("          INTEGRATION_AREA_TABLE, ");
                    prepareKeyStatement.AppendLine("          BUSINESS_KEY_ATTRIBUTE,");
                    prepareKeyStatement.AppendLine("          CASE SUBSTRING(BUSINESS_KEY_ATTRIBUTE, 0, CHARINDEX('(', BUSINESS_KEY_ATTRIBUTE))");
                    prepareKeyStatement.AppendLine("        	 WHEN 'COMPOSITE' THEN CONVERT(XML, '<M>' + REPLACE(BUSINESS_KEY_ATTRIBUTE, ';', '</M><M>') + '</M>') ");
                    prepareKeyStatement.AppendLine("        	 ELSE CONVERT(XML, '<M>' + REPLACE(BUSINESS_KEY_ATTRIBUTE, ',', '</M><M>') + '</M>') ");
                    prepareKeyStatement.AppendLine("          END AS BUSINESS_KEY_ATTRIBUTE_XML");
                    // End of composite key change

                    prepareKeyStatement.AppendLine("        FROM");
                    prepareKeyStatement.AppendLine("        (");
                    prepareKeyStatement.AppendLine("            SELECT DISTINCT STAGING_AREA_TABLE, INTEGRATION_AREA_TABLE, LTRIM(RTRIM(BUSINESS_KEY_ATTRIBUTE)) AS BUSINESS_KEY_ATTRIBUTE");
                    prepareKeyStatement.AppendLine("            FROM MD_TABLE_MAPPING");
                    prepareKeyStatement.AppendLine("            WHERE INTEGRATION_AREA_TABLE LIKE '" + hubTablePrefix + "'");
                    prepareKeyStatement.AppendLine("              AND VERSION_ID = " + versionId);
                    prepareKeyStatement.AppendLine("        ) TableName");
                    prepareKeyStatement.AppendLine("    ) AS A CROSS APPLY BUSINESS_KEY_ATTRIBUTE_XML.nodes('/M') AS Split(a)");
                    prepareKeyStatement.AppendLine("");
                    prepareKeyStatement.AppendLine("    WHERE BUSINESS_KEY_ATTRIBUTE <> 'N/A' AND A.BUSINESS_KEY_ATTRIBUTE != ''");
                    prepareKeyStatement.AppendLine(") pivotsub");
                    prepareKeyStatement.AppendLine("LEFT OUTER JOIN");
                    prepareKeyStatement.AppendLine("       (");
                    prepareKeyStatement.AppendLine("              SELECT STAGING_AREA_TABLE_ID, STAGING_AREA_TABLE_NAME");
                    prepareKeyStatement.AppendLine("              FROM MD_STG");
                    prepareKeyStatement.AppendLine("       ) stgsub");
                    prepareKeyStatement.AppendLine("ON pivotsub.STAGING_AREA_TABLE = stgsub.STAGING_AREA_TABLE_NAME");
                    prepareKeyStatement.AppendLine("LEFT OUTER JOIN");
                    prepareKeyStatement.AppendLine("       (");
                    prepareKeyStatement.AppendLine("              SELECT HUB_TABLE_ID, HUB_TABLE_NAME");
                    prepareKeyStatement.AppendLine("              FROM MD_HUB");
                    prepareKeyStatement.AppendLine("       ) hubsub");
                    prepareKeyStatement.AppendLine("ON pivotsub.INTEGRATION_AREA_TABLE = hubsub.HUB_TABLE_NAME");
                    prepareKeyStatement.AppendLine("ORDER BY stgsub.STAGING_AREA_TABLE_ID, hubsub.HUB_TABLE_ID, COMPONENT_ORDER");

                    var listKeys = GetDataTable(ref connOmd, prepareKeyStatement.ToString());

                    if (listKeys.Rows.Count == 0)
                    {
                        _alert.SetTextLogging("-->  No attributes were found in the metadata, did you reverse-engineer the model?\r\n");
                    }
                    else
                    {
                        foreach (DataRow tableName in listKeys.Rows)
                        {
                            using (var connection = new SqlConnection(metaDataConnection))
                            {
                                _alert.SetTextLogging("-->  Processing the Business Key from " + tableName["STAGING_AREA_TABLE_NAME"] + " to " + tableName["HUB_TABLE_NAME"] + "\r\n");

                                var insertKeyStatement = new StringBuilder();
                                var keyComponent = tableName["COMPONENT_VALUE"]; //Handle quotes between SQL and C%
                                keyComponent = keyComponent.ToString().Replace("'", "''");

                                var businessKeyDefinition = tableName["BUSINESS_KEY_ATTRIBUTE"].ToString();
                                businessKeyDefinition = businessKeyDefinition.Replace("'", "''");

                                insertKeyStatement.AppendLine("INSERT INTO [MD_BUSINESS_KEY_COMPONENT]");
                                insertKeyStatement.AppendLine("(STAGING_AREA_TABLE_ID, HUB_TABLE_ID, BUSINESS_KEY_DEFINITION, COMPONENT_ID, COMPONENT_ORDER, COMPONENT_VALUE, COMPONENT_TYPE)");
                                insertKeyStatement.AppendLine("VALUES ('" + tableName["STAGING_AREA_TABLE_ID"] + "','" + tableName["HUB_TABLE_ID"] + "','" + businessKeyDefinition + "','" + tableName["COMPONENT_ID"] + "','" + tableName["COMPONENT_ORDER"] + "','" + keyComponent + "','" + tableName["COMPONENT_TYPE"] + "')");

                                var command = new SqlCommand(insertKeyStatement.ToString(), connection);

                                try
                                {
                                    connection.Open();
                                    command.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    errorCounter++;
                                    _alert.SetTextLogging("An issue has occured during preparation of the Business Key metadata. Please check the Error Log for more details.\r\n");
                                    errorLog.AppendLine("\r\nAn issue has occured during preparation of Business Key metadata: \r\n\r\n" + ex);
                                }
                            }
                        }
                    }
                    worker.ReportProgress(50);
                    // _alert.SetTextLogging("-->  Processing " + keyCounter + " attributes.\r\n");
                    _alert.SetTextLogging("Preparation of the Business Key definition completed.\r\n");
                }
                catch (Exception ex)
                {
                    errorCounter++;
                    _alert.SetTextLogging("An issue has occured during preparation of the Business Key metadata. Please check the Error Log for more details.\r\n");
                    errorLog.AppendLine("\r\nAn issue has occured during preparation of Business Key metadata: \r\n\r\n" + ex);
                }

                #endregion

                #region Business Key components - 60%
                //9. Understanding the Business Key component parts

                _alert.SetTextLogging("\r\n");
                _alert.SetTextLogging("Commencing the Business Key component analysis.\r\n");

                try
                {
                    var prepareKeyComponentStatement = new StringBuilder();
                    var keyPartCounter = 1;

                    prepareKeyComponentStatement.AppendLine("SELECT DISTINCT");
                    prepareKeyComponentStatement.AppendLine("  STAGING_AREA_TABLE_ID,");
                    prepareKeyComponentStatement.AppendLine("  HUB_TABLE_ID,");
                    prepareKeyComponentStatement.AppendLine("  BUSINESS_KEY_DEFINITION,");
                    prepareKeyComponentStatement.AppendLine("  COMPONENT_ID,");
                    prepareKeyComponentStatement.AppendLine("  ROW_NUMBER() over(partition by STAGING_AREA_TABLE_ID, HUB_TABLE_ID, BUSINESS_KEY_DEFINITION, COMPONENT_ID order by nullif(0 * Split.a.value('count(.)', 'int'), 0)) AS COMPONENT_ELEMENT_ID,");
                    prepareKeyComponentStatement.AppendLine("  ROW_NUMBER() over(partition by STAGING_AREA_TABLE_ID, HUB_TABLE_ID, BUSINESS_KEY_DEFINITION, COMPONENT_ID order by nullif(0 * Split.a.value('count(.)', 'int'), 0)) AS COMPONENT_ELEMENT_ORDER,");
                    prepareKeyComponentStatement.AppendLine("  REPLACE(REPLACE(REPLACE(RTRIM(LTRIM(Split.a.value('.', 'VARCHAR(MAX)'))), 'CONCATENATE(', ''), ')', ''), 'COMPOSITE(', '') AS COMPONENT_ELEMENT_VALUE,");
                    prepareKeyComponentStatement.AppendLine("  CASE");
                    prepareKeyComponentStatement.AppendLine("     WHEN charindex(CHAR(39), REPLACE(REPLACE(RTRIM(LTRIM(Split.a.value('.', 'VARCHAR(MAX)'))), 'CONCATENATE(', ''), ')', '')) = 1 THEN 'User Defined Value'");
                    prepareKeyComponentStatement.AppendLine("    ELSE 'Attribute'");
                    prepareKeyComponentStatement.AppendLine("  END AS COMPONENT_ELEMENT_TYPE,");
                    prepareKeyComponentStatement.AppendLine("  COALESCE(att.ATTRIBUTE_ID, -1) AS ATTRIBUTE_ID");
                    prepareKeyComponentStatement.AppendLine("FROM");
                    prepareKeyComponentStatement.AppendLine("(");
                    prepareKeyComponentStatement.AppendLine("    SELECT");
                    prepareKeyComponentStatement.AppendLine("        STAGING_AREA_TABLE_ID,");
                    prepareKeyComponentStatement.AppendLine("        HUB_TABLE_ID,");
                    prepareKeyComponentStatement.AppendLine("        BUSINESS_KEY_DEFINITION,");
                    prepareKeyComponentStatement.AppendLine("        COMPONENT_ID,");
                    prepareKeyComponentStatement.AppendLine("        COMPONENT_VALUE,");
                    prepareKeyComponentStatement.AppendLine("        CONVERT(XML, '<M>' + REPLACE(COMPONENT_VALUE, ';', '</M><M>') + '</M>') AS COMPONENT_VALUE_XML");
                    prepareKeyComponentStatement.AppendLine("    FROM MD_BUSINESS_KEY_COMPONENT");
                    prepareKeyComponentStatement.AppendLine(") AS A CROSS APPLY COMPONENT_VALUE_XML.nodes('/M') AS Split(a)");
                    prepareKeyComponentStatement.AppendLine("LEFT OUTER JOIN MD_ATT att ON");
                    prepareKeyComponentStatement.AppendLine("    REPLACE(REPLACE(RTRIM(LTRIM(Split.a.value('.', 'VARCHAR(MAX)'))), 'CONCATENATE(', ''), ')', '') = att.ATTRIBUTE_NAME");
                    prepareKeyComponentStatement.AppendLine("WHERE COMPONENT_VALUE <> 'N/A' AND A.COMPONENT_VALUE != ''");
                    prepareKeyComponentStatement.AppendLine("ORDER BY A.STAGING_AREA_TABLE_ID, A.HUB_TABLE_ID, BUSINESS_KEY_DEFINITION, A.COMPONENT_ID, COMPONENT_ELEMENT_ORDER");


                    var listKeyParts = GetDataTable(ref connOmd, prepareKeyComponentStatement.ToString());

                    if (listKeyParts.Rows.Count == 0)
                    {
                        _alert.SetTextLogging("-->  No attributes were found in the metadata, did you reverse-engineer the model?\r\n");
                    }
                    else
                    {
                        foreach (DataRow tableName in listKeyParts.Rows)
                        {
                            using (var connection = new SqlConnection(metaDataConnection))
                            {
                                var insertKeyPartStatement = new StringBuilder();

                                var keyComponent = tableName["COMPONENT_ELEMENT_VALUE"]; //Handle quotes between SQL and C%
                                keyComponent = keyComponent.ToString().Replace("'", "''");

                                var businessKeyDefinition = tableName["BUSINESS_KEY_DEFINITION"];
                                businessKeyDefinition = businessKeyDefinition.ToString().Replace("'", "''");

                                insertKeyPartStatement.AppendLine("INSERT INTO [MD_BUSINESS_KEY_COMPONENT_PART]");
                                insertKeyPartStatement.AppendLine("(STAGING_AREA_TABLE_ID, HUB_TABLE_ID, BUSINESS_KEY_DEFINITION, COMPONENT_ID,COMPONENT_ELEMENT_ID,COMPONENT_ELEMENT_ORDER,COMPONENT_ELEMENT_VALUE,COMPONENT_ELEMENT_TYPE,ATTRIBUTE_ID)");
                                insertKeyPartStatement.AppendLine("VALUES ('" + tableName["STAGING_AREA_TABLE_ID"] + "','" + tableName["HUB_TABLE_ID"] + "','" + businessKeyDefinition + "','" + tableName["COMPONENT_ID"] + "','" + tableName["COMPONENT_ELEMENT_ID"] + "','" + tableName["COMPONENT_ELEMENT_ORDER"] + "','" + keyComponent + "','" + tableName["COMPONENT_ELEMENT_TYPE"] + "','" + tableName["ATTRIBUTE_ID"] + "')");

                                var command = new SqlCommand(insertKeyPartStatement.ToString(), connection);

                                try
                                {
                                    connection.Open();
                                    command.ExecuteNonQuery();
                                    keyPartCounter++;
                                }
                                catch (Exception ex)
                                {
                                    errorCounter++;
                                    _alert.SetTextLogging("An issue has occured during preparation of the Business Key component metadata. Please check the Error Log for more details.\r\n");
                                    errorLog.AppendLine("\r\nAn issue has occured during preparation of Business Key component metadata: \r\n\r\n" + ex);
                                    errorLog.AppendLine("The query that caused a problem was:\r\n");
                                    errorLog.AppendLine(insertKeyPartStatement.ToString());
                                }
                            }
                        }
                    }
                    worker.ReportProgress(60);
                    _alert.SetTextLogging("-->  Processing " + keyPartCounter + " Business Key component attributes\r\n");
                    _alert.SetTextLogging("Preparation of the Business Key components completed.\r\n");
                }
                catch (Exception ex)
                {
                    errorCounter++;
                    _alert.SetTextLogging("An issue has occured during preparation of the Business Key component metadata. Please check the Error Log for more details.\r\n");
                    errorLog.AppendLine("\r\nAn issue has occured during preparation of Business Key component metadata: \r\n\r\n" + ex);
                }

                #endregion

                #region Hub / Link relationship - 75%

                //10. Prepare HUB / LNK xref
                _alert.SetTextLogging("\r\n");
                _alert.SetTextLogging("Commencing preparing the relationship between Hubs and Links.\r\n");

                try
                {
                    var prepareHubLnkXrefStatement = new StringBuilder();

                    prepareHubLnkXrefStatement.AppendLine("SELECT");
                    prepareHubLnkXrefStatement.AppendLine("  hub_tbl.HUB_TABLE_ID,");
                    prepareHubLnkXrefStatement.AppendLine("  hub_tbl.HUB_TABLE_NAME,");
                    prepareHubLnkXrefStatement.AppendLine("  lnk_tbl.LINK_TABLE_ID,");
                    prepareHubLnkXrefStatement.AppendLine("  lnk_tbl.LINK_TABLE_NAME,");
                    prepareHubLnkXrefStatement.AppendLine("  lnk_hubkey_order.HUB_KEY_ORDER AS HUB_ORDER,");
                    prepareHubLnkXrefStatement.AppendLine("  lnk_target_model.HUB_TARGET_KEY_NAME_IN_LINK");
                    prepareHubLnkXrefStatement.AppendLine("FROM");
                    prepareHubLnkXrefStatement.AppendLine("-- This base query adds the Link and its Hubs and their order by pivoting on the full business key");
                    prepareHubLnkXrefStatement.AppendLine("(");
                    prepareHubLnkXrefStatement.AppendLine("  SELECT");
                    prepareHubLnkXrefStatement.AppendLine("    INTEGRATION_AREA_TABLE,");
                    prepareHubLnkXrefStatement.AppendLine("    STAGING_AREA_TABLE,");
                    prepareHubLnkXrefStatement.AppendLine("    BUSINESS_KEY_ATTRIBUTE,");
                    prepareHubLnkXrefStatement.AppendLine("    LTRIM(Split.a.value('.', 'VARCHAR(4000)')) AS BUSINESS_KEY_PART,");
                    prepareHubLnkXrefStatement.AppendLine("    ROW_NUMBER() OVER(PARTITION BY INTEGRATION_AREA_TABLE ORDER BY INTEGRATION_AREA_TABLE) AS HUB_KEY_ORDER");
                    prepareHubLnkXrefStatement.AppendLine("  FROM");
                    prepareHubLnkXrefStatement.AppendLine("  (");
                    prepareHubLnkXrefStatement.AppendLine("    SELECT");
                    prepareHubLnkXrefStatement.AppendLine("      INTEGRATION_AREA_TABLE,");
                    prepareHubLnkXrefStatement.AppendLine("      STAGING_AREA_TABLE,");
                    prepareHubLnkXrefStatement.AppendLine("      ROW_NUMBER() OVER(PARTITION BY INTEGRATION_AREA_TABLE ORDER BY INTEGRATION_AREA_TABLE) AS LINK_ORDER,");
                    prepareHubLnkXrefStatement.AppendLine("      BUSINESS_KEY_ATTRIBUTE, CAST('<M>' + REPLACE(BUSINESS_KEY_ATTRIBUTE, ',', '</M><M>') + '</M>' AS XML) AS BUSINESS_KEY_SOURCE_XML");
                    prepareHubLnkXrefStatement.AppendLine("    FROM  MD_TABLE_MAPPING");
                    prepareHubLnkXrefStatement.AppendLine("    WHERE [INTEGRATION_AREA_TABLE] LIKE '" + lnkTablePrefix + "'");
                    prepareHubLnkXrefStatement.AppendLine("      AND [VERSION_ID] = " + versionId);
                    prepareHubLnkXrefStatement.AppendLine("      AND [GENERATE_INDICATOR] = 'Y'");
                    prepareHubLnkXrefStatement.AppendLine("  ) AS A CROSS APPLY BUSINESS_KEY_SOURCE_XML.nodes('/M') AS Split(a)");
                    prepareHubLnkXrefStatement.AppendLine("  WHERE LINK_ORDER=1 --Any link will do, the order of the Hub keys in the Link will always be the same");
                    prepareHubLnkXrefStatement.AppendLine(") lnk_hubkey_order");

                    prepareHubLnkXrefStatement.AppendLine("-- Adding the information required for the target model in the query");
                    prepareHubLnkXrefStatement.AppendLine("JOIN ");
                    prepareHubLnkXrefStatement.AppendLine("(");
                    prepareHubLnkXrefStatement.AppendLine("SELECT ");
                    prepareHubLnkXrefStatement.AppendLine("	TABLE_NAME AS LINK_TABLE_NAME,");
                    prepareHubLnkXrefStatement.AppendLine("	COLUMN_NAME AS HUB_TARGET_KEY_NAME_IN_LINK ,");
                    prepareHubLnkXrefStatement.AppendLine("	ROW_NUMBER() OVER(PARTITION BY TABLE_NAME ORDER BY ORDINAL_POSITION) AS LINK_ORDER");
                    prepareHubLnkXrefStatement.AppendLine("FROM " + integrationDatabase + ".INFORMATION_SCHEMA.COLUMNS");
                    prepareHubLnkXrefStatement.AppendLine("WHERE [ORDINAL_POSITION]>4");
                    prepareHubLnkXrefStatement.AppendLine("AND [TABLE_NAME] LIKE '" + lnkTablePrefix + "'");
                    prepareHubLnkXrefStatement.AppendLine(") lnk_target_model");
                    prepareHubLnkXrefStatement.AppendLine("ON lnk_hubkey_order.INTEGRATION_AREA_TABLE = lnk_target_model.LINK_TABLE_NAME");
                    prepareHubLnkXrefStatement.AppendLine("AND lnk_hubkey_order.HUB_KEY_ORDER = lnk_target_model.LINK_ORDER");

                    prepareHubLnkXrefStatement.AppendLine("--Adding the Hub mapping data to get the business keys");
                    prepareHubLnkXrefStatement.AppendLine("JOIN MD_TABLE_MAPPING hub");
                    prepareHubLnkXrefStatement.AppendLine("  ON lnk_hubkey_order.[STAGING_AREA_TABLE] = hub.STAGING_AREA_TABLE");
                    prepareHubLnkXrefStatement.AppendLine(" AND lnk_hubkey_order.[BUSINESS_KEY_PART] = hub.BUSINESS_KEY_ATTRIBUTE-- This condition is required to remove the redundant rows caused by the Link key pivoting");
                    prepareHubLnkXrefStatement.AppendLine(" AND hub.[INTEGRATION_AREA_TABLE] LIKE '" + hubTablePrefix + "'");
                    prepareHubLnkXrefStatement.AppendLine(" AND hub.[VERSION_ID] = " + versionId);
                    prepareHubLnkXrefStatement.AppendLine(" AND hub.[GENERATE_INDICATOR] = 'Y'");
                    prepareHubLnkXrefStatement.AppendLine("--Lastly adding the IDs for the Hubs and Links");
                    prepareHubLnkXrefStatement.AppendLine("JOIN dbo.MD_HUB hub_tbl");
                    prepareHubLnkXrefStatement.AppendLine("  ON hub.INTEGRATION_AREA_TABLE = hub_tbl.HUB_TABLE_NAME");
                    prepareHubLnkXrefStatement.AppendLine("JOIN dbo.MD_LINK lnk_tbl");
                    prepareHubLnkXrefStatement.AppendLine("  ON lnk_hubkey_order.INTEGRATION_AREA_TABLE = lnk_tbl.LINK_TABLE_NAME");

                    var listHlXref = GetDataTable(ref connOmd, prepareHubLnkXrefStatement.ToString());

                    foreach (DataRow tableName in listHlXref.Rows)
                    {
                        using (var connection = new SqlConnection(metaDataConnection))
                        {
                            _alert.SetTextLogging("-->  Processing the " + tableName["HUB_TABLE_NAME"] + " / " + tableName["LINK_TABLE_NAME"] + " relationship\r\n");

                            var insertHlXrefStatement = new StringBuilder();

                            insertHlXrefStatement.AppendLine("INSERT INTO [MD_HUB_LINK_XREF]");
                            insertHlXrefStatement.AppendLine("([HUB_TABLE_ID], [LINK_TABLE_ID], [HUB_ORDER], [HUB_TARGET_KEY_NAME_IN_LINK])");
                            insertHlXrefStatement.AppendLine("VALUES ('" + tableName["HUB_TABLE_ID"] + "','" + tableName["LINK_TABLE_ID"] + "','" + tableName["HUB_ORDER"] + "','" + tableName["HUB_TARGET_KEY_NAME_IN_LINK"] + "')");

                            var command = new SqlCommand(insertHlXrefStatement.ToString(), connection);

                            try
                            {
                                connection.Open();
                                command.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                errorCounter++;
                                _alert.SetTextLogging("An issue has occured during preparation of the Hub / Link XREF metadata. Please check the Error Log for more details.\r\n");
                                errorLog.AppendLine("\r\nAn issue has occured during preparation of the Hub / Link XREF metadata: \r\n\r\n" + ex);
                            }
                        }
                    }

                    worker.ReportProgress(75);
                    _alert.SetTextLogging("Preparation of the relationship between Hubs and Links completed.\r\n");
                }
                catch (Exception ex)
                {
                    {
                        errorCounter++;
                        _alert.SetTextLogging("An issue has occured during preparation of the Hub / Link XREF metadata. Please check the Error Log for more details.\r\n");
                        errorLog.AppendLine("\r\nAn issue has occured during preparation of the Hub / Link XREF metadata: \r\n\r\n" + ex);
                    }
                }

                #endregion


                #region Stg / Link relationship - 80%

                //10. Prepare STG / LNK xref
                _alert.SetTextLogging("\r\n");
                _alert.SetTextLogging("Commencing preparing the relationship between Staging Area and Link tables.\r\n");

                try
                {
                    var preparestgLnkXrefStatement = new StringBuilder();

                    preparestgLnkXrefStatement.AppendLine("SELECT");
                    preparestgLnkXrefStatement.AppendLine("  lnk_tbl.LINK_TABLE_ID,");
                    preparestgLnkXrefStatement.AppendLine("  lnk_tbl.LINK_TABLE_NAME,");
                    preparestgLnkXrefStatement.AppendLine("  stg_tbl.STAGING_AREA_TABLE_ID,");
                    preparestgLnkXrefStatement.AppendLine("  stg_tbl.STAGING_AREA_TABLE_NAME,");
                    preparestgLnkXrefStatement.AppendLine("  lnk.FILTER_CRITERIA,");
                    preparestgLnkXrefStatement.AppendLine("  lnk.BUSINESS_KEY_ATTRIBUTE");
                    preparestgLnkXrefStatement.AppendLine("FROM [dbo].[MD_TABLE_MAPPING] lnk");
                    preparestgLnkXrefStatement.AppendLine("JOIN [dbo].[MD_LINK] lnk_tbl ON lnk.INTEGRATION_AREA_TABLE = lnk_tbl.LINK_TABLE_NAME");
                    preparestgLnkXrefStatement.AppendLine("JOIN [dbo].[MD_STG] stg_tbl ON lnk.STAGING_AREA_TABLE = stg_tbl.STAGING_AREA_TABLE_NAME");
                    preparestgLnkXrefStatement.AppendLine("WHERE lnk.INTEGRATION_AREA_TABLE like '" + lnkTablePrefix + "'");
                    preparestgLnkXrefStatement.AppendLine("AND lnk.VERSION_ID = " + versionId);
                    preparestgLnkXrefStatement.AppendLine("AND [GENERATE_INDICATOR] = 'Y'");

                    var listHlXref = GetDataTable(ref connOmd, preparestgLnkXrefStatement.ToString());

                    foreach (DataRow tableName in listHlXref.Rows)
                    {
                        using (var connection = new SqlConnection(metaDataConnection))
                        {
                            _alert.SetTextLogging("-->  Processing the " + tableName["STAGING_AREA_TABLE_NAME"] + " / " + tableName["LINK_TABLE_NAME"] + " relationship\r\n");

                            var insertStgLinkStatement = new StringBuilder();

                            var filterCriterion = tableName["FILTER_CRITERIA"].ToString();
                            filterCriterion = filterCriterion.Replace("'", "''");

                            var businessKeyDefinition = tableName["BUSINESS_KEY_ATTRIBUTE"].ToString();
                            businessKeyDefinition = businessKeyDefinition.Replace("'", "''");

                            insertStgLinkStatement.AppendLine("INSERT INTO [MD_STG_LINK_XREF]");
                            insertStgLinkStatement.AppendLine("([STAGING_AREA_TABLE_ID], [LINK_TABLE_ID], [FILTER_CRITERIA], [BUSINESS_KEY_DEFINITION])");
                            insertStgLinkStatement.AppendLine("VALUES ('" + tableName["STAGING_AREA_TABLE_ID"] + "','" + tableName["LINK_TABLE_ID"] + "','" + filterCriterion + "','" + businessKeyDefinition + "')");

                            var command = new SqlCommand(insertStgLinkStatement.ToString(), connection);

                            try
                            {
                                connection.Open();
                                command.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                errorCounter++;
                                _alert.SetTextLogging("An issue has occured during preparation of the Hub / Link XREF metadata. Please check the Error Log for more details.\r\n");
                                errorLog.AppendLine("\r\nAn issue has occured during preparation of the Hub / Link XREF metadata: \r\n\r\n" + ex);
                            }
                        }
                    }

                    worker.ReportProgress(80);
                    _alert.SetTextLogging("Preparation of the relationship between Staging Area and the Links completed.\r\n");
                }
                catch (Exception ex)
                {
                    {
                        errorCounter++;
                        _alert.SetTextLogging("An issue has occured during preparation of the Staging / Link XREF metadata. Please check the Error Log for more details.\r\n");
                        errorLog.AppendLine("\r\nAn issue has occured during preparation of the Staging / Link XREF metadata: \r\n\r\n" + ex);
                    }
                }

                #endregion

                #region Attribute mapping 90%
                //12. Prepare Attribute mapping
                _alert.SetTextLogging("\r\n");


                try
                {
                    var prepareMappingStatement = new StringBuilder();
                    var mappingCounter = 1;

                    if (checkBoxIgnoreVersion.Checked)
                    {
                        _alert.SetTextLogging("Commencing preparing the column-to-column mapping metadata based on what's available in the database.\r\n");

                        prepareMappingStatement.AppendLine("WITH MAPPED_ATTRIBUTES AS ");
                        prepareMappingStatement.AppendLine("(");
                        prepareMappingStatement.AppendLine("SELECT  stg.STAGING_AREA_TABLE_ID");
                        prepareMappingStatement.AppendLine("	   ,stg.STAGING_AREA_TABLE_NAME");
                        prepareMappingStatement.AppendLine("       ,sat.SATELLITE_TABLE_ID");
                        prepareMappingStatement.AppendLine("	   ,sat.SATELLITE_TABLE_NAME");
                        prepareMappingStatement.AppendLine("	   ,stg_attr.ATTRIBUTE_ID AS ATTRIBUTE_FROM_ID");
                        prepareMappingStatement.AppendLine("	   ,stg_attr.ATTRIBUTE_NAME AS ATTRIBUTE_FROM_NAME");
                        prepareMappingStatement.AppendLine("       ,target_attr.ATTRIBUTE_ID AS ATTRIBUTE_TO_ID   ");
                        prepareMappingStatement.AppendLine("	   ,target_attr.ATTRIBUTE_NAME AS ATTRIBUTE_TO_NAME");
                        prepareMappingStatement.AppendLine("	   ,'N' as MULTI_ACTIVE_KEY_INDICATOR");
                        prepareMappingStatement.AppendLine("	   ,'manually_mapped' as VERIFICATION");
                        prepareMappingStatement.AppendLine("FROM dbo.MD_ATTRIBUTE_MAPPING mapping");
                        prepareMappingStatement.AppendLine("       LEFT OUTER JOIN dbo.MD_SAT sat");
                        prepareMappingStatement.AppendLine("	     on sat.SATELLITE_TABLE_NAME=mapping.TARGET_TABLE");
                        prepareMappingStatement.AppendLine("	   LEFT OUTER JOIN dbo.MD_ATT target_attr");
                        prepareMappingStatement.AppendLine("	     on mapping.TARGET_COLUMN = target_attr.ATTRIBUTE_NAME");
                        prepareMappingStatement.AppendLine("	   LEFT OUTER JOIN dbo.MD_STG stg");
                        prepareMappingStatement.AppendLine("	     on stg.STAGING_AREA_TABLE_NAME = mapping.SOURCE_TABLE");
                        prepareMappingStatement.AppendLine("	   LEFT OUTER JOIN dbo.MD_ATT stg_attr");
                        prepareMappingStatement.AppendLine("	     on mapping.SOURCE_COLUMN = stg_attr.ATTRIBUTE_NAME");
                        prepareMappingStatement.AppendLine("WHERE TARGET_TABLE NOT LIKE '" + dwhKeyIdentifier + "' AND TARGET_TABLE NOT LIKE '" + lnkTablePrefix + "'");
                        prepareMappingStatement.AppendLine("      AND VERSION_ID = " + versionId);
                        prepareMappingStatement.AppendLine("),");
                        prepareMappingStatement.AppendLine("ORIGINAL_ATTRIBUTES AS");
                        prepareMappingStatement.AppendLine("(");
                        prepareMappingStatement.AppendLine("SELECT");
                        prepareMappingStatement.AppendLine("	stg.STAGING_AREA_TABLE_ID,");
                        prepareMappingStatement.AppendLine("	stg.STAGING_AREA_TABLE_NAME,");
                        prepareMappingStatement.AppendLine("	sat.SATELLITE_TABLE_ID,");
                        prepareMappingStatement.AppendLine("	sat.SATELLITE_TABLE_NAME,");
                        prepareMappingStatement.AppendLine("	stg_attr.ATTRIBUTE_ID AS ATTRIBUTE_FROM_ID,");
                        prepareMappingStatement.AppendLine("	stg_attr.ATTRIBUTE_NAME AS ATTRIBUTE_FROM_NAME,");
                        prepareMappingStatement.AppendLine("	stg_attr.ATTRIBUTE_ID AS ATTRIBUTE_TO_ID,");
                        prepareMappingStatement.AppendLine("	stg_attr.ATTRIBUTE_NAME AS ATTRIBUTE_TO_NAME,");
                        prepareMappingStatement.AppendLine("    'N' as MULTI_ACTIVE_KEY_INDICATOR,");
                        prepareMappingStatement.AppendLine("    'automatically_mapped' AS VERIFICATION");
                        prepareMappingStatement.AppendLine("FROM " + linkedServer + stagingDatabase + ".INFORMATION_SCHEMA.COLUMNS mapping");
                        prepareMappingStatement.AppendLine("LEFT OUTER JOIN dbo.MD_STG stg ON stg.STAGING_AREA_TABLE_NAME = mapping.TABLE_NAME");
                        prepareMappingStatement.AppendLine("LEFT OUTER JOIN dbo.MD_ATT stg_attr ON mapping.COLUMN_NAME = stg_attr.ATTRIBUTE_NAME");
                        prepareMappingStatement.AppendLine("JOIN MD_STG_SAT_XREF xref ON xref.STAGING_AREA_TABLE_ID = stg.STAGING_AREA_TABLE_ID");
                        prepareMappingStatement.AppendLine("JOIN MD_SAT sat ON xref.SATELLITE_TABLE_ID = sat.SATELLITE_TABLE_ID");
                        prepareMappingStatement.AppendLine("JOIN " + linkedServer + integrationDatabase + ".INFORMATION_SCHEMA.COLUMNS satatts");
                        prepareMappingStatement.AppendLine("on sat.SATELLITE_TABLE_NAME = satatts.TABLE_NAME");
                        prepareMappingStatement.AppendLine("and UPPER(mapping.COLUMN_NAME) = UPPER(satatts.COLUMN_NAME)");
                        prepareMappingStatement.AppendLine("WHERE mapping.COLUMN_NAME NOT IN");
                        prepareMappingStatement.AppendLine("  ( ");

                        prepareMappingStatement.AppendLine("  '" + recordSource + "',");
                        prepareMappingStatement.AppendLine("  '" + alternativeRecordSource + "',");
                        prepareMappingStatement.AppendLine("  '" + sourceRowId + "',");
                        prepareMappingStatement.AppendLine("  '" + recordChecksum + "',");
                        prepareMappingStatement.AppendLine("  '" + changeDataCaptureIndicator + "',");
                        prepareMappingStatement.AppendLine("  '" + hubAlternativeLdts + "',");
                        prepareMappingStatement.AppendLine("  '" + eventDateTimeAtttribute + "',");
                        prepareMappingStatement.AppendLine("  '" + effectiveDateTimeAttribute + "',");
                        prepareMappingStatement.AppendLine("  '" + etlProcessId + "',");
                        prepareMappingStatement.AppendLine("  '" + loadDateTimeStamp + "'");

                        prepareMappingStatement.AppendLine("  ) ");
                        prepareMappingStatement.AppendLine(")");
                        prepareMappingStatement.AppendLine("SELECT ");
                        prepareMappingStatement.AppendLine("	STAGING_AREA_TABLE_ID,");
                        prepareMappingStatement.AppendLine("	STAGING_AREA_TABLE_NAME,");
                        prepareMappingStatement.AppendLine("	SATELLITE_TABLE_ID,");
                        prepareMappingStatement.AppendLine("	SATELLITE_TABLE_NAME,");
                        prepareMappingStatement.AppendLine("	ATTRIBUTE_FROM_ID,");
                        prepareMappingStatement.AppendLine("	ATTRIBUTE_FROM_NAME,");
                        prepareMappingStatement.AppendLine("	ATTRIBUTE_TO_ID,");
                        prepareMappingStatement.AppendLine("	ATTRIBUTE_TO_NAME,");
                        prepareMappingStatement.AppendLine("	MULTI_ACTIVE_KEY_INDICATOR,");
                        prepareMappingStatement.AppendLine("	VERIFICATION");
                        prepareMappingStatement.AppendLine("FROM MAPPED_ATTRIBUTES");
                        prepareMappingStatement.AppendLine("UNION");
                        prepareMappingStatement.AppendLine("SELECT ");
                        prepareMappingStatement.AppendLine("	a.STAGING_AREA_TABLE_ID,");
                        prepareMappingStatement.AppendLine("	a.STAGING_AREA_TABLE_NAME,");
                        prepareMappingStatement.AppendLine("	a.SATELLITE_TABLE_ID,");
                        prepareMappingStatement.AppendLine("	a.SATELLITE_TABLE_NAME,");
                        prepareMappingStatement.AppendLine("	a.ATTRIBUTE_FROM_ID,");
                        prepareMappingStatement.AppendLine("	a.ATTRIBUTE_FROM_NAME,");
                        prepareMappingStatement.AppendLine("	a.ATTRIBUTE_TO_ID,");
                        prepareMappingStatement.AppendLine("	a.ATTRIBUTE_TO_NAME,");
                        prepareMappingStatement.AppendLine("	a.MULTI_ACTIVE_KEY_INDICATOR,");
                        prepareMappingStatement.AppendLine("	a.VERIFICATION");
                        prepareMappingStatement.AppendLine("FROM ORIGINAL_ATTRIBUTES a");
                        prepareMappingStatement.AppendLine("LEFT OUTER JOIN MAPPED_ATTRIBUTES b ");
                        prepareMappingStatement.AppendLine("	ON a.STAGING_AREA_TABLE_ID=b.STAGING_AREA_TABLE_ID ");
                        prepareMappingStatement.AppendLine("  AND a.SATELLITE_TABLE_ID=b.SATELLITE_TABLE_ID");
                        prepareMappingStatement.AppendLine("  AND a.ATTRIBUTE_FROM_ID=b.ATTRIBUTE_FROM_ID");
                        prepareMappingStatement.AppendLine("WHERE b.ATTRIBUTE_TO_ID IS NULL");
                    }
                    else
                    {
                        _alert.SetTextLogging("Commencing preparing the column-to-column mapping metadata based on the model metadata.\r\n");

                        prepareMappingStatement.AppendLine("WITH MAPPED_ATTRIBUTES AS ");
                        prepareMappingStatement.AppendLine("(");
                        prepareMappingStatement.AppendLine("SELECT  stg.STAGING_AREA_TABLE_ID");
                        prepareMappingStatement.AppendLine("	   ,stg.STAGING_AREA_TABLE_NAME");
                        prepareMappingStatement.AppendLine("       ,sat.SATELLITE_TABLE_ID");
                        prepareMappingStatement.AppendLine("	   ,sat.SATELLITE_TABLE_NAME");
                        prepareMappingStatement.AppendLine("	   ,stg_attr.ATTRIBUTE_ID AS ATTRIBUTE_FROM_ID");
                        prepareMappingStatement.AppendLine("	   ,stg_attr.ATTRIBUTE_NAME AS ATTRIBUTE_FROM_NAME");
                        prepareMappingStatement.AppendLine("       ,target_attr.ATTRIBUTE_ID AS ATTRIBUTE_TO_ID   ");
                        prepareMappingStatement.AppendLine("	   ,target_attr.ATTRIBUTE_NAME AS ATTRIBUTE_TO_NAME");
                        prepareMappingStatement.AppendLine("	   ,'N' as MULTI_ACTIVE_KEY_INDICATOR");
                        prepareMappingStatement.AppendLine("	   ,'manually_mapped' as VERIFICATION");
                        prepareMappingStatement.AppendLine("FROM dbo.MD_ATTRIBUTE_MAPPING mapping");
                        prepareMappingStatement.AppendLine("       LEFT OUTER JOIN dbo.MD_SAT sat");
                        prepareMappingStatement.AppendLine("	     on sat.SATELLITE_TABLE_NAME=mapping.TARGET_TABLE");
                        prepareMappingStatement.AppendLine("	   LEFT OUTER JOIN dbo.MD_ATT target_attr");
                        prepareMappingStatement.AppendLine("	     on mapping.TARGET_COLUMN = target_attr.ATTRIBUTE_NAME");
                        prepareMappingStatement.AppendLine("	   LEFT OUTER JOIN dbo.MD_STG stg");
                        prepareMappingStatement.AppendLine("	     on stg.STAGING_AREA_TABLE_NAME = mapping.SOURCE_TABLE");
                        prepareMappingStatement.AppendLine("	   LEFT OUTER JOIN dbo.MD_ATT stg_attr");
                        prepareMappingStatement.AppendLine("	     on mapping.SOURCE_COLUMN = stg_attr.ATTRIBUTE_NAME");
                        prepareMappingStatement.AppendLine("WHERE TARGET_TABLE NOT LIKE '" + dwhKeyIdentifier + "' AND TARGET_TABLE NOT LIKE '" + lnkTablePrefix + "'");
                        prepareMappingStatement.AppendLine("      AND VERSION_ID = " + versionId);
                        prepareMappingStatement.AppendLine("),");
                        prepareMappingStatement.AppendLine("ORIGINAL_ATTRIBUTES AS");
                        prepareMappingStatement.AppendLine("(");
                        prepareMappingStatement.AppendLine("SELECT ");
                        prepareMappingStatement.AppendLine("	stg.STAGING_AREA_TABLE_ID,");
                        prepareMappingStatement.AppendLine("	stg.STAGING_AREA_TABLE_NAME,");
                        prepareMappingStatement.AppendLine("	sat.SATELLITE_TABLE_ID,");
                        prepareMappingStatement.AppendLine("	sat.SATELLITE_TABLE_NAME,");
                        prepareMappingStatement.AppendLine("	stg_attr.ATTRIBUTE_ID AS ATTRIBUTE_FROM_ID,");
                        prepareMappingStatement.AppendLine("	stg_attr.ATTRIBUTE_NAME AS ATTRIBUTE_FROM_NAME,");
                        prepareMappingStatement.AppendLine("	stg_attr.ATTRIBUTE_ID AS ATTRIBUTE_TO_ID,");
                        prepareMappingStatement.AppendLine("	stg_attr.ATTRIBUTE_NAME AS ATTRIBUTE_TO_NAME,");
                        prepareMappingStatement.AppendLine("	'N' as MULTI_ACTIVE_KEY_INDICATOR,");
                        prepareMappingStatement.AppendLine("	'automatically_mapped' AS VERIFICATION");
                        prepareMappingStatement.AppendLine("FROM MD_VERSION_ATTRIBUTE mapping");

                        prepareMappingStatement.AppendLine("LEFT OUTER JOIN MD_STG stg ON stg.STAGING_AREA_TABLE_NAME = mapping.TABLE_NAME");
                        prepareMappingStatement.AppendLine("LEFT OUTER JOIN MD_STG_SAT_XREF xref ON stg.STAGING_AREA_TABLE_ID = xref.STAGING_AREA_TABLE_ID");
                        prepareMappingStatement.AppendLine("LEFT OUTER JOIN MD_SAT sat ON xref.SATELLITE_TABLE_ID = sat.SATELLITE_TABLE_ID");
                        prepareMappingStatement.AppendLine("LEFT OUTER JOIN MD_ATT stg_attr on mapping.COLUMN_NAME = stg_attr.ATTRIBUTE_NAME");

                        prepareMappingStatement.AppendLine("JOIN MD_VERSION_ATTRIBUTE satatts");
                        prepareMappingStatement.AppendLine("    on sat.SATELLITE_TABLE_NAME=satatts.TABLE_NAME");
                        prepareMappingStatement.AppendLine("    and UPPER(mapping.COLUMN_NAME) = UPPER(satatts.COLUMN_NAME)");
                        prepareMappingStatement.AppendLine("WHERE mapping.COLUMN_NAME NOT IN");
                        prepareMappingStatement.AppendLine("  ( ");

                        prepareMappingStatement.AppendLine("  '" + recordSource + "',");
                        prepareMappingStatement.AppendLine("  '" + alternativeRecordSource + "',");
                        prepareMappingStatement.AppendLine("  '" + sourceRowId + "',");
                        prepareMappingStatement.AppendLine("  '" + recordChecksum + "',");
                        prepareMappingStatement.AppendLine("  '" + changeDataCaptureIndicator + "',");
                        prepareMappingStatement.AppendLine("  '" + hubAlternativeLdts + "',");
                        prepareMappingStatement.AppendLine("  '" + eventDateTimeAtttribute + "',");
                        prepareMappingStatement.AppendLine("  '" + effectiveDateTimeAttribute + "',");
                        prepareMappingStatement.AppendLine("  '" + etlProcessId + "',");
                        prepareMappingStatement.AppendLine("  '" + loadDateTimeStamp + "'");

                        prepareMappingStatement.AppendLine("  ) ");

                        prepareMappingStatement.AppendLine("AND mapping.VERSION_ID = " + versionId + " AND satatts.VERSION_ID = " + versionId);
                        prepareMappingStatement.AppendLine(")");
                        prepareMappingStatement.AppendLine("SELECT ");
                        prepareMappingStatement.AppendLine("	STAGING_AREA_TABLE_ID,");
                        prepareMappingStatement.AppendLine("	STAGING_AREA_TABLE_NAME,");
                        prepareMappingStatement.AppendLine("	SATELLITE_TABLE_ID,");
                        prepareMappingStatement.AppendLine("	SATELLITE_TABLE_NAME,");
                        prepareMappingStatement.AppendLine("	ATTRIBUTE_FROM_ID,");
                        prepareMappingStatement.AppendLine("	ATTRIBUTE_FROM_NAME,");
                        prepareMappingStatement.AppendLine("	ATTRIBUTE_TO_ID,");
                        prepareMappingStatement.AppendLine("	ATTRIBUTE_TO_NAME,");
                        prepareMappingStatement.AppendLine("	MULTI_ACTIVE_KEY_INDICATOR,");
                        prepareMappingStatement.AppendLine("	VERIFICATION");
                        prepareMappingStatement.AppendLine("FROM MAPPED_ATTRIBUTES");
                        prepareMappingStatement.AppendLine("UNION");
                        prepareMappingStatement.AppendLine("SELECT ");
                        prepareMappingStatement.AppendLine("	a.STAGING_AREA_TABLE_ID,");
                        prepareMappingStatement.AppendLine("	a.STAGING_AREA_TABLE_NAME,");
                        prepareMappingStatement.AppendLine("	a.SATELLITE_TABLE_ID,");
                        prepareMappingStatement.AppendLine("	a.SATELLITE_TABLE_NAME,");
                        prepareMappingStatement.AppendLine("	a.ATTRIBUTE_FROM_ID,");
                        prepareMappingStatement.AppendLine("	a.ATTRIBUTE_FROM_NAME,");
                        prepareMappingStatement.AppendLine("	a.ATTRIBUTE_TO_ID,");
                        prepareMappingStatement.AppendLine("	a.ATTRIBUTE_TO_NAME,");
                        prepareMappingStatement.AppendLine("	a.MULTI_ACTIVE_KEY_INDICATOR,");
                        prepareMappingStatement.AppendLine("	a.VERIFICATION");
                        prepareMappingStatement.AppendLine("FROM ORIGINAL_ATTRIBUTES a");
                        prepareMappingStatement.AppendLine("LEFT OUTER JOIN MAPPED_ATTRIBUTES b ");
                        prepareMappingStatement.AppendLine("	ON a.STAGING_AREA_TABLE_ID=b.STAGING_AREA_TABLE_ID ");
                        prepareMappingStatement.AppendLine("  AND a.SATELLITE_TABLE_ID=b.SATELLITE_TABLE_ID");
                        prepareMappingStatement.AppendLine("  AND a.ATTRIBUTE_FROM_ID=b.ATTRIBUTE_FROM_ID");
                        prepareMappingStatement.AppendLine("WHERE b.ATTRIBUTE_TO_ID IS NULL");
                    }

                    var listMappings = GetDataTable(ref connOmd, prepareMappingStatement.ToString());

                    if (listMappings.Rows.Count == 0)
                    {
                        _alert.SetTextLogging("-->  No column-to-column mappings were detected.\r\n");
                    }
                    else
                    {
                        foreach (DataRow tableName in listMappings.Rows)
                        {
                            using (var connection = new SqlConnection(metaDataConnection))
                            {

                                var insertMappingStatement = new StringBuilder();

                                insertMappingStatement.AppendLine("INSERT INTO [MD_STG_SAT_ATT_XREF]");
                                insertMappingStatement.AppendLine("( [STAGING_AREA_TABLE_ID],[SATELLITE_TABLE_ID],[ATTRIBUTE_ID_FROM],[ATTRIBUTE_ID_TO],[MULTI_ACTIVE_KEY_INDICATOR])");
                                insertMappingStatement.AppendLine("VALUES ('" +
                                                               tableName["STAGING_AREA_TABLE_ID"] + "'," +
                                                               tableName["SATELLITE_TABLE_ID"] + "," +
                                                               tableName["ATTRIBUTE_FROM_ID"] + "," +
                                                               tableName["ATTRIBUTE_TO_ID"] + ",'" +
                                                               tableName["MULTI_ACTIVE_KEY_INDICATOR"] + "')");

                                var command = new SqlCommand(insertMappingStatement.ToString(), connection);

                                try
                                {
                                    connection.Open();
                                    command.ExecuteNonQuery();
                                    mappingCounter++;
                                }
                                catch (Exception)
                                {
                                    _alert.SetTextLogging("-----> An issue has occurred mapping columns from table " + tableName["STAGING_AREA_TABLE_NAME"] + " to " + tableName["SATELLITE_TABLE_NAME"] + ". \r\n");
                                    if (tableName["ATTRIBUTE_FROM_ID"].ToString() == "")
                                    {
                                        _alert.SetTextLogging("Both attributes are NULL.");
                                    }
                                }
                            }
                        }
                    }

                    worker.ReportProgress(90);
                    _alert.SetTextLogging("-->  Processing " + mappingCounter + " attribute mappings\r\n");
                    _alert.SetTextLogging("Preparation of the column-to-column mapping metadata completed.\r\n");
                }
                catch (Exception ex)
                {
                    errorCounter++;
                    _alert.SetTextLogging("An issue has occured during preparation of the Satellite Attribute mapping metadata. Please check the Error Log for more details.\r\n");
                    errorLog.AppendLine("\r\nAn issue has occured during preparation of the Satellite Attribute mapping metadata: \r\n\r\n" + ex);
                }

                #endregion

                #region Degenerate attribute mapping 95%
                //13. Prepare degenerate attribute mapping
                _alert.SetTextLogging("\r\n");

                try
                {
                    var prepareDegenerateMappingStatement = new StringBuilder();
                    var degenerateMappingCounter = 1;

                    if (checkBoxIgnoreVersion.Checked)
                    {
                        _alert.SetTextLogging("Commencing preparing the degenerate column metadata using the database.\r\n");

                        prepareDegenerateMappingStatement.AppendLine("WITH MAPPED_ATTRIBUTES AS");
                        prepareDegenerateMappingStatement.AppendLine("(");
                        prepareDegenerateMappingStatement.AppendLine("SELECT  stg.STAGING_AREA_TABLE_ID");
                        prepareDegenerateMappingStatement.AppendLine("       ,lnk.LINK_TABLE_ID");
                        prepareDegenerateMappingStatement.AppendLine("	   ,stg_attr.ATTRIBUTE_ID AS ATTRIBUTE_FROM_ID");
                        prepareDegenerateMappingStatement.AppendLine("       ,target_attr.ATTRIBUTE_ID AS ATTRIBUTE_TO_ID   ");
                        prepareDegenerateMappingStatement.AppendLine("	   ,'manually_mapped' as VERIFICATION");
                        prepareDegenerateMappingStatement.AppendLine("FROM dbo.MD_ATTRIBUTE_MAPPING mapping");
                        prepareDegenerateMappingStatement.AppendLine("       LEFT OUTER JOIN dbo.MD_LINK lnk");
                        prepareDegenerateMappingStatement.AppendLine("	     on lnk.LINK_TABLE_NAME=mapping.TARGET_TABLE");
                        prepareDegenerateMappingStatement.AppendLine("	   LEFT OUTER JOIN dbo.MD_ATT target_attr");
                        prepareDegenerateMappingStatement.AppendLine("	     on mapping.TARGET_COLUMN = target_attr.ATTRIBUTE_NAME");
                        prepareDegenerateMappingStatement.AppendLine("	   LEFT OUTER JOIN dbo.MD_STG stg");
                        prepareDegenerateMappingStatement.AppendLine("	     on stg.STAGING_AREA_TABLE_NAME = mapping.SOURCE_TABLE");
                        prepareDegenerateMappingStatement.AppendLine("	   LEFT OUTER JOIN dbo.MD_ATT stg_attr");
                        prepareDegenerateMappingStatement.AppendLine("	     on mapping.SOURCE_COLUMN = stg_attr.ATTRIBUTE_NAME");
                        prepareDegenerateMappingStatement.AppendLine("WHERE TARGET_TABLE NOT LIKE '" + dwhKeyIdentifier + "' AND TARGET_TABLE LIKE '" + lnkTablePrefix + "'");
                        prepareDegenerateMappingStatement.AppendLine("AND VERSION_ID = " + versionId);
                        prepareDegenerateMappingStatement.AppendLine("),");
                        prepareDegenerateMappingStatement.AppendLine("ORIGINAL_ATTRIBUTES AS");
                        prepareDegenerateMappingStatement.AppendLine("(");
                        prepareDegenerateMappingStatement.AppendLine("SELECT ");
                        prepareDegenerateMappingStatement.AppendLine("	--TABLE_NAME, ");
                        prepareDegenerateMappingStatement.AppendLine("	--COLUMN_NAME, ");
                        prepareDegenerateMappingStatement.AppendLine("	stg.STAGING_AREA_TABLE_ID,");
                        prepareDegenerateMappingStatement.AppendLine("	lnk.LINK_TABLE_ID,");
                        prepareDegenerateMappingStatement.AppendLine("	stg_attr.ATTRIBUTE_ID AS ATTRIBUTE_FROM_ID,");
                        prepareDegenerateMappingStatement.AppendLine("	stg_attr.ATTRIBUTE_ID AS ATTRIBUTE_TO_ID,");
                        prepareDegenerateMappingStatement.AppendLine("	'automatically_mapped' AS VERIFICATION");
                        prepareDegenerateMappingStatement.AppendLine("FROM " + linkedServer + stagingDatabase + ".INFORMATION_SCHEMA.COLUMNS mapping");
                        prepareDegenerateMappingStatement.AppendLine("LEFT OUTER JOIN dbo.MD_STG stg");
                        prepareDegenerateMappingStatement.AppendLine("	on stg.STAGING_AREA_TABLE_NAME = mapping.TABLE_NAME");
                        prepareDegenerateMappingStatement.AppendLine("LEFT OUTER JOIN dbo.MD_ATT stg_attr");
                        prepareDegenerateMappingStatement.AppendLine("	on mapping.COLUMN_NAME = stg_attr.ATTRIBUTE_NAME");
                        prepareDegenerateMappingStatement.AppendLine("JOIN MD_STG_LINK_ATT_XREF stglnk");
                        prepareDegenerateMappingStatement.AppendLine("    on 	stg.STAGING_AREA_TABLE_ID = stglnk.STAGING_AREA_TABLE_ID");
                        prepareDegenerateMappingStatement.AppendLine("JOIN MD_LINK lnk");
                        prepareDegenerateMappingStatement.AppendLine("    on stglnk.LINK_TABLE_ID = lnk.LINK_TABLE_ID");
                        prepareDegenerateMappingStatement.AppendLine("JOIN " + linkedServer + integrationDatabase + ".INFORMATION_SCHEMA.COLUMNS lnkatts");
                        prepareDegenerateMappingStatement.AppendLine("    on lnk.LINK_TABLE_NAME=lnkatts.TABLE_NAME");
                        prepareDegenerateMappingStatement.AppendLine("    and UPPER(mapping.COLUMN_NAME) = UPPER(lnkatts.COLUMN_NAME)");
                        prepareDegenerateMappingStatement.AppendLine("WHERE mapping.COLUMN_NAME NOT IN ");
                        prepareDegenerateMappingStatement.AppendLine("  ( ");

                        prepareDegenerateMappingStatement.AppendLine("  '" + recordSource + "',");
                        prepareDegenerateMappingStatement.AppendLine("  '" + alternativeRecordSource + "',");
                        prepareDegenerateMappingStatement.AppendLine("  '" + sourceRowId + "',");
                        prepareDegenerateMappingStatement.AppendLine("  '" + recordChecksum + "',");
                        prepareDegenerateMappingStatement.AppendLine("  '" + changeDataCaptureIndicator + "',");
                        prepareDegenerateMappingStatement.AppendLine("  '" + hubAlternativeLdts + "',");
                        prepareDegenerateMappingStatement.AppendLine("  '" + eventDateTimeAtttribute + "',");
                        prepareDegenerateMappingStatement.AppendLine("  '" + effectiveDateTimeAttribute + "',");
                        prepareDegenerateMappingStatement.AppendLine("  '" + etlProcessId + "',");
                        prepareDegenerateMappingStatement.AppendLine("  '" + loadDateTimeStamp + "'");

                        prepareDegenerateMappingStatement.AppendLine("  ) ");
                        prepareDegenerateMappingStatement.AppendLine(")");
                        prepareDegenerateMappingStatement.AppendLine("SELECT ");
                        prepareDegenerateMappingStatement.AppendLine("	STAGING_AREA_TABLE_ID,");
                        prepareDegenerateMappingStatement.AppendLine("	LINK_TABLE_ID,");
                        prepareDegenerateMappingStatement.AppendLine("	ATTRIBUTE_FROM_ID,");
                        prepareDegenerateMappingStatement.AppendLine("	ATTRIBUTE_TO_ID");
                        prepareDegenerateMappingStatement.AppendLine("	--VERIFICATION");
                        prepareDegenerateMappingStatement.AppendLine("FROM MAPPED_ATTRIBUTES");
                        prepareDegenerateMappingStatement.AppendLine("UNION");
                        prepareDegenerateMappingStatement.AppendLine("SELECT ");
                        prepareDegenerateMappingStatement.AppendLine("	a.STAGING_AREA_TABLE_ID,");
                        prepareDegenerateMappingStatement.AppendLine("	a.LINK_TABLE_ID,");
                        prepareDegenerateMappingStatement.AppendLine("	a.ATTRIBUTE_FROM_ID,");
                        prepareDegenerateMappingStatement.AppendLine("	a.ATTRIBUTE_TO_ID");
                        prepareDegenerateMappingStatement.AppendLine("	--a.VERIFICATION");
                        prepareDegenerateMappingStatement.AppendLine("FROM ORIGINAL_ATTRIBUTES a");
                        prepareDegenerateMappingStatement.AppendLine("LEFT OUTER JOIN MAPPED_ATTRIBUTES b ");
                        prepareDegenerateMappingStatement.AppendLine("	ON a.STAGING_AREA_TABLE_ID=b.STAGING_AREA_TABLE_ID ");
                        prepareDegenerateMappingStatement.AppendLine("  AND a.LINK_TABLE_ID=b.LINK_TABLE_ID");
                        prepareDegenerateMappingStatement.AppendLine("  AND a.ATTRIBUTE_FROM_ID=b.ATTRIBUTE_FROM_ID");
                        prepareDegenerateMappingStatement.AppendLine("WHERE b.ATTRIBUTE_TO_ID IS NULL");
                    }
                    else
                    {
                        _alert.SetTextLogging("Commencing preparing the degenerate column metadata using model metadata.\r\n");

                        prepareDegenerateMappingStatement.AppendLine("WITH MAPPED_ATTRIBUTES AS");
                        prepareDegenerateMappingStatement.AppendLine("(");
                        prepareDegenerateMappingStatement.AppendLine("SELECT  stg.STAGING_AREA_TABLE_ID");
                        prepareDegenerateMappingStatement.AppendLine("       ,lnk.LINK_TABLE_ID");
                        prepareDegenerateMappingStatement.AppendLine("	   ,stg_attr.ATTRIBUTE_ID AS ATTRIBUTE_FROM_ID");
                        prepareDegenerateMappingStatement.AppendLine("       ,target_attr.ATTRIBUTE_ID AS ATTRIBUTE_TO_ID   ");
                        prepareDegenerateMappingStatement.AppendLine("	   ,'manually_mapped' as VERIFICATION");
                        prepareDegenerateMappingStatement.AppendLine("FROM dbo.MD_ATTRIBUTE_MAPPING mapping");
                        prepareDegenerateMappingStatement.AppendLine("       LEFT OUTER JOIN dbo.MD_LINK lnk");
                        prepareDegenerateMappingStatement.AppendLine("	     on lnk.LINK_TABLE_NAME=mapping.TARGET_TABLE");
                        prepareDegenerateMappingStatement.AppendLine("	   LEFT OUTER JOIN dbo.MD_ATT target_attr");
                        prepareDegenerateMappingStatement.AppendLine("	     on mapping.TARGET_COLUMN = target_attr.ATTRIBUTE_NAME");
                        prepareDegenerateMappingStatement.AppendLine("	   LEFT OUTER JOIN dbo.MD_STG stg");
                        prepareDegenerateMappingStatement.AppendLine("	     on stg.STAGING_AREA_TABLE_NAME = mapping.SOURCE_TABLE");
                        prepareDegenerateMappingStatement.AppendLine("	   LEFT OUTER JOIN dbo.MD_ATT stg_attr");
                        prepareDegenerateMappingStatement.AppendLine("	     on mapping.SOURCE_COLUMN = stg_attr.ATTRIBUTE_NAME");
                        prepareDegenerateMappingStatement.AppendLine("WHERE TARGET_TABLE NOT LIKE '" + dwhKeyIdentifier + "' AND TARGET_TABLE LIKE '" + lnkTablePrefix + "'");
                        prepareDegenerateMappingStatement.AppendLine("AND VERSION_ID = " + versionId);
                        prepareDegenerateMappingStatement.AppendLine("),");
                        prepareDegenerateMappingStatement.AppendLine("ORIGINAL_ATTRIBUTES AS");
                        prepareDegenerateMappingStatement.AppendLine("(");
                        prepareDegenerateMappingStatement.AppendLine("SELECT ");
                        prepareDegenerateMappingStatement.AppendLine("	--TABLE_NAME, ");
                        prepareDegenerateMappingStatement.AppendLine("	--COLUMN_NAME, ");
                        prepareDegenerateMappingStatement.AppendLine("	stg.STAGING_AREA_TABLE_ID,");
                        prepareDegenerateMappingStatement.AppendLine("	lnk.LINK_TABLE_ID,");
                        prepareDegenerateMappingStatement.AppendLine("	stg_attr.ATTRIBUTE_ID AS ATTRIBUTE_FROM_ID,");
                        prepareDegenerateMappingStatement.AppendLine("	stg_attr.ATTRIBUTE_ID AS ATTRIBUTE_TO_ID,");
                        prepareDegenerateMappingStatement.AppendLine("	'automatically_mapped' AS VERIFICATION");
                        prepareDegenerateMappingStatement.AppendLine("FROM MD_VERSION_ATTRIBUTE mapping");
                        prepareDegenerateMappingStatement.AppendLine("LEFT OUTER JOIN dbo.MD_STG stg ON stg.STAGING_AREA_TABLE_NAME = mapping.TABLE_NAME");
                        prepareDegenerateMappingStatement.AppendLine("LEFT OUTER JOIN dbo.MD_ATT stg_attr ON mapping.COLUMN_NAME = stg_attr.ATTRIBUTE_NAME");
                        prepareDegenerateMappingStatement.AppendLine("JOIN MD_STG_LINK_ATT_XREF stglnk ON stg.STAGING_AREA_TABLE_ID = stglnk.STAGING_AREA_TABLE_ID");
                        prepareDegenerateMappingStatement.AppendLine("JOIN MD_LINK lnk ON stglnk.LINK_TABLE_ID = lnk.LINK_TABLE_ID");
                        prepareDegenerateMappingStatement.AppendLine("JOIN MD_VERSION_ATTRIBUTE lnkatts");
                        prepareDegenerateMappingStatement.AppendLine("    on lnk.LINK_TABLE_NAME=lnkatts.TABLE_NAME");
                        prepareDegenerateMappingStatement.AppendLine("    and UPPER(mapping.COLUMN_NAME) = UPPER(lnkatts.COLUMN_NAME)");
                        prepareDegenerateMappingStatement.AppendLine("WHERE mapping.COLUMN_NAME NOT IN ");
                        prepareDegenerateMappingStatement.AppendLine("  ( ");

                        prepareDegenerateMappingStatement.AppendLine("  '" + recordSource + "',");
                        prepareDegenerateMappingStatement.AppendLine("  '" + alternativeRecordSource + "',");
                        prepareDegenerateMappingStatement.AppendLine("  '" + sourceRowId + "',");
                        prepareDegenerateMappingStatement.AppendLine("  '" + recordChecksum + "',");
                        prepareDegenerateMappingStatement.AppendLine("  '" + changeDataCaptureIndicator + "',");
                        prepareDegenerateMappingStatement.AppendLine("  '" + hubAlternativeLdts + "',");
                        prepareDegenerateMappingStatement.AppendLine("  '" + eventDateTimeAtttribute + "',");
                        prepareDegenerateMappingStatement.AppendLine("  '" + effectiveDateTimeAttribute + "',");
                        prepareDegenerateMappingStatement.AppendLine("  '" + etlProcessId + "',");
                        prepareDegenerateMappingStatement.AppendLine("  '" + loadDateTimeStamp + "'");

                        prepareDegenerateMappingStatement.AppendLine("  ) ");
                        prepareDegenerateMappingStatement.AppendLine("AND mapping.VERSION_ID = " + versionId + " AND lnkatts.VERSION_ID = " + versionId);
                        prepareDegenerateMappingStatement.AppendLine(")");
                        prepareDegenerateMappingStatement.AppendLine("SELECT ");
                        prepareDegenerateMappingStatement.AppendLine("	STAGING_AREA_TABLE_ID,");
                        prepareDegenerateMappingStatement.AppendLine("	LINK_TABLE_ID,");
                        prepareDegenerateMappingStatement.AppendLine("	ATTRIBUTE_FROM_ID,");
                        prepareDegenerateMappingStatement.AppendLine("	ATTRIBUTE_TO_ID");
                        prepareDegenerateMappingStatement.AppendLine("	--VERIFICATION");
                        prepareDegenerateMappingStatement.AppendLine("FROM MAPPED_ATTRIBUTES");
                        prepareDegenerateMappingStatement.AppendLine("UNION");
                        prepareDegenerateMappingStatement.AppendLine("SELECT ");
                        prepareDegenerateMappingStatement.AppendLine("	a.STAGING_AREA_TABLE_ID,");
                        prepareDegenerateMappingStatement.AppendLine("	a.LINK_TABLE_ID,");
                        prepareDegenerateMappingStatement.AppendLine("	a.ATTRIBUTE_FROM_ID,");
                        prepareDegenerateMappingStatement.AppendLine("	a.ATTRIBUTE_TO_ID");
                        prepareDegenerateMappingStatement.AppendLine("	--a.VERIFICATION");
                        prepareDegenerateMappingStatement.AppendLine("FROM ORIGINAL_ATTRIBUTES a");
                        prepareDegenerateMappingStatement.AppendLine("LEFT OUTER JOIN MAPPED_ATTRIBUTES b ");
                        prepareDegenerateMappingStatement.AppendLine("	ON a.STAGING_AREA_TABLE_ID=b.STAGING_AREA_TABLE_ID ");
                        prepareDegenerateMappingStatement.AppendLine("  AND a.LINK_TABLE_ID=b.LINK_TABLE_ID");
                        prepareDegenerateMappingStatement.AppendLine("  AND a.ATTRIBUTE_FROM_ID=b.ATTRIBUTE_FROM_ID");
                        prepareDegenerateMappingStatement.AppendLine("WHERE b.ATTRIBUTE_TO_ID IS NULL");
                    }
                    var listDegenerateMappings = GetDataTable(ref connOmd, prepareDegenerateMappingStatement.ToString());

                    if (listDegenerateMappings.Rows.Count == 0)
                    {
                        _alert.SetTextLogging("-->  No degenerate columns were detected.\r\n");
                    }
                    else
                    {
                        foreach (DataRow tableName in listDegenerateMappings.Rows)
                        {
                            using (var connection = new SqlConnection(metaDataConnection))
                            {
                                // _alert.SetTextLogging("--> " + tableName["SATELLITE_TABLE_NAME"] + "\r\n");

                                var insertDegenerateMappingStatement = new StringBuilder();

                                insertDegenerateMappingStatement.AppendLine("INSERT INTO [dbo].MD_STG_LINK_ATT_XREF");
                                insertDegenerateMappingStatement.AppendLine("( [STAGING_AREA_TABLE_ID] ,[LINK_TABLE_ID] ,[ATTRIBUTE_ID_FROM] ,[ATTRIBUTE_ID_TO] )");
                                insertDegenerateMappingStatement.AppendLine("VALUES ");
                                insertDegenerateMappingStatement.AppendLine("(");
                                insertDegenerateMappingStatement.AppendLine("  " + tableName["STAGING_AREA_TABLE_ID"] + ",");
                                insertDegenerateMappingStatement.AppendLine("  " + tableName["LINK_TABLE_ID"] + ",");
                                insertDegenerateMappingStatement.AppendLine("  " + tableName["ATTRIBUTE_FROM_ID"] + ",");
                                insertDegenerateMappingStatement.AppendLine("  " + tableName["ATTRIBUTE_TO_ID"]);
                                insertDegenerateMappingStatement.AppendLine(")");

                                var command = new SqlCommand(insertDegenerateMappingStatement.ToString(), connection);

                                try
                                {
                                    connection.Open();
                                    command.ExecuteNonQuery();
                                    degenerateMappingCounter++;
                                }
                                catch (Exception ex)
                                {
                                    errorCounter++;
                                    _alert.SetTextLogging("An issue has occured during preparation of the degenerate attribute metadata. Please check the Error Log for more details.\r\n");
                                    errorLog.AppendLine("\r\nAn issue has occured during preparation of the degenerate attribute metadata: \r\n\r\n" + ex);
                                }
                            }
                        }
                        _alert.SetTextLogging("-->  Processing " + degenerateMappingCounter + " degenerate columns\r\n");
                    }

                    worker.ReportProgress(95);

                    _alert.SetTextLogging("Preparation of the degenerate column metadata completed.\r\n");
                }
                catch (Exception ex)
                {
                    errorCounter++;
                    _alert.SetTextLogging("An issue has occured during preparation of the degenerate attribute metadata. Please check the Error Log for more details.\r\n");
                    errorLog.AppendLine("\r\nAn issue has occured during preparation of the degenerate attribute metadata: \r\n\r\n" + ex);
                }

                #endregion

                #region 14. Multi-Active Key - 97%

                //14. Handle the Multi-Active Key
                _alert.SetTextLogging("\r\n");

                try
                {
                    var prepareMultiKeyStatement = new StringBuilder();

                    if (checkBoxIgnoreVersion.Checked)
                    {
                        _alert.SetTextLogging("Commencing Multi-Active Key handling using database.\r\n");

                        prepareMultiKeyStatement.AppendLine("SELECT ");
                        prepareMultiKeyStatement.AppendLine("   u.STAGING_AREA_TABLE_ID,");
                        prepareMultiKeyStatement.AppendLine("	u.SATELLITE_TABLE_ID,");
                        prepareMultiKeyStatement.AppendLine("	sat.SATELLITE_TABLE_NAME,");
                        prepareMultiKeyStatement.AppendLine("	u.ATTRIBUTE_ID_FROM,");
                        prepareMultiKeyStatement.AppendLine("	u.ATTRIBUTE_ID_TO,");
                        prepareMultiKeyStatement.AppendLine("	att.ATTRIBUTE_NAME");
                        prepareMultiKeyStatement.AppendLine("FROM MD_STG_SAT_ATT_XREF u");
                        prepareMultiKeyStatement.AppendLine("INNER JOIN MD_SAT sat ON sat.SATELLITE_TABLE_ID=u.SATELLITE_TABLE_ID");
                        prepareMultiKeyStatement.AppendLine("INNER JOIN MD_ATT att ON att.ATTRIBUTE_ID = u.ATTRIBUTE_ID_TO");
                        prepareMultiKeyStatement.AppendLine("INNER JOIN ");
                        prepareMultiKeyStatement.AppendLine("(");
                        prepareMultiKeyStatement.AppendLine("  SELECT ");
                        prepareMultiKeyStatement.AppendLine("  	sc.name AS SATELLITE_TABLE_NAME,");
                        prepareMultiKeyStatement.AppendLine("  	C.name AS ATTRIBUTE_NAME");
                        prepareMultiKeyStatement.AppendLine("  FROM " + linkedServer + integrationDatabase + ".sys.index_columns A");
                        prepareMultiKeyStatement.AppendLine("  JOIN " + linkedServer + integrationDatabase + ".sys.indexes B");
                        prepareMultiKeyStatement.AppendLine("    ON A.object_id=B.object_id AND A.index_id=B.index_id");
                        prepareMultiKeyStatement.AppendLine("  JOIN " + linkedServer + integrationDatabase + ".sys.columns C");
                        prepareMultiKeyStatement.AppendLine("    ON A.column_id=C.column_id AND A.object_id=C.object_id");
                        prepareMultiKeyStatement.AppendLine("  JOIN " + linkedServer + integrationDatabase + ".sys.tables sc on sc.object_id = A.object_id");
                        prepareMultiKeyStatement.AppendLine("    WHERE is_primary_key=1");
                        prepareMultiKeyStatement.AppendLine("  AND C.name!='" + effectiveDateTimeAttribute + "' AND C.name!='" + currentRecordAttribute + "' AND C.name!='" + eventDateTimeAtttribute + "'");
                        prepareMultiKeyStatement.AppendLine("  AND C.name NOT LIKE '" + dwhKeyIdentifier + "'");
                        prepareMultiKeyStatement.AppendLine(") ddsub");
                        prepareMultiKeyStatement.AppendLine("ON sat.SATELLITE_TABLE_NAME=ddsub.SATELLITE_TABLE_NAME");
                        prepareMultiKeyStatement.AppendLine("AND att.ATTRIBUTE_NAME=ddsub.ATTRIBUTE_NAME");
                        prepareMultiKeyStatement.AppendLine("  WHERE ddsub.SATELLITE_TABLE_NAME LIKE '" + satTablePrefix + "' OR ddsub.SATELLITE_TABLE_NAME LIKE '" + lsatTablePrefix + "'");
                    }
                    else
                    {
                        _alert.SetTextLogging("Commencing Multi-Active Key handling using model metadata.\r\n");

                        prepareMultiKeyStatement.AppendLine("SELECT ");
                        prepareMultiKeyStatement.AppendLine("   u.STAGING_AREA_TABLE_ID,");
                        prepareMultiKeyStatement.AppendLine("	u.SATELLITE_TABLE_ID,");
                        prepareMultiKeyStatement.AppendLine("	sat.SATELLITE_TABLE_NAME,");
                        prepareMultiKeyStatement.AppendLine("	u.ATTRIBUTE_ID_FROM,");
                        prepareMultiKeyStatement.AppendLine("	u.ATTRIBUTE_ID_TO,");
                        prepareMultiKeyStatement.AppendLine("	att.ATTRIBUTE_NAME");
                        prepareMultiKeyStatement.AppendLine("FROM MD_STG_SAT_ATT_XREF u");
                        prepareMultiKeyStatement.AppendLine("INNER JOIN MD_SAT sat ON sat.SATELLITE_TABLE_ID=u.SATELLITE_TABLE_ID");
                        prepareMultiKeyStatement.AppendLine("INNER JOIN MD_ATT att ON att.ATTRIBUTE_ID = u.ATTRIBUTE_ID_TO");
                        prepareMultiKeyStatement.AppendLine("INNER JOIN ");
                        prepareMultiKeyStatement.AppendLine("(");
                        prepareMultiKeyStatement.AppendLine("	SELECT");
                        prepareMultiKeyStatement.AppendLine("		TABLE_NAME AS SATELLITE_TABLE_NAME,");
                        prepareMultiKeyStatement.AppendLine("		COLUMN_NAME AS ATTRIBUTE_NAME");
                        prepareMultiKeyStatement.AppendLine("	FROM MD_VERSION_ATTRIBUTE");
                        prepareMultiKeyStatement.AppendLine("	WHERE MULTI_ACTIVE_INDICATOR='Y'");
                        prepareMultiKeyStatement.AppendLine("	AND VERSION_ID=" + versionId);
                        prepareMultiKeyStatement.AppendLine(") sub");
                        prepareMultiKeyStatement.AppendLine("ON sat.SATELLITE_TABLE_NAME=sub.SATELLITE_TABLE_NAME");
                        prepareMultiKeyStatement.AppendLine("AND att.ATTRIBUTE_NAME=sub.ATTRIBUTE_NAME");
                    }

                    var listMultiKeys = GetDataTable(ref connOmd, prepareMultiKeyStatement.ToString());

                    if (listMultiKeys.Rows.Count == 0)
                    {
                        _alert.SetTextLogging("-->  No Multi-Active Keys were detected.\r\n");
                    }
                    else
                    {
                        foreach (DataRow tableName in listMultiKeys.Rows)
                        {
                            using (var connection = new SqlConnection(metaDataConnection))
                            {
                                _alert.SetTextLogging("-->  Processing the Multi-Active Key attribute " +
                                                      tableName["ATTRIBUTE_NAME"] + " for " +
                                                      tableName["SATELLITE_TABLE_NAME"] + "\r\n");

                                var updateMultiActiveKeyStatement = new StringBuilder();

                                updateMultiActiveKeyStatement.AppendLine("UPDATE [MD_STG_SAT_ATT_XREF]");
                                updateMultiActiveKeyStatement.AppendLine("SET MULTI_ACTIVE_KEY_INDICATOR='Y'");
                                updateMultiActiveKeyStatement.AppendLine("WHERE STAGING_AREA_TABLE_ID = " + tableName["STAGING_AREA_TABLE_ID"]);
                                updateMultiActiveKeyStatement.AppendLine("AND SATELLITE_TABLE_ID = " + tableName["SATELLITE_TABLE_ID"]);
                                updateMultiActiveKeyStatement.AppendLine("AND ATTRIBUTE_ID_FROM = " + tableName["ATTRIBUTE_ID_FROM"]);
                                updateMultiActiveKeyStatement.AppendLine("AND ATTRIBUTE_ID_TO = " + tableName["ATTRIBUTE_ID_TO"]);


                                var command = new SqlCommand(updateMultiActiveKeyStatement.ToString(), connection);

                                try
                                {
                                    connection.Open();
                                    command.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    errorCounter++;
                                    _alert.SetTextLogging("An issue has occured during preparation of the Multi-Active key metadata. Please check the Error Log for more details.\r\n");
                                    errorLog.AppendLine("\r\nAn issue has occured during preparation of the Multi-Active key metadata: \r\n\r\n" + ex);
                                }
                            }
                        }
                    }
                    worker.ReportProgress(80);
                    _alert.SetTextLogging("Preparation of the Multi-Active Keys completed.\r\n");
                }
                catch (Exception ex)
                {
                    errorCounter++;
                    _alert.SetTextLogging("An issue has occured during preparation of the Multi-Active key metadata. Please check the Error Log for more details.\r\n");
                    errorLog.AppendLine("\r\nAn issue has occured during preparation of the Multi-Active key metadata: \r\n\r\n" + ex);
                }

                #endregion

                #region Driving Key preparation
                //13. Prepare driving keys
                _alert.SetTextLogging("\r\n");
                _alert.SetTextLogging("Commencing preparing the Driving Key metadata.\r\n");

                try
                {
                    var prepareDrivingKeyStatement = new StringBuilder();

                    prepareDrivingKeyStatement.AppendLine("SELECT DISTINCT");
                    prepareDrivingKeyStatement.AppendLine("    -- base.[TABLE_MAPPING_HASH]");
                    prepareDrivingKeyStatement.AppendLine("    --,base.[VERSION_ID]");
                    prepareDrivingKeyStatement.AppendLine("    --,base.[STAGING_AREA_TABLE]");
                    prepareDrivingKeyStatement.AppendLine("    --,base.[BUSINESS_KEY_ATTRIBUTE]");
                    prepareDrivingKeyStatement.AppendLine("       sat.SATELLITE_TABLE_ID");
                    prepareDrivingKeyStatement.AppendLine("    --,base.[INTEGRATION_AREA_TABLE] AS LINK_SATELLITE_TABLE_NAME");
                    prepareDrivingKeyStatement.AppendLine("    --,base.[FILTER_CRITERIA]");
                    prepareDrivingKeyStatement.AppendLine("    --,base.[DRIVING_KEY_ATTRIBUTE]");
                    prepareDrivingKeyStatement.AppendLine("      ,COALESCE(hubkey.HUB_TABLE_ID, (SELECT HUB_TABLE_ID FROM MD_HUB WHERE HUB_TABLE_NAME = 'Not applicable')) AS HUB_TABLE_ID");
                    prepareDrivingKeyStatement.AppendLine("    --,hub.[INTEGRATION_AREA_TABLE] AS [HUB_TABLE]");
                    prepareDrivingKeyStatement.AppendLine("FROM");
                    prepareDrivingKeyStatement.AppendLine("(");
                    prepareDrivingKeyStatement.AppendLine("       SELECT");
                    prepareDrivingKeyStatement.AppendLine("              STAGING_AREA_TABLE,");
                    prepareDrivingKeyStatement.AppendLine("              INTEGRATION_AREA_TABLE,");
                    prepareDrivingKeyStatement.AppendLine("              VERSION_ID,");
                    prepareDrivingKeyStatement.AppendLine("              CASE");
                    prepareDrivingKeyStatement.AppendLine("                     WHEN CHARINDEX('(', RTRIM(LTRIM(Split.a.value('.', 'VARCHAR(MAX)')))) > 0");
                    prepareDrivingKeyStatement.AppendLine("                     THEN RTRIM(LTRIM(Split.a.value('.', 'VARCHAR(MAX)')))");
                    prepareDrivingKeyStatement.AppendLine("                     ELSE REPLACE(RTRIM(LTRIM(Split.a.value('.', 'VARCHAR(MAX)'))), ')', '')");
                    prepareDrivingKeyStatement.AppendLine("              END AS BUSINESS_KEY_ATTRIBUTE--For Driving Key");
                    prepareDrivingKeyStatement.AppendLine("       FROM");
                    prepareDrivingKeyStatement.AppendLine("       (");
                    prepareDrivingKeyStatement.AppendLine("              SELECT STAGING_AREA_TABLE, INTEGRATION_AREA_TABLE, DRIVING_KEY_ATTRIBUTE, VERSION_ID, CONVERT(XML, '<M>' + REPLACE(DRIVING_KEY_ATTRIBUTE, ',', '</M><M>') + '</M>') AS DRIVING_KEY_ATTRIBUTE_XML");
                    prepareDrivingKeyStatement.AppendLine("              FROM");
                    prepareDrivingKeyStatement.AppendLine("              (");
                    prepareDrivingKeyStatement.AppendLine("                     SELECT DISTINCT STAGING_AREA_TABLE, INTEGRATION_AREA_TABLE, VERSION_ID, LTRIM(RTRIM(DRIVING_KEY_ATTRIBUTE)) AS DRIVING_KEY_ATTRIBUTE");
                    prepareDrivingKeyStatement.AppendLine("                     FROM MD_TABLE_MAPPING");
                    prepareDrivingKeyStatement.AppendLine("                     WHERE INTEGRATION_AREA_TABLE LIKE '" + lsatTablePrefix + "' AND DRIVING_KEY_ATTRIBUTE IS NOT NULL AND DRIVING_KEY_ATTRIBUTE != ''");
                    prepareDrivingKeyStatement.AppendLine("                     AND VERSION_ID =" + versionId);
                    prepareDrivingKeyStatement.AppendLine("              ) TableName");
                    prepareDrivingKeyStatement.AppendLine("       ) AS A CROSS APPLY DRIVING_KEY_ATTRIBUTE_XML.nodes('/M') AS Split(a)");
                    prepareDrivingKeyStatement.AppendLine(")  base");
                    prepareDrivingKeyStatement.AppendLine("LEFT JOIN[dbo].[MD_TABLE_MAPPING]");
                    prepareDrivingKeyStatement.AppendLine("        hub");
                    prepareDrivingKeyStatement.AppendLine(" ON  base.STAGING_AREA_TABLE=hub.STAGING_AREA_TABLE");
                    prepareDrivingKeyStatement.AppendLine(" AND hub.INTEGRATION_AREA_TABLE LIKE '" + hubTablePrefix + "'");
                    prepareDrivingKeyStatement.AppendLine("  AND base.BUSINESS_KEY_ATTRIBUTE=hub.BUSINESS_KEY_ATTRIBUTE");
                    prepareDrivingKeyStatement.AppendLine("LEFT JOIN MD_SAT sat");
                    prepareDrivingKeyStatement.AppendLine("  ON base.INTEGRATION_AREA_TABLE = sat.SATELLITE_TABLE_NAME");
                    prepareDrivingKeyStatement.AppendLine("LEFT JOIN MD_HUB hubkey");
                    prepareDrivingKeyStatement.AppendLine("  ON hub.INTEGRATION_AREA_TABLE = hubkey.HUB_TABLE_NAME");
                    prepareDrivingKeyStatement.AppendLine("WHERE base.VERSION_ID = " + versionId);
                    prepareDrivingKeyStatement.AppendLine("AND base.BUSINESS_KEY_ATTRIBUTE IS NOT NULL");
                    prepareDrivingKeyStatement.AppendLine("AND base.BUSINESS_KEY_ATTRIBUTE!=''");

                    var listDrivingKeys = GetDataTable(ref connOmd, prepareDrivingKeyStatement.ToString());

                    if (listDrivingKeys.Rows.Count == 0)
                    {
                        _alert.SetTextLogging("-->  No Driving Key based Link-Satellites were detected.\r\n");
                    }
                    else
                    {
                        foreach (DataRow tableName in listDrivingKeys.Rows)
                        {
                            using (var connection = new SqlConnection(metaDataConnection))
                            {
                                var insertDrivingKeyStatement = new StringBuilder();

                                insertDrivingKeyStatement.AppendLine("INSERT INTO [MD_DRIVING_KEY_XREF]");
                                insertDrivingKeyStatement.AppendLine("( [SATELLITE_TABLE_ID] ,[HUB_TABLE_ID] )");
                                insertDrivingKeyStatement.AppendLine("VALUES ");
                                insertDrivingKeyStatement.AppendLine("(");
                                insertDrivingKeyStatement.AppendLine("  " + tableName["SATELLITE_TABLE_ID"] + ",");
                                insertDrivingKeyStatement.AppendLine("  " + tableName["HUB_TABLE_ID"]);
                                insertDrivingKeyStatement.AppendLine(")");

                                var command = new SqlCommand(insertDrivingKeyStatement.ToString(), connection);

                                try
                                {
                                    connection.Open();
                                    command.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    errorCounter++;
                                    _alert.SetTextLogging(
                                        "An issue has occured during preparation of the Driving Key metadata. Please check the Error Log for more details.\r\n");
                                    errorLog.AppendLine(
                                        "\r\nAn issue has occured during preparation of the Driving Key metadata: \r\n\r\n" +
                                        ex);
                                }
                            }
                        }
                    }

                    worker.ReportProgress(95);
                    _alert.SetTextLogging("Preparation of the degenerate column metadata completed.\r\n");

                }
                catch (Exception ex)
                {
                    errorCounter++;
                    _alert.SetTextLogging("An issue has occured during preparation of the Driving Key metadata. Please check the Error Log for more details.\r\n");
                    errorLog.AppendLine("\r\nAn issue has occured during preparation of the Driving Key metadata: \r\n\r\n" + ex);
                }

                #endregion


                //Completed

                if (errorCounter > 0)
                {
                    _alert.SetTextLogging("\r\nWarning! There were " + errorCounter + " error(s) found while processing the metadata.\r\n");
                    _alert.SetTextLogging("Please check the Error Log for details \r\n");
                    _alert.SetTextLogging("\r\n");
                    //_alert.SetTextLogging(errorLog.ToString());
                    using (var outfile = new StreamWriter(VedwConfigurationSettings.VedwOutputPath + @"\Error_Log.txt"))
                    {
                        outfile.Write(errorLog.ToString());
                        outfile.Close();
                    }
                }
                else
                {
                    _alert.SetTextLogging("\r\nNo errors were detected.\r\n");
                }



                worker.ReportProgress(100);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // Retrieve the current version and store these in local variables
            var versionMajorMinor = GetVersion(trackBarVersioning.Value);
            var majorVersion = versionMajorMinor.Key;
            var minorVersion = versionMajorMinor.Value;

            PrepareMetadata(majorVersion, minorVersion);
        }

        private void PrepareMetadata(int majorVersion, int minorVersion)
        {

            

            if (checkBoxIgnoreVersion.Checked)
            {
                var ignoreVersionDialog =
                    MessageBox.Show(
                        "Selection this option will activate the selected version of the automation metadata against the model (metadata / Data Vault table structures) that is deployed in the live Integration Layer database (" +
                        TeamConfigurationSettings.IntegrationDatabaseName +
                        ").\r\n Model versioning for the Data Vault model will thus be igored. Are you sure this is what you want?",
                        "Model versioning will be ignored", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (ignoreVersionDialog == DialogResult.Yes)
                {
                    //Active the selected version into the MD schema
                    using (new SqlConnection(TeamConfigurationSettings.ConnectionStringOmd))
                    {
                        // Reset back to latest automation metadata / latest model metadata
                        richTextBoxInformation.Clear();
                        richTextBoxInformation.AppendText("Commencing preparation / activation for version " + majorVersion +
                                                          "." + minorVersion + ".\r\n");

                        if (backgroundWorkerActivateMetadata.IsBusy != true)
                        {
                            // create a new instance of the alert form
                            _alert = new Form_Alert();
                            // event handler for the Cancel button in AlertForm
                            _alert.Canceled += buttonCancel_Click;
                            _alert.Show();
                            // Start the asynchronous operation.
                            backgroundWorkerActivateMetadata.RunWorkerAsync();
                        }
                    }
                }
                else
                {
                    checkBoxIgnoreVersion.Checked = false;
                }
            }
            else if (!checkBoxIgnoreVersion.Checked)
            {
                // Reset back to latest automation metadata / latest model metadata
                richTextBoxInformation.Clear();
                richTextBoxInformation.AppendText("Commencing preparation / activation for version " + majorVersion + "." +
                                                  minorVersion + ".\r\n");

                if (checkBoxIgnoreVersion.Checked == false)
                {
                    var versionExistenceCheck = new StringBuilder();

                    versionExistenceCheck.AppendLine("SELECT * FROM MD_VERSION_ATTRIBUTE WHERE VERSION_ID = " + trackBarVersioning.Value);

                    var connOmd = new SqlConnection(TeamConfigurationSettings.ConnectionStringOmd);

                    var versionExistenceCheckDataTable = GetDataTable(ref connOmd, versionExistenceCheck.ToString());

                    if (versionExistenceCheckDataTable != null && versionExistenceCheckDataTable.Rows.Count > 0)
                    {
                        if (backgroundWorkerActivateMetadata.IsBusy) return;
                        // create a new instance of the alert form
                        _alert = new Form_Alert();
                        // event handler for the Cancel button in AlertForm
                        _alert.Canceled += buttonCancel_Click;
                        _alert.Show();
                        // Start the asynchronous operation.
                        backgroundWorkerActivateMetadata.RunWorkerAsync();
                    }
                    else
                    {
                        richTextBoxInformation.Text +=
                            "There is no model metadata available for this version, so the metadata can only be actived with the 'Ignore Version' enabled for this specific version.\r\n ";
                    }
                }
                else
                {
                    if (backgroundWorkerActivateMetadata.IsBusy) return;
                    // create a new instance of the alert form
                    _alert = new Form_Alert();
                    // event handler for the Cancel button in AlertForm
                    _alert.Canceled += buttonCancel_Click;
                    _alert.Show();
                    // Start the asynchronous operation.
                    backgroundWorkerActivateMetadata.RunWorkerAsync();
                }
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

                queryMetadata.AppendLine("SELECT TABLE_SCHEMA,TABLE_NAME " +
                                               "FROM INFORMATION_SCHEMA.TABLES " +
                                               "WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME LIKE '%"+textBoxFilterCriterionPsa.Text+"%'");

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

                queryMetadata.AppendLine("SELECT DISTINCT HUB_TABLE_NAME AS TABLE_NAME ");
                queryMetadata.AppendLine("FROM MD_HUB ");
                queryMetadata.AppendLine("WHERE HUB_TABLE_NAME LIKE '%"+textBoxFilterCriterionHub.Text+"%'");
                queryMetadata.AppendLine("AND HUB_TABLE_NAME != 'Not applicable'");
                queryMetadata.AppendLine("ORDER BY HUB_TABLE_NAME");

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

                queryMetadata.AppendLine("SELECT DISTINCT SATELLITE_TABLE_NAME AS TABLE_NAME ");
                queryMetadata.AppendLine("FROM MD_SAT ");
                queryMetadata.AppendLine("WHERE SATELLITE_TYPE='Normal' AND SATELLITE_TABLE_NAME LIKE '%"+textBoxFilterCriterionSat.Text+"%'");
                queryMetadata.AppendLine("ORDER BY SATELLITE_TABLE_NAME");

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
                _errorDetails.AppendLine("Eerror logging details: " + ex);
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

                queryMetadata.AppendLine("SELECT DISTINCT LINK_TABLE_NAME AS TABLE_NAME ");
                queryMetadata.AppendLine("FROM MD_LINK ");
                queryMetadata.AppendLine("WHERE LINK_TABLE_NAME LIKE '%"+textBoxFilterCriterionLnk.Text+"%'");
                queryMetadata.AppendLine("AND LINK_TABLE_NAME != 'Not applicable'");
                queryMetadata.AppendLine("ORDER BY LINK_TABLE_NAME");

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
                _errorDetails.AppendLine("Verions error logging details: " + ex);
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

                queryMetadata.AppendLine("SELECT DISTINCT SATELLITE_TABLE_NAME AS TABLE_NAME ");
                queryMetadata.AppendLine("FROM MD_SAT ");
                queryMetadata.AppendLine("WHERE SATELLITE_TYPE!='Normal' AND SATELLITE_TABLE_NAME LIKE '%"+textBoxFilterCriterionLsat.Text+"%'");
                queryMetadata.AppendLine(" ORDER BY SATELLITE_TABLE_NAME");
                
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
                _errorDetails.AppendLine("Verions error logging details: " + exception);
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

        private void trackBarVersioning_Scroll(object sender, EventArgs e)
        {
            var versionMajorMinor = GetVersion(trackBarVersioning.Value);
            var majorVersion = versionMajorMinor.Key;
            var minorVersion = versionMajorMinor.Value;

            labelVersion.Text = majorVersion + "." + minorVersion;

            PrepareMetadata(majorVersion, minorVersion);
            //MessageBox.Show(trackBarVersioning.Value.ToString());
        }

        private void unknownKeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is yet to be implemented.", "Upcoming!", MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
        }


        private void tabPageDefaultSettings_Click(object sender, EventArgs e)
        {

        }

        private void groupBoxVersionSelection_Enter(object sender, EventArgs e)
        {

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
            rootPathConfigurationFile.AppendLine("/* End of file */");

            // Save the VEDW core settings file to disk
            using (var outfile = new StreamWriter(GlobalParameters.VedwConfigurationPath + GlobalParameters.VedwConfigurationfileName + GlobalParameters.VedwFileExtension))
            {
                outfile.Write(rootPathConfigurationFile.ToString());
                outfile.Close();
            }

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
            catch (Exception ex)
            {
                MessageBox.Show("The TEAM application cannot be found. Is it installed?");
            }

        }
    }
}
