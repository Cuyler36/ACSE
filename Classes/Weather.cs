namespace ACSE
{
    public static class Weather
    {
        private static readonly string[] AcWeather = {
            "None", "Rain", "Snow", "Cherry Blossoms", "Fall Leaves"
        };

        public static string[] GetWeatherTypesForGame(SaveGeneration generation)
        {
            switch (generation)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                    return AcWeather;
                default:
                    return new string[0];
            }
        }

        public static byte GetWeatherIndex(byte weather, SaveGeneration generation)
        {
            switch (generation)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                    return (byte)(weather >> 4);
                default:
                    return weather;
            }
        }

        public static byte ToWeatherByte(byte index, SaveGeneration generation)
        {
            switch (generation)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                    return (byte)((MainForm.SaveFile.ReadByte(MainForm.SaveFile.SaveDataStartOffset + MainForm.SaveFile.SaveInfo.SaveOffsets.Weather) & 0x0F) | (index << 4));
                default:
                    return index;
            }
        }

        public static bool UpdateWeather(Save saveFile, byte index)
        {
            if (saveFile.SaveGeneration != SaveGeneration.N64 && saveFile.SaveGeneration != SaveGeneration.GCN)
                return false;
            if (saveFile.SaveType != SaveType.DoubutsuNoMoriEPlus && saveFile.SaveType != SaveType.AnimalForestEPlus
                                                                  && index == 4 && System.Windows.Forms.MessageBox.Show(
                    "Setting the Weather to Fall Leaves will cause glitches, and will crash your game if you run."
                    + "Are you sure you want to change the weather to it?", "Weather Warning",
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Question) !=
                System.Windows.Forms.DialogResult.Yes) return false;
            saveFile.Write(saveFile.SaveDataStartOffset + saveFile.SaveInfo.SaveOffsets.Weather, ToWeatherByte(index, saveFile.SaveGeneration));
            return true;
        }
    }
}
