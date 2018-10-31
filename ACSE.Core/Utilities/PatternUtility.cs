using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using ACSE.Core.Imaging;

namespace ACSE.Core.Utilities
{
    public static class PatternUtility
    {
        private static byte[] DecompressC4(IReadOnlyList<byte> c4ImageData)
        {
            var decompressedData = new byte[c4ImageData.Count * 2];

            for (var i = 0; i < c4ImageData.Count; i++)
            {
                var decompressIdx = i * 2;
                decompressedData[decompressIdx] = (byte)(c4ImageData[i] >> 4);
                decompressedData[decompressIdx + 1] = (byte)(c4ImageData[i] & 0x0F);
            }

            return decompressedData;
        }

        private static byte[] CompressC4(IReadOnlyList<byte> imageData)
        {
            var compressedData = new byte[imageData.Count / 2];

            for (var i = 0; i < compressedData.Length; i++)
            {
                var index = i * 2;
                var condensedPixel = (byte)(((imageData[index] & 0x0F) << 4) | (imageData[index + 1] & 0x0F));
                compressedData[i] = condensedPixel;
            }

            return compressedData;
        }

        private static byte[] C4ImageSubroutineDecode(IReadOnlyList<byte> c4ImageData, uint width = 32, uint height = 32)
        {
            var blockXCount = width / 8;
            var blockYCount = height / 8;

            var outputBuffer = new byte[c4ImageData.Count];
            var pixelIndex = 0;

            for (var yBlock = 0; yBlock < blockYCount; yBlock++)
            {
                for (var xBlock = 0; xBlock < blockXCount; xBlock++)
                {
                    for (var yPixel = 0; yPixel < 8; yPixel++)
                    {
                        for (var xPixel = 0; xPixel < 8; xPixel++)
                        {
                            var outputBufferIndex = (int)(width * 8 * yBlock + yPixel * width + xBlock * 8 + xPixel);
                            outputBuffer[outputBufferIndex] = c4ImageData[pixelIndex];
                            pixelIndex++;
                        }
                    }
                }
            }

            return outputBuffer;
        }

        private static byte[] C4ImageSubroutineEncode(IReadOnlyList<byte> c4ImageData, uint width = 32, uint height = 32)
        {
            var blockXCount = width / 8;
            var blockYCount = height / 8;

            var outputBuffer = new byte[c4ImageData.Count];
            uint outputBufferIndex = 0;

            for (var yBlock = 0; yBlock < blockYCount; yBlock++)
            {
                for (var xBlock = 0; xBlock < blockXCount; xBlock++)
                {
                    for (var yPixel = 0; yPixel < 8; yPixel++)
                    {
                        for (var xPixel = 0; xPixel < 8; xPixel++)
                        {
                            var pixelIndex = (int)(width * 8 * yBlock + yPixel * width + xBlock * 8 + xPixel);
                            outputBuffer[outputBufferIndex] = c4ImageData[pixelIndex];
                            outputBufferIndex++;
                        }
                    }
                }
            }

            return outputBuffer;
        }

        private static Bitmap CreateBitmap(byte[] patternBitmapBuffer, uint width = 32, uint height = 32)
        {
            var patternBitmap = new Bitmap((int)width, (int)height, PixelFormat.Format32bppArgb);
            var bitmapData = patternBitmap.LockBits(new Rectangle(0, 0, (int)width, (int)height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(patternBitmapBuffer, 0, bitmapData.Scan0, patternBitmapBuffer.Length);
            patternBitmap.UnlockBits(bitmapData);
            return patternBitmap;
        }

        public static byte[] DecodeC4(byte[] c4ImageData, uint width = 32, uint height = 32)
        {
            return C4ImageSubroutineDecode(DecompressC4(c4ImageData), width, height);
        }

        public static byte[] EncodeC4(byte[] imageData, uint width = 32, uint height = 32)
        {
            return CompressC4(C4ImageSubroutineEncode(imageData, width, height));
        }

        public static Bitmap C4PaletteMapToBitmap(byte[] decodedC4ImageData, uint[] palette, uint width = 32, uint height = 32)
        {
            var patternBitmapBuffer = new byte[4 * width * height];

            for (var i = 0; i < decodedC4ImageData.Length; i++)
                Buffer.BlockCopy(BitConverter.GetBytes(palette[Math.Max(0, decodedC4ImageData[i] - 1)]), 0, patternBitmapBuffer, i * 4, 4);
            
            return CreateBitmap(patternBitmapBuffer, width, height);
        }

        public static Bitmap GeneratePalettePreview(uint[] palette, int selectedColor = -1, uint width = 32, uint height = 480)
        {
            var paletteBitmapBuffer = new byte[width * height * 4];
            var paletteColorDataLength = (int)(width * (height / 15) * 4); // There are 15 colors in a Palette.
            var paletteIndex = -1;

            for (var i = 0; i < paletteBitmapBuffer.Length; i += 4)
            {
                if (i % paletteColorDataLength == 0)
                    paletteIndex++;

                if (paletteIndex < palette.Length && i < paletteBitmapBuffer.Length)
                    Buffer.BlockCopy(BitConverter.GetBytes(palette[paletteIndex]), 0, paletteBitmapBuffer, i, 4);
            }

            var preview = CreateBitmap(paletteBitmapBuffer, width, height);
            return ImageGeneration.DrawGrid2(preview, (int)width, new Size((int)width, (int)height), null, false, false, true);
        }

        public static byte[] CondenseNonBlockPattern(byte[] buffer)
        {
            var patternBuffer = new byte[buffer.Length / 2];
            for (var i = 0; i < patternBuffer.Length; i++)
            {
                var idx = i * 2;
                patternBuffer[i] = (byte)((buffer[idx + 1] << 4) | buffer[idx]);
            }
            return patternBuffer;
        }
    }
}
