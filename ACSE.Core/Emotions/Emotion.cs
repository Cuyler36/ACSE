using ACSE.Core.Players;
using ACSE.Core.Saves;

namespace ACSE.Core.Emotions
{
    public static class Emotion
    {
        private static readonly byte[] NewLeafEmotions = {
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E,
            0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D,
            0x1E, 0x20, 0x21, 0x24, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2E
        };

        private static byte[] GetEmotionSet(SaveGeneration generation)
        {
            switch (generation)
            {
                case SaveGeneration.N3DS:
                    return NewLeafEmotions;
                default:
                    return null;
            }
        }

        public static bool FillEmotions(Save save, Player player)
        {
            if (save == null || player == null || !player.Exists || player.Offsets.Emotions < 0)
            {
                return false;
            }

            var emotionSet = GetEmotionSet(save.SaveGeneration);

            if (emotionSet == null)
            {
                return false;
            }

            for (var i = 0; i < emotionSet.Length; i++)
            {
                save.Write(player.Offset + player.Offsets.Emotions + i, emotionSet[i]);
            }

            return true;
        }

        public static bool ClearEmotions(Save save, Player player)
        {
            if (save == null || player == null || !player.Exists || player.Offsets.Emotions < 0)
            {
                return false;
            }

            var emotionSet = GetEmotionSet(save.SaveGeneration);

            if (emotionSet == null)
            {
                return false;
            }

            for (var i = 0; i < emotionSet.Length; i++)
            {
                save.Write(player.Offset + player.Offsets.Emotions + i, (byte)0);
            }

            return true;
        }
    }
}
