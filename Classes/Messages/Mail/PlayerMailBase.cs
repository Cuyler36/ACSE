using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACSE.Messages.Mail
{
    public class PlayerMailBase : MailBase
    {
        public string Sender { get; set; }
        public ushort SenderID { get; set; }
        public string Receipiant { get; set; }
        public ushort ReceipiantID { get; set; }
        public string SenderTownName { get; set; }
        public ushort SenderTownID { get; set; }

        public PlayerMailBase() : base()
        {
            Sender = "";
            SenderID = 0;
            Receipiant = "";
            ReceipiantID = 0;
        }

        public bool IsMailFromPlayer()
            => SenderID != 0xFFFF && SenderID != 0 && SenderTownID != 0xFFFF && SenderTownID != 0;
    }
}
