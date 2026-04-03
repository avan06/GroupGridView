using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GroupGridView
{
    /// <summary>
    /// [Reference]
    /// Create datagridview column with NumericUpDown.
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
                if (value != null && !value.GetType().IsAssignableFrom(typeof(DataGridViewUpDownCell)))
                    throw new InvalidCastException("Must be a DataGridViewUpDownCell");
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
        /// <summary>
        /// Notifies up/down adjustments via a delta event. 
        /// Suitable for the outer application to apply changes to multiple cells simultaneously.
        /// </summary>
        public event DataGridViewUpDownDeltaEventHandler UpDownDelta;

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

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            return (keyData == Keys.Left || keyData == Keys.Right || keyData == Keys.Up || keyData == Keys.Down ||
             keyData == Keys.Home || keyData == Keys.End || keyData == Keys.PageDown || keyData == Keys.PageUp);
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
            if (selectAll) Select(0, Text.Length);
        }
        protected override void OnTextChanged(EventArgs e)
        {
            if ((string)EditingControlDataGridView.CurrentCell.Value != Text)
                UpDown?.Invoke(this, new DataGridViewUpDownCellEventArgs(EditingControlColIndex, EditingControlRowIndex, Text));

            base.OnTextChanged(e);
            EditingControlValueChanged = true;
            EditingControlDataGridView.NotifyCurrentCellDirty(true);
        }

        /// <summary>
        /// This event is triggered whenever the text changes. 
        /// Deprecated: Use UpDownDelta for synchronized relative updates.
        /// </summary>
        [Obsolete("UpDown event is deprecated. Please use UpDownDelta event for better support of relative value synchronization and multiple cell selection.")]
        public event DataGridViewUpDownCellEventHandler UpDown;
        #endregion

        #region UpDown

        /// <summary>
        /// Step value (can be a decimal)
        /// </summary>
        public decimal Step { get; set; } = 1m;

        // Internal state used to determine if the action is a continuous repeat (holding mouse or key down)
        private bool _isMouseDown = false;
        private bool _isKeyDownUp = false;
        private bool _isKeyDownDown = false;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _isMouseDown = true;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _isMouseDown = false;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Up) _isKeyDownUp = true;
            if (e.KeyCode == Keys.Down) _isKeyDownDown = true;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.Up) _isKeyDownUp = false;
            if (e.KeyCode == Keys.Down) _isKeyDownDown = false;
        }

        private bool IsRepeating() => _isMouseDown || _isKeyDownUp || _isKeyDownDown;

        public override void UpButton()
        {
            try
            {
                if (!Regex.IsMatch(Text, @"^-?[0-9][0-9,\.]*$")) return;

                decimal oldValue = decimal.Parse(Text);
                decimal delta = Step;
                decimal newValue = oldValue + delta;
                Text = newValue.ToString();

                // Fire the delta event (including column/row information)
                UpDownDelta?.Invoke(this, new DataGridViewUpDownDeltaEventArgs(EditingControlColIndex, EditingControlRowIndex, Text, delta, newValue, IsRepeating()));
            }
            catch (Exception) { }
        }
        public override void DownButton()
        {
            try
            {
                if (!Regex.IsMatch(Text, @"^-?[0-9][0-9,\.]*$")) return;

                decimal oldValue = decimal.Parse(Text);
                decimal delta = -Step;
                decimal newValue = oldValue + delta;
                Text = newValue.ToString();

                // Fire the delta event (including column/row information)
                UpDownDelta?.Invoke(this, new DataGridViewUpDownDeltaEventArgs(EditingControlColIndex, EditingControlRowIndex, Text, delta, newValue, IsRepeating()));
            }
            catch (Exception) { }
        }
        protected override void UpdateEditText() { }
        #endregion
    }

    /// <summary>
    /// Delegate and EventArgs related to UpDownDelta
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DataGridViewUpDownDeltaEventHandler(object sender, DataGridViewUpDownDeltaEventArgs e);

    public class DataGridViewUpDownDeltaEventArgs : DataGridViewCellEventArgs
    {
        /// <summary>
        /// Gets the text content associated with this instance.
        /// </summary>
        public string Text { get; private set; }
        /// <summary>
        /// The amount of change (can be negative)
        /// </summary>
        public decimal Delta { get; private set; }

        /// <summary>
        /// The new value after the change (equals OldValue + Delta)
        /// </summary>
        public decimal NewValue { get; private set; }

        /// <summary>
        /// Indicates if the action is a repeat (e.g., mouse or keyboard button held down)
        /// </summary>
        public bool IsRepeat { get; private set; }

        public DataGridViewUpDownDeltaEventArgs(int columnIndex, int rowIndex, string text, decimal delta, decimal newValue, bool isRepeat)
            : base(columnIndex, rowIndex)
        {
            Text = text;
            Delta = delta;
            NewValue = newValue;
            IsRepeat = isRepeat;
        }
    }

    [Obsolete("Use DataGridViewUpDownDeltaEventHandler instead.")]
    public delegate void DataGridViewUpDownCellEventHandler(object sender, DataGridViewUpDownCellEventArgs e);

    [Obsolete("Use DataGridViewUpDownDeltaEventArgs instead.")]
    public class DataGridViewUpDownCellEventArgs : DataGridViewCellEventArgs
    {
        public string Text { get; private set; }
        public DataGridViewUpDownCellEventArgs(int columnIndex, int rowIndex, string text) : base(columnIndex, rowIndex) { Text = text; }
    }
}
