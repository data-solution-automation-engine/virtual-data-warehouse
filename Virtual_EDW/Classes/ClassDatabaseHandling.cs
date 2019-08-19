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

                foreach (DataRow row in tables.Rows)
                {
                    returnList.Add(row["TARGET_NAME"].ToString());
                }
            }
            catch (Exception)
            {
                // IGNORE FOR NOW
            }

            return returnList;
        }
    }
}
