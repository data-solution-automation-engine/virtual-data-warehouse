using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Virtual_EDW
{
    public partial class FormTestRi : FormBase
    {
        //private readonly FormMain _myParent;

        public FormTestRi (FormMain parent)
        {
            MyParent = parent;
            InitializeComponent();
            // radioButtonPSA.Checked = true;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonGenerateTestcases_Click(object sender, EventArgs e)
        {
            var connOmd = new SqlConnection {ConnectionString = ConfigurationSettings.ConnectionStringOmd};
            var connStg = new SqlConnection { ConnectionString = ConfigurationSettings.ConnectionStringStg };

            var versionId = 0;//_myParent.trackBarVersioning.Value;

            // Assign database connection string

            try
            {
                connOmd.Open();
            }
            catch (Exception exception)
            {
                DebuggingTextbox.Text = "There was an error connecting to the metadata database. \r\n\r\nA connection could not be established. Can you verify the connection details for the metadata in the main screen? \r\n\r\nThe error message is: " + exception.Message;
            }

            var queryRi = new StringBuilder();

                queryRi.AppendLine("--");
                queryRi.AppendLine("-- Referential Integrity Validation Query");
                queryRi.AppendLine("-- Generated at " + DateTime.Now);
                queryRi.AppendLine("--");
                queryRi.AppendLine();

                if (radioButtonPSA.Checked)
                {
                    queryRi.AppendLine("USE [" + ConfigurationSettings.PsaDatabaseName + "]");
                }
                else if (radioButtonIntegrationLayer.Checked)
                {
                    queryRi.AppendLine("USE [" + ConfigurationSettings.IntegrationDatabaseName + "]");
                }

                var stringDataType = ConfigurationSettingsVedwSpecific.EnableUnicode == "True" ? "NVARCHAR" : "VARCHAR";

                // Satellite component
                queryRi.AppendLine("GO");
                queryRi.AppendLine();
                queryRi.AppendLine("-- Satellite validation");
                queryRi.AppendLine();

                var queryTableArraySat = new StringBuilder();

                queryTableArraySat.AppendLine("SELECT");
                queryTableArraySat.AppendLine("  [STAGING_AREA_TABLE_ID]");
                queryTableArraySat.AppendLine(" ,[STAGING_AREA_TABLE_NAME]");
                queryTableArraySat.AppendLine(" ,[STAGING_AREA_SCHEMA_NAME]");
                queryTableArraySat.AppendLine(" ,[FILTER_CRITERIA]");
                queryTableArraySat.AppendLine(" ,[SATELLITE_TABLE_ID]");
                queryTableArraySat.AppendLine(" ,[SATELLITE_TABLE_NAME]");
                queryTableArraySat.AppendLine(" ,[SATELLITE_TYPE]");
                queryTableArraySat.AppendLine(" ,[HUB_TABLE_ID]");
                queryTableArraySat.AppendLine(" ,[HUB_TABLE_NAME]");
                queryTableArraySat.AppendLine(" ,[BUSINESS_KEY_DEFINITION]");
                queryTableArraySat.AppendLine(" ,[LINK_TABLE_ID]");
                queryTableArraySat.AppendLine(" ,[LINK_TABLE_NAME]");
                queryTableArraySat.AppendLine("FROM[interface].[INTERFACE_STAGING_SATELLITE_XREF]");
                queryTableArraySat.AppendLine("WHERE [SATELLITE_TYPE]='Normal'");

                var satTables = MyParent.GetDataTable(ref connOmd, queryTableArraySat.ToString());

                if (satTables.Rows.Count == 0)
                {
                    richTextBoxOutput.Text += "There was no metadata available to create Satellite Referential Integrity scripts.";
                }
                else
                {
                    foreach (DataRow row in satTables.Rows)
                    {
                        var satTableName = (string)row["SATELLITE_TABLE_NAME"];
                        var stagingAreaTableName = (string)row["STAGING_AREA_TABLE_NAME"];
                        var businessKeyDefinition = (string) row["BUSINESS_KEY_DEFINITION"];
                        var hubTableName = (string)row["HUB_TABLE_NAME"];
                        var hubSk = hubTableName.Substring(4) + "_" + ConfigurationSettings.DwhKeyIdentifier;

                        // Retrieving the business key attributes for the Hub                 
                        var hubKeyList = MyParent.GetHubTargetBusinessKeyList(hubTableName, versionId);

                        var fieldList = new StringBuilder();
                        var compositeKey = new StringBuilder();
                        var fieldDict = new Dictionary<string, string>();
                        var fieldOrderedList = new List<string>();
                        string firstKey;
                        var sqlStatementForSourceQuery = new StringBuilder();
                        var hubQuerySelect = new StringBuilder();
                        var hubQueryWhere = new StringBuilder();
                        var hubQueryGroupBy = new StringBuilder();

                    // For every STG / Hub relationship, the business key needs to be defined - starting with the components of the key
                    var componentList = MyParent.GetBusinessKeyComponentList(stagingAreaTableName, hubTableName, businessKeyDefinition);

                    // Components are key parts, such as a composite key (2 or more components) or regular and concatenated keys (1 component)
                    foreach (DataRow component in componentList.Rows)
                    {
                        var componentId = (int)component["BUSINESS_KEY_COMPONENT_ID"] - 1;

                        // Retrieve the elements of each business key component
                        // This only concerns concatenated keys as they are single component keys comprising of multiple elements.
                        var elementList = MyParent.GetBusinessKeyElements(stagingAreaTableName, hubTableName, businessKeyDefinition, (int)component["BUSINESS_KEY_COMPONENT_ID"]);

                        if (elementList == null)
                        {
                            richTextBoxOutput.Text += ("An error occurred for {hubTableName}. The collection of Business Keys is empty.\r\n");
                        }
                        else
                        {
                            if (elementList.Rows.Count > 1) // Build a concatinated key if the count of elements is greater than 1 for a component (key part)
                            {
                                fieldList.Clear();
                                fieldDict.Clear();

                                foreach (DataRow element in elementList.Rows)
                                {
                                    var elementType = element["BUSINESS_KEY_COMPONENT_ELEMENT_TYPE"].ToString();

                                    if (elementType == "Attribute")
                                    {
                                        fieldList.Append("'" + element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] + "',");

                                        sqlStatementForSourceQuery.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION");
                                        sqlStatementForSourceQuery.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS");
                                        sqlStatementForSourceQuery.AppendLine("WHERE TABLE_NAME= '" + stagingAreaTableName + "'");
                                        sqlStatementForSourceQuery.AppendLine("AND COLUMN_NAME IN (" + fieldList.ToString().Substring(0, fieldList.ToString().Length - 1) + ")");

                                        var elementDataTypes = MyParent.GetDataTable(ref connStg, sqlStatementForSourceQuery.ToString());

                                        foreach (DataRow attribute in elementDataTypes.Rows)
                                        {
                                            fieldDict.Add(attribute["COLUMN_NAME"].ToString(), attribute["DATA_TYPE"].ToString());
                                        }
                                    }
                                    else if (elementType == "User Defined Value")
                                    {
                                        fieldList.Append("''" + element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] + "'',");
                                    }

                                    fieldOrderedList.Add(element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"].ToString());
                                }


                                // Build the concatenated key
                                foreach (var busKey in fieldOrderedList)
                                {
                                    if (fieldDict.ContainsKey(busKey))
                                    {
                                        var key = "ISNULL([" + busKey + "], '')";

                                        if ((fieldDict[busKey] == "datetime2") || (fieldDict[busKey] == "datetime"))
                                        {
                                            key = "CASE WHEN [" + busKey + "] IS NOT NULL THEN CONVERT(" + stringDataType + "(100), [" + busKey + "], 112) ELSE '' END";
                                        }
                                        else if ((fieldDict[busKey] == "numeric") || (fieldDict[busKey] == "integer") || (fieldDict[busKey] == "int") || (fieldDict[busKey] == "tinyint") || (fieldDict[busKey] == "decimal"))
                                        {
                                            key = "CASE WHEN [" + busKey + "] IS NOT NULL THEN CAST([" + busKey + "] AS " + stringDataType + "(100)) ELSE '' END";
                                        }

                                        compositeKey.Append(key).Append(" + ");
                                    }
                                    else
                                    {
                                        var key = " " + busKey;
                                        compositeKey.Append(key).Append(" + ");
                                    }
                                }

                                hubQuerySelect.AppendLine(compositeKey.ToString().Substring(0, compositeKey.ToString().Length - 2) + " AS [" + hubKeyList.Rows[componentId]["COLUMN_NAME"] + "],");
                                hubQueryWhere.AppendLine(compositeKey.ToString().Substring(0, compositeKey.ToString().Length - 2) + " != '' AND");
                                hubQueryGroupBy.AppendLine(compositeKey.ToString().Substring(0, compositeKey.ToString().Length - 2) + ",");
                            }
                            else // Handle a component of a single or composite key 
                            {
                                foreach (DataRow element in elementList.Rows) // Only a single element...
                                {
                                    if (element["BUSINESS_KEY_COMPONENT_ELEMENT_TYPE"].ToString() == "User Defined Value")
                                    {
                                        firstKey = element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"].ToString();
                                        hubQuerySelect.AppendLine("    " + firstKey + " AS [" + hubKeyList.Rows[componentId]["COLUMN_NAME"] + "],");
                                    }
                                    else // It's a normal attribute
                                    {
                                        // We need the data type again
                                        sqlStatementForSourceQuery.AppendLine("SELECT COLUMN_NAME, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION");
                                        sqlStatementForSourceQuery.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS");
                                        sqlStatementForSourceQuery.AppendLine("WHERE TABLE_NAME= '" + stagingAreaTableName + "'");
                                        sqlStatementForSourceQuery.AppendLine("AND COLUMN_NAME = ('" + element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] + "')");

                                        firstKey = "[" + element["BUSINESS_KEY_COMPONENT_ELEMENT_VALUE"] + "]";

                                        var elementDataTypes = MyParent.GetDataTable(ref connStg, sqlStatementForSourceQuery.ToString());

                                        foreach (DataRow attribute in elementDataTypes.Rows)
                                        {
                                            if (attribute["DATA_TYPE"].ToString() == "numeric" || attribute["DATA_TYPE"].ToString() == "int")
                                            {
                                                hubQuerySelect.AppendLine("    CAST(" + firstKey + " AS " + stringDataType + "(100)) AS [" + hubKeyList.Rows[componentId]["COLUMN_NAME"] + "],");
                                            }
                                            else
                                            {
                                                hubQuerySelect.AppendLine("      " + firstKey + " AS [" + hubKeyList.Rows[componentId]["COLUMN_NAME"] + "],");
                                            }
                                        }

                                        hubQueryWhere.AppendLine(" " + firstKey + " IS NOT NULL AND");
                                        hubQueryGroupBy.AppendLine("    " + firstKey + ",");
                                    }

                                } // End of element loop (for single element)
                            }
                        }
                    } // End of component elements

                    hubQueryWhere.Remove(hubQueryWhere.Length - 6, 6);

                    queryRi.AppendLine("SELECT COUNT(*) AS RI_ISSUES, '" + satTableName + "'");
                        queryRi.AppendLine("FROM " + satTableName + " A");

                        if (radioButtonDeltaValidation.Checked) // Join to the Staging Area, on the business key
                        {
                            queryRi.AppendLine("JOIN ");
                            queryRi.AppendLine("(");
                            queryRi.AppendLine("  SELECT "); 
                            //Regular Hash
                            queryRi.AppendLine("    CONVERT(CHAR(32),HASHBYTES('MD5',");

                            foreach (DataRow hubKey in hubKeyList.Rows)
                            {
                                queryRi.AppendLine("      ISNULL(RTRIM(CONVERT(" + stringDataType + "(100),[" + (string)hubKey["COLUMN_NAME"] + "])),'NA')+'|'+");
                            }
                            queryRi.Remove(queryRi.Length - 3, 3);
                            queryRi.AppendLine();
                            queryRi.AppendLine("    ),2) AS " + hubSk);
                            queryRi.AppendLine("  FROM ");
                            queryRi.AppendLine("  (");
                            queryRi.AppendLine("    SELECT "+ hubQuerySelect + " FROM ["+ConfigurationSettings.StagingDatabaseName+ "].[dbo].["+stagingAreaTableName+"]");
                            queryRi.AppendLine("  ) stgsub");
                            queryRi.AppendLine(") staging ON ");
                            queryRi.AppendLine("A." + hubSk + " = staging." + hubSk);
                            // queryRI.AppendLine("JOIN " + stgTableName + " stg ON "+hubQuerySelect);
                        }
                        queryRi.AppendLine("LEFT OUTER JOIN " + hubTableName + " B on A." + hubSk + " = B." + hubSk);
                        queryRi.AppendLine("WHERE B." + hubSk + " IS NULL");
                        queryRi.AppendLine("--");
                        queryRi.AppendLine("UNION ALL");
                        queryRi.AppendLine("--");
                    }

                    queryRi.Remove(queryRi.Length - 15, 15);
                }


                // Link component
                var queryTableArrayLink = "SELECT DISTINCT LINK_TABLE_ID, LINK_TABLE_NAME FROM MD_LINK WHERE LINK_TABLE_NAME !='Not applicable'";

                var linkTables = MyParent.GetDataTable(ref connOmd, queryTableArrayLink);

                queryRi.AppendLine();
                queryRi.AppendLine("-- Link validation");
                queryRi.AppendLine();

                if (linkTables.Rows.Count == 0)
                {
                    richTextBoxOutput.Text += "There was no metadata available to create Link Referential Integrity scripts.";
                }
                else
                {
                    foreach (DataRow row in linkTables.Rows)
                    {
                        var linkTableName = (string)row["LINK_TABLE_NAME"];

                        queryRi.AppendLine("SELECT COUNT(*) AS RI_ISSUES, '" + linkTableName + "'");
                        queryRi.AppendLine("FROM " + linkTableName);

                        var queryHubArray = "SELECT DISTINCT b.HUB_TABLE_NAME " +
                                            "FROM MD_HUB_LINK_XREF a " +
                                            "JOIN MD_HUB b ON a.HUB_TABLE_ID=b.HUB_TABLE_ID " +
                                            "WHERE a.LINK_TABLE_ID = " + (int)row["LINK_TABLE_ID"];

                        var hubTables = MyParent.GetDataTable(ref connOmd, queryHubArray);
                        foreach (DataRow hubRow in hubTables.Rows)
                        {
                            var hubTableName = (string)hubRow["HUB_TABLE_NAME"];
                            var hubSk = hubTableName.Substring(4) + "_" + ConfigurationSettings.DwhKeyIdentifier;
                            queryRi.AppendLine("LEFT OUTER JOIN " + hubTableName + " on " + linkTableName + "." + hubSk +
                                               " = " + hubTableName + "." + hubSk);
                        }

                        queryRi.AppendLine("WHERE (");
                        foreach (DataRow hubRow in hubTables.Rows)
                        {
                            var hubTableName = (string)hubRow["HUB_TABLE_NAME"];
                            var hubSk = hubTableName.Substring(4) + "_" + ConfigurationSettings.DwhKeyIdentifier;
                            queryRi.AppendLine("  " + hubTableName + "." + hubSk + " IS NULL OR");
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
                    queryTableArrayLinkSat.AppendLine("  [STAGING_AREA_TABLE_ID]");
                    queryTableArrayLinkSat.AppendLine(" ,[STAGING_AREA_TABLE_NAME]");
                    queryTableArrayLinkSat.AppendLine(" ,[STAGING_AREA_SCHEMA_NAME]");
                    queryTableArrayLinkSat.AppendLine(" ,[FILTER_CRITERIA]");
                    queryTableArrayLinkSat.AppendLine(" ,[SATELLITE_TABLE_ID]");
                    queryTableArrayLinkSat.AppendLine(" ,[SATELLITE_TABLE_NAME]");
                    queryTableArrayLinkSat.AppendLine(" ,[SATELLITE_TYPE]");
                    queryTableArrayLinkSat.AppendLine(" ,[HUB_TABLE_ID]");
                    queryTableArrayLinkSat.AppendLine(" ,[HUB_TABLE_NAME]");
                    queryTableArrayLinkSat.AppendLine(" ,[BUSINESS_KEY_DEFINITION]");
                    queryTableArrayLinkSat.AppendLine(" ,[LINK_TABLE_ID]");
                    queryTableArrayLinkSat.AppendLine(" ,[LINK_TABLE_NAME]");
                    queryTableArrayLinkSat.AppendLine("FROM[interface].[INTERFACE_STAGING_SATELLITE_XREF]");
                    queryTableArrayLinkSat.AppendLine("WHERE [SATELLITE_TYPE]='Link Satellite'");

                    var linkSatTables = MyParent.GetDataTable(ref connOmd, queryTableArrayLinkSat.ToString());

                    if (satTables.Rows.Count == 0)
                    {
                        richTextBoxOutput.Text += "There was no metadata available to create Link Satellite Referential Integrity scripts.";
                    }
                    else
                    {
                        foreach (DataRow row in linkSatTables.Rows)
                        {
                            var lsatTableName = (string)row["SATELLITE_TABLE_NAME"];
                            var linkTableName = (string)row["LINK_TABLE_NAME"];
                            var hubSk = linkTableName.Substring(4) + "_" + ConfigurationSettings.DwhKeyIdentifier;

                            queryRi.AppendLine("SELECT COUNT(*) AS RI_ISSUES, '" + lsatTableName + "'");
                            queryRi.AppendLine("FROM " + lsatTableName + " A");
                            queryRi.AppendLine("LEFT OUTER JOIN " + linkTableName + " B on A." + hubSk + " = B." + hubSk);
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
