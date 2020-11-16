using System.Collections.Generic;
using System.Windows.Forms;
using TEAM;

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

        // List of TEAM working environments.
        public static TeamWorkingEnvironmentCollection TeamEnvironmentCollection { get; set; } = new TeamWorkingEnvironmentCollection();

        // TEAM configuration settings.
        public static TeamConfiguration TeamConfigurationSettings { get; set; } = new TeamConfiguration();

        // List of versions.
        public static TeamVersionList EnvironmentVersion { get; set; } = new TeamVersionList();

        /// <summary>
        /// Application specific global parameters - not meant to be updated via the GUI.
        /// </summary>
        internal static class GlobalParameters
        {

            // VDW core path parameters, not meant to be updated
            public static string RootPath { get; } = Application.StartupPath;
            public static string VdwConfigurationPath { get; } = RootPath + @"\Configuration\";
            public static string VdwConfigurationFileName { get; } = "VDW_Configuration.txt";
            public static string LoadPatternListFileName { get; } = "loadPatternCollection.json";
            public static string LoadPatternDefinitionFileName { get; } = "loadPatternDefinition.json";

            // TEAM core file names, not meant to be updated
            public static string TeamConfigurationFileName { get; } = "TEAM_configuration";
            public static string ConfigurationFileExtension { get; } = ".txt";

            // Json
            public static string JsonConnectionFileName { get; } = "TEAM_connections";
            public static string JsonEnvironmentFileName { get; } = "TEAM_environments";
            public static string JsonExtension { get; } = ".json";
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
            public static string VdwInputPath { get; set; } = Application.StartupPath + @"\Input\";
            public static string VdwOutputPath { get; set; } = Application.StartupPath + @"\Output\";
            public static string VdwExamplesPath { get; set; } = Application.StartupPath + @"\Examples\";



            // Parameters that can be changed at runtime
            public static string hashingStartSnippet { get; set; }
            public static string hashingEndSnippet { get; set; }
            public static string hashingCollation { get; set; }
            public static string hashingZeroKey { get; set; } = "0x00000000000000000000000000000000";

            public static List<LoadPattern> patternList { get; set; }

            // Related to TEAM configuration settings
            //public static string TeamEnvironmentFilePath { get; set; } = GlobalParameters.RootPath + @"\Configuration\";
            public static string TeamConfigurationPath { get; set; } = Application.StartupPath + @"\Configuration\";
            public static string TeamConnectionsPath { get; set; } = Application.StartupPath + @"\Configuration\";
            public static string TeamEnvironmentFilePath { get; set; } = GlobalParameters.RootPath + @"\Configuration\" + GlobalParameters.JsonEnvironmentFileName + GlobalParameters.JsonExtension;

            public static string TeamSelectedEnvironmentInternalId { get; set; } = "";

            // TEAM active working environment
            public static TeamWorkingEnvironment ActiveEnvironment { get; set; } = new TeamWorkingEnvironment();
        }


 
    }
}
