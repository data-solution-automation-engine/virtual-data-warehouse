using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Virtual_Data_Warehouse
{
    class CustomDataGridViewAttribute : DataGridView
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var hubIdentifier = "HUB_";
            var satIdentifier = "SAT_";
            var lnkIdentifier = "LNK_";
            var lsatIdentifier = "LSAT_";

            ColourGridViewAttribute(hubIdentifier, satIdentifier, lnkIdentifier, lsatIdentifier);
        }

        private void ColourGridViewAttribute(string hubIdentifier, string satIdentifier, string lnkIdentifier, string lsatIdentifier)
        {
            var counter = 0;

            foreach (DataGridViewRow row in Rows)
            {
               // var genericTable = row.Cells[0].Value;
                var integrationTable = row.Cells[4].Value;
                var businessKeySyntax = row.Cells[3].Value;

                if (integrationTable != null && businessKeySyntax != null && row.IsNewRow == false)
                {
                    // Backcolour for Integration Layer tables
                    if (Regex.Matches(integrationTable.ToString(), hubIdentifier).Count>0)
                    {
                        this[4, counter].Style.BackColor = Color.CadetBlue;
                    }
                    else if (Regex.Matches(integrationTable.ToString(), lsatIdentifier).Count > 0)
                    {
                        this[4, counter].Style.BackColor = Color.Yellow;
                    }
                    else if (Regex.Matches(integrationTable.ToString(), satIdentifier).Count > 0)
                    {
                        this[4, counter].Style.BackColor = Color.Gold;
                    }
                    else if (Regex.Matches(integrationTable.ToString(), lnkIdentifier).Count > 0)
                    {
                        this[4, counter].Style.BackColor = Color.Red;
                    }

                    //Syntax highlighting for code
                    if (businessKeySyntax.ToString().Contains("COALESCE"))
                    {      
                        this[3, counter].Style.ForeColor = Color.DarkBlue;
                        this[3, counter].Style.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
                    }
                }

                counter++;
            }
        }
    }
}
