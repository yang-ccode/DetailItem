using Autodesk.Revit.DB;

namespace DetailItem.Models
{
    /// <summary>
    /// Represents one row in the Detail Item data grid.
    /// </summary>
    public class DetailItemRow
    {
        /// <summary>Revit ElementId of the Detail Item element (stored as long for Revit 2024+ compatibility).</summary>
        public long ElementIdValue { get; set; }

        /// <summary>Name of the active view the element was collected from.</summary>
        public string ActiveView { get; set; } = string.Empty;

        /// <summary>
        /// Name of the parameter currently selected in the ComboBox.
        /// Empty when no parameter is selected.
        /// </summary>
        public string ParameterName { get; set; } = string.Empty;

        /// <summary>
        /// String representation of the selected parameter's value on this element.
        /// Empty when no parameter is selected or the element does not have the parameter.
        /// </summary>
        public string ParameterValue { get; set; } = string.Empty;

        /// <summary>Whether the checkbox in this row is checked.</summary>
        public bool IsChecked { get; set; }
    }
}
