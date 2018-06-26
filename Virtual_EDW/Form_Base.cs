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
                var configurationSettingObject = new ConfigurationSettings();
                configurationSettingObject.ConfigurationPath = configList["ConfigurationPath"];
                configurationSettingObject.OutputPath = configList["OutputPath"];

                // And the same for some of the VEDW specific (i.e. not shared with TEAM) variables
                var configurationSettingObjectVedwSpecific = new ConfigurationSettingsVedwSpecific();
                configurationSettingObjectVedwSpecific.EnableUnicode = configList["EnableUnicode"];
                configurationSettingObjectVedwSpecific.DisableHash = configList["DisableHash"];
                configurationSettingObjectVedwSpecific.HashKeyOutputType = configList["HashKeyOutputType"];
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
                var configurationSettingObject = new ConfigurationSettings();

                configurationSettingObject.ConnectionStringSource = connectionStringSource;
                configurationSettingObject.ConnectionStringStg = connectionStringStg;
                configurationSettingObject.ConnectionStringHstg = connectionStringHstg;
                configurationSettingObject.ConnectionStringInt = connectionStringInt;
                configurationSettingObject.ConnectionStringOmd = connectionStringOmd;
                configurationSettingObject.ConnectionStringPres = connectionStringPres;

                configurationSettingObject.metadataRepositoryType = configList["metadataRepositoryType"];

                configurationSettingObject.StgTablePrefixValue = configList["StagingAreaPrefix"];
                configurationSettingObject.PsaTablePrefixValue = configList["PersistentStagingAreaPrefix"];
                configurationSettingObject.HubTablePrefixValue = configList["HubTablePrefix"];
                configurationSettingObject.SatTablePrefixValue = configList["SatTablePrefix"];
                configurationSettingObject.LinkTablePrefixValue = configList["LinkTablePrefix"];
                configurationSettingObject.LsatPrefixValue = configList["LinkSatTablePrefix"];

                configurationSettingObject.DwhKeyIdentifier = configList["KeyIdentifier"];
                configurationSettingObject.PsaKeyLocation = configList["PSAKeyLocation"];
                configurationSettingObject.TableNamingLocation = configList["TableNamingLocation"];
                configurationSettingObject.KeyNamingLocation = configList["KeyNamingLocation"];

                configurationSettingObject.SchemaName = configList["SchemaName"];
                configurationSettingObject.SourceSystemPrefix = configList["SourceSystemPrefix"];

                configurationSettingObject.EventDateTimeAttribute = configList["EventDateTimeStamp"];
                configurationSettingObject.LoadDateTimeAttribute = configList["LoadDateTimeStamp"];
                configurationSettingObject.ExpiryDateTimeAttribute = configList["ExpiryDateTimeStamp"];
                configurationSettingObject.ChangeDataCaptureAttribute = configList["ChangeDataIndicator"];
                configurationSettingObject.RecordSourceAttribute = configList["RecordSourceAttribute"];
                configurationSettingObject.EtlProcessAttribute = configList["ETLProcessID"];
                configurationSettingObject.EtlProcessUpdateAttribute = configList["ETLUpdateProcessID"];
                configurationSettingObject.RowIdAttribute = configList["RowID"];
                configurationSettingObject.RecordChecksumAttribute = configList["RecordChecksum"];
                configurationSettingObject.CurrentRowAttribute = configList["CurrentRecordAttribute"];
                configurationSettingObject.LogicalDeleteAttribute = configList["LogicalDeleteAttribute"];

                configurationSettingObject.EnableAlternativeRecordSourceAttribute = configList["AlternativeRecordSourceFunction"];
                configurationSettingObject.AlternativeRecordSourceAttribute = configList["AlternativeRecordSource"];

                configurationSettingObject.EnableAlternativeLoadDateTimeAttribute = configList["AlternativeHubLDTSFunction"];
                configurationSettingObject.AlternativeLoadDateTimeAttribute = configList["AlternativeHubLDTS"];

                configurationSettingObject.EnableAlternativeSatelliteLoadDateTimeAttribute = configList["AlternativeSatelliteLDTSFunction"];
                configurationSettingObject.AlternativeSatelliteLoadDateTimeAttribute = configList["AlternativeSatelliteLDTS"];


                configurationSettingObject.SourceDatabaseName = configList["SourceDatabase"];
                configurationSettingObject.StagingDatabaseName = configList["StagingDatabase"];
                configurationSettingObject.PsaDatabaseName = configList["PersistentStagingDatabase"];
                configurationSettingObject.IntegrationDatabaseName = configList["IntegrationDatabase"];
                configurationSettingObject.PresentationDatabaseName = configList["PresentationDatabase"];

                configurationSettingObject.OutputPath = configList["OutputPath"];
                configurationSettingObject.ConfigurationPath = configList["ConfigurationPath"];

                configurationSettingObject.LinkedServer = configList["LinkedServerName"];


            }
            catch (Exception)
            {
                // richTextBoxInformation.AppendText("\r\n\r\nAn error occured while interpreting the configuration file. The original error is: '" + ex.Message + "'");
            }
        }

        public class ConfigurationSettingsVedwSpecific
        {
            // Unicode checkbox
            private static string _enableUnicode;
            public string EnableUnicode
            {
                get { return _enableUnicode; }
                set { _enableUnicode = value; }
            }

            // Disable hash checkbox (use natural business key)
            private static string _disableHash;
            public string DisableHash
            {
                get { return _disableHash; }
                set { _disableHash = value; }
            }

            // Toggle output for the hash (binary or character)
            private static string _hashKeyOutputType;
            public string HashKeyOutputType
            {
                get { return _hashKeyOutputType; }
                set { _hashKeyOutputType = value; }
            }

        }

        public class ConfigurationSettings
        {

            //Prefixes
            private static string _localStgPrefix;
            public string StgTablePrefixValue
            {
                get { return _localStgPrefix; }
                set { _localStgPrefix = value; }
            }

            private static string _localPsaPrefix;
            public string PsaTablePrefixValue
            {
                get { return _localPsaPrefix; }
                set { _localPsaPrefix = value; }
            }

            private static string _localHubPrefix;
            public string HubTablePrefixValue
            {
                get { return _localHubPrefix; }
                set { _localHubPrefix = value; }
            }

            private static string _localSatPrefix;
            public string SatTablePrefixValue
            {
                get { return _localSatPrefix; }
                set { _localSatPrefix = value; }
            }

            private static string _localLnkPrefix;
            public string LinkTablePrefixValue
            {
                get { return _localLnkPrefix; }
                set { _localLnkPrefix = value; }
            }

            private static string _localLsatPrefix;
            public string LsatPrefixValue
            {
                get { return _localLsatPrefix; }
                set { _localLsatPrefix = value; }
            }

            //Connection strings
            private static string _connectionStringSource;
            public string ConnectionStringSource
            {
                get { return _connectionStringSource; }
                set { _connectionStringSource = value; }
            }

            private static string _connectionStringStg;
            public string ConnectionStringStg
            {
                get { return _connectionStringStg; }
                set { _connectionStringStg = value; }
            }

            private static string _connectionStringHstg;
            public string ConnectionStringHstg
            {
                get { return _connectionStringHstg; }
                set { _connectionStringHstg = value; }
            }

            private static string _connectionStringInt;
            public string ConnectionStringInt
            {
                get { return _connectionStringInt; }
                set { _connectionStringInt = value; }
            }

            private static string _connectionStringPres;
            public string ConnectionStringPres
            {
                get { return _connectionStringPres; }
                set { _connectionStringPres = value; }
            }

            private static string _connectionStringOmd;
            public string ConnectionStringOmd
            {
                get { return _connectionStringOmd; }
                set { _connectionStringOmd = value; }
            }



            private static string _dwhKeyIdentifier;
            public string DwhKeyIdentifier
            {
                get { return _dwhKeyIdentifier; }
                set { _dwhKeyIdentifier = value; }
            }

            private static string _psaKeyLocation;
            public string PsaKeyLocation
            {
                get { return _psaKeyLocation; }
                set { _psaKeyLocation = value; }
            }

            private static string _schemaName;
            public string SchemaName
            {
                get { return _schemaName; }
                set { _schemaName = value; }
            }

            private static string _sourceSystemPrefix;
            public string SourceSystemPrefix
            {
                get { return _sourceSystemPrefix; }
                set { _sourceSystemPrefix = value; }
            }

            private static string _eventDateTimeAttribute;
            public string EventDateTimeAttribute
            {
                get { return _eventDateTimeAttribute; }
                set { _eventDateTimeAttribute = value; }
            }

            private static string _loadDateTimeAttribute;
            public string LoadDateTimeAttribute
            {
                get { return _loadDateTimeAttribute; }
                set { _loadDateTimeAttribute = value; }
            }

            private static string _expiryDateTimeAttribute;
            public string ExpiryDateTimeAttribute
            {
                get { return _expiryDateTimeAttribute; }
                set { _expiryDateTimeAttribute = value; }
            }

            private static string _changeDataCaptureAttribute;
            public string ChangeDataCaptureAttribute
            {
                get { return _changeDataCaptureAttribute; }
                set { _changeDataCaptureAttribute = value; }
            }

            private static string _recordSourceAttribute;
            public string RecordSourceAttribute
            {
                get { return _recordSourceAttribute; }
                set { _recordSourceAttribute = value; }
            }

            private static string _etlProcessAttribute;
            public string EtlProcessAttribute
            {
                get { return _etlProcessAttribute; }
                set { _etlProcessAttribute = value; }
            }


            private static string _etlProcessUpdateAttribute;
            public string EtlProcessUpdateAttribute
            {
                get { return _etlProcessUpdateAttribute; }
                set { _etlProcessUpdateAttribute = value; }
            }

            private static string _rowIdAttribute;
            public string RowIdAttribute
            {
                get { return _rowIdAttribute; }
                set { _rowIdAttribute = value; }
            }

            private static string _recordChecksumAttribute;
            public string RecordChecksumAttribute
            {
                get { return _recordChecksumAttribute; }
                set { _recordChecksumAttribute = value; }
            }

            private static string _currentRowAttribute;
            public string CurrentRowAttribute
            {
                get { return _currentRowAttribute; }
                set { _currentRowAttribute = value; }
            }


            private static string _alternativeRecordSourceAttribute;
            public string AlternativeRecordSourceAttribute
            {
                get { return _alternativeRecordSourceAttribute; }
                set { _alternativeRecordSourceAttribute = value; }
            }

            private static string _alternativeLoadDateTimeAttribute;
            public string AlternativeLoadDateTimeAttribute
            {
                get { return _alternativeLoadDateTimeAttribute; }
                set { _alternativeLoadDateTimeAttribute = value; }
            }

            private static string _alternativeSatelliteLoadDateTimeAttribute;
            public string AlternativeSatelliteLoadDateTimeAttribute
            {
                get { return _alternativeSatelliteLoadDateTimeAttribute; }
                set { _alternativeSatelliteLoadDateTimeAttribute = value; }
            }

            private static string _logicalDeleteAttribute;
            public string LogicalDeleteAttribute
            {
                get { return _logicalDeleteAttribute; }
                set { _logicalDeleteAttribute = value; }
            }

            private static string _SourceDatabaseName;
            public string SourceDatabaseName
            {
                get { return _SourceDatabaseName; }
                set { _SourceDatabaseName = value; }
            }

            private static string _StagingDatabaseName;
            public string StagingDatabaseName
            {
                get { return _StagingDatabaseName; }
                set { _StagingDatabaseName = value; }
            }

            private static string _PsaDatabaseName;
            public string PsaDatabaseName
            {
                get { return _PsaDatabaseName; }
                set { _PsaDatabaseName = value; }
            }

            private static string _IntegrationDatabaseName;
            public string IntegrationDatabaseName
            {
                get { return _IntegrationDatabaseName; }
                set { _IntegrationDatabaseName = value; }
            }

            private static string _PresentationDatabaseName;
            public string PresentationDatabaseName
            {
                get { return _PresentationDatabaseName; }
                set { _PresentationDatabaseName = value; }
            }



            private static string _OutputPath;
            public string OutputPath
            {
                get { return _OutputPath; }
                set { _OutputPath = value; }
            }

            private static string _ConfigurationPath;
            public string ConfigurationPath
            {
                get { return _ConfigurationPath; }
                set { _ConfigurationPath = value; }
            }

            private static string _LinkedServer;
            public string LinkedServer
            {
                get { return _LinkedServer; }
                set { _LinkedServer = value; }
            }



            private static string _TableNamingLocation;
            public string TableNamingLocation
            {
                get { return _TableNamingLocation; }
                set { _TableNamingLocation = value; }
            }

            private static string _KeyNamingLocation;
            public string KeyNamingLocation
            {
                get { return _KeyNamingLocation; }
                set { _KeyNamingLocation = value; }
            }



            private static string _EnableAlternativeSatelliteLoadDateTimeAttribute;
            public string EnableAlternativeSatelliteLoadDateTimeAttribute
            {
                get { return _EnableAlternativeSatelliteLoadDateTimeAttribute; }
                set { _EnableAlternativeSatelliteLoadDateTimeAttribute = value; }
            }
            private static string _EnableAlternativeRecordSourceAttribute;
            public string EnableAlternativeRecordSourceAttribute
            {
                get { return _EnableAlternativeRecordSourceAttribute; }
                set { _EnableAlternativeRecordSourceAttribute = value; }
            }

            private static string _EnableAlternativeLoadDateTimeAttribute;
            public string EnableAlternativeLoadDateTimeAttribute
            {
                get { return _EnableAlternativeLoadDateTimeAttribute; }
                set { _EnableAlternativeLoadDateTimeAttribute = value; }
            }

            private static string _metadataRepositoryType;
            public string metadataRepositoryType
            {
                get { return _metadataRepositoryType; }
                set { _metadataRepositoryType = value; }
            }
        }

        public class GlobalParameters
        {
            // These variables are used as global vairables throughout the applicatoin
            private static string _configurationLocalPath = Application.StartupPath + @"\Configuration\";
            private static string _outputLocalPath = Application.StartupPath + @"\Output\";

            private static string _fileConfigLocalName = "TEAM_configuration.txt";
            private static string _filePathLocalName = "TEAM_Path_configuration.txt";

            private static string _jsonTableMappingFileName = "TEAM_Table_Mapping.json";
            private static string _jsonAttributeMappingFileName = "TEAM_Attribute_Mapping.json";

            public static string ConfigurationPath
            {
                get { return _configurationLocalPath; }
                set { _configurationLocalPath = value; }
            }

            public static string OutputPath
            {
                get { return _outputLocalPath; }
                set { _outputLocalPath = value; }
            }

            public static string ConfigfileName
            {
                get { return _fileConfigLocalName; }
                set { _fileConfigLocalName = value; }
            }

            public static string PathfileName
            {
                get { return _filePathLocalName; }
                set { _filePathLocalName = value; }
            }

            public static string jsonTableMappingFileName
            {
                get { return _jsonTableMappingFileName; }
                set { _jsonTableMappingFileName = value; }
            }
            public static string jsonAttributeMappingFileName
            {
                get { return _jsonAttributeMappingFileName; }
                set { _jsonAttributeMappingFileName = value; }
            }
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
            var configurationSettings = new ConfigurationSettings();


            var connOmd = new SqlConnection { ConnectionString = configurationSettings.ConnectionStringOmd };

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

        protected int GetMaxVersionId()
        {
            var configurationSettings = new ConfigurationSettings();

            var connOmd = new SqlConnection { ConnectionString = configurationSettings.ConnectionStringOmd };
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
            var configurationSettings = new ConfigurationSettings();

            var connOmd = new SqlConnection { ConnectionString = configurationSettings.ConnectionStringOmd };
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
