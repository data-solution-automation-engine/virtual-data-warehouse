using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Virtual_EDW
{
    public partial class FormBase : Form
    {
        protected FormMain MyParent;

        public FormBase()
        {
            InitializeComponent();
        }

        public FormBase(FormMain myParent)
        {
            MyParent = myParent;
            InitializeComponent();
        }


        delegate int GetVersionFromTrackBarCallBack();
        private int GetVersionFromTrackBar()
        {
            if (MyParent.trackBarVersioning.InvokeRequired)
            {
                var d = new GetVersionFromTrackBarCallBack(GetVersionFromTrackBar);
                return Int32.Parse(Invoke(d).ToString());
            }
            else
            {
                return MyParent.trackBarVersioning.Value;
            }
        }


        public static void InitialiseRootPath()
        {
            // This is the hardcoded base path that always needs to be accessible, it has the main file which can locate the rest of the configuration
            var configList = new Dictionary<string, string>();
            var fs = new FileStream(GlobalParameters.ConfigurationPath + GlobalParameters.PathfileName, FileMode.Open, FileAccess.Read);
            var sr = new StreamReader(fs);

            try
            {
                string textline;
                while ((textline = sr.ReadLine()) != null)
                {
                    if (textline.IndexOf(@"/*", StringComparison.Ordinal) == -1)
                    {
                        var line = textline.Split('|');
                        configList.Add(line[0], line[1]);
                    }
                }

                sr.Close();
                fs.Close();

                // These variables are used as global vairables throughout the application
                ConfigurationSettings.ConfigurationPath = configList["ConfigurationPath"];
                ConfigurationSettings.OutputPath = configList["OutputPath"];

                // And the same for some of the VEDW specific (i.e. not shared with TEAM) variables
                ConfigurationSettingsVedwSpecific.EnableUnicode = configList["EnableUnicode"];
                ConfigurationSettingsVedwSpecific.DisableHash = configList["DisableHash"];
                ConfigurationSettingsVedwSpecific.HashKeyOutputType = configList["HashKeyOutputType"];
            }
            catch (Exception)
            {
                // richTextBoxInformation.AppendText("\r\n\r\nAn error occured while interpreting the configuration file. The original error is: '" + ex.Message + "'");
            }
        }

        public static void InitialiseConfiguration(string chosenFile)
        {
            try
            {
                var configList = new Dictionary<string, string>();
                var fs = new FileStream(chosenFile, FileMode.Open, FileAccess.Read);
                var sr = new StreamReader(fs);

                string textline;
                while ((textline = sr.ReadLine()) != null)
                {
                    if (textline.IndexOf(@"/*", StringComparison.Ordinal) == -1)
                    {
                        var line = textline.Split('|');
                        configList.Add(line[0], line[1]);
                    }
                }

                sr.Close();
                fs.Close();

                var connectionStringOmd = configList["connectionStringMetadata"];
                connectionStringOmd = connectionStringOmd.Replace("Provider=SQLNCLI10;", "").Replace("Provider=SQLNCLI11;", "").Replace("Provider=SQLNCLI12;", "");

                var connectionStringSource = configList["connectionStringSource"];
                connectionStringSource = connectionStringSource.Replace("Provider=SQLNCLI10;", "").Replace("Provider=SQLNCLI11;", "").Replace("Provider=SQLNCLI12;", "");

                var connectionStringStg = configList["connectionStringStaging"];
                connectionStringStg = connectionStringStg.Replace("Provider=SQLNCLI10;", "").Replace("Provider=SQLNCLI11;", "").Replace("Provider=SQLNCLI12;", "");

                var connectionStringHstg = configList["connectionStringPersistentStaging"];
                connectionStringHstg = connectionStringHstg.Replace("Provider=SQLNCLI10;", "").Replace("Provider=SQLNCLI11;", "").Replace("Provider=SQLNCLI12;", "");

                var connectionStringInt = configList["connectionStringIntegration"];
                connectionStringInt = connectionStringInt.Replace("Provider=SQLNCLI10;", "").Replace("Provider=SQLNCLI11;", "").Replace("Provider=SQLNCLI12;", "");

                var connectionStringPres = configList["connectionStringPresentation"];
                connectionStringPres = connectionStringPres.Replace("Provider=SQLNCLI10;", "").Replace("Provider=SQLNCLI11;", "").Replace("Provider=SQLNCLI12;", "");

                // These variables are used as global vairables throughout the application
                // They will be set once after startup

                ConfigurationSettings.ConnectionStringSource = connectionStringSource;
                ConfigurationSettings.ConnectionStringStg = connectionStringStg;
                ConfigurationSettings.ConnectionStringHstg = connectionStringHstg;
                ConfigurationSettings.ConnectionStringInt = connectionStringInt;
                ConfigurationSettings.ConnectionStringOmd = connectionStringOmd;
                ConfigurationSettings.ConnectionStringPres = connectionStringPres;

                //ConfigurationSetting.metadataRepositoryType = configList["metadataRepositoryType"];

                ConfigurationSettings.StgTablePrefixValue = configList["StagingAreaPrefix"];
                ConfigurationSettings.PsaTablePrefixValue = configList["PersistentStagingAreaPrefix"];
                ConfigurationSettings.HubTablePrefixValue = configList["HubTablePrefix"];
                ConfigurationSettings.SatTablePrefixValue = configList["SatTablePrefix"];
                ConfigurationSettings.LinkTablePrefixValue = configList["LinkTablePrefix"];
                ConfigurationSettings.LsatPrefixValue = configList["LinkSatTablePrefix"];
                ConfigurationSettings.DwhKeyIdentifier = configList["KeyIdentifier"];
                ConfigurationSettings.PsaKeyLocation = configList["PSAKeyLocation"];
                ConfigurationSettings.TableNamingLocation = configList["TableNamingLocation"];
                ConfigurationSettings.KeyNamingLocation = configList["KeyNamingLocation"];
                ConfigurationSettings.SchemaName = configList["SchemaName"];
                ConfigurationSettings.SourceSystemPrefix = configList["SourceSystemPrefix"];

                ConfigurationSettings.EventDateTimeAttribute = configList["EventDateTimeStamp"];
                ConfigurationSettings.LoadDateTimeAttribute = configList["LoadDateTimeStamp"];
                ConfigurationSettings.ExpiryDateTimeAttribute = configList["ExpiryDateTimeStamp"];
                ConfigurationSettings.ChangeDataCaptureAttribute = configList["ChangeDataIndicator"];
                ConfigurationSettings.RecordSourceAttribute = configList["RecordSourceAttribute"];
                ConfigurationSettings.EtlProcessAttribute = configList["ETLProcessID"];
                ConfigurationSettings.EtlProcessUpdateAttribute = configList["ETLUpdateProcessID"];
                ConfigurationSettings.RowIdAttribute = configList["RowID"];
                ConfigurationSettings.RecordChecksumAttribute = configList["RecordChecksum"];
                ConfigurationSettings.CurrentRowAttribute = configList["CurrentRecordAttribute"];
                ConfigurationSettings.LogicalDeleteAttribute = configList["LogicalDeleteAttribute"];
                ConfigurationSettings.EnableAlternativeRecordSourceAttribute = configList["AlternativeRecordSourceFunction"];
                ConfigurationSettings.AlternativeRecordSourceAttribute = configList["AlternativeRecordSource"];
                ConfigurationSettings.EnableAlternativeLoadDateTimeAttribute = configList["AlternativeHubLDTSFunction"];
                ConfigurationSettings.AlternativeLoadDateTimeAttribute = configList["AlternativeHubLDTS"];
                
                ConfigurationSettings.EnableAlternativeSatelliteLoadDateTimeAttribute = configList["AlternativeSatelliteLDTSFunction"];
                ConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute = configList["AlternativeSatelliteLDTS"];
                ConfigurationSettings.SourceDatabaseName = configList["SourceDatabase"];
                ConfigurationSettings.StagingDatabaseName = configList["StagingDatabase"];
                ConfigurationSettings.PsaDatabaseName = configList["PersistentStagingDatabase"];
                ConfigurationSettings.IntegrationDatabaseName = configList["IntegrationDatabase"];
                ConfigurationSettings.PresentationDatabaseName = configList["PresentationDatabase"];
                ConfigurationSettings.OutputPath = configList["OutputPath"];
                ConfigurationSettings.ConfigurationPath = configList["ConfigurationPath"];
                ConfigurationSettings.LinkedServer = configList["LinkedServerName"];


            }
            catch (Exception)
            {
                // richTextBoxInformation.AppendText("\r\n\r\nAn error occured while interpreting the configuration file. The original error is: '" + ex.Message + "'");
            }
        }


        /// <summary>
        /// These are the VEDW specific configuration settings (i.e. not TEAM driven)
        /// </summary>
        internal static class ConfigurationSettingsVedwSpecific
        {
            // Unicode checkbox
            public static string EnableUnicode { get; set; }

            // Disable hash checkbox (use natural business key)
            public static string DisableHash { get; set; }

            // Toggle output for the hash (binary or character)
            public static string HashKeyOutputType { get; set; }
        }

        /// <summary>
        /// These settings are driven by the TEAM application, so can't be saved from VEDW.
        /// They have to be updated through TEAM.
        /// </summary>
        internal static class ConfigurationSettings
        {
            //Prefixes
            internal static string StgTablePrefixValue { get; set; }
            internal static string PsaTablePrefixValue { get; set; }
            internal static string HubTablePrefixValue { get; set; }
            internal static string SatTablePrefixValue { get; set; }
            internal static string LinkTablePrefixValue { get; set; }
            internal static string LsatPrefixValue { get; set; }

            //Connection strings
            internal static string ConnectionStringSource { get; set; }
            internal static string ConnectionStringStg { get; set; }
            internal static string ConnectionStringHstg { get; set; }
            internal static string ConnectionStringInt { get; set; }
            internal static string ConnectionStringPres { get; set; }
            internal static string ConnectionStringOmd { get; set; }

            internal static string DwhKeyIdentifier { get; set; }
            internal static string PsaKeyLocation { get; set; }
            internal static string SchemaName { get; set; }

            internal static string SourceSystemPrefix { get; set; }

            internal static string EventDateTimeAttribute { get; set; }

            internal static string LoadDateTimeAttribute { get; set; }

            internal static string ExpiryDateTimeAttribute { get; set; }

            internal static string ChangeDataCaptureAttribute { get; set; }

            internal static string RecordSourceAttribute { get; set; }

            internal static string EtlProcessAttribute { get; set; }


            internal static string EtlProcessUpdateAttribute { get; set; }

            internal static string RowIdAttribute { get; set; }

            internal static string RecordChecksumAttribute { get; set; }

            internal static string CurrentRowAttribute { get; set; }


            internal static string AlternativeRecordSourceAttribute { get; set; }

            internal static string AlternativeLoadDateTimeAttribute { get; set; }

            internal static string AlternativeSatelliteLoadDateTimeAttribute { get; set; }

            internal static string LogicalDeleteAttribute { get; set; }

            internal static string SourceDatabaseName { get; set; }

            internal static string StagingDatabaseName { get; set; }

            internal static string PsaDatabaseName { get; set; }

            internal static string IntegrationDatabaseName { get; set; }

            internal static string PresentationDatabaseName { get; set; }

            internal static string OutputPath { get; set; }

            internal static string ConfigurationPath { get; set; }

            internal static string LinkedServer { get; set; }

            internal static string TableNamingLocation { get; set; }

            internal static string KeyNamingLocation { get; set; }

            internal static string EnableAlternativeSatelliteLoadDateTimeAttribute { get; set; }

            internal static string EnableAlternativeRecordSourceAttribute { get; set; }

            internal static string EnableAlternativeLoadDateTimeAttribute { get; set; }

            internal static string MetadataRepositoryType { get; set; }

            internal static string WorkingEnvironment { get; set; }
        }

        internal static class GlobalParameters
        {
            // TEAM core path parameters
            public static string ConfigurationPath { get; set; } = Application.StartupPath + @"\Configuration\";
            public static string OutputPath { get; set; } = Application.StartupPath + @"\Output\";

            public static string ConfigfileName { get; set; } = "TEAM_configuration";
            public static string PathfileName { get; set; } = "TEAM_Path_configuration";
            public static string ValidationFileName { get; set; } = "TEAM_validation";
            public static string FileExtension { get; set; } = ".txt";

            // Json file name parameters
            public static string JsonTableMappingFileName { get; set; } = "TEAM_Table_Mapping";
            public static string JsonAttributeMappingFileName { get; set; } = "TEAM_Attribute_Mapping";
            public static string JsonModelMetadataFileName { get; set; } = "TEAM_Model_Metadata";
            public static string JsonExtension { get; set; } = ".json";
        }


        public DataTable GetDataTable(ref SqlConnection sqlConnection, string sql)
        {
            // Pass the connection to a command object
            var sqlCommand = new SqlCommand(sql, sqlConnection);
            var sqlDataAdapter = new SqlDataAdapter { SelectCommand = sqlCommand };

            var dataTable = new DataTable();

            // Adds or refreshes rows in the DataSet to match those in the data source

            try
            {
                sqlDataAdapter.Fill(dataTable);
            }

            catch (Exception)
            {
              //  MessageBox.Show(@"SQL error: " + exception.Message + "\r\n\r\n The executed query was: " + sql + "\r\n\r\n The connection used was " + sqlConnection.ConnectionString, "An issue has been encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return dataTable;
        }



        public KeyValuePair<int, int> GetVersion(int selectedVersion)
        {

            var connOmd = new SqlConnection { ConnectionString = ConfigurationSettings.ConnectionStringOmd };

            var currentVersion = selectedVersion;
            var majorRelease = new int();
            var minorRelease = new int();

            try
            {
                connOmd.Open();
            }
            catch (Exception)
            {
                //richTextBoxInformation.Text += exception.Message;
            }

            var sqlStatementForVersion = new StringBuilder();

            sqlStatementForVersion.AppendLine("SELECT VERSION_ID, MAJOR_RELEASE_NUMBER, MINOR_RELEASE_NUMBER");
            sqlStatementForVersion.AppendLine("FROM MD_VERSION");
            sqlStatementForVersion.AppendLine("WHERE VERSION_ID = " + currentVersion);

            var versionList = GetDataTable(ref connOmd, sqlStatementForVersion.ToString());

            if (versionList != null)
            {
                foreach (DataRow version in versionList.Rows)
                {
                    majorRelease = (int) version["MAJOR_RELEASE_NUMBER"];
                    minorRelease = (int) version["MINOR_RELEASE_NUMBER"];
                }

                if (majorRelease.Equals(null))
                {
                    majorRelease = 0;
                }

                if (minorRelease.Equals(null))
                {
                    minorRelease = 0;
                }

                return new KeyValuePair<int, int>(majorRelease, minorRelease);
            }
            else
            {
                return new KeyValuePair<int, int>(0, 0);
            }
        }

        /// <summary>
        /// Display the most recent version in order to display the most recent version
        /// </summary>
        /// <returns>The version ID as an integer</returns>
        protected int GetMaxVersionId()
        {
            var connOmd = new SqlConnection { ConnectionString = ConfigurationSettings.ConnectionStringOmd };
            var versionId = new int();

            try
            {
                connOmd.Open();
            }
            catch (Exception)
            {
               // richTextBoxInformation.Text += exception.Message;
            }

            var sqlStatementForVersion = new StringBuilder();

            sqlStatementForVersion.AppendLine("SELECT COALESCE(MAX(VERSION_ID),0) AS VERSION_ID");
            sqlStatementForVersion.AppendLine("FROM MD_VERSION");

            var versionList = GetDataTable(ref connOmd, sqlStatementForVersion.ToString());

            if (versionList!= null)
            {
                foreach (DataRow version in versionList.Rows)
                {
                    versionId = (int) version["VERSION_ID"];
                }
                return versionId;
            }
            else
            {
                return 0;
            }
         
        }
        protected int GetVersionCount()
        {
            var connOmd = new SqlConnection { ConnectionString = ConfigurationSettings.ConnectionStringOmd };
            var versionCount = new int();

            try
            {
                connOmd.Open();
            }
            catch (Exception)
            {
                //richTextBoxInformation.Text += exception.Message;
            }

            var sqlStatementForVersion = new StringBuilder();

            sqlStatementForVersion.AppendLine("SELECT COUNT(*) AS VERSION_COUNT");
            sqlStatementForVersion.AppendLine("FROM MD_VERSION");

            var versionList = GetDataTable(ref connOmd, sqlStatementForVersion.ToString());

            if (versionList != null)
            {
                if (versionList.Rows.Count == 0)
                {
                    //richTextBoxInformation.Text += "The version cannot be established.\r\n";
                }
                else
                {
                    foreach (DataRow version in versionList.Rows)
                    {
                        versionCount = (int) version["VERSION_COUNT"];
                    }
                }

                return versionCount;
            }
            else
            {
                return 0;
            }
        }

        private void Form_Base_Load(object sender, EventArgs e)
        {

        }
    }
}
