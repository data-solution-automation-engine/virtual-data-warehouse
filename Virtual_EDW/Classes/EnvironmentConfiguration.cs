using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Virtual_EDW.Classes
{
    class EnvironmentConfiguration
    {
        /// <summary>
        ///    Method to create a new configuration file with default values at the default location
        /// </summary>
        internal void CreateDummyEnvironmentConfiguration(string filename)
        {
            var initialConfigurationFile = new StringBuilder();

            initialConfigurationFile.AppendLine("/* TEAM Configuration Settings */");
            initialConfigurationFile.AppendLine("/* Roelant Vos - 2018 */");
            initialConfigurationFile.AppendLine("SourceDatabase|Source_Database");
            initialConfigurationFile.AppendLine("StagingDatabase|Staging_Area_Database");
            initialConfigurationFile.AppendLine("PersistentStagingDatabase|Persistent_Staging_Area_Database");
            initialConfigurationFile.AppendLine("IntegrationDatabase|Data_Vault_Database");
            initialConfigurationFile.AppendLine("PresentationDatabase|Presentation_Database");
            initialConfigurationFile.AppendLine("OutputPath|" + FormBase.GlobalParameters.OutputPath);
            initialConfigurationFile.AppendLine("ConfigurationPath|" + FormBase.GlobalParameters.ConfigurationPath);
            initialConfigurationFile.AppendLine(
                @"connectionStringSource|Provider=SQLNCLI11;Server=<>;Initial Catalog=<Source_Database>;user id=sa; password=<>");
            initialConfigurationFile.AppendLine(
                @"connectionStringStaging|Provider=SQLNCLI11;Server=<>;Initial Catalog=<Staging_Area>;user id=sa; password=<>");
            initialConfigurationFile.AppendLine(
                @"connectionStringPersistentStaging|Provider=SQLNCLI11;Server=<>;Initial Catalog=<Persistent_Staging_Area>;user id=sa; password=<>");
            initialConfigurationFile.AppendLine(
                @"connectionStringMetadata|Provider=SQLNCLI11;Server=<>;Initial Catalog=<Metadata>;user id=sa; password=<>");
            initialConfigurationFile.AppendLine(
                @"connectionStringIntegration|Provider=SQLNCLI11;Server=<>;Initial Catalog=<Data_Vault>;user id=sa; password=<>");
            initialConfigurationFile.AppendLine(
                @"connectionStringPresentation|Provider=SQLNCLI11;Server=<>;Initial Catalog=<Presentation>;user id=sa; password=<>");
            initialConfigurationFile.AppendLine("SourceSystemPrefix|PROFILER");
            initialConfigurationFile.AppendLine("StagingAreaPrefix|STG");
            initialConfigurationFile.AppendLine("PersistentStagingAreaPrefix|PSA");
            initialConfigurationFile.AppendLine("HubTablePrefix|HUB");
            initialConfigurationFile.AppendLine("SatTablePrefix|SAT");
            initialConfigurationFile.AppendLine("LinkTablePrefix|LNK");
            initialConfigurationFile.AppendLine("LinkSatTablePrefix|LSAT");
            initialConfigurationFile.AppendLine("KeyIdentifier|HSH");
            initialConfigurationFile.AppendLine("SchemaName|dbo");
            initialConfigurationFile.AppendLine("RowID|SOURCE_ROW_ID");
            initialConfigurationFile.AppendLine("EventDateTimeStamp|EVENT_DATETIME");
            initialConfigurationFile.AppendLine("LoadDateTimeStamp|LOAD_DATETIME");
            initialConfigurationFile.AppendLine("ExpiryDateTimeStamp|LOAD_END_DATETIME");
            initialConfigurationFile.AppendLine("ChangeDataIndicator|CDC_OPERATION");
            initialConfigurationFile.AppendLine("RecordSourceAttribute|RECORD_SOURCE");
            initialConfigurationFile.AppendLine("ETLProcessID|ETL_INSERT_RUN_ID");
            initialConfigurationFile.AppendLine("ETLUpdateProcessID|ETL_UPDATE_RUN_ID");
            initialConfigurationFile.AppendLine("LogicalDeleteAttribute|DELETED_RECORD_INDICATOR");
            initialConfigurationFile.AppendLine("LinkedServerName|");
            initialConfigurationFile.AppendLine("TableNamingLocation|Prefix");
            initialConfigurationFile.AppendLine("KeyNamingLocation|Suffix");
            initialConfigurationFile.AppendLine("RecordChecksum|HASH_FULL_RECORD");
            initialConfigurationFile.AppendLine("CurrentRecordAttribute|CURRENT_RECORD_INDICATOR");
            initialConfigurationFile.AppendLine("AlternativeRecordSource|N/A");
            initialConfigurationFile.AppendLine("AlternativeHubLDTS|N/A");
            initialConfigurationFile.AppendLine("AlternativeSatelliteLDTS|N/A");
            initialConfigurationFile.AppendLine("AlternativeRecordSourceFunction|False");
            initialConfigurationFile.AppendLine("AlternativeHubLDTSFunction|False");
            initialConfigurationFile.AppendLine("AlternativeSatelliteLDTSFunction|False");
            initialConfigurationFile.AppendLine("PSAKeyLocation|PrimaryKey"); //Can be PrimaryKey or UniqueIndex
            initialConfigurationFile.AppendLine("metadataRepositoryType|JSON");

            initialConfigurationFile.AppendLine("/* End of file */");

            using (var outfile = new StreamWriter(filename))
            {
                outfile.Write(initialConfigurationFile.ToString());
                outfile.Close();
            }
        }


        /// <summary>
        ///    Check if the paths exists and create them if necessary
        /// </summary>
        internal static void InitialiseRootPath()
        {
            // Create the configuration directory if it does not exist yet
            try
            {
                if (!Directory.Exists(FormBase.GlobalParameters.ConfigurationPath))
                {
                    Directory.CreateDirectory(FormBase.GlobalParameters.ConfigurationPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error creation default directory at " + FormBase.GlobalParameters.ConfigurationPath +
                    " the message is " + ex, "An issue has been encountered", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            // Create the output directory if it does not exist yet
            try
            {
                if (!Directory.Exists(FormBase.GlobalParameters.OutputPath))
                {
                    Directory.CreateDirectory(FormBase.GlobalParameters.OutputPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error creation default directory at " + FormBase.GlobalParameters.OutputPath + " the message is " +
                    ex, "An issue has been encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Create root path file, with dummy values if it doesn't exist already
            try
            {
                if (!File.Exists(FormBase.GlobalParameters.ConfigurationPath + FormBase.GlobalParameters.PathfileName +
                                 FormBase.GlobalParameters.FileExtension))
                {
                    var initialConfigurationFile = new StringBuilder();

                    initialConfigurationFile.AppendLine("/* TEAM File Path Settings */");
                    initialConfigurationFile.AppendLine("/* Roelant Vos - 2018 */");
                    initialConfigurationFile.AppendLine("ConfigurationPath|" +
                                                        FormBase.GlobalParameters.ConfigurationPath);
                    initialConfigurationFile.AppendLine("OutputPath|" + FormBase.GlobalParameters.OutputPath);
                    initialConfigurationFile.AppendLine("WorkingEnvironment|Development");
                    initialConfigurationFile.AppendLine("/* End of file */");

                    using (var outfile = new StreamWriter(FormBase.GlobalParameters.ConfigurationPath +
                                                          FormBase.GlobalParameters.PathfileName +
                                                          FormBase.GlobalParameters.FileExtension))
                    {
                        outfile.Write(initialConfigurationFile.ToString());
                        outfile.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred while creation the default path file. The error message is " + ex,
                    "An issue has been encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Create root path file, with dummy values, if it doesn't exist already
            try
            {
                if (!File.Exists(FormBase.GlobalParameters.ConfigurationPath + FormBase.GlobalParameters.PathfileName))
                {
                    var initialConfigurationFile = new StringBuilder();

                    initialConfigurationFile.AppendLine("/* TEAM File Path Settings */");
                    initialConfigurationFile.AppendLine("/* Roelant Vos - 2018 */");
                    initialConfigurationFile.AppendLine("ConfigurationPath|" +
                                                        FormBase.GlobalParameters.ConfigurationPath);
                    initialConfigurationFile.AppendLine("OutputPath|" + FormBase.GlobalParameters.OutputPath);
                    initialConfigurationFile.AppendLine("EnableUnicode|True");
                    initialConfigurationFile.AppendLine("DisableHash|False");
                    initialConfigurationFile.AppendLine("HashKeyOutputType|Binary");
                    initialConfigurationFile.AppendLine("/* End of file */");

                    using (var outfile =
                        new StreamWriter(FormBase.GlobalParameters.ConfigurationPath +
                                         FormBase.GlobalParameters.PathfileName))
                    {
                        outfile.Write(initialConfigurationFile.ToString());
                        outfile.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred while creation the default path file. The error message is " + ex,
                    "An issue has been encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        /// <summary>
        ///    Check if the paths exists and create them if necessary
        /// </summary>
        internal static void InitialiseConfigurationPath()
        {
            // Create the configuration directory if it does not exist yet
            try
            {
                if (!Directory.Exists(FormBase.ConfigurationSettings.ConfigurationPath))
                {
                    Directory.CreateDirectory(FormBase.ConfigurationSettings.ConfigurationPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error creation default directory at " + FormBase.ConfigurationSettings.ConfigurationPath +
                    " the message is " + ex, "An issue has been encountered", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            // Create the output directory if it does not exist yet
            try
            {
                if (!Directory.Exists(FormBase.ConfigurationSettings.OutputPath))
                {
                    Directory.CreateDirectory(FormBase.ConfigurationSettings.OutputPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error creation default directory at " + FormBase.ConfigurationSettings.OutputPath +
                    " the message is " + ex, "An issue has been encountered", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            // Create a new dummy configuration file
            try
            {
                // Create a default configuration file if the file does not exist as expected
                if (File.Exists(FormBase.ConfigurationSettings.ConfigurationPath +
                                FormBase.GlobalParameters.ConfigfileName + '_' +
                                FormBase.ConfigurationSettings.WorkingEnvironment +
                                FormBase.GlobalParameters.FileExtension)) return;
                var newEnvironmentConfiguration = new EnvironmentConfiguration();
                newEnvironmentConfiguration.CreateDummyEnvironmentConfiguration(
                    FormBase.ConfigurationSettings.ConfigurationPath + FormBase.GlobalParameters.ConfigfileName + '_' +
                    FormBase.ConfigurationSettings.WorkingEnvironment + FormBase.GlobalParameters.FileExtension);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred while creation the default Configuration File. The error message is " + ex,
                    "An issue has been encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Create a new dummy validation file
            try
            {
                // Create a default configuration file if the file does not exist as expected
                if (File.Exists(FormBase.ConfigurationSettings.ConfigurationPath +
                                FormBase.GlobalParameters.ValidationFileName + '_' +
                                FormBase.ConfigurationSettings.WorkingEnvironment +
                                FormBase.GlobalParameters.FileExtension)) return;
                var newEnvironmentConfiguration = new EnvironmentConfiguration();
                newEnvironmentConfiguration.CreateDummyEnvironmentConfiguration(
                    FormBase.ConfigurationSettings.ConfigurationPath + FormBase.GlobalParameters.ValidationFileName +
                    '_' + FormBase.ConfigurationSettings.WorkingEnvironment + FormBase.GlobalParameters.FileExtension);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred while creation the default Configuration File. The error message is " + ex,
                    "An issue has been encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// Retrieve the values of the application root path (where the paths to the configuration file is maintained)
        /// </summary>
        public static void LoadRootPathFile()
        {
            // This is the hardcoded base path that always needs to be accessible, it has the main file which can locate the rest of the configuration
            var configList = new Dictionary<string, string>();
            var fs = new FileStream(
                FormBase.GlobalParameters.ConfigurationPath + FormBase.GlobalParameters.PathfileName +
                FormBase.GlobalParameters.FileExtension, FileMode.Open, FileAccess.Read);
            var sr = new StreamReader(fs);

            try
            {
                string textline;
                while ((textline = sr.ReadLine()) != null)
                {
                    if (textline.IndexOf(@"/*", StringComparison.Ordinal) == -1)
                    {
                        var line = textline.Split('|');
                        configList.Add(line[0], line[1]);
                    }
                }

                sr.Close();
                fs.Close();

                // These variables are used as global vairables throughout the application
                FormBase.ConfigurationSettings.ConfigurationPath = configList["ConfigurationPath"];
                FormBase.ConfigurationSettings.OutputPath = configList["OutputPath"];
                FormBase.ConfigurationSettings.WorkingEnvironment = configList["WorkingEnvironment"];

            }
            catch (Exception)
            {
                // richTextBoxInformation.AppendText("\r\n\r\nAn error occured while interpreting the configuration file. The original error is: '" + ex.Message + "'");
            }
        }


        /// <summary>
        ///    Retrieve the configuration information from memory and save this to disk
        /// </summary>
        internal static void SaveConfigurationFile()
        {
            try
            {
                var configurationFile = new StringBuilder();
                configurationFile.AppendLine("/* TEAM Configuration Settings */");
                configurationFile.AppendLine("/* Saved at " + DateTime.Now + " */");
                configurationFile.AppendLine("SourceDatabase|" + FormBase.ConfigurationSettings.SourceDatabaseName +
                                             "");
                configurationFile.AppendLine("StagingDatabase|" + FormBase.ConfigurationSettings.StagingDatabaseName +
                                             "");
                configurationFile.AppendLine("PersistentStagingDatabase|" +
                                             FormBase.ConfigurationSettings.PsaDatabaseName + "");
                configurationFile.AppendLine("IntegrationDatabase|" +
                                             FormBase.ConfigurationSettings.IntegrationDatabaseName + "");
                configurationFile.AppendLine("PresentationDatabase|" +
                                             FormBase.ConfigurationSettings.PresentationDatabaseName +
                                             "");
                configurationFile.AppendLine("OutputPath|" + FormBase.ConfigurationSettings.OutputPath + "");
                configurationFile.AppendLine("ConfigurationPath|" + FormBase.ConfigurationSettings.ConfigurationPath +
                                             "");
                configurationFile.AppendLine(@"connectionStringSource|" +
                                             FormBase.ConfigurationSettings.ConnectionStringSource +
                                             "");
                configurationFile.AppendLine(@"connectionStringStaging|" +
                                             FormBase.ConfigurationSettings.ConnectionStringStg +
                                             "");
                configurationFile.AppendLine(@"connectionStringPersistentStaging|" +
                                             FormBase.ConfigurationSettings.ConnectionStringHstg + "");
                configurationFile.AppendLine(@"connectionStringMetadata|" +
                                             FormBase.ConfigurationSettings.ConnectionStringOmd +
                                             "");
                configurationFile.AppendLine(@"connectionStringIntegration|" +
                                             FormBase.ConfigurationSettings.ConnectionStringInt + "");
                configurationFile.AppendLine(@"connectionStringPresentation|" +
                                             FormBase.ConfigurationSettings.ConnectionStringPres + "");
                configurationFile.AppendLine("SourceSystemPrefix|" + FormBase.ConfigurationSettings.SourceSystemPrefix +
                                             "");
                configurationFile.AppendLine("StagingAreaPrefix|" + FormBase.ConfigurationSettings.StgTablePrefixValue +
                                             "");
                configurationFile.AppendLine("PersistentStagingAreaPrefix|" +
                                             FormBase.ConfigurationSettings.PsaTablePrefixValue + "");
                configurationFile.AppendLine(
                    "HubTablePrefix|" + FormBase.ConfigurationSettings.HubTablePrefixValue + "");
                configurationFile.AppendLine(
                    "SatTablePrefix|" + FormBase.ConfigurationSettings.SatTablePrefixValue + "");
                configurationFile.AppendLine("LinkTablePrefix|" + FormBase.ConfigurationSettings.LinkTablePrefixValue +
                                             "");
                configurationFile.AppendLine(
                    "LinkSatTablePrefix|" + FormBase.ConfigurationSettings.LsatPrefixValue + "");
                configurationFile.AppendLine("KeyIdentifier|" + FormBase.ConfigurationSettings.DwhKeyIdentifier + "");
                configurationFile.AppendLine("SchemaName|" + FormBase.ConfigurationSettings.SchemaName + "");
                configurationFile.AppendLine("RowID|" + FormBase.ConfigurationSettings.RowIdAttribute + "");
                configurationFile.AppendLine("EventDateTimeStamp|" +
                                             FormBase.ConfigurationSettings.EventDateTimeAttribute + "");
                configurationFile.AppendLine("LoadDateTimeStamp|" +
                                             FormBase.ConfigurationSettings.LoadDateTimeAttribute + "");
                configurationFile.AppendLine(
                    "ExpiryDateTimeStamp|" + FormBase.ConfigurationSettings.ExpiryDateTimeAttribute + "");
                configurationFile.AppendLine("ChangeDataIndicator|" +
                                             FormBase.ConfigurationSettings.ChangeDataCaptureAttribute +
                                             "");
                configurationFile.AppendLine(
                    "RecordSourceAttribute|" + FormBase.ConfigurationSettings.RecordSourceAttribute + "");
                configurationFile.AppendLine("ETLProcessID|" + FormBase.ConfigurationSettings.EtlProcessAttribute + "");
                configurationFile.AppendLine("ETLUpdateProcessID|" +
                                             FormBase.ConfigurationSettings.EtlProcessUpdateAttribute +
                                             "");
                configurationFile.AppendLine("LogicalDeleteAttribute|" +
                                             FormBase.ConfigurationSettings.LogicalDeleteAttribute +
                                             "");
                configurationFile.AppendLine("LinkedServerName|" + FormBase.ConfigurationSettings.LinkedServer + "");
                configurationFile.AppendLine("TableNamingLocation|" +
                                             FormBase.ConfigurationSettings.TableNamingLocation + "");
                configurationFile.AppendLine("KeyNamingLocation|" + FormBase.ConfigurationSettings.KeyNamingLocation +
                                             "");


                configurationFile.AppendLine("RecordChecksum|" +
                                             FormBase.ConfigurationSettings.RecordChecksumAttribute + "");
                configurationFile.AppendLine("CurrentRecordAttribute|" +
                                             FormBase.ConfigurationSettings.CurrentRowAttribute +
                                             "");

                configurationFile.AppendLine("AlternativeRecordSource|" +
                                             FormBase.ConfigurationSettings.AlternativeRecordSourceAttribute + "");
                configurationFile.AppendLine("AlternativeHubLDTS|" +
                                             FormBase.ConfigurationSettings.AlternativeLoadDateTimeAttribute + "");
                configurationFile.AppendLine("AlternativeSatelliteLDTS|" +
                                             FormBase.ConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute +
                                             "");
                configurationFile.AppendLine("AlternativeRecordSourceFunction|" +
                                             FormBase.ConfigurationSettings.EnableAlternativeRecordSourceAttribute +
                                             "");
                configurationFile.AppendLine("AlternativeHubLDTSFunction|" +
                                             FormBase.ConfigurationSettings.EnableAlternativeLoadDateTimeAttribute +
                                             "");
                configurationFile.AppendLine("AlternativeSatelliteLDTSFunction|" +
                                             FormBase.ConfigurationSettings
                                                 .EnableAlternativeSatelliteLoadDateTimeAttribute +
                                             "");

                configurationFile.AppendLine("PSAKeyLocation|" + FormBase.ConfigurationSettings.PsaKeyLocation + "");
                configurationFile.AppendLine("metadataRepositoryType|" +
                                             FormBase.ConfigurationSettings.MetadataRepositoryType +
                                             "");

                // Closing off
                configurationFile.AppendLine("/* End of file */");

                using (var outfile =
                    new StreamWriter(FormBase.ConfigurationSettings.ConfigurationPath +
                                     FormBase.GlobalParameters.ConfigfileName + '_' +
                                     FormBase.ConfigurationSettings.WorkingEnvironment +
                                     FormBase.GlobalParameters.FileExtension))
                {
                    outfile.Write(configurationFile.ToString());
                    outfile.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured saving the Configuration File. The error message is " + ex,
                    "An issue has been encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        ///    Retrieve the configuration information from disk and save this to memory
        /// </summary>
        internal static void LoadConfigurationFile(string filename)
        {
            try
            {
                var configList = new Dictionary<string, string>();
                var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                var sr = new StreamReader(fs);

                string textline;
                while ((textline = sr.ReadLine()) != null)
                {
                    if (textline.IndexOf(@"/*", StringComparison.Ordinal) == -1)
                    {
                        var line = textline.Split('|');
                        configList.Add(line[0], line[1]);
                    }
                }

                sr.Close();
                fs.Close();

                var connectionStringOmd = configList["connectionStringMetadata"];
                connectionStringOmd = connectionStringOmd.Replace("Provider=SQLNCLI10;", "")
                    .Replace("Provider=SQLNCLI11;", "").Replace("Provider=SQLNCLI12;", "");

                var connectionStringSource = configList["connectionStringSource"];
                connectionStringSource = connectionStringSource.Replace("Provider=SQLNCLI10;", "")
                    .Replace("Provider=SQLNCLI11;", "").Replace("Provider=SQLNCLI12;", "");

                var connectionStringStg = configList["connectionStringStaging"];
                connectionStringStg = connectionStringStg.Replace("Provider=SQLNCLI10;", "")
                    .Replace("Provider=SQLNCLI11;", "").Replace("Provider=SQLNCLI12;", "");

                var connectionStringHstg = configList["connectionStringPersistentStaging"];
                connectionStringHstg = connectionStringHstg.Replace("Provider=SQLNCLI10;", "")
                    .Replace("Provider=SQLNCLI11;", "").Replace("Provider=SQLNCLI12;", "");

                var connectionStringInt = configList["connectionStringIntegration"];
                connectionStringInt = connectionStringInt.Replace("Provider=SQLNCLI10;", "")
                    .Replace("Provider=SQLNCLI11;", "").Replace("Provider=SQLNCLI12;", "");

                var connectionStringPres = configList["connectionStringPresentation"];
                connectionStringPres = connectionStringPres.Replace("Provider=SQLNCLI10;", "")
                    .Replace("Provider=SQLNCLI11;", "").Replace("Provider=SQLNCLI12;", "");

                // These variables are used as global vairables throughout the application
                // They will be set once after startup
                FormBase.ConfigurationSettings.ConnectionStringSource = connectionStringSource;
                FormBase.ConfigurationSettings.ConnectionStringStg = connectionStringStg;
                FormBase.ConfigurationSettings.ConnectionStringHstg = connectionStringHstg;
                FormBase.ConfigurationSettings.ConnectionStringInt = connectionStringInt;
                FormBase.ConfigurationSettings.ConnectionStringOmd = connectionStringOmd;
                FormBase.ConfigurationSettings.ConnectionStringPres = connectionStringPres;
                FormBase.ConfigurationSettings.MetadataRepositoryType = configList["metadataRepositoryType"];
                FormBase.ConfigurationSettings.StgTablePrefixValue = configList["StagingAreaPrefix"];
                FormBase.ConfigurationSettings.PsaTablePrefixValue = configList["PersistentStagingAreaPrefix"];
                FormBase.ConfigurationSettings.HubTablePrefixValue = configList["HubTablePrefix"];
                FormBase.ConfigurationSettings.SatTablePrefixValue = configList["SatTablePrefix"];
                FormBase.ConfigurationSettings.LinkTablePrefixValue = configList["LinkTablePrefix"];
                FormBase.ConfigurationSettings.LsatPrefixValue = configList["LinkSatTablePrefix"];
                FormBase.ConfigurationSettings.DwhKeyIdentifier = configList["KeyIdentifier"];
                FormBase.ConfigurationSettings.PsaKeyLocation = configList["PSAKeyLocation"];
                FormBase.ConfigurationSettings.TableNamingLocation = configList["TableNamingLocation"];
                FormBase.ConfigurationSettings.KeyNamingLocation = configList["KeyNamingLocation"];
                FormBase.ConfigurationSettings.SchemaName = configList["SchemaName"];
                FormBase.ConfigurationSettings.SourceSystemPrefix = configList["SourceSystemPrefix"];
                FormBase.ConfigurationSettings.EventDateTimeAttribute = configList["EventDateTimeStamp"];
                FormBase.ConfigurationSettings.LoadDateTimeAttribute = configList["LoadDateTimeStamp"];
                FormBase.ConfigurationSettings.ExpiryDateTimeAttribute = configList["ExpiryDateTimeStamp"];
                FormBase.ConfigurationSettings.ChangeDataCaptureAttribute = configList["ChangeDataIndicator"];
                FormBase.ConfigurationSettings.RecordSourceAttribute = configList["RecordSourceAttribute"];
                FormBase.ConfigurationSettings.EtlProcessAttribute = configList["ETLProcessID"];
                FormBase.ConfigurationSettings.EtlProcessUpdateAttribute = configList["ETLUpdateProcessID"];
                FormBase.ConfigurationSettings.RowIdAttribute = configList["RowID"];
                FormBase.ConfigurationSettings.RecordChecksumAttribute = configList["RecordChecksum"];
                FormBase.ConfigurationSettings.CurrentRowAttribute = configList["CurrentRecordAttribute"];
                FormBase.ConfigurationSettings.LogicalDeleteAttribute = configList["LogicalDeleteAttribute"];
                FormBase.ConfigurationSettings.EnableAlternativeRecordSourceAttribute =
                    configList["AlternativeRecordSourceFunction"];
                FormBase.ConfigurationSettings.AlternativeRecordSourceAttribute = configList["AlternativeRecordSource"];
                FormBase.ConfigurationSettings.EnableAlternativeLoadDateTimeAttribute =
                    configList["AlternativeHubLDTSFunction"];
                FormBase.ConfigurationSettings.AlternativeLoadDateTimeAttribute = configList["AlternativeHubLDTS"];
                FormBase.ConfigurationSettings.EnableAlternativeSatelliteLoadDateTimeAttribute =
                    configList["AlternativeSatelliteLDTSFunction"];
                FormBase.ConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute =
                    configList["AlternativeSatelliteLDTS"];
                FormBase.ConfigurationSettings.SourceDatabaseName = configList["SourceDatabase"];
                FormBase.ConfigurationSettings.StagingDatabaseName = configList["StagingDatabase"];
                FormBase.ConfigurationSettings.PsaDatabaseName = configList["PersistentStagingDatabase"];
                FormBase.ConfigurationSettings.IntegrationDatabaseName = configList["IntegrationDatabase"];
                FormBase.ConfigurationSettings.PresentationDatabaseName = configList["PresentationDatabase"];
                FormBase.ConfigurationSettings.OutputPath = configList["OutputPath"];
                FormBase.ConfigurationSettings.ConfigurationPath = configList["ConfigurationPath"];
                FormBase.ConfigurationSettings.LinkedServer = configList["LinkedServerName"];
            }
            catch (Exception)
            {
                // richTextBoxInformation.AppendText("\r\n\r\nAn error occured while interpreting the configuration file. The original error is: '" + ex.Message + "'");
            }
        }

    }


}
