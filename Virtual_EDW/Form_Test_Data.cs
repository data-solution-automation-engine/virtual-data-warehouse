using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

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
            textBoxOutput.AutoSize = true;
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
            var connStg = new SqlConnection
            {
                ConnectionString = TeamConfigurationSettings.ConnectionStringStg
            };

            var connMetadata = new SqlConnection
            {
                ConnectionString = TeamConfigurationSettings.ConnectionStringOmd
            };

            var connVariable = new SqlConnection();


            try
            {
                connStg.Open();

                var testCaseQuery = new StringBuilder();

                testCaseQuery.AppendLine("--");
                testCaseQuery.AppendLine("-- Test data");
                testCaseQuery.AppendLine("-- Generated at " + DateTime.Now);
                testCaseQuery.AppendLine("--");

                const string queryTableArray = @"
                    SELECT [SOURCE_SCHEMA_NAME]
                          ,[SOURCE_NAME]
                          ,[SOURCE_BUSINESS_KEY_DEFINITION]
                          ,[TARGET_SCHEMA_NAME]
                          ,[TARGET_NAME]
                          ,[TARGET_BUSINESS_KEY_DEFINITION]
                          ,[TARGET_TYPE]
                          ,[SURROGATE_KEY]
                          ,[FILTER_CRITERIA]
                          ,[LOAD_VECTOR]
                      FROM [interface].[INTERFACE_SOURCE_STAGING_XREF]
                    ";

                var tables = MyParent.GetDataTable(ref connMetadata, queryTableArray);

                if (tables != null)
                {
                    textBoxOutput.Clear();
                    foreach (DataRow row in tables.Rows)
                    {
                        var stgTableName = (string) row["TARGET_NAME"];

                        if (checkBoxTruncate.Checked)
                        {
                            testCaseQuery.AppendLine();
                            testCaseQuery.AppendLine("-- Truncating the target table " + stgTableName);
                            testCaseQuery.AppendLine("TRUNCATE TABLE " + stgTableName);
                            testCaseQuery.AppendLine();
                        }

                        testCaseQuery.AppendLine();
                        testCaseQuery.AppendLine("-- Creating test cases for " + stgTableName);
                        testCaseQuery.AppendLine();

                        var localkeyLength = TeamConfigurationSettings.DwhKeyIdentifier.Length;
                        var localkeySubstring = TeamConfigurationSettings.DwhKeyIdentifier.Length + 1;

                        var queryAttributeArray = 
                            //SELECT [SOURCE_SCHEMA_NAME]
                            //      ,[SOURCE_NAME]
                            //      ,[TARGET_SCHEMA_NAME]
                            //      ,[TARGET_NAME]
                            //      ,[SOURCE_ATTRIBUTE_NAME]
                            //      ,[TARGET_ATTRIBUTE_NAME]
                            //  FROM [interface].[INTERFACE_SOURCE_STAGING_ATTRIBUTE_XREF]
                            //";
                        "SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION " +
                        "FROM INFORMATION_SCHEMA.COLUMNS " +
                        "WHERE SUBSTRING(COLUMN_NAME,LEN(COLUMN_NAME)-" + localkeyLength + "," + localkeySubstring + ")!='_" + TeamConfigurationSettings.DwhKeyIdentifier + "'" +
                        " AND TABLE_NAME= '" + stgTableName + "'" +
                        " AND TABLE_SCHEMA = '" + (string)row["TARGET_SCHEMA_NAME"] + "'" +
                        " AND COLUMN_NAME NOT IN ('" + TeamConfigurationSettings.RecordSourceAttribute + "','" +
                        TeamConfigurationSettings.AlternativeRecordSourceAttribute + "','" +
                        TeamConfigurationSettings.AlternativeLoadDateTimeAttribute + "','" +
                        TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute + "','" +
                        TeamConfigurationSettings.EtlProcessAttribute + "','" +
                        TeamConfigurationSettings.EventDateTimeAttribute + "','" +
                        TeamConfigurationSettings.ChangeDataCaptureAttribute + "','" +
                        TeamConfigurationSettings.RecordChecksumAttribute + "','" +
                        TeamConfigurationSettings.RowIdAttribute + "','" +
                        TeamConfigurationSettings.LoadDateTimeAttribute + "')";

                        var attributeArray = MyParent.GetDataTable(ref connStg, queryAttributeArray);

                        for (var intCounter = 1; intCounter <= Convert.ToInt32(textBoxTestCaseAmount.Text); intCounter++)
                        {
                            testCaseQuery.AppendLine("-- Test case " + intCounter);
                            testCaseQuery.AppendLine("INSERT INTO [dbo].[" + stgTableName + "]");
                            testCaseQuery.AppendLine("(");
                            testCaseQuery.AppendLine("[" + TeamConfigurationSettings.EtlProcessAttribute + "],");
                            testCaseQuery.AppendLine("[" + TeamConfigurationSettings.EventDateTimeAttribute + "],");
                            testCaseQuery.AppendLine("[" + TeamConfigurationSettings.RecordSourceAttribute + "],");
                            testCaseQuery.AppendLine("[" + TeamConfigurationSettings.ChangeDataCaptureAttribute + "],");
                            testCaseQuery.AppendLine("[" + TeamConfigurationSettings.RecordChecksumAttribute + "],");

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
                            testCaseQuery.AppendLine("'Test cases',");
                            testCaseQuery.AppendLine("'Insert',");

                            if (radioButtonBinaryHash.Checked)
                            {
                                testCaseQuery.AppendLine("0x00000000000000000000000000000000,");
                            }
                            else
                            {
                                testCaseQuery.AppendLine("'N/A',");
                            }

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
                                    textBoxOutput.Text+=("Issue encountered, the data type " +
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
                                connVariable.ConnectionString = TeamConfigurationSettings.ConnectionStringStg;
                            }

                            if (radioButtonPSA.Checked)
                            {
                                connVariable.ConnectionString = TeamConfigurationSettings.ConnectionStringHstg;
                            }

                            if (radiobuttonSource.Checked)
                            {
                                connVariable.ConnectionString = TeamConfigurationSettings.ConnectionStringSource;
                            }

                            int errorCounter = 0;
                            try
                            {
                                using (var connection = connVariable)
                                {
                                    var server = new Server(new ServerConnection(connection));
                                    try
                                    {
                                        server.ConnectionContext.ExecuteNonQuery(testCaseQuery.ToString());
                                        richTextBoxInformationMain.AppendText(
                                            "The statement was executed successfully.\r\n");
                                    }
                                    catch (Exception exception)
                                    {
                                        richTextBoxInformationMain.AppendText(
                                            "Issues occurred executing the SQL statement.\r\n");
                                        richTextBoxInformationMain.AppendText(
                                            @"SQL error: " + exception.Message + "\r\n\r\n");
                                        errorCounter++;
                                    }
                                }


                                richTextBoxInformationMain.Text =
                                    "The " + textBoxTestCaseAmount.Text +
                                    " test cases were created in the designated database.";
                            }
                            catch (Exception ex)
                            {
                                richTextBoxInformationMain.Text =
                                    "There have been " + errorCounter +
                                    " errors detected when executing the SQL statement against the database. The error message is " +
                                    ex;
                            }
                        }

                        textBoxOutput.Text += testCaseQuery.ToString();
                    }
                }
                else
                {
                    textBoxOutput.Text = "There is no metadata to process";
                }
            }
            catch (Exception exception)
            {
                textBoxOutput.Text = "There was an error connecting to the database. \r\n\r\nA connection could not be established. Can you verify the connection details for the Staging Area in the main screen? \r\n\r\nThe error message is: " + exception.Message;
            } 
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
