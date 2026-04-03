using GroupGridView;
using System;
using System.Windows.Forms;

namespace GroupGridViewApp
{
    public partial class Form1 : Form
    {
        private void ToolStripExpandAll_Click(object sender, EventArgs e) => GroupGridView1.CollapseExpandAll(false);

        private void ToolStripCollapseAll_Click(object sender, EventArgs e) => GroupGridView1.CollapseExpandAll(true);

        public Form1()
        {
            InitializeComponent();
            GroupGridView1.Rows.Add(new object[] { "ONE", "TWO", "0" });
            GroupGridView1.Rows.Add(new object[] { "ONE", "TWO", "4" });
            GroupGridView1.Rows.Add(new object[] { "ONE", "TWO", "1" });
            GroupGridView1.Rows.Add(new object[] { "ONE", "TWO", "3" });
            GroupGridView1.Rows.Add(new object[] { "ONE", "TWO", "5" });
            GroupGridView1.Rows.Add(new object[] { "ONE", "TWO", "10" });
            GroupGridView1.Rows.Add(new object[] { "ONE", "THREE", "7" });
            GroupGridView1.Rows.Add(new object[] { "ONE", "THREE", "6" });
            GroupGridView1.Rows.Add(new object[] { "ONE", "THREE", "2" });
            GroupGridView1.Rows.Add(new object[] { "ONE", "THREE", "1" });
            GroupGridView1.Rows.Add(new object[] { "ONE", "TWO", "2" });
            GroupGridView1.Rows.Add(new object[] { "ONE", "TWO", "6" });
            GroupGridView1.Rows.Add(new object[] { "ONE", "TWO", "7" });
            GroupGridView1.Rows.Add(new object[] { "ONE", "TWO", "8" });
            GroupGridView1.Rows.Add(new object[] { "ONE", "TWO", "9" });
            GroupGridView1.Rows.Add(new object[] { "ONE", "TWO1", "6" });
            GroupGridView1.Rows.Add(new object[] { "ONE", "TWO1", "3" });
            GroupGridView1.Rows.Add(new object[] { "ONE", "TWO1", "5" });
            GroupGridView1.Rows.Add(new object[] { "ONE", "TWO1", "4" });
            GroupGridView1.Rows.Add(new object[] { "ONE", "TWO1", "2" });
            GroupGridView1.Rows.Add(new object[] { "ONE", "TWO1", "1" });
            GroupGridView1.Rows.Add(new object[] { "TWO", "THREE", "5" });
            GroupGridView1.Rows.Add(new object[] { "TWO", "THREE", "1" });
            GroupGridView1.Rows.Add(new object[] { "TWO", "THREE", "8" });
            GroupGridView1.Rows.Add(new object[] { "TWO", "THREE", "4" });
            GroupGridView1.Rows.Add(new object[] { "TWO", "THREE", "6" });
            GroupGridView1.Rows.Add(new object[] { "TWO", "THREE", "9" });
            GroupGridView1.Rows.Add(new object[] { "TWO", "THREE", "2" });
            GroupGridView1.Rows.Add(new object[] { "TWO", "THREE", "3" });
        }

        /// <summary>
        /// This event handler manually raises the CellValueChanged event by calling the CommitEdit method.
        /// https://stackoverflow.com/a/7194708
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (GroupGridView1.IsCurrentCellDirty) GroupGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void GroupGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (GroupGridView1.CurrentCell == null) return;

            Console.WriteLine(String.Format("col:{0}, row:{1}, Text:{2}...CellValueChanged", e.ColumnIndex, e.RowIndex, GroupGridView1.CurrentCell.Value));
        }

        private void GroupGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewUpDownEditingControl upDownControl = GroupGridView1.EditingControl as DataGridViewUpDownEditingControl;
            if (upDownControl == null) return;

            upDownControl.UpDownDelta -= UpDownControl_UpDownDelta;
            upDownControl.UpDownDelta += UpDownControl_UpDownDelta;
        }

        private bool isBulkUpdating = false; // Flag to prevent recursive triggering

        private void UpDownControl_UpDownDelta(object sender, DataGridViewUpDownDeltaEventArgs e)
        {
            if (isBulkUpdating) return;

            try
            {
                isBulkUpdating = true;
                var dgv = GroupGridView1;
                int targetCol = e.ColumnIndex;

                // Iterate through all selected cells to perform broadcast updates
                foreach (DataGridViewCell cell in dgv.SelectedCells)
                {

                    // Safety check: only process cells within the specific column type
                    if (dgv.Columns[cell.ColumnIndex] is DataGridViewUpDownColumn)
                    {
                        if (cell.Value == null || !decimal.TryParse(cell.Value.ToString(), out decimal val)) continue;

                        // Calculate the new relative value
                        string newValue = (val + e.Delta).ToString();


                        // Skip updating the cell.Value for the current editing cell to avoid UI conflicts.
                        // The EditingControl itself handles the display for the active cell.
                        if (cell.ColumnIndex == e.ColumnIndex && cell.RowIndex == e.RowIndex) continue;

                        // Update the UI display for other linked cells
                        cell.Value = newValue;
                    }
                }
            }
            finally
            {
                isBulkUpdating = false;
            }
        }
    }
}
