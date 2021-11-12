﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace GroupGridView
{
    /// <summary>
    /// GroupGridView
    /// This is an Expandable/Collapsable grid view which extends DataGridView.
    /// Reference: Expanding / Collapsing Data Grid View
    /// https://www.codeproject.com/Tips/1066176/Expanding-Collapsing-Data-Grid-View
    /// </summary>
    public class GroupGridView : DataGridView
    {
        private bool rowHeadersExist;
        private List<int> baseRows;
        private List<List<int>> groupRows;
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (rowHeadersExist || !GroupByEnabled) return;
            if (RowHeadersCollapse == null) RowHeadersCollapse = Properties.Resources.collapse_circle_16x;
            if (RowHeadersExpand == null) RowHeadersExpand = Properties.Resources.expand_circle_16x;
            if (RowHeadersSeparater == null) RowHeadersSeparater = Properties.Resources.separater_16x;
            if (RowHeadersSeparaterEnd == null) RowHeadersSeparaterEnd = Properties.Resources.separater_end_16x;

            RowHeadersVisible = true;
            RowHeadersWidth = 16;
            RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            rowHeadersExist = true;

            SuspendLayout();
            (baseRows, groupRows) = DetermineBaseRows();
            ColorBases(baseRows);
            ResumeLayout();
            RowPostPaint += GroupGridView_RowPostPaint;
            RowHeaderMouseClick += GroupGridView_RowHeaderMouseClick;
            Sorted += GroupGridView_SortCompare;
            CellEndEdit += GroupGridView_CellEndEdit;
            Refresh();
        }

        public enum Order
        {
            Ascending = 1,
            Descending = 2
        }

        [Category("Group"), Description("Determines if grouping behaviour is enabled. If set to false, it will be a normal gridview.")]
        public bool GroupByEnabled { get; set; } = true;

        [Category("Group"), Description("Add each column index which is required to match in order to form a group. The GroupBy Columns attribute grouping gridview that have the same values into groups of rows.")]
        public List<int> GroupByColumns { get; set; } = new List<int>(0);

        [Category("Group"), Description("Determine if the grouping gridview only compares the content before the underline.")]
        public bool GroupByColumnsOnlyBeforeUnderlineEnabled { get; set; } = false;

        [Category("Group"), Description("Determine if nonsequence row grouping is enabled.")]
        public bool GroupByNonsequenceEnabled { get; set; } = true;

        [Category("Group"), Description("Determine if grouping for null value of columns is enabled.")]
        public bool GroupByNullValueEnabled { get; set; } = false;

        [Category("BaseRow"), Description("Determines if single group for 'Base' Rows is enabled.")]
        public bool BaseRowSingleGroupEnabled { get; set; } = false;

        [Category("BaseRow"), Description("Determines which column is used to order groups. This is necessary to identify the 'Base' row for each group. A value of -1 means that it relies on existing order.")]
        public int BaseRowGroupOrderColumn { get; set; } = -1;

        [Category("BaseRow"), Description("Determines if Ascending or Descending order groups. Grouping order is based on the order column. Only effective when BaseRowGroupOrderColumn is set.")]
        public Order BaseRowGroupOrder { get; set; } = Order.Ascending;

        [Category("BaseRow"), Description("Determines if defines a Color for 'Base' Rows is enabled.")]
        public bool BaseRowColorEnabled { get; set; } = true;

        [Category("BaseRow"), Description("Defines a Color for 'Base' Rows in groups. Only effective when BaseRowColorEnabled is true.")]
        public Color BaseRowColor { get; set; } = Color.White;

        [Category("RowHeaders"), Description("Defines a Collapse image of 'Base' Rows in groups.")]
        public Bitmap RowHeadersCollapse { get; set; }

        [Category("RowHeaders"), Description("Defines a Expand image of 'Base' Rows in groups.")]
        public Bitmap RowHeadersExpand { get; set; }

        [Category("RowHeaders"), Description("Determines if separater wire for row headers of each group is enabled.")]
        public bool RowHeadersSeparaterWireEnabled { get; set; } = true;

        [Category("RowHeaders"), Description("Defines a Separater image for RowHeaders in groups.")]
        public Bitmap RowHeadersSeparater { get; set; }

        [Category("RowHeaders"), Description("Defines a SeparaterEnd image for RowHeaders in groups.")]
        public Bitmap RowHeadersSeparaterEnd { get; set; }

        #region event
        private void GroupGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (!RowHeadersVisible) return;

            string value = (string)Rows[e.RowIndex].HeaderCell.Tag;
            Image image = null;
            Rectangle headerBounds = new Rectangle(e.RowBounds.X, e.RowBounds.Y + (e.RowBounds.Height - RowHeadersWidth) / 2, RowHeadersWidth, RowHeadersWidth);

            switch (value)
            {
                case "+":
                case "-":
                    if (value == "+") image = new Bitmap(RowHeadersExpand);
                    else if (value == "-") image = new Bitmap(RowHeadersCollapse);
                    break;
                case "|":
                case "└":
                    if (!RowHeadersSeparaterWireEnabled) return;
                    headerBounds.Y = e.RowBounds.Y;
                    headerBounds.Height = e.RowBounds.Height;
                    if (value == "|") image = new Bitmap(RowHeadersSeparater);
                    else if (value == "└") image = new Bitmap(RowHeadersSeparaterEnd);
                    break;
                default:
                    return;
            }

            using (image) e.Graphics.DrawImage(image, headerBounds);
        }
        /// <summary>
        /// Determines to show(+) or hide(-) the rows by check RowHeader Tag value.
        /// </summary>
        private void GroupGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex > -1) return;

            int idx = baseRows.IndexOf(e.RowIndex);
            if (idx == -1) return;

            string value = (string)Rows[e.RowIndex].HeaderCell.Tag;
            if (value == null) return;

            var group = groupRows[idx];
            foreach (int rowid in group) if (rowid != e.RowIndex) Rows[rowid].Visible = value != "-";

            Rows[e.RowIndex].HeaderCell.Tag = value == "-" ? "+" : "-";
        }

        private void GroupGridView_SortCompare(object sender, EventArgs e)
        {
            Application.DoEvents();

            GroupRefresh();
        }

        private void GroupGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (Rows[e.RowIndex].IsNewRow || !GroupByColumns.Contains(e.ColumnIndex)) return;

            GroupRefresh();
        }
        #endregion
        #region private
        private (List<int> baseRows, List<List<int>> groupRows) DetermineBaseRows()
        {
            List<int> baseRows = new List<int>();
            List<int> confirmedRows = new List<int>();
            List<List<int>> groupRows = new List<List<int>>();
            try
            {
                for (int idx = 0; idx <= RowCount - 1; idx++)
                {
                    if (confirmedRows.Contains(idx)) continue;

                    List<int> group = new List<int>();
                    group.Add(idx);
                    confirmedRows.Add(idx);
                    for (int sRowId = 0; sRowId <= RowCount - 1; sRowId++)
                    {
                        if (confirmedRows.Contains(sRowId)) continue;

                        bool match = true;
                        foreach (int colIdx in GroupByColumns)
                        { //The GroupByColumns attribute grouping gridview that have the same values into groups of rows.
                            string value1 = (string)Rows[idx].Cells[colIdx].Value ?? "";
                            string value2 = (string)Rows[sRowId].Cells[colIdx].Value ?? "";
                            if (GroupByColumnsOnlyBeforeUnderlineEnabled)
                            {
                                value1 = value1.Split('_')[0];
                                value2 = value1.Split('_')[0];
                            }
                            if (value1 != value2 || (!GroupByNullValueEnabled && value1 == ""))
                            {
                                match = false;
                                break;
                            }
                        }
                        if (match)
                        {
                            group.Add(sRowId);
                            confirmedRows.Add(sRowId);
                        }
                        else if (GroupByNonsequenceEnabled) continue;
                        else break;
                    }
                    if (!BaseRowSingleGroupEnabled && group.Count <= 1) continue;

                    groupRows.Add(group);
                    if (BaseRowGroupOrderColumn < 0)
                    {
                        if ((string)Rows[group[0]].HeaderCell.Tag == null)
                        {
                            baseRows.Add(group[0]);
                            Rows[group[0]].HeaderCell.Tag = "-";
                            if (group.Count > 1) Rows[group[group.Count - 1]].HeaderCell.Tag = "└";
                            if (group.Count > 2) for (int gIdx = 1; gIdx < group.Count - 1; gIdx++) Rows[group[gIdx]].HeaderCell.Tag = "|";
                        }
                    }
                    else
                    {
                        List<(string value, int rowId)> sortlist = new List<(string value, int rowId)>();
                        foreach (int rowId in group)
                        {
                            if (Rows[rowId].IsNewRow) continue;
                            sortlist.Add(((string)Rows[rowId].Cells[BaseRowGroupOrderColumn].Value, rowId));
                        }

                        if (BaseRowGroupOrder == Order.Ascending) sortlist.Sort((a, b) => a.value.CompareTo(b.value));
                        else sortlist.Sort((a, b) => b.value.CompareTo(a.value));
                        if ((string)Rows[sortlist[0].rowId].HeaderCell.Tag == null)
                        {
                            baseRows.Add(sortlist[0].rowId);
                            Rows[sortlist[0].rowId].HeaderCell.Tag = "-";
                            if (sortlist.Count > 1) Rows[sortlist[sortlist.Count - 1].rowId].HeaderCell.Tag = "└";
                            if (sortlist.Count > 2) for (int sIdx = 1; sIdx < sortlist.Count - 1; sIdx++) Rows[sortlist[sIdx].rowId].HeaderCell.Tag = "|";
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return (baseRows, groupRows);
        }

        private void ColorBases(List<int> baseRows)
        {
            if (!BaseRowColorEnabled || baseRows.Count <= 0) return;

            foreach (int rowId in baseRows)
            {
                Rows[rowId].DefaultCellStyle.BackColor = BaseRowColor;
                Rows[rowId].HeaderCell.Style.BackColor = BaseRowColor;
            }
        }

        private void ClearAll()
        {
            if (baseRows != null) baseRows.Clear();
            if (groupRows != null) groupRows.Clear();
            for (int rowId = 0; rowId <= RowCount - 1; rowId++)
            {
                Rows[rowId].Visible = true;
                Rows[rowId].HeaderCell.Tag = null;
                if (BaseRowColorEnabled) Rows[rowId].DefaultCellStyle.BackColor = default;
            }
        }
        #endregion

        /// <summary>
        /// collapse or expand all rows.
        /// isCollapse: true is collapse, false is expand.
        /// </summary>
        public void CollapseExpandAll(bool isCollapse)
        {
            for (int rowIdx = 0; rowIdx <= Rows.Count - 1; rowIdx++)
            {
                if ((string)Rows[rowIdx].HeaderCell.Tag != (isCollapse ? "-" : "+") || !Rows[rowIdx].Visible) continue;

                foreach (List<int> groupIdx in groupRows)
                {
                    if (!groupIdx.Contains(rowIdx)) continue;

                    foreach (int sRowid in groupIdx) if (sRowid != rowIdx) Rows[sRowid].Visible = !isCollapse;
                }

                Rows[rowIdx].HeaderCell.Tag = (!isCollapse ? "-" : "+");
            }
        }

        /// <summary>
        /// Determine base rows and redraw control.
        /// </summary>
        public void GroupRefresh()
        {
            SuspendLayout();
            ClearAll();
            (baseRows, groupRows) = DetermineBaseRows();
            ColorBases(baseRows);
            Refresh();
            ResumeLayout();
        }
    }
}
