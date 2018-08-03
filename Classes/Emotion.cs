namespace ACSE
{
    public static class Emotion
    {
        private static byte[] NewLeafEmotions = new byte[]
        {
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E,
            0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D,
            0x1E, 0x20, 0x21, 0x24, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2E
        };

        private static byte[] GetEmotionSet(SaveGeneration Generation)
        {
            switch (Generation)
            {
                case SaveGeneration.N3DS:
                    return NewLeafEmotions;
                default:
                    return null;
            }
        }

        public static bool FillEmotions(Save Save, Player Player)
        {
            if (Save == null || Player == null || !Player.Exists || Player.Offsets.Emotions < 0)
            {
                return false;
            }

            byte[] EmotionSet = GetEmotionSet(Save.SaveGeneration);

            if (EmotionSet == null)
            {
                return false;
            }

            for (int i = 0; i < EmotionSet.Length; i++)
            {
                Save.Write(Player.Offset + Player.Offsets.Emotions + i, EmotionSet[i]);
            }

            return true;
        }

        public static bool ClearEmotions(Save Save, Player Player)
        {
            if (Save == null || Player == null || !Player.Exists || Player.Offsets.Emotions < 0)
            {
                return false;
            }

            byte[] EmotionSet = GetEmotionSet(Save.SaveGeneration);

            if (EmotionSet == null)
            {
                return false;
            }

            for (int i = 0; i < EmotionSet.Length; i++)
            {
                Save.Write(Player.Offset + Player.Offsets.Emotions + i, (byte)0);
            }

            return true;
        }
    }
}
