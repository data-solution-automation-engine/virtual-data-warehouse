using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Virtual_EDW
{
    class CustomDataGridViewTable : DataGridView 
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var hubIdentifier = "HUB_";
            var satIdentifier = "SAT_";
            var lnkIdentifier = "LNK_";
            var lsatIdentifier = "LSAT_";

            ColourGridView(hubIdentifier, satIdentifier, lnkIdentifier, lsatIdentifier);
        }

        private void ColourGridView(string hubIdentifier, string satIdentifier, string lnkIdentifier, string lsatIdentifier)
        {
            var counter = 0;

            foreach (DataGridViewRow row in Rows)
            {
               // var genericTable = row.Cells[0].Value;
                var integrationTable = row.Cells[3].Value;
                var businessKeySyntax = row.Cells[4].Value;

                if (integrationTable != null && businessKeySyntax != null && row.IsNewRow == false)
                {
                   // Backcolour for Integration Layer tables
                    if (Regex.Matches(integrationTable.ToString(), hubIdentifier).Count>0)
                    {
                        this[3, counter].Style.BackColor = Color.CadetBlue;
                        row.Cells[5].ReadOnly = true;
                        row.Cells[5].Style.BackColor = System.Drawing.Color.LightGray;
                    }
                    else if (Regex.Matches(integrationTable.ToString(), lsatIdentifier).Count > 0)
                    {
                        this[3, counter].Style.BackColor = Color.Yellow;
                    }
                    else if (Regex.Matches(integrationTable.ToString(), satIdentifier).Count > 0)
                    {
                        this[3, counter].Style.BackColor = Color.Gold;
                        row.Cells[5].ReadOnly = true;
                        row.Cells[5].Style.BackColor = System.Drawing.Color.LightGray;
                    }
                    else if (Regex.Matches(integrationTable.ToString(), lnkIdentifier).Count > 0)
                    {
                        this[3, counter].Style.BackColor = Color.Red;
                        row.Cells[5].ReadOnly = true;
                        row.Cells[5].Style.BackColor = System.Drawing.Color.LightGray;
                    }

                    //Syntax highlighting for code
                    if (businessKeySyntax.ToString().Contains("CONCATENATE") || businessKeySyntax.ToString().Contains("COMPOSITE"))
                    {      
                        this[4, counter].Style.ForeColor = Color.DarkBlue;
                        this[4, counter].Style.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
                    }
                }

                counter++;
            }
        }
    }
}
