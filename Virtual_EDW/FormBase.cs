using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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


        /// <summary>
        /// Application specific global parameters - not meant to be updated via the software
        /// </summary>
        internal static class GlobalParameters
        {
            // VEDW core path parameters, not meant to be updated
            public static string VedwConfigurationPath { get; } = Application.StartupPath + @"\Configuration\";
            public static string VedwConfigurationfileName { get; } = "VEDW_configuration";
            public static string VedwFileExtension { get; } = ".txt";


            // TEAM core file names, not meant to be updated
            public static string TeamConfigurationfileName { get; } = "TEAM_configuration";
            public static string TeamPathfileName { get; } = "TEAM_Path_configuration";
        }


        /// <summary>
        /// These are the VEDW specific configuration settings (i.e. not TEAM driven)
        /// Elements in this class are saved to / retrieved from the VEDW Core Settings file
        /// </summary>
        internal static class VedwConfigurationSettings
        {
            public static string EnableUnicode { get; set; } // Unicode checkbox
            public static string DisableHash { get; set; } // Disable hash checkbox (use natural business key)
            public static string HashKeyOutputType { get; set; } // Toggle output for the hash (binary or character)
            public static string VedwSchema { get; set; } = "dbo";
            public static string TeamConfigurationPath { get; set; } = Application.StartupPath + @"\Configuration\";
            public static string VedwOutputPath { get; set; } = Application.StartupPath + @"\Configuration\";
            public static string WorkingEnvironment { get; set; }

            // Parameters that can be changed at runtime
            public static string hashingStartSnippet { get; set; }
            public static string hashingEndSnippet { get; set; }
            public static string hashingCollation { get; set; }
            public static string hashingZeroKey { get; set; }

            public static List<LoadPattern> patternList { get; set; }
            public static string activeLoadPatternHub { get; set; }
            public static string activeLoadPatternSat { get; set; }
    }

        /// <summary>
        /// These settings are driven by the TEAM application, so can't be saved from VEDW.
        /// They have to be updated through TEAM, i.e. via the Team Configuration / Settings file in the designated directory.
        /// </summary>
        internal static class TeamConfigurationSettings
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

            internal static string MetadataDatabaseName { get; set; }

            internal static string PhysicalModelServerName { get; set; }

            internal static string MetadataServerName { get; set; }

            internal static string LinkedServer { get; set; }

            internal static string TableNamingLocation { get; set; }

            internal static string KeyNamingLocation { get; set; }

            internal static string EnableAlternativeSatelliteLoadDateTimeAttribute { get; set; }

            internal static string EnableAlternativeRecordSourceAttribute { get; set; }

            internal static string EnableAlternativeLoadDateTimeAttribute { get; set; }

            internal static string MetadataRepositoryType { get; set; }
        }


        /// <summary>
        /// Load a data set into an in-memory datatable
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
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
               return null;
            }
            return dataTable;
        }



        public KeyValuePair<int, int> GetVersion(int selectedVersion)
        {

            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };

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
            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
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
            var connOmd = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };
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
