using System.Windows.Forms;

namespace GroupGridViewApp
{
    public partial class Form1 : Form
    {
        private void ToolStripExpandAll_Click(object sender, System.EventArgs e)
        {
            GroupGridView1.CollapseExpandAll(false);
        }

        private void ToolStripCollapseAll_Click(object sender, System.EventArgs e)
        {
            GroupGridView1.CollapseExpandAll(true);
        }

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
    }
}
