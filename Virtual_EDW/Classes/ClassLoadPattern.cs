using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Virtual_EDW;

namespace Virtual_Data_Warehouse.Classes
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
    class MappingList
    {
        public List<IndividualMetadataMapping> metadataMapping { get; set; }
    }

    class IndividualMetadataMapping
    {
        public string businessKeySource { get; set; }
        public string businessKeyTarget { get; set; }
        public string hubTable { get; set; }
        public string hubTableHashKey { get; set; }
        public string sourceTable { get; set; }
    }
    #endregion



}
