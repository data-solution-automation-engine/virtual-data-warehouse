using System;
using DataWarehouseAutomation;

namespace Virtual_Data_Warehouse
{
    /// <summary>
    /// The parent object containing the list of source-to-target mappings. This is the highest level and contains the list of mappings (as individual objects
    /// but also the parameters inherited from TEAM and VDW.
    /// </summary>
    class VDW_DataObjectMappingList : DataObjectMappings
    {
        // Generic interface definitions
        //public List<DataObjectMapping> dataObjectMapping { get; set; }

        // TEAM and VDW specific details
        public MetadataConfiguration metadataConfiguration { get; set; }
        public GenerationSpecificMetadata generationSpecificMetadata { get; set; }
        public string metadataFileName { get; set; }
    }


    /// <summary>
    /// Specific metadata related for generation purposes, but which is relevant to use in templates.
    /// </summary>
    public class GenerationSpecificMetadata
    {
        public string selectedDataObject { get; set; }
        public DateTime generationDateTime { get; set; }
    }

    /// <summary>
    /// The parameters that have been inherited from TEAM or are set in VDW, passed as properties of the metadata - and can be used in the templates.
    /// </summary>
    class MetadataConfiguration
    {
        public string vdwSchemaName { get; set; } = FormBase.VdwConfigurationSettings.VdwSchema;

        // Attributes
        public string changeDataCaptureAttribute { get; set; }
        public string recordSourceAttribute { get; set; }
        public string loadDateTimeAttribute { get; set; }
        public string expiryDateTimeAttribute { get; set; }
        public string eventDateTimeAttribute { get; set; }
        public string recordChecksumAttribute { get; set; }
        public string etlProcessAttribute { get; set; }
        public string sourceRowIdAttribute { get; set; }
    }
}
