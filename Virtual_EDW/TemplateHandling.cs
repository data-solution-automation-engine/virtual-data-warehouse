using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.Management.Smo;
using Newtonsoft.Json;
using TEAM_Library;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Virtual_Data_Warehouse
{

    /// <summary>
    ///   This class contains the basic information for a template, such as name, type and location.
    /// </summary>
    class TemplateHandling
    {
        public string TemplateName { get; set; }
        public string TemplateType { get; set; }
        public string TemplateConnectionKey { get; set; }
        public string TemplateOutputFileConvention { get; set; }
        public string TemplateFilePath { get; set; }
        public string TemplateNotes { get; set; }

        /// <summary>
        /// Create a file backup for the configuration file at the provided location and return notice of success or failure as a string.
        /// </summary>
        /// <param name="templateFilePath"></param>
        /// <returns></returns>
        internal static string BackupTemplateCollection(string templateFilePath)
        {
            string returnMessage = "";

            try
            {
                if (File.Exists(templateFilePath))
                {
                    var targetFilePathName = templateFilePath + string.Concat("Backup_" + DateTime.Now.ToString("yyyyMMddHHmmss"));

                    File.Copy(templateFilePath, targetFilePathName);
                    returnMessage = "A backup was created at: " + targetFilePathName;
                }
                else
                {
                    returnMessage = "A template file could not be located to backup. Can you check the paths and existence of directories?";
                }
            }
            catch (Exception ex)
            {
                returnMessage = ("An error has occurred while creating a file backup. The error message is " + ex.Message);
            }

            return returnMessage;
        }

        /// <summary>
        /// The method that backs-up and saves a specific template (based on its path) with whatever is passed as contents.
        /// </summary>
        /// <param name="templateFilePath"></param>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        internal static string SaveTemplateCollection(string templateFilePath, string fileContent)
        {
            string returnMessage;

            try
            {
                using (var outfile = new StreamWriter(templateFilePath))
                {
                    outfile.Write(fileContent);
                    outfile.Close();
                }

                returnMessage = "The file has been updated.";
            }
            catch (Exception ex) 
            {
                returnMessage = ("An error has occurred while creating saving the file. The error message is " + ex.Message);
            }

            return returnMessage;
        }

        internal static List<TemplateHandling> LoadTemplateCollection(EventLog eventLog)
        {
            List<TemplateHandling> templateList = new List<TemplateHandling>();

            try
            {
                // Retrieve the file contents and store in a string.
                if (File.Exists(FormBase.VdwConfigurationSettings.TemplatePath + FormBase.GlobalParameters.TemplateCollectionFileName))
                {
                    var jsonInput = File.ReadAllText(FormBase.VdwConfigurationSettings.TemplatePath + FormBase.GlobalParameters.TemplateCollectionFileName);

                    //Move the (json) string into a List object (a list of the type Template).
                    templateList = JsonConvert.DeserializeObject<List<TemplateHandling>>(jsonInput);
                }
            }
            catch (Exception exception)
            {
                eventLog.Add(Event.CreateNewEvent(EventTypes.Error, $"An error was encountered loading the template collection from '{FormBase.VdwConfigurationSettings.TemplatePath + FormBase.GlobalParameters.TemplateCollectionFileName}. The error encountered was '{exception.Message}'."));
            }

            // Return the list to the instance
            return templateList;
        }
    }
}
