using System;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ACSE.Utilities
{
    internal static class StringUtility
    {
        internal static readonly string[] AcCharacterDictionary = {
            "¡", "¿", "Ä", "À", "Á", "Â", "Ã", "Å", "Ç", "È", "É", "Ê", "Ë", "Ì", "Í", "Î",
            "Ï", "Ð", "Ñ", "Ò", "Ó", "Ô", "Õ", "Ö", "Ø", "Ù", "Ú", "Û", "Ü", "ß", "Þ", "à",
            " ", "!", "\"", "á", "â", "%", "&", "'", "(", ")", "~", "♥", ",", "-", ".", "♪",
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ":", "🌢", "<", "=", ">", "?",
            "@", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O",
            "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "ã", "💢", "ä", "å", "_",
            "ç", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o",
            "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "è", "é", "ê", "ë", "□",
            "�", "ì", "í", "î", "ï", "•", "ð", "ñ", "ò", "ó", "ô", "õ", "ö", "⁰", "ù", "ú",
            "–", "û", "ü", "ý", "ÿ", "þ", "Ý", "¦", "§", "a̱", "o̱", "‖", "µ", "³", "²", "¹",
            "¯", "¬", "Æ", "æ", "„", "»", "«", "☀", "☁", "☂", "🌬", "☃", "∋", "∈", "/", "∞",
            "○", "🗙", "□", "△", "+", "⚡", "♂", "♀", "🍀", "★", "💀", "😮", "😄", "😣", "😠", "😃",
            "×", "÷", "🔨", "🎀", "✉", "💰", "🐾", "🐶", "🐱", "🐰", "🐦", "🐮", "🐷", "\n", "🐟", "🐞",
            ";", "#", "", "", "⚷", "", "", "", "", "", "", "", "", "", "", "",
            "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",
            "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""
            };

        internal static readonly string[] DoubutsuNoMoriEPlusCharMap = {
            "あ", "い", "う", "え", "お", "か", "き", "く", "け", "こ", "さ", "し", "す", "せ", "そ", "た",
            "ち", "つ", "て", "と", "な", "に", "ぬ", "ね", "の", "は", "ひ", "ふ", "へ", "ほ", "ま", "み",
            " ", "!", "\"", "む", "め", "%", "&", "'", "(", ")", "~", "♥", ",", "-", ".", "♪",
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ":", "🌢", "<", "+", ">", "?",
            "@", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O",
            "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "も", "💢", "や", "ゆ", "_",
            "よ", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o",
            "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "ら", "り", "る", "れ", "�",
            "□", "。", "｢", "｣", "、", "･", "ヲ", "ァ", "ィ", "ゥ", "ェ", "ォ", "ャ", "ュ", "ョ", "ッ",
            "ー", "ア", "イ", "ウ", "エ", "オ", "カ", "キ", "ク", "ケ", "コ", "サ", "シ", "ス", "セ", "ソ",
            "タ", "チ", "ツ", "テ", "ト", "ナ", "ニ", "ヌ", "ネ", "ノ", "ハ", "ヒ", "フ", "ヘ", "ホ", "マ",
            "ミ", "ム", "メ", "モ", "ヤ", "ユ", "ヨ", "ラ", "リ", "ル", "レ", "ロ", "ワ", "ン", "ヴ", "☺",
            "ろ", "わ", "を", "ん", "ぁ", "ぃ", "ぅ", "ぇ", "ぉ", "ゃ", "ゅ", "ょ", "っ", "\n", "ガ", "ギ",
            "グ", "ゲ", "ゴ", "ザ", "ジ", "ズ", "ゼ", "ゾ", "ダ", "ヂ", "ヅ", "デ", "ド", "バ", "ビ", "ブ",
            "ベ", "ボ", "パ", "ピ", "プ", "ペ", "ポ", "が", "ぎ", "ぐ", "げ", "ご", "ざ", "じ", "ず", "ぜ",
            "ぞ", "だ", "ぢ", "づ", "で", "ど", "ば", "び", "ぶ", "べ", "ぼ", "ぱ", "ぴ", "ぷ", "ぺ", "ぽ",
        };

        // TODO: some of the new characters are wrong. Look at them one-by-one in Wild World somehow.
        internal static readonly string[] WwCharacterDictionary = {
            "\0", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O",
            "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e",
            "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u",
            "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "⨍",
            "s̊", "Œ", "Ž", "š", "œ", "ž", "Ÿ", "À", "Á", "Â", "Ã", "Ä", "Å", "Æ", "Ç", "È",
            "É", "Ê", "Ë", "Ì", "Í", "Î", "Ï", "Đ", "Ñ", "Ò", "Ó", "Ô", "Õ", "Ö", "Ø", "Ù",
            "Ú", "Û", "Ü", "Ý", "Þ", "ß", "à", "á", "â", "ã", "ä", "å", "æ", "ç", "è", "é",
            "ê", "ë", "ì", "í", "î", "ï", "ð", "ñ", "ò", "ó", "ô", "õ", "ö", "ø", "ù", "ú",
            "û", "ü", "ý", "þ", "ÿ", " ", "\n", "!", "“", "#", "$", "%", "&", "´", "(", ")",
            "*", "+", ",", "-", ".", "/", ":", ";", "<", "=", ">", "?", "@", "[", "{", "]",
            "|", "_", "}", "、", "˷", "…", "~", "£", "†", "‡", "^", "‰", "⟨", "`", "”", "•",
            "‒", "'", "—", "\"", "™", "⟩", " ", "˜", "¥", "╎", "§", "¡", "¢", "£", "¨", "©",
            "ª", "«", "¬", "–", "®", "°", "±", "²", "³", "‾", "ˢ", "µ", "¶", "→", "¹", "º",
            "»", "･", "¼", "½", "¾", "", "", "", "", "¿", "×", "÷", "💧", "★", "❤", "♪",
            "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",
            "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",
        };
    }

    public class AcString
    {
        public string String = "";
        private static SaveType _saveType;
        private static string[] _charDictionary;

        public AcString(byte[] stringBuffer, SaveType saveType)
        {
            _saveType = saveType;
            switch (saveType)
            {
                case SaveType.DoubutsuNoMori:
                case SaveType.DoubutsuNoMoriPlus:
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                    _charDictionary = StringUtility.DoubutsuNoMoriEPlusCharMap;
                    break;
                case SaveType.AnimalCrossing:
                    _charDictionary = StringUtility.AcCharacterDictionary;
                    break;
                case SaveType.WildWorld:
                    _charDictionary = StringUtility.WwCharacterDictionary;
                    break;
                default:
                    _charDictionary = null;
                    break;
            }

            if (_charDictionary != null)
            {
                foreach (var character in stringBuffer)
                {
                    String += _charDictionary[character];
                }
            }
            else
            {
                switch (saveType)
                {
                    case SaveType.CityFolk:
                        String = Encoding.BigEndianUnicode.GetString(stringBuffer);
                        break;
                    case SaveType.NewLeaf:
                    case SaveType.WelcomeAmiibo:
                        String = Encoding.Unicode.GetString(stringBuffer);
                        break;
                    default:
                        String = "";
                        break;
                }
            }
        }

        public static byte[] GetBytes(string String, int maxSize = 0)
        {
            var i = 0;
            switch (_saveType)
            {
                case SaveType.AnimalCrossing:
                    var returnedString = new byte[maxSize > 0 ? maxSize : String.Length];
                    var t = StringInfo.GetTextElementEnumerator(String);
                    while (t.MoveNext() && i < returnedString.Length)
                    {
                        var idx = Array.IndexOf(_charDictionary, (string) t.Current);
                        if (idx > -1)
                            returnedString[i] = (byte) idx;
                        else
                            returnedString[i] = Encoding.UTF8.GetBytes(new[] { String[t.ElementIndex] })[0];
                        i++;
                    }
                    for (var x = 0; x < returnedString.Length; x++)
                        if (returnedString[x] == 0)
                            returnedString[x] = 0x20;
                    return returnedString;
                case SaveType.DoubutsuNoMori:
                case SaveType.DoubutsuNoMoriPlus:
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                    {
                    var stringBytes = new byte[maxSize > 0 ? maxSize : String.Length];
                    for (i = 0; i < String.Length; i++)
                    {
                        if (i >= stringBytes.Length)
                            break;

                        var idx = Array.IndexOf(StringUtility.DoubutsuNoMoriEPlusCharMap, String[i].ToString());
                        if (idx > -1)
                        {
                            stringBytes[i] = (byte) idx;
                        }
                        else
                        {
                            stringBytes[i] = Encoding.ASCII.GetBytes(new[] { String[i] })[0];
                        }
                    }

                    if (maxSize <= 0 || String.Length >= maxSize) return stringBytes;
                    for (i = String.Length; i < maxSize; i++)
                    {
                        stringBytes[i] = 0x20;
                    }
                    return stringBytes;
                }
                case SaveType.WildWorld:
                {
                    var stringBuffer = new byte[maxSize > 0 ? maxSize : String.Length];
                    for (i = 0; i < String.Length; i++)
                    {
                        var idx = Array.IndexOf(StringUtility.WwCharacterDictionary, String[i].ToString());
                        if (idx > -1)
                        {
                            stringBuffer[i] = (byte) idx;
                        }
                        else
                        {
                            stringBuffer[i] = 0x85; // Space
                        }
                    }
                
                    return stringBuffer;
                }
                case SaveType.CityFolk:
                {
                    var stringBuffer = new byte[maxSize > 0 ? maxSize : String.Length * 2]; //Characters are now unicode
                    var stringBytes = Encoding.Unicode.GetBytes(String);
                    for (i = 0; i < stringBytes.Length; i += 2)
                        Buffer.BlockCopy(stringBytes.Skip(i).Take(2).Reverse().ToArray(), 0, stringBuffer, i, 2);
                    return stringBuffer;
                }
                case SaveType.NewLeaf:
                case SaveType.WelcomeAmiibo:
                {
                    var stringBuffer = Encoding.Unicode.GetBytes(String);
                    if (maxSize > 0)
                        Array.Resize(ref stringBuffer, maxSize);
                    return stringBuffer;
                }
                case SaveType.AnimalForest: // Animal Forest iQue support will be added soon
                    return null;
                case SaveType.Unknown:
                    MainForm.DebugManager.WriteLine(
                        $"StringUtil was passed an unknown SaveType enum. Received Type: {_saveType.ToString()}", DebugLevel.Error);
                    return null;
                default:
                    return null;
            }
        }

        public string Trim()
            => String.Trim(' ');

        public string Clean()
            => Regex.Replace(Regex.Replace(String, "[\n]{3,}", "\n\n").Trim(' '), "[ ]{2,}", " ");
    }
}