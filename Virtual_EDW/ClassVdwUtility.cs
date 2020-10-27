using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using TEAM;

namespace Virtual_Data_Warehouse
{
    public class VdwUtility
    {

       /// <summary>
        /// Check if a Team Graph Configuration File exists, and create a default configuration file if not.
        /// </summary>
        internal static void CreateNewVdwConfigurationFile()
        {
            Event localEvent;

            var initialConfigurationFile = new StringBuilder();
            initialConfigurationFile.AppendLine("/* Virtual Data Warehouse (VDW) Core Settings */");
            initialConfigurationFile.AppendLine("TeamEnvironmentFilePath|" + FormBase.VdwConfigurationSettings.TeamEnvironmentFilePath);
            initialConfigurationFile.AppendLine("TeamConfigurationPath|" + FormBase.VdwConfigurationSettings.TeamConfigurationPath);
            initialConfigurationFile.AppendLine("TeamSelectedEnvironment|" + "");
            initialConfigurationFile.AppendLine("InputPath|" + FormBase.VdwConfigurationSettings.VdwInputPath);
            initialConfigurationFile.AppendLine("OutputPath|" + FormBase.VdwConfigurationSettings.VdwOutputPath);
            initialConfigurationFile.AppendLine("LoadPatternPath|" + FormBase.VdwConfigurationSettings.LoadPatternPath);
            initialConfigurationFile.AppendLine("VdwSchema|vdw");
            initialConfigurationFile.AppendLine("/* End of file */");

            var vdwConfigurationFileName =
                FormBase.GlobalParameters.VdwConfigurationPath + 
                FormBase.GlobalParameters.VdwConfigurationFileName;

            localEvent = FileHandling.CreateConfigurationFile(vdwConfigurationFileName, initialConfigurationFile);
            if (localEvent.eventDescription != null)
            {
                FormBase.GlobalParameters.TeamEventLog.Add(localEvent);
            }
        }

        /// <summary>
        /// Load the information from the VDW configuration file into memory and display on the form
        /// </summary>
        internal static void LoadVdwConfigurationFile()
        {
            var configList = FileHandling.LoadConfigurationFile(
                FormBase.GlobalParameters.VdwConfigurationPath +
                FormBase.GlobalParameters.VdwConfigurationFileName);

            string[] configurationArray = new[] { "TeamEnvironmentFilePath", "TeamConfigurationPath", "TeamSelectedEnvironment", "InputPath", "OutputPath", "LoadPatternPath", "VdwSchema" };

            foreach (string configuration in configurationArray)
            {
                if (configList.ContainsKey(configuration))
                {
                    switch (configuration)
                    {
                        case "TeamEnvironmentFilePath":
                            FormBase.VdwConfigurationSettings.TeamEnvironmentFilePath = configList[configuration];
                            break;
                        case "TeamConfigurationPath":
                            FormBase.VdwConfigurationSettings.TeamConfigurationPath = configList[configuration];
                            break;
                        case "TeamSelectedEnvironment":
                            FormBase.VdwConfigurationSettings.TeamSelectedEnvironment = configList[configuration];
                            break;
                        case "InputPath":
                            FormBase.VdwConfigurationSettings.VdwInputPath = configList[configuration];
                            break;
                        case "OutputPath":
                            FormBase.VdwConfigurationSettings.VdwOutputPath = configList[configuration];
                            break;
                        case "LoadPatternPath":
                            FormBase.VdwConfigurationSettings.LoadPatternPath = configList[configuration];
                            break;
                        case "VdwSchema":
                            FormBase.VdwConfigurationSettings.VdwSchema = configList[configuration];
                            break;
                        default:
                            FormBase.GlobalParameters.TeamEventLog.Add(Event.CreateNewEvent(EventTypes.Error, $"Incorrect configuration '{configuration}' encountered."));
                            break;
                    }

                    FormBase.GlobalParameters.TeamEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"The entry {configuration} was loaded from the configuration file with value {configList[configuration]}."));

                }
                else
                {
                    FormBase.GlobalParameters.TeamEventLog.Add(Event.CreateNewEvent(EventTypes.Error, $"* The entry {configuration} was not found in the configuration file. Please make sure an entry exists ({configuration}|<value>)."));
                }
            }

            FormBase.GlobalParameters.TeamEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"The VDW configuration has been updated in memory" + $"."));

        }

        /// <summary>
        /// Load the configuration and connection information from file, based on the selected environment and input path.
        /// </summary>
        /// <param name="environmentName"></param>
        public static void LoadTeamConfigurations(string environmentName)
        {
            if (environmentName != null)
            {
                var connectionFileName = FormBase.VdwConfigurationSettings.TeamConfigurationPath +
                                         FormBase.GlobalParameters.JsonConnectionFileName + '_' + environmentName +
                                         FormBase.GlobalParameters.JsonExtension;

                FormBase.TeamConfigurationSettings.ConnectionDictionary = TeamConnectionFile.LoadConnectionFile(connectionFileName);

                if (FormBase.TeamConfigurationSettings.ConnectionDictionary is null)
                {
                    FormBase.GlobalParameters.TeamEventLog.Add(Event.CreateNewEvent(EventTypes.Warning, $"The connection dictionary is empty."));
                }

                var configurationFileName = FormBase.VdwConfigurationSettings.TeamConfigurationPath +
                                            FormBase.GlobalParameters.TeamConfigurationFileName + '_' +
                                            environmentName +
                                            FormBase.GlobalParameters.ConfigurationFileExtension;

                FormBase.TeamConfigurationSettings.LoadTeamConfigurationFile(configurationFileName);
            }
            else
            {
                FormBase.GlobalParameters.TeamEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"The working environment has not been set, no configurations have been loaded."));
            }
        }


        public static string CreateSchema(string connString)
        {
            string returnMessage ="";

            var createStatement = new StringBuilder();

            createStatement.AppendLine("-- Creating the schema");
            createStatement.AppendLine("IF NOT EXISTS (");
            createStatement.AppendLine("SELECT SCHEMA_NAME");
            createStatement.AppendLine("FROM INFORMATION_SCHEMA.SCHEMATA");
            createStatement.AppendLine("WHERE SCHEMA_NAME = '" + FormBase.VdwConfigurationSettings.VdwSchema + "')");
            createStatement.AppendLine("");
            createStatement.AppendLine("BEGIN");
            createStatement.AppendLine(" EXEC sp_executesql N'CREATE SCHEMA [" + FormBase.VdwConfigurationSettings.VdwSchema + "]'");
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
                    returnMessage = "An issue occured creating the VDW schema '" + FormBase.VdwConfigurationSettings.VdwSchema + "'. The reported error is " + ex;
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

        //internal static Dictionary<String, String> MatchConnectionKey(string connectionKey)
        //{
        //    Dictionary<string, string> returnValue = new Dictionary<string, string>();

        //    if (connectionKey == "SourceDatabase")
        //    {
        //        returnValue.Add(connectionKey, FormBase.TeamConfigurationSettings.ConnectionStringSource);
        //    }
        //    else if (connectionKey == "StagingDatabase")
        //    {
        //        returnValue.Add(connectionKey, FormBase.TeamConfigurationSettings.ConnectionStringStg);
        //    }
        //    else if (connectionKey == "PersistentStagingDatabase")
        //    {
        //        returnValue.Add(connectionKey, FormBase.TeamConfigurationSettings.ConnectionStringHstg);
        //    }
        //    else if (connectionKey == "IntegrationDatabase")
        //    {
        //        returnValue.Add(connectionKey, FormBase.TeamConfigurationSettings.ConnectionStringInt);
        //    }
        //    else if (connectionKey == "PresentationDatabase")
        //    {
        //        returnValue.Add(connectionKey, FormBase.TeamConfigurationSettings.ConnectionStringPres);
        //    }

        //    return returnValue;
        //}
    }
}
