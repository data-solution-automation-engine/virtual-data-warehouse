using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Virtual_Data_Warehouse_Library;

namespace Virtual_Data_Warehouse
{
    //public class EventLog : List<Event> { }

    //public class Event
    //{
    //    public int eventCode { get; set; }
    //    public string eventDescription { get; set; }
    //}

    public class Utility
    {
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

        public static EventLog ExecuteOutputInDatabase(SqlConnection sqlConnection, string query)
        {
            EventLog eventLog = new EventLog();

            {
                try
                {
                    //sqlConnection.ConnectionString = TeamConfigurationSettings.ConnectionStringHstg;
                    using (var connection = sqlConnection)
                    {
                        var server = new Server(new ServerConnection(connection));
                        try
                        {
                            server.ConnectionContext.ExecuteNonQuery(query);

                            eventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"The SQL statement was executed successfully.\r\n"));

                        }
                        catch (Exception ex)
                        {
                            eventLog.Add(Event.CreateNewEvent(EventTypes.Error, "Issues occurred executing the SQL statement.\r\n. SQL error: " + ex.Message + "\r\n\r\n"));
                        }
                    }
                }
                catch (Exception ex)
                {
                    eventLog.Add(Event.CreateNewEvent(EventTypes.Error, "There was an issue executing the code against the database. The message is: " + ex.Message + "\r\n\r\n"));
                }

                return eventLog;
            }
        }

        public static EventLog SaveOutputToDisk(string targetFile, string textContent)
        {
            EventLog eventLog = new EventLog();

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
                eventLog.Add(Event.CreateNewEvent(EventTypes.Error, "There was an issue saving the output to disk. The message is: " + ex.Message + "\r\n\r\n"));
            }

            return eventLog;
        }

        internal static Dictionary<String, String> MatchConnectionKey(string connectionKey)
        {
            Dictionary<string, string> returnValue = new Dictionary<string, string>();

            if (connectionKey == "SourceDatabase")
            {
                returnValue.Add(connectionKey, FormBase.TeamConfigurationSettings.ConnectionStringSource);
            }
            else if (connectionKey == "StagingDatabase")
            {
                returnValue.Add(connectionKey, FormBase.TeamConfigurationSettings.ConnectionStringStg);
            }
            else if (connectionKey == "PersistentStagingDatabase")
            {
                returnValue.Add(connectionKey, FormBase.TeamConfigurationSettings.ConnectionStringHstg);
            }
            else if (connectionKey == "IntegrationDatabase")
            {
                returnValue.Add(connectionKey, FormBase.TeamConfigurationSettings.ConnectionStringInt);
            }
            else if (connectionKey == "PresentationDatabase")
            {
                returnValue.Add(connectionKey, FormBase.TeamConfigurationSettings.ConnectionStringPres);
            }

            return returnValue;
        }
    }
}
