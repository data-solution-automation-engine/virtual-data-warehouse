using System;
using System.Collections.Generic;
using DataWarehouseAutomation;

namespace Virtual_Data_Warehouse.Classes
{
    /// <summary>
    /// The parent object containing the list of source-to-target mappings. This is the highest level and contains the list of mappings (as individual objects
    /// but also the parameters inherited from TEAM and VEDW.
    /// </summary>
    class VEDW_DataObjectMappingList : DataObjectMappingList
    {
        // Generic interface definitions
        //public List<DataObjectMapping> dataObjectMapping { get; set; }

        // TEAM and VDW specific details
        public MetadataConfiguration metadataConfiguration { get; set; }
        public GenerationSpecificMetadata generationSpecificMetadata { get; set; }
    }

    /// <summary>
    /// Specific metadata related for generation purposes, but which is relevant to use in templates.
    /// </summary>
    public class GenerationSpecificMetadata
    {
        public string selectedDataObject { get; set; }
        public DateTime generationDateTime { get; } = DateTime.Now;
    }

    /// <summary>
    /// The parameters that have been inherited from TEAM or are set in VDW, passed as properties of the metadata - and can be used in the templates.
    /// </summary>
    class MetadataConfiguration
    {
        // Databases
        public string sourceDatabaseName { get; } = FormBase.TeamConfigurationSettings.SourceDatabaseName;
        public string sourceDatabaseConnection { get; } = FormBase.TeamConfigurationSettings.ConnectionStringSource;
        public string stagingAreaDatabaseName { get; } = FormBase.TeamConfigurationSettings.StagingDatabaseName;
        public string stagingAreaDatabaseConnection { get; } = FormBase.TeamConfigurationSettings.ConnectionStringSource;
        public string persistentStagingDatabaseName { get; } = FormBase.TeamConfigurationSettings.PsaDatabaseName;
        public string persistentStagingDatabaseConnection { get; } = FormBase.TeamConfigurationSettings.ConnectionStringSource;
        public string persistentStagingSchemaName { get; } = FormBase.TeamConfigurationSettings.SchemaName;
        public string integrationDatabaseName { get; } = FormBase.TeamConfigurationSettings.IntegrationDatabaseName;
        public string integrationDatabaseConnection { get; } = FormBase.TeamConfigurationSettings.ConnectionStringSource;
        public string presentationDatabaseName { get; } = FormBase.TeamConfigurationSettings.PresentationDatabaseName;
        public string presentationDatabaseConnection { get; } = FormBase.TeamConfigurationSettings.ConnectionStringSource;
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
}
