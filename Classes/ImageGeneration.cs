using System;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ACSE
{
    public static class ImageGeneration
    {
        //From: http://stackoverflow.com/questions/7350679/convert-a-bitmap-into-a-byte-array
        public static byte[] BitmapToByteArray(Bitmap bitmap)
        {
            BitmapData bmpdata = null;

            try
            {
                bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                var numbytes = bmpdata.Stride * bitmap.Height;
                var bytedata = new byte[numbytes];
                var ptr = bmpdata.Scan0;

                Marshal.Copy(ptr, bytedata, 0, numbytes);

                return bytedata;
            }
            finally
            {
                if (bmpdata != null)
                    bitmap.UnlockBits(bmpdata);
            }

        }

        private static void ReplaceGrayscaleColor(ref Bitmap editingImage, Color replacingColor)
        {
            for (var y = 0; y < editingImage.Height; y++)
            {
                for (var x = 0; x < editingImage.Width; x++)
                {
                    var pixel = editingImage.GetPixel(x, y);
                    if (pixel.A <= 0 || (pixel.R != pixel.B) || (pixel.R != pixel.G)) continue;
                    // The pixel is gray
                    var vibrance = pixel.R / 255.0f;

                    editingImage.SetPixel(x, y, Color.FromArgb(pixel.A, (int)(replacingColor.R * vibrance), (int)(replacingColor.G * vibrance), (int)(replacingColor.B * vibrance)));
                }
            }
        }

        public static void DrawBuriedIcons(Bitmap map, WorldItem[] items, int itemSize, bool useText = false)
        {
            var bitmapGraphics = Graphics.FromImage(map);
            bitmapGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            bitmapGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            bitmapGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            foreach (var item in items)
            {
                if (!item.Buried || item.Name.Equals("Empty")) continue;
                if (useText)
                {
                    bitmapGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                    bitmapGraphics.DrawString("X", new Font("Tahoma", itemSize), Brushes.White, new RectangleF(item.Location.X * itemSize, item.Location.Y * itemSize, itemSize, itemSize));
                }
                else
                    bitmapGraphics.DrawImage(Properties.Resources.Buried, item.Location.X * itemSize + 1, item.Location.Y * itemSize + 1, itemSize - 1, itemSize - 1);
            }
            bitmapGraphics.Flush();
            bitmapGraphics.Dispose();
        }

        public static Bitmap DrawGrid(Bitmap map, int itemSize, uint gridColor = 0xFFAAAAAA, int gridPixelSize = 1)
        {
            var bitmapGraphics = Graphics.FromImage(map);
            bitmapGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            bitmapGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            bitmapGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            int numLinesX = map.Width / itemSize, numLinesY = map.Height / itemSize;
            var gridPen = new Pen(new SolidBrush(Color.FromArgb((int)gridColor)))
            {
                Width = gridPixelSize
            };
            for (var y = 1; y < numLinesY; y++)
                bitmapGraphics.DrawLine(gridPen, 0, y * itemSize - 1, map.Width, y * itemSize - 1);
            for (var x = 1; x < numLinesX; x++)
                bitmapGraphics.DrawLine(gridPen, x * itemSize - 1, 0, x * itemSize - 1, map.Height);
            bitmapGraphics.Flush();
            bitmapGraphics.Dispose();
            return map;
        }

        public static Bitmap DrawGrid2(Image img, int cellSize, Size imageSize, Pen gridPen = null, bool resize = true, bool drawVertical = true, bool skipFirstLine = false)
        {
            if (gridPen == null)
                gridPen = Pens.Black;

            var gridBitmap = resize ? new Bitmap(imageSize.Width, imageSize.Height) : new Bitmap(img);

            using (var gridGraphics = Graphics.FromImage(gridBitmap))
            {
                gridGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                if (resize)
                    gridGraphics.DrawImage(img, new Rectangle(0, 0, imageSize.Width, imageSize.Height), new RectangleF((float)-0.5, (float)-0.5, 32, 32), GraphicsUnit.Pixel);

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

        public static Bitmap DrawAcreHighlight()
        {
            var acreHighlight = new Bitmap(64, 64);
            var bitmapGraphics = Graphics.FromImage(acreHighlight);
            var borderColor = new Pen(Color.FromArgb(0x80, Color.Gold))
            {
                Width = 8
            };
            bitmapGraphics.DrawRectangle(borderColor, new Rectangle(0, 0, 64, 64));
            bitmapGraphics.FillRectangle(new SolidBrush(Color.FromArgb(0x80, Color.Yellow)), new Rectangle(4, 4, 56, 56));
            bitmapGraphics.Flush();
            bitmapGraphics.Dispose();
            borderColor.Dispose();
            return acreHighlight;
        }

        public static Bitmap DrawBuilding(Bitmap acreMap, Building buildingToDraw, int itemSize,  bool useText = false)
        {
            var rectF = new RectangleF(buildingToDraw.XPos * itemSize, buildingToDraw.YPos * itemSize, itemSize, itemSize);
            var bitmapGraphics = Graphics.FromImage(acreMap);
            bitmapGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            bitmapGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            bitmapGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            if (useText)
            {
                bitmapGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                bitmapGraphics.DrawString("B", new Font("Tahoma", itemSize), Brushes.White, rectF);
            }
            else
                bitmapGraphics.DrawImage(Properties.Resources.Building, buildingToDraw.XPos * itemSize, buildingToDraw.YPos * itemSize, itemSize, itemSize);
            bitmapGraphics.Flush();
            bitmapGraphics.Dispose();
            return acreMap;
        }

        public static Bitmap DrawBuildings(Bitmap acreMap, Building[] buildingList, int acre, int itemSize)
        {
            if (buildingList == null)
                return acreMap;
            foreach (var b in buildingList)
            {
                if (b.Exists && b.AcreIndex == acre)
                {
                    DrawBuilding(acreMap, b, itemSize);
                }
            }
            return acreMap;
        }

        public static Bitmap DrawInnerHouseExtents(Bitmap furnitureMap, int floorSize = 8)
        {
            using (var bitmapGraphics = Graphics.FromImage(furnitureMap))
            {
                var corner = (16 - floorSize) / 2;
                using (var borderPen = new Pen(Color.Gray, 2))
                {
                    borderPen.Alignment = PenAlignment.Inset;
                    bitmapGraphics.DrawRectangle(borderPen, new Rectangle(corner, corner, floorSize * 16, floorSize * 16));
                }
            }
            return furnitureMap;
        }

        public static void OverlayItemBoxGlow(Bitmap itemBitmap, int itemSize, int x, int y)
        {
            using (var g = Graphics.FromImage(itemBitmap))
            {
                g.DrawImage(Properties.Resources.BoxGlow, x * itemSize, y * itemSize, itemSize, itemSize);
                g.Flush();
            }
        }

        public static Bitmap DrawFurnitureArrows(Bitmap furnitureMap, Furniture[] furniture, int columns = 16)
        {
            var bitmapGraphics = Graphics.FromImage(furnitureMap);
            bitmapGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            bitmapGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            bitmapGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            for (var i = 0; i < furniture.Length; i++)
            {
                var itemType = ItemData.GetItemType(furniture[i].ItemId, MainForm.SaveFile.SaveType);
                if (furniture[i].Name == "Empty" || (itemType != ItemType.Furniture &&
                    itemType != ItemType.Gyroid)) continue;
                Image arrow = Properties.Resources.Arrow;

                if (furniture[i].Rotation > 0)
                {
                    switch (furniture[i].Rotation % 360)
                    {
                        case 90:
                            arrow.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                        case 180:
                            arrow.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case 270:
                            arrow.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                    }
                }
                bitmapGraphics.DrawImage(arrow, (i % columns) * (furnitureMap.Width / columns), (i / columns) * (furnitureMap.Width / columns));
            }
            bitmapGraphics.Flush();
            bitmapGraphics.Dispose();
            return furnitureMap;
        }

        public static readonly int[] GrassWearOffsetMap = {
            0, 1, 4, 5, 16, 17, 20, 21,
            2, 3, 6, 7, 18, 19, 22, 23,
            8, 9, 12, 13, 24, 25, 28, 29,
            10, 11, 14, 15, 26, 27, 30, 31,
            32, 33, 36, 37, 48, 49, 52, 53,
            34, 35, 38, 39, 50, 51, 54, 55,
            40, 41, 44, 45, 56, 57, 60, 61,
            42, 43, 46, 47, 58, 59, 62, 63
        };

        public static Bitmap DrawGrassWear(byte[] grassBuffer)
        {
            Bitmap grassBitmap;
            Graphics bitmapGraphics;
            if (MainForm.SaveFile.SaveType == SaveType.CityFolk)
            {
                grassBitmap = new Bitmap(64, 64, PixelFormat.Format32bppArgb);
                bitmapGraphics = Graphics.FromImage(grassBitmap);
                var i = 0;
                for (var y2 = 0; y2 < 4; y2++)
                    for (var x2 = 0; x2 < 2; x2++)
                        for (var y = 0; y < 4; y++)
                            for (var x = 0; x < 8; x++)
                            {
                                bitmapGraphics.FillRectangle(new SolidBrush(Color.FromArgb(grassBuffer[i] == 0 ? 0 : 0x60, 0, grassBuffer[i], 0)), (x + x2 * 8) * 4, (y + y2 * 4) * 4, 4, 4);
                                i++;
                            }
                bitmapGraphics.Flush();
                bitmapGraphics.Dispose();
                return grassBitmap;
            }

            grassBitmap = new Bitmap(7 * 64, 6 * 64, PixelFormat.Format32bppArgb);
            bitmapGraphics = Graphics.FromImage(grassBitmap);
            for (var y = 0; y < 6 * 16; y++)
            {
                for (var x = 0; x < 7 * 16; x++)
                {
                    var offset = 64 * ((y / 8) * 16 + (x / 8)) + GrassWearOffsetMap[(y % 8) * 8 + (x % 8)];
                    var brush = new SolidBrush(Color.FromArgb(grassBuffer[offset] == 0 ? 0 : 0x60, 0, grassBuffer[offset], 0));
                    bitmapGraphics.FillRectangle(brush, x * 4, y * 4, 4, 4);
                }
            }
            bitmapGraphics.Flush();
            bitmapGraphics.Dispose();
            return grassBitmap;
        }

        public static Bitmap DrawNewLeafGrassBg(PictureBox[] acreMap)
        {
            var bgBitmap = new Bitmap(64 * 7, 64 * 6);
            var bgGraphics = Graphics.FromImage(bgBitmap);
            for (var i = 0; i < acreMap.Length; i++)
                if (acreMap[i].BackgroundImage != null)
                    bgGraphics.DrawImage(MakeGrayscale((Bitmap)acreMap[i].BackgroundImage), 64 * (i % 7), 64 * (i / 7));
            bgGraphics.Flush();
            bgGraphics.Dispose();
            return bgBitmap;
        }


        //From: http://stackoverflow.com/questions/2265910/convert-an-image-to-grayscale
        public static Bitmap MakeGrayscale(Bitmap original)
        {
            if (original == null)
                return null;
            var newBitmap = new Bitmap(original.Width, original.Height);
            var g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            var colorMatrix = new ColorMatrix(
               new[]
               {
                 new[] {0.3f, 0.3f, 0.3f, 0f, 0f},
                 new[] {0.59f, 0.59f, 0.59f, 0f, 0f},
                 new[] {0.11f, 0.11f, 0.11f, 0f, 0f},
                 new[] {0f, 0f, 0f, 1f, 0f},
                 new[] {0f, 0f, 0f, 0f, 1f}
               });

            var attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);

            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            g.Dispose();
            return newBitmap;
        }

        public static byte[] GetTpcTrimmedBytes(byte[] tpcBytes)
        {
            var finalizedTpcBytes = new byte[0x1400];
            if (tpcBytes.Length > 0x1400)
            {
                MessageBox.Show("Received TPC Card file size: " + tpcBytes.Length.ToString("X"));
                Array.Resize(ref tpcBytes, 0x1400);
            }
            for (var i = tpcBytes.Length - 1; i > 0; i--)
            {
                if (i <= 0 || tpcBytes[i - 1] != 0xFF || tpcBytes[i] != 0xD9) continue;
                Buffer.BlockCopy(tpcBytes, 0, finalizedTpcBytes, 0, i == 0x13FF ? i : i + 1);
                break;
            }
            return finalizedTpcBytes;
        }

        public static Image GetTpcImage(byte[] tpcBytes)
        {
            if (tpcBytes.Length != 0x1400)
            {
                MessageBox.Show("The TPC Picture was an incorrect data size.");
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
            MainForm.DebugManager.WriteLine("Unable to find JPEG End-of-File marker. No TPC?", DebugLevel.Error);
            return Properties.Resources.no_tpc;
        }

        public static uint[] GetBitmapDataFromPng(string pngLocation)
        {
            try
            {
                byte[] bitmapBuffer;
                var img = Image.FromFile(pngLocation);
                using (var stream = new MemoryStream())
                {
                    img.Save(stream, ImageFormat.Bmp);
                    bitmapBuffer = stream.ToArray();
                    img.Dispose();
                }

                var dataSize = bitmapBuffer[0x1C] / 8;
                var width = BitConverter.ToInt32(bitmapBuffer, 0x12);
                var height = BitConverter.ToInt32(bitmapBuffer, 0x16);

                if (width != 32 || height != 32)
                {
                    MessageBox.Show("Pattern Import Error: Image must be sized to 32x32!");
                    return null;
                }

                if (dataSize < 3 || dataSize > 4)
                {
                    MessageBox.Show("Pattern Import Error: Image must have either 24bpp or 32bpp color depth!");
                    return null;
                }

                if (dataSize == 3 || dataSize == 4)
                {
                    var imageData = bitmapBuffer.Skip(BitConverter.ToInt32(bitmapBuffer.Skip(0xA).Take(4).ToArray(), 0)).ToArray();
                    var pixelData = new uint[imageData.Length / dataSize];

                    for (var i = 0; i < pixelData.Length; i++)
                    {
                        var index = i * dataSize;
                        pixelData[i] = (uint)((0xFF << 24) | (imageData[index + 2] << 16) | (imageData[index + 1] << 8) | imageData[index]); // i + 3 would be alpha data
                    }

                    // Flip Vertically
                    Array.Reverse(pixelData);

                    // Flip Horizontally
                    for (var i = 0; i < pixelData.Length; i += width)
                        Array.Reverse(pixelData, i, width);

                    return pixelData;
                }

                return new uint[0];
            }
            catch
            {
                MainForm.DebugManager.WriteLine("Unable to import pattern!", DebugLevel.Error);
                MessageBox.Show("Pattern Import Error: Failed to import the image!");
                return null;
            }
        }

        public static Image GetFaceImage(SaveGeneration saveGeneration, int index, byte gender)
        {
            Image faceImage = null;
            var facesFolder = MainForm.AssemblyLocation + "\\Resources\\Images\\Faces";
            if (!Directory.Exists(facesFolder)) return null;
            switch (saveGeneration)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                case SaveGeneration.iQue:
                    facesFolder += "\\Animal Crossing";
                    break;
                case SaveGeneration.NDS:
                case SaveGeneration.Wii:
                    facesFolder += "\\Wild World";
                    break;
                case SaveGeneration.N3DS:
                    facesFolder += "\\New Leaf\\" + (gender == 1 ? "Female" : "Male");
                    break;
            }

            if (!Directory.Exists(facesFolder)) return null;
            var faceImageFile = facesFolder + "\\" + index + ".bmp";
            if (!File.Exists(faceImageFile)) return null;
            try
            {
                faceImage = Image.FromFile(faceImageFile);
            }
            catch
            {
                MainForm.DebugManager.WriteLine($"Could not open face file: {faceImageFile}", DebugLevel.Error);
            }
            return faceImage;
        }

        public static Bitmap GetHairImage(SaveGeneration saveGeneration, int index, int colorIndex)
        {
            Bitmap hairImage = null;
            var hairFolder = MainForm.AssemblyLocation + "\\Resources\\Images\\Hair Styles";
            if (!Directory.Exists(hairFolder)) return null;
            // TODO: Wild World, City Folk
            if (saveGeneration == SaveGeneration.N3DS)
            {
                hairFolder += "\\New Leaf";
            }

            if (!Directory.Exists(hairFolder)) return null;
            var hairImageFile = hairFolder + "\\" + index + ".png";
            if (!File.Exists(hairImageFile)) return null;
            try
            {
                hairImage = (Bitmap)Image.FromFile(hairImageFile);
                ReplaceGrayscaleColor(ref hairImage, Color.FromArgb((int)PlayerInfo.NlHairColorValues[colorIndex]));
            }
            catch
            {
                MainForm.DebugManager.WriteLine($"Could not open hair file: {hairImageFile}", DebugLevel.Error);
            }
            return hairImage;
        }

        public static void DumpTownAcreBitmap(int acreCountX, int acreCountY, ref PictureBoxWithInterpolationMode[] pictureBoxes)
        {
            var townAcrePreview = new Bitmap(acreCountX * pictureBoxes[0].BackgroundImage.Width, acreCountY * pictureBoxes[0].BackgroundImage.Height);

            using (var g = Graphics.FromImage(townAcrePreview))
            {
                for (var y = 0; y < acreCountY; y++)
                {
                    for (var x = 0; x < acreCountX; x++)
                    {
                        var locationX = x * pictureBoxes[0].BackgroundImage.Width;
                        var locationY = y * pictureBoxes[0].BackgroundImage.Height;

                        (pictureBoxes[y * acreCountX + x].BackgroundImage as Bitmap)?.SetResolution(96, 96);

                        g.DrawImage(pictureBoxes[y * acreCountX + x].BackgroundImage, locationX, locationY);
                    }
                }
            }

            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Portable Network Graphic|*.png";
                saveDialog.FileName = "";

                if (saveDialog.ShowDialog() != DialogResult.OK) return;
                try
                {
                    townAcrePreview.Save(saveDialog.FileName, ImageFormat.Png);
                }
                catch
                {
                    MessageBox.Show("An error occured while saving your town acre preview!", "Acre Preview Image Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static void DrawTownMapViewHouseImages(Villager[] villagers, PictureBoxWithInterpolationMode[] pictureBoxes, Size pictureBoxSize)
        {
            const int houseImageSize = 16;
            var pixelsPerItemSlotX = pictureBoxSize.Width / 16;
            var pixelsPerItemSlotY = pictureBoxSize.Height / 16;

            Console.WriteLine("X: " + pixelsPerItemSlotX + " | Y: " + pixelsPerItemSlotY);
            var houseImage = Properties.Resources.VillagerHouse;
            foreach (var villager in villagers)
            {
                if (!villager.Exists ||
                    villager.Data.HouseCoordinates[0] < 1 || villager.Data.HouseCoordinates[0] > 5 ||
                    villager.Data.HouseCoordinates[1] < 1 || villager.Data.HouseCoordinates[1] > 6 ||
                    villager.Data.HouseCoordinates[2] > 15 || villager.Data.HouseCoordinates[3] > 15) continue;
                var index = (villager.Data.HouseCoordinates[0]) + villager.Data.HouseCoordinates[1] * 7;
                var position = new Point((villager.Data.HouseCoordinates[2] * pixelsPerItemSlotX) - houseImageSize / 2, 
                    (villager.Data.HouseCoordinates[3] * pixelsPerItemSlotY) - houseImageSize / 2);

                Image cloneImage;
                if (pictureBoxes[index].BackgroundImage != null)
                    cloneImage = (Image)pictureBoxes[index].BackgroundImage.Clone();
                else
                    cloneImage = new Bitmap(pictureBoxes[index].Size.Width, pictureBoxes[index].Size.Height);

                using (var g = Graphics.FromImage(cloneImage))
                {
                    g.DrawImage(houseImage, position.X, position.Y, 48, 48);
                    g.Flush();
                    pictureBoxes[index].BackgroundImage = cloneImage;
                }
            }

            houseImage.Dispose();
        }
    }
}
