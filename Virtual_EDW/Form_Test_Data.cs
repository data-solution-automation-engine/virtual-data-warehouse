using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Virtual_EDW
{
    public partial class FormTestData : FormBase
    {
        // Make parent form controls accessible (create instance of Form Main)
        //private readonly FormMain _myParent;

        public FormTestData(FormMain parent)
        {
            MyParent = parent;
            InitializeComponent();

            radioButtonStagingArea.Checked=true;
            DebuggingTextbox.AutoSize = true;
        }

        public int GetRandomNumber(int maxNumber)
        {
            if (maxNumber < 1)
                throw new Exception("The maxNumber value should be greater than 1");
            var b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            var seed = (b[0] & 0x7f) << 24 | b[1] << 16 | b[2] << 8 | b[3];
            var r = new Random(seed);
            return r.Next(1, maxNumber);
        }

        public DateTime GetRandomDate(int startYear = 1995)
        {
            var start = new DateTime(startYear, 1, 1);
            var range = (DateTime.Today - start).Days;
            var b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            var seed = (b[0] & 0x7f) << 24 | b[1] << 16 | b[2] << 8 | b[3];
            return start.AddDays(new Random(seed).Next(1, range)).AddSeconds(new Random(seed).Next(1,86400));
        } 

        public string GetRandomString(int length)
        {
            var array = new[]
	        {
		        "0","2","3","4","5","6","8","9",
		        "a","b","c","d","e","f","g","h","j","k","m","n","p","q","r","s","t","u","v","w","x","y","z",
		        "A","B","C","D","E","F","G","H","J","K","L","M","N","P","R","S","T","U","V","W","X","Y","Z"
	        };
            var sb = new StringBuilder();
            for (var i = 0; i < length; i++) sb.Append(array[GetRandomNumber(53)]);
            return sb.ToString();
        }

        private void buttonGenerateTestcases_Click(object sender, EventArgs e)
        {
            var configurationSettings = new ConfigurationSettings();

            var connStg = new SqlConnection();
            var connVariable = new SqlConnection();

            // Assign database connection string
            connStg.ConnectionString = configurationSettings.ConnectionStringStg;

            try
            {
                connStg.Open();

                var testCaseQuery = new StringBuilder();

                testCaseQuery.AppendLine("--");
                testCaseQuery.AppendLine("-- Test Data");
                testCaseQuery.AppendLine("-- Generated at " + DateTime.Now);
                testCaseQuery.AppendLine("--");

                const string queryTableArray =
                    "SELECT TABLE_SCHEMA,TABLE_NAME, ROW_NUMBER() OVER (ORDER BY TABLE_NAME) as ROW_NR " +
                    "FROM INFORMATION_SCHEMA.TABLES " +
                    "WHERE TABLE_TYPE='BASE TABLE' AND TABLE_NAME NOT LIKE '%_USERMANAGED_%'";

                var tables = MyParent.GetDataTable(ref connStg, queryTableArray);

                if (tables != null)
                {
                    DebuggingTextbox.Clear();
                    foreach (DataRow row in tables.Rows)
                    {
                        var stgTableName = (string) row["TABLE_NAME"];

                        if (checkBoxTruncate.Checked)
                        {
                            testCaseQuery.AppendLine();
                            testCaseQuery.AppendLine("-- Truncating the target table " + stgTableName);
                            testCaseQuery.AppendLine("TRUNCATE TABLE " + stgTableName);
                            testCaseQuery.AppendLine();
                        }

                        testCaseQuery.AppendLine();
                        testCaseQuery.AppendLine("-- Creating testcases for " + stgTableName);
                        testCaseQuery.AppendLine();

                        var localkeyLength = configurationSettings.DwhKeyIdentifier.Length;
                        var localkeySubstring = configurationSettings.DwhKeyIdentifier.Length + 1;

                        var queryAttributeArray =
                            "SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION " +
                            "FROM INFORMATION_SCHEMA.COLUMNS " +
                            "WHERE SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" + localkeyLength + "," + localkeySubstring + ")!='_" + configurationSettings.DwhKeyIdentifier + "'" +
                            " AND TABLE_NAME= '" + stgTableName + "'" +
                            " AND COLUMN_NAME NOT IN ('" + configurationSettings.RecordSourceAttribute + "','" +
                            configurationSettings.AlternativeRecordSourceAttribute + "','" +
                            configurationSettings.AlternativeLoadDateTimeAttribute + "','" +
                            configurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "','" +
                            configurationSettings.EtlProcessAttribute + "','" +
                            configurationSettings.EventDateTimeAttribute + "','" +
                            configurationSettings.ChangeDataCaptureAttribute + "','" +
                            configurationSettings.RecordChecksumAttribute + "','" +
                            configurationSettings.RowIdAttribute + "','" +
                            configurationSettings.LoadDateTimeAttribute + "')";

                        var attributeArray = MyParent.GetDataTable(ref connStg, queryAttributeArray);

                        for (var intCounter = 1; intCounter <= Convert.ToInt32(textBoxTestCaseAmount.Text); intCounter++)
                        {
                            testCaseQuery.AppendLine("-- Testcase " + intCounter);
                            testCaseQuery.AppendLine("INSERT INTO [dbo].[" + stgTableName + "]");
                            testCaseQuery.AppendLine("(");
                            testCaseQuery.AppendLine("[" + configurationSettings.EtlProcessAttribute + "],");
                            testCaseQuery.AppendLine("[" + configurationSettings.EventDateTimeAttribute + "],");
                            testCaseQuery.AppendLine("[" + configurationSettings.RecordSourceAttribute + "],");
                            testCaseQuery.AppendLine("[" + configurationSettings.ChangeDataCaptureAttribute + "],");
                            testCaseQuery.AppendLine("[" + configurationSettings.RecordChecksumAttribute + "],");

                            foreach (DataRow attributeRow in attributeArray.Rows)
                            {
                                testCaseQuery.AppendLine("["+(string) attributeRow["COLUMN_NAME"] + "],");
                            }

                            testCaseQuery.Remove(testCaseQuery.Length - 3, 3);
                            testCaseQuery.AppendLine();
                            testCaseQuery.AppendLine(")");
                            testCaseQuery.AppendLine("VALUES");
                            testCaseQuery.AppendLine("(");

                            testCaseQuery.AppendLine("-1,");
                            testCaseQuery.AppendLine("GETDATE(),");
                            testCaseQuery.AppendLine("'Testcases',");
                            testCaseQuery.AppendLine("'Insert',");
                            testCaseQuery.AppendLine("'N/A',");

                            foreach (DataRow attributeRow in attributeArray.Rows)
                            {
                                var localAttribute = attributeRow["DATA_TYPE"].ToString();

                                if (localAttribute == "numeric" || localAttribute == "int")
                                {
                                    testCaseQuery.AppendLine(
                                        GetRandomNumber(Convert.ToInt32(attributeRow["NUMERIC_PRECISION"].ToString())) + ",");
                                }
                                else if (localAttribute == "varchar" || localAttribute == "nvarchar")
                                {
                                    testCaseQuery.AppendLine("'" +
                                                             GetRandomString(
                                                                 Convert.ToInt32(
                                                                     attributeRow["CHARACTER_MAXIMUM_LENGTH"].ToString())) +
                                                             "',");
                                }
                                else if (localAttribute == "datetime2" || localAttribute == "datetime")
                                {
                                    testCaseQuery.AppendLine("'" + GetRandomDate(1975).ToString("yyyy-MM-dd HH:mm:ss.fff") + "',");
                                }
                                else
                                {
                                    DebuggingTextbox.Text+=("Issue encountered, the datatype " +
                                                    attributeRow["DATA_TYPE"] + " is not supported. The attribute is " + attributeRow["COLUMN_NAME"] + " of table " + stgTableName + ".\n\r");
                                }
                            }
                            testCaseQuery.Remove(testCaseQuery.Length - 3, 3);
                            testCaseQuery.AppendLine();
                            testCaseQuery.AppendLine(")");
                            testCaseQuery.AppendLine();
                        }

                        if (checkBoxGenerateInDatabase.Checked)
                        {
                            if (radioButtonStagingArea.Checked)
                            {
                                connVariable.ConnectionString = configurationSettings.ConnectionStringStg;
                            }
                            if (radioButtonPSA.Checked)
                            {
                                connVariable.ConnectionString = configurationSettings.ConnectionStringHstg;
                            }
                            if (radiobuttonSource.Checked)
                            {
                                connVariable.ConnectionString = configurationSettings.ConnectionStringSource;
                                    //_myParent.textBoxSourceConnection.Text;
                            }

                            try
                            {
                                MyParent.GenerateInDatabase(connVariable, testCaseQuery.ToString());
                                richTextBoxOutput.Text = "The " + textBoxTestCaseAmount.Text +
                                                         " testcases were created in the designated database.";
                            }
                            catch (Exception ex)
                            {
                                richTextBoxOutput.Text =
                                    "Errors in executing the SQL statement against the database. The error message is " + ex;
                            }
                        }

                        DebuggingTextbox.Text += testCaseQuery.ToString();
                    }
                }
                else
                {
                    DebuggingTextbox.Text = "There is no metadata to process";
                }
            }
            catch (Exception exception)
            {
                DebuggingTextbox.Text = "There was an error connecting to the Staging Area database. \r\n\r\nA connection could not be established. Can you verify the connection details for the Staging Area in the main screen? \r\n\r\nThe error message is: " + exception.Message;
            } 
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
