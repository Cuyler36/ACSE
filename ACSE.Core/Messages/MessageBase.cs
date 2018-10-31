using ACSE.Core.Utilities;

namespace ACSE.Core.Messages
{
    public abstract class MessageBase
    {
        public string Contents { get; set; }
        public string TownName { get; set; }
        public ushort TownId { get; set; }
        public AcDate Date { get; set; }

        public MessageBase()
        {
            Contents = "";
            TownName = "";
            TownId = 0;
        }

        public MessageBase(string contents, string townName, ushort townId, AcDate date)
        {
            Contents = contents;
            TownName = townName;
            TownId = townId;
            Date = date;
        }
    }
}
