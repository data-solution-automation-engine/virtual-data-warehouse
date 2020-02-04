using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using static Virtual_Data_Warehouse.FormBase;

namespace Virtual_Data_Warehouse
{
    class ClassJsonHandling
    {

        /// <summary>
        ///    Create a backup of a given JSON file.
        /// </summary>
        /// <param name="inputFileName"></param>
        internal string BackupJsonFile(string inputFileName)
        {
            string result;
            var shortDatetime = DateTime.Now.ToString("yyyyMMddHHmmss");
            var targetFilePathName = string.Concat(inputFileName,"_",shortDatetime,"_backup.json");

            File.Copy(inputFileName, targetFilePathName);
            result = targetFilePathName;

            return result;
        }

        /// <summary>
        ///   Clear out (remove) an existing Json file, to facilitate overwriting.
        /// </summary>
        internal static void RemoveExistingJsonFile(string inputFileName)
        {
            File.Delete(VedwConfigurationSettings.VedwInputPath + inputFileName);
        }

    }
}
