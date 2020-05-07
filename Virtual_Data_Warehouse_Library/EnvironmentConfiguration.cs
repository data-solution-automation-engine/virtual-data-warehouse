using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Virtual_Data_Warehouse_Library
{
    public class EnvironmentConfiguration
    {
        /// <summary>
        /// Check if a path exists and create it if necessary.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static EventLog InitialisePath(string filePath)
        {
            EventLog eventLog = new EventLog();

            try
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                    eventLog.Add(Event.CreateNewEvent(EventTypes.Information, $"Created a new root path directory for {filePath}.\r\n"));
                }
            }
            catch (Exception ex)
            {
                eventLog.Add(Event.CreateNewEvent(EventTypes.Error, $"Error creation default directory at {filePath} the message is {ex}.\r\n"));
            }

            return eventLog;
        }

        /// <summary>
        /// Create a new file with input content, if it does not exist yet.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        public static EventLog CreateConfigurationFile(string fileName, StringBuilder fileContent)
        {
            EventLog eventLog = new EventLog();

            try
            {
                if (!File.Exists(fileName))
                {
                    using (var outfile = new StreamWriter(fileName))
                    {
                        outfile.Write(fileContent.ToString());
                        outfile.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                eventLog.Add(Event.CreateNewEvent(EventTypes.Error, $"Error creating a new configuration file at {fileName}. The message is {ex}.\r\n"));
            }

            return eventLog;
        }

        /// <summary>
        /// Retrieve the values of a settings file and return this as a dictionary<string,string> object containing the configuration settings.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Dictionary<string,string> LoadConfigurationFile(string filename)
        {
            // This is the hardcoded base path that always needs to be accessible, it has the main file which can locate the rest of the configuration
            var configList = new Dictionary<string, string>();

            var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            var sr = new StreamReader(fs);

            string textLine;
            while ((textLine = sr.ReadLine()) != null)
            {
                if (textLine.IndexOf(@"/*", StringComparison.Ordinal) == -1 && textLine.Trim() != "")
                {
                    var line = textLine.Split('|');
                    configList.Add(line[0], line[1]);
                }
            }

            sr.Close();
            fs.Close();

            return configList;
        }
    }
}
