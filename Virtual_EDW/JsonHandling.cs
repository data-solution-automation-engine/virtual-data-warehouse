using System;
using System.IO;

namespace Virtual_Data_Warehouse
{
    class JsonHandling
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
    }
}
