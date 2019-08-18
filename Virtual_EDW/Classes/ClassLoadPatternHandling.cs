﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using static Virtual_Data_Warehouse.FormBase;

namespace Virtual_Data_Warehouse
{
    class LoadPatternDefinition
    {
        public int LoadPatternKey { get; set; }
        public string LoadPatternType { get; set; }
        public string LoadPatternSelectionQuery { get; set; }
        public string LoadPatternBaseQuery { get; set; }
        public string LoadPatternAttributeQuery { get; set; }
        public string LoadPatternNotes { get; set; }

        /// <summary>
        /// Create a file backup for the configuration file at the provided location and return notice of success or failure as a string.
        /// /// </summary>
        internal static string BackupLoadPatternDefinition(string loadPatternDefinitionFilePath)
        {
            string returnMessage = "";

            try
            {
                if (File.Exists(loadPatternDefinitionFilePath))
                {
                    var targetFilePathName = loadPatternDefinitionFilePath +
                                             string.Concat("Backup_" + DateTime.Now.ToString("yyyyMMddHHmmss"));

                    File.Copy(loadPatternDefinitionFilePath, targetFilePathName);
                    returnMessage = "A backup was created at: " + targetFilePathName;
                }
                else
                {
                    returnMessage = "VEDW couldn't locate a configuration file! Can you check the paths and existence of directories?";
                }
            }
            catch (Exception ex)
            {
                returnMessage = ("An error has occured while creating a file backup. The error message is " + ex);
            }

            return returnMessage;
        }

        /// <summary>
        /// The method that backs-up and saves a specific pattern (based on its path) with whatever is passed as contents.
        /// </summary>
        /// <param name="loadPatternFilePath"></param>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        internal static string SaveLoadPattern(string loadPatternDefinitionFilePath, string fileContent)
        {
            string returnMessage = "";

            try
            {
                using (var outfile = new StreamWriter(loadPatternDefinitionFilePath))
                {
                    outfile.Write(fileContent);
                    outfile.Close();
                }

                returnMessage = "The file has been updated.";
            }
            catch (Exception ex)
            {
                returnMessage = ("An error has occured while creating saving the file. The error message is " + ex);
            }


            return returnMessage;
        }


    }

    class LoadPatternDefinitionFileHandling
    {
        internal List<LoadPatternDefinition> DeserializeLoadPatternDefinition()
        {
            List<LoadPatternDefinition> loadPatternDefinitionList = new List<LoadPatternDefinition>();
            // Retrieve the file contents and store in a string
            if (File.Exists(VedwConfigurationSettings.LoadPatternListPath + GlobalParameters.LoadPatternListFile))
            {
                var jsonInput = File.ReadAllText(VedwConfigurationSettings.LoadPatternListPath +
                                                 GlobalParameters.LoadPatternDefinitionFile);

                //Move the (json) string into a List object (a list of the type LoadPattern)
                loadPatternDefinitionList = JsonConvert.DeserializeObject<List<LoadPatternDefinition>>(jsonInput);
            }

            // Return the list to the instance
            return loadPatternDefinitionList;
        }
    }

    /// <summary>
    ///   This class contains the basic information for a load pattern, such as name, type and location.
    /// </summary>
    class LoadPattern
    {
        public string LoadPatternName { get; set; }
        public string LoadPatternType { get; set; }
        public string LoadPatternFilePath { get; set; }
        public string LoadPatternNotes { get; set; }

        /// <summary>
        ///    Create a file backup for the configuration file at the provided location and return notice of success or failure as a string.
        /// </summary>
        internal static string BackupLoadPattern(string loadPatternFilePath)
        {
            string returnMessage = "";

            try
            {
                if (File.Exists(loadPatternFilePath))
                {
                    var targetFilePathName = loadPatternFilePath +
                                             string.Concat("Backup_" + DateTime.Now.ToString("yyyyMMddHHmmss"));

                    File.Copy(loadPatternFilePath, targetFilePathName);
                    returnMessage = "A backup was created at: " + targetFilePathName;
                }
                else
                {
                    returnMessage = "VEDW couldn't locate a configuration file! Can you check the paths and existence of directories?";
                }
            }
            catch (Exception ex)
            {
                returnMessage = ("An error has occured while creating a file backup. The error message is " + ex);
            }

            return returnMessage;
        }

        /// <summary>
        /// The method that backs-up and saves a specific pattern (based on its path) with whatever is passed as contents.
        /// </summary>
        /// <param name="loadPatternFilePath"></param>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        internal static string SaveLoadPattern(string loadPatternFilePath, string fileContent)
        {
            string returnMessage = "";

            try
            {
                using (var outfile = new StreamWriter(loadPatternFilePath))
                {
                    outfile.Write(fileContent);
                    outfile.Close();
                }

                returnMessage = "The file has been updated.";
            }
            catch (Exception ex) 
            {
                returnMessage = ("An error has occured while creating saving the file. The error message is " + ex);
            }


            return returnMessage;
        }

    }

    class LoadPatternCollectionFileHandling
    {
        internal List<LoadPattern> DeserializeLoadPatternCollection()
        {
            List<LoadPattern> loadPatternList = new List<LoadPattern>();
            // Retrieve the file contents and store in a string
            if (File.Exists(VedwConfigurationSettings.LoadPatternListPath + GlobalParameters.LoadPatternListFile))
            {
                var jsonInput = File.ReadAllText(VedwConfigurationSettings.LoadPatternListPath +
                                                 GlobalParameters.LoadPatternListFile);

                //Move the (json) string into a List object (a list of the type LoadPattern)
                loadPatternList = JsonConvert.DeserializeObject<List<LoadPattern>>(jsonInput);
            }

            // Return the list to the instance
            return loadPatternList;
        }
    }

    #region Object Models
    /// <summary>
    /// The parent object containing the list of source-to-target mappings. This is the highest level and contains the list of mappings (as individual objects
    /// but also the parameters inherited from TEAM and VEDW.
    /// </summary>
    class SourceToTargetMappingList
    {
        public string mainTable { get; set; }
        public List<SourceToTargetMapping> individualSourceToTargetMapping { get; set; }
        public DateTime generationDateTime { get; } = DateTime.Now;
        public MetadataConfiguration metadataConfiguration { get; set; }
    }

    /// <summary>
    /// The mapping between a source and target data set / table / file.
    /// </summary>
    class SourceToTargetMapping
    {
        public string sourceTable { get; set; }
        public string targetTable { get; set; }
        public string lookupTable { get; set; }
        public string targetTableHashKey { get; set; }
        public List<BusinessKey> businessKey { get; set; }
        public string filterCriterion { get; set; }
        public List<ColumnMapping> columnMapping { get; set; }
    }

    /// <summary>
    /// A Business Key, which consists of one or more components (column mappings) and has its own surrogate key.
    /// </summary>
    class BusinessKey
    {
        public List<ColumnMapping> businessKeyComponentMapping { get; set; }
        public string surrogateKey { get; set; }
    }

    /// <summary>
    /// The individual column-to-column mapping
    /// </summary>
    class ColumnMapping
    {
        public Column sourceColumn { get; set; }
        public Column targetColumn { get; set; }
    }

    /// <summary>
    /// And individual column
    /// </summary>
    class Column
    {
        public string columnName { get; set; }
        public string columnType { get; set; }
    }


    /// <summary>
    /// The parameters that have been inherited from TEAM or are set in VEDW, passed as properties of the metadata.
    /// </summary>
    class MetadataConfiguration
    {
        // Databases
        public string sourceDatabaseName { get; } = FormBase.TeamConfigurationSettings.SourceDatabaseName;
        public string stagingAreaDatabaseName { get; } = FormBase.TeamConfigurationSettings.StagingDatabaseName;
        public string persistentStagingDatabaseName { get; } = FormBase.TeamConfigurationSettings.PsaDatabaseName;
        public string persistentStagingSchemaName { get;} = FormBase.TeamConfigurationSettings.SchemaName;
        public string integrationDatabaseName { get; } = FormBase.TeamConfigurationSettings.IntegrationDatabaseName;
        public string presentationDatabaseName { get; } = FormBase.TeamConfigurationSettings.PresentationDatabaseName;
        public string vedwSchemaName { get; } = FormBase.VedwConfigurationSettings.VedwSchema;
        // Attributes
        public string changeDataCaptureAttribute { get; set; } = FormBase.TeamConfigurationSettings.ChangeDataCaptureAttribute;
        public string recordSourceAttribute { get; } = FormBase.TeamConfigurationSettings.RecordSourceAttribute;
        public string loadDateTimeAttribute { get; } = FormBase.TeamConfigurationSettings.LoadDateTimeAttribute;
        public string eventDateTimeAttribute { get; set; } = FormBase.TeamConfigurationSettings.EventDateTimeAttribute;
        public string recordChecksumAttribute { get; set; } = FormBase.TeamConfigurationSettings.RecordChecksumAttribute;
        public string etlProcessAttribute { get; } = FormBase.TeamConfigurationSettings.EtlProcessAttribute;
        public string sourceRowIdAttribute { get; } = FormBase.TeamConfigurationSettings.RowIdAttribute;
    }
    #endregion
}