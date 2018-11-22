using System.Windows.Forms;

namespace ACSE.WinForms.Utilities
{
    public static class ToolTipManager
    {
        /// <summary>
        /// The shared <see cref="ToolTip"/> instance.
        /// </summary>
        private static readonly ToolTip ToolTip = new ToolTip {ShowAlways = true};

        /// <summary>
        /// Gets the shared <see cref="ToolTip"/> instance.
        /// </summary>
        /// <returns><see cref="ToolTip"/> SharedToolTip</returns>
        public static ToolTip GetSharedToolTip() => ToolTip;
    }
}
