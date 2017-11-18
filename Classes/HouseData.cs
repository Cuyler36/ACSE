using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACSE
{
    public struct House_Offsets
    {
        public int Room_Start;
        public int Room_Size;
        public int Layer_Size;
        public int Layer_Count;
        public int House_Upgrade_Size;
        public int Owning_Player_ID;
        public int Owning_Player_Name;
        public int Town_ID;
        public int Town_Name;
        public int Design;
        public int Design_Size;
        public int Bed; //CF Exclusive
        public int Roof_Color;
        public int Customization_Start; //NL Exclusive
        public int Mailbox;
        public int Mail_Size;
        public int Mail_Count;
        public int Gyroid_Items; //AC Exclusive
        public int Gryoid_Message; //AC Exclusive
    }

    public struct House_Data
    {
        //public Room[] Rooms;
        public byte House_Upgrade_Size;
        public ushort Owning_Player_ID;
        public string Owning_Player_Name;
        public ushort Town_ID;
        public string Town_Name;
        public Pattern Design;
        public Item Bed;
        public byte Roof_Color;
        public Item[] Customizations;
        //public Mail[] Mailbox;
        public Gyroid_Item[] Gyroid_Items;
        public string Gyroid_Message;
    }

    public class House
    {
        public int Index;
        public int Offset;
        public House_Data Data;
    }

    class HouseData
    {
        public static House_Offsets City_Folk_Offsets = new House_Offsets
        {
            Room_Start = 0x8AC,
            Room_Size = 0x458,
            Layer_Size = 0x200, //16 * 16 DWORDs
            Layer_Count = 2,
            House_Upgrade_Size = 0x15B4, //Also at 0x15B5

        };

        public static House_Offsets New_Leaf_Offsets = new House_Offsets //HouseData is duplicated starting at 0x9 (0x0 - 0x8)
        {
            House_Upgrade_Size = 0,
            Customization_Start = 1,
            //Style = 1,
            //DoorShape = 2,
            //Walls = 3,
            //Roof = 4,
            //Door = 5,
            //Fence = 6,
            //Pavement = 7,
            //Mailbox = 8,
            Room_Start = 0x76,
            Room_Size = 0x302,
        };
    }
}
