using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Virtual_Data_Warehouse;

namespace Virtual_Data_Warehouse
{
    public partial class FormAlert : FormBase
    {
        //Make the label and progressbar accessible from the main form for updates
        public string Message
        {
            set { labelProgressMessage.Text = value; }
        }

        public string Log
        {
            set { richTextBoxMetadataLog.Text += value; }
        }

        public int ProgressValue
        {
            set { progressBar1.Value = value; }
        }

        #region Delegate & function for hiding the Progress Bar
        delegate void ShowProgressBarCallBack(bool showProgressBar);
        public void ShowProgressBar(bool showProgressBar)
        {
            if (progressBar1.InvokeRequired)
            {
                var d = new ShowProgressBarCallBack(ShowProgressBar);
                try
                {
                    Invoke(d, showProgressBar);
                }
                catch
                {
                    // ignored
                }
            }
            else
            {
                try
                {
                    progressBar1.Visible = false;
                }
                catch
                {
                    // ignored
                }
            }
        }
        #endregion

        #region Delegate & function for hiding the Show Log button
        delegate void ShowLogButtonCallBack(bool showLogButton);
        public void ShowLogButton(bool showLogButton)
        {
            if (buttonShowLog.InvokeRequired)
            {
                var d = new ShowLogButtonCallBack(ShowLogButton);
                try
                {
                    Invoke(d, showLogButton);
                }
                catch
                {
                    // ignored
                }
            }
            else
            {
                try
                {
                    buttonShowLog.Visible = false;
                }
                catch
                {
                    // ignored
                }
            }
        }
        #endregion

        #region Delegate & function for hiding the Cancel button
        delegate void ShowCancelButtonCallBack(bool showCancelButton);
        public void ShowCancelButton(bool showCancelButton)
        {
            if (buttonCancel.InvokeRequired)
            {
                var d = new ShowCancelButtonCallBack(ShowCancelButton);
                try
                {
                    Invoke(d, showCancelButton);
                }
                catch
                {
                    // ignored
                }
            }
            else
            {
                try
                {
                    buttonCancel.Visible = false;
                }
                catch
                {
                    // ignored
                }
            }
        }
        #endregion

        #region Delegate & function for hiding the Progress Label
        delegate void ShowProgressLabelCallBack(bool showProgressLabel);
        public void ShowProgressLabel(bool showProgressLabel)
        {
            if (labelProgressMessage.InvokeRequired)
            {
                var d = new ShowCancelButtonCallBack(ShowProgressLabel);
                try
                {
                    Invoke(d, showProgressLabel);
                }
                catch
                {
                    // ignored
                }
            }
            else
            {
                try
                {
                    labelProgressMessage.Visible = false;
                }
                catch
                {
                    // ignored
                }
            }
        }
        #endregion

        public FormAlert()
        {
            InitializeComponent();
        }

        // Multithreading for updating the user
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
                    // ignored
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
                    // ignored
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
            //Check if the file exists, otherwise create a dummy / empty file   
            if (File.Exists(GlobalParameters.VdwConfigurationPath + @"\Error_Log.txt"))
            {
                Process.Start(GlobalParameters.VdwConfigurationPath + @"\Error_Log.txt");
            }
            else
            {
                MessageBox.Show("There is no error file. This is a good thing right?", "No error file found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
