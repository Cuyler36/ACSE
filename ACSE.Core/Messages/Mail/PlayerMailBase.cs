namespace ACSE.Core.Messages.Mail
{
    public class PlayerMailBase : MailBase
    {
        public string Sender { get; set; }
        public ushort SenderId { get; set; }
        public string Receipiant { get; set; }
        public ushort ReceipiantId { get; set; }
        public string SenderTownName { get; set; }
        public ushort SenderTownId { get; set; }

        public PlayerMailBase() : base()
        {
            Sender = "";
            SenderId = 0;
            Receipiant = "";
            ReceipiantId = 0;
        }

        public bool IsMailFromPlayer()
            => SenderId != 0xFFFF && SenderId != 0 && SenderTownId != 0xFFFF && SenderTownId != 0;
    }
}
