namespace ACSE.Core.Messages
{
    public class MessageBoardPost : MessageBase
    {
        public string Author { get; set; }
        public ushort AuthorId { get; set; }
    }
}
