using System;
using ACSE.Core.Items;

namespace ACSE.Core.Messages.Mail
{
    public sealed class GCNAnimalMail
    {
        private byte _paperType;

        public LetterType LetterType;

        public byte PaperType
        {
            get => _paperType;
            set
            {
                if (value > 0x3F)
                {
                    throw new ArgumentOutOfRangeException(
                        $"PaperType must one of the 64 paper types. Values expected are in the range of: 0 - 63. Value Received: {value}");
                }

                _paperType = value;
            }
        }

        public Item Present;
    }
}
