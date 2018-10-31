using ACSE.Core.Saves;

namespace ACSE.Core.Weather
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
                    return (byte) ((Save.SaveInstance.ReadByte(
                                        Save.SaveInstance.SaveDataStartOffset +
                                        Save.SaveInstance.SaveInfo.SaveOffsets.Weather) & 0x0F) | (index << 4));
                default:
                    return index;
            }
        }

        public static bool UpdateWeather(Save saveFile, byte index)
        {
            if (saveFile.SaveGeneration != SaveGeneration.N64 && saveFile.SaveGeneration != SaveGeneration.GCN)
                return false;
            saveFile.Write(saveFile.SaveDataStartOffset + saveFile.SaveInfo.SaveOffsets.Weather, ToWeatherByte(index, saveFile.SaveGeneration));
            return true;
        }
    }
}
