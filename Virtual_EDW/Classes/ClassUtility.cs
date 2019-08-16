using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using static Virtual_Data_Warehouse.FormBase;

namespace Virtual_Data_Warehouse
{
    public class Utility
    {
        public event EventHandler<MyEventArgs> OnChangeMainText = delegate { };

        public void RaiseOnChangeMainText(string inputText)
        {
            OnChangeMainText(this, new MyEventArgs(inputText));
        }
        /// <summary>
        /// Load a data set into an in-memory datatable
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(ref SqlConnection sqlConnection, string sql)
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

        public static string CreateSchema(string connString)
        {
            string returnMessage ="";

            var createStatement = new StringBuilder();

            createStatement.AppendLine("-- Creating the schema");
            createStatement.AppendLine("IF NOT EXISTS (");
            createStatement.AppendLine("SELECT SCHEMA_NAME");
            createStatement.AppendLine("FROM INFORMATION_SCHEMA.SCHEMATA");
            createStatement.AppendLine("WHERE SCHEMA_NAME = '" + FormBase.VedwConfigurationSettings.VedwSchema + "')");
            createStatement.AppendLine("");
            createStatement.AppendLine("BEGIN");
            createStatement.AppendLine(" EXEC sp_executesql N'CREATE SCHEMA [" + FormBase.VedwConfigurationSettings.VedwSchema + "]'");
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
                    returnMessage = "An issue occured creating the VEDW schema '" + FormBase.VedwConfigurationSettings.VedwSchema + "'. The reported error is " + ex;
                }
            }

            return returnMessage;
        }

        public static int ExecuteOutputInDatabase(bool generateFlag, SqlConnection sqlConnection, string result, int errorCounter)
        {
            if (generateFlag == true)
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
                         //   SetTextMain("The statement was executed successfully.\r\n");
                        }
                        catch (Exception ex)
                        {
                          //  SetTextMain("Issues occurred executing the SQL statement.\r\n");
                         //   SetTextMain(@"SQL error: " + exception.Message + "\r\n\r\n");
                            errorCounter++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorCounter++;
                    //SetTextMain("There was an issue executing the code against the database. The message is: " +                        ex);
                }
            }

            return errorCounter;
        }

        public static int SaveOutputToDisk(bool saveFlag, string targetFile, string textContent, int errorCounter)
        {
            if (saveFlag==true)
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
                    //SetTextMain("There was an issue in saving the output to disk. The message is: " + ex);
                }
            }

            return errorCounter;
        }
    }
}
