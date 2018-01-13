using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using ACSE.Properties;
using System.Resources;
using System.Globalization;
using System.Collections;

namespace ACSE
{
    class AcreData
    {
        /*
         * Doubutsu no Mori - Doubutsu no Mori e+ Acre Info
         * 
         * Each acre consists of 4 height levels starting at the lowest height.
         * Example:
             * 0x0278 = Empty Acre (Lower) (4)
             * 0x0279 = Empty Acre (Middle) (4)
             * 0x027A = Empty Acre (Upper) (4)
             * 0x027B = Empty Acre (Uppermost) (4)
         */

        public static string[] Acre_Height_Identifiers = new string[4]
        {
            "Lower", "Middle", "Upper", "Uppermost" //"Uppermost" subject to change
        };

        public static Dictionary<string, Image> GetAcreImageSet(SaveType Save_Type)
        {
            Dictionary<string, Image> Image_List = new Dictionary<string, Image>();
            string Image_Dir = NewMainForm.Assembly_Location + "\\Resources\\Images\\";
            if (Save_Type == SaveType.Doubutsu_no_Mori || Save_Type == SaveType.Animal_Crossing || Save_Type == SaveType.Doubutsu_no_Mori_Plus || Save_Type == SaveType.Doubutsu_no_Mori_e_Plus) // TODO: DnM needs its own set?
                Image_Dir += "Acre_Images";
            else if (Save_Type == SaveType.Wild_World)
                Image_Dir += "WW_Acre_Images";
            else if (Save_Type == SaveType.City_Folk)
                Image_Dir += "CF_Acre_Images";
            else if (Save_Type == SaveType.New_Leaf)
                Image_Dir += "NL_Acre_Images";
            else if (Save_Type == SaveType.Welcome_Amiibo)
                Image_Dir += "WA_Acre_Images";

            if (Directory.Exists(Image_Dir))
            {
                var Files = Directory.GetFiles(Image_Dir);
                for (int i = 0; i < Files.Length; i++)
                {
                    var File = Files[i];
                    var Extension = Path.GetExtension(File);
                    if (Extension.Equals(".jpg") || Extension.Equals(".png"))
                    {
                        try
                        {
                            Image_List.Add((Save_Type == SaveType.New_Leaf || Save_Type == SaveType.Welcome_Amiibo)
                                ? ushort.Parse(Path.GetFileNameWithoutExtension(File).Replace("acre_", "")).ToString("X4") : Path.GetFileNameWithoutExtension(File), Image.FromFile(File));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Format("An error occured while loading the acre images! File Index: {0} | File Name: {1} | Error:\n{2}", i, Path.GetFileName(File), ex.Message));
                        }
                    }
                }
            }
            else
                MessageBox.Show("Acre Images Folder doesn't exist!");
            return Image_List;
        }

        public static Image FetchAcreImage(SaveType Save_Type, ushort AcreId)
        {
            string Image_Dir = NewMainForm.Assembly_Location + "\\Resources\\Images\\";
            if (Save_Type == SaveType.Doubutsu_no_Mori || Save_Type == SaveType.Animal_Crossing || Save_Type == SaveType.Doubutsu_no_Mori_Plus || Save_Type == SaveType.Doubutsu_no_Mori_e_Plus) // TODO: DnM needs its own set?
                Image_Dir += "Acre_Images";
            else if (Save_Type == SaveType.Wild_World)
                Image_Dir += "WW_Acre_Images";
            else if (Save_Type == SaveType.City_Folk)
                Image_Dir += "CF_Acre_Images";
            else if (Save_Type == SaveType.New_Leaf)
                Image_Dir += "NL_Acre_Images";
            else if (Save_Type == SaveType.Welcome_Amiibo)
                Image_Dir += "WA_Acre_Images";
            else
                return null;
            if (Directory.Exists(Image_Dir))
            {
                foreach (string File in Directory.GetFiles(Image_Dir))
                {
                    var Extension = Path.GetExtension(File);
                    if (Extension.Equals(".png") || Extension.Equals(".jpg"))
                    {
                        if (Save_Type == SaveType.New_Leaf || Save_Type == SaveType.Welcome_Amiibo)
                        {
                            if (ushort.TryParse(Path.GetFileNameWithoutExtension(File).Replace("acre_", ""), out ushort FileAcreId))
                            {
                                if (FileAcreId == AcreId)
                                {
                                    return Image.FromFile(File);
                                }
                            }
                        }
                        else
                        {

                        }
                    }
                }
            }
            return null;
        }

        public static Dictionary<byte, Image> Load_AC_Map_Icons()
        {
            string Icon_Directory = NewMainForm.Assembly_Location + "\\Resources\\Images\\AC_Map_Icons";
            Dictionary<byte, Image> Icons = new Dictionary<byte, Image>();
            if (Directory.Exists(Icon_Directory))
            {
                foreach (string Icon_File in Directory.GetFiles(Icon_Directory))
                {
                    var Extension = Path.GetExtension(Icon_File);
                    if (Extension.Equals(".png") || Extension.Equals(".jpg"))
                    {
                        if (byte.TryParse(Path.GetFileNameWithoutExtension(Icon_File), out byte Index))
                        {
                            Icons.Add(Index, Image.FromFile(Icon_File));
                        }
                    }
                }
            }
            return Icons;
        }

        public static Image FetchACMapIcon(byte Index)
        {
            string Icon_Directory = NewMainForm.Assembly_Location + "\\Resources\\Images\\AC_Map_Icons";
            if (Directory.Exists(Icon_Directory))
            {
                foreach (string Icon_File in Directory.GetFiles(Icon_Directory))
                {
                    if (byte.TryParse(Path.GetFileNameWithoutExtension(Icon_File), out byte FileIndex))
                    {
                        if (FileIndex == Index)
                        {
                            return Image.FromFile(Icon_File);
                        }
                    }
                }
            }
            return null;
        }

        public static Dictionary<ushort, byte> Load_AC_Map_Index(SaveType saveType)
        {
            string Index_File = NewMainForm.Assembly_Location;
            switch (saveType)
            {
                case SaveType.Doubutsu_no_Mori:
                case SaveType.Doubutsu_no_Mori_Plus:
                    Index_File += "\\Resources\\DnM_Map_Icon_Index.txt";
                    break;
                case SaveType.Animal_Crossing:
                case SaveType.Doubutsu_no_Mori_e_Plus:
                default:
                    Index_File += "\\Resources\\AC_Map_Icon_Index.txt";
                    break;
            }
            
            if (File.Exists(Index_File))
            {
                try
                {
                    using (StreamReader Index_Reader = File.OpenText(Index_File))
                    {
                        Dictionary<ushort, byte> Icon_Index = new Dictionary<ushort, byte>();
                        string Line;
                        while ((Line = Index_Reader.ReadLine()) != null)
                        {
                            if (Line.Contains("0x"))
                            {
                                string Acre_ID_Str = Line.Substring(0, 6).Replace("0x", ""), Index_Str = Line.Substring(7).Trim();
                                if (ushort.TryParse(Acre_ID_Str, NumberStyles.AllowHexSpecifier, null, out ushort Acre_ID) && byte.TryParse(Index_Str, out byte Index))
                                {
                                    Icon_Index.Add(Acre_ID, Index);
                                }
                            }
                        }
                        return Icon_Index;
                    }
                }
                catch { }
            }
            return null;
        }
    }

    public class Acre
    {
        public ushort AcreID = 0;
        public ushort BaseAcreID = 0; // GCN/N64
        public int Index = 0;
        public string Name = "";

        public Acre(ushort acreId, int position)
        {
            AcreID = acreId;
            BaseAcreID = (ushort)(acreId - acreId % 4);
            Index = position;
            //Name = AcreData.GetAcreName(acreId);
        }

        public Acre(byte acreId, int position)
        {
            AcreID = acreId;
            Index = position;
            //Name = AcreData.GetAcreName(AcreID);
        }
    }

    public class Normal_Acre : Acre
    {
        public WorldItem[] Acre_Items = new WorldItem[16 * 16];

        public Normal_Acre(ushort acreId, int position, ushort[] items = null, byte[] burriedItemData = null, SaveType saveType = SaveType.Animal_Crossing, uint[] nl_items = null, int townPosition = -1) : base(acreId, position)
        {
            if (items != null)
                for (int i = 0; i < 256; i++)
                {
                    Acre_Items[i] = new WorldItem(items[i], i);
                    if (burriedItemData != null)
                        SetBuried(Acre_Items[i], townPosition == -1 ? position : townPosition, burriedItemData, saveType); //Broken in original save editor lol.. needs a position - 1 to function properly
                }
            else if (nl_items != null)
            {
                for (int i = 0; i < 256; i++)
                {
                    Acre_Items[i] = new WorldItem(nl_items[i], i);
                    //add buried logic
                }
            }
        }

        public Normal_Acre(ushort acreId, int position) : base(acreId, position) { }

        public Normal_Acre(ushort acreId, int position, uint[] items = null, byte[] burriedItemData = null, SaveType saveType = SaveType.Animal_Crossing)
            : this(acreId, position, null, null, saveType, items) { }

        public Normal_Acre(ushort acreId, int position, WorldItem[] items, byte[] buriedItemData = null, SaveType saveType = SaveType.Animal_Crossing, int townPosition = -1) : base(acreId, position)
        {
            Acre_Items = items;
            if (buriedItemData != null && townPosition > -1)
                for (int i = 0; i < 256; i++)
                    SetBuried(Acre_Items[i], townPosition, buriedItemData, saveType);
        }
        //TODO: Change BuriedData from byte[] to ushort[] and use updated code
        private int GetBuriedDataLocation(WorldItem item, int acre, SaveType saveType)
        {
            if (item != null)
            {
                int worldPosition = 0;
                if (saveType == SaveType.Animal_Crossing || saveType == SaveType.Doubutsu_no_Mori_e_Plus || saveType == SaveType.City_Folk)
                    worldPosition = (acre * 256) + (15 - item.Location.X) + item.Location.Y * 16; //15 - item.Location.X because it's stored as a ushort in memory w/ reversed endianess
                else if (saveType == SaveType.Wild_World)
                    worldPosition = (acre * 256) + item.Index;
                return worldPosition / 8;
            }
            return -1;
        }

        public void SetBuriedInMemory(WorldItem item, int acre, byte[] burriedItemData, bool buried, SaveType saveType)
        {
            if (saveType != SaveType.New_Leaf && saveType != SaveType.Welcome_Amiibo)
            {
                int buriedLocation = GetBuriedDataLocation(item, acre, saveType);
                if (buriedLocation > -1)
                {
                    DataConverter.SetBit(ref burriedItemData[buriedLocation], item.Location.X % 8, buried);
                    item.Burried = DataConverter.ToBit(burriedItemData[buriedLocation], item.Location.X % 8) == 1;
                }
                else
                    item.Burried = false;
            }
        }
        //Correct decoding/setting of buried items. Fixes the hacky SaveType case for AC/CF. (Don't forget to implement this!)
        private void SetBuriedInMemoryFixed(WorldItem item, int acre, ushort[] buriedItemData, bool buried, SaveType saveType)
        {
            if (saveType != SaveType.New_Leaf && saveType != SaveType.Welcome_Amiibo)
            {
                int buriedLocation = (acre * 256 + item.Index) / 16;
                if (buriedLocation > -1)
                {
                    byte[] Buried_Row_Bytes = BitConverter.GetBytes(buriedItemData[buriedLocation]);
                    DataConverter.SetBit(ref Buried_Row_Bytes[item.Location.X / 8], item.Location.X % 8, buried); //Should probably rewrite bit editing functions to take any data type
                    item.Burried = DataConverter.ToBit(Buried_Row_Bytes[item.Location.X / 8], item.Location.X % 8) == 1;
                    buriedItemData[buriedLocation] = BitConverter.ToUInt16(Buried_Row_Bytes, 0);
                }
                else
                    item.Burried = false;
            }
        }

        private void SetBuried(WorldItem item, int acre, byte[] burriedItemData, SaveType saveType)
        {
            int burriedDataOffset = GetBuriedDataLocation(item, acre, saveType);
            if (burriedDataOffset > -1 && burriedDataOffset < burriedItemData.Length)
                item.Burried = DataConverter.ToBit(burriedItemData[burriedDataOffset], item.Location.X % 8) == 1;
        }
    }
}
