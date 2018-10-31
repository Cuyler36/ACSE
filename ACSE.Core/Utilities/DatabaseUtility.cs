using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ACSE.Core.Utilities
{
    public static class DatabaseUtility
    {
        public enum DatabaseReturnCode
        {
            FailedToRead = -2,
            FailedToOpen = -1,
            Success = 0
        }

        /// <summary>
        /// Loads a specified database with the key type as bytes.
        /// </summary>
        /// <param name="databasePath">The path to the database to load.</param>
        /// <returns>The result of the method <see cref="DatabaseReturnCode"/>, and the loaded database if execution was successful.</returns>
        public static (DatabaseReturnCode, Dictionary<byte, string>) LoadDatabaseByte(string databasePath)
        {
            StreamReader databaseReader;
            try
            {
                databaseReader = File.OpenText(databasePath);
            }
            catch
            {
                return (DatabaseReturnCode.FailedToOpen, null);
            }

            try
            {
                using (databaseReader)
                {
                    var database = new Dictionary<byte, string>();
                    string line;

                    while ((line = databaseReader.ReadLine()) != null)
                    {
                        if (line.Contains("//")) continue;

                        database.Add(byte.Parse(line.Substring(0, 4).Replace("0x", ""), NumberStyles.HexNumber),
                            line.Substring(5));
                    }

                    return (DatabaseReturnCode.Success, database);
                }
            }
            catch
            {
                return (DatabaseReturnCode.FailedToRead, null);
            }
        }

        /// <summary>
        /// Loads a specified database with the key type as ushorts.
        /// </summary>
        /// <param name="databasePath">The path to the database to load.</param>
        /// <returns>The result of the method <see cref="DatabaseReturnCode"/>, and the loaded database if execution was successful.</returns>
        public static (DatabaseReturnCode, Dictionary<ushort, string>) LoadDatabase(string databasePath)
        {
            StreamReader databaseReader;
            try
            {
                databaseReader = File.OpenText(databasePath);
            }
            catch
            {
                return (DatabaseReturnCode.FailedToOpen, null);
            }

            try
            {
                using (databaseReader)
                {
                    var database = new Dictionary<ushort, string>();
                    string line;

                    while ((line = databaseReader.ReadLine()) != null)
                    {
                        if (line.Contains("//")) continue;

                        database.Add(ushort.Parse(line.Substring(0, 6).Replace("0x", ""), NumberStyles.HexNumber),
                            line.Substring(7));
                    }

                    return (DatabaseReturnCode.Success, database);
                }
            }
            catch
            {
                return (DatabaseReturnCode.FailedToRead, null);
            }
        }
    }
}
