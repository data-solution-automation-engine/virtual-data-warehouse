using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Virtual_Data_Warehouse.FormBase;

namespace Virtual_Data_Warehouse
{
    class DatabaseHandling
    {
        public List<string> GetItemList(string inputType)
        {
            List<string> returnList = new List<string>();


            var conn = new SqlConnection { ConnectionString = TeamConfigurationSettings.ConnectionStringOmd };

            StringBuilder inputMetadataQuery = new StringBuilder();

            inputMetadataQuery.AppendLine("SELECT");
            inputMetadataQuery.AppendLine("  [TARGET_NAME]");
            inputMetadataQuery.AppendLine("FROM [interface].[INTERFACE_SOURCE_STAGING_XREF]");

            try
            {
                var tables = Utility.GetDataTable(ref conn, inputMetadataQuery.ToString());

                //if (tables.Rows.Count == 0)
                //{
                //    localRichTextBox.AppendText($"There was no metadata available to display {patternNiceName} content. Please check the metadata schema (are there any {patternNiceName} tables available?) or the database connection.");
                //}

                foreach (DataRow row in tables.Rows)
                {
                    returnList.Add(row["TARGET_NAME"].ToString());
                    //localCheckedListBox.Items.Add(row["TARGET_NAME"]);
                }
            }
            catch (Exception ex)
            {
                //returnDetails.AppendLine($"Unable to populate the {patternNiceName} selection, there is no database connection.");
                //returnDetails.AppendLine("Error logging details: " + ex);
            }

            return returnList;
        }
    }
}
