using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace ACSE.Utilities
{
    public static class Utility
    {
        public static int[] FindAllMatches(ref List<byte> dictionary, byte match)
        {
            var matchPositons = new List<int>();

            for (var i = 0; i < dictionary.Count; i++)
            {
                if (dictionary[i] == match)
                {
                    matchPositons.Add(i);
                }
            }

            return matchPositons.ToArray();
        }

        public static int[] FindLargestMatch(ref List<byte> dictionary, int[] matchesFound, ref byte[] file, int fileIndex, int maxMatch)
        {
            var matchSizes = new int[matchesFound.Length];

            for (var i = 0; i < matchesFound.Length; i++)
            {
                var matchSize = 1;
                var matchFound = true;

                while (matchFound && matchSize < maxMatch && (fileIndex + matchSize < file.Length) && (matchesFound[i] + matchSize < dictionary.Count)) //NOTE: This could be relevant to compression issues? I suspect it's more related to writing
                {
                    if (file[fileIndex + matchSize] == dictionary[matchesFound[i] + matchSize])
                    {
                        matchSize++;
                    }
                    else
                    {
                        matchFound = false;
                    }
                }
                matchSizes[i] = matchSize;
            }

            var bestMatch = new[] {matchesFound[0], matchSizes[0]};
            for (var i = 1; i < matchesFound.Length; i++)
            {
                if (matchSizes[i] <= bestMatch[1]) continue;
                bestMatch[0] = matchesFound[i];
                bestMatch[1] = matchSizes[i];
            }
            return bestMatch;
        }

        public static void Scan_For_NL_Int32()
        {
            if (MainForm.SaveFile == null || MainForm.SaveFile.SaveGeneration != SaveGeneration.N3DS) return;
            using (var int32Stream = File.CreateText(MainForm.AssemblyLocation + "\\" +
                                                               (MainForm.SaveFile.SaveType == SaveType.WelcomeAmiibo ? "WA_" : "") + "NL_Int32_Database.txt"))
                for (var i = 0; i < MainForm.SaveFile.WorkingSaveData.Length - 4; i += 4)
                {
                    var possibleNlInt32 = new NewLeafInt32(MainForm.SaveFile.ReadUInt32(i), MainForm.SaveFile.ReadUInt32(i + 4));
                    if (possibleNlInt32.Valid)
                        int32Stream.WriteLine(
                            $"Found Valid NewLeafInt32 at offset 0x{i:X} | Value: {possibleNlInt32.Value}");
                }
        }

        public static Image Set_Image_Color(Image grayscaleImage, ColorMatrix transformMatrix)
        {
            using (var attributes = new ImageAttributes())
            {
                attributes.SetColorMatrix(transformMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                var transformedImage = new Bitmap(grayscaleImage.Width, grayscaleImage.Height);
                using (var g = Graphics.FromImage(transformedImage))
                {
                    g.DrawImage(grayscaleImage, 0, 0);
                    g.DrawImage(transformedImage, new Rectangle(0, 0, grayscaleImage.Size.Width, grayscaleImage.Size.Height),
                        0, 0, grayscaleImage.Size.Width, grayscaleImage.Size.Height, GraphicsUnit.Pixel, attributes);
                    return transformedImage;
                }
            }
        }

        public static Tuple<byte[], bool> Find_Villager_House(ushort villagerId) // TODO: Apply to WW
        {
            if (MainForm.SaveFile == null) return new Tuple<byte[], bool>(new byte[] {0xFF, 0xFF, 0xFF, 0xFF}, false);
            var villagerHouseId = (ushort)(0x5000 + (villagerId & 0xFF));
            foreach (var acre in MainForm.TownAcres)
            {
                var villagerHouse = acre.AcreItems.FirstOrDefault(o => o.ItemId == villagerHouseId);
                if (villagerHouse != null)
                {
                    return new Tuple<byte[], bool>(
                        new byte[4] { (byte)(acre.Index % 7), (byte)(acre.Index / 7), (byte)(villagerHouse.Location.X), (byte)(villagerHouse.Location.Y + 1) },
                        true);
                }
            }
            return new Tuple<byte[], bool>(new byte[4] { 0xFF, 0xFF, 0xFF, 0xFF }, false);
        }

        public static Villager GetVillagerFromHouse(ushort houseId, Villager[] villagers)
        {
            var villagerId = (ushort)(0xE000 | (houseId & 0x00FF));
            return villagers.FirstOrDefault(villager => villager.Data.VillagerId == villagerId);
        }


        public static bool[] Check_Perfect_Town_Requirements(WorldAcre[] Acres, bool Make_Perfect = false)
        {
            var acreResults = new bool[Acres.Length];
            var points = 0;
            for (var i = 0; i < acreResults.Length; i++)
            {
                var acre = Acres[i];
                switch (MainForm.SaveFile.SaveGeneration)
                {
                    case SaveGeneration.N64:
                    case SaveGeneration.GCN:
                        //TODO: Implement Special Acre Check (Player Houses, Train Station, Oceanfront Acres, Lake Acres, Wishing Well, & Museum
                        //Special Acre Info: < 9 Trees, 0 Points | 9 - 11, 1 Point | 12 - 14, 2 Points | 15 - 17, 1 Point | > 18, 0 Points
                        var treeCount = 0;
                        var weedCount = 0;
                        for (var o = 0; o < 256; o++)
                        {
                            var item = acre.AcreItems[o];
                            if (item.Name == "Weed")
                            {
                                weedCount++;
                                if (Make_Perfect)
                                {
                                    acre.AcreItems[o] = new WorldItem(0, o);
                                }
                            }
                            else if (ItemData.GetItemType(item.ItemId, MainForm.SaveFile.SaveType) == "Tree")
                            {
                                treeCount++;
                            }
                        }
                        if (Make_Perfect)
                        {
                            if (treeCount > 14)
                            {
                                for (var o = 0; o < treeCount - 13; o++)
                                {
                                    for (var x = 0; x < 256; x++)
                                    {
                                        if (ItemData.GetItemType(acre.AcreItems[x].ItemId,
                                                MainForm.SaveFile.SaveType) != "Tree") continue;
                                        acre.AcreItems[x] = new WorldItem(0, x);
                                        break;
                                    }
                                }
                            }
                            else if (treeCount < 12)
                            {
                                for (var o = 0; o < 13 - treeCount; o++)
                                {
                                    for (var x = 0; x < 256; x++)
                                    {
                                        // Check to make sure the item directly above, below, and to the left and right isn't already occupied.
                                        if (acre.AcreItems[x].ItemId != 0 ||
                                            (x >= 16 && acre.AcreItems[x - 16].ItemId != 0) ||
                                            (x <= 239 && acre.AcreItems[x + 16].ItemId != 0) ||
                                            (x != 0 && acre.AcreItems[x - 1].ItemId != 0) ||
                                            (x != 255 && acre.AcreItems[x + 1].ItemId != 0)) continue;
                                        acre.AcreItems[x] = new WorldItem(0x0804, x);
                                        break;
                                    }
                                }
                            }
                        }
                        acreResults[i] = Make_Perfect || ((treeCount > 11 && treeCount < 15) && weedCount < 4);
                        if (acreResults[i])
                        {
                            points++;
                        }
                        break;
                    case SaveGeneration.NDS:
                    case SaveGeneration.Wii:
                    case SaveGeneration.N3DS:
                        throw new NotImplementedException();
                }
            }
            return acreResults;
        }

        public static void Place_Structure(WorldAcre acre, int startIndex, List<ushort[]> structureInfo)
        {
            if (startIndex <= -1 || startIndex >= 256) return;
            if (MainForm.SaveFile.SaveGeneration != SaveGeneration.GCN) return;
            for (var y = 0; y < structureInfo.Count; y++)
            {
                for (var x = 0; x < structureInfo[y].Length; x++)
                {
                    var index = startIndex + y * 16 + x;
                    if (index >= 256) continue;
                    switch (structureInfo[y][x])
                    {
                        case 0: // Just for alignment
                            break;
                        case 1:
                            acre.AcreItems[index] = new WorldItem(0xFFFF, index);
                            break;
                        default:
                            acre.AcreItems[index] = new WorldItem(structureInfo[y][x], index);
                            break;
                    }
                }
            }
        }

        public static byte GetWildWorldGrassBaseType(byte seasonalGrassValue)
        {
            if (seasonalGrassValue < 3)
                return seasonalGrassValue;
            return (byte)((seasonalGrassValue - 1) % 3); // May not be right.
        }

        public static void FloodFillItemArray(ref Item[] items, int itemsPerRow, int startIndex, Item originalItem, Item newItem)
        {
            var rows = items.Length / itemsPerRow;
            var locationStack = new Stack<Point>();
            var previousPoints = new int[items.Length];

            var x = startIndex % itemsPerRow;
            var y = startIndex / itemsPerRow;

            locationStack.Push(new Point(x, y));

            while (locationStack.Count > 0)
            {
                var p = locationStack.Pop();
                var idx = p.X + p.Y * itemsPerRow;

                if (p.X < itemsPerRow && p.X > -1 &&
                        p.Y < rows && p.Y > -1 && previousPoints[idx] == 0) // Make sure we stay within bounds
                {
                    var i = items[idx];
                    if (i.Equals(originalItem))
                    {
                        items[idx] = new Item(newItem);
                        if (p.X - 1 > -1)
                            locationStack.Push(new Point(p.X - 1, p.Y));
                        if (p.X + 1 < itemsPerRow)
                            locationStack.Push(new Point(p.X + 1, p.Y));
                        if (p.Y - 1 > -1)
                            locationStack.Push(new Point(p.X, p.Y - 1));
                        if (p.Y + 1 < rows)
                            locationStack.Push(new Point(p.X, p.Y + 1));
                    }
                }
                previousPoints[idx] = 1;
            }
        }

        public static void FloodFillWorldItemArray(ref WorldItem[] items, int itemsPerRow, int startIndex, WorldItem originalItem, WorldItem newItem)
        {
            var rows = items.Length / itemsPerRow;
            var locationStack = new Stack<Point>();
            var previousPoints = new int[items.Length];

            var x = startIndex % itemsPerRow;
            var y = startIndex / itemsPerRow;

            locationStack.Push(new Point(x, y));

            while (locationStack.Count > 0)
            {
                var p = locationStack.Pop();

                var idx = p.X + p.Y * itemsPerRow;

                if (p.X < itemsPerRow && p.X > -1 &&
                        p.Y < rows && p.Y > -1 && previousPoints[idx] == 0) // Make sure we stay within bounds
                {
                    var i = items[idx];
                    if (i.Equals(originalItem))
                    {
                        items[idx] = new WorldItem(newItem.ItemId, newItem.Flag1, newItem.Flag2, i.Index);
                        if (p.X - 1 > -1)
                            locationStack.Push(new Point(p.X - 1, p.Y));
                        if (p.X + 1 < itemsPerRow)
                            locationStack.Push(new Point(p.X + 1, p.Y));
                        if (p.Y - 1 > -1)
                            locationStack.Push(new Point(p.X, p.Y - 1));
                        if (p.Y + 1 < rows)
                            locationStack.Push(new Point(p.X, p.Y + 1));
                    }
                }
                previousPoints[idx] = 1;
            }
        }

        public static void FloodFillFurnitureArray(ref Furniture[] items, int itemsPerRow, int startIndex, Furniture originalItem, Furniture newItem)
        {
            var rows = items.Length / itemsPerRow;
            var locationStack = new Stack<Point>();
            var previousPoints = new int[items.Length];

            var x = startIndex % itemsPerRow;
            var y = startIndex / itemsPerRow;

            locationStack.Push(new Point(x, y));

            while (locationStack.Count > 0)
            {
                var p = locationStack.Pop();

                var idx = p.X + p.Y * itemsPerRow;

                if (p.X < itemsPerRow && p.X > -1 &&
                        p.Y < rows && p.Y > -1 && previousPoints[idx] == 0) // Make sure we stay within bounds
                {
                    var i = items[idx];
                    if (i.Equals(originalItem))
                    {
                        items[idx] = new Furniture(newItem);
                        if (p.X - 1 > -1)
                            locationStack.Push(new Point(p.X - 1, p.Y));
                        if (p.X + 1 < itemsPerRow)
                            locationStack.Push(new Point(p.X + 1, p.Y));
                        if (p.Y - 1 > -1)
                            locationStack.Push(new Point(p.X, p.Y - 1));
                        if (p.Y + 1 < rows)
                            locationStack.Push(new Point(p.X, p.Y + 1));
                    }
                }
                previousPoints[idx] = 1;
            }
        }

        // Export/Import Methods
        public static void ExportAcres(WorldAcre[] acres, SaveGeneration saveGeneration, string saveFileName)
        {
            using (var saveDialog = new System.Windows.Forms.SaveFileDialog())
            {
                saveDialog.Filter = "ACSE Acre Save (*.aas)|*.aas";
                saveDialog.FileName = saveFileName + " Acre Data.aas";

                if (saveDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                try
                {
                    using (var stream = new FileStream(saveDialog.FileName, FileMode.Create))
                    {
                        using (var writer = new BinaryWriter(stream))
                        {
                            writer.Write(new byte[] { 0x41, 0x41, 0x53 }); // "AAS" Identifier
                            writer.Write((byte)acres.Length); // Total Acre Count
                            writer.Write((byte)saveGeneration); // Save Generation
                            writer.Write(new byte[] { 0, 0, 0 }); // Padding
                            foreach (var t in acres)
                            {
                                writer.Write(BitConverter.GetBytes(t.AcreId));
                            }

                            writer.Flush();
                        }
                    }
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Acre exportation failed!", "Acre Export Error", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
        }

        public static void ImportAcres(ref WorldAcre[] acres, SaveGeneration saveGeneration)
        {
            using (var openDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openDialog.Filter = "ACSE Acre Save (*.aas)|*.aas";
                openDialog.FileName = "";

                if (openDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                try
                {
                    using (var stream = new FileStream(openDialog.FileName, FileMode.Open))
                    {
                        using (var reader = new BinaryReader(stream))
                        {
                            if (!System.Text.Encoding.ASCII.GetString(reader.ReadBytes(3)).Equals("AAS") ||
                                reader.ReadByte() != acres.Length ||
                                (SaveGeneration) reader.ReadByte() != saveGeneration) return;
                            reader.BaseStream.Seek(8, SeekOrigin.Begin);
                            foreach (var t in acres)
                            {
                                t.AcreId = reader.ReadUInt16();
                                t.BaseAcreId = (ushort)(t.AcreId & 0xFFFC);
                            }
                        }
                    }
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Acre importation failed!", "Acre Import Error", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
        }

        public static void ExportTown(WorldAcre[] acres, SaveGeneration saveGeneration, string saveFileName)
        {
            using (var saveDialog = new System.Windows.Forms.SaveFileDialog())
            {
                saveDialog.Filter = "ACSE Town Save (*.ats)|*.ats";
                saveDialog.FileName = saveFileName + " Town Data.ats";

                if (saveDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                try
                {
                    using (var stream = new FileStream(saveDialog.FileName, FileMode.Create))
                    {
                        using (var writer = new BinaryWriter(stream))
                        {
                            writer.Write(new byte[] { 0x41, 0x54, 0x53 }); // "ATS" Identifier
                            writer.Write((byte)acres.Length); // Total Acre Count
                            writer.Write((byte)saveGeneration); // Save Generation
                            writer.Write(new byte[] { 0, 0, 0 }); // Padding

                            if (saveGeneration == SaveGeneration.N3DS)
                            {
                                foreach (var acre in acres)
                                {
                                    foreach (var item in acre.AcreItems)
                                    {
                                        writer.Write(BitConverter.GetBytes(item.ToUInt32()));
                                    }
                                }
                            }
                            else
                            {
                                foreach (var acre in acres)
                                {
                                    foreach (var item in acre.AcreItems)
                                    {
                                        writer.Write(BitConverter.GetBytes(item.ItemId));
                                    }
                                }
                            }

                            writer.Flush();
                        }
                    }
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Town exportation failed!", "Town Export Error", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
        }

        public static void ImportTown(ref WorldAcre[] acres, SaveGeneration saveGeneration)
        {
            using (var openDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openDialog.Filter = "ACSE Town Save (*.ats)|*.ats";
                openDialog.FileName = "";

                if (openDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                try
                {
                    using (var stream = new FileStream(openDialog.FileName, FileMode.Open))
                    {
                        using (var reader = new BinaryReader(stream))
                        {
                            if (!System.Text.Encoding.ASCII.GetString(reader.ReadBytes(3)).Equals("ATS") ||
                                reader.ReadByte() != acres.Length ||
                                (SaveGeneration) reader.ReadByte() != saveGeneration) return;
                            reader.BaseStream.Seek(8, SeekOrigin.Begin);
                            if (saveGeneration == SaveGeneration.N3DS)
                            {
                                foreach (var acre in acres)
                                {
                                    for (var x = 0; x < acre.AcreItems.Length; x++)
                                    {
                                        acre.AcreItems[x] = new WorldItem(reader.ReadUInt32(), acre.AcreItems[x].Index);
                                    }
                                }
                            }
                            else
                            {
                                foreach (var acre in acres)
                                {
                                    for (var x = 0; x < acre.AcreItems.Length; x++)
                                    {
                                        acre.AcreItems[x] = new WorldItem(reader.ReadUInt16(), acre.AcreItems[x].Index);
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Acre importation failed!", "Acre Import Error", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
        }
    }
}
