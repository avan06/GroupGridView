using System;
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
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (rowHeadersExist || !GroupByEnabled) return;
            if (RowHeadersCollapse == null) RowHeadersCollapse = Properties.Resources.collapse_circle_16x;
            if (RowHeadersExpand == null) RowHeadersExpand = Properties.Resources.expand_circle_16x;
            if (RowHeadersSeparater == null) RowHeadersSeparater = Properties.Resources.separater_16x;
            if (RowHeadersSeparaterEnd == null) RowHeadersSeparaterEnd = Properties.Resources.separater_end_16x;
            if (TopLeftHeaderCollapseAll == null) TopLeftHeaderCollapseAll = Properties.Resources.collapseAll_16x;
            if (TopLeftHeaderExpandAll == null) TopLeftHeaderExpandAll = Properties.Resources.expandAll_16x;

            TopLeftHeaderCell.Tag = "-";
            RowHeadersVisible = true;
            RowHeadersWidth = 16;
            RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            rowHeadersExist = true;

            SuspendLayout();
            (BaseRows, GroupRows) = DetermineBaseRows();
            ColorBases(BaseRowColorInterleaved);
            ResumeLayout();
            CellPainting += GroupGridView_CellPainting;
            RowPostPaint += GroupGridView_RowPostPaint;
            RowHeaderMouseClick += GroupGridView_RowHeaderMouseClick;
            Sorted += GroupGridView_SortCompare;
            CellEndEdit += GroupGridView_CellEndEdit;
            Refresh();
        }

        protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex == -1) base.OnRowHeaderMouseClick(e);
            else base.OnCellMouseDown(e);
        }

        public enum Order
        {
            Ascending = 1,
            Descending = 2
        }

        #region properties
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<int> BaseRows { get; private set; }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<List<int>> GroupRows { get; private set; }

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

        [Category("BaseRow"), Description("Determines if to make the group list color interleaved when BaseRowColorEnabled is enabled.")]
        public bool BaseRowColorInterleaved { get; set; } = false;

        [Category("BaseRow"), Description("Defines a Color for 'Base' Rows in groups. Only effective when BaseRowColorEnabled is true.")]
        public Color BaseRowColor { get; set; } = Color.White;

        [Category("TopLeftHeader"), Description("Determines if CollapseExpandAll button for TopLeft Header is enabled.")]
        public bool TopLeftHeaderButtonEnabled { get; set; } = true;

        [Category("TopLeftHeader"), Description("Defines a CollapseAll image for TopLeft Header.")]
        public Bitmap TopLeftHeaderCollapseAll { get; set; }

        [Category("TopLeftHeader"), Description("Defines a ExpandAll image for TopLeft Header.")]
        public Bitmap TopLeftHeaderExpandAll { get; set; }

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
        #endregion

        #region event
        private void GroupGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (!RowHeadersVisible || !TopLeftHeaderButtonEnabled || e.RowIndex != -1 || e.ColumnIndex != -1) return;

            Image image = null;
            string topLeftValue = (string)TopLeftHeaderCell.Tag;

            if (topLeftValue == "+") image = new Bitmap(TopLeftHeaderExpandAll);
            else if (topLeftValue == "-") image = new Bitmap(TopLeftHeaderCollapseAll);
            else return;

            using (image) e.Graphics.DrawImage(image, e.CellBounds);
            e.Handled = true;
        }

        private void GroupGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (!RowHeadersVisible) return;

            string value = (string)Rows[e.RowIndex].HeaderCell.Tag;
            Image image = null;
            Rectangle headerBounds = new Rectangle(e.RowBounds.X, e.RowBounds.Y + (e.RowBounds.Height - RowHeadersWidth) / 2, RowHeadersWidth, RowHeadersWidth);

            if (value == "+") image = new Bitmap(RowHeadersExpand);
            else if (value == "-") image = new Bitmap(RowHeadersCollapse);
            else if (RowHeadersSeparaterWireEnabled && (value == "|" || value == "└"))
            {
                headerBounds.Y = e.RowBounds.Y;
                headerBounds.Height = e.RowBounds.Height;
                if (value == "|") image = new Bitmap(RowHeadersSeparater);
                else if (value == "└") image = new Bitmap(RowHeadersSeparaterEnd);
            }
            else return;

            using (image) e.Graphics.DrawImage(image, headerBounds);
        }

        /// <summary>
        /// Determines to ExpandAll(+) or CollapseAll(-) all rows by check TopLeftHeader Tag value.
        /// Determines to Expand(+) or Collapse(-) the rows by check RowHeader Tag value.
        /// </summary>
        private void GroupGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!RowHeadersVisible) return;
            else if (TopLeftHeaderButtonEnabled && e.RowIndex == -1 && e.ColumnIndex == -1)
            {
                bool isCollapse = (string)TopLeftHeaderCell.Tag == "-";
                CollapseExpandAll(isCollapse);
                TopLeftHeaderCell.Tag = isCollapse ? "+" : "-";
                return;
            }
            else if (e.RowIndex < 0 || e.ColumnIndex > -1) return;

            int idx = BaseRows.IndexOf(e.RowIndex);
            if (idx == -1) return;

            string value = (string)Rows[e.RowIndex].HeaderCell.Tag;
            if (value == null) return;

            var group = GroupRows[idx];
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
                                value2 = value2.Split('_')[0];
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

            confirmedRows.Clear();
            confirmedRows = null;

            return (baseRows, groupRows);
        }

        private void ColorBases(bool interleaved = false)
        {
            if (!BaseRowColorEnabled || BaseRows.Count <= 0) return;

            Color backColor = default;
            for (int idx = 0; idx < Rows.Count; idx++)
            {
                string tag = (string)Rows[idx].HeaderCell.Tag;
                if (tag == "+" || tag == "-") backColor = backColor == default ? BaseRowColor : default;
                else if (interleaved && (tag == "|" || tag == "└")) { }
                else
                {
                    backColor = default;
                    continue;
                }

                Rows[idx].DefaultCellStyle.BackColor = backColor;
                Rows[idx].HeaderCell.Style.BackColor = backColor;
            }
        }

        private void ClearAll()
        {
            if (BaseRows != null) BaseRows.Clear();
            if (GroupRows != null) GroupRows.Clear();
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
        public void CollapseExpandAll(bool? isCollapse = null)
        {
            if (isCollapse == null) isCollapse = (string)Rows[0].HeaderCell.Tag == "-";
            for (int rowIdx = 0; rowIdx <= Rows.Count - 1; rowIdx++)
            {
                if ((string)Rows[rowIdx].HeaderCell.Tag != ((bool)isCollapse ? "-" : "+") || !Rows[rowIdx].Visible) continue;

                foreach (List<int> groupIdx in GroupRows)
                {
                    if (!groupIdx.Contains(rowIdx)) continue;

                    foreach (int sRowid in groupIdx) if (sRowid != rowIdx) Rows[sRowid].Visible = !(bool)isCollapse;
                }

                Rows[rowIdx].HeaderCell.Tag = (!(bool)isCollapse ? "-" : "+");
            }
        }

        /// <summary>
        /// Determine base rows and redraw control.
        /// </summary>
        public void GroupRefresh()
        {
            SuspendLayout();
            ClearAll();
            (BaseRows, GroupRows) = DetermineBaseRows();
            ColorBases(BaseRowColorInterleaved);
            Refresh();
            ResumeLayout();
        }
    }
}
