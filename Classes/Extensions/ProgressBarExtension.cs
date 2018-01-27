using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    public enum ProgressBarState : int
    {
        Normal = 1,
        Error = 2,
        Warning = 3
    }

    public static class ProgressBarExtension
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);

        public static void SetState(this ProgressBar progressBar, ProgressBarState state)
        {
            SendMessage(progressBar.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }

        public static ProgressBarState GetState(this ProgressBar progressBar)
        {
            return (ProgressBarState)(int)SendMessage(progressBar.Handle, 1041, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
