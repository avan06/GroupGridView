using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GroupGridView
{
    /// <summary>
    /// [Reference]
    /// create datagridview column with NumericUpDown.
    /// https://stackoverflow.com/a/55788490
    /// </summary>
    public class DataGridViewUpDownColumn : DataGridViewColumn
    {
        public DataGridViewUpDownColumn() : base(new DataGridViewUpDownCell()) { }
        public override DataGridViewCell CellTemplate
        {
            get => base.CellTemplate;
            set
            {
                if (value != null && !value.GetType().IsAssignableFrom(typeof(DataGridViewUpDownCell))) throw new InvalidCastException("Must be a DataGridViewUpDownCell");
                base.CellTemplate = value;
            }
        }
    }

    public class DataGridViewUpDownCell : DataGridViewTextBoxCell
    {
        public DataGridViewUpDownCell() : base() { }
        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            DataGridViewUpDownEditingControl upDownControl = DataGridView.EditingControl as DataGridViewUpDownEditingControl;
            if (upDownControl != null) upDownControl.Text = (string)Value;
        }

        public override Type EditType => typeof(DataGridViewUpDownEditingControl);
        public override object DefaultNewRowValue => null; //未編集の新規行に余計な初期値が出ないようにする
    }

    public class DataGridViewUpDownEditingControl : UpDownBase, IDataGridViewEditingControl
    {
        #region DataGridViewEditingControl
        public event DataGridViewUpDownCellEventHandler UpDown;
        public int EditingControlColIndex { get => EditingControlDataGridView.CurrentCell.ColumnIndex; }
        public int EditingControlRowIndex { get; set; }
        public bool EditingControlValueChanged { get; set; }
        public DataGridView EditingControlDataGridView { get; set; }
        public object EditingControlFormattedValue
        {
            get => GetEditingControlFormattedValue(DataGridViewDataErrorContexts.Formatting);
            set => Text = (string)value;
        }
        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) => Text;
        public Cursor EditingPanelCursor => Cursors.Default;
        public bool RepositionEditingControlOnValueChange => false;
        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            Font = dataGridViewCellStyle.Font;
            ForeColor = dataGridViewCellStyle.ForeColor;
            BackColor = dataGridViewCellStyle.BackColor;
        }
        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey) =>
            (keyData == Keys.Left || keyData == Keys.Right || keyData == Keys.Up || keyData == Keys.Down ||
             keyData == Keys.Home || keyData == Keys.End || keyData == Keys.PageDown || keyData == Keys.PageUp);
        public void PrepareEditingControlForEdit(bool selectAll) {
            if (selectAll) Select(0, Text.Length);
        }
        protected override void OnTextChanged(EventArgs e)
        {
            if ((string)EditingControlDataGridView.CurrentCell.Value != Text) UpDown?.Invoke(this, new DataGridViewUpDownCellEventArgs(EditingControlColIndex, EditingControlRowIndex, Text));

            base.OnTextChanged(e);
            EditingControlValueChanged = true;
            EditingControlDataGridView.NotifyCurrentCellDirty(true);
        }
        #endregion

        #region UpDown
        public override void UpButton()
        {
            try
            {
                if (!Regex.IsMatch(Text, @"^-?[0-9][0-9,\.]*$")) return;
                decimal value = decimal.Parse(Text) + 1;
                Text = value.ToString();
            }
            catch (Exception) {}
        }
        public override void DownButton()
        {
            try
            {
                if (!Regex.IsMatch(Text, @"^-?[0-9][0-9,\.]*$")) return;
                decimal value = decimal.Parse(Text) - 1;
                Text = value.ToString();
            }
            catch (Exception) {}
        }
        protected override void UpdateEditText() { }
        #endregion
    }
    public delegate void DataGridViewUpDownCellEventHandler(object sender, DataGridViewUpDownCellEventArgs e);

    public class DataGridViewUpDownCellEventArgs : DataGridViewCellEventArgs
    {
        public string Text { get; private set; }
        public DataGridViewUpDownCellEventArgs(int columnIndex, int rowIndex, string text) : base(columnIndex, rowIndex) { Text = text; }
    }
}
