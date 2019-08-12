using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Virtual_Data_Warehouse.Classes;

namespace Virtual_EDW
{
    public partial class FormTestRi : FormBase
    {
        public FormTestRi (FormMain parent)
        {
            MyParent = parent;
            InitializeComponent();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonGenerateTestcases_Click(object sender, EventArgs e)
        {
            var connOmd = new SqlConnection {ConnectionString = TeamConfigurationSettings.ConnectionStringOmd};
            var metaDataTable = new DataTable();

            try
            {
                connOmd.Open();
            }
            catch (Exception exception)
            {
                richTextBoxOutput.Text =
                    "There was an error connecting to the metadata database. \r\n\r\nA connection could not be established. Can you verify the connection details for the metadata in the main screen? \r\n\r\nThe error message is: " +
                    exception.Message;
            }

            // Evaluate the query types based on the environments / radio buttons
            string environmentSnippet = "";
            string schemaSnippet = "";
            
            if (radioButtonPSA.Checked)
            {
                environmentSnippet=TeamConfigurationSettings.PsaDatabaseName;
                schemaSnippet = VedwConfigurationSettings.VedwSchema;
            }
            else if (radioButtonIntegrationLayer.Checked)
            {
                environmentSnippet = TeamConfigurationSettings.IntegrationDatabaseName;
                schemaSnippet = "";
            }

            var queryRi = new StringBuilder();
            queryRi.AppendLine("--");
            queryRi.AppendLine("-- Referential Integrity Validation Query");
            queryRi.AppendLine("-- Generated at " + DateTime.Now);
            queryRi.AppendLine("--");
            queryRi.AppendLine();

            #region Satellite
            // Satellite component
            queryRi.AppendLine("GO");
            queryRi.AppendLine();
            queryRi.AppendLine("-- Satellite validation");
            queryRi.AppendLine();

            var queryTableArraySat = new StringBuilder();

            queryTableArraySat.AppendLine(@"
            SELECT DISTINCT
               sat.[SOURCE_SCHEMA_NAME]
              ,sat.[SOURCE_NAME]
              ,sat.[TARGET_SCHEMA_NAME]
              ,sat.[TARGET_NAME]
              ,sat.[SOURCE_BUSINESS_KEY_DEFINITION]
              ,sat.[TARGET_BUSINESS_KEY_DEFINITION]
              ,sat.[TARGET_TYPE]
              ,sat.[SURROGATE_KEY]
              ,sat.[FILTER_CRITERIA]
              ,sat.[LOAD_VECTOR]
              ,hub.[TARGET_SCHEMA_NAME] AS [HUB_SCHEMA_NAME]
              ,hub.[TARGET_NAME] AS [HUB_NAME]
            FROM [interface].[INTERFACE_SOURCE_SATELLITE_XREF] sat
            JOIN [interface].[INTERFACE_SOURCE_HUB_XREF] hub 
	          ON sat.[SOURCE_NAME] = hub.[SOURCE_NAME]
            AND sat.[TARGET_BUSINESS_KEY_DEFINITION] = hub.[TARGET_BUSINESS_KEY_DEFINITION]
            WHERE sat.[TARGET_TYPE]='Normal'
            ");

            metaDataTable = MyParent.GetDataTable(ref connOmd, queryTableArraySat.ToString());

            if (metaDataTable.Rows.Count == 0)
            {
                richTextBoxInformationMain.Text += "There was no metadata available to create Satellite Referential Integrity scripts.";
            }
            else
            {
                foreach (DataRow row in metaDataTable.Rows)
                {
                    queryRi.AppendLine("SELECT COUNT(*) AS RI_ISSUES, '" + (string)row["TARGET_NAME"] + "'");
                    queryRi.AppendLine("FROM [" + environmentSnippet + "].[" + (string)row["TARGET_SCHEMA_NAME"] + "].[" + (string)row["TARGET_NAME"] + "] sat");
                    queryRi.AppendLine("WHERE NOT EXISTS");
                    queryRi.AppendLine("(");
                    queryRi.AppendLine("  SELECT 1 FROM [" + environmentSnippet + "].[" + (string)row["HUB_SCHEMA_NAME"] + "].[" + (string)row["HUB_NAME"] + "] hub WHERE sat.["+(string)row["SURROGATE_KEY"]+"] = hub.["+(string)row["SURROGATE_KEY"]+"]");
                    queryRi.AppendLine(")");

                    if (radioButtonDeltaValidation.Checked)
                    {
                        var businessKeyList = InterfaceHandling.BusinessKeyComponentMappingList((string)row["SOURCE_BUSINESS_KEY_DEFINITION"], (string)row["TARGET_BUSINESS_KEY_DEFINITION"]);

                        var surrogateKeySnippet = new StringBuilder();
                        surrogateKeySnippet.AppendLine("HASHBYTES('MD5',");

                        
                        foreach (var businessKey in businessKeyList)
                        {
                            string businessKeyEval = InterfaceHandling.EvaluateBusinessKey(businessKey);

                            surrogateKeySnippet.AppendLine("    ISNULL(RTRIM(CONVERT(NVARCHAR(100)," + businessKeyEval + ")),'NA')+'|'+");
                        }

                        surrogateKeySnippet.Remove(surrogateKeySnippet.Length - 3, 3);
                        surrogateKeySnippet.AppendLine();
                        surrogateKeySnippet.AppendLine("  )");

                        queryRi.AppendLine("AND EXISTS");
                        queryRi.AppendLine("(");
                        queryRi.AppendLine("  SELECT 1 FROM [" + TeamConfigurationSettings.StagingDatabaseName + "].[" + (string)row["SOURCE_SCHEMA_NAME"] + "].[" + (string)row["SOURCE_NAME"] + "] WHERE sat.[" + (string)row["SURROGATE_KEY"] + "] = ");
                        queryRi.AppendLine("  " + surrogateKeySnippet.ToString());
                        queryRi.Remove(queryRi.Length - 3, 3);
                        queryRi.AppendLine(")");
                    }

                    queryRi.AppendLine("--");
                    queryRi.AppendLine("UNION ALL");
                    queryRi.AppendLine("--");
                }
                queryRi.Remove(queryRi.Length - 19, 19);
            }
            #endregion

            #region Link
            // Link component
            var queryTableArrayLink = @"
            SELECT 
                 [SOURCE_SCHEMA_NAME]
                ,[SOURCE_NAME]
                ,[LINK_SCHEMA_NAME]
                ,[LINK_NAME]
                ,[HUB_SCHEMA_NAME]
                ,[HUB_NAME]
                ,[HUB_SURROGATE_KEY]
                ,[HUB_TARGET_KEY_NAME_IN_LINK]
                ,[HUB_SOURCE_BUSINESS_KEY_DEFINITION]
                ,[HUB_TARGET_BUSINESS_KEY_DEFINITION]
                ,[HUB_ORDER]
            FROM [interface].[INTERFACE_HUB_LINK_XREF]
            WHERE LINK_NAME !='Not applicable'
            ";

            metaDataTable= MyParent.GetDataTable(ref connOmd, queryTableArrayLink);

            queryRi.AppendLine();
            queryRi.AppendLine("-- Link validation");
            queryRi.AppendLine();

            if (metaDataTable.Rows.Count == 0)
            {
                richTextBoxInformationMain.Text +=
                    "There was no metadata available to create Link Referential Integrity scripts.";
            }
            else
            {
                foreach (DataRow row in metaDataTable.Rows)
                {
                    queryRi.AppendLine("SELECT COUNT(*) AS RI_ISSUES, '" + (string)row["LINK_NAME"] + "'");
                    queryRi.AppendLine("FROM [" + environmentSnippet + "].[" + (string)row["LINK_SCHEMA_NAME"] + "].[" + (string)row["LINK_NAME"] + "] lnk");
                    queryRi.AppendLine("WHERE NOT EXISTS");
                    queryRi.AppendLine("(");
                    queryRi.AppendLine("  SELECT 1 FROM [" + environmentSnippet + "].[" + (string)row["HUB_SCHEMA_NAME"] + "].[" + (string)row["HUB_NAME"] + "] hub WHERE lnk.[" + (string)row["HUB_TARGET_KEY_NAME_IN_LINK"] + "] = hub.[" + (string)row["HUB_SURROGATE_KEY"] + "]");
                    queryRi.AppendLine(")");

                    if (radioButtonDeltaValidation.Checked)
                    {
                        var businessKeyList = InterfaceHandling.BusinessKeyComponentMappingList((string)row["HUB_SOURCE_BUSINESS_KEY_DEFINITION"], (string)row["HUB_TARGET_BUSINESS_KEY_DEFINITION"]);

                        var surrogateKeySnippet = new StringBuilder();
                        surrogateKeySnippet.AppendLine("HASHBYTES('MD5',");


                        foreach (var businessKey in businessKeyList)
                        {
                            string businessKeyEval = InterfaceHandling.EvaluateBusinessKey(businessKey);

                            surrogateKeySnippet.AppendLine("    ISNULL(RTRIM(CONVERT(NVARCHAR(100)," + businessKeyEval + ")),'NA')+'|'+");
                        }

                        surrogateKeySnippet.Remove(surrogateKeySnippet.Length - 3, 3);
                        surrogateKeySnippet.AppendLine();
                        surrogateKeySnippet.AppendLine("  )");

                        queryRi.AppendLine("AND EXISTS");
                        queryRi.AppendLine("(");
                        queryRi.AppendLine("  SELECT 1 FROM [" + TeamConfigurationSettings.StagingDatabaseName + "].[" + (string)row["SOURCE_SCHEMA_NAME"] + "].[" + (string)row["SOURCE_NAME"] + "] WHERE lnk.[" + (string)row["HUB_SURROGATE_KEY"] + "] = ");
                        queryRi.AppendLine("  " + surrogateKeySnippet.ToString());
                        queryRi.Remove(queryRi.Length - 3, 3);
                        queryRi.AppendLine(")");
                    }

                    queryRi.AppendLine("--");
                    queryRi.AppendLine("UNION ALL");
                    queryRi.AppendLine("--");
                }
                queryRi.Remove(queryRi.Length - 19, 19);
            }
            #endregion

            #region LinkSatellite
            // Link Satellite component
            queryRi.AppendLine("GO");
            queryRi.AppendLine();
            queryRi.AppendLine("-- Satellite validation");
            queryRi.AppendLine();

            var queryTableArrayLsat = new StringBuilder();

            queryTableArrayLsat.AppendLine(@"
            SELECT DISTINCT
               sat.[SOURCE_SCHEMA_NAME]
              ,sat.[SOURCE_NAME]
              ,sat.[TARGET_SCHEMA_NAME]
              ,sat.[TARGET_NAME]
              ,sat.[SOURCE_BUSINESS_KEY_DEFINITION]
              ,sat.[TARGET_BUSINESS_KEY_DEFINITION]
              ,sat.[TARGET_TYPE]
              ,sat.[SURROGATE_KEY]
              ,sat.[FILTER_CRITERIA]
              ,sat.[LOAD_VECTOR]
              ,lnk.[TARGET_SCHEMA_NAME] AS [LINK_SCHEMA_NAME]
              ,lnk.[TARGET_NAME] AS [LINK_NAME]
            FROM [interface].[INTERFACE_SOURCE_SATELLITE_XREF] sat
            JOIN [interface].[INTERFACE_SOURCE_LINK_XREF] lnk 
	          ON sat.[SOURCE_NAME] = lnk.[SOURCE_NAME]
            AND sat.[TARGET_BUSINESS_KEY_DEFINITION] = lnk.[TARGET_BUSINESS_KEY_DEFINITION]
            WHERE sat.[TARGET_TYPE]='Link Satellite'
            ");

            metaDataTable = MyParent.GetDataTable(ref connOmd, queryTableArrayLsat.ToString());

            if (metaDataTable.Rows.Count == 0)
            {
                richTextBoxInformationMain.Text += "There was no metadata available to create Link Satellite Referential Integrity scripts.";
            }
            else
            {
                foreach (DataRow row in metaDataTable.Rows)
                {
                    queryRi.AppendLine("SELECT COUNT(*) AS RI_ISSUES, '" + (string)row["TARGET_NAME"] + "'");
                    queryRi.AppendLine("FROM [" + environmentSnippet + "].[" + (string)row["TARGET_SCHEMA_NAME"] + "].[" + (string)row["TARGET_NAME"] + "] sat");
                    queryRi.AppendLine("WHERE NOT EXISTS");
                    queryRi.AppendLine("(");
                    queryRi.AppendLine("  SELECT 1 FROM [" + environmentSnippet + "].[" + (string)row["LINK_SCHEMA_NAME"] + "].[" + (string)row["LINK_NAME"] + "] lnk WHERE sat.[" + (string)row["SURROGATE_KEY"] + "] = lnk.[" + (string)row["SURROGATE_KEY"] + "]");
                    queryRi.AppendLine(")");

                    if (radioButtonDeltaValidation.Checked)
                    {
                        var businessKeyList = InterfaceHandling.BusinessKeyComponentMappingList((string)row["SOURCE_BUSINESS_KEY_DEFINITION"], (string)row["TARGET_BUSINESS_KEY_DEFINITION"]);

                        var surrogateKeySnippet = new StringBuilder();
                        surrogateKeySnippet.AppendLine("HASHBYTES('MD5',");

                        foreach (var businessKey in businessKeyList)
                        {
                            string businessKeyEval = InterfaceHandling.EvaluateBusinessKey(businessKey);

                            surrogateKeySnippet.AppendLine("    ISNULL(RTRIM(CONVERT(NVARCHAR(100)," + businessKeyEval + ")),'NA')+'|'+");
                        }

                        surrogateKeySnippet.Remove(surrogateKeySnippet.Length - 3, 3);
                        surrogateKeySnippet.AppendLine();
                        surrogateKeySnippet.AppendLine("  )");

                        queryRi.AppendLine("AND EXISTS");
                        queryRi.AppendLine("(");
                        queryRi.AppendLine("  SELECT 1 FROM [" + TeamConfigurationSettings.StagingDatabaseName + "].[" + (string)row["SOURCE_SCHEMA_NAME"] + "].[" + (string)row["SOURCE_NAME"] + "] WHERE sat.[" + (string)row["SURROGATE_KEY"] + "] = ");
                        queryRi.AppendLine("  " + surrogateKeySnippet.ToString());
                        queryRi.Remove(queryRi.Length - 3, 3);
                        queryRi.AppendLine(")");
                    }

                    queryRi.AppendLine("--");
                    queryRi.AppendLine("UNION ALL");
                    queryRi.AppendLine("--");
                }
                queryRi.Remove(queryRi.Length - 19, 19);
            }
            #endregion

            richTextBoxOutput.Text = queryRi.ToString();

        }


    }
}
