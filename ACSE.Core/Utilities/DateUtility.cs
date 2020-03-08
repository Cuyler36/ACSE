using System;
using ACSE.Core.Saves;

namespace ACSE.Core.Utilities
{
    public class AcDate
    {
        private static readonly string[] Months = {
            "January", "February", "March", "April",
            "May", "June", "July", "August",
            "September", "October", "November", "December"
        };

        public uint Second;
        public uint Minute;
        public uint Hour;
        public uint Day;
        public uint DayOfWeek;
        public uint Month;
        public uint Year;
        public string DateTimeString;
        public bool IsPm;

        public AcDate()
        {
            var now = DateTime.Now;
            Second = (uint)now.Second;
            Minute = (uint)now.Minute;
            Hour = (uint)now.Hour;
            Day = (uint)now.Day;
            DayOfWeek = (uint)now.DayOfWeek;
            Month = (uint)now.Month;
            Year = (uint)now.Year;
            IsPm = Hour >= 12;

            DateTimeString = Format("(h):(m):(s) (a), (M)/(d)/(y)");
        }

        public AcDate(byte[] dateData)
        {
            switch (Save.SaveInstance.SaveType)
            {
                case SaveType.DoubutsuNoMori:
                case SaveType.DoubutsuNoMoriPlus:
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                case SaveType.AnimalCrossing:
                case SaveType.DongwuSenlin:
                case SaveType.CityFolk: // TODO: Determine if cases 4 and 8 are correct.
                    switch (dateData.Length)
                    {
                        case 2:
                            Month = dateData[0];
                            Day = dateData[1];
                            break;
                        case 4:
                            Year = (ushort)((dateData[0] << 8) | dateData[1]);
                            Month = dateData[2];
                            Day = dateData[3];
                            break;
                        case 8:
                            Second = dateData[0];
                            Minute = dateData[1];
                            Hour = dateData[2];
                            Day = dateData[3];
                            DayOfWeek = dateData[4];
                            Month = dateData[5];
                            Year = (ushort)((dateData[6] << 8) | dateData[7]);
                            break;
                    }
                    break;

                case SaveType.WildWorld:
                    switch (dateData.Length)
                    {
                        case 2:
                            Day = dateData[0];
                            Month = dateData[1];
                            break;
                        case 3:
                            Day = dateData[0];
                            Month = dateData[1];
                            Year = 2000u + dateData[2];
                            break;
                        case 4:
                        case 8:
                            break;
                    }
                    break;
                case SaveType.NewLeaf:
                case SaveType.WelcomeAmiibo:
                    switch (dateData.Length)
                    {
                        case 2:
                            Day = dateData[1];
                            Month = dateData[0];
                            break;
                        case 4:
                            Day = dateData[3];
                            Month = dateData[2];
                            Year = (ushort)((dateData[1] << 8) + dateData[0]);
                            break;
                    }
                    break;
            }
            IsPm = Hour >= 12;

            DateTimeString = Format("(h):(m):(s) (a), (M)/(d)/(y)");
        }

        public string Format(string formatString)
        {
            formatString = formatString.Replace("(s)", Second.ToString("D2"));
            formatString = formatString.Replace("(m)", Minute.ToString("D2"));
            formatString = formatString.Replace("(h)", (Hour % 12) == 0 ? "12" : (Hour % 12).ToString());
            formatString = formatString.Replace("(H)", Hour.ToString("D2"));
            formatString = formatString.Replace("(d)", Day.ToString());
            formatString = formatString.Replace("(D)", Day.ToString("D2"));
            formatString = formatString.Replace("(w)", DayOfWeek.ToString());
            formatString = formatString.Replace("(W)", ((DayOfWeek)DayOfWeek).ToString());

            if (formatString.Contains("(mo)"))
            {
                formatString = formatString.Replace("(mo)", Months[Month]);
            }

            formatString = formatString.Replace("(M)", Month.ToString("D2"));
            formatString = formatString.Replace("(y)", Year.ToString());
            formatString = formatString.Replace("(Y)", Year.ToString("D4").Substring(2, 2));
            formatString = formatString.Replace("(a)", IsPm ? "PM" : "AM");
            formatString = formatString.Replace("(A)", IsPm ? "P.M." : "A.M.");
            return formatString;
        }

        public byte[] ToFullDateData()
        {
            switch (Save.SaveInstance.SaveGeneration)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                    return new[]
                    {
                        (byte)Second,
                        (byte)Minute,
                        (byte)Hour,
                        (byte)Day,
                        (byte)DayOfWeek,
                        (byte)Month,
                        (byte)((Year & 0xFF00) >> 8),
                        (byte)(Year & 0x00FF)
                    };
                default:
                    return new byte[0]; // TODO: Wild World+ Date Research
            }
        }

        public byte[] ToYearMonthDayDateData()
        {
            switch (Save.SaveInstance.SaveGeneration)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                    return new[]
                    {
                        (byte)((Year & 0xFF00) >> 8),
                        (byte)(Year & 0x00FF),
                        (byte)Month,
                        (byte)Day
                    };
                case SaveGeneration.NDS:
                    return new[]
                    {
                        (byte)Day,
                        (byte)Month,
                        (byte)Year
                    };
                case SaveGeneration.N3DS:
                    return new[]
                    {
                        (byte)(Year & 0x00FF),
                        (byte)((Year >> 8) & 0x00FF),
                        (byte)Month,
                        (byte)Day
                    };
                default:
                    return new byte[0];
            }
        }

        public byte[] ToMonthDayDateData()
        {
            switch (Save.SaveInstance.SaveGeneration)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                case SaveGeneration.Wii:
                case SaveGeneration.N3DS:
                    return new[] { (byte)Month, (byte)Day };

                case SaveGeneration.NDS:
                    return new[] { (byte)Day, (byte)Month };

                default:
                    return new byte[0];
            }
        }
    }
}