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
        public List<string> GetItemList(string inputType, string inputQuery, SqlConnection conn)
        {
            List<string> returnList = new List<string>();

            try
            {
                var tables = Utility.GetDataTable(ref conn, inputQuery);

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
