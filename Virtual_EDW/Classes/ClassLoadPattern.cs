using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Virtual_EDW;

namespace Virtual_EDW
{
    /// <summary>
    ///   This class contains the basic information for a load pattern, such as name, type and location.
    /// </summary>
    class LoadPattern
    {
        public string loadPatternName { get; set; }
        public string loadPatternType { get; set; }
        public string loadPatternFilePath { get; set; }

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

        /// <summary>
        ///   Make sure the load pattern is centrally available
        /// </summary>
        /// <param name="loadPattern"></param>
        /// <param name="loadPatternType></param>
        internal static void ActivateLoadPattern(string loadPattern, string loadPatternType)
        {
            if (loadPatternType == "Hub")
            {
                FormBase.VedwConfigurationSettings.activeLoadPatternHub = loadPattern;
            }
        }
    }

    class LoadPatternHandling
    {

        internal List<LoadPattern> DeserializeLoadPatternCollection()
        {
            // Retrieve metadata and store in a data table object
            var jsonInput = File.ReadAllText(@"D:\Git_Repositories\Virtual_Enterprise_Data_Warehouse\loadPatterns\loadPatternCollection.json");

            List<LoadPattern> loadPatternList = JsonConvert.DeserializeObject<List<LoadPattern>>(jsonInput);

            // Update the list in memory
            FormBase.VedwConfigurationSettings.patternList = loadPatternList;

            // Return the list to the instance
            return loadPatternList;
        }

    }

    #region Object Models
    class SourceToTargetMappingList
    {
        public string mainTable { get; set; }
        public List<SourceToTargetMapping> individualSourceToTargetMapping { get; set; }
        public DateTime generationDateTime { get; } = DateTime.Now;
        public MetadataConfiguration metadataConfiguration { get; set; }
    }

    class SourceToTargetMapping
    {
        public string sourceTable { get; set; }
        public string targetTable { get; set; }
        public string targetTableHashKey { get; set; }
        public BusinessKey businessKey { get; set; }
        public string filterCriterion { get; set; }
    }

    class BusinessKey
    {
        public List<BusinessKeyComponentMapping> businessKeyComponentMapping { get; set; }
    }

    class BusinessKeyComponentMapping
    {
        public string sourceComponentName { get; set; } 
        public string targetComponentName { get; set; }
    }

    class MetadataConfiguration
    {
        public string psadatabaseName { get; } = FormBase.TeamConfigurationSettings.PsaDatabaseName;
        public string psaSchemaName { get;} = FormBase.TeamConfigurationSettings.SchemaName;
        public string vedwSchemaName { get; } = FormBase.VedwConfigurationSettings.VedwSchema;
        public string recordSourceAttribute { get; } = FormBase.TeamConfigurationSettings.RecordSourceAttribute;
        public string loadDateTimeAttribute { get; } = FormBase.TeamConfigurationSettings.LoadDateTimeAttribute;
        public string etlProcessAttribute { get; } = FormBase.TeamConfigurationSettings.EtlProcessAttribute;
    }
    #endregion
}
