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
        protected PictureBoxWithInterpolationMode pictureBox;

        public Point Offset
        {
            get => pictureBox.Location;
            set => pictureBox.Location = value.Negate();
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

            pictureBox.Paint += pictureBox_Paint;
            Controls.Add(pictureBox);
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (ImageMaskingType == MaskingType.Circular)
            {
                e.Graphics.DrawImage(Properties.Resources.Villager_Crop, pictureBox.Location.Negate());
            }
        }
    }
}