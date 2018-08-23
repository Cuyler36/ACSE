using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ACSE.Utilities
{
    public static class ImageUtility
    {
        /// <summary>
        /// Changes an image's opacity.
        /// </summary>
        /// <param name="image">The image whose transparency will be modified.</param>
        /// <param name="transparency">The transparency change amount. Clamped between 0 and 1.</param>
        public static Bitmap ChangeOpacity(Image image, double transparency)
        {
            if (image == null || image.PixelFormat.HasFlag(PixelFormat.Indexed)) return null;
            
            // Clamp transparency between 0 and 1, since it's a percentage.
            transparency = transparency.Clamp(0, 1);

            var bitmap = (Bitmap) image.Clone();

            // Modify image data
            var (bitmapData, scanPtr, pixelData) = GetBitmapInfo(bitmap);
            for (var i = 0; i < pixelData.Length; i += 4)
            {
                pixelData[i + 3] = (byte) (pixelData[i + 3] * transparency);
            }

            // Copy data & unlock bitmap data
            Marshal.Copy(scanPtr, pixelData, 0, pixelData.Length);
            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }

        public static Bitmap LayerImage(in Image baseImage, in Image overlayImage, int x, int y)
        {
            if (baseImage == null) return null;
            if (overlayImage == null) return baseImage as Bitmap;

            var layeredBitmap = new Bitmap(baseImage.Width, baseImage.Height);
            using (var graphics = Graphics.FromImage(layeredBitmap))
            {
                graphics.DrawImage(baseImage, 0, 0);
                graphics.DrawImage(overlayImage, x, y);
            }

            return layeredBitmap;
        }

        public static Bitmap LayerImage(in Image baseImage, in Image overlayImage, int x, int y, double overlayTransparency)
        {
            if (baseImage == null) return null;
            if (overlayImage == null) return baseImage as Bitmap;

            return LayerImage(baseImage, ChangeOpacity(overlayImage, overlayTransparency), x, y);
        }

        public static Bitmap OverlayColor(in Image image, Color color)
        {
            if (image == null || image.PixelFormat.HasFlag(PixelFormat.Indexed)) return null;

            var bitmap = (Bitmap) image.Clone();

            // Modify image data
            var (bitmapData, scanPtr, pixelData) = GetBitmapInfo(bitmap);
            for (var i = 0; i < pixelData.Length; i += 4)
            {
                // Skip transparent pixels
                if (pixelData[i + 3] == 0) continue;
                pixelData[i + 0] = color.R;
                pixelData[i + 1] = color.G;
                pixelData[i + 2] = color.B;
            }

            // Copy data & unlock bitmap data
            Marshal.Copy(scanPtr, pixelData, 0, pixelData.Length);
            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }

        public static Bitmap OverlayColor(in Image image, byte r, byte g, byte b) =>
            OverlayColor(image, Color.FromArgb(0xFF, r, g, b));

        public static Bitmap MakeGrayscale(in Image image)
        {
            if (image == null || image.PixelFormat.HasFlag(PixelFormat.Indexed)) return null;

            var bitmap = (Bitmap) image.Clone();

            // Modify image data
            var (bitmapData, scanPtr, pixelData) = GetBitmapInfo(bitmap);

            for (var i = 0; i < pixelData.Length; i += 4)
            {
                // Skip transparent pixels
                if (pixelData[i + 3] == 0) continue;
                var grayValue =
                    (byte) ((pixelData[i + 2] * 0.3 + pixelData[i + 1] * 0.59 + pixelData[i + 0] * 0.11) / 3);
                pixelData[i + 0] = grayValue;
                pixelData[i + 1] = grayValue;
                pixelData[i + 2] = grayValue;
            }

            // Copy data & unlock bitmap data
            Marshal.Copy(scanPtr, pixelData, 0, pixelData.Length);
            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }

        public static Bitmap ColorizeGrayscale(in Image image, Color replacingColor)
        {
            if (image == null || image.PixelFormat.HasFlag(PixelFormat.Indexed)) return null;

            var bitmap = (Bitmap) image.Clone();

            // Modify image data
            var (bitmapData, scanPtr, pixelData) = GetBitmapInfo(bitmap);

            for (var i = 0; i < pixelData.Length; i += 4)
            {
                // Skip transparent pixels & pixels that aren't gray
                if (pixelData[i + 3] == 0 || pixelData[i + 0] != pixelData[i + 1] || pixelData[i + 1] != pixelData[i + 2]) continue;

                var intensity = pixelData[i + 0] / 255.0d;
                pixelData[i + 0] = (byte) (replacingColor.R * intensity);
                pixelData[i + 1] = (byte) (replacingColor.G * intensity);
                pixelData[i + 2] = (byte) (replacingColor.B * intensity);
            }

            // Copy data & unlock bitmap data
            Marshal.Copy(scanPtr, pixelData, 0, pixelData.Length);
            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }

        public static Bitmap ColorizeGrayscale(in Image image, byte r, byte g, byte b) =>
            ColorizeGrayscale(image, Color.FromArgb(0xFF, r, g, b));

        private static (BitmapData data, IntPtr pointer, byte[] output) GetBitmapInfo(in Bitmap bitmap)
        {
            // Allocate pixel data buffer and lock bitmap for reading
            var pixelData = new byte[bitmap.Width * bitmap.Height * 4];
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
                bitmap.PixelFormat);

            // Copy pixel data from Scan0 to allocated byte array
            Marshal.Copy(bitmapData.Scan0, pixelData, 0, pixelData.Length);

            return (bitmapData, bitmapData.Scan0, pixelData);
        }
    }
}
