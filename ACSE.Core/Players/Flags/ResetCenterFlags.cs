using System;
using ACSE.Core.Saves;

namespace ACSE.Core.Players.Flags
{
    public static partial class Flags
    {
        private static readonly int[] mode_table_0 = {2, 4, 6};
        private static readonly int[] mode_table_1 = {2, 3, 4, 5, 6, 7};

        public enum ResetFlag : byte
        {
            UseGenericDialog = 5, // When unset, the dialog that is retrieved will be unique.
            EnteredResetCenter = 6, // When unset, the unique dialog that is shown will be the dialog played when first entering the reset center.
            MetDonResetti = 7 // When unset, the unique dialog that will be shown is Don & Resetti drama. Occurs after the 5th entry.
        }

        private static byte GetResetFlags(this Player player) => Save.SaveInstance.ReadByte(player.Offset + 0x235C);

        public static int GetNextResetMode(this Player player)
        {
            var flags = player.GetResetFlags();
            var type = flags & 0xF;

            if (flags.GetBit(6) == 1)
            {
                if (player.Data.ResetCount < 5)
                {
                    if (type >= 3)
                    {
                        type = 0;
                    }

                    return mode_table_0[type];
                }

                if (flags.GetBit(7) == 1)
                {
                    if (type >= 6)
                    {
                        type = 0;
                    }

                    return mode_table_1[type];
                }

                return 1;
            }

            return 0;
        }

        public static void ToggleResetFlag(this Player player, ResetFlag resetFlag, bool on) =>
            Save.SaveInstance.Write(player.Offset + 0x235C, player.GetResetFlags().SetBit((byte) resetFlag, on));

        public static bool IsResetFlagEnabled(this Player player, ResetFlag resetFlag) =>
            player.GetResetFlags().GetBit((byte) resetFlag) == 1;
    }
}
