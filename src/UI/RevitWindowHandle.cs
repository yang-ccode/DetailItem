using System;
using System.Windows.Forms;

namespace DetailItem.UI
{
    /// <summary>
    /// Wraps a native window handle (HWND) as <see cref="IWin32Window"/> so that
    /// WinForms modeless forms can be parented to the Revit main window and stay
    /// on top of it without being forced modal.
    /// </summary>
    internal sealed class RevitWindowHandle : IWin32Window
    {
        public IntPtr Handle { get; }

        public RevitWindowHandle(IntPtr handle)
        {
            Handle = handle;
        }
    }
}
