using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Virtual_EDW
{
    class CustomDataGridView : DataGridView
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ColourGridView();
        }

        private void ColourGridView()
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
                    if (Regex.Matches(integrationTable.ToString(), "HUB_").Count>0)
                    {
                        this[3, counter].Style.BackColor = Color.CadetBlue;
                    }
                    else if (Regex.Matches(integrationTable.ToString(), "LSAT_").Count > 0)
                    {
                        this[3, counter].Style.BackColor = Color.Yellow;
                    }
                    else if (Regex.Matches(integrationTable.ToString(), "SAT_").Count > 0)
                    {
                        this[3, counter].Style.BackColor = Color.Gold;
                    }
                    else if (Regex.Matches(integrationTable.ToString(), "LNK_").Count > 0)
                    {
                        this[3, counter].Style.BackColor = Color.Red;
                    }

                    ////Other style tables
                    //if (Regex.Matches(genericTable.ToString(), "HUB_").Count > 0)
                    //{
                    //    this[0, counter].Style.BackColor = Color.CadetBlue;
                    //}
                    //else if (Regex.Matches(genericTable.ToString(), "LSAT_").Count > 0)
                    //{
                    //    this[0, counter].Style.BackColor = Color.Yellow;
                    //}
                    //else if (Regex.Matches(genericTable.ToString(), "SAT_").Count > 0)
                    //{
                    //    this[0, counter].Style.BackColor = Color.Gold;
                    //}
                    //else if (Regex.Matches(genericTable.ToString(), "LNK_").Count > 0)
                    //{
                    //    this[0, counter].Style.BackColor = Color.Red;
                    //}

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
