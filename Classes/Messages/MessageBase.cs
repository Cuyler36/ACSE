using ACSE.Classes.Utilities;

namespace ACSE.Messages
{
    public abstract class MessageBase
    {
        public string Contents { get; set; }
        public string TownName { get; set; }
        public ushort TownID { get; set; }
        public ACDate Date { get; set; }

        public MessageBase()
        {
            Contents = "";
            TownName = "";
            TownID = 0;
        }

        public MessageBase(string Contents, string TownName, ushort TownID, ACDate Date)
        {
            this.Contents = Contents;
            this.TownName = TownName;
            this.TownID = TownID;
            this.Date = Date;
        }
    }
}
