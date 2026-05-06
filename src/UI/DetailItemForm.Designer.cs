namespace DetailItem.UI
{
    partial class DetailItemForm
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer components = null;

        // ── Controls ──────────────────────────────────────────────────────────
        private System.Windows.Forms.Panel          pnlTop;
        private System.Windows.Forms.Label          lblParameter;
        private System.Windows.Forms.ComboBox       cboParameter;
        private System.Windows.Forms.Button         btnRefresh;

        private System.Windows.Forms.Panel          pnlToolbar;
        private System.Windows.Forms.CheckBox       chkSelectAll;
        private System.Windows.Forms.Button         btnHighlightChecked;
        private System.Windows.Forms.Label          lblStatus;
        private System.Windows.Forms.Button         btnClose;

        private System.Windows.Forms.DataGridView   dgvItems;

        // ──────────────────────────────────────────────────────────────────────

        /// <summary>Clean up any resources being used.</summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>Required method for Designer support.</summary>
        private void InitializeComponent()
        {
            this.pnlTop             = new System.Windows.Forms.Panel();
            this.lblParameter       = new System.Windows.Forms.Label();
            this.cboParameter       = new System.Windows.Forms.ComboBox();
            this.btnRefresh         = new System.Windows.Forms.Button();

            this.pnlToolbar         = new System.Windows.Forms.Panel();
            this.chkSelectAll       = new System.Windows.Forms.CheckBox();
            this.btnHighlightChecked = new System.Windows.Forms.Button();
            this.lblStatus          = new System.Windows.Forms.Label();
            this.btnClose           = new System.Windows.Forms.Button();

            this.dgvItems           = new System.Windows.Forms.DataGridView();

            this.pnlTop.SuspendLayout();
            this.pnlToolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.SuspendLayout();

            // ── pnlTop ────────────────────────────────────────────────────────
            this.pnlTop.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pnlTop.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Height    = 42;
            this.pnlTop.Padding   = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.pnlTop.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                this.lblParameter,
                this.cboParameter,
                this.btnRefresh,
            });

            // lblParameter
            this.lblParameter.AutoSize  = true;
            this.lblParameter.Location  = new System.Drawing.Point(8, 13);
            this.lblParameter.Text      = "Parameter:";
            this.lblParameter.Font      = new System.Drawing.Font("Segoe UI", 9f);

            // cboParameter
            this.cboParameter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboParameter.Location      = new System.Drawing.Point(80, 10);
            this.cboParameter.Width         = 280;
            this.cboParameter.Font          = new System.Drawing.Font("Segoe UI", 9f);
            this.cboParameter.SelectedIndexChanged += new System.EventHandler(this.cboParameter_SelectedIndexChanged);

            // btnRefresh
            this.btnRefresh.Text     = "⟳  Refresh";
            this.btnRefresh.Location = new System.Drawing.Point(370, 9);
            this.btnRefresh.Width    = 90;
            this.btnRefresh.Height   = 26;
            this.btnRefresh.Font     = new System.Drawing.Font("Segoe UI", 9f);
            this.btnRefresh.Click   += new System.EventHandler(this.btnRefresh_Click);

            // ── pnlToolbar ────────────────────────────────────────────────────
            this.pnlToolbar.BackColor = System.Drawing.SystemColors.Control;
            this.pnlToolbar.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlToolbar.Height    = 38;
            this.pnlToolbar.Padding   = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.pnlToolbar.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                this.chkSelectAll,
                this.btnHighlightChecked,
                this.lblStatus,
                this.btnClose,
            });

            // chkSelectAll
            this.chkSelectAll.Text           = "Check All";
            this.chkSelectAll.Location       = new System.Drawing.Point(8, 10);
            this.chkSelectAll.AutoSize       = true;
            this.chkSelectAll.ThreeState     = true;
            this.chkSelectAll.Font           = new System.Drawing.Font("Segoe UI", 9f);
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);

            // btnHighlightChecked
            this.btnHighlightChecked.Text     = "Highlight Checked";
            this.btnHighlightChecked.Location = new System.Drawing.Point(110, 8);
            this.btnHighlightChecked.Width    = 130;
            this.btnHighlightChecked.Height   = 24;
            this.btnHighlightChecked.Font     = new System.Drawing.Font("Segoe UI", 9f);
            this.btnHighlightChecked.Click   += new System.EventHandler(this.btnHighlightChecked_Click);

            // lblStatus  (right-aligned)
            this.lblStatus.AutoSize  = false;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStatus.Location  = new System.Drawing.Point(250, 10);
            this.lblStatus.Width     = 300;
            this.lblStatus.Font      = new System.Drawing.Font("Segoe UI", 9f);
            this.lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.lblStatus.Text      = "0 items  |  0 checked";

            // btnClose (far right)
            this.btnClose.Text     = "Close";
            this.btnClose.Anchor   = System.Windows.Forms.AnchorStyles.Top
                                   | System.Windows.Forms.AnchorStyles.Right;
            this.btnClose.Location = new System.Drawing.Point(770, 8);
            this.btnClose.Width    = 75;
            this.btnClose.Height   = 24;
            this.btnClose.Font     = new System.Drawing.Font("Segoe UI", 9f);
            this.btnClose.Click   += new System.EventHandler(this.btnClose_Click);

            // ── dgvItems ──────────────────────────────────────────────────────
            this.dgvItems.Dock                  = System.Windows.Forms.DockStyle.Fill;
            this.dgvItems.AllowUserToAddRows     = false;
            this.dgvItems.AllowUserToDeleteRows  = false;
            this.dgvItems.AllowUserToResizeRows  = false;
            this.dgvItems.AutoGenerateColumns    = true;
            this.dgvItems.AutoSizeRowsMode       = System.Windows.Forms.DataGridViewAutoSizeRowsMode.None;
            this.dgvItems.ColumnHeadersHeight    = 28;
            this.dgvItems.ColumnHeadersHeightSizeMode =
                System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvItems.SelectionMode          = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvItems.MultiSelect            = true;
            this.dgvItems.RowHeadersVisible      = false;
            this.dgvItems.BackgroundColor        = System.Drawing.SystemColors.Window;
            this.dgvItems.BorderStyle            = System.Windows.Forms.BorderStyle.None;
            this.dgvItems.Font                   = new System.Drawing.Font("Segoe UI", 9f);
            this.dgvItems.GridColor              = System.Drawing.Color.Gainsboro;
            this.dgvItems.RowTemplate.Height     = 24;
            this.dgvItems.ClipboardCopyMode      = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;

            // Alternating row colour for readability
            this.dgvItems.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.AliceBlue;

            this.dgvItems.SelectionChanged           += new System.EventHandler(this.dgvItems_SelectionChanged);
            this.dgvItems.CellValueChanged           += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvItems_CellValueChanged);
            this.dgvItems.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvItems_CurrentCellDirtyStateChanged);

            // ── Form ──────────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(880, 560);
            this.Font                = new System.Drawing.Font("Segoe UI", 9f);
            this.MinimumSize         = new System.Drawing.Size(640, 400);
            this.Text                = "Detail Item Manager";
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ShowInTaskbar       = false;

            // Add controls (order matters: Fill panel must be added after Top panels)
            this.Controls.Add(this.dgvItems);
            this.Controls.Add(this.pnlToolbar);
            this.Controls.Add(this.pnlTop);

            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlToolbar.ResumeLayout(false);
            this.pnlToolbar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
