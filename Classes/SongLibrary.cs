using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACSE
{
    class SongLibrary
    {
        public static Dictionary<int, byte> Welcome_Amiibo_KKSongs_Bit_Map = new Dictionary<int, byte>
        {
            { 0x8F9C, 0xFF },
            { 0x8F9D, 0xFF },
            { 0x8F9E, 0xFF },
            { 0x8F9F, 0xFF },
            { 0x8FA0, 0xFF },
            { 0x8FA1, 0xFF },
            { 0x8FA2, 0xFF },
            { 0x8FA3, 0xFF },
            { 0x8FA4, 0xFF },
            { 0x8FA5, 0xFF },
            { 0x8FA6, 0xFF },
            { 0x8FA7, 0x07 }
        };

        public static Dictionary<int, byte> New_Leaf_KKSongs_Bit_Map = new Dictionary<int, byte>
        {
            { 0x8C54, 0xFF },
            { 0x8C55, 0xFF },
            { 0x8C56, 0xFF },
            { 0x8C57, 0xFF },
            { 0x8C58, 0xFF },
            { 0x8C59, 0xFF },
            { 0x8C5A, 0xFF },
            { 0x8FAB, 0xFF },
            { 0x8FAC, 0xFF },
            { 0x8FAD, 0xFF },
            { 0x8FAE, 0xFF },
            { 0x8FAF, 0x07 }
        };

        public static string[][] New_Leaf_KKSongs_Names = new string[12][]
        {
            new string[8] { "K.K. Chorale", "K.K. March", "K.K. Waltz", "K.K. Swing", "K.K. Jazz", "K.K. Fusion", "K.K. Étude", "K.K. Lullaby" },
            new string[8] { "K.K. Aria", "K.K. Samba", "K.K. Bossa", "K.K. Calypso", "K.K. Salsa", "K.K. Mambo", "K.K. Reggae", "K.K. Ska" },
            new string[8] { "K.K. Tango", "K.K. Faire", "Aloha K.K.", "Lucky K.K.", "K.K. Condor", "K.K. Steppe", "Imperial K.K.", "K.K. Casbah" },
            new string[8] { "K.K. Safari", "K.K. Folk", "K.K. Rock", "Rockin' K.K.", "K.K. Ragtime", "K.K. Gumbo", "The K. Funk", "K.K. Blues" },
            new string[8] { "Soulful K.K.'", "K.K. Soul", "K.K. Cruisin'", "K.K. Love Song", "K.K. D&B", "K.K. Technopop", "DJ K.K.", "Only Me" },
            new string[8] { "K.K. Country", "Surfin' K.K.", "K.K. Ballad", "Comrade K.K.", "K.K. Lament", "Go K.K. Rider", "K.K. Dirge", "K.K. Western" },
            new string[8] { "Mr. K.K.", "Café K.K.", "K.K. Parade", "Señor K.K.", "K.K. Song", "I Love You", "Two Days Ago", "My Place" },
            new string[8] { "Forest Life", "To The Edge", "Pondering", "K.K. Dixie", "K.K. Marathon", "King K.K.", "Mountain Song", "Marine Song 2001" },
            new string[8] { "Neapolitan", "Steep Hill", "K.K. Rockabilly", "Agent K.K.", "K.K. Rally", "K.K. Metal", "Stale Cupcakes", "Spring Blossoms" },
            new string[8] { "Wandering", "K.K. House", "K.K. Sonata", "Hypno K.K.", "K.K. Stroll", "K.K. Island", "Space K.K.", "K.K. Adventure" },
            new string[8] { "K.K. Oasis", "K.K. Bazaar", "K.K. Milonga", "K.K. Groove", "K.K. Jongara", "K.K. Flamenco", "K.K. Moody", "Bubblegum K.K." },
            new string[8] { "K.K. Synth", "K.K. Disco", "K.K. Birthday", "", "", "", "", "" }
        };

        private static Dictionary<int, byte> GetBitMap(SaveType Save_Type)
        {
            switch (Save_Type)
            {
                case SaveType.New_Leaf:
                    return New_Leaf_KKSongs_Bit_Map;
                case SaveType.Welcome_Amiibo:
                    return Welcome_Amiibo_KKSongs_Bit_Map;
                default:
                    throw new NotImplementedException(string.Format("K.K. Songs Bit Map for save type {0} has not been implemented!", Save_Type.ToString()));
            }
        }

        public static void FillKKSongs(Save Save_File, NewPlayer Player)
        {
            Dictionary<int, byte> Current_Bit_Map = GetBitMap(Save_File.Save_Type);
            foreach (KeyValuePair<int, byte> Bit_Value in Current_Bit_Map)
                Save_File.Write(Player.Offset + Bit_Value.Key, (byte)(Save_File.ReadByte(Player.Offset + Bit_Value.Key) | Bit_Value.Value));
        }
    }
}
