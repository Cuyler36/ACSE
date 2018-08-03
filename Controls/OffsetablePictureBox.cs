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

    public class OffsetablePictureBox : Panel
    {
        protected PictureBoxWithInterpolationMode PictureBox;

        public Point Offset
        {
            get => PictureBox.Location;
            set => PictureBox.Location = value.Negate();
        }

        public Image Image
        {
            get => PictureBox.Image;
            set
            {
                PictureBox.Image = value;
                if (value != null)
                {
                    PictureBox.Size = Image.Size;
                }
            }
        }

        public MaskingType ImageMaskingType = MaskingType.Circular;

        public OffsetablePictureBox()
        {
            PictureBox = new PictureBoxWithInterpolationMode
            {
                SizeMode = PictureBoxSizeMode.AutoSize,
                InterpolationMode = (InterpolationMode)Properties.Settings.Default.ImageResizeMode
            };

            PictureBox.Paint += pictureBox_Paint;
            Controls.Add(PictureBox);
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (ImageMaskingType == MaskingType.Circular)
            {
                e.Graphics.DrawImage(Properties.Resources.Villager_Crop, PictureBox.Location.Negate());
            }
        }
    }
}