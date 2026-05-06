using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace DetailItem.UI
{
    /// <summary>
    /// Handles the external event that updates the Revit element selection
    /// from the modeless form.  Because Revit API calls are only allowed on
    /// the main Revit thread, this handler is invoked via <see cref="ExternalEvent.Raise"/>.
    /// </summary>
    public class SelectionHandler : IExternalEventHandler
    {
        private readonly object _lock = new object();
        private IList<ElementId> _elementIds = new List<ElementId>();

        /// <summary>
        /// Element IDs to select in the active document.
        /// Set this (from the UI thread) before calling <see cref="ExternalEvent.Raise"/>.
        /// Reads and writes are protected by an internal lock so that <see cref="Execute"/>
        /// on the Revit main thread always sees a fully-assigned list.
        /// </summary>
        public IList<ElementId> ElementIds
        {
            // Return a defensive copy so callers cannot mutate the internal list
            // outside the lock, preserving thread safety.
            get { lock (_lock) { return new List<ElementId>(_elementIds); } }
            set { lock (_lock) { _elementIds = value != null ? new List<ElementId>(value) : new List<ElementId>(); } }
        }

        /// <inheritdoc/>
        public void Execute(UIApplication app)
        {
            // Take a snapshot while holding the lock so the UI thread cannot
            // replace _elementIds mid-read.
            IList<ElementId> ids;
            lock (_lock) { ids = new List<ElementId>(_elementIds); }

            UIDocument? uiDoc = app.ActiveUIDocument;
            if (uiDoc == null) return;

            uiDoc.Selection.SetElementIds(ids);
        }

        /// <inheritdoc/>
        public string GetName() => "DetailItem_SelectElements";
    }
}
