using System.Collections.Generic;
using System.Windows.Forms;
using TEAM_Library;

namespace Virtual_Data_Warehouse
{
    public partial class FormBase : Form
    {
        protected FormMain MyParent;

        public FormBase()
        {
            InitializeComponent();
        }

        public FormBase(FormMain myParent)
        {
            MyParent = myParent;
            InitializeComponent();
        }
        internal static LoadPatternGridView _loadPatternGridView;

        // List of TEAM working environments.
        public static TeamEnvironmentCollection TeamEnvironmentCollection { get; set; } = new TeamEnvironmentCollection();

        // TEAM configuration settings, containing all the conventions etc.
        public static TeamConfiguration TeamConfigurationSettings { get; set; } = new TeamConfiguration();

        /// <summary>
        /// Application specific global parameters - not meant to be updated via the GUI.
        /// </summary>
        internal static class GlobalParameters
        {
            // VDW core parameters, not meant to be updated
            public static string RootPath { get; } = Application.StartupPath;
            public static string CorePath { get; } = Application.StartupPath + @"\" + @"Core\";
            public static string VdwConfigurationPath { get; } = RootPath + @"\Configuration\";
            public static string VdwConfigurationFileName { get; } = "VDW_Configuration.txt";
            public static string LoadPatternListFileName { get; } = "loadPatternCollection.json";

            // TEAM core file names, not meant to be updated
            public static string TeamConfigurationFileName { get; } = "TEAM_configuration";
            public static string ConfigurationFileExtension { get; } = ".txt";

            // Json
            public static string JsonConnectionFileName { get; } = "TEAM_connections";
            public static string JsonEnvironmentFileName { get; } = "TEAM_environments";
            public static string JsonExtension { get; } = ".json";

            public static string JsonSchemaForDataWarehouseAutomationFileName { get; } = "interfaceDataWarehouseAutomationMetadata.json";
        }

        /// <summary>
        /// These are the VDW specific configuration settings (i.e. not TEAM driven)
        /// Elements in this class are saved to / retrieved from the VDW Core Settings file
        /// </summary>
        internal static class VdwConfigurationSettings
        {
            internal static EventLog VdwEventLog { get; set; } = new EventLog();

            public static string VdwSchema { get; set; } = "dbo";
            public static string LoadPatternPath { get; set; } = GlobalParameters.RootPath + @"\LoadPatterns\";
            public static string VdwMetadatPath { get; set; } = Application.StartupPath + @"\Metadata\";
            public static string VdwOutputPath { get; set; } = Application.StartupPath + @"\Output\";
            public static string VdwExamplesPath { get; set; } = Application.StartupPath + @"\Examples\";
            
            public static List<LoadPatternFileHandling> patternList { get; set; }

            // Related to TEAM configuration settings
            //public static string TeamEnvironmentFilePath { get; set; } = GlobalParameters.RootPath + @"\Configuration\";
            public static string TeamConfigurationPath { get; set; } = Application.StartupPath + @"\Configuration\";
            public static string TeamConnectionsPath { get; set; } = Application.StartupPath + @"\Configuration\";
            public static string TeamEnvironmentFilePath { get; set; } = GlobalParameters.RootPath + @"\Configuration\" + GlobalParameters.JsonEnvironmentFileName + GlobalParameters.JsonExtension;

            // TEAM active working environment.
            public static string TeamSelectedEnvironmentInternalId { get; set; } = "";
            public static TeamEnvironment ActiveEnvironment { get; set; } = new TeamEnvironment();

            // In memory settings for reloading.
            public static string SelectedSubTab { get; set; }
            public static string SelectedMainTab { get; set; }
            public static string SelectedPatternText { get; set; }
        }
    }
}
