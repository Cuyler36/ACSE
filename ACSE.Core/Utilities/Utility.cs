using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using ACSE.Core.Encryption;
using ACSE.Core.Items;
using ACSE.Core.Saves;
using ACSE.Core.Town.Acres;
using ACSE.Core.Villagers;

namespace ACSE.Core.Utilities
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

        public static Tuple<byte[], bool> FindVillagerHouse(ushort villagerId, WorldAcre[] townAcres) // TODO: Apply to WW
        {
            if (Save.SaveInstance == null) return new Tuple<byte[], bool>(new byte[] {0xFF, 0xFF, 0xFF, 0xFF}, false);
            var villagerHouseId = (ushort)(0x5000 + (villagerId & 0xFF));
            foreach (var acre in townAcres)
            {
                var villagerHouse = acre.Items.FirstOrDefault(o => o.ItemId == villagerHouseId);
                if (villagerHouse != null)
                {
                    var idx = Array.IndexOf(acre.Items, villagerHouse);

                    return new Tuple<byte[], bool>(
                        new[]
                        {
                            (byte) (acre.Index % 7), (byte) (acre.Index / 7), (byte) (idx % 16),
                            (byte) (idx / 16 + 1)
                        },
                        true);
                }
            }
            return new Tuple<byte[], bool>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }, false);
        }

        public static (byte[], bool) FindVillagerHouseWildWorld(int villagerIndex, WorldAcre[] townAcres)
        {
            if (Save.SaveInstance == null) return (new byte[] {0xFF, 0xFF}, false);

            var houseId = 0x5001 + villagerIndex;
            foreach (var acre in townAcres)
            {
                var villagerHouse = acre.Items.FirstOrDefault(o => o.ItemId == houseId);
                if (villagerHouse != null)
                {
                    var idx = Array.IndexOf(acre.Items, villagerHouse);

                    return (
                        new[]
                        {
                            (byte) (((acre.Index % 4 + 1) << 4) | (idx % 16)),
                            (byte) (((acre.Index / 4 + 1) << 4) | (idx / 16))
                        }, true);
                }
            }

            return (new byte[] { 0xFF, 0xFF }, false);
        }

        public static Villager GetVillagerFromHouse(ushort houseId, Villager[] villagers)
        {
            var villagerId = (ushort)(0xE000 | (houseId & 0x00FF));
            return villagers.FirstOrDefault(villager => villager.Data.VillagerId == villagerId);
        }


        public static bool[] CheckPerfectTownRequirements(WorldAcre[] acres, bool makePerfect = false)
        {
            var acreResults = new bool[acres.Length];
            var points = 0;
            for (var i = 0; i < acreResults.Length; i++)
            {
                var acre = acres[i];
                switch (Save.SaveInstance.SaveGeneration)
                {
                    case SaveGeneration.N64:
                    case SaveGeneration.GCN:
                        //TODO: Implement Special Acre Check (Player Houses, Train Station, Oceanfront Acres, Lake Acres, Wishing Well, & Museum
                        //Special Acre Info: < 9 Trees, 0 Points | 9 - 11, 1 Point | 12 - 14, 2 Points | 15 - 17, 1 Point | > 18, 0 Points
                        var treeCount = 0;
                        var weedCount = 0;
                        for (var o = 0; o < 256; o++)
                        {
                            var item = acre.Items[o];
                            if (item.Name == "Weed")
                            {
                                weedCount++;
                                if (makePerfect)
                                {
                                    acre.Items[o] = new Item(0);
                                }
                            }
                            else if (ItemData.GetItemType(item.ItemId, Save.SaveInstance.SaveType) == ItemType.Tree)
                            {
                                treeCount++;
                            }
                        }
                        if (makePerfect)
                        {
                            if (treeCount > 14)
                            {
                                for (var o = 0; o < treeCount - 13; o++)
                                {
                                    for (var x = 0; x < 256; x++)
                                    {
                                        if (ItemData.GetItemType(acre.Items[x].ItemId,
                                                Save.SaveInstance.SaveType) != ItemType.Tree) continue;
                                        acre.Items[x] = new Item(0);
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
                                        if (acre.Items[x].ItemId != 0 ||
                                            (x >= 16 && acre.Items[x - 16].ItemId != 0) ||
                                            (x <= 239 && acre.Items[x + 16].ItemId != 0) ||
                                            (x != 0 && acre.Items[x - 1].ItemId != 0) ||
                                            (x != 255 && acre.Items[x + 1].ItemId != 0)) continue;
                                        acre.Items[x] = new Item(0x0804);
                                        break;
                                    }
                                }
                            }
                        }
                        acreResults[i] = makePerfect || ((treeCount > 11 && treeCount < 15) && weedCount < 4);
                        if (acreResults[i])
                        {
                            points++;
                        }

                        if (points > 14)
                            return acreResults;
                        break;
                    case SaveGeneration.NDS:
                    case SaveGeneration.Wii:
                    case SaveGeneration.N3DS:
                        throw new NotImplementedException();
                }
            }
            return acreResults;
        }

        public static void PlaceStructure(WorldAcre acre, int startIndex, List<ushort[]> structureInfo)
        {
            if (startIndex <= -1 || startIndex >= 256) return;
            if (Save.SaveInstance.SaveGeneration != SaveGeneration.GCN) return;
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
                            acre.Items[index] = new Item(0xFFFF);
                            break;
                        default:
                            acre.Items[index] = new Item(structureInfo[y][x]);
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
                        Save.SaveInstance.ChangesMade = true;
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
                        Save.SaveInstance.ChangesMade = true;
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
    }
}
