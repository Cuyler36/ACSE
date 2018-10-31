using System.Linq;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using ACSE.Core.Debug;
using ACSE.Core.Utilities;

namespace ACSE.Core.Imaging
{
    public static class ImageGeneration
    {
        public static Bitmap DrawGrid2(Image img, int cellSize, Size imageSize, Pen gridPen = null, bool resize = true,
            bool drawVertical = true, bool skipFirstLine = false)
        {
            if (gridPen == null)
                gridPen = Pens.Black;

            var gridBitmap = resize ? new Bitmap(imageSize.Width, imageSize.Height) : new Bitmap(img);

            using (var gridGraphics = Graphics.FromImage(gridBitmap))
            {
                gridGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                if (resize)
                    gridGraphics.DrawImage(img, new Rectangle(0, 0, imageSize.Width, imageSize.Height),
                        new RectangleF((float) -0.5, (float) -0.5, 32, 32), GraphicsUnit.Pixel);

                if (drawVertical)
                    for (var x = 0; x < gridBitmap.Width; x += cellSize)
                    {
                        gridGraphics.DrawLine(gridPen, x, 0, x, gridBitmap.Height);
                    }

                for (var y = skipFirstLine ? cellSize : 0; y < gridBitmap.Height; y += cellSize)
                {
                    gridGraphics.DrawLine(gridPen, 0, y, gridBitmap.Width, y);
                }
            }

            return gridBitmap;
        }

        public static Image GetTpcImage(byte[] tpcBytes)
        {
            if (tpcBytes.Length != 0x1400)
            {
                return null;
            }

            for (var i = tpcBytes.Length - 1; i > 0; i--)
            {
                if (i > 0 && tpcBytes[i - 1] == 0xFF && tpcBytes[i] == 0xD9)
                {
                    using (var ms = new MemoryStream(tpcBytes.Take(i).ToArray()))
                    {
                        return Image.FromStream(ms);
                    }
                }
            }

            DebugUtility.DebugManagerInstance.WriteLine("Unable to find JPEG End-of-File marker. No TPC?",
                DebugLevel.Error);
            return null;
        }
    }
}
