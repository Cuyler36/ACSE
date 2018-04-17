using ACSE.Classes.Utilities;

namespace ACSE
{
    public abstract class Message
    {
        private string contents;
        private string townName;
        private ushort townId;
        private ACDate date;

        public string Contents { get => contents; set => contents = value; }
        public string TownName { get => townName; set => townName = value; }
        public ushort TownId { get => townId; set => townId = value; }
        public ACDate Date { get => date; set => date = value; }

        public Message()
        {
            contents = "";
            townName = "";
            townId = 0;
        }

        public Message(string Contents, string TownName, ushort TownId, ACDate Date)
        {
            this.Contents = Contents;
            this.TownName = TownName;
            this.TownId = TownId;
            this.Date = Date;
        }
    }

    public class AnimalMail : Message
    {
        private bool read;
        private string header;
        private string footer;
        private Item stationaryType;

        public bool Read { get => read; set => read = value; }
        public string Header { get => header; set => header = value; }
        public string Footer { get => footer; set => footer = value; }
        public Item StationaryType { get => stationaryType; set => stationaryType = value; }

        public AnimalMail() : base()
        {
            read = false;
            header = "";
            footer = "";
            stationaryType = new Item();
        }

        public AnimalMail(int Offset, SaveType Save_Type)
        {

        }
    }

    public class Mail : AnimalMail
    {
        // Mail in Welcome Amiibo is 0x280 bytes long per (starts with Player Id then Player Name)
        private string sender;
        private ushort senderId;
        private string receipiant;
        private ushort receipiantId;

        public string Sender { get => sender; set => sender = value; }
        public ushort SenderID { get => senderId; set => senderId = value; }
        public string Receipiant { get => receipiant; set => receipiant = value; }
        public ushort ReceipiantID { get => receipiantId; set => receipiantId = value; }

        public Mail() : base()
        {
            sender = "";
            senderId = 0;
            receipiant = "";
            receipiantId = 0;
        }
    }

    public class MessageBoardPost : Message
    {
        private string author;
        private ushort authorId;

        public string Author { get => author; set => author = value; }
        public ushort AuthorID { get => authorId; set => authorId = value; }
    }
}
