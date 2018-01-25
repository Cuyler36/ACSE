using ACSE.Classes.Utilities;

namespace ACSE
{
    public abstract class Message
    {
        private string contents;
        private string townName;
        private ushort townId;
        private ACDate date;

        public string Contents { get { return contents; } set { contents = value; } }
        public string TownName { get { return townName; } set { townName = value; } }
        public ushort TownID { get { return townId; } set { townId = value; } }
        public ACDate Date { get { return date; } set { date = value; } }
    }

    public class Mail : Message
    {
        private string sender;
        private ushort senderId;
        private string receipiant;
        private ushort receipiantId;


        public string Sender { get { return sender; } set { sender = value; } }
        public ushort SenderID { get { return senderId; } set { senderId = value; } }
        public string Receipiant { get { return receipiant; } set { receipiant = value; } }
        public ushort ReceipiantID { get { return receipiantId; } set { receipiantId = value; } }
    }

    public class MessageBoardPost : Message
    {
        private string author;
        private ushort authorId;

        public string Author { get { return author; } set { author = value; } }
        public ushort AuthorID { get { return authorId; } set { authorId = value; } }
    }
}
