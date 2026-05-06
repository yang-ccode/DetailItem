using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DetailItem.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace DetailItem.UI
{
    /// <summary>
    /// Modeless form that lists all Detail Item elements from the active Revit view.
    ///
    /// Features:
    ///   • ComboBox to pick the parameter shown in ParameterName / ParameterValue columns.
    ///   • Checkbox column (+ Check All / Uncheck All toolbar buttons) for multi-row selection.
    ///   • "Highlight Checked" button — highlights checked elements in the Revit active view
    ///     via an ExternalEvent (form stays visible, no view switch).
    ///   • DataGridView row selection also triggers Revit element selection in real time.
    ///   • All sortable columns (click header to sort asc/desc).
    ///   • Refresh button to reload data from the current active view.
    /// </summary>
    public partial class DetailItemForm : Form
    {
        // ──────────────────────────────────────────────────────────────────────
        // Fields
        // ──────────────────────────────────────────────────────────────────────

        private readonly UIApplication   _uiApp;
        private readonly ExternalEvent   _externalEvent;
        private readonly SelectionHandler _selectionHandler;

        /// <summary>Backing DataTable – DataGridView is bound to its default DataView.</summary>
        private DataTable _dataTable = new DataTable();

        /// <summary>Prevents re-entrant checkbox-all logic.</summary>
        private bool _suppressCheckAllChange;

        /// <summary>Prevents SelectionChanged from firing while programmatic row selections happen.</summary>
        private bool _suppressSelectionChanged;

        // Column name constants used throughout the class
        private const string ColCheck   = "IsChecked";
        private const string ColView    = "ActiveView";
        private const string ColParam   = "ParameterName";
        private const string ColValue   = "ParameterValue";
        private const string ColElemId  = "ElementIdValue"; // hidden

        // ──────────────────────────────────────────────────────────────────────
        // Construction
        // ──────────────────────────────────────────────────────────────────────

        public DetailItemForm(
            UIApplication   uiApp,
            ExternalEvent   externalEvent,
            SelectionHandler selectionHandler)
        {
            _uiApp            = uiApp            ?? throw new ArgumentNullException(nameof(uiApp));
            _externalEvent    = externalEvent    ?? throw new ArgumentNullException(nameof(externalEvent));
            _selectionHandler = selectionHandler ?? throw new ArgumentNullException(nameof(selectionHandler));

            InitializeComponent();

            LoadParameterNames();
        }

        // ──────────────────────────────────────────────────────────────────────
        // Data loading helpers
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>Populate the parameter ComboBox from the active view.</summary>
        private void LoadParameterNames()
        {
            cboParameter.Items.Clear();
            cboParameter.Items.Add("(none)");

            var doc  = _uiApp.ActiveUIDocument?.Document;
            var view = _uiApp.ActiveUIDocument?.ActiveView;

            if (doc != null && view != null)
            {
                foreach (string name in DetailItemCollector.GetParameterNames(doc, view))
                    cboParameter.Items.Add(name);
            }

            // Selecting index 0 triggers cboParameter_SelectedIndexChanged → LoadData
            cboParameter.SelectedIndex = 0;
        }

        /// <summary>
        /// (Re)build the DataTable from the active view using the currently
        /// selected parameter.
        /// </summary>
        private void LoadData()
        {
            var doc  = _uiApp.ActiveUIDocument?.Document;
            var view = _uiApp.ActiveUIDocument?.ActiveView;

            // Build fresh DataTable
            var dt = BuildEmptyDataTable();

            if (doc != null && view != null)
            {
                string? selectedParam = cboParameter.SelectedItem as string;
                if (selectedParam == "(none)") selectedParam = null;

                var rows = DetailItemCollector.Collect(doc, view, selectedParam);

                foreach (var r in rows)
                {
                    dt.Rows.Add(
                        r.IsChecked,
                        r.ActiveView,
                        r.ParameterName,
                        r.ParameterValue,
                        r.ElementIdValue);
                }
            }

            // Replace grid data source atomically
            _suppressSelectionChanged = true;
            try
            {
                dgvItems.DataSource = null;
                _dataTable          = dt;
                dgvItems.DataSource = _dataTable.DefaultView;
            }
            finally
            {
                _suppressSelectionChanged = false;
            }

            ConfigureGridColumns();
            UpdateItemCount();
            UpdateCheckAllState();
        }

        /// <summary>Returns a new, correctly typed DataTable schema.</summary>
        private static DataTable BuildEmptyDataTable()
        {
            var dt = new DataTable();
            dt.Columns.Add(ColCheck,  typeof(bool));   // checkbox
            dt.Columns.Add(ColView,   typeof(string));
            dt.Columns.Add(ColParam,  typeof(string));
            dt.Columns.Add(ColValue,  typeof(string));
            dt.Columns.Add(ColElemId, typeof(long));    // hidden ElementId
            return dt;
        }

        // ──────────────────────────────────────────────────────────────────────
        // Grid configuration
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>Configure column headers, widths, and sort modes after binding.</summary>
        private void ConfigureGridColumns()
        {
            if (!dgvItems.Columns.Contains(ColCheck))  return;

            // Hidden ElementId column
            if (dgvItems.Columns.Contains(ColElemId))
            {
                dgvItems.Columns[ColElemId].Visible = false;
            }

            // Checkbox column
            var chkCol = dgvItems.Columns[ColCheck];
            chkCol.HeaderText  = "✓";
            chkCol.Width       = 36;
            chkCol.ReadOnly    = false;  // only this column is editable
            chkCol.SortMode    = DataGridViewColumnSortMode.Automatic;

            // ActiveView
            dgvItems.Columns[ColView].HeaderText = "Active View";
            dgvItems.Columns[ColView].Width      = 180;
            dgvItems.Columns[ColView].ReadOnly   = true;
            dgvItems.Columns[ColView].SortMode   = DataGridViewColumnSortMode.Automatic;

            // ParameterName
            dgvItems.Columns[ColParam].HeaderText = "Parameter Name";
            dgvItems.Columns[ColParam].Width      = 220;
            dgvItems.Columns[ColParam].ReadOnly   = true;
            dgvItems.Columns[ColParam].SortMode   = DataGridViewColumnSortMode.Automatic;

            // ParameterValue – fills remaining width
            dgvItems.Columns[ColValue].HeaderText  = "Parameter Value";
            dgvItems.Columns[ColValue].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvItems.Columns[ColValue].ReadOnly     = true;
            dgvItems.Columns[ColValue].SortMode     = DataGridViewColumnSortMode.Automatic;
        }

        // ──────────────────────────────────────────────────────────────────────
        // Status bar helpers
        // ──────────────────────────────────────────────────────────────────────

        private void UpdateItemCount()
        {
            int total   = _dataTable.Rows.Count;
            int checked_ = _dataTable.AsEnumerable()
                           .Count(r => r.Field<bool>(ColCheck));
            lblStatus.Text = $"{total} items  |  {checked_} checked";
        }

        /// <summary>
        /// Sync the Check-All checkbox header state without firing the change event.
        /// </summary>
        private void UpdateCheckAllState()
        {
            int total   = _dataTable.Rows.Count;
            int checked_ = _dataTable.AsEnumerable()
                           .Count(r => r.Field<bool>(ColCheck));

            _suppressCheckAllChange = true;
            try
            {
                chkSelectAll.CheckState =
                    checked_ == 0     ? CheckState.Unchecked :
                    checked_ == total ? CheckState.Checked   :
                                        CheckState.Indeterminate;
            }
            finally
            {
                _suppressCheckAllChange = false;
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // Revit selection
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Raise the ExternalEvent to update the Revit selection to
        /// <paramref name="elementIds"/>.
        /// </summary>
        private void SelectInRevit(IEnumerable<long> elementIds)
        {
            _selectionHandler.ElementIds = elementIds
                .Select(id => ElementIdHelper.Create(id))
                .ToList();
            _externalEvent.Raise();
        }

        // ──────────────────────────────────────────────────────────────────────
        // Event handlers – ComboBox
        // ──────────────────────────────────────────────────────────────────────

        private void cboParameter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        // ──────────────────────────────────────────────────────────────────────
        // Event handlers – DataGridView
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>
        /// When the user selects one or more grid rows, select the corresponding
        /// Detail Item elements in the Revit active view so they appear in the
        /// Properties Panel.
        /// </summary>
        private void dgvItems_SelectionChanged(object sender, EventArgs e)
        {
            if (_suppressSelectionChanged) return;

            var ids = dgvItems.SelectedRows
                .Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .Select(r =>
                {
                    var dv = r.DataBoundItem as DataRowView;
                    return dv == null ? 0L : (long)dv.Row[ColElemId];
                })
                .Where(id => id != 0L)
                .ToList();

            SelectInRevit(ids);
        }

        /// <summary>
        /// Handle cell value changes so that checkbox column edits trigger
        /// the Check-All state update.
        /// </summary>
        private void dgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvItems.Columns[e.ColumnIndex].DataPropertyName == ColCheck)
            {
                UpdateItemCount();
                UpdateCheckAllState();
            }
        }

        /// <summary>
        /// Commit checkbox edits immediately when the current cell leaves a
        /// checkbox cell (needed so DataTable sees the change right away).
        /// </summary>
        private void dgvItems_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvItems.CurrentCell is DataGridViewCheckBoxCell)
                dgvItems.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        // ──────────────────────────────────────────────────────────────────────
        // Event handlers – toolbar buttons
        // ──────────────────────────────────────────────────────────────────────

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressCheckAllChange) return;

            // Indeterminate → treat as "check all"
            bool check = chkSelectAll.CheckState != CheckState.Unchecked;

            _suppressSelectionChanged = true;
            try
            {
                foreach (DataRow row in _dataTable.Rows)
                    row[ColCheck] = check;

                dgvItems.Refresh();
            }
            finally
            {
                _suppressSelectionChanged = false;
            }

            UpdateItemCount();
        }

        private void btnHighlightChecked_Click(object sender, EventArgs e)
        {
            var ids = _dataTable.AsEnumerable()
                .Where(r => r.Field<bool>(ColCheck))
                .Select(r => r.Field<long>(ColElemId))
                .ToList();

            SelectInRevit(ids);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadParameterNames();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
