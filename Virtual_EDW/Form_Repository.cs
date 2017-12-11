using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace Virtual_EDW
{
    public partial class FormManageRepository : Form
    {
        private readonly FormMain _myParent;
        Form_Alert _alert;

        public FormManageRepository(FormMain parent)
        {
            _myParent = parent;
            InitializeComponent();

        }

        private void buttonDeploy_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy != true)
            {
                // create a new instance of the alert form
                _alert = new Form_Alert();
                // event handler for the Cancel button in AlertForm
                _alert.Canceled += new EventHandler<EventArgs>(buttonCancel_Click);
                _alert.Show();
                // Start the asynchronous operation.
                backgroundWorker1.RunWorkerAsync();
            }
        }

        // This event handler cancels the backgroundworker, fired from Cancel button in AlertForm.
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.WorkerSupportsCancellation == true)
            {
                // Cancel the asynchronous operation.
                backgroundWorker1.CancelAsync();
                // Close the AlertForm
                _alert.Close();
            }
        }

        // This event handler deals with the results of the background operation.
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                labelResult.Text = "Cancelled!";
            }
            else if (e.Error != null)
            {
                labelResult.Text = "Error: " + e.Error.Message;
            }
            else
            {
                labelResult.Text = "Done!";
                MessageBox.Show("The metadata repository has been created.", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);           
            }

        }

        // This event handler updates the progress.
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Show the progress in main form (GUI)
            labelResult.Text = (e.ProgressPercentage + "%");

            // Pass the progress to AlertForm label and progressbar
            _alert.Message = "In progress, please wait... " + e.ProgressPercentage + "%";
            _alert.ProgressValue = e.ProgressPercentage;

            // Manage the logging
        }

        // This event handler is where the time-consuming work is done.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            var connOmdString = _myParent.textBoxMetadataConnection.Text;

            // Handle multithreading
            if (worker.CancellationPending == true)
            {
                e.Cancel = true;
            }
            else
            {
                // Determine the version
                _alert.SetTextLogging("Commencing metadata repository creation.\r\n\r\n");

                var createStatement = new StringBuilder();

                // Drop any existing Foreign Key Constraints
                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_BUSINESS_KEY_COMPONENT_MD_STG_HUB_XREF]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_BUSINESS_KEY_COMPONENT] DROP CONSTRAINT [FK_MD_BUSINESS_KEY_COMPONENT_MD_STG_HUB_XREF]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 0);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_BUSINESS_KEY_COMPONENT_PART_MD_ATT]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_BUSINESS_KEY_COMPONENT_PART] DROP CONSTRAINT [FK_MD_BUSINESS_KEY_COMPONENT_PART_MD_ATT]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 1);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_BUSINESS_KEY_COMPONENT_PART_MD_BUSINESS_KEY_COMPONENT]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_BUSINESS_KEY_COMPONENT_PART] DROP CONSTRAINT [FK_MD_BUSINESS_KEY_COMPONENT_PART_MD_BUSINESS_KEY_COMPONENT]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 2);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_DRIVING_KEY_XREF_MD_HUB]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_DRIVING_KEY_XREF] DROP CONSTRAINT [FK_MD_DRIVING_KEY_XREF_MD_HUB]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 3);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_DRIVING_KEY_XREF_MD_SAT]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_DRIVING_KEY_XREF] DROP CONSTRAINT [FK_MD_DRIVING_KEY_XREF_MD_SAT]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 3);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_HUB_LINK_XREF_MD_HUB]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_HUB_LINK_XREF] DROP CONSTRAINT [FK_MD_HUB_LINK_XREF_MD_HUB]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 3);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_HUB_LINK_XREF_MD_HUB]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_HUB_LINK_XREF] DROP CONSTRAINT [FK_MD_HUB_LINK_XREF_MD_LINK]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 3);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_HUB_LINK_XREF_MD_HUB]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_HUB_LINK_XREF] DROP CONSTRAINT [FK_MD_HUB_LINK_XREF_MD_LINK]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 3);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_SAT_MD_HUB]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_SAT] DROP CONSTRAINT [FK_MD_SAT_MD_HUB]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 3);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_SAT_MD_LINK]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_SAT] DROP CONSTRAINT [FK_MD_SAT_MD_LINK]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 3);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_STG_HUB_XREF_MD_HUB]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_STG_HUB_XREF] DROP CONSTRAINT [FK_MD_STG_HUB_XREF_MD_HUB]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 3);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_STG_HUB_XREF_MD_STG]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_STG_HUB_XREF] DROP CONSTRAINT [FK_MD_STG_HUB_XREF_MD_STG]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 3);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_STG_LINK_ATT_XREF_MD_ATT_FROM]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_STG_LINK_ATT_XREF] DROP CONSTRAINT [FK_MD_STG_LINK_ATT_XREF_MD_ATT_FROM]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 3);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_STG_LINK_ATT_XREF_MD_ATT_TO]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_STG_LINK_ATT_XREF] DROP CONSTRAINT [FK_MD_STG_LINK_ATT_XREF_MD_ATT_TO]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 3);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_STG_LINK_ATT_XREF_MD_STG]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_STG_LINK_ATT_XREF] DROP CONSTRAINT [FK_MD_STG_LINK_ATT_XREF_MD_STG]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 3);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_STG_LINK_ATT_XREF_MD_LINK]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_STG_LINK_ATT_XREF] DROP CONSTRAINT [FK_MD_STG_LINK_ATT_XREF_MD_LINK]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 3);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_STG_SAT_ATT_XREF_MD_ATTRIBUTE_FROM]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_STG_SAT_ATT_XREF] DROP CONSTRAINT [FK_MD_STG_SAT_ATT_XREF_MD_ATTRIBUTE_FROM]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 3);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_STG_SAT_ATT_XREF_MD_ATTRIBUTE_TO]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_STG_SAT_ATT_XREF] DROP CONSTRAINT [FK_MD_STG_SAT_ATT_XREF_MD_ATTRIBUTE_TO]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 3);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_STG_SAT_ATT_XREF_MD_SAT]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_STG_SAT_ATT_XREF] DROP CONSTRAINT [FK_MD_STG_SAT_ATT_XREF_MD_SAT]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 3);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_STG_SAT_ATT_XREF_MD_STG]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_STG_SAT_ATT_XREF] DROP CONSTRAINT [FK_MD_STG_SAT_ATT_XREF_MD_STG]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 3);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_STG_SAT_XREF_MD_SAT]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_STG_SAT_XREF] DROP CONSTRAINT [FK_MD_STG_SAT_XREF_MD_SAT]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 3);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_STG_SAT_XREF_MD_STG]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_STG_SAT_XREF] DROP CONSTRAINT [FK_MD_STG_SAT_XREF_MD_STG]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 3);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_STG_SAT_XREF_MD_STG]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_STG_SAT_XREF] DROP CONSTRAINT [FK_MD_STG_SAT_XREF_MD_STG]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 3);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_STG_LINK_XREF_MD_LINK]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_STG_LINK_XREF] DROP CONSTRAINT [FK_MD_STG_LINK_XREF_MD_LINK]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 3);
                createStatement.Clear();

                createStatement.AppendLine("IF OBJECT_ID('[FK_MD_STG_LINK_XREF_MD_STG]', 'F') IS NOT NULL");
                createStatement.AppendLine("ALTER TABLE [MD_STG_LINK_XREF] DROP CONSTRAINT [FK_MD_STG_LINK_XREF_MD_STG]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 3);
                createStatement.Clear();
       

                // Attribute 
                createStatement.AppendLine();
                createStatement.AppendLine("--Attribute");
                createStatement.AppendLine("IF OBJECT_ID('[MD_ATT]', 'U') IS NOT NULL"); 
                createStatement.AppendLine(" DROP TABLE [MD_ATT]");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE TABLE [MD_ATT]");
                createStatement.AppendLine("(");
                createStatement.AppendLine("    [ATTRIBUTE_ID]       integer NOT NULL ,");
                createStatement.AppendLine("    [ATTRIBUTE_NAME]     varchar(100) NOT NULL,"); 
                createStatement.AppendLine("    CONSTRAINT[PK_MD_ATT] PRIMARY KEY CLUSTERED ( [ATTRIBUTE_ID] ASC)");
                createStatement.AppendLine(")");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE UNIQUE NONCLUSTERED INDEX [IX_MD_ATT] ON [MD_ATT]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("    [ATTRIBUTE_NAME] ASC");
                createStatement.AppendLine(")");

                RunSqlCommand(connOmdString, createStatement, worker, 5);
                createStatement.Clear();

                // Attribute Mapping 
                createStatement.AppendLine();
                createStatement.AppendLine("-- Attribute mapping");
                createStatement.AppendLine("IF OBJECT_ID('[MD_ATTRIBUTE_MAPPING]', 'U') IS NOT NULL");
                createStatement.AppendLine(" DROP TABLE [MD_ATTRIBUTE_MAPPING]");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE TABLE [MD_ATTRIBUTE_MAPPING]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("    [ATTRIBUTE_MAPPING_HASH] AS(");
                createStatement.AppendLine("                CONVERT([CHAR](32),HASHBYTES('MD5',");
                createStatement.AppendLine("                ISNULL(RTRIM(CONVERT(VARCHAR(100),[TARGET_TABLE])),'NA')+'|'+");
                createStatement.AppendLine("                ISNULL(RTRIM(CONVERT(VARCHAR(100),[TARGET_COLUMN])),'NA')+'|'+");
                createStatement.AppendLine("                ISNULL(RTRIM(CONVERT(VARCHAR(100),[SOURCE_TABLE])),'NA')+'|'+");
                createStatement.AppendLine("                ISNULL(RTRIM(CONVERT(VARCHAR(100),[SOURCE_COLUMN])),'NA')+'|' +");
                createStatement.AppendLine("                ISNULL(RTRIM(CONVERT(VARCHAR(100),[TRANSFORMATION_RULE])),'NA')+'|'");
                createStatement.AppendLine("			),(2)");
                createStatement.AppendLine("			)");
                createStatement.AppendLine("		) PERSISTED NOT NULL,");
                createStatement.AppendLine("	[VERSION_ID]          integer NOT NULL,");
                createStatement.AppendLine("	[SOURCE_TABLE]        varchar(100)  NULL,");
                createStatement.AppendLine("	[SOURCE_COLUMN]       varchar(100)  NULL,");
                createStatement.AppendLine("	[TARGET_TABLE]        varchar(100)  NULL,");
                createStatement.AppendLine("	[TARGET_COLUMN]       varchar(100)  NULL,");
                createStatement.AppendLine("	[TRANSFORMATION_RULE] varchar(4000)  NULL,");
                createStatement.AppendLine("    CONSTRAINT[PK_MD_ATTRIBUTE_MAPPING] PRIMARY KEY CLUSTERED ([ATTRIBUTE_MAPPING_HASH] ASC, [VERSION_ID] ASC)");
                createStatement.AppendLine(")");

                RunSqlCommand(connOmdString, createStatement, worker, 10);
                createStatement.Clear();

                // Business Key Component
                createStatement.AppendLine();
                createStatement.AppendLine("-- Business Key Component");
                createStatement.AppendLine("IF OBJECT_ID('[MD_BUSINESS_KEY_COMPONENT]', 'U') IS NOT NULL");
                createStatement.AppendLine(" DROP TABLE [MD_BUSINESS_KEY_COMPONENT]");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE TABLE [MD_BUSINESS_KEY_COMPONENT]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("    [STAGING_AREA_TABLE_ID]        integer NOT NULL,");
                createStatement.AppendLine("	[HUB_TABLE_ID]      integer NOT NULL,");
                createStatement.AppendLine("	[BUSINESS_KEY_DEFINITION] [varchar](4000) NOT NULL,");
                createStatement.AppendLine("	[COMPONENT_ID]       integer NOT NULL,");
                createStatement.AppendLine("	[COMPONENT_ORDER]       integer NOT NULL,");
                createStatement.AppendLine("	[COMPONENT_VALUE]    varchar(100)  NOT NULL,");
                createStatement.AppendLine("    [COMPONENT_TYPE]     varchar(100)  NOT NULL,");
                createStatement.AppendLine("    CONSTRAINT[PK_MD_BUSINESS_KEY_COMPONENT] PRIMARY KEY CLUSTERED ([STAGING_AREA_TABLE_ID] ASC, [HUB_TABLE_ID] ASC, [BUSINESS_KEY_DEFINITION] ASC, [COMPONENT_ID] ASC)");
                createStatement.AppendLine(")");

                RunSqlCommand(connOmdString, createStatement, worker, 15);
                createStatement.Clear();

                // Business Key Component Part
                createStatement.AppendLine();
                createStatement.AppendLine("-- Business Key Component Part");
                createStatement.AppendLine("IF OBJECT_ID('[MD_BUSINESS_KEY_COMPONENT_PART]', 'U') IS NOT NULL");
                createStatement.AppendLine(" DROP TABLE [MD_BUSINESS_KEY_COMPONENT_PART]");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE TABLE [MD_BUSINESS_KEY_COMPONENT_PART]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("");
                createStatement.AppendLine("    [STAGING_AREA_TABLE_ID]  integer NOT NULL,");
                createStatement.AppendLine("	[HUB_TABLE_ID]       integer NOT NULL,");
                createStatement.AppendLine("	[COMPONENT_ID]      integer NOT NULL,");
                createStatement.AppendLine("	[BUSINESS_KEY_DEFINITION] [varchar](4000) NOT NULL, ");
                createStatement.AppendLine("	[COMPONENT_ELEMENT_ID]     integer NOT NULL,");
                createStatement.AppendLine("	[COMPONENT_ELEMENT_ORDER]      integer NULL,");
                createStatement.AppendLine("    [COMPONENT_ELEMENT_VALUE] varchar(1000)  NULL,");
                createStatement.AppendLine("	[COMPONENT_ELEMENT_TYPE] varchar(100)  NOT NULL,");
                createStatement.AppendLine("    [ATTRIBUTE_ID]       integer NULL,");
                createStatement.AppendLine("    CONSTRAINT [PK_MD_BUSINESS_KEY_COMPONENT_PART] PRIMARY KEY CLUSTERED ([STAGING_AREA_TABLE_ID] ASC, [HUB_TABLE_ID] ASC, [BUSINESS_KEY_DEFINITION] ASC, [COMPONENT_ID] ASC, [COMPONENT_ELEMENT_ID] ASC)");
                createStatement.AppendLine(")");

                try
                {
                    RunSqlCommand(connOmdString, createStatement, worker, 20);
                }
                catch (Exception ex)
                {
                    _alert.SetTextLogging("An issue has occured creating the Business Key Component Part. The full error message is: " + ex);
                }
                createStatement.Clear();

                // Driving Key Xref
                createStatement.AppendLine();
                createStatement.AppendLine("-- Driving Key Xref");
                createStatement.AppendLine("IF OBJECT_ID('[MD_DRIVING_KEY_XREF]', 'U') IS NOT NULL");
                createStatement.AppendLine(" DROP TABLE [MD_DRIVING_KEY_XREF]");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE TABLE [MD_DRIVING_KEY_XREF]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("    [SATELLITE_TABLE_ID]  integer NOT NULL,");
                createStatement.AppendLine("	[HUB_TABLE_ID]       integer NOT NULL,");
                createStatement.AppendLine("    CONSTRAINT [PK_MD_DRIVING_KEY_XREF] PRIMARY KEY CLUSTERED ([SATELLITE_TABLE_ID] ASC, [HUB_TABLE_ID] ASC)");
                createStatement.AppendLine(")");

                RunSqlCommand(connOmdString, createStatement, worker, 25);
                createStatement.Clear();

                // Hub
                createStatement.AppendLine();
                createStatement.AppendLine("-- Hub");
                createStatement.AppendLine("IF OBJECT_ID('[MD_HUB]', 'U') IS NOT NULL");
                createStatement.AppendLine(" DROP TABLE [MD_HUB]");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE TABLE [MD_HUB]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("    [HUB_TABLE_ID]        integer NOT NULL ,");
                createStatement.AppendLine("	[HUB_TABLE_NAME]     varchar(100)  NOT NULL,");
                createStatement.AppendLine("    CONSTRAINT [PK_MD_HUB] PRIMARY KEY CLUSTERED ([HUB_TABLE_ID] ASC)");
                createStatement.AppendLine(")");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE UNIQUE NONCLUSTERED INDEX [IX_MD_HUB] ON [MD_HUB]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("    [HUB_TABLE_NAME] ASC");
                createStatement.AppendLine(")");

                RunSqlCommand(connOmdString, createStatement, worker, 30);
                createStatement.Clear();

                // Hub Link Xref
                createStatement.AppendLine();
                createStatement.AppendLine("-- Hub Link XREF");
                createStatement.AppendLine("IF OBJECT_ID('[MD_HUB_LINK_XREF]', 'U') IS NOT NULL");
                createStatement.AppendLine(" DROP TABLE[MD_HUB_LINK_XREF]");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE TABLE[MD_HUB_LINK_XREF]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("    [HUB_TABLE_ID]       integer NOT NULL,");
                createStatement.AppendLine("	[LINK_TABLE_ID]      integer NOT NULL,");
                createStatement.AppendLine("    CONSTRAINT[PK_MD_HUB_LINK_XREF] PRIMARY KEY CLUSTERED ( [HUB_TABLE_ID] ASC, [LINK_TABLE_ID] ASC)");
                createStatement.AppendLine(")");

                RunSqlCommand(connOmdString, createStatement, worker, 35);
                createStatement.Clear();

                // Link
                createStatement.AppendLine();
                createStatement.AppendLine("-- Link");
                createStatement.AppendLine("IF OBJECT_ID('[MD_LINK]', 'U') IS NOT NULL");
                createStatement.AppendLine(" DROP TABLE [MD_LINK]");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE TABLE [MD_LINK]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("    [LINK_TABLE_ID]      integer NOT NULL,");
                createStatement.AppendLine("	[LINK_TABLE_NAME]    varchar(100)  NOT NULL,");
                createStatement.AppendLine("    CONSTRAINT [PK_MD_LINK] PRIMARY KEY CLUSTERED ( [LINK_TABLE_ID] ASC)");
                createStatement.AppendLine(")");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE UNIQUE NONCLUSTERED INDEX [IX_MD_LINK] ON [MD_LINK]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("    [LINK_TABLE_NAME] ASC");
                createStatement.AppendLine(")");

                RunSqlCommand(connOmdString, createStatement, worker, 40);
                createStatement.Clear();

                // Satellite
                createStatement.AppendLine();
                createStatement.AppendLine("-- Sat");
                createStatement.AppendLine("IF OBJECT_ID('[MD_SAT]', 'U') IS NOT NULL");
                createStatement.AppendLine(" DROP TABLE[MD_SAT]");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE TABLE[MD_SAT]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("    [SATELLITE_TABLE_ID]  integer NOT NULL ,");
                createStatement.AppendLine("	[SATELLITE_TABLE_NAME] varchar(100)  NOT NULL,");
                createStatement.AppendLine("    [SATELLITE_TYPE]     varchar(100)  NOT NULL,");
                createStatement.AppendLine("	[HUB_TABLE_ID]  integer NOT NULL,");
                createStatement.AppendLine("	[LINK_TABLE_ID] integer NOT NULL ,");
                createStatement.AppendLine("    CONSTRAINT [PK_MD_SAT] PRIMARY KEY CLUSTERED ([SATELLITE_TABLE_ID] ASC)");
                createStatement.AppendLine(")");

                RunSqlCommand(connOmdString, createStatement, worker, 41);
                createStatement.Clear();

                // Source CDC Xref
                createStatement.AppendLine();
                createStatement.AppendLine("-- Source / CDC Xref");
                createStatement.AppendLine("IF OBJECT_ID('[MD_SOURCE_CDC_TYPE_XREF]', 'U') IS NOT NULL");
                createStatement.AppendLine(" DROP TABLE [MD_SOURCE_CDC_TYPE_XREF]");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE TABLE [MD_SOURCE_CDC_TYPE_XREF]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("	[STAGING_AREA_TABLE_NAME] varchar(100)  NOT NULL,");
                createStatement.AppendLine("    [CHANGE_DATA_CAPTURE_TYPE]     varchar(100)  NOT NULL,");
                createStatement.AppendLine("    CONSTRAINT [PK_MD_SOURCE_CDC_TYPE_XREF] PRIMARY KEY CLUSTERED ( [CHANGE_DATA_CAPTURE_TYPE], [STAGING_AREA_TABLE_NAME] ASC)");
                createStatement.AppendLine(")");

                RunSqlCommand(connOmdString, createStatement, worker, 42);
                createStatement.Clear();

                // Source CDC Xref
                createStatement.AppendLine();
                createStatement.AppendLine("-- Source / Staging Xref");
                createStatement.AppendLine("IF OBJECT_ID('[MD_SOURCE_STG_XREF]', 'U') IS NOT NULL");
                createStatement.AppendLine(" DROP TABLE [MD_SOURCE_STG_XREF]");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE TABLE [MD_SOURCE_STG_XREF]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("	[STAGING_AREA_TABLE_NAME] varchar(100)  NOT NULL,");
                createStatement.AppendLine("    [SOURCE_TABLE_NAME]     varchar(100)  NOT NULL,");
                createStatement.AppendLine("    [SOURCE_TABLE_SCHEMA]     varchar(100)  NOT NULL,");
                createStatement.AppendLine("    CONSTRAINT [PK_MD_SOURCE_STG_XREF] PRIMARY KEY CLUSTERED ( [STAGING_AREA_TABLE_NAME] ASC, [SOURCE_TABLE_NAME])");
                createStatement.AppendLine(")");

                RunSqlCommand(connOmdString, createStatement, worker, 43);
                createStatement.Clear();

                // Staging
                createStatement.AppendLine();
                createStatement.AppendLine("-- Staging");
                createStatement.AppendLine("IF OBJECT_ID ('[MD_STG]', 'U') IS NOT NULL");
                createStatement.AppendLine(" DROP TABLE [MD_STG]");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE TABLE [MD_STG]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("    [STAGING_AREA_TABLE_ID] integer NOT NULL,");
                createStatement.AppendLine("	[STAGING_AREA_TABLE_NAME] varchar(100) NOT NULL,");
                createStatement.AppendLine("	[STAGING_AREA_SCHEMA_NAME] varchar(100) NULL,");
                createStatement.AppendLine("    CONSTRAINT[PK_MD_STG] PRIMARY KEY CLUSTERED ([STAGING_AREA_TABLE_ID] ASC)");
                createStatement.AppendLine(")");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE UNIQUE NONCLUSTERED INDEX [IX_MD_STG] ON [MD_STG]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("    [STAGING_AREA_TABLE_NAME]   ASC");
                createStatement.AppendLine(")");

                try
                {
                    RunSqlCommand(connOmdString, createStatement, worker, 44);
                }
                catch (Exception ex)
                {
                    _alert.SetTextLogging("An issue has occured creating the Staging metadata table. The full error message is: " + ex);
                }
                createStatement.Clear();

                // Staging Hub Xref
                createStatement.AppendLine();
                createStatement.AppendLine("-- Staging Hub XREF");
                createStatement.AppendLine("IF OBJECT_ID('[MD_STG_HUB_XREF]', 'U') IS NOT NULL");
                createStatement.AppendLine(" DROP TABLE[MD_STG_HUB_XREF]");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE TABLE[MD_STG_HUB_XREF]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("    [STAGING_AREA_TABLE_ID]  integer NOT NULL,");
                createStatement.AppendLine("	[HUB_TABLE_ID] integer NOT NULL,");
                createStatement.AppendLine("	[BUSINESS_KEY_DEFINITION] varchar(4000) NOT NULL,");
                createStatement.AppendLine("	[FILTER_CRITERIA]  varchar(4000)  NULL");
                createStatement.AppendLine("    CONSTRAINT[PK_MD_STG_HUB_XREF] PRIMARY KEY CLUSTERED([STAGING_AREA_TABLE_ID] ASC, [HUB_TABLE_ID] ASC, [BUSINESS_KEY_DEFINITION] ASC)");
                createStatement.AppendLine(")");

                RunSqlCommand(connOmdString, createStatement, worker, 45);
                createStatement.Clear();

                // Staging Link Attribute Xref
                createStatement.AppendLine();
                createStatement.AppendLine("-- Staging Link Attribute XREF");
                createStatement.AppendLine("IF OBJECT_ID('[MD_STG_LINK_ATT_XREF]', 'U') IS NOT NULL");
                createStatement.AppendLine(" DROP TABLE[MD_STG_LINK_ATT_XREF]");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE TABLE[MD_STG_LINK_ATT_XREF]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("    [STAGING_AREA_TABLE_ID] integer NOT NULL,");
                createStatement.AppendLine("	[LINK_TABLE_ID] integer NOT NULL,");
                createStatement.AppendLine("	[ATTRIBUTE_ID_FROM] integer NOT NULL,");
                createStatement.AppendLine("	[ATTRIBUTE_ID_TO] integer NOT NULL,");
                createStatement.AppendLine("    CONSTRAINT[PK_MD_STG_LINK_ATT_XREF] PRIMARY KEY CLUSTERED([STAGING_AREA_TABLE_ID] ASC, [LINK_TABLE_ID] ASC, [ATTRIBUTE_ID_FROM] ASC, [ATTRIBUTE_ID_TO] ASC)");
                createStatement.AppendLine(")");

                RunSqlCommand(connOmdString, createStatement, worker, 46);
                createStatement.Clear();

                // Staging Link  Xref
                createStatement.AppendLine();
                createStatement.AppendLine("-- Staging Link  XREF");
                createStatement.AppendLine("IF OBJECT_ID('[MD_STG_LINK_XREF]', 'U') IS NOT NULL");
                createStatement.AppendLine(" DROP TABLE[MD_STG_LINK_XREF]");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE TABLE[MD_STG_LINK_XREF]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("    [STAGING_AREA_TABLE_ID] integer NOT NULL,");
                createStatement.AppendLine("	[LINK_TABLE_ID] integer NOT NULL,");
                createStatement.AppendLine("	[FILTER_CRITERIA] varchar(4000) NULL,");
                createStatement.AppendLine("    CONSTRAINT[PK_MD_STG_LINK_XREF] PRIMARY KEY CLUSTERED([STAGING_AREA_TABLE_ID] ASC, [LINK_TABLE_ID] ASC)");
                createStatement.AppendLine(")");

                RunSqlCommand(connOmdString, createStatement, worker, 47);
                createStatement.Clear();

                // Staging / Satellite Attribute Xref
                createStatement.AppendLine();
                createStatement.AppendLine("-- Staging Attribute XREF");
                createStatement.AppendLine("IF OBJECT_ID('[MD_STG_SAT_ATT_XREF]', 'U') IS NOT NULL");
                createStatement.AppendLine(" DROP TABLE[MD_STG_SAT_ATT_XREF]");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE TABLE[MD_STG_SAT_ATT_XREF]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("    [STAGING_AREA_TABLE_ID] integer NOT NULL,");
                createStatement.AppendLine("	[SATELLITE_TABLE_ID] integer NOT NULL,");
                createStatement.AppendLine("	[ATTRIBUTE_ID_FROM]  integer NOT NULL,");
                createStatement.AppendLine("	[ATTRIBUTE_ID_TO] integer NOT NULL,");
                createStatement.AppendLine("	[MULTI_ACTIVE_KEY_INDICATOR] varchar(100)  NOT NULL,");
                createStatement.AppendLine("    CONSTRAINT[PK_MD_STG_SAT_ATT_XREF] PRIMARY KEY CLUSTERED([STAGING_AREA_TABLE_ID] ASC, [SATELLITE_TABLE_ID] ASC, [ATTRIBUTE_ID_FROM] ASC, [ATTRIBUTE_ID_TO] ASC)");
                createStatement.AppendLine(")");

                RunSqlCommand(connOmdString, createStatement, worker, 48);
                createStatement.Clear();

                // Staging / Satellite  Xref
                createStatement.AppendLine();
                createStatement.AppendLine("-- Staging Satellite XREF");
                createStatement.AppendLine("IF OBJECT_ID('[MD_STG_SAT_XREF]', 'U') IS NOT NULL");
                createStatement.AppendLine(" DROP TABLE[MD_STG_SAT_XREF]");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE TABLE[MD_STG_SAT_XREF]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("	[SATELLITE_TABLE_ID] integer NOT NULL,");
                createStatement.AppendLine("    [STAGING_AREA_TABLE_ID] integer NOT NULL,");
                createStatement.AppendLine("    [BUSINESS_KEY_DEFINITION] [varchar](1000) NOT NULL,");
                createStatement.AppendLine("	[FILTER_CRITERIA] varchar(4000)  NULL,");
                createStatement.AppendLine("    CONSTRAINT[PK_MD_STG_SAT_XREF] PRIMARY KEY CLUSTERED([SATELLITE_TABLE_ID] ASC, [STAGING_AREA_TABLE_ID] ASC)");
                createStatement.AppendLine(")");

                RunSqlCommand(connOmdString, createStatement, worker, 50);
                createStatement.Clear();

                // Table Mapping
                createStatement.AppendLine();
                createStatement.AppendLine("-- Table Mapping");
                createStatement.AppendLine("IF OBJECT_ID('[MD_TABLE_MAPPING]', 'U') IS NOT NULL");
                createStatement.AppendLine(" DROP TABLE[MD_TABLE_MAPPING]");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE TABLE[MD_TABLE_MAPPING]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("    [TABLE_MAPPING_HASH] AS(");
                createStatement.AppendLine("                CONVERT([CHAR](32),HASHBYTES('MD5',");
                createStatement.AppendLine("                ISNULL(RTRIM(CONVERT(VARCHAR(100),[INTEGRATION_AREA_TABLE])),'NA')+'|'+");
                createStatement.AppendLine("                ISNULL(RTRIM(CONVERT(VARCHAR(100),[STAGING_AREA_TABLE])),'NA')+'|'+");
                createStatement.AppendLine("                ISNULL(RTRIM(CONVERT(VARCHAR(100),[BUSINESS_KEY_ATTRIBUTE])),'NA')+'|'+");
                createStatement.AppendLine("                ISNULL(RTRIM(CONVERT(VARCHAR(100),[DRIVING_KEY_ATTRIBUTE])),'NA')+'|'+");
                createStatement.AppendLine("                ISNULL(RTRIM(CONVERT(VARCHAR(100),[FILTER_CRITERIA])),'NA')+'|'");
                createStatement.AppendLine("			),(2)");
                createStatement.AppendLine("			)");
                createStatement.AppendLine("		) PERSISTED NOT NULL ,");
                createStatement.AppendLine("	[VERSION_ID] integer NOT NULL ,");
                createStatement.AppendLine("	[STAGING_AREA_TABLE] varchar(100)  NULL,");
                createStatement.AppendLine("	[BUSINESS_KEY_ATTRIBUTE] varchar(4000)  NULL,");
                createStatement.AppendLine("	[DRIVING_KEY_ATTRIBUTE] varchar(4000)  NULL,");
                createStatement.AppendLine("	[INTEGRATION_AREA_TABLE] varchar(100)  NULL,");
                createStatement.AppendLine("	[FILTER_CRITERIA]    varchar(4000)  NULL,");
                createStatement.AppendLine("    CONSTRAINT[PK_MD_TABLE_MAPPING] PRIMARY KEY CLUSTERED([TABLE_MAPPING_HASH] ASC, [VERSION_ID] ASC)");
                createStatement.AppendLine(")");

                RunSqlCommand(connOmdString, createStatement, worker, 55);
                createStatement.Clear();

                // Version
                createStatement.AppendLine();
                createStatement.AppendLine("-- Version");
                createStatement.AppendLine("IF OBJECT_ID('[MD_VERSION]', 'U') IS NOT NULL");
                createStatement.AppendLine(" DROP TABLE[MD_VERSION]");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE TABLE[MD_VERSION]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("    [VERSION_ID] integer NOT NULL IDENTITY( 1,1 ) ,");
                createStatement.AppendLine("	[VERSION_NAME] varchar(100)  NOT NULL,");
                createStatement.AppendLine("");
                createStatement.AppendLine("    [VERSION_NOTES]      varchar(1000)  NULL ,");
                createStatement.AppendLine("	[MAJOR_RELEASE_NUMBER] integer NULL,");
                createStatement.AppendLine("    [MINOR_RELEASE_NUMBER] integer NULL ");
                createStatement.AppendLine(")");
                createStatement.AppendLine("");
                createStatement.AppendLine("ALTER TABLE [MD_VERSION]");
                createStatement.AppendLine("    ADD CONSTRAINT[PK_MD_VERSION] PRIMARY KEY CLUSTERED([VERSION_ID] ASC)");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE UNIQUE NONCLUSTERED INDEX[IX_MD_VERSION] ON[MD_VERSION]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("");
                createStatement.AppendLine("    [MAJOR_RELEASE_NUMBER] ASC,");
                createStatement.AppendLine("	[MINOR_RELEASE_NUMBER] ASC");
                createStatement.AppendLine(")");

                RunSqlCommand(connOmdString, createStatement, worker, 58);
                createStatement.Clear();

                // Version Attribute
                createStatement.AppendLine();
                createStatement.AppendLine("-- Version Attribute");
                createStatement.AppendLine("IF OBJECT_ID('[MD_VERSION_ATTRIBUTE]', 'U') IS NOT NULL");
                createStatement.AppendLine(" DROP TABLE[MD_VERSION_ATTRIBUTE]");
                createStatement.AppendLine("");
                createStatement.AppendLine("CREATE TABLE[MD_VERSION_ATTRIBUTE]");
                createStatement.AppendLine("( ");
                createStatement.AppendLine("");
                createStatement.AppendLine("    [VERSION_ATTRIBUTE_HASH] AS(");
                createStatement.AppendLine("                CONVERT([CHAR](32),HASHBYTES('MD5',");
                createStatement.AppendLine("                ISNULL(RTRIM(CONVERT(VARCHAR(100),[TABLE_NAME])),'NA')+'|'+");
                createStatement.AppendLine("                ISNULL(RTRIM(CONVERT(VARCHAR(100),[COLUMN_NAME])),'NA')+'|'+");
                createStatement.AppendLine("                ISNULL(RTRIM(CONVERT(VARCHAR(100),[VERSION_ID])),'NA')+'|'");
                createStatement.AppendLine("			),(2)");
                createStatement.AppendLine("			)");
                createStatement.AppendLine("		) PERSISTED NOT NULL ,");
                createStatement.AppendLine("	[VERSION_ID] integer NOT NULL ,");
                createStatement.AppendLine("	[TABLE_NAME]         varchar(100)  NULL ,");
                createStatement.AppendLine("	[COLUMN_NAME]        varchar(100)  NOT NULL,");
                createStatement.AppendLine("    [DATA_TYPE]          varchar(100)  NULL ,");
                createStatement.AppendLine("	[CHARACTER_MAXIMUM_LENGTH] integer NULL,");
                createStatement.AppendLine("    [NUMERIC_PRECISION]  integer NULL,");
                createStatement.AppendLine("    [ORDINAL_POSITION]   integer NULL,");
                createStatement.AppendLine("    [PRIMARY_KEY_INDICATOR] varchar(1)  NULL ,");
                createStatement.AppendLine("	[DRIVING_KEY_INDICATOR] varchar(1)  NULL ,");
                createStatement.AppendLine("	[MULTI_ACTIVE_INDICATOR] varchar(1)  NULL ");
                createStatement.AppendLine(")");
                createStatement.AppendLine("");
                createStatement.AppendLine("ALTER TABLE[MD_VERSION_ATTRIBUTE]");
                createStatement.AppendLine("    ADD CONSTRAINT[PK_MD_VERSION_ATTRIBUTE] PRIMARY KEY CLUSTERED([VERSION_ATTRIBUTE_HASH] ASC, [VERSION_ID] ASC)");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 59);
                createStatement.Clear();

                // Create existing Foreign Key Constraints
                createStatement.AppendLine("ALTER TABLE [MD_BUSINESS_KEY_COMPONENT] WITH CHECK ADD CONSTRAINT [FK_MD_BUSINESS_KEY_COMPONENT_MD_STG_HUB_XREF] FOREIGN KEY([STAGING_AREA_TABLE_ID], [HUB_TABLE_ID], [BUSINESS_KEY_DEFINITION])");
                createStatement.AppendLine("REFERENCES  [MD_STG_HUB_XREF] ([STAGING_AREA_TABLE_ID], [HUB_TABLE_ID], [BUSINESS_KEY_DEFINITION])");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 60);
                createStatement.Clear();

                createStatement.AppendLine("ALTER TABLE [MD_BUSINESS_KEY_COMPONENT_PART] WITH CHECK ADD CONSTRAINT [FK_MD_BUSINESS_KEY_COMPONENT_PART_MD_ATT] FOREIGN KEY([ATTRIBUTE_ID])");
                createStatement.AppendLine("REFERENCES  [MD_ATT] ([ATTRIBUTE_ID])");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 61);
                createStatement.Clear();

                createStatement.AppendLine("ALTER TABLE [MD_BUSINESS_KEY_COMPONENT_PART] WITH CHECK ADD CONSTRAINT [FK_MD_BUSINESS_KEY_COMPONENT_PART_MD_BUSINESS_KEY_COMPONENT] FOREIGN KEY([STAGING_AREA_TABLE_ID], [HUB_TABLE_ID], [BUSINESS_KEY_DEFINITION], [COMPONENT_ID])");
                createStatement.AppendLine("REFERENCES  [MD_BUSINESS_KEY_COMPONENT]([STAGING_AREA_TABLE_ID], [HUB_TABLE_ID], [BUSINESS_KEY_DEFINITION], [COMPONENT_ID])");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 62);
                createStatement.Clear();

                createStatement.AppendLine("ALTER TABLE [MD_DRIVING_KEY_XREF]  WITH CHECK ADD  CONSTRAINT [FK_MD_DRIVING_KEY_XREF_MD_HUB] FOREIGN KEY([HUB_TABLE_ID])");
                createStatement.AppendLine("REFERENCES  [MD_HUB] ([HUB_TABLE_ID])");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 63);
                createStatement.Clear();

                createStatement.AppendLine("ALTER TABLE [MD_DRIVING_KEY_XREF]  WITH CHECK ADD  CONSTRAINT [FK_MD_DRIVING_KEY_XREF_MD_SAT] FOREIGN KEY([SATELLITE_TABLE_ID])");
                createStatement.AppendLine("REFERENCES  [MD_SAT] ([SATELLITE_TABLE_ID])");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 64);
                createStatement.Clear();

                createStatement.AppendLine("ALTER TABLE [MD_HUB_LINK_XREF]  WITH CHECK ADD  CONSTRAINT [FK_MD_HUB_LINK_XREF_MD_HUB] FOREIGN KEY([HUB_TABLE_ID])");
                createStatement.AppendLine("REFERENCES  [MD_HUB] ([HUB_TABLE_ID])");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 65);
                createStatement.Clear();

                createStatement.AppendLine("ALTER TABLE [MD_HUB_LINK_XREF]  WITH CHECK ADD  CONSTRAINT [FK_MD_HUB_LINK_XREF_MD_LINK] FOREIGN KEY([LINK_TABLE_ID])");
                createStatement.AppendLine("REFERENCES  [MD_LINK] ([LINK_TABLE_ID])");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 66);
                createStatement.Clear();

                createStatement.AppendLine("ALTER TABLE [MD_SAT]  WITH CHECK ADD  CONSTRAINT [FK_MD_SAT_MD_HUB] FOREIGN KEY([HUB_TABLE_ID])");
                createStatement.AppendLine("REFERENCES  [MD_HUB] ([HUB_TABLE_ID])");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 67);
                createStatement.Clear();

                createStatement.AppendLine("ALTER TABLE [MD_SAT]  WITH CHECK ADD  CONSTRAINT [FK_MD_SAT_MD_LINK] FOREIGN KEY([LINK_TABLE_ID])");
                createStatement.AppendLine("REFERENCES  [MD_LINK] ([LINK_TABLE_ID])");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 68);
                createStatement.Clear();

                createStatement.AppendLine("ALTER TABLE [MD_STG_HUB_XREF]  WITH CHECK ADD  CONSTRAINT [FK_MD_STG_HUB_XREF_MD_HUB] FOREIGN KEY([HUB_TABLE_ID])"); 
                createStatement.AppendLine("REFERENCES  [MD_HUB] ([HUB_TABLE_ID])");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 69);
                createStatement.Clear();

                createStatement.AppendLine("ALTER TABLE [MD_STG_HUB_XREF]  WITH CHECK ADD  CONSTRAINT [FK_MD_STG_HUB_XREF_MD_STG] FOREIGN KEY([STAGING_AREA_TABLE_ID])");
                createStatement.AppendLine("REFERENCES  [MD_STG] ([STAGING_AREA_TABLE_ID])");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 70);
                createStatement.Clear();

                createStatement.AppendLine("ALTER TABLE [MD_STG_LINK_ATT_XREF]  WITH CHECK ADD  CONSTRAINT [FK_MD_STG_LINK_ATT_XREF_MD_ATT_FROM] FOREIGN KEY([ATTRIBUTE_ID_FROM])");
                createStatement.AppendLine("REFERENCES  [MD_ATT] ([ATTRIBUTE_ID])");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 71);
                createStatement.Clear();

                createStatement.AppendLine("ALTER TABLE [MD_STG_LINK_ATT_XREF]  WITH CHECK ADD  CONSTRAINT [FK_MD_STG_LINK_ATT_XREF_MD_ATT_TO] FOREIGN KEY([ATTRIBUTE_ID_TO])");
                createStatement.AppendLine("REFERENCES  [MD_ATT] ([ATTRIBUTE_ID])");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 71);
                createStatement.Clear();

                createStatement.AppendLine("ALTER TABLE [MD_STG_LINK_ATT_XREF]  WITH CHECK ADD  CONSTRAINT [FK_MD_STG_LINK_ATT_XREF_MD_LINK] FOREIGN KEY([LINK_TABLE_ID])");
                createStatement.AppendLine("REFERENCES  [MD_LINK] ([LINK_TABLE_ID])");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 72);
                createStatement.Clear();

                createStatement.AppendLine("ALTER TABLE [MD_STG_LINK_ATT_XREF]  WITH CHECK ADD  CONSTRAINT [FK_MD_STG_LINK_ATT_XREF_MD_STG] FOREIGN KEY([STAGING_AREA_TABLE_ID])");
                createStatement.AppendLine("REFERENCES  [MD_STG] ([STAGING_AREA_TABLE_ID])");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 73);
                createStatement.Clear();

                createStatement.AppendLine("ALTER TABLE [MD_STG_LINK_XREF]  WITH CHECK ADD  CONSTRAINT [FK_MD_STG_LINK_XREF_MD_LINK] FOREIGN KEY([LINK_TABLE_ID])");
                createStatement.AppendLine("REFERENCES  [MD_LINK] ([LINK_TABLE_ID])");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 74);
                createStatement.Clear();

                createStatement.AppendLine("ALTER TABLE [MD_STG_LINK_XREF]  WITH CHECK ADD  CONSTRAINT [FK_MD_STG_LINK_XREF_MD_STG] FOREIGN KEY([STAGING_AREA_TABLE_ID])");
                createStatement.AppendLine("REFERENCES  [MD_STG] ([STAGING_AREA_TABLE_ID])");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 75);
                createStatement.Clear();

                createStatement.AppendLine("ALTER TABLE [MD_STG_SAT_ATT_XREF]  WITH CHECK ADD  CONSTRAINT [FK_MD_STG_SAT_ATT_XREF_MD_ATTRIBUTE_FROM] FOREIGN KEY([ATTRIBUTE_ID_FROM])");
                createStatement.AppendLine("REFERENCES  [MD_ATT] ([ATTRIBUTE_ID])");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 76);
                createStatement.Clear();

                createStatement.AppendLine("ALTER TABLE [MD_STG_SAT_ATT_XREF]  WITH CHECK ADD  CONSTRAINT [FK_MD_STG_SAT_ATT_XREF_MD_ATTRIBUTE_TO] FOREIGN KEY([ATTRIBUTE_ID_TO])");
                createStatement.AppendLine("REFERENCES  [MD_ATT] ([ATTRIBUTE_ID])");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 77);
                createStatement.Clear();

                createStatement.AppendLine("ALTER TABLE [MD_STG_SAT_ATT_XREF]  WITH CHECK ADD  CONSTRAINT [FK_MD_STG_SAT_ATT_XREF_MD_SAT] FOREIGN KEY([SATELLITE_TABLE_ID])");
                createStatement.AppendLine("REFERENCES  [MD_SAT] ([SATELLITE_TABLE_ID])");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 78);
                createStatement.Clear();

                createStatement.AppendLine("ALTER TABLE [MD_STG_SAT_ATT_XREF]  WITH CHECK ADD  CONSTRAINT [FK_MD_STG_SAT_ATT_XREF_MD_STG] FOREIGN KEY([STAGING_AREA_TABLE_ID])");
                createStatement.AppendLine("REFERENCES  [MD_STG] ([STAGING_AREA_TABLE_ID])");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 78);
                createStatement.Clear();
                
                createStatement.AppendLine("ALTER TABLE [MD_STG_SAT_XREF]  WITH CHECK ADD  CONSTRAINT [FK_MD_STG_SAT_XREF_MD_SAT] FOREIGN KEY([SATELLITE_TABLE_ID])");
                createStatement.AppendLine("REFERENCES  [MD_SAT] ([SATELLITE_TABLE_ID])");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 79);
                createStatement.Clear();

                createStatement.AppendLine("ALTER TABLE [MD_STG_SAT_XREF]  WITH CHECK ADD  CONSTRAINT [FK_MD_STG_SAT_XREF_MD_STG] FOREIGN KEY([STAGING_AREA_TABLE_ID])");
                createStatement.AppendLine("REFERENCES  [MD_STG] ([STAGING_AREA_TABLE_ID])");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 79);
                createStatement.Clear();

                // Drop the views

                createStatement.AppendLine("-- INTERFACE_BUSINESS_KEY_COMPONENT");
                createStatement.AppendLine("IF OBJECT_ID('[interface].[INTERFACE_BUSINESS_KEY_COMPONENT]', 'V') IS NOT NULL");
                createStatement.AppendLine(" DROP VIEW [interface].[INTERFACE_BUSINESS_KEY_COMPONENT]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 78);
                createStatement.Clear();

                createStatement.AppendLine("-- INTERFACE_BUSINESS_KEY_COMPONENT_PART");
                createStatement.AppendLine("IF OBJECT_ID('[interface].[INTERFACE_BUSINESS_KEY_COMPONENT_PART]', 'V') IS NOT NULL");
                createStatement.AppendLine(" DROP VIEW [interface].[INTERFACE_BUSINESS_KEY_COMPONENT_PART]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 78);
                createStatement.Clear();

                createStatement.AppendLine("-- INTERFACE_STAGING_SATELLITE_ATTRIBUTE_XREF");
                createStatement.AppendLine("IF OBJECT_ID('[interface].[INTERFACE_STAGING_SATELLITE_ATTRIBUTE_XREF]', 'V') IS NOT NULL");
                createStatement.AppendLine(" DROP VIEW [interface].[INTERFACE_STAGING_SATELLITE_ATTRIBUTE_XREF]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 79);
                createStatement.Clear();

                createStatement.AppendLine("-- INTERFACE_HUB_LINK_XREF");
                createStatement.AppendLine("IF OBJECT_ID('[interface].[INTERFACE_HUB_LINK_XREF]', 'V') IS NOT NULL");
                createStatement.AppendLine(" DROP VIEW [interface].[INTERFACE_HUB_LINK_XREF]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 79);
                createStatement.Clear();

                createStatement.AppendLine("-- INTERFACE_DRIVING_KEY");
                createStatement.AppendLine("IF OBJECT_ID('[interface].[INTERFACE_DRIVING_KEY]', 'V') IS NOT NULL");
                createStatement.AppendLine(" DROP VIEW [interface].[INTERFACE_DRIVING_KEY]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 80);
                createStatement.Clear();

                createStatement.AppendLine("-- INTERFACE_SOURCE_TO_STAGING");
                createStatement.AppendLine("IF OBJECT_ID('[interface].[INTERFACE_SOURCE_TO_STAGING]', 'V') IS NOT NULL");
                createStatement.AppendLine(" DROP VIEW [interface].[INTERFACE_SOURCE_TO_STAGING]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 80);
                createStatement.Clear();

                createStatement.AppendLine("-- INTERFACE_STAGING_SATELLITE_XREF");
                createStatement.AppendLine("IF OBJECT_ID('[interface].[INTERFACE_STAGING_SATELLITE_XREF]', 'V') IS NOT NULL");
                createStatement.AppendLine(" DROP VIEW [interface].[INTERFACE_STAGING_SATELLITE_XREF]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 81);
                createStatement.Clear();

                createStatement.AppendLine("-- INTERFACE_STAGING_HUB_XREF");
                createStatement.AppendLine("IF OBJECT_ID('[interface].[INTERFACE_STAGING_HUB_XREF]', 'V') IS NOT NULL");
                createStatement.AppendLine(" DROP VIEW [interface].[INTERFACE_STAGING_HUB_XREF]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 81);
                createStatement.Clear();

                createStatement.AppendLine("-- INTERFACE_STAGING_LINK_XREF");
                createStatement.AppendLine("IF OBJECT_ID('[interface].[INTERFACE_STAGING_LINK_XREF]', 'V') IS NOT NULL");
                createStatement.AppendLine(" DROP VIEW [interface].[INTERFACE_STAGING_LINK_XREF]");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 82);
                createStatement.Clear();

                // Create the schemas

                createStatement.AppendLine("-- Creating the scema");
                createStatement.AppendLine("IF EXISTS ( SELECT schema_name FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'interface')");
                createStatement.AppendLine("DROP SCHEMA [interface]");
                RunSqlCommand(connOmdString, createStatement, worker, 82);
                createStatement.Clear();

                createStatement.AppendLine("CREATE SCHEMA [interface]");
                RunSqlCommand(connOmdString, createStatement, worker, 83);
                createStatement.Clear();


                // Create the views

                createStatement.AppendLine("CREATE VIEW [interface].[INTERFACE_BUSINESS_KEY_COMPONENT]");
                createStatement.AppendLine("AS");
                createStatement.AppendLine("SELECT");
                createStatement.AppendLine(" xref.STAGING_AREA_TABLE_ID,");
                createStatement.AppendLine(" STAGING_AREA_TABLE_NAME,");
                createStatement.AppendLine(" STAGING_AREA_SCHEMA_NAME,");
                createStatement.AppendLine(" xref.HUB_TABLE_ID,");
                createStatement.AppendLine(" HUB_TABLE_NAME,");
                createStatement.AppendLine(" BUSINESS_KEY_DEFINITION,");
                createStatement.AppendLine(" COMPONENT_ID AS BUSINESS_KEY_COMPONENT_ID,");
                createStatement.AppendLine(" COMPONENT_ORDER AS BUSINESS_KEY_COMPONENT_ORDER,");
                createStatement.AppendLine(" COMPONENT_VALUE AS BUSINESS_KEY_COMPONENT_VALUE");
                createStatement.AppendLine("FROM MD_BUSINESS_KEY_COMPONENT xref");
                createStatement.AppendLine("JOIN MD_STG stg ON xref.STAGING_AREA_TABLE_ID = stg.STAGING_AREA_TABLE_ID");
                createStatement.AppendLine("JOIN MD_HUB hub ON xref.HUB_TABLE_ID = hub.HUB_TABLE_ID");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 85);
                createStatement.Clear();
                
                createStatement.AppendLine("CREATE VIEW [interface].[INTERFACE_BUSINESS_KEY_COMPONENT_PART]");
                createStatement.AppendLine("AS");
                createStatement.AppendLine("SELECT");
                createStatement.AppendLine("  comp.STAGING_AREA_TABLE_ID, ");
                createStatement.AppendLine("  stg.STAGING_AREA_TABLE_NAME,");
                createStatement.AppendLine("  stg.STAGING_AREA_SCHEMA_NAME,");
                createStatement.AppendLine("  comp.HUB_TABLE_ID, ");
                createStatement.AppendLine("  hub.HUB_TABLE_NAME,");
                createStatement.AppendLine("  comp.BUSINESS_KEY_DEFINITION,");
                createStatement.AppendLine("  comp.COMPONENT_ID AS BUSINESS_KEY_COMPONENT_ID, ");
                createStatement.AppendLine("  comp.COMPONENT_ORDER AS BUSINESS_KEY_COMPONENT_ORDER,");
                createStatement.AppendLine("  elem.COMPONENT_ELEMENT_ID AS BUSINESS_KEY_COMPONENT_ELEMENT_ID, ");
                createStatement.AppendLine("  elem.COMPONENT_ELEMENT_ORDER AS BUSINESS_KEY_COMPONENT_ELEMENT_ORDER,");
                createStatement.AppendLine("  elem.COMPONENT_ELEMENT_VALUE AS BUSINESS_KEY_COMPONENT_ELEMENT_VALUE,");
                createStatement.AppendLine("  elem.COMPONENT_ELEMENT_TYPE AS BUSINESS_KEY_COMPONENT_ELEMENT_TYPE,");
                createStatement.AppendLine("  elem.ATTRIBUTE_ID AS BUSINESS_KEY_COMPONENT_ELEMENT_ATTRIBUTE_ID,");
                createStatement.AppendLine("  COALESCE(att.ATTRIBUTE_NAME, 'Not applicable') AS BUSINESS_KEY_COMPONENT_ELEMENT_ATTRIBUTE_NAME");
                createStatement.AppendLine("FROM MD_BUSINESS_KEY_COMPONENT comp");
                createStatement.AppendLine("JOIN MD_BUSINESS_KEY_COMPONENT_PART elem");
                createStatement.AppendLine("  ON comp.STAGING_AREA_TABLE_ID = elem.STAGING_AREA_TABLE_ID");
                createStatement.AppendLine(" AND comp.HUB_TABLE_ID = elem.HUB_TABLE_ID");
                createStatement.AppendLine(" AND comp.BUSINESS_KEY_DEFINITION = elem.BUSINESS_KEY_DEFINITION");
                createStatement.AppendLine(" AND comp.COMPONENT_ID = elem.COMPONENT_ID");
                createStatement.AppendLine("JOIN MD_STG stg ON comp.STAGING_AREA_TABLE_ID = stg.STAGING_AREA_TABLE_ID");
                createStatement.AppendLine("JOIN MD_HUB hub ON comp.HUB_TABLE_ID = hub.HUB_TABLE_ID");
                createStatement.AppendLine("LEFT JOIN MD_ATT att ON elem.ATTRIBUTE_ID = att.ATTRIBUTE_ID");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 87);
                createStatement.Clear();

                createStatement.AppendLine("CREATE VIEW[interface].[INTERFACE_DRIVING_KEY]");
                createStatement.AppendLine("AS");
                createStatement.AppendLine("SELECT");
                createStatement.AppendLine("  MD.SATELLITE_TABLE_ID, ");
                createStatement.AppendLine("  sat.SATELLITE_TABLE_NAME,");
                createStatement.AppendLine("  MD.HUB_TABLE_ID,");
                createStatement.AppendLine("  hub.HUB_TABLE_NAME");
                createStatement.AppendLine("FROM MD_DRIVING_KEY_XREF MD");
                createStatement.AppendLine("LEFT OUTER JOIN dbo.MD_SAT sat ON MD.SATELLITE_TABLE_ID = sat.SATELLITE_TABLE_ID");
                createStatement.AppendLine("LEFT OUTER JOIN dbo.MD_HUB hub ON MD.HUB_TABLE_ID = hub.HUB_TABLE_ID");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 89);
                createStatement.Clear();

                createStatement.AppendLine("CREATE VIEW[interface].[INTERFACE_HUB_LINK_XREF]");
                createStatement.AppendLine("AS");
                createStatement.AppendLine("SELECT");
                createStatement.AppendLine(" slxref.LINK_TABLE_ID,");
                createStatement.AppendLine(" lnk.LINK_TABLE_NAME,");
                createStatement.AppendLine(" slxref.STAGING_AREA_TABLE_ID,");
                createStatement.AppendLine(" stg.STAGING_AREA_TABLE_NAME,");
                createStatement.AppendLine(" stg.STAGING_AREA_SCHEMA_NAME,");
                createStatement.AppendLine(" hub.HUB_TABLE_ID,");
                createStatement.AppendLine(" hub.HUB_TABLE_NAME,");
                createStatement.AppendLine(" shxref.BUSINESS_KEY_DEFINITION --The Business Key Definition specifically for this Staging / Hub combination(shared by the Link)");
                createStatement.AppendLine("FROM MD_STG_LINK_XREF slxref");
                createStatement.AppendLine("JOIN MD_HUB_LINK_XREF hlxref ON slxref.LINK_TABLE_ID = hlxref.LINK_TABLE_ID");
                createStatement.AppendLine("JOIN MD_HUB hub ON hlxref.HUB_TABLE_ID = hub.HUB_TABLE_ID");
                createStatement.AppendLine("JOIN MD_STG stg ON slxref.STAGING_AREA_TABLE_ID = stg.STAGING_AREA_TABLE_ID");
                createStatement.AppendLine("JOIN MD_LINK lnk ON slxref.LINK_TABLE_ID = lnk.LINK_TABLE_ID");
                createStatement.AppendLine("JOIN MD_STG_HUB_XREF shxref");
                createStatement.AppendLine("    ON slxref.STAGING_AREA_TABLE_ID = shxref.STAGING_AREA_TABLE_ID");
                createStatement.AppendLine("    AND hub.HUB_TABLE_ID = shxref.HUB_TABLE_ID");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 91);
                createStatement.Clear();

                createStatement.AppendLine("CREATE view[interface].[INTERFACE_SOURCE_TO_STAGING]");
                createStatement.AppendLine("AS");
                createStatement.AppendLine("/*");
                createStatement.AppendLine("This view combines the staging area listing / interfaces with the exceptions where a single source file/table is mapped to more than one Staging Area tables.");
                createStatement.AppendLine("This is the default source for source-to-staging interfaces.");
                createStatement.AppendLine("*/");
                createStatement.AppendLine("");
                createStatement.AppendLine("select");
                createStatement.AppendLine("      schema_listing.TABLE_NAME AS STAGING_AREA_TABLE_NAME -- the Staging Area tables queried from the catalog");
                createStatement.AppendLine("                , '[dbo]' AS STAGING_AREA_SCHEMA_NAME");
                createStatement.AppendLine("                , coalesce(naming_exception.SOURCE_TABLE_NAME");
                createStatement.AppendLine("                , substring(schema_listing.TABLE_NAME");
                createStatement.AppendLine("                , charindex(N'_', schema_listing.TABLE_NAME, 5) + 1,len(schema_listing.TABLE_NAME))) as SOURCE_TABLE_NAME");
                createStatement.AppendLine("                , substring(schema_listing.TABLE_NAME , 5 , charindex(N'_', schema_listing.TABLE_NAME, 5) - 5) as SOURCE_TABLE_SYSTEM_NAME");
                createStatement.AppendLine("                ,'tbd' AS SOURCE_SCHEMA_NAME");
                createStatement.AppendLine("                , COALESCE(cdctype.CHANGE_DATA_CAPTURE_TYPE, 'Undefined') AS CHANGE_DATA_CAPTURE_TYPE");
                createStatement.AppendLine("FROM "+_myParent.textBoxStagingDatabase.Text+".INFORMATION_SCHEMA.TABLES as schema_listing");
                createStatement.AppendLine("                left join dbo.MD_SOURCE_STG_XREF as naming_exception");
                createStatement.AppendLine("                                                on naming_exception.STAGING_AREA_TABLE_NAME = schema_listing.TABLE_NAME");
                createStatement.AppendLine("                left join dbo.MD_SOURCE_CDC_TYPE_XREF as cdctype");
                createStatement.AppendLine("                       on schema_listing.TABLE_NAME = cdctype.STAGING_AREA_TABLE_NAME");
                createStatement.AppendLine("where TABLE_TYPE = 'BASE TABLE'");
                createStatement.AppendLine("and TABLE_NAME not like '%LANDING%'");
                createStatement.AppendLine("and TABLE_NAME not like '%USERMANAGED%';");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 93);
                createStatement.Clear();
                
                createStatement.AppendLine("CREATE VIEW[interface].[INTERFACE_STAGING_HUB_XREF]");
                createStatement.AppendLine("AS");
                createStatement.AppendLine("SELECT");
                createStatement.AppendLine(" xref.STAGING_AREA_TABLE_ID,");
                createStatement.AppendLine(" STAGING_AREA_TABLE_NAME,");
                createStatement.AppendLine(" STAGING_AREA_SCHEMA_NAME,");
                createStatement.AppendLine(" xref.HUB_TABLE_ID,");
                createStatement.AppendLine(" HUB_TABLE_NAME,");
                createStatement.AppendLine(" BUSINESS_KEY_DEFINITION,");
                createStatement.AppendLine(" FILTER_CRITERIA");
                createStatement.AppendLine("FROM MD_STG_HUB_XREF xref");
                createStatement.AppendLine("JOIN MD_STG stg ON xref.STAGING_AREA_TABLE_ID = stg.STAGING_AREA_TABLE_ID");
                createStatement.AppendLine("JOIN MD_HUB hub ON xref.HUB_TABLE_ID = hub.HUB_TABLE_ID");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 95);
                createStatement.Clear();

                createStatement.AppendLine("CREATE VIEW [interface].[INTERFACE_STAGING_LINK_XREF]");
                createStatement.AppendLine("AS");
                createStatement.AppendLine("SELECT");
                createStatement.AppendLine("  xref.[STAGING_AREA_TABLE_ID]");
                createStatement.AppendLine(" ,stg.[STAGING_AREA_TABLE_NAME]");
                createStatement.AppendLine(" ,xref.[LINK_TABLE_ID]");
                createStatement.AppendLine(" ,lnk.[LINK_TABLE_NAME]");
                createStatement.AppendLine(" ,[FILTER_CRITERIA]");
                createStatement.AppendLine("FROM[MD_STG_LINK_XREF] xref");
                createStatement.AppendLine("JOIN MD_STG stg ON xref.STAGING_AREA_TABLE_ID = stg.STAGING_AREA_TABLE_ID");
                createStatement.AppendLine("JOIN MD_LINK lnk ON xref.LINK_TABLE_ID = lnk.LINK_TABLE_ID");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 97);
                createStatement.Clear();

                createStatement.AppendLine("CREATE VIEW [interface].[INTERFACE_STAGING_SATELLITE_ATTRIBUTE_XREF]");
                createStatement.AppendLine("AS");
                createStatement.AppendLine("SELECT");
                createStatement.AppendLine("  xref.[STAGING_AREA_TABLE_ID]");
                createStatement.AppendLine(" ,stg.STAGING_AREA_TABLE_NAME");
                createStatement.AppendLine(" ,stg.STAGING_AREA_SCHEMA_NAME");
                createStatement.AppendLine(" ,xref.[SATELLITE_TABLE_ID]");
                createStatement.AppendLine(" ,[SATELLITE_TABLE_NAME]");
                createStatement.AppendLine(" ,[ATTRIBUTE_ID_FROM]");
                createStatement.AppendLine(" ,att_from.[ATTRIBUTE_NAME] AS ATTRIBUTE_NAME_FROM");
                createStatement.AppendLine(" ,[ATTRIBUTE_ID_TO]");
                createStatement.AppendLine(" ,UPPER(att_to.[ATTRIBUTE_NAME]) AS ATTRIBUTE_NAME_TO");
                createStatement.AppendLine(" ,[MULTI_ACTIVE_KEY_INDICATOR]");
                createStatement.AppendLine("FROM [MD_STG_SAT_ATT_XREF] xref");
                createStatement.AppendLine("JOIN MD_STG stg ON xref.STAGING_AREA_TABLE_ID = stg.STAGING_AREA_TABLE_ID");
                createStatement.AppendLine("JOIN MD_SAT sat ON xref.SATELLITE_TABLE_ID = sat.SATELLITE_TABLE_ID");
                createStatement.AppendLine("JOIN MD_ATT att_from ON xref.ATTRIBUTE_ID_FROM = att_from.ATTRIBUTE_ID");
                createStatement.AppendLine("JOIN MD_ATT att_to ON xref.ATTRIBUTE_ID_TO = att_to.ATTRIBUTE_ID");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 98);
                createStatement.Clear();

                createStatement.AppendLine("CREATE VIEW[interface].[INTERFACE_STAGING_SATELLITE_XREF]");
                createStatement.AppendLine("AS");
                createStatement.AppendLine("SELECT");
                createStatement.AppendLine(" xref.STAGING_AREA_TABLE_ID,");
                createStatement.AppendLine(" STAGING_AREA_TABLE_NAME,");
                createStatement.AppendLine(" STAGING_AREA_SCHEMA_NAME,");
                createStatement.AppendLine(" xref.FILTER_CRITERIA,");
                createStatement.AppendLine(" xref.SATELLITE_TABLE_ID,");
                createStatement.AppendLine(" sat.SATELLITE_TABLE_NAME,");
                createStatement.AppendLine(" sat.SATELLITE_TYPE,");
                createStatement.AppendLine(" sat.HUB_TABLE_ID,");
                createStatement.AppendLine(" hub.HUB_TABLE_NAME,");
                createStatement.AppendLine(" stghubxref.BUSINESS_KEY_DEFINITION,");
                createStatement.AppendLine(" sat.LINK_TABLE_ID,");
                createStatement.AppendLine(" lnk.LINK_TABLE_NAME");
                createStatement.AppendLine("FROM MD_STG_SAT_XREF xref");
                createStatement.AppendLine("JOIN MD_STG stg ON xref.STAGING_AREA_TABLE_ID = stg.STAGING_AREA_TABLE_ID");
                createStatement.AppendLine("JOIN MD_SAT sat ON xref.SATELLITE_TABLE_ID = sat.SATELLITE_TABLE_ID");
                createStatement.AppendLine("JOIN MD_HUB hub ON sat.HUB_TABLE_ID = hub.HUB_TABLE_ID");
                createStatement.AppendLine("JOIN MD_LINK lnk ON sat.LINK_TABLE_ID = lnk.LINK_TABLE_ID");
                createStatement.AppendLine("LEFT JOIN MD_STG_HUB_XREF stghubxref");
                createStatement.AppendLine("  ON xref.STAGING_AREA_TABLE_ID = stghubxref.STAGING_AREA_TABLE_ID");
                createStatement.AppendLine("  AND hub.HUB_TABLE_ID = stghubxref.HUB_TABLE_ID");
                createStatement.AppendLine("  AND xref.BUSINESS_KEY_DEFINITION = stghubxref.BUSINESS_KEY_DEFINITION");
                createStatement.AppendLine();
                RunSqlCommand(connOmdString, createStatement, worker, 100);
                createStatement.Clear();

            }
        }

        private void RunSqlCommand(string connOmdString, StringBuilder createStatement, BackgroundWorker worker, int progressCounter)
        {
            using (var connectionVersion = new SqlConnection(connOmdString))
            {
                var commandVersion = new SqlCommand(createStatement.ToString(), connectionVersion);

                try
                {
                    connectionVersion.Open();
                    commandVersion.ExecuteNonQuery();

                    worker.ReportProgress(progressCounter);
                    _alert.SetTextLogging(createStatement.ToString());
                }
                catch (Exception ex)
                {
                    _alert.SetTextLogging("An issue has occured " + ex);
                }
            }
            createStatement.Clear();
        }

        private void buttonTruncate_Click(object sender, EventArgs e)
        {
            // Truncating the entire repository
            const string commandText = "DELETE FROM [MD_STG_LINK_ATT_XREF]; " +
                                       "DELETE FROM [MD_STG_SAT_ATT_XREF]; " +
                                       "DELETE FROM [MD_STG_LINK_XREF]; " +
                                       "DELETE FROM [MD_STG_SAT_XREF]; " +
                                       "DELETE FROM [MD_DRIVING_KEY_XREF]; " +
                                       "DELETE FROM [MD_HUB_LINK_XREF]; " +
                                       "DELETE FROM [MD_SAT]; " +
                                       "DELETE FROM [MD_BUSINESS_KEY_COMPONENT_PART]; " +
                                       "DELETE FROM [MD_BUSINESS_KEY_COMPONENT]; " +
                                       "DELETE FROM [MD_STG_HUB_XREF]; " +
                                       "DELETE FROM [MD_ATT]; " +
                                       "DELETE FROM [MD_STG]; " +
                                       "DELETE FROM [MD_HUB]; " +
                                       "DELETE FROM [MD_LINK]; " +
                                       "DELETE FROM [MD_TABLE_MAPPING]; " +
                                       "DELETE FROM [MD_ATTRIBUTE_MAPPING]; " +
                                       "TRUNCATE TABLE [MD_VERSION_ATTRIBUTE]; " +
                                       "TRUNCATE TABLE [MD_VERSION];";


            using (var connection = new SqlConnection(_myParent.textBoxMetadataConnection.Text))
            {
                var command = new SqlCommand(commandText, connection);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("The metadata tables have been truncated.", "Completed", MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An issue occurred when truncating the metadata tables. The error message is: "+ex, "An issue has occured", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}
