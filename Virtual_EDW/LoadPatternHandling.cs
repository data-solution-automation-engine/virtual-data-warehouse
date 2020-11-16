using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Virtual_Data_Warehouse
{

    /// <summary>
    ///   This class contains the basic information for a load pattern, such as name, type and location.
    /// </summary>
    class LoadPattern
    {
        public string LoadPatternName { get; set; }
        public string LoadPatternType { get; set; }
        public string LoadPatternConnectionKey { get; set; }
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
                    returnMessage = "A pattern file could not be located to backup. Can you check the paths and existence of directories?";
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
            string returnMessage;

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
        internal static List<LoadPattern> DeserializeLoadPatternCollection()
        {
            List<LoadPattern> loadPatternList = new List<LoadPattern>();
            // Retrieve the file contents and store in a string
            if (File.Exists(FormBase.VdwConfigurationSettings.LoadPatternPath + FormBase.GlobalParameters.LoadPatternListFileName))
            {
                var jsonInput = File.ReadAllText(FormBase.VdwConfigurationSettings.LoadPatternPath +
                                                 FormBase.GlobalParameters.LoadPatternListFileName);

                //Move the (json) string into a List object (a list of the type LoadPattern)
                loadPatternList = JsonConvert.DeserializeObject<List<LoadPattern>>(jsonInput);
            }

            // Return the list to the instance
            return loadPatternList;
        }
    }
}
