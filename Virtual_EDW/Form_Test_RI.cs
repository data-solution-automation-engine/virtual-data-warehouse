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

            var satTables = MyParent.GetDataTable(ref connOmd, queryTableArraySat.ToString());

            if (satTables.Rows.Count == 0)
            {
                richTextBoxInformationMain.Text += "There was no metadata available to create Satellite Referential Integrity scripts.";
            }
            else
            {
                foreach (DataRow row in satTables.Rows)
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


            // Link component
            //var queryTableArrayLink = @"
            //    SELECT DISTINCT LINK_ID, LINK_NAME 
            //    FROM MD_LINK 
            //    WHERE LINK_NAME !='Not applicable'
            //";

            //var linkTables = MyParent.GetDataTable(ref connOmd, queryTableArrayLink);

            //queryRi.AppendLine();
            //queryRi.AppendLine("-- Link validation");
            //queryRi.AppendLine();

            //if (linkTables.Rows.Count == 0)
            //{
            //    richTextBoxInformationMain.Text +=
            //        "There was no metadata available to create Link Referential Integrity scripts.";
            //}
            //else
            //{
            //    foreach (DataRow row in linkTables.Rows)
            //    {
            //        var linkTableName = (string) row["LINK_NAME"];

            //        queryRi.AppendLine("SELECT COUNT(*) AS RI_ISSUES, '" + linkTableName + "'");
            //        queryRi.AppendLine("FROM " + virtualisationSnippet + linkTableName);

            //        var queryHubArray = "SELECT DISTINCT b.HUB_NAME " +
            //                            "FROM MD_HUB_LINK_XREF a " +
            //                            "JOIN MD_HUB b ON a.HUB_ID=b.HUB_ID " +
            //                            "WHERE a.LINK_ID = " + (int) row["LINK_ID"];

            //        var hubTables = MyParent.GetDataTable(ref connOmd, queryHubArray);
            //        foreach (DataRow hubRow in hubTables.Rows)
            //        {
            //            var hubTableName = (string) hubRow["HUB_NAME"];
            //            var hubSk = hubTableName.Substring(4) + "_" + TeamConfigurationSettings.DwhKeyIdentifier;
            //            queryRi.AppendLine("LEFT OUTER JOIN " + virtualisationSnippet + hubTableName + " on " + virtualisationSnippet + linkTableName + "." + hubSk +
            //                               " = " + hubTableName + "." + hubSk);
            //        }

            //        queryRi.AppendLine("WHERE (");
            //        foreach (DataRow hubRow in hubTables.Rows)
            //        {
            //            var hubTableName = (string) hubRow["HUB_NAME"];
            //            var hubSk = hubTableName.Substring(4) + "_" + TeamConfigurationSettings.DwhKeyIdentifier;
            //            queryRi.AppendLine("  " + virtualisationSnippet + hubTableName + "." + hubSk + " IS NULL OR");
            //        }

            //        queryRi.Remove(queryRi.Length - 5, 5);
            //        queryRi.AppendLine();
            //        queryRi.AppendLine(")");

            //        queryRi.AppendLine("--");
            //        queryRi.AppendLine("UNION ALL");
            //        queryRi.AppendLine("--");

            //    }

            //    queryRi.Remove(queryRi.Length - 15, 15);

            // Link-Satellite component
            //    queryRi.AppendLine("GO");
            //    queryRi.AppendLine();
            //    queryRi.AppendLine("-- Link Satellite validation");
            //    queryRi.AppendLine();

            //    var queryTableArrayLinkSat = new StringBuilder();

            //    queryTableArrayLinkSat.AppendLine("SELECT");
            //    queryTableArrayLinkSat.AppendLine("  [SOURCE_ID]");
            //    queryTableArrayLinkSat.AppendLine(" ,[SOURCE_NAME]");
            //    queryTableArrayLinkSat.AppendLine(" ,[SOURCE_BUSINESS_KEY_DEFINITION]");
            //    queryTableArrayLinkSat.AppendLine(" ,[FILTER_CRITERIA]");
            //    queryTableArrayLinkSat.AppendLine(" ,[SATELLITE_ID]");
            //    queryTableArrayLinkSat.AppendLine(" ,[SATELLITE_NAME]");
            //    queryTableArrayLinkSat.AppendLine(" ,[SATELLITE_TYPE]");
            //    queryTableArrayLinkSat.AppendLine(" ,[HUB_ID]");
            //    queryTableArrayLinkSat.AppendLine(" ,[HUB_NAME]");
            //    queryTableArrayLinkSat.AppendLine(" ,[LINK_ID]");
            //    queryTableArrayLinkSat.AppendLine(" ,[LINK_NAME]");
            //    queryTableArrayLinkSat.AppendLine("FROM[interface].[INTERFACE_SOURCE_SATELLITE_XREF]");
            //    queryTableArrayLinkSat.AppendLine("WHERE [SATELLITE_TYPE]='Link Satellite'");

            //    var linkSatTables = MyParent.GetDataTable(ref connOmd, queryTableArrayLinkSat.ToString());

            //    if (satTables.Rows.Count == 0)
            //    {
            //        richTextBoxInformationMain.Text +=
            //            "There was no metadata available to create Link Satellite Referential Integrity scripts.";
            //    }
            //    else
            //    {
            //        foreach (DataRow row in linkSatTables.Rows)
            //        {
            //            var lsatTableName = (string) row["SATELLITE_NAME"];
            //            var linkTableName = (string) row["LINK_NAME"];
            //            var hubSk = linkTableName.Substring(4) + "_" + TeamConfigurationSettings.DwhKeyIdentifier;

            //            queryRi.AppendLine("SELECT COUNT(*) AS RI_ISSUES, '" + lsatTableName + "'");
            //            queryRi.AppendLine("FROM " + virtualisationSnippet + lsatTableName + " A");
            //            queryRi.AppendLine("LEFT OUTER JOIN " + virtualisationSnippet + linkTableName + " B on A." + hubSk + " = B." + hubSk);
            //            queryRi.AppendLine("WHERE B." + hubSk + " IS NULL");
            //            queryRi.AppendLine("--");
            //            queryRi.AppendLine("UNION ALL");
            //            queryRi.AppendLine("--");
            //        }
            //        queryRi.Remove(queryRi.Length - 15, 15);
            //    }
            //}

            richTextBoxOutput.Text = queryRi.ToString();

        }


    }
}
