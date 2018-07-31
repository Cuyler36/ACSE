namespace System.Windows.Forms
{
    public static class ControlExtension
    {
        private const int WM_SETREDRAW = 0x000B;

        public static void Suspend(this Control control)
        {
            Message msgSuspendUpdate = Message.Create(control.Handle, WM_SETREDRAW, IntPtr.Zero,
                IntPtr.Zero);

            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgSuspendUpdate);
        }

        public static void Resume(this Control control)
        {
            // Create a C "true" boolean as an IntPtr
            IntPtr wparam = new IntPtr(1);
            Message msgResumeUpdate = Message.Create(control.Handle, WM_SETREDRAW, wparam,
                IntPtr.Zero);

            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgResumeUpdate);

            control.Invalidate();
        }

        public static void DisposeChildren(this Control.ControlCollection collection)
        {
            if (collection.IsReadOnly) return;
            for (var i = collection.Count - 1; i >= 0; i--)
            {
                collection[i].Dispose();
            }
            collection.Clear();
        }

        public static void SetCenterMargins(this Control control, Control parent, int leftPadding = 0, int rightPadding = 0)
        {
            var margin = (int)(0.5f * (parent.Height - control.Height));
            control.Margin = new Padding(leftPadding, margin, rightPadding, margin);
        }
    }
}
