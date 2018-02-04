using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ACSE
{
    class OffsetablePictureBox : Panel
    {
        private PictureBoxWithInterpolationMode pictureBox;

        public Point Offset
        {
            get => pictureBox.Location;
            set => pictureBox.Location = new Point(-value.X, -value.Y);
        }

        public Image Image
        {
            get => pictureBox.Image;
            set
            {
                pictureBox.Image = value;
                if (value != null)
                {
                    pictureBox.Size = Image.Size;
                }
            }
        }

        public OffsetablePictureBox()
        {
            pictureBox = new PictureBoxWithInterpolationMode();
            pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox.InterpolationMode = (InterpolationMode)Properties.Settings.Default.ImageResizeMode;

            Controls.Add(pictureBox);
        }
    }
}