using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using TEAM_Library;

namespace Virtual_Data_Warehouse
{
    public class LoadPatternGridView : DataGridView
    {
        public LoadPatternGridView(TeamConfiguration teamConfiguration)
        {
            #region Generic properties

            AutoGenerateColumns = false;
            ColumnHeadersVisible = true;
            EditMode = DataGridViewEditMode.EditOnEnter;

            Location = new Point(3, 191);
            Size = new Size(1665, 521);
            BackgroundColor = SystemColors.AppWorkspace;
            GridColor = SystemColors.ControlDark;
            BorderStyle = BorderStyle.FixedSingle;

            Name = "dataGridViewLoadPatternCollection";

            #endregion

            #region Columns

            DataGridViewTextBoxColumn loadPatternName = new DataGridViewTextBoxColumn
            {
                Name = LoadPatternGridColumns.LoadPatternName.ToString(),
                HeaderText = "Name",
                DataPropertyName = LoadPatternGridColumns.LoadPatternName.ToString()
            };
            Columns.Add(loadPatternName);

            DataGridViewTextBoxColumn loadPatternType = new DataGridViewTextBoxColumn
            {
                Name = LoadPatternGridColumns.LoadPatternType.ToString(),
                HeaderText = "Type",
                DataPropertyName = LoadPatternGridColumns.LoadPatternType.ToString()
            };
            Columns.Add(loadPatternType);

            DataGridViewComboBoxColumn loadPatternConnectionKey = new DataGridViewComboBoxColumn
            {
                Name = LoadPatternGridColumns.LoadPatternConnectionKey.ToString(),
                HeaderText = "Connection Key",
                DataPropertyName = LoadPatternGridColumns.LoadPatternConnectionKey.ToString(),
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DataSource = LocalTeamConnection.GetConnections(teamConfiguration.ConnectionDictionary),
                DisplayMember = "ConnectionKey",
                ValueMember = "ConnectionId",
                ValueType = typeof(string)
            };
            Columns.Add(loadPatternConnectionKey);

            DataGridViewTextBoxColumn loadPatternOutputFilePattern= new DataGridViewTextBoxColumn
            {
                Name = LoadPatternGridColumns.LoadPatternOutputFilePattern.ToString(),
                HeaderText = "File Pattern",
                DataPropertyName = LoadPatternGridColumns.LoadPatternOutputFilePattern.ToString()
            };
            Columns.Add(loadPatternOutputFilePattern);

            DataGridViewTextBoxColumn loadPatternPath = new DataGridViewTextBoxColumn
            {
                Name = LoadPatternGridColumns.LoadPatternFilePath.ToString(),
                HeaderText = "Path",
                DataPropertyName = LoadPatternGridColumns.LoadPatternFilePath.ToString()
            };
            Columns.Add(loadPatternPath);

            DataGridViewTextBoxColumn loadPatternNotes = new DataGridViewTextBoxColumn
            {
                Name = LoadPatternGridColumns.LoadPatternNotes.ToString(),
                HeaderText = "Notes",
                DataPropertyName = LoadPatternGridColumns.LoadPatternNotes.ToString()
            };
            Columns.Add(loadPatternNotes);

            #endregion


            #region Event Handlers

            CurrentCellDirtyStateChanged += dataGridViewLoadPatternCollection_CurrentCellDirtyStateChanged;
            DataError += dataGridViewDataError;

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
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            Columns[ColumnCount - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // Disable the auto size again (to enable manual resizing).
            for (var i = 0; i < Columns.Count - 1; i++)
            {
                Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                Columns[i].Width = Columns[i].GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, true);
            }
        }

        /// <summary>
        /// Ensure changes, especially in the combobox are managed straight away and not require leaving the cell to commit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewLoadPatternCollection_CurrentCellDirtyStateChanged(object sender, EventArgs e)
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
    public enum LoadPatternGridColumns
    {
        LoadPatternName = 0,
        LoadPatternType = 1,
        LoadPatternConnectionKey = 2,
        LoadPatternOutputFilePattern = 3,
        LoadPatternFilePath = 4,
        LoadPatternNotes = 5,
    }


}
