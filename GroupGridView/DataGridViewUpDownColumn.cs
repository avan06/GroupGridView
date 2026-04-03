using System;
using System.ComponentModel;
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

        [Category("Data")]
        [Description("The amount to increment or decrement the value.")]
        [DefaultValue(1.0)]
        public decimal Step { get; set; } = 1m;

        public override object Clone()
        {
            DataGridViewUpDownColumn col = (DataGridViewUpDownColumn)base.Clone();
            col.Step = this.Step;
            return col;
        }

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
            if (DataGridView.EditingControl is DataGridViewUpDownEditingControl upDownControl)
            {
                upDownControl.Text = (string)Value;

                if (OwningColumn is DataGridViewUpDownColumn parentCol)
                {
                    upDownControl.Step = parentCol.Step;
                }
            }
        }

        public override Type EditType => typeof(DataGridViewUpDownEditingControl);

        public override object DefaultNewRowValue => null; //未編集の新規行に余計な初期値が出ないようにする
    }

    public class DataGridViewUpDownEditingControl : UpDownBase, IDataGridViewEditingControl
    {
        #region DataGridViewEditingControl
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
        #endregion

        #region UpDown
        /// <summary>
        /// Step value (can be a decimal)
        /// </summary>
        public decimal Step { get; set; } = 1m;

        /// <summary>
        /// Notifies up/down adjustments via a delta event. 
        /// Suitable for the outer application to apply changes to multiple cells simultaneously.
        /// </summary>
        public event DataGridViewUpDownDeltaEventHandler UpDownDelta;

        public override void UpButton() => FireUpDownDelta(Step);
        public override void DownButton() => FireUpDownDelta(-Step);

        // Internal state used to determine if the action is a continuous repeat (holding mouse or key down)
        private bool _isMouseDown = false;
        private bool _isKeyDownUp = false;
        private bool _isKeyDownDown = false;

        private bool IsRepeating() => _isMouseDown || _isKeyDownUp || _isKeyDownDown;

        private void FireUpDownDelta(decimal delta)
        {
            try
            {
                if (!Regex.IsMatch(Text, @"^-?[0-9][0-9,\.]*$")) return;

                decimal oldValue = decimal.Parse(Text);
                decimal newValue = oldValue + delta;
                Text = newValue.ToString();

                // Fire event with current repeat status
                // When called from UpButton, IsRepeating() is true.
                // When called from OnMouseUp (after setting flag), IsRepeating() is false.
                UpDownDelta?.Invoke(this, new DataGridViewUpDownDeltaEventArgs(
                    EditingControlColIndex, EditingControlRowIndex, Text, delta, newValue, IsRepeating()));
            }
            catch { }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _isMouseDown = true;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_isMouseDown)
            {
                _isMouseDown = false;

                // If no keys are being held after releasing the mouse, the interaction has completely finished.
                if (!IsRepeating())
                {
                    FireUpDownDelta(0); // Trigger one last time with Delta 0 and IsRepeat = false
                }
            }
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

            // Only process the state if the Up or Down key was released
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                // Record the repeat status prior to the key release
                bool wasRepeating = IsRepeating();

                if (e.KeyCode == Keys.Up) _isKeyDownUp = false;
                if (e.KeyCode == Keys.Down) _isKeyDownDown = false;

                // if the interaction is no longer repeating after the release, 
                // send the final signal (Delta 0, IsRepeat = false)
                if (wasRepeating && !IsRepeating())
                {
                    FireUpDownDelta(0); // Trigger one last time with Delta 0 and IsRepeat = false
                }
            }
        }

        protected override void UpdateEditText()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This event is triggered whenever the text changes. 
        /// Deprecated: Use UpDownDelta for synchronized relative updates.
        /// </summary>
        [Obsolete("UpDown event is deprecated. Please use UpDownDelta event for better support of relative value synchronization and multiple cell selection.")]
        public event DataGridViewUpDownCellEventHandler UpDown;
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
