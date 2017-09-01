using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace ACSE
{
    public static class Utility
    {
        public static void Scan_For_NL_Int32()
        {
            if (NewMainForm.Save_File != null && (NewMainForm.Save_File.Save_Type == SaveType.New_Leaf || NewMainForm.Save_File.Save_Type == SaveType.Welcome_Amiibo))
            {
                using (StreamWriter Int32_Stream = File.CreateText(NewMainForm.Assembly_Location + "\\" +
                    (NewMainForm.Save_File.Save_Type == SaveType.Welcome_Amiibo ? "WA_" : "") + "NL_Int32_Database.txt"))
                    for (int i = 0; i < NewMainForm.Save_File.Working_Save_Data.Length - 4; i += 4)
                    {
                        NL_Int32 Possible_NL_Int32 = new NL_Int32(NewMainForm.Save_File.ReadUInt32(i), NewMainForm.Save_File.ReadUInt32(i + 4));
                        if (Possible_NL_Int32.Valid)
                            Int32_Stream.WriteLine(string.Format("Found Valid NL_Int32 at offset 0x{0} | Value: {1}", i.ToString("X"), Possible_NL_Int32.Value));
                    }
            }
        }

        public static Image Set_Image_Color(Image Grayscale_Image, ColorMatrix Transform_Matrix)
        {
            using (ImageAttributes Attributes = new ImageAttributes())
            {
                Attributes.SetColorMatrix(Transform_Matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                Bitmap Transformed_Image = new Bitmap(Grayscale_Image.Width, Grayscale_Image.Height);
                using (Graphics G = Graphics.FromImage(Transformed_Image))
                {
                    G.DrawImage(Grayscale_Image, 0, 0);
                    G.DrawImage(Transformed_Image, new Rectangle(0, 0, Grayscale_Image.Size.Width, Grayscale_Image.Size.Height),
                        0, 0, Grayscale_Image.Size.Width, Grayscale_Image.Size.Height, GraphicsUnit.Pixel, Attributes);
                    return Transformed_Image;
                }
            }
        }

        public static void Increment_Town_ID(Save Save_File)
        {
            int Total_IDs = 0;
            ushort Town_ID = Save_File.ReadUInt16(Save_File.Save_Data_Start_Offset + 8, true);
            for (int i = 0x26040; i < 0x4C040; i += 2)
            {
                ushort Value = Save_File.ReadUInt16(i, true);
                if (Value == Town_ID)
                {
                    Total_IDs++;
                    Save_File.Write(i, (ushort)(Value + 1), true);
                }
            }
            System.Windows.Forms.MessageBox.Show("Total IDs Replaced: " + Total_IDs);
            Save_File.Flush();
        }

        public static byte[] Find_Villager_House(ushort Villager_ID) // TODO: Apply to WW
        {
            if (NewMainForm.Save_File != null)
            {
                ushort Villager_House_ID = (ushort)(0x5000 + (Villager_ID & 0xFF));
                foreach (Normal_Acre Acre in NewMainForm.Town_Acres)
                {
                    WorldItem Villager_House = Acre.Acre_Items.FirstOrDefault(o => o.ItemID == Villager_House_ID);
                    if (Villager_House != null)
                    {
                        return new byte[4] { (byte)(Acre.Index % 7), (byte)(Acre.Index / 7), (byte)(Villager_House.Location.X), (byte)(Villager_House.Location.Y + 1) };
                    }
                }
            }
            return new byte[4] { 0xFF, 0xFF, 0xFF, 0xFF };
        }

        public static bool[] Check_Perfect_Town_Requirements(Normal_Acre[] Acres, bool Make_Perfect = false)
        {
            bool[] Acre_Results = new bool[Acres.Length];
            int Points = 0;
            for (int i = 0; i < Acre_Results.Length; i++)
            {
                Normal_Acre Acre = Acres[i];
                switch (NewMainForm.Save_File.Game_System)
                {
                    case SaveGeneration.N64:
                    case SaveGeneration.GCN:
                        //TODO: Implement Special Acre Check (Player Houses, Train Station, Oceanfront Acres, Lake Acres, Wishing Well, & Museum
                        //Special Acre Info: < 9 Trees, 0 Points | 9 - 11, 1 Point | 12 - 14, 2 Points | 15 - 17, 1 Point | > 18, 0 Points
                        int Tree_Count = 0;
                        int Weed_Count = 0;
                        for (int o = 0; o < 256; o++)
                        {
                            WorldItem Item = Acre.Acre_Items[o];
                            if (Item.Name == "Weed")
                            {
                                Weed_Count++;
                                if (Make_Perfect)
                                {
                                    Acre.Acre_Items[o] = new WorldItem(0, o);
                                }
                            }
                            else if (ItemData.GetItemType(Item.ItemID) == "Tree")
                            {
                                Tree_Count++;
                            }
                        }
                        if (Make_Perfect)
                        {
                            if (Tree_Count > 14)
                            {
                                for (int o = 0; o < Tree_Count - 13; o++)
                                {
                                    for (int x = 0; x < 256; x++)
                                    {
                                        if (ItemData.GetItemType(Acre.Acre_Items[x].ItemID) == "Tree")
                                        {
                                            Acre.Acre_Items[x] = new WorldItem(0, x);
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (Tree_Count < 12)
                            {
                                for (int o = 0; o < 13 - Tree_Count; o++)
                                {
                                    for (int x = 0; x < 256; x++)
                                    {
                                        // Check to make sure the item directly above, below, and to the left and right isn't already occupied.
                                        if (Acre.Acre_Items[x].ItemID == 0 && (x < 16 || Acre.Acre_Items[x - 16].ItemID == 0) && (x > 239 || Acre.Acre_Items[x + 16].ItemID == 0)
                                            && (x == 0 || Acre.Acre_Items[x - 1].ItemID == 0) && (x == 255 || Acre.Acre_Items[x + 1].ItemID == 0))
                                        {
                                            Acre.Acre_Items[x] = new WorldItem(0x0804, x);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        Acre_Results[i] = Make_Perfect || ((Tree_Count > 11 && Tree_Count < 15) && Weed_Count < 4);
                        if (Acre_Results[i])
                        {
                            Points++;
                        }
                        break;
                    case SaveGeneration.NDS:
                    case SaveGeneration.Wii:
                    case SaveGeneration.N3DS:
                        throw new NotImplementedException();
                }
            }
            return Acre_Results;
        }

        public static void Place_Structure(Normal_Acre Acre, int Start_Index, List<ushort[]> Structure_Info)
        {
            if (Start_Index > -1 && Start_Index < 256)
            {
                if (NewMainForm.Save_File.Game_System == SaveGeneration.GCN)
                {
                    for (int y = 0; y < Structure_Info.Count; y++)
                    {
                        for (int x = 0; x < Structure_Info[y].Length; x++)
                        {
                            int Index = Start_Index + y * 16 + x;
                            if (Index < 256)
                            {
                                switch (Structure_Info[y][x])
                                {
                                    case 0: // Just for alignment
                                        break;
                                    case 1:
                                        Acre.Acre_Items[Index] = new WorldItem(0xFFFF, Index);
                                        break;
                                    default:
                                        Acre.Acre_Items[Index] = new WorldItem(Structure_Info[y][x], Index);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static Normal_Acre[] Generate_Random_Town_Layout()
        {
            Normal_Acre[] Acres = new Normal_Acre[70];
            Random Random_Generator = new Random();
            int Town_Cliff_Count = 1;
            int Bridge_Count = 0;
            int Ramp_Count = 0;
            bool River_Turned = false;
            bool Cliff_Turned = false;
            int Random = Random_Generator.Next(1, 20);
            if (Random > 18)
                Town_Cliff_Count++;
            Acres[0] = new Normal_Acre((ushort)(0x0344 + Town_Cliff_Count), 0);
            Acres[6] = new Normal_Acre((ushort)(0x0348 + Town_Cliff_Count), 6);
            int River_Start_Acre = Random_Generator.Next(1, 5);
            // Generate Border layer
            for (int i = 1; i < 6; i++)
            {
                Acres[i] = new Normal_Acre((i == River_Start_Acre) ? (ushort)(0x0328 + Town_Cliff_Count) : (ushort)(0x0324 + Town_Cliff_Count), i);
            }
            // Generate Cliffs
            for (int i = 0; i < Town_Cliff_Count; i++)
            {
                int Starting_Offset = 21 + (i * 7) + (Town_Cliff_Count == 1 ? Random_Generator.Next(0, 1) : 0) * 7;
                //TODO: Add more Node Labels to the Acre list for parsing
            }
            return Acres;
        }
    }
}
