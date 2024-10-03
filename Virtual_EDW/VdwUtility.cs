using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.Win32;
using TEAM_Library;
using System.Data;
using Snowflake.Data.Client;


namespace Virtual_Data_Warehouse
{
    public static class VdwUtility
    {
        /// <summary>
        /// Check if a Team Graph Configuration File exists, and create a default configuration file if not.
        /// </summary>
        internal static void CreateNewVdwConfigurationFile()
        {
            var initialConfigurationFile = new StringBuilder();
            initialConfigurationFile.AppendLine("/* Virtual Data Warehouse (VDW) Core Settings */");
            initialConfigurationFile.AppendLine("TeamEnvironmentFilePath|" + FormBase.VdwConfigurationSettings.TeamEnvironmentFilePath);
            //initialConfigurationFile.AppendLine("TeamConfigurationPath|" + FormBase.VdwConfigurationSettings.TeamConfigurationPath);
            //nitialConfigurationFile.AppendLine("TeamConnectionsPath|" + FormBase.VdwConfigurationSettings.TeamConnectionsPath);
            initialConfigurationFile.AppendLine("TeamSelectedEnvironment|" + "F3958C0634E41306A16639B9445CD0B3");
            //initialConfigurationFile.AppendLine("InputPath|" + FormBase.VdwConfigurationSettings.VdwExamplesPath);
            //initialConfigurationFile.AppendLine("OutputPath|" + FormBase.VdwConfigurationSettings.VdwOutputPath);
            //initialConfigurationFile.AppendLine("TemplatePath|" + FormBase.VdwConfigurationSettings.TemplatePath);
            initialConfigurationFile.AppendLine("VdwSchema|vdw");
            initialConfigurationFile.AppendLine("/* End of file */");

            var vdwConfigurationFileName = FormBase.GlobalParameters.CorePath + FormBase.GlobalParameters.VdwConfigurationFileName;

            var localEvent = FileHandling.CreateConfigurationFile(vdwConfigurationFileName, initialConfigurationFile);

            if (localEvent.eventDescription != null)
            {
                FormBase.VdwConfigurationSettings.VdwEventLog.Add(localEvent);
            }
        }

        /// <summary>
        /// Load the information from the VDW configuration file into memory and display on the form.
        /// </summary>
        internal static void LoadVdwConfigurationFile()
        {
            try
            {
                var configList = FileHandling.LoadConfigurationFile(FormBase.GlobalParameters.CorePath + FormBase.GlobalParameters.VdwConfigurationFileName);

                string[] configurationArray =
                {
                    "TeamEnvironmentFilePath",
                    //"TeamConfigurationPath",
                    //"TeamConnectionsPath",
                    "TeamSelectedEnvironment",
                    //"InputPath",
                    //"OutputPath",
                    //"TemplatePath",
                    "VdwSchema"
                };

                foreach (string configuration in configurationArray)
                {
                    if (configList.ContainsKey(configuration))
                    {
                        switch (configuration)
                        {
                            case "TeamEnvironmentFilePath":
                                FormBase.VdwConfigurationSettings.TeamEnvironmentFilePath = configList[configuration];
                                break;
                            //case "TeamConfigurationPath":
                            //    FormBase.VdwConfigurationSettings.TeamConfigurationPath = configList[configuration];
                            //    break;
                            //case "TeamConnectionsPath":
                            //    FormBase.VdwConfigurationSettings.TeamConnectionsPath = configList[configuration];
                            //    break;
                            case "TeamSelectedEnvironment":
                                FormBase.VdwConfigurationSettings.TeamSelectedEnvironmentInternalId =
                                    configList[configuration];
                                break;
                            //case "InputPath":
                            //    FormBase.VdwConfigurationSettings.VdwMetadatPath = configList[configuration];
                            //    break;
                            //case "OutputPath":
                            //    FormBase.VdwConfigurationSettings.VdwOutputPath = configList[configuration];
                            //    break;
                            //case "TemplatePath":
                            //    FormBase.VdwConfigurationSettings.TemplatePath = configList[configuration];
                            //    break;
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
            catch (Exception exception)
            {
                FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Error, $"An error was encountered loading the VDW configuration file. The reported error is: \r\n\r\n{exception.Message}."));
            }
        }

        /// <summary>
        /// Load the connection information from file, based on the selected environment.
        /// </summary>
        /// <param name="environmentName"></param>
        public static void LoadTeamConnectionsFileForVdw(string environmentName, EventLog eventLog)
        {
            if (environmentName != null)
            {
                // Connection information (TEAM_connections).
                var connectionFileName = FormBase.VdwConfigurationSettings.TeamConnectionsPath + FormBase.GlobalParameters.JsonConnectionFileName + '_' + environmentName + FormBase.GlobalParameters.JsonExtension;

                FormBase.TeamConfigurationSettings.ConnectionDictionary = TeamConnectionFile.LoadConnectionFile(connectionFileName, eventLog);

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
                var configurationFileName = FormBase.VdwConfigurationSettings.TeamConfigurationPath + FormBase.GlobalParameters.TeamConfigurationFileName + '_' + environmentName + FormBase.GlobalParameters.ConfigurationFileExtension;

                try
                {
                    if (!File.Exists(configurationFileName))
                    {
                        FormBase.TeamConfigurationSettings.CreateDummyTeamConfigurationFile(configurationFileName);
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
        public static void CreateVdwSchema(Microsoft.Data.SqlClient.SqlConnection connection)
        {
            try
            {
                connection.Open();

                // Execute the check to see if the schema exists or not
                var checkCommand = new Microsoft.Data.SqlClient.SqlCommand($"SELECT CASE WHEN EXISTS (SELECT * FROM sys.schemas WHERE name = '{FormBase.VdwConfigurationSettings.VdwSchema}') THEN 1 ELSE 0 END", connection);
                var exists = (int)checkCommand.ExecuteScalar() == 1;

                if (exists == false)
                {
                    var createStatement = new StringBuilder();

                    createStatement.AppendLine("IF SCHEMA_ID('" + FormBase.VdwConfigurationSettings.VdwSchema + "') IS NULL EXEC('CREATE SCHEMA " + FormBase.VdwConfigurationSettings.VdwSchema + "')");

                    var commandVersion = new Microsoft.Data.SqlClient.SqlCommand(createStatement.ToString(), connection);

                    commandVersion.ExecuteNonQuery();

                    FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"The VDW schema '{FormBase.VdwConfigurationSettings.VdwSchema}' was created for database '{connection.Database}'."));
                }
            }
            catch (Exception exception)
            {
                var errorMessage = $"An issue occurred creating the VDW schema '{FormBase.VdwConfigurationSettings.VdwSchema}' in the '{connection.Database}' database. The reported error is {exception.Message} - {exception.InnerException}";
                FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Error, errorMessage));
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public static void ExecuteInDatabase(TeamConnection teamConnection, string query)
        {
            // SQL Server
            if (teamConnection.TechnologyConnectionType == TechnologyConnectionType.SqlServer)
            {
                var connectionString = teamConnection.CreateSqlServerConnectionString(false);

                Microsoft.Data.SqlClient.SqlConnection sqlConnection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);

                try
                {
                    sqlConnection.Open();

                    var server = new Server(new ServerConnection(sqlConnection));
                    server.ConnectionContext.ExecuteNonQuery(query);

                    FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"The SQL statement was executed successfully."));
                }

                catch (Exception exception)
                {
                    FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Error, $"There was an issue executing the code against the database. The reported error is {exception.Message}, {exception.InnerException.Message}"));
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            // Snowflake
            else if (teamConnection.TechnologyConnectionType == TechnologyConnectionType.Snowflake)
            {
                IDbConnection conn = new SnowflakeDbConnection();
                conn.ConnectionString = teamConnection.CreateSnowflakeSSOConnectionString(false);

                try
                {
                    conn.Open();
                    IDbCommand cmd = conn.CreateCommand();
                    // Select the Snowflake warehouse.
                    cmd.CommandText = $"USE WAREHOUSE {teamConnection.DatabaseServer.Warehouse}";
                    cmd.ExecuteNonQuery();
                    // Support multiple statements for this session.
                    cmd.CommandText = "ALTER SESSION SET MULTI_STATEMENT_COUNT = 0;";
                    cmd.ExecuteNonQuery();
                    // Run the query generated from the template.
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception exception)
                {
                    FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Error, $"There was an issue executing the code against the database. The reported error is {exception.Message}."));
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            else
            {
                var errorMessage = $"VDW was not able to assert the connection type.";
                FormBase.VdwConfigurationSettings.VdwEventLog.Add(Event.CreateNewEvent(EventTypes.Error, errorMessage));
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

        public static void DoubleBuffered(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }

        private static string CleanBrowserPath(string p)
        {
            string[] url = p.Split('"');
            string clean = url[1];
            return clean;
        }

        public static string GetDefaultBrowserPath()
        {
            string urlAssociation = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http";
            string browserPathKey = @"$BROWSER$\shell\open\command";

            Microsoft.Win32.RegistryKey userChoiceKey = null;
            string browserPath = "";

            try
            {
                //Read default browser path from userChoiceLKey
                userChoiceKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(urlAssociation + @"\UserChoice", false);

                //If user choice was not found, try machine default
                if (userChoiceKey == null)
                {
                    //Read default browser path from Win XP registry key
                    var browserKey = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

                    //If browser path wasn’t found, try Win Vista (and newer) registry key
                    if (browserKey == null)
                    {
                        browserKey = Registry.CurrentUser.OpenSubKey(urlAssociation, false);
                    }
                    var path = CleanBrowserPath(browserKey.GetValue(null) as string);
                    browserKey.Close();
                    return path;
                }
                else
                {
                    // user defined browser choice was found
                    string progId = (userChoiceKey.GetValue("ProgId").ToString());
                    userChoiceKey.Close();

                    // now look up the path of the executable
                    string concreteBrowserKey = browserPathKey.Replace("$BROWSER$", progId);
                    var kp = Registry.ClassesRoot.OpenSubKey(concreteBrowserKey, false);
                    browserPath = CleanBrowserPath(kp.GetValue(null) as string);
                    kp.Close();
                    return browserPath;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
