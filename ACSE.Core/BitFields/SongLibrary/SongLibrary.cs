using System.Collections.Generic;
using ACSE.Core.Players;
using ACSE.Core.Saves;

namespace ACSE.Core.BitFields.SongLibrary
{
    public static class SongLibrary
    {
        public static readonly Dictionary<int, byte> AnimalCrossingSongLibraryBitMap = new Dictionary<int, byte>
        {
            { 0x2684, 0xFF },
            { 0x2685, 0xFF },
            { 0x2686, 0xFF },
            { 0x2687, 0xFF },
            { 0x2689, 0x7F },
            { 0x268A, 0xFF },
            { 0x268B, 0xFF },
        };

        public static readonly Dictionary<int, byte> WildWorldSongLibraryBitMap = new Dictionary<int, byte>
        {
            { 0xFAE0, 0xFF },
            { 0xFAE1, 0xFF },
            { 0xFAE2, 0xFF },
            { 0xFAE3, 0xFF },
            { 0xFAE4, 0xFF },
            { 0xFAE5, 0xFF },
            { 0xFAE6, 0xFF },
            { 0xFAE7, 0xFF },
            { 0xFAE8, 0x3F },
        };

        public static readonly Dictionary<int, byte> NewLeafSongLibraryBitMap = new Dictionary<int, byte>
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

        public static readonly Dictionary<int, byte> WelcomeAmiiboSongLibraryBitMap = new Dictionary<int, byte>
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

        public static readonly string[][] AnimalCrossingSongLibraryNames = {
            new[] { "K.K. Safari", "K.K. Folk", "K.K. Rock", "Rockin' K.K.", "K.K. Ragtime", "K.K. Gumbo", "The K. Funk", "K.K. Blues" },
            new[] { "K.K. Tango", "K.K. Faire", "Aloha K.K.", "Lucky K.K.", "K.K. Condor", "K.K. Steppe", "Imperial K.K.", "K.K. Casbah" },
            new[] { "K.K. Aria", "K.K. Samba", "K.K. Bossa", "K.K. Calypso", "K.K. Salsa", "K.K. Mambo", "K.K. Reggae", "K.K. Ska" },
            new[] { "K.K. Chorale", "K.K. March", "K.K. Waltz", "K.K. Swing", "K.K. Jazz", "K.K. Fusion", "K.K. Etude", "K.K. Lullaby" },
            new[] { "Mr. K.K.", "Café K.K.", "K.K. Parade", "Señor K.K.", "K.K. Song", "I Love You", "Two Days Ago", "" },
            new[] { "K.K. Country", "Surfin' K.K.", "K.K. Ballad", "Comrade K.K.", "K.K. Lament", "Go K.K. Rider!", "K.K. Dirge", "K.K. Western" },
            new[] { "Soulful K.K.", "K.K. Soul", "K.K. Crusin'", "K.K. Love Song", "K.K. D & B", "K.K. Technopop", "DJ K.K.", "Only Me" }
        };

        public static readonly string[][] WildWorldSongLibraryNames = {
            new[] { "K.K. Chorale", "K.K. March", "K.K. Waltz", "K.K. Swing", "K.K. Jazz", "K.K. Fusion", "K.K. Etude", "K.K. Lullaby" },
            new[] { "K.K. Aria", "K.K. Samba", "K.K. Bossa", "K.K. Calypso", "K.K. Salsa", "K.K. Mambo", "K.K. Reggae", "K.K. Ska" },
            new[] { "K.K. Tango", "K.K. Faire", "Aloha K.K.", "Lucky K.K.", "K.K. Condor", "K.K. Steppe", "Imperial K.K.", "K.K. Casbah" },
            new[] { "K.K. Safari", "K.K. Folk", "K.K. Rock", "Rockin' K.K.", "K.K. Ragtime", "K.K. Gumbo", "The K. Funk", "K.K. Blues" },
            new[] { "Soulful K.K.", "K.K. Soul", "K.K. Cruisin'", "K.K. Love Song", "K.K. D & B", "K.K. Technopop", "DJ K.K.", "Only Me" },
            new[] { "K.K. Country", "Surfin' K.K.", "K.K. Ballad", "Comrade K.K.", "K.K. Lament", "Go K.K. Rider!", "K.K. Dirge", "K.K. Western" },
            new[] { "Mr. K.K.", "Café K.K.", "K.K. Parade", "Señor K.K.", "K.K. Song", "I Love You", "Two Days Ago", "My Place" },
            new[] { "Forest Life", "To the Edge", "Pondering", "K.K. Dixie", "K.K. Marathon", "King K.K.", "Mountain Song", "Marine Song 2001" },
            new[] { "Neapolitan", "Steep Hill", "K.K. Rockabilly", "Agent K.K.", "K.K. Rally", "K.K. Metal", "", "" }
        };

        public static readonly string[][] NewLeafSongLibraryNames = {
            new[] { "K.K. Chorale", "K.K. March", "K.K. Waltz", "K.K. Swing", "K.K. Jazz", "K.K. Fusion", "K.K. Étude", "K.K. Lullaby" },
            new[] { "K.K. Aria", "K.K. Samba", "K.K. Bossa", "K.K. Calypso", "K.K. Salsa", "K.K. Mambo", "K.K. Reggae", "K.K. Ska" },
            new[] { "K.K. Tango", "K.K. Faire", "Aloha K.K.", "Lucky K.K.", "K.K. Condor", "K.K. Steppe", "Imperial K.K.", "K.K. Casbah" },
            new[] { "K.K. Safari", "K.K. Folk", "K.K. Rock", "Rockin' K.K.", "K.K. Ragtime", "K.K. Gumbo", "The K. Funk", "K.K. Blues" },
            new[] { "Soulful K.K.'", "K.K. Soul", "K.K. Cruisin'", "K.K. Love Song", "K.K. D&B", "K.K. Technopop", "DJ K.K.", "Only Me" },
            new[] { "K.K. Country", "Surfin' K.K.", "K.K. Ballad", "Comrade K.K.", "K.K. Lament", "Go K.K. Rider", "K.K. Dirge", "K.K. Western" },
            new[] { "Mr. K.K.", "Café K.K.", "K.K. Parade", "Señor K.K.", "K.K. Song", "I Love You", "Two Days Ago", "My Place" },
            new[] { "Forest Life", "To The Edge", "Pondering", "K.K. Dixie", "K.K. Marathon", "King K.K.", "Mountain Song", "Marine Song 2001" },
            new[] { "Neapolitan", "Steep Hill", "K.K. Rockabilly", "Agent K.K.", "K.K. Rally", "K.K. Metal", "Stale Cupcakes", "Spring Blossoms" },
            new[] { "Wandering", "K.K. House", "K.K. Sonata", "Hypno K.K.", "K.K. Stroll", "K.K. Island", "Space K.K.", "K.K. Adventure" },
            new[] { "K.K. Oasis", "K.K. Bazaar", "K.K. Milonga", "K.K. Groove", "K.K. Jongara", "K.K. Flamenco", "K.K. Moody", "Bubblegum K.K." },
            new[] { "K.K. Synth", "K.K. Disco", "K.K. Birthday", "", "", "", "", "" }
        };

        private static Dictionary<int, byte> GetBitMap(SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.AnimalCrossing:
                    return AnimalCrossingSongLibraryBitMap;
                case SaveType.WildWorld:
                    return WildWorldSongLibraryBitMap;
                case SaveType.NewLeaf:
                    return NewLeafSongLibraryBitMap;
                case SaveType.WelcomeAmiibo:
                    return WelcomeAmiiboSongLibraryBitMap;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Clears a Player's Song Library.
        /// </summary>
        /// <param name="saveFile">Current Save File</param>
        /// <param name="player">Current Player whose Song Library will be cleared</param>
        public static void ClearSongLibrary(Save saveFile, Player player)
        {
            var currentBitMap = GetBitMap(saveFile.SaveType);
            if (currentBitMap == null) return;
            if (saveFile.SaveGeneration == SaveGeneration.N64 || saveFile.SaveGeneration == SaveGeneration.GCN)
            {
                if (player.House == null) return;
                foreach (var bitValue in currentBitMap)
                {
                    saveFile.Write(player.House.Offset + bitValue.Key, (byte)(saveFile.ReadByte(player.House.Offset + bitValue.Key) & ~bitValue.Value));
                }
            }
            else
            {
                foreach (var bitValue in currentBitMap)
                {
                    saveFile.Write(player.Offset + bitValue.Key, (byte)(saveFile.ReadByte(player.Offset + bitValue.Key) & ~bitValue.Value));
                }
            }
        }

        /// <summary>
        /// Fills a Player's Song Library.
        /// </summary>
        /// <param name="saveFile">Current Save File</param>
        /// <param name="player">Current Player whose Song Library will be filled</param>
        public static void FillSongLibrary(Save saveFile, Player player)
        {
            var currentBitMap = GetBitMap(saveFile.SaveType);
            if (currentBitMap == null) return;
            if (saveFile.SaveGeneration == SaveGeneration.N64 || saveFile.SaveGeneration == SaveGeneration.GCN)
            {
                if (player.House == null) return;
                foreach (var bitValue in currentBitMap)
                {
                    saveFile.Write(player.House.Offset + bitValue.Key, (byte)(saveFile.ReadByte(player.House.Offset + bitValue.Key) | bitValue.Value));
                }
            }
            else
            {
                foreach (var bitValue in currentBitMap)
                {
                    saveFile.Write(player.Offset + bitValue.Key, (byte)(saveFile.ReadByte(player.Offset + bitValue.Key) | bitValue.Value));
                }
            }
        }
    }
}
