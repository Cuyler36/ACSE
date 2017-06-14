using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Resources;
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
                int numbytes = bmpdata.Stride * bitmap.Height;
                byte[] bytedata = new byte[numbytes];
                IntPtr ptr = bmpdata.Scan0;

                Marshal.Copy(ptr, bytedata, 0, numbytes);

                return bytedata;
            }
            finally
            {
                if (bmpdata != null)
                    bitmap.UnlockBits(bmpdata);
            }

        }

        public static Bitmap Draw_Grid(Bitmap Map, int Item_Size, uint Grid_Color = 0x41444444, int Grid_Pixel_Size = 1)
        {
            Graphics Bitmap_Graphics = Graphics.FromImage(Map);
            Bitmap_Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Bitmap_Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Bitmap_Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            int Num_Lines_X = Map.Width / Item_Size, Num_Lines_Y = Map.Height / Item_Size;
            Pen Grid_Pen = new Pen(new SolidBrush(Color.FromArgb((int)Grid_Color)));
            Grid_Pen.Width = Grid_Pixel_Size;
            for (int Y = 1; Y < Num_Lines_Y; Y++)
                Bitmap_Graphics.DrawLine(Grid_Pen, 0, Y * Item_Size - 1, Map.Width, Y * Item_Size - 1);
            for (int X = 1; X < Num_Lines_X; X++)
                Bitmap_Graphics.DrawLine(Grid_Pen, X * Item_Size - 1, 0, X * Item_Size - 1, Map.Height);
            Bitmap_Graphics.Flush();
            Bitmap_Graphics.Dispose();
            return Map;
        }

        public static Bitmap Draw_Acre_Highlight()
        {
            Bitmap Acre_Highlight = new Bitmap(64, 64);
            Graphics Bitmap_Graphics = Graphics.FromImage(Acre_Highlight);
            Pen Border_Color = new Pen(Color.FromArgb(0x80, Color.Gold));
            Border_Color.Width = 8;
            Bitmap_Graphics.DrawRectangle(Border_Color, new Rectangle(0, 0, 64, 64));
            Bitmap_Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0x80, Color.Yellow)), new Rectangle(4, 4, 56, 56));
            Bitmap_Graphics.Flush();
            Bitmap_Graphics.Dispose();
            return Acre_Highlight;
        }

        public static Bitmap Draw_Building(Bitmap Acre_Map, Building Building_to_Draw, bool Use_Text = false)
        {
            RectangleF RectF = new RectangleF(Building_to_Draw.X_Pos * 8, Building_to_Draw.Y_Pos * 8, 8, 8);
            Graphics Bitmap_Graphics = Graphics.FromImage(Acre_Map);
            Bitmap_Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Bitmap_Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Bitmap_Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            if (Use_Text)
            {
                Bitmap_Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                Bitmap_Graphics.DrawString("B", new Font("Tahoma", 8), Brushes.White, RectF);
            }
            else
                Bitmap_Graphics.DrawImage(Properties.Resources.Building, Building_to_Draw.X_Pos * 8, Building_to_Draw.Y_Pos * 8, 8, 8);
            Bitmap_Graphics.Flush();
            return Acre_Map;
        }

        public static Bitmap Draw_Buildings(Bitmap Acre_Map, Building[] Building_List, int Acre)
        {
            if (Building_List == null)
                return Acre_Map;
            foreach (Building B in Building_List)
            {
                if (B.Exists && B.Acre_Index == Acre)
                {
                    Draw_Building(Acre_Map, B);
                }
            }
            return Acre_Map;
        }

        public static Bitmap Draw_Inner_House_Extents(Bitmap Furniture_Map, int Floor_Size = 8)
        {
            using (Graphics Bitmap_Graphics = Graphics.FromImage(Furniture_Map))
            {
                int Corner = (16 - Floor_Size) / 2;
                using (Pen Border_Pen = new Pen(Color.Gray, 2))
                {
                    Border_Pen.Alignment = PenAlignment.Inset;
                    Bitmap_Graphics.DrawRectangle(Border_Pen, new Rectangle(Corner, Corner, Floor_Size * 16, Floor_Size * 16));
                }
            }
            return Furniture_Map;
        }

        public static Bitmap Draw_Furniture_Arrows(Bitmap Furniture_Map, Furniture[] Furniture, int Columns = 16)
        {
            Graphics Bitmap_Graphics = Graphics.FromImage(Furniture_Map);
            Bitmap_Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Bitmap_Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Bitmap_Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            for (int i = 0; i < Furniture.Length; i++)
            {
                if (Furniture[i].Name != "Empty")
                {
                    Image Arrow = Properties.Resources.Arrow;
                    if (Furniture[i].Rotation > 0)
                    {
                        switch(Furniture[i].Rotation)
                        {
                            case 90:
                                Arrow.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                break;
                            case 180:
                                Arrow.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                break;
                            case 270:
                                Arrow.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                break;
                        }
                    }
                    Bitmap_Graphics.DrawImage(Arrow, (i % Columns) * (Furniture_Map.Width / Columns), (i / Columns) * (Furniture_Map.Width / Columns));
                }
            }
            Bitmap_Graphics.Flush();
            Bitmap_Graphics.Dispose();
            return Furniture_Map;
        }

        public static int[] Grass_Wear_Offset_Map = new int[64] //Lazy Hack
        {
            0, 1, 4, 5, 16, 17, 20, 21,
            2, 3, 6, 7, 18, 19, 22, 23,
            8, 9, 12, 13, 24, 25, 28, 29,
            10, 11, 14, 15, 26, 27, 30, 31,
            32, 33, 36, 37, 48, 49, 52, 53,
            34, 35, 38, 39, 50, 51, 54, 55,
            40, 41, 44, 45, 56, 57, 60, 61,
            42, 43, 46, 47, 58, 59, 62, 63
        };

        public static Bitmap Draw_Grass_Wear(byte[] Grass_Buffer)
        {
            if (NewMainForm.Save_File.Save_Type == SaveType.City_Folk)
            {
                Bitmap Grass_Bitmap = new Bitmap(64, 64, PixelFormat.Format32bppArgb);
                Graphics Bitmap_Graphics = Graphics.FromImage(Grass_Bitmap);
                int i = 0;
                for (int Y2 = 0; Y2 < 4; Y2++)
                    for (int X2 = 0; X2 < 2; X2++)
                        for (int Y = 0; Y < 4; Y++)
                            for (int X = 0; X < 8; X++)
                            {
                                Bitmap_Graphics.FillRectangle(new SolidBrush(Color.FromArgb(Grass_Buffer[i] == 0 ? 0 : 0x60, 0, Grass_Buffer[i], 0)), (X + X2 * 8) * 4, (Y + Y2 * 4) * 4, 4, 4);
                                i++;
                            }
                Bitmap_Graphics.Flush();
                Bitmap_Graphics.Dispose();
                return Grass_Bitmap;
            }
            else // NL/WA
            {
                Bitmap Grass_Bitmap = new Bitmap(7 * 64, 6 * 64, PixelFormat.Format32bppArgb);
                Graphics Bitmap_Graphics = Graphics.FromImage(Grass_Bitmap);
                for (int Y = 0; Y < 6 * 16; Y++)
                {
                    for (int X = 0; X < 7 * 16; X++)
                    {
                        int Offset = 64 * ((Y / 8) * 16 + (X / 8)) + Grass_Wear_Offset_Map[(Y % 8) * 8 + (X % 8)];
                        SolidBrush Brush = new SolidBrush(Color.FromArgb(Grass_Buffer[Offset] == 0 ? 0 : 0x60, 0, Grass_Buffer[Offset], 0));
                        Bitmap_Graphics.FillRectangle(Brush, X * 4, Y * 4, 4, 4);
                    }
                }
                Bitmap_Graphics.Flush();
                Bitmap_Graphics.Dispose();
                return Grass_Bitmap;
            }
        }

        public static Bitmap Draw_NL_Grass_BG(PictureBox[] Acre_Map)
        {
            Bitmap BG_Bitmap = new Bitmap(64 * 7, 64 * 6);
            Graphics BG_Graphics = Graphics.FromImage(BG_Bitmap);
            for (int i = 0; i < Acre_Map.Length; i++)
                if (Acre_Map[i].BackgroundImage != null)
                    BG_Graphics.DrawImage(MakeGrayscale((Bitmap)Acre_Map[i].BackgroundImage), 64 * (i % 7), 64 * (i / 7));
            BG_Graphics.Flush();
            BG_Graphics.Dispose();
            return BG_Bitmap;
        }


        //From: http://stackoverflow.com/questions/2265910/convert-an-image-to-grayscale
        public static Bitmap MakeGrayscale(Bitmap original)
        {
            if (original == null)
                return null;
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);
            Graphics g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
               {
                 new float[] {.3f, .3f, .3f, 0, 0},
                 new float[] {.59f, .59f, .59f, 0, 0},
                 new float[] {.11f, .11f, .11f, 0, 0},
                 new float[] {0, 0, 0, 1, 0},
                 new float[] {0, 0, 0, 0, 1}
               });

            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);

            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            g.Dispose();
            return newBitmap;
        }

        public static byte[] GetTPCTrimmedBytes(byte[] TPC_Bytes)
        {
            byte[] Finalized_TPC_Bytes = new byte[0x1400];
            if (TPC_Bytes.Length > 0x1400)
            {
                MessageBox.Show("Received TPC Card file size: " + TPC_Bytes.Length.ToString("X"));
                Array.Resize(ref TPC_Bytes, 0x1400);
            }
            for (int i = TPC_Bytes.Length - 1; i > 0; i--)
            {
                if (i > 0 && TPC_Bytes[i - 1] == 0xFF && TPC_Bytes[i] == 0xD9)
                {
                    Buffer.BlockCopy(TPC_Bytes, 0, Finalized_TPC_Bytes, 0, i == 0x13FF ? i : i + 1);
                    break;
                }
            }
            return Finalized_TPC_Bytes;
        }

        public static Image GetTPCImage(byte[] TPC_Bytes)
        {
            if (TPC_Bytes.Length != 0x1400)
            {
                MessageBox.Show("The TPC Picture was an incorrect data size.");
                return null;
            }
            if (TPC_Bytes[TPC_Bytes.Length - 1] == 0xD9 && TPC_Bytes[TPC_Bytes.Length - 2] == 0xFF)
                return Image.FromStream(new MemoryStream(TPC_Bytes));
            for (int i = TPC_Bytes.Length - 1; i > 0; i --)
            {
                if (i > 0 && TPC_Bytes[i - 1] == 0xFF && TPC_Bytes[i] == 0xD9)
                {
                    return Image.FromStream(new MemoryStream(TPC_Bytes.Take(i).ToArray()));
                }
            }
            NewMainForm.Debug_Manager.WriteLine("Unable to find JPEG End-of-File marker. No TPC?", DebugLevel.Error);
            return Properties.Resources.no_tpc;
        }
    }
}
