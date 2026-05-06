using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DetailItem.UI;
using System;
using System.Runtime.InteropServices;
using WinForms = System.Windows.Forms;

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

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public Result Execute(ExternalCommandData commandData,
                              ref string message,
                              ElementSet elements)
        {
            try
            {
                UIApplication uiApp = commandData.Application;

                DetailItemForm? form = _form;

                // If already open, just bring it to the front.
                if (form != null && !form.IsDisposed)
                {
                    // Restore if minimized using Form's WindowState property (inherited from Form)
                    if (form.WindowState == WinForms.FormWindowState.Minimized)
                        form.WindowState = WinForms.FormWindowState.Normal;

                    if (!form.Visible)
                        form.Show(new RevitWindowHandle(uiApp.MainWindowHandle));

                    form.BringToFront();
                    form.Activate();
                    SetForegroundWindow(form.Handle);
                    return Result.Succeeded;
                }

                // Create the external event + handler that will drive Revit element
                // selection from the modeless form.
                var selectionHandler = new SelectionHandler();
                var externalEvent = ExternalEvent.Create(selectionHandler);

                var newForm = new DetailItemForm(uiApp, externalEvent, selectionHandler);
                newForm.FormClosed += (_, __) => _form = null;

                _form = newForm;

                // Show as modeless so the user can interact with Revit while the form
                // is open.  Pass the Revit main window as IWin32Window owner so the
                // form stays on top.
                newForm.Show(new RevitWindowHandle(uiApp.MainWindowHandle));

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                _form = null;
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
