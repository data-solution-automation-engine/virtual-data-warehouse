using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Virtual_Data_Warehouse;

namespace Virtual_Data_Warehouse
{
    public partial class FormDimensional : FormBase
    {
       // private readonly FormMain _myParent;

        public FormDimensional(FormMain parent) : base(parent)
        {
            MyParent = parent;
            InitializeComponent();

            PopulateHubRadioButtonList();
        }

        private void ButtonHubSelection(object sender, EventArgs e)
        {
            PopulateHubRadioButtonList();
        }


        private void PopulateHubRadioButtonList()
        {
            richTextBoxInformation.Clear();

            var conn = new SqlConnection
            {
                ConnectionString =
                    radioButtonPSA.Checked ? TeamConfigurationSettings.ConnectionStringHstg : TeamConfigurationSettings.ConnectionStringInt
            };

            var hubIdentifier = TeamConfigurationSettings.HubTablePrefixValue;

            if (TeamConfigurationSettings.TableNamingLocation == "Prefix")
            {
                hubIdentifier = string.Concat(hubIdentifier, "_%");
            }
            else if (TeamConfigurationSettings.TableNamingLocation == "Suffix")
            {
                hubIdentifier = string.Concat("%_", hubIdentifier);
            }
            else
            {
                MessageBox.Show("Something is not right - a prefix / suffix for the table names should always be checked.",
                    "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            var queryHubs = @"SELECT TABLE_NAME as HUB_NAME " +
               "FROM INFORMATION_SCHEMA.TABLES " +
                            "WHERE TABLE_NAME like '" + hubIdentifier + @"' " +
                            "ORDER BY 1";
            conn.Open();

            var hubQuery = new SqlCommand(queryHubs, conn);
            var queryCommandReaderSource = hubQuery.ExecuteReader();

            // Load the results of the query in a DataTable for display later (in a grid).
            var dataTableHubs = new DataTable();
            dataTableHubs.Load(queryCommandReaderSource);

            queryCommandReaderSource.Dispose();
            hubQuery.Dispose();

            if (dataTableHubs.Rows.Count == 0)
            {
                richTextBoxInformation.Text +=
                    "There was no metadata available to display Hub content. Please check the metadata schema (are there any Hubs available?) or the database connection.";
            }

            // Dynamically create the radio buttons
            var dynamicRadioButton = new RadioButton[dataTableHubs.Rows.Count];
                  
            var hubCounter = 0;
            foreach (DataRow row in dataTableHubs.Rows)
            {
                dynamicRadioButton[hubCounter] = new RadioButton();
                dynamicRadioButton[hubCounter].CheckedChanged += RadioButtonCheckedChanged;
                dynamicRadioButton[hubCounter].Text = row["HUB_NAME"].ToString();
                dynamicRadioButton[hubCounter].Location = new System.Drawing.Point(10, 10 + hubCounter*20);
                dynamicRadioButton[hubCounter].Width = 500;
                tabHub.Controls.Add(dynamicRadioButton[hubCounter]);
                hubCounter++;
            }
        }

        // Event handler for dynamically generated radio buttons
        private void RadioButtonCheckedChanged(object sender, EventArgs e)
        {
            // Inform the user (base Hub)
            var tempRadioButton = (RadioButton)sender;
            if (!tempRadioButton.Checked) return;
            labelFirstHub.Visible = true;
            pictureBoxCheck.Visible = true;

            labelFirstHub.Text = tempRadioButton.Text;

            // Inform the user (pathway)
            pictureBoxPathway.Visible = false;
            labelPathway.Visible = false;


            // Populate the attribute data grid
            GetAttributes("'"+tempRadioButton.Text+"'");
        }

        private void GetAttributes(string hubList)
        {
            var conn = new SqlConnection
            {
                ConnectionString =
                    radioButtonPSA.Checked ? TeamConfigurationSettings.ConnectionStringHstg : TeamConfigurationSettings.ConnectionStringInt
            };

            var keyIdentifier = TeamConfigurationSettings.DwhKeyIdentifier;
            if (TeamConfigurationSettings.KeyNamingLocation == "Prefix")//_myParent.keyPrefixRadiobutton.Checked)
            {
                keyIdentifier = string.Concat(keyIdentifier, "_%");
            }
            else if (TeamConfigurationSettings.KeyNamingLocation == "Suffix")
            {
                keyIdentifier = string.Concat("%_", keyIdentifier);
            }
            else
            {
                MessageBox.Show("Something is not right - a prefix / suffix for the key attribute should always be checked.","Warning!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            // Retrieve the prefix/suffix settings for the tables (Hubs, Links, Satellites)
            var linkIdentifier = TeamConfigurationSettings.LinkTablePrefixValue;
            var linkSatIdentifier = TeamConfigurationSettings.LinkTablePrefixValue;
            var satIdentifier = TeamConfigurationSettings.SatTablePrefixValue;

            if (TeamConfigurationSettings.TableNamingLocation == "Prefix")//_myParent.tablePrefixRadiobutton.Checked)
            {
                linkIdentifier = string.Concat(linkIdentifier, "_%");
                linkSatIdentifier = string.Concat(linkSatIdentifier, "_%");
                satIdentifier = string.Concat(satIdentifier, "_%");
            }
            else if (TeamConfigurationSettings.TableNamingLocation == "Suffix")
            {
                linkIdentifier = string.Concat("%_", linkIdentifier);
                linkSatIdentifier = string.Concat("%_", linkSatIdentifier);
                satIdentifier = string.Concat("%_", satIdentifier);
            }
            else
            {
                MessageBox.Show("Something is not right - a prefix / suffix for the table names should always be checked.","Warning!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            // Catch if no Hub was selected
            if (hubList == "")
            {
                hubList = "'Unknown'";
            }

            var queryAttributes = new StringBuilder();

            // Create the list of attributes to select / survive
            queryAttributes.AppendLine("SELECT ");
            queryAttributes.AppendLine(" TABLE_NAME,");
            queryAttributes.AppendLine(" COLUMN_NAME ");
            queryAttributes.AppendLine("FROM INFORMATION_SCHEMA.COLUMNS");
            queryAttributes.AppendLine("WHERE TABLE_NAME IN ");
            queryAttributes.AppendLine("   (");
            queryAttributes.AppendLine("      SELECT TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS ");
            queryAttributes.AppendLine("      WHERE COLUMN_NAME IN ");
            queryAttributes.AppendLine("        (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS ");
            queryAttributes.AppendLine("         WHERE TABLE_NAME IN (" + hubList + @") ");
            queryAttributes.AppendLine(("          AND COLUMN_NAME LIKE '" + keyIdentifier + @"') "));
            queryAttributes.AppendLine("           AND (TABLE_NAME LIKE '" + satIdentifier + @"' OR TABLE_NAME LIKE '" +linkIdentifier + @"') ");
            queryAttributes.AppendLine("      UNION ");
            queryAttributes.AppendLine("      SELECT TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS ");
            queryAttributes.AppendLine("      WHERE COLUMN_NAME IN ");
            queryAttributes.AppendLine("        (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS ");
            queryAttributes.AppendLine("         WHERE TABLE_NAME IN ");
            queryAttributes.AppendLine("           (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS ");
            queryAttributes.AppendLine("            WHERE COLUMN_NAME IN ");
            queryAttributes.AppendLine("               (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS ");
            queryAttributes.AppendLine("                WHERE TABLE_NAME IN (" + hubList + @") ");
            queryAttributes.AppendLine("                AND COLUMN_NAME LIKE '" + keyIdentifier + @"') ");
            queryAttributes.AppendLine("            AND TABLE_NAME LIKE '" + linkIdentifier + @"') ");
            queryAttributes.AppendLine("         AND COLUMN_NAME LIKE '" + keyIdentifier + @"') ");
            queryAttributes.AppendLine("      AND TABLE_NAME LIKE '" + linkSatIdentifier + @"' ");
            queryAttributes.AppendLine("   )");
            queryAttributes.AppendLine("AND COLUMN_NAME not like '" + keyIdentifier + @"'  ");
            queryAttributes.AppendLine("ORDER BY 1,2  ");

            conn.Open();

            var satQuery = new SqlCommand(queryAttributes.ToString(), conn);
            var queryCommandReaderSource = satQuery.ExecuteReader();

            // Load the results of the query in a DataTable for display later (in a grid).
            var dataTableSats = new DataTable();
            dataTableSats.Load(queryCommandReaderSource);

            queryCommandReaderSource.Dispose();
            dataTableSats.Dispose();
            dataGridAttributes.Rows.Clear();

            if (checkBoxHideSystemAttributes.Checked)
            {
                foreach (DataRow row in dataTableSats.Rows)
                {
                    var attributeName = row["COLUMN_NAME"].ToString();

                    if (
                        attributeName == TeamConfigurationSettings.RecordSourceAttribute || 
                        attributeName == TeamConfigurationSettings.AlternativeRecordSourceAttribute ||
                        attributeName == TeamConfigurationSettings.RowIdAttribute ||
                        attributeName == TeamConfigurationSettings.RecordChecksumAttribute ||
                        attributeName == TeamConfigurationSettings.AlternativeLoadDateTimeAttribute ||
                        attributeName == TeamConfigurationSettings.EtlProcessAttribute ||
                        attributeName == TeamConfigurationSettings.LoadDateTimeAttribute ||
                        attributeName == TeamConfigurationSettings.CurrentRowAttribute ||
                        attributeName == TeamConfigurationSettings.EtlProcessUpdateAttribute ||
                        attributeName == TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute ||
                        attributeName == TeamConfigurationSettings.ExpiryDateTimeAttribute
                       )
                    {
                        row.Delete();
                    }
                }
                dataTableSats.AcceptChanges();
            }

            if (dataGridAttributes.Rows.Count == 0)
            {
                richTextBoxInformation.Text = "There were no attributes to select";
            }
            else
            {
                foreach (DataRow row in dataTableSats.Rows)
                {
                    dataGridAttributes.Rows.Add(row["TABLE_NAME"].ToString(), row["COLUMN_NAME"].ToString());
                }
            }

            GridAutoLayout();

            //Sort the data grid
            dataGridAttributes.Sort(dataGridAttributes.Columns[0],ListSortDirection.Ascending);
            
            //Populate Pathways
            // Clear the existing checkboxes
            checkedListBoxPathways.Items.Clear();

            try
            {
                // Query the potential paths
                var queryMetadata = new StringBuilder();

                queryMetadata.AppendLine("WITH HUBS AS ");
                queryMetadata.AppendLine("(");
                queryMetadata.AppendLine("  SELECT TABLE_NAME, COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME LIKE '%_SK' AND TABLE_NAME LIKE 'HUB_%'");
                queryMetadata.AppendLine(")");
                queryMetadata.AppendLine(", LNKS AS");
                queryMetadata.AppendLine("(");
                queryMetadata.AppendLine("  SELECT TABLE_NAME, COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME LIKE '%_SK' AND TABLE_NAME LIKE 'LNK_%' AND TABLE_NAME NOT LIKE 'LSAT_%'");
                queryMetadata.AppendLine("), DV_INFO_BASE AS ");
                queryMetadata.AppendLine("(");
                queryMetadata.AppendLine("  SELECT ");
                queryMetadata.AppendLine("    'Add '+HUBS_END.TABLE_NAME+' (via '+LNKS_ALL.TABLE_NAME+')' AS JOIN_DIRECTION,");
                queryMetadata.AppendLine("	HUBS.TABLE_NAME + ' --> ' + LNKS_ALL.TABLE_NAME + ' --> ' + HUBS_END.TABLE_NAME as JOIN_PATHWAY, ");
                queryMetadata.AppendLine("	HUBS.TABLE_NAME AS HUB_TABLE_SOURCE,");
                queryMetadata.AppendLine("	HUBS.COLUMN_NAME AS HUB_KEY,");
                queryMetadata.AppendLine("	LNKS_ALL.TABLE_NAME AS LNK_TABLE,");
                queryMetadata.AppendLine("	LNKS_ALL.COLUMN_NAME AS LNK_KEY,");
                queryMetadata.AppendLine("	HUBS_END.TABLE_NAME AS HUB_TABLE_TARGET");
                queryMetadata.AppendLine("  FROM HUBS");
                queryMetadata.AppendLine("	INNER JOIN LNKS ON HUBS.COLUMN_NAME=LNKS.COLUMN_NAME");
                queryMetadata.AppendLine("	INNER JOIN LNKS LNKS_ALL ON LNKS.TABLE_NAME=LNKS_ALL.TABLE_NAME");
                queryMetadata.AppendLine("	INNER JOIN HUBS HUBS_END ON HUBS_END.COLUMN_NAME=LNKS_ALL.COLUMN_NAME and HUBS_END.TABLE_NAME!=HUBS.TABLE_NAME");
                queryMetadata.AppendLine(")");
                queryMetadata.AppendLine("SELECT ");
                queryMetadata.AppendLine("	JOIN_DIRECTION,");
                queryMetadata.AppendLine("	JOIN_PATHWAY,");
                queryMetadata.AppendLine("	HUB_TABLE_SOURCE,");
                queryMetadata.AppendLine("	HUB_KEY,");
                queryMetadata.AppendLine("	LNK_TABLE,");
                queryMetadata.AppendLine("	LNK_KEY,");
                queryMetadata.AppendLine("	HUB_TABLE_TARGET");
                queryMetadata.AppendLine("FROM DV_INFO_BASE");
                queryMetadata.AppendLine("WHERE HUB_TABLE_SOURCE IN ("+hubList+")");
                queryMetadata.AppendLine("ORDER BY 1");

                var pathWaytables = Utility.GetDataTable(ref conn, queryMetadata.ToString());

                if (pathWaytables.Rows.Count == 0)
                {
                    richTextBoxInformation.Text = "There were no pathways to detect for this Hub.";
                }

                foreach (DataRow row in pathWaytables.Rows)
                {
                    checkedListBoxPathways.Items.Add(row["JOIN_DIRECTION"]);
                }
            }
            catch (Exception ex)
            {
                richTextBoxInformation.Text += "An issue was encountered: "+ex;
                // MessageBox.Show("There is no database connection! Please check the details in the information pane.","An issue has been encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GridAutoLayout()
        {
            for (var i = 0; i < dataGridAttributes.Columns.Count - 1; i++)
            {
                var column = dataGridAttributes.Columns[i];
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            }
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {

        }

        private void checkedListBoxPathways_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkedListBoxPathways.CheckedItems.Count != 0)
            {
                var labelString = new StringBuilder();

                pictureBoxPathway.Visible = true;
                labelPathway.Visible = true;
                labelPathway.Text = "";
                var checkBoxCounter = 0;

                // Create hubList for attribute rebuild later on
                var hubList = new StringBuilder();
                hubList.AppendLine("'" + labelFirstHub +"'");

                foreach (object item in checkedListBoxPathways.Items)
                {
                   var hubTableName = checkedListBoxPathways.Items[checkBoxCounter].ToString();
                   hubTableName = hubTableName.Split(' ').Skip(1).FirstOrDefault();

                   if (checkedListBoxPathways.CheckedItems.Contains(item))
                   {
                       // Add stuff
                       labelString.Append("--> " + hubTableName +" ");

                       // Rebuild Datatable with attributes
                       hubList.AppendLine(",'" + hubTableName + "'");
                   }

                  if (!checkedListBoxPathways.CheckedItems.Contains(item))
                  {
                    // Remove stuff
                      labelString.Replace(" --> " + hubTableName +" ", string.Empty);
                  }

                  checkBoxCounter++;
                }

                labelPathway.Text = labelString.ToString();
                GetAttributes(hubList.ToString());
            }
            else
            {
                richTextBoxInformation.Text += "There was no metadata selected to add more Hubs. Are there any Hubs selected?";
                pictureBoxPathway.Visible = false;
                labelPathway.Visible = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAllAttributes.Checked)
            {
                foreach (DataGridViewRow row in dataGridAttributes.Rows)
                {
                    row.Cells[2].Value = true;
                }
            }
            else
            {
                foreach (DataGridViewRow row in dataGridAttributes.Rows)
                {
                    row.Cells[2].Value = false;
                }
            }
        }

    }
}
