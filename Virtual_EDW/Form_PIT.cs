using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace Virtual_EDW
{
    public partial class FormPit : FormBase
    {
        //private readonly FormMain _myParent;

        public FormPit(FormMain parent)
        {
            MyParent = parent;
            InitializeComponent();

            PopulateHubRadioButtonList();


            InitializeComboBox();

            groupBoxSingleSnapshot.Enabled = false;
            groupBoxRangeSnapshot.Enabled = false;

            groupBoxFrequencyCondensing.Enabled = false;
            groupBoxContinuousCondensing.Enabled = false;

        }

        private void InitializeComboBox()
        {
            var configurationSettings = new ConfigurationSettings();

            //Populate the dropdown combobox for time selection in PIT
            comboBoxTimePerspective.Items.Add(configurationSettings.LoadDateTimeAttribute);

            comboBoxTimePerspective.SelectedIndex = 0;
            comboBoxChangeCondensing.SelectedIndex = 0;
            comboBoxSnapshotRange.SelectedIndex = 0;
        }

        private void ButtonHubSelection(object sender, EventArgs e)
        {
            tabControl.SelectTab(0);
            PopulateHubRadioButtonList();
        }


        private void PopulateHubRadioButtonList()
        {
            richTextBoxInformation.Clear();

            var configurationSettings = new ConfigurationSettings();

            var conn = new SqlConnection
            {
                ConnectionString =
                    radioButtonPSA.Checked ? configurationSettings.ConnectionStringHstg : configurationSettings.ConnectionStringInt
            };

            var hubIdentifier = configurationSettings.HubTablePrefixValue;

            if (configurationSettings.TableNamingLocation == "Prefix")
            {
                hubIdentifier = string.Concat(hubIdentifier, "_%"); // E.g. HSH_%, a prefix
            }
            else if (configurationSettings.TableNamingLocation == "Suffix")
            {
                hubIdentifier = string.Concat("%_", hubIdentifier); // E.g. %_HSH, a suffix
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
                var environmentSelection = radioButtonPSA.Checked ? "Persistent Staging Area (virtual Data Vault model)" : "Integration Layer (physical Data Vault model)";
                richTextBoxInformation.Text +=
                    "There was no metadata available to display Hub content. Please check the metadata schema (are there any Hubs available?) or the database connection. The "+ environmentSelection + " is currently selected";
            }

            // Dynamically create the radio buttons
            var dynamicRadioButton = new RadioButton[dataTableHubs.Rows.Count];
                  
            var hubCounter = 0;
            foreach (DataRow row in dataTableHubs.Rows)
            {
                dynamicRadioButton[hubCounter] = new RadioButton();
                dynamicRadioButton[hubCounter].CheckedChanged += RadioButtonCheckedChanged;
                dynamicRadioButton[hubCounter].Text = row["HUB_NAME"].ToString();
                dynamicRadioButton[hubCounter].Location = new Point(10, 10 + hubCounter*20);
                dynamicRadioButton[hubCounter].Width = 500;
                tabHub.Controls.Add(dynamicRadioButton[hubCounter]);
                hubCounter++;
            }
        }

        // Event handler for dynamically generated radio buttons
        private void RadioButtonCheckedChanged(object sender, EventArgs e)
        {
            // Clear out previously generated output
            richTextBoxOutput.Clear();

            // Inform the user (base Hub)
            var tempRadioButton = (RadioButton)sender;
            if (!tempRadioButton.Checked) return;
            labelFirstHub.Visible = true;
            pictureBoxCheck.Visible = true;

            labelFirstHub.Text = tempRadioButton.Text;

            // Populate the attribute data grid
            GetAttributes("'"+tempRadioButton.Text+"'");
        }

        private void GetAttributes(string hubList)
        {
            var configurationSettings = new ConfigurationSettings();

            // Set connection depending on target area
            var conn = new SqlConnection
            {
                ConnectionString = radioButtonPSA.Checked ? configurationSettings.ConnectionStringHstg : configurationSettings.ConnectionStringInt
            };

            // Retrieve the location of the key indicator (suffix or prefix)
            var keyIdentifier = configurationSettings.DwhKeyIdentifier;
            var suffixLocation ="Unknown";
            var loadDateTimeStamp = configurationSettings.LoadDateTimeAttribute;

            var recordSource = configurationSettings.EnableAlternativeRecordSourceAttribute == "True" ? configurationSettings.AlternativeRecordSourceAttribute : configurationSettings.RecordSourceAttribute;

            var etlProcessId = configurationSettings.EtlProcessAttribute;

            if (configurationSettings.KeyNamingLocation == "Prefix")
            {
                keyIdentifier = string.Concat(keyIdentifier, "_%"); // E.g. HSH_%, a prefix
                suffixLocation = "prefix";
            }
            else if (configurationSettings.KeyNamingLocation == "Suffix")
            {
                keyIdentifier = string.Concat("%_", keyIdentifier); // E.g. %_HSH, a suffix
                suffixLocation = "suffix";
            }
            else
            {
                MessageBox.Show("Something is not right - a prefix / suffix for the key attribute should always be checked.","Warning!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            // Retrieve the prefix/suffix settings for the tables (Hubs, Links, Sats)
            var linkIdentifier = configurationSettings.LinkTablePrefixValue;
            var linkSatIdentifier = configurationSettings.LinkTablePrefixValue;
            var satIdentifier = configurationSettings.SatTablePrefixValue;

            if (configurationSettings.TableNamingLocation == "Prefix")
            {
                linkIdentifier = string.Concat(linkIdentifier, "_%");
                linkSatIdentifier = string.Concat(linkSatIdentifier, "_%");
                satIdentifier = string.Concat(satIdentifier, "_%");
            }
            else if (configurationSettings.TableNamingLocation == "Suffix")
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
            //queryAttributes.AppendLine("AND COLUMN_NAME not like '" + keyIdentifier + @"'  ");

            // Don't load Satellites
            if (checkBoxSuppressSat.Checked)
            {
                queryAttributes.AppendLine("  AND TABLE_NAME NOT LIKE  '" + satIdentifier + "'");
            }

            // Don't load links
            if (checkBoxSuppressLink.Checked)
            {
                queryAttributes.AppendLine("  AND TABLE_NAME NOT LIKE  '" + linkIdentifier + "'-- AND TABLE_NAME NOT LIKE '"+ linkSatIdentifier +"'");
            }
            queryAttributes.AppendLine("  AND TABLE_NAME NOT LIKE '" + linkSatIdentifier + "'");
            queryAttributes.AppendLine("UNION");
            queryAttributes.AppendLine("SELECT TABLE_NAME, COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS ");
            queryAttributes.AppendLine("WHERE TABLE_NAME IN (" + hubList + @") ");
            //queryAttributes.AppendLine(("AND COLUMN_NAME LIKE '" + keyIdentifier + @"'"));
            queryAttributes.AppendLine("AND COLUMN_NAME NOT IN ('"+ loadDateTimeStamp + "', 'ROW_NR', '"+ recordSource + "', '"+ etlProcessId + "')");

            queryAttributes.AppendLine("ORDER BY 1,2  ");


            // Create the DataTable for later drawing into the GridView
            conn.Open();
            var attributeSelection = GetDataTable(ref conn, queryAttributes.ToString());

            // Create the Hub Primary Key / Surrogate Key from the above information so it can be automatically included
            var hubSk = "Unknown";
            foreach (DataRow attribute in attributeSelection.Rows)
            {
                if ("'"+attribute["TABLE_NAME"]+"'" == hubList && attribute["COLUMN_NAME"].ToString().Contains(configurationSettings.DwhKeyIdentifier))
                {
                    hubSk=attribute["COLUMN_NAME"].ToString();
                }
            }

            // Create an array of attributes to check by default
            string[] defaultAttributes =
            {
                //_myParent.configurationSettings.AlternativeLoadDateTimeAttribute,
                configurationSettings.LoadDateTimeAttribute,
                configurationSettings.AlternativeSatelliteLoadDateTimeAttribute,
                //_myParent.configurationSettings.ExpiryDateTimeAttribute,
                hubSk
            };

            // Create an array of process attributes so these can be selected/checked easily later
            string[] systemAttributes =
            {
                configurationSettings.RecordSourceAttribute,
                configurationSettings.RowIdAttribute,
                configurationSettings.RecordChecksumAttribute,
                configurationSettings.EtlProcessAttribute,
                configurationSettings.CurrentRowAttribute,
                configurationSettings.EtlProcessUpdateAttribute,
                configurationSettings.AlternativeRecordSourceAttribute,
                "ROW_NR",
                "ROW_NUMBER"
            };

            // Create an Array / List of the normal attributes
            List<string> regularAttributes = new List<string>();
            foreach (DataRow attribute in attributeSelection.Rows)
            {
                if (!systemAttributes.Contains(attribute["COLUMN_NAME"].ToString()))
                {
                    regularAttributes.Add(attribute["COLUMN_NAME"].ToString());
                }
            }
            regularAttributes.ToArray();


            // Remove system attributes from Datatable (if selected)

            foreach (DataRow row in attributeSelection.Rows)
            {
                var rowAttribute = row["COLUMN_NAME"].ToString();
                // Remove system attributes - if selected
                if (checkBoxHideSystemAttributes.Checked)
                {
                    if (systemAttributes.Contains(rowAttribute) && !defaultAttributes.Contains(rowAttribute))
                    {
                        row.Delete();
                    }
                }

                // Remove regular attributes - if selected
                if (checkBoxSupressAttributes.Checked)
                {
                    if (regularAttributes.Contains(rowAttribute) && !defaultAttributes.Contains(rowAttribute))
                    {
                        row.Delete();
                    }
                }

            }
            attributeSelection.AcceptChanges();



            //Draw the GridView
            dataGridAttributes.Rows.Clear();
            dataGridAttributes.Columns.Clear();

            // Set the column header names.
            //dataGridAttributes.ReadOnly = true;
            dataGridAttributes.AutoGenerateColumns = false;
            dataGridAttributes.ColumnHeadersVisible = true;
            dataGridAttributes.AllowUserToAddRows = false;

            dataGridAttributes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Table Name" });
            dataGridAttributes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Column Name" });
            dataGridAttributes.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Include" });
            
            dataGridAttributes.Columns[0].ReadOnly = true;
            dataGridAttributes.Columns[1].ReadOnly = true;
            dataGridAttributes.Columns[2].ReadOnly = false;

            foreach (DataRow attribute in attributeSelection.Rows)
            {
                dataGridAttributes.Rows.Add(
                    attribute["TABLE_NAME"],
                    attribute["COLUMN_NAME"],
                    false
                );
            }

            GridAutoLayout();

            if (dataGridAttributes.Rows.Count == 0)
            {
                var environmentSelection = radioButtonPSA.Checked ? "Persistent Staging Area (virtual Data Vault model)" : "Integration Layer (physical Data Vault model)";

                richTextBoxInformation.Text = "There were no attributes to select! This is usually because there are issues with the prefixes, or because the wrong environment is selected. The selected environment is the "+environmentSelection+".\r\n\r\n";
                richTextBoxInformation.Text += "The key can be identified with "+keyIdentifier+" and is defined as a "+suffixLocation+".";
            }

            //Sort the data grid
            dataGridAttributes.Sort(dataGridAttributes.Columns[0],ListSortDirection.Ascending);


            // Check attributes for defaults
            foreach (DataGridViewRow row in dataGridAttributes.Rows)
            {
                if (defaultAttributes.Contains(row.Cells[1].Value))
                {
                    row.Cells[2].Value = true;
                }

                //Make sure the Hub Key cannot be removed from the selection
            //    if (defaultAttributes.Contains(row.Cells[1].Value))
            //    {
            //        row.Cells[2].Value = true;
           //     }

                if ("'"+row.Cells[0].Value+"'" == hubList && row.Cells[1].Value.ToString().Contains(configurationSettings.DwhKeyIdentifier))
                {
                    row.Cells[2].ReadOnly = true;
                    row.Cells[2].Style.BackColor = Color.LightGray;
                    row.Cells[2].Style.ForeColor = Color.DarkGray;
                }
            }
            
            attributeSelection.Dispose();
        }

        private void GridAutoLayout()
        {
            //Set the autosize based on all cells for each column
            for (var i = 0; i < dataGridAttributes.Columns.Count - 1; i++)
            {
                dataGridAttributes.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            if (dataGridAttributes.Columns.Count > 0)
            {
                dataGridAttributes.Columns[dataGridAttributes.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            // Disable the auto size again (to enable manual resizing)
            for (var i = 0; i < dataGridAttributes.Columns.Count - 1; i++)
            {
                int columnWidth = dataGridAttributes.Columns[i].Width;
                dataGridAttributes.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dataGridAttributes.Columns[i].Width = columnWidth;
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

        private void checkBoxSupressAttributes_CheckedChanged(object sender, EventArgs e)
        {
            richTextBoxInformation.Clear();
            GetAttributes("'"+labelFirstHub.Text+"'");
        }

        private void checkBoxHideSystemAttributes_CheckedChanged(object sender, EventArgs e)
        {
            richTextBoxInformation.Clear();
            GetAttributes("'" + labelFirstHub.Text + "'");
        }

        private void butGenerate_Click(object sender, EventArgs e)
        {
            var configurationSettings = new ConfigurationSettings();

            // Build arrays - list of each table in and its columns
            var tableList = new SortedList<string, List<string>>();

            // Create a counter to see if anything is checked at all
            var checkedCounter = 0;

            foreach (DataGridViewRow row in dataGridAttributes.Rows)
            {
                var checkedAttribute = row.Cells[2] as DataGridViewCheckBoxCell;
                if ((bool)checkedAttribute.Value == true) // Only take checked attributes into account
                {
                    checkedCounter++;
                    string integrationTable = row.Cells[0].Value.ToString();
                    string integrationAttribute = row.Cells[1].Value.ToString();

                    richTextBoxInformation.Text += integrationTable + " " + integrationAttribute +"\r\n";

                    if (tableList.ContainsKey((integrationTable)))
                    {
                        List<string> columnList;
                        tableList.TryGetValue(integrationTable, out columnList);

                        if (columnList != null)
                        {
                            columnList.Add(integrationAttribute);
                        }
                    }
                    else
                    {
                        List<string> columnList = new List<string>();
                        columnList.Add(integrationAttribute);
                        tableList.Add(integrationTable,columnList);
                    }
                }
            }

            // Understand what the effective dates are for the Hub
            var hubEffectiveDate = "Unknown";
            if (configurationSettings.EnableAlternativeLoadDateTimeAttribute == "True")
            {
                hubEffectiveDate = configurationSettings.AlternativeLoadDateTimeAttribute;
            }
            else
            {
                hubEffectiveDate = configurationSettings.LoadDateTimeAttribute;
            }

            // Understand what the effective dates are for the Sat
            var satEffectiveDate = "Unknown";
            satEffectiveDate = configurationSettings.EnableAlternativeSatelliteLoadDateTimeAttribute == "True" ? configurationSettings.AlternativeSatelliteLoadDateTimeAttribute : configurationSettings.LoadDateTimeAttribute;

            // Build up Hub, Sat and Link identifiers
            var linkIdentifier = configurationSettings.LinkTablePrefixValue;
            var linkSatIdentifier = configurationSettings.LinkTablePrefixValue;
            var satIdentifier = configurationSettings.SatTablePrefixValue;
            var hubIdentifier = configurationSettings.HubTablePrefixValue;

            if (configurationSettings.TableNamingLocation == "Prefix")
            {
                linkIdentifier = string.Concat(linkIdentifier, "_");
                linkSatIdentifier = string.Concat(linkSatIdentifier, "_");
                satIdentifier = string.Concat(satIdentifier, "_");
                hubIdentifier = string.Concat(hubIdentifier, "_");
            }
            else if (configurationSettings.TableNamingLocation == "Suffix")
            {
                linkIdentifier = string.Concat("_", linkIdentifier);
                linkSatIdentifier = string.Concat("_", linkSatIdentifier);
                satIdentifier = string.Concat("_", satIdentifier);
                hubIdentifier = string.Concat(hubIdentifier, "_");
            }

            // Grab the Hub table and key from the Hub
            var hubTable = "Unknown";
            var hubKey = "Unknown";
            foreach (DataGridViewRow row in dataGridAttributes.Rows)
            {
                string integrationTable = row.Cells[0].Value.ToString();
                string integrationAttribute = row.Cells[1].Value.ToString();

                if (integrationTable.Contains(hubIdentifier))
                {
                    hubTable = integrationTable;
                    if (integrationAttribute.Contains(configurationSettings.DwhKeyIdentifier))
                    hubKey = integrationAttribute;
                }             
                        
            }

            // Build the query  
            var outputQuery = new StringBuilder();

            var satLoadDateTime = "";
            if (configurationSettings.EnableAlternativeSatelliteLoadDateTimeAttribute == "True")
            {
                satLoadDateTime = configurationSettings.AlternativeSatelliteLoadDateTimeAttribute;
            }
            else
            {
                satLoadDateTime = configurationSettings.LoadDateTimeAttribute;
            }

            outputQuery.AppendLine("SELECT");
            //outputQuery.AppendLine("  "+hubTable+"."+hubKey+",");

            if (radioButtonSingleDateSnapshot.Checked)
            {
                outputQuery.AppendLine("  [PIT_EFFECTIVE_DATETIME] AS SNAPSHOT_DATETIME,");
            }
            else
            {
                outputQuery.AppendLine("  [PIT_EFFECTIVE_DATETIME],");
            }

            if (checkBoxPITExpiryDate.Checked)
            {
                outputQuery.AppendLine("  LEAD(PIT_EFFECTIVE_DATETIME,1,'9999-12-31') OVER (PARTITION BY  [" + hubTable +
                                       "." + hubKey + "] ORDER BY PIT_EFFECTIVE_DATETIME ASC) AS PIT_EXPIRY_DATETIME,");
            }
            foreach (var tableName in tableList.Keys)
            {
                var columnNames = new List<string>();
                tableList.TryGetValue(tableName, out columnNames); // Get the list with the attribute names for the table

                if (columnNames == null) continue;
                foreach (var columnName in columnNames)
                {
                    if (checkBoxInheritZeroSatelliteKey.Checked)
                    {
                        if (columnName == hubKey)
                        {
                            outputQuery.AppendLine("  COALESCE([" + tableName + "." + columnName +
                                                   "],'00000000000000000000000000000000') AS [" + tableName + "." +
                                                   columnName + "],");
                        }
                        else
                        {
                            if (columnName == satLoadDateTime)
                            {
                                outputQuery.AppendLine("  COALESCE([" + tableName + "." + columnName +
                                                       "],'1900-01-01') AS [" + tableName + "." + columnName + "],");
                            }
                            else
                            {
                                outputQuery.AppendLine("  [" + tableName + "." + columnName + "],");
                            }
                        }

                    }
                    else
                    {
                        outputQuery.AppendLine("  [" + tableName + "." + columnName + "],");
                    }

                }
            }

            outputQuery.Remove(outputQuery.Length - 3, 3);
            outputQuery.AppendLine();
            outputQuery.AppendLine("FROM");
            outputQuery.AppendLine("(");
            outputQuery.AppendLine("  SELECT");
            outputQuery.AppendLine("    *,");
            outputQuery.AppendLine("    LAG(ATTRIBUTE_CHECKSUM, 1, '-1') OVER(PARTITION BY ["+hubTable+"."+hubKey+"] ORDER BY PIT_EFFECTIVE_DATETIME ASC) AS PREVIOUS_ATTRIBUTE_CHECKSUM");
            outputQuery.AppendLine("  FROM");
            outputQuery.AppendLine("  (");
            outputQuery.AppendLine("    SELECT *,");
            outputQuery.AppendLine("      CONVERT(CHAR(32),HASHBYTES('MD5',");
            foreach (String tableName in tableList.Keys)
            {
                List<String> columnNames = new List<string>();
                tableList.TryGetValue(tableName, out columnNames); // Get the list with the attribute names for the table
                foreach (String columnName in columnNames)
                {
                    outputQuery.AppendLine("        ISNULL(RTRIM(CONVERT(VARCHAR(100),[" + tableName + "." + columnName + "])),'NA')+'|'+");
                }
            }
            outputQuery.Remove(outputQuery.Length - 3, 3);
            outputQuery.AppendLine();
            outputQuery.AppendLine("      ),2) AS ATTRIBUTE_CHECKSUM");

            outputQuery.AppendLine("    FROM");
            outputQuery.AppendLine("    (");
            outputQuery.AppendLine("      SELECT ");
            foreach (String tableName in tableList.Keys)
            {
                List<String> columnNames = new List<string>();
                tableList.TryGetValue(tableName, out columnNames); // Get the list with the attribute names for the table
                foreach (String columnName in columnNames)
                {
                    outputQuery.AppendLine("        "+tableName+"."+columnName+ " AS [" + tableName + "." + columnName + "],");
                }
            }
            outputQuery.AppendLine("        PIT_EFFECTIVE_DATETIME");
            outputQuery.AppendLine("      FROM");
            outputQuery.AppendLine("      (");

            // Add subselection for snapshot
            if (radioButtonSingleDateSnapshot.Checked)
            {
                outputQuery.AppendLine("      SELECT *");
                outputQuery.AppendLine("      FROM");
                outputQuery.AppendLine("      (");
            }


            outputQuery.AppendLine("        SELECT");
            outputQuery.AppendLine("          "+ hubKey + ",");

            if (radioButtonSingleDateSnapshot.Checked)
            {
                outputQuery.AppendLine("          '"+dateTimePickerSnapShot.Value.ToString("yyyy-MM-dd HH:mm:ss")+"' AS PIT_EFFECTIVE_DATETIME,");
                //     .Value.ToString("yyyyMMdd"); 2017-03-26 20:07:57.1333363
            }
            else
            {
                outputQuery.AppendLine("          PIT_EFFECTIVE_DATETIME,");
            }
            outputQuery.AppendLine("          LEAD(PIT_EFFECTIVE_DATETIME,1,'9999-12-31') OVER (PARTITION BY " + hubKey + " ORDER BY PIT_EFFECTIVE_DATETIME ASC) AS PIT_EXPIRY_DATETIME");
            outputQuery.AppendLine("        FROM");
            outputQuery.AppendLine("        (");
            if (checkBoxIncorporateZeroRecords.Checked)
            {
                outputQuery.AppendLine("          SELECT " + hubKey +", CONVERT(datetime2(7), '1900-01-01') AS PIT_EFFECTIVE_DATETIME FROM  " +hubTable);
                outputQuery.AppendLine("          UNION");
            }
            foreach (String tablename in tableList.Keys)
            {
                if (tablename.Contains(hubIdentifier) || tablename.Contains(linkIdentifier))
                {
                    outputQuery.AppendLine("          SELECT " + hubKey + ", " + hubEffectiveDate + " AS PIT_EFFECTIVE_DATETIME FROM " + tablename);
                }
                else
                {
                    outputQuery.AppendLine("          SELECT " + hubKey + ", " + satEffectiveDate + " AS PIT_EFFECTIVE_DATETIME FROM " + tablename);
                }
             
                if (tableList.Keys.Last() != tablename)
                {
                  outputQuery.AppendLine("          UNION");
                }
            }

            outputQuery.AppendLine("        ) PIT");

            // Add subselection for snapshot
            if (radioButtonSingleDateSnapshot.Checked)
            {
                outputQuery.AppendLine("      ) SNAPSHOTDATE");
                outputQuery.AppendLine("      WHERE '" + dateTimePickerSnapShot.Value + "' BETWEEN PIT_EFFECTIVE_DATETIME AND PIT_EXPIRY_DATETIME");
            }





            outputQuery.AppendLine("      ) TimeRanges");

            // Join the Hub for the business key
            outputQuery.AppendLine("      INNER JOIN " + hubTable);
            outputQuery.AppendLine("        ON TimeRanges." + hubKey +" = "+hubTable+"."+hubKey);

            // Join in the historised tables
            foreach (String tableName in tableList.Keys)
            {
                if (!tableName.Contains(hubIdentifier))
                {
                    if (tableName.Contains(linkIdentifier))
                    {
                        outputQuery.AppendLine("      LEFT OUTER JOIN " + tableName);
                        outputQuery.AppendLine("        ON " + tableName + "." + hubKey + " = TimeRanges." + hubKey);
                       // outputQuery.AppendLine("        AND " + tableName + "." + satEffectiveDate + " <= TimeRanges.PIT_EFFECTIVE_DATETIME");
                       // outputQuery.AppendLine("        AND " + tableName + "." + _myParent.configurationSettings.ExpiryDateTimeAttribute + " >= TimeRanges.PIT_EXPIRY_DATETIME");
                    }
                    if (tableName.Contains(satIdentifier))
                    {
                        outputQuery.AppendLine("      LEFT OUTER JOIN " + tableName);
                        outputQuery.AppendLine("        ON " + tableName + "." + hubKey + " = TimeRanges." + hubKey);
                        outputQuery.AppendLine("        AND " + tableName + "." + satEffectiveDate +" <= TimeRanges.PIT_EFFECTIVE_DATETIME");
                        outputQuery.AppendLine("        AND " + tableName + "." +configurationSettings.ExpiryDateTimeAttribute +" >= TimeRanges.PIT_EXPIRY_DATETIME");
                    }
                }
            }
            outputQuery.AppendLine("    ) SUB_TIMELINES");
            outputQuery.AppendLine("  ) SUB_CHECKSUM");
            outputQuery.AppendLine(") SUB_FINAL");
            outputQuery.AppendLine("WHERE ATTRIBUTE_CHECKSUM <> PREVIOUS_ATTRIBUTE_CHECKSUM");

            // Output to new tab (only if something was checked
            if (checkedCounter > 0)
            {
                tabControl.SelectTab(1);
                richTextBoxOutput.Text = outputQuery.ToString();
            }
            else
            {
                richTextBoxInformation.Text = "Nothing was selected! Can you verify if you selected an attribute to incorporate?";
            }
        }

        private void checkBoxSuppressSat_CheckedChanged(object sender, EventArgs e)
        {
            richTextBoxInformation.Clear();
            GetAttributes("'" + labelFirstHub.Text + "'");
        }

        private void checkBoxSuppressLink_CheckedChanged(object sender, EventArgs e)
        {
            richTextBoxInformation.Clear();
            GetAttributes("'" + labelFirstHub.Text + "'");
        }

        private void radioButtonIntegrationLayer_CheckedChanged(object sender, EventArgs e)
        {
            richTextBoxInformation.Clear();
            richTextBoxInformation.Text += "Refreshing information using the Integration Layer metadata.";
            GetAttributes("'" + labelFirstHub.Text + "'");
        }

        private void radioButtonPSA_CheckedChanged(object sender, EventArgs e)
        {
            richTextBoxInformation.Clear();
            richTextBoxInformation.Text += "Refreshing information using the Persistent Staging Area metadata.";
            GetAttributes("'" + labelFirstHub.Text + "'");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tabHub_Click(object sender, EventArgs e)
        {

        }

        private void FormPit_Load(object sender, EventArgs e)
        {

        }



        private void radioButtonNoShapshot_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButtonNoShapshot.Checked)
            {
                //labelSnapshotRange.Enabled = false;
                //comboBoxSnapshotRange.Enabled = false;
                //labelSnapshotDateRangeFrom.Enabled = false;
                //labelSnapshotDateRangeTo.Enabled = false;
                //dateTimePickerSnapshotFrom.Enabled = false;
                //dateTimePickerSnapshotTo.Enabled = false;
                //groupBoxRangeSnapshot.Enabled = false;
                //groupBoxSingleSnapshot.Enabled = false;

            }

            if (radioButtonNoShapshot.Checked)
            {
                groupBoxSingleSnapshot.Enabled = false;
                groupBoxRangeSnapshot.Enabled = false;

            }
        }

        private void radioButtonSingleDateSnapshot_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButtonSingleDateSnapshot.Checked)
            {
                groupBoxSingleSnapshot.Enabled = false;

                groupBoxRangeSnapshot.Enabled = true;
                checkBoxPITExpiryDate.Checked = true;
                checkBoxPITExpiryDate.Enabled = true;
            }

            if (radioButtonSingleDateSnapshot.Checked)
            {
                groupBoxSingleSnapshot.Enabled = true;

                groupBoxRangeSnapshot.Enabled = false;
                checkBoxPITExpiryDate.Checked = false;
                checkBoxPITExpiryDate.Enabled = false;
            }

        }

        private void radioButtonRangeSnapshot_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButtonRangeSnapshot.Checked)
            {
                groupBoxRangeSnapshot.Enabled = false;

                groupBoxSingleSnapshot.Enabled =true;
                checkBoxPITExpiryDate.Checked = true;
                checkBoxPITExpiryDate.Enabled = true;
            }

            if (radioButtonRangeSnapshot.Checked)
            {
                groupBoxRangeSnapshot.Enabled = true;

                groupBoxSingleSnapshot.Enabled = false;
                checkBoxPITExpiryDate.Checked = false;
                checkBoxPITExpiryDate.Enabled = false;
            }

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

            if (radioButtonNoCondensing.Checked)
            {
                groupBoxFrequencyCondensing.Enabled = false;
                groupBoxContinuousCondensing.Enabled = false;

            }
        }
    }
}
