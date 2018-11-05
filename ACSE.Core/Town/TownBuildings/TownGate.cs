using System.Drawing;
using System.IO;
using ACSE.Core.Saves;
using ACSE.Core.Utilities;

namespace ACSE.Core.Town.TownBuildings
{
    /// <summary>
    /// Handles Town Gate related operations.
    /// </summary>
    public static class TownGate
    {
        /// <summary>
        /// Wild World Town Gate descriptions.
        /// </summary>
        private static readonly string[] WildWorldGateNames = {"Red Roof", "Green Roof", "Blue Roof"};

        /// <summary>
        /// City Folk Town Gate descriptions.
        /// </summary>
        private static readonly string[] CityFolkGateNames = {"Stone (Blue)", "Wood (Green)", "Brick (Pink)"};

        /// <summary>
        /// Gets an array of the Town Gate descriptions for a <see cref="SaveGeneration"/>.
        /// </summary>
        /// <param name="saveGeneration">The <see cref="SaveGeneration"/> of the current <see cref="Save"/>.</param>
        /// <returns>An array containing the descriptions of the Town Gate types OR null if there are none.</returns>
        public static string[] GetTownGateTypeNames(SaveGeneration saveGeneration)
        {
            switch (saveGeneration)
            {
                case SaveGeneration.NDS:
                    return WildWorldGateNames;
                case SaveGeneration.Wii:
                    return CityFolkGateNames;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the current Town Gate type.
        /// </summary>
        /// <param name="save">The <see cref="Save"/> file.</param>
        /// <returns>The current Town Gate type OR -1 if it doesn't exist for the <see cref="SaveGeneration"/>.</returns>
        public static int GetTownGateType(Save save)
        {
            switch (save.SaveGeneration)
            {
                case SaveGeneration.NDS:
                    return save.ReadByte(0x15B58, true);
                case SaveGeneration.Wii:
                    return save.ReadByte(0x5EAE0, true);
                default:
                    return -1;
            }
        }

        /// <summary>
        /// Sets the <see cref="Save"/>'s Town Gate type.
        /// </summary>
        /// <param name="save">The <see cref="Save"/> file.</param>
        /// <param name="type">The Town Gate type.</param>
        public static void SetTownGateType(Save save, int type)
        {
            switch (save.SaveGeneration)
            {
                case SaveGeneration.NDS:
                    save.Write(0x15B58, (byte) type, true);
                    break;
                case SaveGeneration.Wii:
                    save.Write(0x5EAE0, (byte)type, true);
                    break;
            }
        }

        /// <summary>
        /// Gets an <see cref="Image"/> icon of the current Town Gate.
        /// </summary>
        /// <param name="saveGeneration">The <see cref="SaveGeneration"/> of the current <see cref="Save"/>.</param>
        /// <param name="gateType">The gate type whose image will be retrieved.</param>
        /// <returns><see cref="Image"/> TownGateIcon OR null if the image does not exist or cannot be accessed.</returns>
        public static Image GetTownGateImage(SaveGeneration saveGeneration, int gateType)
        {
            string path;

            switch (saveGeneration)
            {
                case SaveGeneration.NDS:
                    path = Path.Combine(PathUtility.GetIconsDirectory(), "Wild World", $"{(uint)gateType}.png");
                    break;
                case SaveGeneration.Wii:
                    path = Path.Combine(PathUtility.GetIconsDirectory(), "City Folk", $"{(uint) gateType}.png");
                    break;
                default:
                    return null;
            }

            if (!File.Exists(path)) return null;

            try
            {
                return Image.FromFile(path);
            }
            catch
            {
                return null;
            }
        }
    }
}
