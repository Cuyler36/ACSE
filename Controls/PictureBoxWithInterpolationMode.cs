using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ACSE
{
    /// <inheritdoc />
    /// <summary>
    /// Inherits from PictureBox; adds Interpolation Mode Setting
    /// </summary>
    public class PictureBoxWithInterpolationMode : PictureBox
    {
        public InterpolationMode? InterpolationMode { get; set; }
        public bool UseInternalInterpolationSetting { get; set; }

        protected override void OnPaint(PaintEventArgs paintEventArgs)
        {
            if (UseInternalInterpolationSetting || InterpolationMode == null)
                paintEventArgs.Graphics.InterpolationMode = (InterpolationMode)Properties.Settings.Default.ImageResizeMode;
            else
                paintEventArgs.Graphics.InterpolationMode = InterpolationMode.Value;
            try { base.OnPaint(paintEventArgs); }
            catch
            {
                // ignored
            }
        }

        protected override void OnPaintBackground(PaintEventArgs paintEventArgs)
        {
            if (UseInternalInterpolationSetting || InterpolationMode == null)
                paintEventArgs.Graphics.InterpolationMode = (InterpolationMode)Properties.Settings.Default.ImageResizeMode;
            else
                paintEventArgs.Graphics.InterpolationMode = InterpolationMode.Value;
            try { base.OnPaintBackground(paintEventArgs); }
            catch
            {
                // ignored
            }
        }
    }
}
