using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Virtual_EDW
{
    public partial class Form_Alert : FormBase
    {
        //Make the label and progressbar accessbile from the main form for updates
        public string Message
        {
            set { labelMessage.Text = value; }
        }

        public string Log
        {
            set { richTextBoxMetadataLog.Text += value; }
        }

        public int ProgressValue
        {
            set { progressBar1.Value = value; }
        }

        public Form_Alert()
        {
            InitializeComponent();
        }

        // Multithreading for updating the user (Staging Area form)
        delegate void SetTextCallBackLogging(string text);
        public void SetTextLogging(string text)
        {
            if (richTextBoxMetadataLog.InvokeRequired)
            {
                var d = new SetTextCallBackLogging(SetTextLogging);
                try
                {
                    Invoke(d, text);
                }
                catch
                {
                }
            }
        else
            {
                try
                {
                    richTextBoxMetadataLog.AppendText(text);
                }
                catch
                {
                }
            }           
        }

        public event EventHandler<EventArgs> Canceled;

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            EventHandler<EventArgs> ea = Canceled;
            if (ea != null)
            {
                ea(this, e);
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonShowLog_Click(object sender, EventArgs e)
        {
            Process.Start(GlobalParameters.VedwConfigurationPath + @"\Error_Log.txt");
        }
    }
}
