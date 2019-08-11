using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

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
                DebuggingTextbox.Text =
                    "There was an error connecting to the metadata database. \r\n\r\nA connection could not be established. Can you verify the connection details for the metadata in the main screen? \r\n\r\nThe error message is: " +
                    exception.Message;
            }

            string virtualisationSnippet = "";

            var queryRi = new StringBuilder();
            queryRi.AppendLine("--");
            queryRi.AppendLine("-- Referential Integrity Validation Query");
            queryRi.AppendLine("-- Generated at " + DateTime.Now);
            queryRi.AppendLine("--");
            queryRi.AppendLine();

            if (radioButtonPSA.Checked)
            {
                queryRi.AppendLine("USE [" + TeamConfigurationSettings.PsaDatabaseName + "]");
                virtualisationSnippet = VedwConfigurationSettings.VedwSchema + ".";
            }
            else if (radioButtonIntegrationLayer.Checked)
            {
                queryRi.AppendLine("USE [" + TeamConfigurationSettings.IntegrationDatabaseName + "]");
                virtualisationSnippet = "";
            }

            var stringDataType = VedwConfigurationSettings.EnableUnicode == "True" ? "NVARCHAR" : "VARCHAR";

            // Satellite component
            queryRi.AppendLine("GO");
            queryRi.AppendLine();
            queryRi.AppendLine("-- Satellite validation");
            queryRi.AppendLine();

            var queryTableArraySat = new StringBuilder();

            queryTableArraySat.AppendLine("SELECT");
            queryTableArraySat.AppendLine(" ,[SOURCE_NAME]");
            queryTableArraySat.AppendLine(" ,[SOURCE_BUSINESS_KEY_DEFINITION]");
            queryTableArraySat.AppendLine(" ,[FILTER_CRITERIA]");
            queryTableArraySat.AppendLine(" ,[SATELLITE_NAME]");
            queryTableArraySat.AppendLine(" ,[SATELLITE_TYPE]");
            queryTableArraySat.AppendLine(" ,[HUB_ID]");
            queryTableArraySat.AppendLine(" ,[HUB_NAME]");
            queryTableArraySat.AppendLine(" ,[LINK_ID]");
            queryTableArraySat.AppendLine(" ,[LINK_NAME]");
            queryTableArraySat.AppendLine("FROM[interface].[INTERFACE_SOURCE_SATELLITE_XREF]");
            queryTableArraySat.AppendLine("WHERE [SATELLITE_TYPE]='Normal'");

            var satTables = MyParent.GetDataTable(ref connOmd, queryTableArraySat.ToString());

            if (satTables.Rows.Count == 0)
            {
                richTextBoxOutput.Text +=
                    "There was no metadata available to create Satellite Referential Integrity scripts.";
            }
            else
            {
                foreach (DataRow row in satTables.Rows)
                {
                    var satTableName = (string) row["SATELLITE_NAME"];
                    var stagingAreaTableName = (string) row["SOURCE_NAME"];
                    var businessKeyDefinition = (string) row["SOURCE_BUSINESS_KEY_DEFINITION"];
                    var hubTableName = (string) row["HUB_NAME"];
                    var hubSk = hubTableName.Substring(4) + "_" + TeamConfigurationSettings.DwhKeyIdentifier;

                    // Retrieving the business key attributes for the Hub                 
                    var hubKeyList = MyParent.GetHubTargetBusinessKeyList(hubTableName);


                    var hubQuerySelect = "";


                    queryRi.AppendLine("SELECT COUNT(*) AS RI_ISSUES, '" + satTableName + "'");
                    queryRi.AppendLine("FROM "+ virtualisationSnippet + satTableName + " A");

                    if (radioButtonDeltaValidation.Checked) // Join to the Staging Area, on the business key
                    {
                        queryRi.AppendLine("JOIN ");
                        queryRi.AppendLine("(");
                        queryRi.AppendLine("  SELECT ");
                        //Regular Hash
                        queryRi.AppendLine("    CONVERT(CHAR(32),HASHBYTES('MD5',");

                        foreach (DataRow hubKey in hubKeyList.Rows)
                        {
                            queryRi.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100),[" +(string) hubKey["COLUMN_NAME"] + "])),'NA')+'|'+");
                        }

                        queryRi.Remove(queryRi.Length - 3, 3);
                        queryRi.AppendLine();
                        queryRi.AppendLine("    ),2) AS " + hubSk);
                        queryRi.AppendLine("  FROM ");
                        queryRi.AppendLine("  (");
                        queryRi.AppendLine("    SELECT " + hubQuerySelect + " FROM [" +TeamConfigurationSettings.StagingDatabaseName + "].[dbo].[" +stagingAreaTableName + "]");
                        queryRi.AppendLine("  ) stgsub");
                        queryRi.AppendLine(") staging ON ");
                        queryRi.AppendLine("A." + hubSk + " = staging." + hubSk);
                        // queryRI.AppendLine("JOIN " + stgTableName + " stg ON "+hubQuerySelect);
                    }

                    queryRi.AppendLine("LEFT OUTER JOIN " + virtualisationSnippet + hubTableName + " B on A." + hubSk + " = B." + hubSk);
                    queryRi.AppendLine("WHERE B." + hubSk + " IS NULL");
                    queryRi.AppendLine("--");
                    queryRi.AppendLine("UNION ALL");
                    queryRi.AppendLine("--");
                }

                queryRi.Remove(queryRi.Length - 15, 15);
            }


            // Link component
            var queryTableArrayLink =
                "SELECT DISTINCT LINK_ID, LINK_NAME FROM MD_LINK WHERE LINK_NAME !='Not applicable'";

            var linkTables = MyParent.GetDataTable(ref connOmd, queryTableArrayLink);

            queryRi.AppendLine();
            queryRi.AppendLine("-- Link validation");
            queryRi.AppendLine();

            if (linkTables.Rows.Count == 0)
            {
                richTextBoxOutput.Text +=
                    "There was no metadata available to create Link Referential Integrity scripts.";
            }
            else
            {
                foreach (DataRow row in linkTables.Rows)
                {
                    var linkTableName = (string) row["LINK_NAME"];

                    queryRi.AppendLine("SELECT COUNT(*) AS RI_ISSUES, '" + linkTableName + "'");
                    queryRi.AppendLine("FROM " + virtualisationSnippet + linkTableName);

                    var queryHubArray = "SELECT DISTINCT b.HUB_NAME " +
                                        "FROM MD_HUB_LINK_XREF a " +
                                        "JOIN MD_HUB b ON a.HUB_ID=b.HUB_ID " +
                                        "WHERE a.LINK_ID = " + (int) row["LINK_ID"];

                    var hubTables = MyParent.GetDataTable(ref connOmd, queryHubArray);
                    foreach (DataRow hubRow in hubTables.Rows)
                    {
                        var hubTableName = (string) hubRow["HUB_NAME"];
                        var hubSk = hubTableName.Substring(4) + "_" + TeamConfigurationSettings.DwhKeyIdentifier;
                        queryRi.AppendLine("LEFT OUTER JOIN " + virtualisationSnippet + hubTableName + " on " + virtualisationSnippet + linkTableName + "." + hubSk +
                                           " = " + hubTableName + "." + hubSk);
                    }

                    queryRi.AppendLine("WHERE (");
                    foreach (DataRow hubRow in hubTables.Rows)
                    {
                        var hubTableName = (string) hubRow["HUB_NAME"];
                        var hubSk = hubTableName.Substring(4) + "_" + TeamConfigurationSettings.DwhKeyIdentifier;
                        queryRi.AppendLine("  " + virtualisationSnippet + hubTableName + "." + hubSk + " IS NULL OR");
                    }

                    queryRi.Remove(queryRi.Length - 5, 5);
                    queryRi.AppendLine();
                    queryRi.AppendLine(")");

                    queryRi.AppendLine("--");
                    queryRi.AppendLine("UNION ALL");
                    queryRi.AppendLine("--");

                }

                queryRi.Remove(queryRi.Length - 15, 15);

                // Link-Satellite component
                queryRi.AppendLine("GO");
                queryRi.AppendLine();
                queryRi.AppendLine("-- Link Satellite validation");
                queryRi.AppendLine();

                var queryTableArrayLinkSat = new StringBuilder();

                queryTableArrayLinkSat.AppendLine("SELECT");
                queryTableArrayLinkSat.AppendLine("  [SOURCE_ID]");
                queryTableArrayLinkSat.AppendLine(" ,[SOURCE_NAME]");
                queryTableArrayLinkSat.AppendLine(" ,[SOURCE_BUSINESS_KEY_DEFINITION]");
                queryTableArrayLinkSat.AppendLine(" ,[FILTER_CRITERIA]");
                queryTableArrayLinkSat.AppendLine(" ,[SATELLITE_ID]");
                queryTableArrayLinkSat.AppendLine(" ,[SATELLITE_NAME]");
                queryTableArrayLinkSat.AppendLine(" ,[SATELLITE_TYPE]");
                queryTableArrayLinkSat.AppendLine(" ,[HUB_ID]");
                queryTableArrayLinkSat.AppendLine(" ,[HUB_NAME]");
                queryTableArrayLinkSat.AppendLine(" ,[LINK_ID]");
                queryTableArrayLinkSat.AppendLine(" ,[LINK_NAME]");
                queryTableArrayLinkSat.AppendLine("FROM[interface].[INTERFACE_SOURCE_SATELLITE_XREF]");
                queryTableArrayLinkSat.AppendLine("WHERE [SATELLITE_TYPE]='Link Satellite'");

                var linkSatTables = MyParent.GetDataTable(ref connOmd, queryTableArrayLinkSat.ToString());

                if (satTables.Rows.Count == 0)
                {
                    richTextBoxOutput.Text +=
                        "There was no metadata available to create Link Satellite Referential Integrity scripts.";
                }
                else
                {
                    foreach (DataRow row in linkSatTables.Rows)
                    {
                        var lsatTableName = (string) row["SATELLITE_NAME"];
                        var linkTableName = (string) row["LINK_NAME"];
                        var hubSk = linkTableName.Substring(4) + "_" + TeamConfigurationSettings.DwhKeyIdentifier;

                        queryRi.AppendLine("SELECT COUNT(*) AS RI_ISSUES, '" + lsatTableName + "'");
                        queryRi.AppendLine("FROM " + virtualisationSnippet + lsatTableName + " A");
                        queryRi.AppendLine("LEFT OUTER JOIN " + virtualisationSnippet + linkTableName + " B on A." + hubSk + " = B." + hubSk);
                        queryRi.AppendLine("WHERE B." + hubSk + " IS NULL");
                        queryRi.AppendLine("--");
                        queryRi.AppendLine("UNION ALL");
                        queryRi.AppendLine("--");
                    }

                    queryRi.Remove(queryRi.Length - 15, 15);
                }

            }

            DebuggingTextbox.Text = queryRi.ToString();

        }
    }
}
