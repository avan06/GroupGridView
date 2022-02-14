using GroupGridView;
using System;
using System.Windows.Forms;

namespace GroupGridViewApp
{
    public partial class Form1 : Form
    {
        private void ToolStripExpandAll_Click(object sender, System.EventArgs e) => GroupGridView1.CollapseExpandAll(false);

        private void ToolStripCollapseAll_Click(object sender, System.EventArgs e) => GroupGridView1.CollapseExpandAll(true);

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

            GroupGridView1.CurrentCellDirtyStateChanged += GroupGridView1_CurrentCellDirtyStateChanged;
            GroupGridView1.CellValueChanged += GroupGridView1_CellValueChanged;
            GroupGridView1.EditingControlShowing += GroupGridView1_EditingControlShowing;
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

        private void GroupGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e) =>
            Console.WriteLine(String.Format("col:{0}, row:{1}, Text:{2}...CellValueChanged", e.ColumnIndex, e.RowIndex, GroupGridView1.CurrentCell.Value));

        private void GroupGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewUpDownEditingControl upDownControl = GroupGridView1.EditingControl as DataGridViewUpDownEditingControl;
            if (upDownControl == null) return;

            upDownControl.UpDown -= UpDownControl_UpDown;
            upDownControl.UpDown += UpDownControl_UpDown;
        }

        private void UpDownControl_UpDown(object sender, DataGridViewUpDownCellEventArgs e) =>
            Console.WriteLine(String.Format("col:{0}, row:{1}, Text:{2}...UpDown", e.ColumnIndex, e.RowIndex, e.Text));
    }
}
