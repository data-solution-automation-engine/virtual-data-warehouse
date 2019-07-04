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

    class LoadPatternDetail
    {
        public string loadPatternName { get; set; }
        public string loadPatternType { get; set; }
        public string loadPatternFilePath { get; set; }
    }

    class LoadPatternHandling
    {
        internal List<LoadPatternDetail> DeserializeLoadPatternCollection()
        {

            // Retrieve metadata and store in a data table object
            var jsonInput = File.ReadAllText(@"D:\Git_Repositories\Virtual_Enterprise_Data_Warehouse\loadPatterns\loadPatternCollection.json");

            List<LoadPatternDetail> loadPatternList = JsonConvert.DeserializeObject<List<LoadPatternDetail>>(jsonInput);

            // Update the list in memory
            FormBase.VedwConfigurationSettings.patternList = loadPatternList;

            // Return the list to the instance
            return loadPatternList;
        }
    }

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

}
