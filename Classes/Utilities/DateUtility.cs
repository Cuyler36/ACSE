using System;

namespace ACSE.Classes.Utilities
{
    public static class DateUtility
    {
        //Might use for something
        public static uint Day_String_to_Number(string day)
        {
            if (Enum.IsDefined(typeof(DayOfWeek), day))
                return Convert.ToUInt32((DayOfWeek)Enum.Parse(typeof(DayOfWeek), day, true));
            return 0; //Guess we're going Sunday
        }
    }

    public class ACDate
    {
        public uint Second = 0;
        public uint Minute = 0;
        public uint Hour = 0;
        public uint Day = 0;
        public uint Day_of_Week = 0;
        public uint Month = 0;
        public uint Year = 2000; //Default
        public string Date_Time_String = "";
        public bool Is_PM = false;
        public bool Is_Birthday = false;

        public ACDate(byte[] dateData)
        {
            switch (NewMainForm.Save_File.Save_Type)
            {
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
                        Year = BitConverter.ToUInt16(new byte[] { dateData[7], dateData[6] }, 0);
                    }
                    else if (dateData.Length == 0x4)
                    {
                        Year = BitConverter.ToUInt16(new byte[] { dateData[1], dateData[0] }, 0);
                        Month = dateData[2];
                        Day = dateData[3];
                    }
                    else if (dateData.Length == 0x2)
                    {
                        Month = dateData[0];
                        Day = dateData[1];
                        Is_Birthday = true;
                    }
                    break;
                case SaveType.New_Leaf:
                case SaveType.Welcome_Amiibo:
                    if (dateData.Length == 2)
                    {
                        Day = dateData[1];
                        Month = dateData[0];
                        Is_Birthday = true; // ??
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
                Minute.ToString("D2"), Second.ToString("D2"), Is_PM ? "PM" : "AM", Month, Day, Year); //Default date/time string
            //System.Windows.Forms.MessageBox.Show(Format("(M)/(D)/(y)"));
        }

        public string Format(string formatString) //Need to redo this if there is a more efficient/cleaner way
        {
            formatString = formatString.Replace("(s)", Second.ToString("D2"));
            formatString = formatString.Replace("(m)", Minute.ToString("D2"));
            formatString = formatString.Replace("(h)", (Hour % 12) == 0 ? "12" : (Hour % 12).ToString());
            formatString = formatString.Replace("(H)", Hour.ToString());
            formatString = formatString.Replace("(d)", Day.ToString());
            formatString = formatString.Replace("(D)", Day.ToString("D2"));
            formatString = formatString.Replace("(w)", Day_of_Week.ToString());
            formatString = formatString.Replace("(W)", Enum.GetName(typeof(DayOfWeek), Day_of_Week));
            formatString = formatString.Replace("(mo)", Month.ToString());
            formatString = formatString.Replace("(M)", Month.ToString("D2"));
            formatString = formatString.Replace("(y)", Year.ToString());
            formatString = formatString.Replace("(Y)", Year.ToString().Substring(2, 2));
            formatString = formatString.Replace("(a)", Is_PM ? "PM" : "AM");
            formatString = formatString.Replace("(A)", Is_PM ? "P.M." : "A.M.");
            return formatString;
        }

        public byte[] ToBytes()
        {
            if (!Is_Birthday)
            {
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
            }
            else
            {
                return new byte[2] { (byte)Month, (byte)Day };
            }
        }
    }

}
