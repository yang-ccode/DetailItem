using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DetailItem.UI;

namespace DetailItem
{
    /// <summary>
    /// Entry point for the Detail Item Manager command.
    /// Registered in the .addin manifest as an ExternalCommand.
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        // Keep a single form instance so the user cannot open it twice.
        private static DetailItemForm? _form;

        public Result Execute(ExternalCommandData commandData,
                              ref string message,
                              ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;

            // If already open, just bring it to the front.
            if (_form != null && !_form.IsDisposed)
            {
                _form.BringToFront();
                _form.Activate();
                return Result.Succeeded;
            }

            // Create the external event + handler that will drive Revit element
            // selection from the modeless form.
            var selectionHandler = new SelectionHandler();
            var externalEvent = ExternalEvent.Create(selectionHandler);

            _form = new DetailItemForm(uiApp, externalEvent, selectionHandler);

            // Show as modeless so the user can interact with Revit while the form
            // is open.  Pass the Revit main window as IWin32Window owner so the
            // form stays on top.
            _form.Show(new RevitWindowHandle(uiApp.MainWindowHandle));

            return Result.Succeeded;
        }
    }
}
