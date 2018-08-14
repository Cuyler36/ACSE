namespace System.Windows.Forms
{
    public static class ControlExtension
    {
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

        public static void SetCenterMargins(this Control control, int leftPadding = 0, int rightPadding = 0)
        {
            if (control.Parent != null)
            {
                SetCenterMargins(control, control.Parent, leftPadding, rightPadding);
            }
        }
    }
}
