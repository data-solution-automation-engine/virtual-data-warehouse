using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using TEAM_Library;

namespace Virtual_Data_Warehouse
{
    public class TemplateGridView : DataGridView
    {
        public TemplateGridView(TeamConfiguration teamConfiguration)
        {
            #region Generic properties

            // Disable resizing for performance, will be enabled after binding.
            RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            AutoGenerateColumns = false;
            ColumnHeadersVisible = true;

            EditMode = DataGridViewEditMode.EditOnEnter;

            Location = new Point(4, 215);
            Size = new Size(1417, 360);
            BackgroundColor = SystemColors.AppWorkspace;
            GridColor = SystemColors.ControlDark;

            //BorderStyle = BorderStyle.FixedSingle;
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BorderStyle = BorderStyle.None;

            Name = "dataGridViewTemplateCollection";

            #endregion

            #region Columns

            DataGridViewTextBoxColumn TemplateName = new DataGridViewTextBoxColumn
            {
                Name = TemplateGridColumns.TemplateName.ToString(),
                HeaderText = "Template Name",
                DataPropertyName = TemplateGridColumns.TemplateName.ToString()
            };
            Columns.Add(TemplateName);

            DataGridViewTextBoxColumn TemplateType = new DataGridViewTextBoxColumn
            {
                Name = TemplateGridColumns.TemplateType.ToString(),
                HeaderText = "Type",
                DataPropertyName = TemplateGridColumns.TemplateType.ToString()
            };
            Columns.Add(TemplateType);

            DataGridViewComboBoxColumn TemplateConnectionKey = new DataGridViewComboBoxColumn
            {
                Name = TemplateGridColumns.TemplateConnectionKey.ToString(),
                HeaderText = "Connection Key",
                DataPropertyName = TemplateGridColumns.TemplateConnectionKey.ToString(),
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DataSource = LocalTeamConnection.GetConnections(teamConfiguration.ConnectionDictionary),
                DisplayMember = "ConnectionKey",
                ValueMember = "ConnectionId",
                ValueType = typeof(string)
            };
            Columns.Add(TemplateConnectionKey);

            DataGridViewTextBoxColumn TemplateOutputFileConvention = new DataGridViewTextBoxColumn
            {
                Name = TemplateGridColumns.TemplateOutputFileConvention.ToString(),
                HeaderText = "Output File Convention",
                DataPropertyName = TemplateGridColumns.TemplateOutputFileConvention.ToString()
            };
            Columns.Add(TemplateOutputFileConvention);

            DataGridViewTextBoxColumn TemplatePath = new DataGridViewTextBoxColumn
            {
                Name = TemplateGridColumns.TemplateFilePath.ToString(),
                HeaderText = "Path",
                DataPropertyName = TemplateGridColumns.TemplateFilePath.ToString()
            };
            Columns.Add(TemplatePath);

            DataGridViewTextBoxColumn TemplateNotes = new DataGridViewTextBoxColumn
            {
                Name = TemplateGridColumns.TemplateNotes.ToString(),
                HeaderText = "Notes",
                DataPropertyName = TemplateGridColumns.TemplateNotes.ToString()
                //Width = 400
            };
            Columns.Add(TemplateNotes);

            #endregion

            #region Event Handlers

            CurrentCellDirtyStateChanged += dataGridViewTemplateCollection_CurrentCellDirtyStateChanged;
            //DataError += dataGridViewDataError;

            #endregion
        }

        private void dataGridViewDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Error happened " + e.Context);

            if (e.Context == DataGridViewDataErrorContexts.Commit)
            {
                MessageBox.Show("Commit error");
            }

            if (e.Context == DataGridViewDataErrorContexts.CurrentCellChange)
            {
                MessageBox.Show("Cell change");
            }

            if (e.Context == DataGridViewDataErrorContexts.Parsing)
            {
                MessageBox.Show("parsing error");
            }

            if (e.Context == DataGridViewDataErrorContexts.LeaveControl)
            {
                MessageBox.Show("leave control error");
            }

            if ((e.Exception) is ConstraintException)
            {
                DataGridView view = (DataGridView)sender;
                view.Rows[e.RowIndex].ErrorText = "an error";
                view.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "an error";
                e.ThrowException = false;
            }
        }

        public void AutoLayout()
        {
            //AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            //Columns[ColumnCount - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            //Disable the auto size again(to enable manual resizing).
            //for (var i = 0; i < Columns.Count - 1; i++)
            //{
            //    Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //    Columns[i].Width = Columns[i].GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, true);
            //}

            try
            {
                //dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                //dataGridView.Columns[dataGridView.ColumnCount - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                // Disable the auto size again (to enable manual resizing).
                for (var i = 0; i < Columns.Count - 1; i++)
                {
                    Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    Columns[i].Width = Columns[i].GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, true);
                }
            }
            catch
            {
                // Ignore it for now.
            }
        }

        /// <summary>
        /// Ensure changes, especially in the combobox are managed straight away and not require leaving the cell to commit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewTemplateCollection_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (IsCurrentCellDirty)
            {
                CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
    }

    /// <summary>
    /// Enumerator to hold the column index for the columns (headers) in the Table Metadata data grid view.
    /// </summary>
    public enum TemplateGridColumns
    {
        TemplateName = 0,
        TemplateType = 1,
        TemplateConnectionKey = 2,
        TemplateOutputFileConvention = 3,
        TemplateFilePath = 4,
        TemplateNotes = 5,
    }


}
