using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ACSE
{
    public enum MaskingType
    {
        None = 0,
        Circular = 1
    }

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

        public MaskingType ImageMaskingType = MaskingType.Circular;

        public OffsetablePictureBox()
        {
            pictureBox = new PictureBoxWithInterpolationMode
            {
                SizeMode = PictureBoxSizeMode.AutoSize,
                InterpolationMode = (InterpolationMode)Properties.Settings.Default.ImageResizeMode
            };

            Controls.Add(pictureBox);
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (ImageMaskingType == MaskingType.Circular)
            {
                //pictureBox.Image = ImageGeneration.CropToCircle(pictureBox.Image, new Point(-Offset.X + Size.Width / 2, -Offset.Y + Size.Height / 2), 32, BackColor);
            }
        }
    }
}