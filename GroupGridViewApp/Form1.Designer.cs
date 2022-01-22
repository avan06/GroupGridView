
using GroupGridView;

namespace GroupGridViewApp
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panelView = new System.Windows.Forms.Panel();
            this.ToolStrip1 = new System.Windows.Forms.ToolStrip();
            this.ToolStripExpandAll = new System.Windows.Forms.ToolStripButton();
            this.ToolStripCollapseAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.GroupGridView1 = new GroupGridView.GroupGridView();
            this.GroupOne = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GroupFactorTwo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrderFactor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelView.SuspendLayout();
            this.ToolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GroupGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelView
            // 
            this.panelView.Controls.Add(this.GroupGridView1);
            this.panelView.Controls.Add(this.ToolStrip1);
            this.panelView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelView.Location = new System.Drawing.Point(0, 0);
            this.panelView.Margin = new System.Windows.Forms.Padding(2);
            this.panelView.Name = "panelView";
            this.panelView.Size = new System.Drawing.Size(471, 311);
            this.panelView.TabIndex = 0;
            // 
            // ToolStrip1
            // 
            this.ToolStrip1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripExpandAll,
            this.ToolStripCollapseAll,
            this.toolStripSeparator1});
            this.ToolStrip1.Location = new System.Drawing.Point(0, 0);
            this.ToolStrip1.Name = "ToolStrip1";
            this.ToolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.ToolStrip1.Size = new System.Drawing.Size(471, 25);
            this.ToolStrip1.TabIndex = 2;
            this.ToolStrip1.Text = "ToolStrip1";
            // 
            // ToolStripExpandAll
            // 
            this.ToolStripExpandAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripExpandAll.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripExpandAll.Image")));
            this.ToolStripExpandAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripExpandAll.Name = "ToolStripExpandAll";
            this.ToolStripExpandAll.Size = new System.Drawing.Size(23, 22);
            this.ToolStripExpandAll.Text = "ExpandAll";
            this.ToolStripExpandAll.Click += new System.EventHandler(this.ToolStripExpandAll_Click);
            // 
            // ToolStripCollapseAll
            // 
            this.ToolStripCollapseAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripCollapseAll.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripCollapseAll.Image")));
            this.ToolStripCollapseAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripCollapseAll.Name = "ToolStripCollapseAll";
            this.ToolStripCollapseAll.Size = new System.Drawing.Size(23, 22);
            this.ToolStripCollapseAll.Text = "CollapseAll";
            this.ToolStripCollapseAll.Click += new System.EventHandler(this.ToolStripCollapseAll_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // GroupGridView1
            // 
            this.GroupGridView1.AllowUserToAddRows = false;
            this.GroupGridView1.AllowUserToDeleteRows = false;
            this.GroupGridView1.BackgroundColor = System.Drawing.Color.DimGray;
            this.GroupGridView1.BaseRowColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.GroupGridView1.BaseRowColorEnabled = true;
            this.GroupGridView1.BaseRowColorInterleaved = true;
            this.GroupGridView1.BaseRowGroupOrder = GroupGridView.GroupGridView.Order.Ascending;
            this.GroupGridView1.BaseRowGroupOrderColumn = -1;
            this.GroupGridView1.BaseRowSingleGroupEnabled = false;
            this.GroupGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GroupGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.GroupOne,
            this.GroupFactorTwo,
            this.OrderFactor});
            this.GroupGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GroupGridView1.GridColor = System.Drawing.Color.Silver;
            this.GroupGridView1.GroupByColumns = ((System.Collections.Generic.List<int>)(resources.GetObject("GroupGridView1.GroupByColumns")));
            this.GroupGridView1.GroupByColumnsOnlyBeforeUnderlineEnabled = false;
            this.GroupGridView1.GroupByEnabled = true;
            this.GroupGridView1.GroupByNonsequenceEnabled = false;
            this.GroupGridView1.GroupByNullValueEnabled = true;
            this.GroupGridView1.Location = new System.Drawing.Point(0, 25);
            this.GroupGridView1.Margin = new System.Windows.Forms.Padding(2);
            this.GroupGridView1.Name = "GroupGridView1";
            this.GroupGridView1.RowHeadersCollapse = ((System.Drawing.Bitmap)(resources.GetObject("GroupGridView1.RowHeadersCollapse")));
            this.GroupGridView1.RowHeadersExpand = ((System.Drawing.Bitmap)(resources.GetObject("GroupGridView1.RowHeadersExpand")));
            this.GroupGridView1.RowHeadersSeparater = ((System.Drawing.Bitmap)(resources.GetObject("GroupGridView1.RowHeadersSeparater")));
            this.GroupGridView1.RowHeadersSeparaterEnd = ((System.Drawing.Bitmap)(resources.GetObject("GroupGridView1.RowHeadersSeparaterEnd")));
            this.GroupGridView1.RowHeadersSeparaterWireEnabled = true;
            this.GroupGridView1.RowHeadersWidth = 16;
            this.GroupGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.GroupGridView1.Size = new System.Drawing.Size(471, 286);
            this.GroupGridView1.TabIndex = 0;
            this.GroupGridView1.TopLeftHeaderButtonEnabled = true;
            this.GroupGridView1.TopLeftHeaderCollapseAll = ((System.Drawing.Bitmap)(resources.GetObject("GroupGridView1.TopLeftHeaderCollapseAll")));
            this.GroupGridView1.TopLeftHeaderExpandAll = ((System.Drawing.Bitmap)(resources.GetObject("GroupGridView1.TopLeftHeaderExpandAll")));
            // 
            // GroupOne
            // 
            this.GroupOne.HeaderText = "GroupFactorOne";
            this.GroupOne.MinimumWidth = 8;
            this.GroupOne.Name = "GroupOne";
            this.GroupOne.Width = 150;
            // 
            // GroupFactorTwo
            // 
            this.GroupFactorTwo.HeaderText = "GroupTwo";
            this.GroupFactorTwo.MinimumWidth = 8;
            this.GroupFactorTwo.Name = "GroupFactorTwo";
            this.GroupFactorTwo.Width = 150;
            // 
            // OrderFactor
            // 
            this.OrderFactor.HeaderText = "OrderFactor";
            this.OrderFactor.MinimumWidth = 8;
            this.OrderFactor.Name = "OrderFactor";
            this.OrderFactor.Width = 150;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(471, 311);
            this.Controls.Add(this.panelView);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panelView.ResumeLayout(false);
            this.panelView.PerformLayout();
            this.ToolStrip1.ResumeLayout(false);
            this.ToolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GroupGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelView;
        internal GroupGridView.GroupGridView GroupGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn GroupOne;
        private System.Windows.Forms.DataGridViewTextBoxColumn GroupFactorTwo;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrderFactor;
        private System.Windows.Forms.ToolStrip ToolStrip1;
        private System.Windows.Forms.ToolStripButton ToolStripExpandAll;
        private System.Windows.Forms.ToolStripButton ToolStripCollapseAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

