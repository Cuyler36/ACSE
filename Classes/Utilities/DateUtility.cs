using System;

namespace ACSE.Classes.Utilities
{
    public class ACDate
    {
        static readonly string[] Months = new string[12]
        {
            "January", "February", "March", "April",
            "May", "June", "July", "August",
            "September", "October", "November", "December"
        };

        public uint Second = 0;
        public uint Minute = 0;
        public uint Hour = 0;
        public uint Day = 0;
        public uint Day_of_Week = 0;
        public uint Month = 0;
        public uint Year = 0;
        public string Date_Time_String = "";
        public bool Is_PM = false;

        public ACDate()
        {
            var Now = DateTime.Now;
            Second = (uint)Now.Second;
            Minute = (uint)Now.Minute;
            Hour = (uint)Now.Hour;
            Day = (uint)Now.Day;
            Day_of_Week = (uint)Now.DayOfWeek;
            Month = (uint)Now.Month;
            Year = (uint)Now.Year;
            Is_PM = Hour >= 12;

            Date_Time_String = string.Format("{0}:{1}:{2} {3}, {4}/{5}/{6}", (Hour % 12) == 0 ? 12 : Hour % 12,
                Minute.ToString("D2"), Second.ToString("D2"), Is_PM ? "PM" : "AM", Month, Day, Year);
        }

        public ACDate(byte[] dateData)
        {
            switch (MainForm.Save_File.Save_Type)
            {
                case SaveType.Doubutsu_no_Mori:
                case SaveType.Doubutsu_no_Mori_Plus:
                case SaveType.Doubutsu_no_Mori_e_Plus:
                case SaveType.Animal_Crossing:
                    if (dateData.Length == 0x8)
                    {
                        Second = dateData[0];
                        Minute = dateData[1];
                        Hour = dateData[2];
                        Day = dateData[3];
                        Day_of_Week = dateData[4];
                        Month = dateData[5];
                        Year = (ushort)((dateData[6] << 8) | dateData[7]);
                    }
                    else if (dateData.Length == 0x4)
                    {
                        Year = (ushort)((dateData[0] << 8) | dateData[1]);
                        Month = dateData[2];
                        Day = dateData[3];
                    }
                    else if (dateData.Length == 0x2)
                    {
                        Month = dateData[0];
                        Day = dateData[1];
                    }
                    break;
                case SaveType.New_Leaf:
                case SaveType.Welcome_Amiibo:
                    if (dateData.Length == 2)
                    {
                        Day = dateData[1];
                        Month = dateData[0];
                    }
                    else if (dateData.Length == 4)
                    {
                        Day = dateData[3];
                        Month = dateData[2];
                        Year = (ushort)((dateData[1] << 8) + dateData[0]);
                    }
                    break;
            }
            Is_PM = Hour >= 12;

            Date_Time_String = string.Format("{0}:{1}:{2} {3}, {4}/{5}/{6}", (Hour % 12) == 0 ? 12 : Hour % 12,
                Minute.ToString("D2"), Second.ToString("D2"), Is_PM ? "PM" : "AM", Month, Day, Year);
        }

        public ACDate(int Offset, int Length, Save SaveFile) : this(SaveFile.ReadByteArray(Offset, Length)) { }

        public string Format(string formatString)
        {
            formatString = formatString.Replace("(s)", Second.ToString("D2"));
            formatString = formatString.Replace("(m)", Minute.ToString("D2"));
            formatString = formatString.Replace("(h)", (Hour % 12) == 0 ? "12" : (Hour % 12).ToString());
            formatString = formatString.Replace("(H)", Hour.ToString("D2"));
            formatString = formatString.Replace("(d)", Day.ToString());
            formatString = formatString.Replace("(D)", Day.ToString("D2"));
            formatString = formatString.Replace("(w)", Day_of_Week.ToString());
            formatString = formatString.Replace("(W)", Enum.GetName(typeof(DayOfWeek), Day_of_Week));
            formatString = formatString.Replace("(mo)", Months[Month]);
            formatString = formatString.Replace("(M)", Month.ToString("D2"));
            formatString = formatString.Replace("(y)", Year.ToString());
            formatString = formatString.Replace("(Y)", Year.ToString().Substring(2, 2));
            formatString = formatString.Replace("(a)", Is_PM ? "PM" : "AM");
            formatString = formatString.Replace("(A)", Is_PM ? "P.M." : "A.M.");
            return formatString;
        }

        public byte[] ToFullDateData()
        {
            switch (MainForm.Save_File.Save_Generation)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                    return new byte[8]
                    {
                        (byte)Second,
                        (byte)Minute,
                        (byte)Hour,
                        (byte)Day,
                        (byte)Day_of_Week,
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
            switch (MainForm.Save_File.Save_Generation)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                    return new byte[4]
                    {
                        (byte)((Year & 0xFF00) >> 8),
                        (byte)(Year & 0x00FF),
                        (byte)Month,
                        (byte)Day
                    };
                case SaveGeneration.N3DS:
                    return new byte[4]
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
            switch (MainForm.Save_File.Save_Generation)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                case SaveGeneration.N3DS:
                    return new byte[2] { (byte)Month, (byte)Day };
                default:
                    return new byte[0];
            }
        }
    }
}