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
            { 0x00, "¡" },
            { 0x01, "¿" },
            { 0x02, "Ä" },
            { 0x03, "À" },
            { 0x04, "Á" },
            { 0x05, "Â" },
            { 0x06, "Ã" },
            { 0x07, "Å" },
            { 0x08, "Ç" },
            { 0x09, "È" },
            { 0x0A, "É" },
            { 0x0B, "Ê" },
            { 0x0C, "Ë" },
            { 0x0D, "Ì" },
            { 0x0E, "Í" },
            { 0x0F, "Î" },
            { 0x10, "Ï" },
            { 0x11, "Ð" },
            { 0x12, "Ñ" },
            { 0x13, "Ò" },
            { 0x14, "Ó" },
            { 0x15, "Ô" },
            { 0x16, "Õ" },
            { 0x17, "Ö" },
            { 0x18, "Ø" },
            { 0x19, "Ù" },
            { 0x1A, "Ú" },
            { 0x1B, "Û" },
            { 0x1C, "Ü" },
            { 0x1D, "ß" },
            { 0x1E, "Þ" },
            { 0x1F, "à" },
            { 0x20, " " },
            { 0x21, "!" },
            { 0x22, "\"" },
            { 0x23, "á" },
            { 0x24, "â" },
            { 0x25, "%" },
            { 0x26, "&" },
            { 0x27, "'" },
            { 0x28, "(" },
            { 0x29, ")" },
            { 0x2A, "~" },
            { 0x2B, "♥" },
            { 0x2C, "," },
            { 0x2D, "-" },
            { 0x2E, "." },
            { 0x2F, "♪" },
            { 0x30, "0" },
            { 0x31, "1" },
            { 0x32, "2" },
            { 0x33, "3" },
            { 0x34, "4" },
            { 0x35, "5" },
            { 0x36, "6" },
            { 0x37, "7" },
            { 0x38, "8" },
            { 0x39, "9" },
            { 0x3A, ":" },
            { 0x3B, "🌢" },
            { 0x3C, "<" },
            { 0x3D, "=" },
            { 0x3E, ">" },
            { 0x3F, "?" },
            { 0x40, "@" },
            { 0x41, "A" },
            { 0x42, "B" },
            { 0x43, "C" },
            { 0x44, "D" },
            { 0x45, "E" },
            { 0x46, "F" },
            { 0x47, "G" },
            { 0x48, "H" },
            { 0x49, "I" },
            { 0x4A, "J" },
            { 0x4B, "K" },
            { 0x4C, "L" },
            { 0x4D, "M" },
            { 0x4E, "N" },
            { 0x4F, "O" },
            { 0x50, "P" },
            { 0x51, "Q" },
            { 0x52, "R" },
            { 0x53, "S" },
            { 0x54, "T" },
            { 0x55, "U" },
            { 0x56, "V" },
            { 0x57, "W" },
            { 0x58, "X" },
            { 0x59, "Y" },
            { 0x5A, "Z" },
            { 0x5B, "ã" },
            { 0x5C, "💢" },
            { 0x5D, "ä" },
            { 0x5E, "å" },
            { 0x5F, "_" },
            { 0x60, "ç" },
            { 0x61, "a" },
            { 0x62, "b" },
            { 0x63, "c" },
            { 0x64, "d" },
            { 0x65, "e" },
            { 0x66, "f" },
            { 0x67, "g" },
            { 0x68, "h" },
            { 0x69, "i" },
            { 0x6A, "j" },
            { 0x6B, "k" },
            { 0x6C, "l" },
            { 0x6D, "m" },
            { 0x6E, "n" },
            { 0x6F, "o" },
            { 0x70, "p" },
            { 0x71, "q" },
            { 0x72, "r" },
            { 0x73, "s" },
            { 0x74, "t" },
            { 0x75, "u" },
            { 0x76, "v" },
            { 0x77, "w" },
            { 0x78, "x" },
            { 0x79, "y" },
            { 0x7A, "z" },
            { 0x7B, "è" },
            { 0x7C, "é" },
            { 0x7D, "ê" },
            { 0x7E, "ë" },
            { 0x7F, "□" },
            { 0x80, "�" },
            { 0x81, "ì" },
            { 0x82, "í" },
            { 0x83, "î" },
            { 0x84, "ï" },
            { 0x85, "•" },
            { 0x86, "ð" },
            { 0x87, "ñ" },
            { 0x88, "ò" },
            { 0x89, "ó" },
            { 0x8A, "ô" },
            { 0x8B, "õ" },
            { 0x8C, "ö" },
            { 0x8D, "⁰" },
            { 0x8E, "ù" },
            { 0x8F, "ú" },
            { 0x90, "–" },
            { 0x91, "û" },
            { 0x92, "ü" },
            { 0x93, "ý" },
            { 0x94, "ÿ" },
            { 0x95, "þ" },
            { 0x96, "Ý" },
            { 0x97, "¦" },
            { 0x98, "§" },
            { 0x99, "a̱" },
            { 0x9A, "o̱" },
            { 0x9B, "‖" },
            { 0x9C, "µ" },
            { 0x9D, "³" },
            { 0x9E, "²" },
            { 0x9F, "¹" },
            { 0xA0, "¯" },
            { 0xA1, "¬" },
            { 0xA2, "Æ" },
            { 0xA3, "æ" },
            { 0xA4, "„" },
            { 0xA5, "»" },
            { 0xA6, "«" },
            { 0xA7, "☀" },
            { 0xA8, "☁" },
            { 0xA9, "☂" },
            { 0xAA, "🌬" }, //Wind...
            { 0xAB, "☃" },
            { 0xAC, "∋" },
            { 0xAD, "∈" },
            { 0xAE, "/" },
            { 0xAF, "∞" },
            { 0xB0, "○" },
            { 0xB1, "🗙" },
            { 0xB2, "□" },
            { 0xB3, "△" },
            { 0xB4, "+" },
            { 0xB5, "⚡" },
            { 0xB6, "♂" },
            { 0xB7, "♀" },
            { 0xB8, "🍀"},
            { 0xB9, "★" },
            { 0xBA, "💀" },
            { 0xBB, "😮" },
            { 0xBC, "😄" },
            { 0xBD, "😣" },
            { 0xBE, "😠" },
            { 0xBF, "😃" },
            { 0xC0, "×" },
            { 0xC1, "÷" },
            { 0xC2, "🔨" }, //Hammer??
            { 0xC3, "🎀" }, //Not sure wtf this is (put it as ribbon)
            { 0xC4, "✉" },
            { 0xC5, "💰" },
            { 0xC6, "🐾" },
            { 0xC7, "🐶" },
            { 0xC8, "🐱" },
            { 0xC9, "🐰" },
            { 0xCA, "🐦" },
            { 0xCB, "🐮" },
            { 0xCC, "🐷" },
            { 0xCD, "\n" },
            { 0xCE, "🐟" },
            { 0xCF, "🐞" },
            { 0xD0, ";" },
            { 0xD1, "#" },
            //0xD2
            //0xD3
            { 0xD4, "⚷" },
            //0xD5
            //0xD6
            //0xD7
            //0xD8
            //0xD9
            //0xDA
            //0xDB
            //0xDC
            //0xDD
            //0xDE
            //0xDF
            //0xE0
            //0xE1
            //0xE2
            //0xE3
            //0xE4
            //0xE5
            //0xE6
            //0xE7
            //0xE8
            //0xE9
            //0xEA
            //0xEB
            //0xEC
            //0xED
            //0xEE
            //0xEF
            //0xF0
            //0xF1
            //0xF2
            //0xF3
            //0xF4
            //0xF5
            //0xF6
            //0xF7
            //0xF8
            //0xF9
            //0xFA
            //0xFB
            //0xFC
            //0xFD
            //0xFE
            //0xFF
        };

        public static Dictionary<byte, string> Doubutsu_no_Mori_e_Plus_Char_Map = new Dictionary<byte, string>
        {
            { 0x00, "あ" },
            { 0x01, "い" },
            { 0x02, "う" },
            { 0x03, "え" },
            { 0x04, "お" },
            { 0x05, "か" },
            { 0x06, "き" },
            { 0x07, "く" },
            { 0x08, "け" },
            { 0x09, "こ" },
            { 0x0A, "さ" },
            { 0x0B, "し" },
            { 0x0C, "す" },
            { 0x0D, "せ" },
            { 0x0E, "そ" },
            { 0x0F, "た" },
            { 0x10, "ち" },
            { 0x11, "つ" },
            { 0x12, "て" },
            { 0x13, "と" },
            { 0x14, "な" },
            { 0x15, "に" },
            { 0x16, "ぬ" },
            { 0x17, "ね" },
            { 0x18, "の" },
            { 0x19, "は" },
            { 0x1A, "ひ" },
            { 0x1B, "ふ" },
            { 0x1C, "へ" },
            { 0x1D, "ほ" },
            { 0x1E, "ま" },
            { 0x1F, "み" },
            { 0x20, " " },
            { 0x21, "!" },
            { 0x22, "\"" },
            { 0x23, "む" },
            { 0x24, "め" },
            { 0x25, "%" },
            { 0x26, "&" },
            { 0x27, "'" },
            { 0x28, "(" },
            { 0x29, ")" },
            { 0x2A, "~" },
            { 0x2B, "♥" },
            { 0x2C, "," },
            { 0x2D, "-" },
            { 0x2E, "." },
            { 0x2F, "♪" },
            { 0x30, "0" },
            { 0x31, "1" },
            { 0x32, "2" },
            { 0x33, "3" },
            { 0x34, "4" },
            { 0x35, "5" },
            { 0x36, "6" },
            { 0x37, "7" },
            { 0x38, "8" },
            { 0x39, "9" },
            { 0x3A, ":" },
            { 0x3B, ";" }, // Actually is this: 🌢 (it's double width, I need to write a better text utility)
            { 0x3C, "<" },
            { 0x3D, "+" },
            { 0x3E, ">" },
            { 0x3F, "?" },
            { 0x40, "@" },
            { 0x41, "A" },
            { 0x42, "B" },
            { 0x43, "C" },
            { 0x44, "D" },
            { 0x45, "E" },
            { 0x46, "F" },
            { 0x47, "G" },
            { 0x48, "H" },
            { 0x49, "I" },
            { 0x4A, "J" },
            { 0x4B, "K" },
            { 0x4C, "L" },
            { 0x4D, "M" },
            { 0x4E, "N" },
            { 0x4F, "O" },
            { 0x50, "P" },
            { 0x51, "Q" },
            { 0x52, "R" },
            { 0x53, "S" },
            { 0x54, "T" },
            { 0x55, "U" },
            { 0x56, "V" },
            { 0x57, "W" },
            { 0x58, "X" },
            { 0x59, "Y" },
            { 0x5A, "Z" },
            { 0x5B, "も" },
            { 0x5C, "\\" }, // Angry symbol (or annoyed)
            { 0x5D, "や" },
            { 0x5E, "ゆ" },
            { 0x5F, "_" },
            { 0x60, "よ" },
            { 0x61, "a" },
            { 0x62, "b" },
            { 0x63, "c" },
            { 0x64, "d" },
            { 0x65, "e" },
            { 0x66, "f" },
            { 0x67, "g" },
            { 0x68, "h" },
            { 0x69, "i" },
            { 0x6A, "j" },
            { 0x6B, "k" },
            { 0x6C, "l" },
            { 0x6D, "m" },
            { 0x6E, "n" },
            { 0x6F, "o" },
            { 0x70, "p" },
            { 0x71, "q" },
            { 0x72, "r" },
            { 0x73, "s" },
            { 0x74, "t" },
            { 0x75, "u" },
            { 0x76, "v" },
            { 0x77, "w" },
            { 0x78, "x" },
            { 0x79, "y" },
            { 0x7A, "z" },
            { 0x7B, "ら" },
            { 0x7C, "り" },
            { 0x7D, "る" },
            { 0x7E, "れ" },
            { 0x7F, "�" }, // Cont Character
            { 0x80, "□" }, // Tag Character
            { 0x81, "。" },
            { 0x82, "｢" },
            { 0x83, "｣" },
            { 0x84, "、" },
            { 0x85, "･" },
            { 0x86, "ヲ" },
            { 0x87, "ァ" },
            { 0x88, "ィ" },
            { 0x89, "ゥ" },
            { 0x8A, "ェ" },
            { 0x8B, "ォ" },
            { 0x8C, "ャ" },
            { 0x8D, "ュ" },
            { 0x8E, "ョ" },
            { 0x8F, "ッ" },
            { 0x90, "ー" },
            { 0x91, "ア" },
            { 0x92, "イ" },
            { 0x93, "ウ" },
            { 0x94, "エ" },
            { 0x95, "オ" },
            { 0x96, "カ" },
            { 0x97, "キ" },
            { 0x98, "ク" },
            { 0x99, "ケ" },
            { 0x9A, "コ" },
            { 0x9B, "サ" },
            { 0x9C, "シ" },
            { 0x9D, "ス" },
            { 0x9E, "セ" },
            { 0x9F, "ソ" },
            { 0xA0, "タ" },
            { 0xA1, "チ" },
            { 0xA2, "ツ" },
            { 0xA3, "テ" },
            { 0xA4, "ト" },
            { 0xA5, "ナ" },
            { 0xA6, "ニ" },
            { 0xA7, "ヌ" },
            { 0xA8, "ネ" },
            { 0xA9, "ノ" },
            { 0xAA, "ハ" },
            { 0xAB, "ヒ" },
            { 0xAC, "フ" },
            { 0xAD, "ヘ" },
            { 0xAE, "ホ" },
            { 0xAF, "マ" },
            { 0xB0, "ミ" },
            { 0xB1, "ム" },
            { 0xB2, "メ" },
            { 0xB3, "モ" },
            { 0xB4, "ヤ" },
            { 0xB5, "ユ" },
            { 0xB6, "ヨ" },
            { 0xB7, "ラ" },
            { 0xB8, "リ" },
            { 0xB9, "ル" },
            { 0xBA, "レ" },
            { 0xBB, "ロ" },
            { 0xBC, "ワ" },
            { 0xBD, "ン" },
            { 0xBE, "ヴ" },
            { 0xBF, "☺" },
            { 0xC0, "ろ" },
            { 0xC1, "わ" },
            { 0xC2, "を" },
            { 0xC3, "ん" },
            { 0xC4, "ぁ" },
            { 0xC5, "ぃ" },
            { 0xC6, "ぅ" },
            { 0xC7, "ぇ" },
            { 0xC8, "ぉ" },
            { 0xC9, "ゃ" },
            { 0xCA, "ゅ" },
            { 0xCB, "ょ" },
            { 0xCC, "っ" },
            { 0xCD, "\n" },
            { 0xCE, "ガ" },
            { 0xCF, "ギ" },
            { 0xD0, "グ" },
            { 0xD1, "ゲ" },
            { 0xD2, "ゴ" },
            { 0xD3, "ザ" },
            { 0xD4, "ジ" },
            { 0xD5, "ズ" },
            { 0xD6, "ゼ" },
            { 0xD7, "ゾ" },
            { 0xD8, "ダ" },
            { 0xD9, "ヂ" },
            { 0xDA, "ヅ" },
            { 0xDB, "デ" },
            { 0xDC, "ド" },
            { 0xDD, "バ" },
            { 0xDE, "ビ" },
            { 0xDF, "ブ" },
            { 0xE0, "ベ" },
            { 0xE1, "ボ" },
            { 0xE2, "パ" },
            { 0xE3, "ピ" },
            { 0xE4, "プ" },
            { 0xE5, "ペ" },
            { 0xE6, "ポ" },
            { 0xE7, "が" },
            { 0xE8, "ぎ" },
            { 0xE9, "ぐ" },
            { 0xEA, "げ" },
            { 0xEB, "ご" },
            { 0xEC, "ざ" },
            { 0xED, "じ" },
            { 0xEE, "ず" },
            { 0xEF, "ぜ" },
            { 0xF0, "ぞ" },
            { 0xF1, "だ" },
            { 0xF2, "ぢ" },
            { 0xF3, "づ" },
            { 0xF4, "で" },
            { 0xF5, "ど" },
            { 0xF6, "ば" },
            { 0xF7, "び" },
            { 0xF8, "ぶ" },
            { 0xF9, "べ" },
            { 0xFA, "ぼ" },
            { 0xFB, "ぱ" },
            { 0xFC, "ぴ" },
            { 0xFD, "ぷ" },
            { 0xFE, "ぺ" },
            { 0xFF, "ぽ" },
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
        static SaveType Save_Type;
        public string String = "";
        static Dictionary<byte, string> Char_Dictionary;

        public ACString(byte[] stringBuffer, SaveType saveType)
        {
            Save_Type = saveType;
            String_Bytes = stringBuffer;
            switch (saveType)
            {
                case SaveType.Doubutsu_no_Mori_Plus:
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    Char_Dictionary = StringUtil.Doubutsu_no_Mori_e_Plus_Char_Map;
                    break;
                case SaveType.Animal_Crossing:
                    Char_Dictionary = StringUtil.AC_CharacterDictionary;
                    break;
                case SaveType.Wild_World:
                    Char_Dictionary = StringUtil.WW_CharacterDictionary;
                    break;
                default:
                    Char_Dictionary = null;
                    break;
            }
            if ((saveType == SaveType.Doubutsu_no_Mori_Plus || saveType == SaveType.Animal_Crossing || saveType == SaveType.Doubutsu_no_Mori_e_Plus
                || saveType == SaveType.Wild_World) && Char_Dictionary != null)
                foreach (byte b in stringBuffer)
                    if (Char_Dictionary.ContainsKey(b))
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
            else if (Save_Type == SaveType.Doubutsu_no_Mori_Plus || Save_Type == SaveType.Doubutsu_no_Mori_e_Plus)
            {
                byte[] String_Bytes = new byte[maxSize > 0 ? maxSize : String.Length];
                for (int i = 0; i < String.Length; i++)
                {
                    if (i >= String_Bytes.Length)
                        break;
                    if (StringUtil.Doubutsu_no_Mori_e_Plus_Char_Map.ContainsValue(String[i].ToString()))
                    {
                        String_Bytes[i] = StringUtil.Doubutsu_no_Mori_e_Plus_Char_Map.First(o => o.Value == String[i].ToString()).Key;
                    }
                    else
                    {
                        String_Bytes[i] = Encoding.ASCII.GetBytes(new char[1] { String[i] })[0];
                    }
                }
                if (maxSize > 0 && String.Length < maxSize)
                {
                    for (int i = String.Length; i < maxSize; i++)
                    {
                        String_Bytes[i] = 0x20;
                    }
                }
                return String_Bytes;
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
                for (int i = 0; i < String_Bytes.Length; i += 2)
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