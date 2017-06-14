using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ACSE
{
    public static class StringUtil
    {
        public static readonly Dictionary<byte, string> AC_CharacterDictionary = new Dictionary<byte, string>()
        {
            {0x90, "–" },
            {0xCD, "\n" },
            {0x2A, "~" },
            {0xD0, ";" },
            {0xD4, "⚷" },
            {0xD1, "#" },
            {0x85, "•" },
            {0xA2, "Æ" },
            {0xA0, "¯" },
            {0xAE, "/" },
            {0x97, "¦" },
            {0xC0, "×" },
            {0xC1, "÷" },
            {0xA5, "»" },
            {0xA6, "«" },
            {0xAC, "∋" },
            {0xAD, "∈" },
            {0xB4, "+" },
            {0x1D, "ß" },
            {0x1E, "Þ" },
            {0x86, "ð" },
            {0x98, "§" },
            {0x9B, "‖" },
            {0x9C, "µ" },
            {0xA1, "¬" },
            {0x2B, "♥" },
            {0xB9, "★" },
            {0x2F, "♪" },
            {0x3B, "🌢" },
            {0x5C, "💢" },
            {0xB8, "🍀"},
            {0xC6, "🐾" },
            {0xB6, "♂" },
            {0xB7, "♀" },
            {0xAF, "∞" },
            {0xB0, "○" },
            {0xB1, "🗙" },
            {0xB2, "□" },
            {0xB3, "△" },
            {0xBA, "💀" },
            {0xBB, "😮" },
            {0xBC, "😄" },
            {0xBD, "😣" },
            {0xBE, "😠" },
            {0xBF, "😃" },
            {0xA7, "☀" },
            {0xA8, "☁" },
            {0xA9, "☂" },
            {0xAB, "☃" },
            {0xAA, "🌬" }, //Wind...
            {0xB5, "⚡" },
            {0xC2, "🔨" }, //Hammer??
            {0xC3, "🎀" }, //Not sure wtf this is (put it as ribbon)
            {0xC4, "✉" },
            {0xC7, "🐶" },
            {0xC8, "🐱" },
            {0xC9, "🐰" },
            {0xCA, "🐦" },
            {0xCB, "🐮" },
            {0xCC, "🐷" },
            {0xC5, "💰" },
            {0xCE, "🐟" },
            {0xCF, "🐞" },
            {0x7B, "è" },
            {0x7C, "é" },
            {0x7D, "ê" },
            {0x7E, "ë" },
            {0x93, "ý" },
            {0x94, "ÿ" },
            {0x8E, "ù" },
            {0x8F, "ú" },
            {0x91, "û" },
            {0x92, "ü" },
            {0x81, "ì" },
            {0x82, "í" },
            {0x83, "î" },
            {0x84, "ï" },
            {0x88, "ò" },
            {0x89, "ó" },
            {0x8A, "ô" },
            {0x8B, "õ" },
            {0x8C, "ö" },
            {0x1F, "à" },
            {0x23, "á" },
            {0x24, "â" },
            {0x5B, "ã" },
            {0x5D, "ä" },
            {0x5E, "å" },
            {0x09, "È" },
            {0x0A, "É" },
            {0x0B, "Ê" },
            {0x0C, "Ë" },
            {0x96, "Ý" },
            {0x19, "Ù" },
            {0x1A, "Ú" },
            {0x1B, "Û" },
            {0x1C, "Ü" },
            {0x0D, "Ì" },
            {0x0E, "Í" },
            {0x0F, "Î" },
            {0x10, "Ï" },
            {0x13, "Ò" },
            {0x14, "Ó" },
            {0x15, "Ô" },
            {0x16, "Õ" },
            {0x17, "Ö" },
            {0x02, "Ä" },
            {0x03, "À" },
            {0x04, "Á" },
            {0x05, "Â" },
            {0x06, "Ã" },
            {0x07, "Å" },
            {0x11, "Ð" },
            {0x08, "Ç" },
            {0x12, "Ñ" },
            {0x87, "ñ" },
            {0x60, "ç" },
            {0x95, "þ" },
            {0x01, "¿" },
            {0xA4, "„" },
            {0xA3, "æ" }
        };

        public static readonly Dictionary<byte, string> WW_CharacterDictionary = new Dictionary<byte, string>()
        {
            { 0x41, "Œ"},
            { 0x44, "œ"},
            { 0x47, "À"},
            { 0x48, "Á"},
            { 0x49, "Â"},
            { 0x4B, "Ä"},
            { 0x4E, "Ç"},
            { 0x4F, "È"},
            { 0x50, "É"},
            { 0x51, "Ê"},
            { 0x52, "Ë"},
            { 0x53, "Ì"},
            { 0x54, "Í"},
            { 0x55, "Î"},
            { 0x56, "Ï"},
            { 0x58, "Ñ"},
            { 0x59, "Ò"},
            { 0x5A, "Ó"},
            { 0x5B, "Ô"},
            { 0x5D, "Ö"},
            { 0x5F, "Ù"},
            { 0x60, "Ú"},
            { 0x61, "Û"},
            { 0x62, "Ü"},
            { 0x65, "ß"},
            { 0x66, "à"},
            { 0x67, "á"},
            { 0x68, "â"},
            { 0x6A, "ä"},
            { 0x6D, "ç"},
            { 0x6E, "è"},
            { 0x6F, "é"},
            { 0x70, "ê"},
            { 0x71, "ë"},
            { 0x72, "ì"},
            { 0x73, "í"},
            { 0x74, "î"},
            { 0x75, "ï"},
            { 0x77, "ñ"},
            { 0x78, "ò"},
            { 0x79, "ó"},
            { 0x7A, "ô"},
            { 0x7C, "ö"},
            { 0x7E, "ù"},
            { 0x7F, "ú"},
            { 0x80, "û"},
            { 0x81, "ü"},
            { 0x85, " "}, // "(space)"
            { 0x86, " "}, //breaking space
            { 0x87, "!" },
            //{ 0x87, "\n"},
            { 0x8C, "&" },
            { 0x8D, "\"" }, // single quote
            { 0x8E, "(" },
            { 0x8F, ")" },
            { 0x91, "+" },
            { 0x92, "," },
            { 0x93, "-" },
            { 0x94, "." },
            { 0x96, ":" },
            { 0x97, ";" },
            { 0x98, "<" },
            { 0x99, "=" },
            { 0x9A, ">" },
            { 0x9B, "?" },
            { 0x9C, "@" },
            { 0x9D, "[" },
            { 0x9F, "]" },
            { 0xA1, "_" },
            { 0xA6, "~" },
            { 0xA7, "€"},
            { 0xB1, "'" },
            { 0xB3, "\"" },
            { 0xBB, "¡"}, //(i)??
            { 0xBC, "¢"},
            { 0xBD, "£"},
            { 0xD1, "•"},
            { 0xD9, "¿"},
            { 0xDA, "×"},
            { 0xDB, "÷"},
            { 0xDC, "💧"},
            { 0xDD, "★"},
            { 0xDE, "❤"},
            { 0xDF, "♪"},
        };

        public static int StringToMaxChars(string s)
        {
            TextElementEnumerator t = StringInfo.GetTextElementEnumerator(s);
            int size = 0;
            int chars = 0;
            while (t.MoveNext())
            {
                chars++;
                size += Encoding.UTF8.GetBytes(((string)(t.Current)).ToCharArray()).Length;
            }
            return size;
        }

        public static byte[] Fix_Wild_World_String(byte[] mangled_String_Bytes)
        {
            byte[] Fixed_String_Bytes = new byte[mangled_String_Bytes.Length];
            for (int i = 0; i < mangled_String_Bytes.Length; i++)
            {
                byte Mangled_Char = mangled_String_Bytes[i];
                if (Mangled_Char > 0 && Mangled_Char <= 0x1A)
                    Fixed_String_Bytes[i] = (byte)(Mangled_Char + 0x40);
                else if (Mangled_Char >= 0x1B && Mangled_Char <= 0x34)
                    Fixed_String_Bytes[i] = (byte)(Mangled_Char + 0x46);
                else if (Mangled_Char >= 0x35 && Mangled_Char <= 0x3E)
                    Fixed_String_Bytes[i] = (byte)(Mangled_Char - 0x5); //Char - 0x35 + 0x30 (0)
            }
            return Fixed_String_Bytes;
        }

        public static byte[] To_Wild_World_String(byte[] string_Bytes)
        {
            byte[] WW_String_Bytes = new byte[string_Bytes.Length];
            for (int i = 0; i < WW_String_Bytes.Length; i++)
            {
                byte Char = string_Bytes[i];
                if (Char > 0x40 && Char <= 0x5A)
                    WW_String_Bytes[i] = (byte)(Char - 0x40);
                else if (Char >= 0x61 && Char <= 0x7A)
                    WW_String_Bytes[i] = (byte)(Char - 0x46);
                else if (Char >= 0x30 && Char <= 0x39)
                    WW_String_Bytes[i] = (byte)(Char + 0x5);
                else if (WW_CharacterDictionary.Values.Contains(Encoding.ASCII.GetString(new byte[1] { Char })))
                    WW_String_Bytes[i] = WW_CharacterDictionary.First(x => x.Value == Encoding.ASCII.GetString(new byte[1] { Char })).Key;
            }
            return WW_String_Bytes;
        }
    }

    public class ACString
    {
        byte[] String_Bytes;
        static SaveType Save_Type; //This won't be changing during our save, so a static cast is fine
        public string String = "";
        static Dictionary<byte, string> Char_Dictionary;

        public ACString(byte[] stringBuffer, SaveType saveType = SaveType.Animal_Crossing)
        {
            Save_Type = saveType;
            String_Bytes = stringBuffer;
            Char_Dictionary = saveType == SaveType.Animal_Crossing ? StringUtil.AC_CharacterDictionary
                : (saveType == SaveType.Wild_World ? StringUtil.WW_CharacterDictionary : null);
            if (saveType == SaveType.Animal_Crossing || saveType == SaveType.Wild_World)
                foreach (byte b in stringBuffer)
                    if (Char_Dictionary != null && Char_Dictionary.ContainsKey(b))
                        String += Char_Dictionary[b];
                    else
                        String += Encoding.UTF8.GetString(saveType == SaveType.Wild_World ? StringUtil.Fix_Wild_World_String(new byte[1] { b }) : new byte[1] { b });
            else if (saveType == SaveType.City_Folk)
                for (int i = 0; i < stringBuffer.Length; i += 2)
                    String += Encoding.Unicode.GetString(stringBuffer.Skip(i).Take(2).Reverse().ToArray());
            else if (saveType == SaveType.New_Leaf || saveType == SaveType.Welcome_Amiibo)
            {
                String = Encoding.Unicode.GetString(stringBuffer);
            }
        }

        public static byte[] GetBytes(string String, int maxSize = 0)
        {
            if (Save_Type == SaveType.Animal_Crossing)
            {
                byte[] returnedString = new byte[maxSize > 0 ? maxSize : String.Length];
                TextElementEnumerator t = StringInfo.GetTextElementEnumerator(String);
                int i = 0;
                while (t.MoveNext() && i < returnedString.Length)
                {
                    if (Char_Dictionary.ContainsValue((string)t.Current))
                        returnedString[i] = Char_Dictionary.FirstOrDefault(o => o.Value == (string)t.Current).Key;
                    else
                        returnedString[i] = Encoding.UTF8.GetBytes(new char[1] { String[t.ElementIndex] })[0];
                    i++;
                }
                for (int x = 0; x < returnedString.Length; x++)
                    if (returnedString[x] == 0)
                        returnedString[x] = 0x20;
                return returnedString;
            }
            else if (Save_Type == SaveType.Wild_World)
            {
                byte[] String_Buffer = StringUtil.To_Wild_World_String(Encoding.UTF8.GetBytes(String));
                if (maxSize > 0)
                    Array.Resize(ref String_Buffer, maxSize);
                return String_Buffer;
            }
            else if (Save_Type == SaveType.City_Folk)
            {
                byte[] String_Buffer = new byte[maxSize > 0 ? maxSize : String.Length * 2]; //Characters are now unicode
                byte[] String_Bytes = Encoding.Unicode.GetBytes(String);
                for (int i = 0; i < String_Bytes.Length; i+=2)
                    Buffer.BlockCopy(String_Bytes.Skip(i).Take(2).Reverse().ToArray(), 0, String_Buffer, i, 2);
                return String_Buffer;
            }
            else if (Save_Type == SaveType.New_Leaf || Save_Type == SaveType.Welcome_Amiibo)
            {
                byte[] String_Buffer = Encoding.Unicode.GetBytes(String);
                if (maxSize > 0)
                    Array.Resize(ref String_Buffer, maxSize);
                return String_Buffer;
            }
            else
            {
                NewMainForm.Debug_Manager.WriteLine(string.Format("StringUtil was passed an unknown SaveType enum. Received Type: {0}", Save_Type.ToString()), DebugLevel.Error);
                return null;
            }
        }

        public string Trim()
        {
            return String.Trim(' ');
        }

        public string Clean()
        {
            string Cleaned_String = Regex.Replace(String, "[\n]{3,}", "\n\n");
            return Regex.Replace(Cleaned_String.Trim(' '), "[ ]{2,}", " ");
        }
    }
}
