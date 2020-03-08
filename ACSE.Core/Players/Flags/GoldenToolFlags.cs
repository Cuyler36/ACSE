using System;
using ACSE.Core.Saves;

namespace ACSE.Core.Players.Flags
{
    public static partial class Flags
    {
        // TODO: This will only work for gen1 games. The methods will have to check the save type when other games are supported.
        public enum GoldenItemFlag
        {
            GoldenNet = 1,
            GoldenAxe = 2,
            GoldenShovel = 3,
            GoldenRod = 4
        }

        private static int GetGoldenItemFlagsOffset(this Player player)
        {
            switch (Save.SaveInstance.SaveType)
            {
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                    return player.Offset + 0x235A;
            }

            return -1;
        }

        private static byte GetGoldenItemFlags(this Player player) => GetGoldenItemFlagsOffset(player) == -1
            ? (byte) 0xFF
            : Save.SaveInstance.ReadByte(GetGoldenItemFlagsOffset(player));

        public static void ToggleGoldenItemRecievedFlag(this Player player, GoldenItemFlag flag, bool on) =>
            Save.SaveInstance.Write(GetGoldenItemFlagsOffset(player), GetGoldenItemFlags(player).SetBit((byte) flag, on));

        public static bool IsGoldenItemFlagSet(this Player player, GoldenItemFlag flag) =>
            GetGoldenItemFlags(player).GetBit((byte) flag) == 1;
    }
}
