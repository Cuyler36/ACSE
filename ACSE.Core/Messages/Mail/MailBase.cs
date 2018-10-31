using ACSE.Core.Items;
using ACSE.Core.Saves;

namespace ACSE.Core.Messages.Mail
{
    /// <summary>
    /// N64/GameCube/iQue letter type/state
    /// </summary>
    public enum LetterType : sbyte
    {
        None = -1,
        Unread = 0,
        Written = 1,
        Opened = 2,
        // TODO: Determine what differences these have.
        Unread2 = 3,
        Opened2 = 4
    }

    /// <summary>
    /// N64/GameCube/iQue letter sender type
    /// </summary>
    public enum LetterSenderType : byte
    {
        Normal = 0,
        Jingle = 1,
        Nook = 2,
        Redd = 3,
        Home = 4,
        Katrina = 5,
        HRA = 6,
        Nook2 = 7, // Special Nook case? TODO: Figure out where it's used. Might be sale fliers.
        Snowman = 8,
        Chip = 9
    }

    public class MailBase : MessageBase
    {
        public bool Read { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public byte HeaderReceipiantStartOffset { get; set; }
        public Item StationaryType { get; set; }
        public Item Present { get; set; }

        public MailBase() : base()
        {
            Read = false;
            Header = "";
            Footer = "";
            StationaryType = new Item();
            Present = new Item();
        }

        public MailBase(int offset, SaveType saveType)
        {

        }
    }
}
