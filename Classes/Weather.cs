using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACSE
{
    public static class Weather
    {
        private static string[] AC_Weather = new string[5]
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
                    return (byte)((NewMainForm.Save_File.ReadByte(NewMainForm.Save_File.Save_Data_Start_Offset + NewMainForm.Save_File.Save_Info.Save_Offsets.Weather) & 0x0F) | (Index << 4));
                default:
                    return Index;
            }
        }
    }
}
