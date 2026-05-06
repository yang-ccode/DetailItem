using Autodesk.Revit.DB;
using DetailItem.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DetailItem.Services
{
    /// <summary>
    /// Collects Detail Item elements and their parameter data from the Revit model.
    /// All methods are pure (read-only); no transaction is required.
    /// </summary>
    public static class DetailItemCollector
    {
        // BuiltInCategory value for Detail Items
        private const BuiltInCategory DetailItemCategory = BuiltInCategory.OST_DetailComponents;

        /// <summary>
        /// Returns all Detail Item instances visible in <paramref name="activeView"/>,
        /// each mapped to a row with the currently selected parameter resolved.
        /// </summary>
        /// <param name="doc">Active Revit document.</param>
        /// <param name="activeView">View used to scope the element collection.</param>
        /// <param name="selectedParameterName">
        ///   Parameter name whose value should be shown in the ParameterValue column.
        ///   Pass <see langword="null"/> or an empty string to leave both columns blank.
        /// </param>
        public static List<DetailItemRow> Collect(
            Document doc,
            View activeView,
            string? selectedParameterName)
        {
            if (doc is null) throw new ArgumentNullException(nameof(doc));
            if (activeView is null) throw new ArgumentNullException(nameof(activeView));

            var rows = new List<DetailItemRow>();

            var elements = new FilteredElementCollector(doc, activeView.Id)
                .OfCategory(DetailItemCategory)
                .WhereElementIsNotElementType()
                .ToElements();

            bool resolveParam = !string.IsNullOrWhiteSpace(selectedParameterName);

            foreach (Element elem in elements)
            {
                string paramName = string.Empty;
                string paramValue = string.Empty;

                if (resolveParam)
                {
                    Parameter? param = elem.LookupParameter(selectedParameterName!);
                    if (param != null)
                    {
                        paramName = param.Definition?.Name ?? selectedParameterName!;
                        paramValue = GetValueString(param);
                    }
                }

                rows.Add(new DetailItemRow
                {
                    ElementIdValue = ElementIdHelper.GetValue(elem.Id),
                    ActiveView     = activeView.Name,
                    ParameterName  = paramName,
                    ParameterValue = paramValue,
                    IsChecked      = false,
                });
            }

            return rows;
        }

        /// <summary>
        /// Returns a sorted, distinct list of parameter names available on any
        /// Detail Item element in <paramref name="activeView"/>.
        /// </summary>
        public static List<string> GetParameterNames(Document doc, View activeView)
        {
            if (doc is null) throw new ArgumentNullException(nameof(doc));
            if (activeView is null) throw new ArgumentNullException(nameof(activeView));

            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var elements = new FilteredElementCollector(doc, activeView.Id)
                .OfCategory(DetailItemCategory)
                .WhereElementIsNotElementType()
                .ToElements();

            foreach (Element elem in elements)
            {
                foreach (Parameter param in elem.Parameters)
                {
                    string? name = param.Definition?.Name;
                    if (!string.IsNullOrWhiteSpace(name))
                        names.Add(name!);
                }
            }

            return names.OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToList();
        }

        // ------------------------------------------------------------------
        // Private helpers
        // ------------------------------------------------------------------

        private static string GetValueString(Parameter param)
        {
            if (!param.HasValue)
                return string.Empty;

            return param.StorageType switch
            {
                StorageType.String    => param.AsString()     ?? string.Empty,
                StorageType.Integer   => param.AsInteger().ToString(),
                StorageType.Double    => param.AsValueString() ?? param.AsDouble().ToString("F4"),
                StorageType.ElementId => param.AsElementId() is ElementId eid
                                         ? ElementIdHelper.GetValue(eid).ToString()
                                         : string.Empty,
                _                     => string.Empty,
            };
        }
    }
}
