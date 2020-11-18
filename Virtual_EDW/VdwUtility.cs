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
            initialConfigurationFile.AppendLine("TeamConnectionsPath|" + FormBase.VdwConfigurationSettings.TeamConnectionsPath);
            initialConfigurationFile.AppendLine("TeamSelectedEnvironment|" + "F3958C0634E41306A16639B9445CD0B3");
            initialConfigurationFile.AppendLine("InputPath|" + FormBase.VdwConfigurationSettings.VdwExamplesPath);
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
                FormBase.VdwConfigurationSettings.VdwEventLog.Add(localEvent);
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

            string[] configurationArray = new[] { "TeamEnvironmentFilePath", "TeamConfigurationPath", "TeamConnectionsPath", "TeamSelectedEnvironment", "InputPath", "OutputPath", "LoadPatternPath", "VdwSchema" };

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
                        case "TeamConnectionsPath":
                            FormBase.VdwConfigurationSettings.TeamConnectionsPath = configList[configuration];
                            break;
                        case "TeamSelectedEnvironment":
                            FormBase.VdwConfigurationSettings.TeamSelectedEnvironmentInternalId = configList[configuration];
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
                            FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Error, $"Incorrect configuration '{configuration}' encountered."));
                            break;
                    }

                    FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"The entry {configuration} was loaded from the configuration file with value {configList[configuration]}."));

                }
                else
                {
                    FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Error, $"* The entry {configuration} was not found in the configuration file. Please make sure an entry exists ({configuration}|<value>)."));
                }
            }

            FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"The VDW configuration has been updated in memory" + $"."));

        }

        /// <summary>
        /// Load the connection information from file, based on the selected environment.
        /// </summary>
        /// <param name="environmentName"></param>
        public static void LoadTeamConnectionsFileForVdw(string environmentName)
        {
            if (environmentName != null)
            {
                // Connection information (TEAM_connections).
                var connectionFileName = 
                                         FormBase.VdwConfigurationSettings.TeamConnectionsPath +
                                         FormBase.GlobalParameters.JsonConnectionFileName + '_' + environmentName +
                                         FormBase.GlobalParameters.JsonExtension;

                FormBase.TeamConfigurationSettings.ConnectionDictionary = TeamConnectionFile.LoadConnectionFile(connectionFileName);

                if (FormBase.TeamConfigurationSettings.ConnectionDictionary is null)
                {
                    FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Warning, $"The connection dictionary is empty."));
                }
                else
                {
                    foreach (var localConnection in FormBase.TeamConfigurationSettings.ConnectionDictionary)
                    {
                        FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"The connection key {localConnection.Value.ConnectionKey} is available as part of the {FormBase.VdwConfigurationSettings.ActiveEnvironment.environmentKey} environment."));

                    }
                }
            }
            else
            {
                FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"The working environment has not been set, no connections have been loaded."));
            }
        }

        /// <summary>
        /// Load the configuration information from file, based on the selected environment.
        /// </summary>
        /// <param name="environmentName"></param>
        public static void LoadTeamConfigurationFileForVdw(string environmentName)
        {
            if (environmentName != null)
            {
                // Configuration information (TEAM_configuration.txt).
                var configurationFileName = 
                                            FormBase.VdwConfigurationSettings.TeamConfigurationPath +
                                            FormBase.GlobalParameters.TeamConfigurationFileName + '_' + environmentName +
                                            FormBase.GlobalParameters.ConfigurationFileExtension;

                try
                {
                    if (!File.Exists(configurationFileName))
                    {
                        FormBase.TeamConfigurationSettings.CreateDummyEnvironmentConfigurationFile(configurationFileName);
                        FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"A new configuration file {configurationFileName} was created."));
                    }
                    else
                    {
                        FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"The existing configuration file {configurationFileName} was detected."));
                    }
                }
                catch
                {
                    FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Error, $"An issue was encountered creating or detecting the configuration paths for {configurationFileName}."));
                }

                FormBase.TeamConfigurationSettings.LoadTeamConfigurationFile(configurationFileName);
                
                // Report back any events that may have happened while loading the TEAM information, by adding events to the global VDW event log.
                foreach (var localEvent in FormBase.TeamConfigurationSettings.ConfigurationSettingsEventLog)
                {
                    FormBase.VdwConfigurationSettings.VdwEventLog.Add(localEvent);
                }
            }
            else
            {
                FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"The working environment has not been set, no configurations have been loaded."));
            }
        }

        /// <summary>
        /// Create the designated VDW schema, if it doesn't exist already.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static void CreateVdwSchema(SqlConnection connection)
        {
            try
            {
                connection.Open();

                // Execute the check to see if the schema exists or not
                var checkCommand =
                    new SqlCommand(
                        $"SELECT CASE WHEN EXISTS (SELECT * FROM sys.schemas WHERE name = '{FormBase.VdwConfigurationSettings.VdwSchema}') THEN 1 ELSE 0 END",
                        connection);
                var exists = (int) checkCommand.ExecuteScalar() == 1;

                if (exists == false)
                {
                    var createStatement = new StringBuilder();

                    //createStatement.AppendLine("-- Creating the schema");
                    //createStatement.AppendLine("IF NOT EXISTS (");
                    //createStatement.AppendLine("SELECT SCHEMA_NAME");
                    //createStatement.AppendLine("FROM INFORMATION_SCHEMA.SCHEMATA");
                    //createStatement.AppendLine("WHERE SCHEMA_NAME = '" + FormBase.VdwConfigurationSettings.VdwSchema + "')");
                    //createStatement.AppendLine("");
                    //createStatement.AppendLine("BEGIN");
                    //createStatement.AppendLine(" EXEC sp_executesql N'CREATE SCHEMA " + FormBase.VdwConfigurationSettings.VdwSchema + "'");
                    //createStatement.AppendLine("END");

                    createStatement.AppendLine("IF SCHEMA_ID('" + FormBase.VdwConfigurationSettings.VdwSchema + "') IS NULL EXEC('CREATE SCHEMA " + FormBase.VdwConfigurationSettings.VdwSchema + "')");
                    

                    var commandVersion = new SqlCommand(createStatement.ToString(), connection);

                    commandVersion.ExecuteNonQuery();

                    FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information,
                        $"The VDW schema '{FormBase.VdwConfigurationSettings.VdwSchema}' was created for database '{connection.Database}'."));

                }
            }
            catch (Exception ex)
            {
                FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Error,
                    $"An issue occured creating the VDW schema '{FormBase.VdwConfigurationSettings.VdwSchema}'. The reported error is {ex}"));
            }
        }

        public static void ExecuteOutputInDatabase(SqlConnection sqlConnection, string query)
        {
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

                            FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"The SQL statement was executed successfully."));

                        }
                        catch (Exception ex)
                        {
                            FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Error, "Issues occurred executing the SQL statement. SQL error: " + ex.Message + ""));
                        }
                    }
                }
                catch (Exception ex)
                {
                    FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Error, "There was an issue executing the code against the database. The message is: " + ex.Message + ""));
                }
            }
        }

        public static void SaveOutputToDisk(string targetFile, string textContent)
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
                FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Error, "There was an issue saving the output to disk. The message is: " + ex.Message + "\r\n\r\n"));
            }

  
        }
    }
}
