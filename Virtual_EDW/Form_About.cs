using System.Diagnostics;
using System.Windows.Forms;

namespace Virtual_Data_Warehouse
{
    public partial class FormAbout : FormBase
    {
        //private readonly FormMain _myParent;

        public FormAbout()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;
        }
        
        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Specify that the link was visited. 
            linkLabelTeam.LinkVisited = true;
            // Navigate to a URL.
            Process.Start("https://github.com/RoelantVos/virtual-data-warehouse");
        }

        private void linkLabelTeam_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Specify that the link was visited. 
            linkLabel1.LinkVisited = true;
            // Navigate to a URL.
            Process.Start("http://www.roelantvos.com");
        }

        private void buttonClose_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
