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
        public Room[] Rooms;
        public byte House_Upgrade_Size;
        public ushort Owning_Player_ID;
        public string Owning_Player_Name;
        public ushort Town_ID;
        public string Town_Name;
        public Pattern Design;
        public Item Bed;
        public byte Roof_Color;
        public Item[] Customizations;
        public Mail[] Mailbox;
        public Gyroid_Item[] Gyroid_Items;
        public string Gyroid_Message;
    }

    public class NewHouse
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

        /*
         * New Discovery (AC):
         *      Houses have four levels in them. These are layers you can store items on top of each other.
         *      This is how dressers work in the game. Each layer is 0x228 bytes away from the beginning of the previous one.
         *      So, a dresser with three items looks like this:
         *          Fourth Layer: Item 3
         *          Third Layer: Item 2
         *          Second Layer: Item 1
         *          Main Floor: Dresser
         *          
         *      This means that it's unnecessary to add "storage" to the inventory editor.
         */
        public static ushort[] House_Identifiers = new ushort[10]
        {
            0x0480, 0x2480, 0x4880, 0x24A0, 0x4890, 0x48A0, 0x6C90, 0x6C80, 0x7000, 0x0000
            //StarterHouse, First Upgrade, Expanded Main Room (No Basement), First Upgrade + Basement, Expanded Room + Basement (From Basement),
            //Expanded Room + Basement (From Expanded Room), 2nd Floor (From Expanded Room), 2nd Floor (From Basement), Statue (From Basement)
        };

        //Rewrote all methods here to be significantly shorter. I originally wrote them when I had just started in C#.

        public static int ReadHouseSize(ushort[] houseBuffer, bool includesPadding = true)
        {
            int x;
            for (x = (includesPadding ? 0x11 : 0x0); x < houseBuffer.Length; x++)
                if (houseBuffer[x] == 0xFFFE)
                    break;
            return (x - (includesPadding ? 0x11 : 0x0));
        }

        public static Furniture[] ReadHouseData(ushort[] houseBuffer, int size = 0, bool includesPadding = true)
        {
            if (size == 0)
                size = ReadHouseSize(houseBuffer, includesPadding);
            Furniture[] Furniture_Array = new Furniture[size * size];

            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    Furniture_Array[y * size + x] = new Furniture(houseBuffer[(includesPadding ? 0x11 : 0x0) + 0x10 * y + x]);

            return Furniture_Array;
        }

        public static void UpdateHouseData(Furniture[] houseItems, ushort[] houseBuffer, int size, bool includesPadding = true)
        {
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    houseBuffer[(includesPadding ? 0x11 : 0x0) + 0x10 * y + x] = houseItems[y * size + x].ItemID;
        }

    }

    //Struct Size: 0x26B0
    public class House
    {
        public int Offset;
        //
        public string Owning_Player_Name;
        public string Town_Name;
        public uint Player_ID;
        public ACDate Last_Entry_Date;   //Last day the house was entered
        public ACDate Last_Upgrade_Date;    //Not sure if this is accurate (Also is reversed)
        public ushort Garbage;      //Not really sure what this is (it could also be two bytes)
        public byte House_Number;   //Appears again two bytes away, with 0xFF following (not always FF following..)
        public Room[] Rooms = new Room[3];
        public Mail[] Mailbox = new Mail[10];
        public Gyroid_Item[] Gyroid_Storage = new Gyroid_Item[4];
        public ACString Gyroid_Message;
        public byte Cockroach_Count;

        public House(int offset)
        {
            Offset = offset;
            Owning_Player_Name = DataConverter.ReadString(offset, 8).Trim();
            Town_Name = DataConverter.ReadString(offset + 8, 8).Trim();
            Player_ID = DataConverter.ReadUInt(offset + 0x10);
            Last_Entry_Date = new ACDate(DataConverter.ReadDataRaw(offset + 0x1C, 4));
            Last_Upgrade_Date = new ACDate(DataConverter.ReadDataRaw(offset + 0x26, 4), true);
            //Garbage here
            House_Number = (byte)((offset - 0x9CE8) / 0x26B0);
            for (int i = 0; i < 3; i++)
                Rooms[i] = new Room(DataConverter.ReadUShortArray(offset + 0x38 + i * 0x8A8, 0x454), i);
            for (int i = 0; i < 10; i++)
                Mailbox[i] = new Mail(offset + 0x1A30 + i * 0x12A);
            for (int i = 0; i < 4; i++)
            {
                int local_Offset = offset + 0x25D4 + i * 0x8;
                Gyroid_Storage[i] = new Gyroid_Item(DataConverter.ReadUShort(local_Offset), DataConverter.ReadUInt(local_Offset + 4), DataConverter.ReadByte(local_Offset + 3));
            }

            Gyroid_Message = DataConverter.ReadString(offset + 0x25F4, 0x80);
            Cockroach_Count = DataConverter.ReadByte(offset + 0x2648);
            //MessageBox.Show(Gyroid_Message.Clean());
        }
    }

    //Struct Size: 0x8A8
    public class Room
    {
        public Layer[] Layers = new Layer[4];
        public Item Carpet;
        public Item Wallpaper;
        public byte Unknown_1;
        public byte Unknown_2;
        public int Room_Size;

        public Room(ushort[] Room_Buffer, int Floor)
        {
            bool Includes_Wall = Room_Buffer[1] == 0xFFFE;
            int Data_Start_Offset = Includes_Wall ? 16 : 1; //Data with wall looks like this: 0xFFFE, 0xFFFE, 0xFFFE, 0xFFFE, ..., 0xFFFE, 0xITEM_ID
            Room_Size = HouseData.ReadHouseSize(Room_Buffer, Includes_Wall);
            for (int i = 0; i < 4; i++) //4 layers per room
            {
                ushort[] Layer_Buffer = new ushort[0x114];
                Buffer.BlockCopy(Room_Buffer, i * 0x228, Layer_Buffer, 0, 0x228);
                Layers[i] = new Layer(Layer_Buffer, Room_Size, Floor, i, true);
            }
            Carpet = new Item((ushort)((0x26 << 8) + (Room_Buffer[0x450] & 0xFF00) >> 8));
            Wallpaper = new Item((ushort)((0x27 << 8) + Room_Buffer[0x450] & 0x00FF));
            Unknown_1 = (byte)((Room_Buffer[0x451] & 0xFF00) >> 4);
            Unknown_2 = (byte)(Room_Buffer[0x451] & 0x00FF);
        }
    }

    //Struct Size: 0x228
    public class Layer
    {
        public Furniture[] Furniture;
        public ushort[] Raw_Data;
        public bool Has_Wall;
        public int Size;
        public string Name;

        public Layer(ushort[] Layer_Buffer, int Layer_Size, int Floor, int Layer, bool Includes_Wall = false)
        {
            Name = (Floor == 0 ? "First Floor" : (Floor == 1 ? "Second Floor" : "Basement")) + " Layer #" + Layer;
            Size = Layer_Size;
            Has_Wall = Includes_Wall;
            Raw_Data = Layer_Buffer; //Remove this to use Furniture in the future
            Furniture = new Furniture[Size * Size];
            for (int y = 0; y < Size; y++)
                for (int x = 0; x < Size; x++)
                    Furniture[y * Size + x] = new Furniture(Layer_Buffer[(Includes_Wall ? 0x11 : 0) + y * 0x10 + x]);
        }

        public ushort[] Get_Data(Furniture[] Updated_Furniture)
        {
            if (Updated_Furniture.Length == Furniture.Length)
                for (int y = 0; y < Size; y++)
                    for (int x = 0; x < Size; x++)
                        Raw_Data[(Has_Wall ? 0x11 : 0) + y * 0x10 + x] = Updated_Furniture[y * Size + x].ItemID;
            else
                MessageBox.Show(string.Format("{0}: New Furniture Array length ({1}) does not match original length ({2})!", Name, Updated_Furniture.Length, Furniture.Length));
            Furniture = Updated_Furniture;
            return Raw_Data;
        }
    }

    //Struct Size: 0x12A (Villager Stored Mail Size: 0x100)
    public class Mail
    {
        public int Offset;
        public bool Player_Mail;
        public string Reciepiant_Name;
        public string Reciepiant_Town_Name;
        public uint Reciepiant_ID;
        public string Sender_Name;
        public string Sender_Town_Name;
        public uint Sender_ID;
        public byte Event_Flag; //Not sure about this
        public byte Unknown_1; //Never seen it be anything other than 0
        public Item Present;
        public byte Read;
        public byte Unknown_2; //Letter_Type/Event_ID? (Could have something to do with name positioning, since it's not in the message)
        public byte Sender_Type; //Not 100% sure (0 = Player, 2 = Tom Nook, 6 = HRA, A = Post Office)
        public Item Stationary_Type;
        public ACString Message_Data;
        public string Message;

        public Mail(int offset, bool Player_Mail = false)
        {
            this.Player_Mail = Player_Mail;
            Offset = Player_Mail ? offset : offset - 0x2A;
            if (Player_Mail)
            {
                Reciepiant_Name = DataConverter.ReadString(Offset, 8).Trim();
                Reciepiant_Town_Name = DataConverter.ReadString(Offset + 8, 8).Trim();
                Reciepiant_ID = DataConverter.ReadUInt(Offset + 0x10);
                Sender_Name = DataConverter.ReadString(Offset + 0x16, 8).Trim();
                Sender_Town_Name = DataConverter.ReadString(Offset + 0x1E, 8).Trim();
                Sender_ID = DataConverter.ReadUInt(Offset + 0x26);
            }
            Event_Flag = DataConverter.ReadByte(Offset + 0x2A);
            Unknown_1 = DataConverter.ReadByte(Offset + 0x2B);
            Present = new Item(DataConverter.ReadUShort(Offset + 0x2C));
            Read = DataConverter.ReadByte(Offset + 0x2E);
            Unknown_2 = DataConverter.ReadByte(Offset + 0x2F);
            Sender_Type = DataConverter.ReadByte(Offset + 0x30);
            Stationary_Type = new Item((ushort)((0x20 << 8) + DataConverter.ReadByte(Offset + 0x31)));
            Message_Data = DataConverter.ReadString(Offset + 0x32, 0xF8);
            Message = Message_Data.Clean();
        }

        public string GetMessage()
        {
            int idx = Message.IndexOf(",");
            if (idx > -1)
                return string.Format("{0}{1},\n\n{2}", idx > 0 ? Message.Substring(0, idx - 1) : "", Reciepiant_Name, Message.Substring(idx + 1));
            else
                return string.Format("{0}{1}{2}", Message.IndexOf(" ") > 0 ? Message.Substring(0, Message.IndexOf(" ") + 1) : "", Reciepiant_Name + "\n",
                    Message.IndexOf(" ") > 0 ? Message.Substring(Message.IndexOf(" ")) : Message);
        }
    }

    //Struct Size: 0xC8
    public class Messageboard_Post
    {
        public ACString Post;
        public string Post_String;
        public ACDate Post_Date;

        public Messageboard_Post(int offset)
        {
            Post = DataConverter.ReadString(offset, 0xC0);
            Post_String = Post.Clean();
            Post_Date = new ACDate(DataConverter.ReadDataRaw(offset + 0xC0, 8));
            //MessageBox.Show(string.Format("{0}\n\nPosted at:\n{1}", Post_String, Post_Date.Date_Time_String));
        }
    }
}
