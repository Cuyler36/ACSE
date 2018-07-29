using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACSE.Messages.Mail
{
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

        public MailBase(int Offset, SaveType Save_Type)
        {

        }
    }
}
