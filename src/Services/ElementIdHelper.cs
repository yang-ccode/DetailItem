using Autodesk.Revit.DB;

namespace DetailItem.Services
{
    /// <summary>
    /// Abstracts <c>ElementId</c> integer-value access so the same code compiles
    /// against both pre-2024 Revit (where <c>IntegerValue</c> returns <c>int</c>) and
    /// Revit 2024+ (where <c>Value</c> returns <c>long</c> and <c>IntegerValue</c> is
    /// deprecated).
    ///
    /// Compile with <c>-DREVIT2024_OR_GREATER</c> (or set the MSBuild property
    /// <c>DefineConstants</c>) when targeting Revit 2024 or later.
    /// </summary>
    internal static class ElementIdHelper
    {
        /// <summary>Returns the numeric value of an <see cref="ElementId"/>.</summary>
        public static long GetValue(ElementId id)
        {
#if REVIT2024_OR_GREATER
            return id.Value;
#else
            return id.IntegerValue;
#endif
        }

        /// <summary>
        /// Creates an <see cref="ElementId"/> from a stored numeric value.
        /// </summary>
        /// <remarks>
        /// On pre-2024 Revit the Revit API stores ElementIds internally as
        /// <c>int</c>, so any value that was obtained via <see cref="GetValue"/>
        /// on the same Revit version is guaranteed to fit within
        /// <see cref="int.MaxValue"/>.  The checked cast surfaces a clear
        /// <see cref="OverflowException"/> rather than silent data corruption
        /// should a future caller misuse this helper.
        /// </remarks>
        public static ElementId Create(long value)
        {
#if REVIT2024_OR_GREATER
            return new ElementId(value);
#else
            return new ElementId(checked((int)value));
#endif
        }
    }
}
