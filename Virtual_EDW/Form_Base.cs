using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace Virtual_EDW
{
    public partial class Form_Base : Form
    {
        protected FormMain _myParent;

        public Form_Base()
        {
            InitializeComponent();
        }

        public Form_Base(FormMain _myParent)
        {
            this._myParent = _myParent;
            InitializeComponent();
        }




        delegate int GetVersionFromTrackBarCallBack();
        private int GetVersionFromTrackBar()
        {
            if (_myParent.trackBarVersioning.InvokeRequired)
            {
                var d = new GetVersionFromTrackBarCallBack(GetVersionFromTrackBar);
                return Int32.Parse(Invoke(d).ToString());
            }
            else
            {
                return _myParent.trackBarVersioning.Value;
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

        public class GlobalVariables
        {
            // These variables are used as global vairables throughout the applicatoin
            private static string _configurationLocalPath = Application.StartupPath + @"\Configuration\";
            private static string _outputLocalPath = Application.StartupPath + @"\Output\";
            private static string _fileLocalName = "Virtual_EDW_configuration.txt";

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
                get { return _fileLocalName; }
                set { _fileLocalName = value; }
            }

        }

        public KeyValuePair<int, int> GetVersion(int selectedVersion)
        {

            var connOmd = new SqlConnection { ConnectionString = _myParent.textBoxMetadataConnection.Text };

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
            var connOmd = new SqlConnection { ConnectionString = _myParent.textBoxMetadataConnection.Text };
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
            var connOmd = new SqlConnection { ConnectionString = _myParent.textBoxMetadataConnection.Text };
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
