namespace ACSE
{
    public static class Weather
    {
        private static readonly string[] AC_Weather = new string[5]
        {
            "None", "Rain", "Snow", "Cherry Blossoms", "Fall Leaves"
        };

        public static string[] GetWeatherTypesForGame(SaveGeneration Generation)
        {
            switch (Generation)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                    return AC_Weather;
                default:
                    return new string[0];
            }
        }

        public static byte GetWeatherIndex(byte Weather, SaveGeneration Generation)
        {
            switch (Generation)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                    return (byte)(Weather >> 4);
                default:
                    return Weather;
            }
        }

        public static byte ToWeatherByte(byte Index, SaveGeneration Generation)
        {
            switch (Generation)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                    return (byte)((MainForm.Save_File.ReadByte(MainForm.Save_File.Save_Data_Start_Offset + MainForm.Save_File.Save_Info.Save_Offsets.Weather) & 0x0F) | (Index << 4));
                default:
                    return Index;
            }
        }

        public static bool UpdateWeather(Save SaveFile, byte Index)
        {
            if (SaveFile.Save_Generation == SaveGeneration.N64 || SaveFile.Save_Generation == SaveGeneration.GCN)
            {
                if (Index != 4 || System.Windows.Forms.MessageBox.Show("Setting the Weather to Fall Leaves will cause glitches, and will crash your game if you run."
                    + "Are you sure you want to change the weather to it?", "Weather Warning", System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    SaveFile.Write(SaveFile.Save_Data_Start_Offset + SaveFile.Save_Info.Save_Offsets.Weather, ToWeatherByte(Index, SaveFile.Save_Generation));
                    return true;
                }
            }
            return false;
        }
    }
}
