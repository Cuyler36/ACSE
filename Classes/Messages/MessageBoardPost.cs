using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACSE.Messages
{
    public class MessageBoardPost : MessageBase
    {
        public string Author { get; set; }
        public ushort AuthorID { get; set; }
    }
}
