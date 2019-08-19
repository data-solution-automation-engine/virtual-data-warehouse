using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using static Virtual_Data_Warehouse.FormBase;

namespace Virtual_Data_Warehouse
{
    public class EventLog : List<Event> { }

    public class Event
    {
        public int eventCode { get; set; }
        public string eventDescription { get; set; }
    }

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

                            var localEvent = new Event
                            {
                                eventCode = 0,
                                eventDescription = "The SQL statement was executed successfully.\r\n"
                            };

                            eventLog.Add(localEvent);

                        }
                        catch (Exception ex)
                        {
                            var localEvent = new Event
                            {
                                eventCode = 1,
                                eventDescription = "Issues occurred executing the SQL statement.\r\n. SQL error: " +
                                                   ex.Message + "\r\n\r\n"
                            };

                            eventLog.Add(localEvent);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var localEvent = new Event
                    {
                        eventCode = 1,
                        eventDescription =
                            @"There was an issue executing the code against the database. The message is: " + ex
                    };

                    eventLog.Add(localEvent);

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
                var localEvent = new Event
                {
                    eventCode = 1,
                    eventDescription =
                        @"There was an issue saving the output to disk. The message is: " + ex
                };

                eventLog.Add(localEvent);
            }

            return eventLog;
        }
    }
}
