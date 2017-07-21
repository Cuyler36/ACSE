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
            if (Save_Type == SaveType.Animal_Crossing || Save_Type == SaveType.Doubutsu_no_Mori_Plus || Save_Type == SaveType.Doubutsu_no_Mori_e_Plus)
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
                foreach (string File in Directory.GetFiles(Image_Dir))
                    Image_List.Add((Save_Type == SaveType.New_Leaf || Save_Type == SaveType.Welcome_Amiibo)
                        ? ushort.Parse(Path.GetFileNameWithoutExtension(File).Replace("acre_", "")).ToString("X4") : Path.GetFileNameWithoutExtension(File), Image.FromFile(File));
            else
                MessageBox.Show("Acre Images Folder doesn't exist!");
            return Image_List;
        }

        public Dictionary<int, Item> GetAcreData(ushort[] acreBuffer)
        {
            Dictionary<int, Item> acreData = new Dictionary<int, Item> { };
            int i = 0;
            foreach (ushort cellData in acreBuffer)
            {
                acreData.Add(i, new Item(cellData));
                i++;
            }
            return acreData;
        }

        public static Dictionary<int, Acre> GetAcreTileData(ushort[] acreBuffer)
        {
            Dictionary<int, Acre> AcreTileData = new Dictionary<int, Acre> { };
            int i = 0;
            foreach (ushort acre in acreBuffer)
            {
                //MessageBox.Show(acre.ToString("X"));
                i++;
                AcreTileData.Add(i, new Acre(acre, i));
            }
            return AcreTileData;
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

        public Normal_Acre(ushort acreId, int position, ushort[] items = null, byte[] burriedItemData = null, SaveType saveType = SaveType.Animal_Crossing, uint[] nl_items = null) : base(acreId, position)
        {
            if (items != null)
                for (int i = 0; i < 256; i++)
                {
                    Acre_Items[i] = new WorldItem(items[i], i);
                    if (burriedItemData != null)
                        SetBuried(Acre_Items[i], position, burriedItemData, saveType); //Broken in original save editor lol.. needs a position - 1 to function properly
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

        public Normal_Acre(ushort acreId, int position, WorldItem[] items, byte[] buriedItemData = null, SaveType saveType = SaveType.Animal_Crossing) : base(acreId, position)
        {
            Acre_Items = items;
            if (buriedItemData != null)
                for (int i = 0; i < 256; i++)
                    SetBuried(Acre_Items[i], position, buriedItemData, saveType);
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
