using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using ACSE.Core.Saves;
using ACSE.Core.Utilities;
using ACSE.WinForms.Controls;

namespace ACSE.WinForms.Managers
{
    public static class AcreImageManager
    {
        private static Dictionary<ushort, Image> _loadedImageDictionary = new Dictionary<ushort, Image>();
        private static Dictionary<byte, Image> _loadedAnimalCrossingMapIcons = new Dictionary<byte, Image>();

        /*
         * Doubutsu no Mori - Doubutsu no Mori e+ Acre Info
         * 
         * Each acre consists of 4 height levels starting at the lowest height.
         * Example:
             * 0x0278 = Empty Acre (Lower) (4)
             * 0x0279 = Empty Acre (Middle) (4)
             * 0x027A = Empty Acre (Upper) (4)
             * 0x027B = Empty Acre (Uppermost) (4)
         */

        public static string[] AcreHeightIdentifiers = {
            "Lower", "Middle", "Upper", "Uppermost"
        };

        public static Image FetchAcreImage(SaveType saveType, ushort acreId)
        {
            if (_loadedImageDictionary.ContainsKey(acreId))
                return _loadedImageDictionary[acreId];

            Image result = null;

            var imageDir = Path.Combine(PathUtility.GetResourcesDirectory(), "Images");
            switch (saveType)
            {
                case SaveType.DoubutsuNoMori:
                case SaveType.AnimalCrossing:
                case SaveType.DoubutsuNoMoriPlus:
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                // TODO: DnM needs its own set?
                case SaveType.DongwuSenlin:
                    imageDir = Path.Combine(imageDir, "Acre_Images");
                    break;
                case SaveType.WildWorld:
                    imageDir = Path.Combine(imageDir, "WW_Acre_Images");
                    break;
                case SaveType.CityFolk:
                    imageDir = Path.Combine(imageDir, "CF_Acre_Images");
                    break;
                case SaveType.NewLeaf:
                    imageDir = Path.Combine(imageDir, "NL_Acre_Images");
                    break;
                case SaveType.WelcomeAmiibo:
                    imageDir = Path.Combine(imageDir, "WA_Acre_Images");
                    break;
                default:
                    return null;
            }

            if (Directory.Exists(imageDir))
            {
                foreach (var file in Directory.GetFiles(imageDir))
                {
                    var extension = Path.GetExtension(file);
                    if (!extension.Equals(".png") && !extension.Equals(".jpg")) continue;
                    if (saveType == SaveType.NewLeaf || saveType == SaveType.WelcomeAmiibo)
                    {
                        if (!ushort.TryParse(Path.GetFileNameWithoutExtension(file).Replace("acre_", ""),
                                out var fileAcreId) || fileAcreId != acreId) continue;
                        try
                        {
                            result = Image.FromFile(file);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                    else
                    {
                        if (!ushort.TryParse(Path.GetFileNameWithoutExtension(file), NumberStyles.HexNumber, null,
                                out var fileAcreId) || fileAcreId != acreId) continue;
                        try
                        {
                            result = Image.FromFile(file);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
            }

            if (result != null)
                _loadedImageDictionary.Add(acreId, result);

            return result;
        }

        public static Image FetchAcMapIcon(byte index)
        {
            if (_loadedAnimalCrossingMapIcons.ContainsKey(index))
                return _loadedAnimalCrossingMapIcons[index];

            Image result = null;

            var iconDirectory = Path.Combine(PathUtility.GetResourcesDirectory(), "Images", "AC_Map_Icons");
            if (Directory.Exists(iconDirectory))
            {
                foreach (var iconFile in Directory.GetFiles(iconDirectory))
                {
                    if (!byte.TryParse(Path.GetFileNameWithoutExtension(iconFile), out var fileIndex) ||
                        fileIndex != index) continue;
                    try
                    {
                        result = Image.FromFile(iconFile);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            if (result != null)
                _loadedAnimalCrossingMapIcons.Add(index, result);

            return result;
        }

        public static void CheckReferencesAndDispose(Image referencedImage,
            IList<PictureBoxWithInterpolationMode> pictureBoxes, PictureBoxWithInterpolationMode selectedAcreBox)
        {
            if (referencedImage == null || selectedAcreBox.Image == referencedImage
                                        || pictureBoxes.Any(
                                            box => box != null && box.BackgroundImage == referencedImage)) return;

            foreach (var pair in _loadedImageDictionary)
                if (pair.Value == referencedImage)
                {
                    _loadedImageDictionary.Remove(pair.Key);
                    break;
                }

            foreach (var pair in _loadedAnimalCrossingMapIcons)
                if (pair.Value == referencedImage)
                {
                    _loadedAnimalCrossingMapIcons.Remove(pair.Key);
                    break;
                }

            referencedImage.Dispose();
        }

        public static void CheckReferencesAndDispose(Image referencedImage,
            IList<AcreItemEditor> itemEditors, PictureBoxWithInterpolationMode selectedAcreBox)
        {
            if (referencedImage == null || selectedAcreBox.Image == referencedImage
                                        || itemEditors.Any(
                                            box => box != null && box.BackgroundImage == referencedImage)) return;

            foreach (var pair in _loadedImageDictionary)
                if (pair.Value == referencedImage)
                {
                    _loadedImageDictionary.Remove(pair.Key);
                    break;
                }

            foreach (var pair in _loadedAnimalCrossingMapIcons)
                if (pair.Value == referencedImage)
                {
                    _loadedAnimalCrossingMapIcons.Remove(pair.Key);
                    break;
                }

            referencedImage.Dispose();
        }

        public static void DisposeLoadedImages()
        {
            foreach (var i in _loadedImageDictionary.Values)
            {
                i.Dispose();
            }

            foreach (var i in _loadedAnimalCrossingMapIcons.Values)
            {
                i.Dispose();
            }

            _loadedImageDictionary = new Dictionary<ushort, Image>();
            _loadedAnimalCrossingMapIcons = new Dictionary<byte, Image>();
        }

        public static Dictionary<ushort, byte> LoadAcMapIndex(SaveType saveType)
        {
            var indexFile = PathUtility.GetResourcesDirectory();
            switch (saveType)
            {
                case SaveType.DoubutsuNoMori:
                case SaveType.DoubutsuNoMoriPlus:
                case SaveType.DongwuSenlin:
                    indexFile = Path.Combine(indexFile, "DnM_Map_Icon_Index.txt");
                    break;
                case SaveType.AnimalCrossing:
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                    indexFile = Path.Combine(indexFile, "AC_Map_Icon_Index.txt");
                    break;
            }

            if (!File.Exists(indexFile)) return null;
            try
            {
                using (var indexReader = File.OpenText(indexFile))
                {
                    var iconIndex = new Dictionary<ushort, byte>();
                    string line;
                    while ((line = indexReader.ReadLine()) != null)
                    {
                        if (!line.Contains("0x")) continue;
                        string acreIdStr = line.Substring(0, 6).Replace("0x", ""), indexStr = line.Substring(7).Trim();

                        if (ushort.TryParse(acreIdStr, NumberStyles.AllowHexSpecifier, null, out var acreId) &&
                            byte.TryParse(indexStr, out var index))
                        {
                            iconIndex.Add(acreId, index);
                        }
                    }
                    return iconIndex;
                }
            }
            catch
            {
                // ignored
            }

            return null;
        }
    }
}
