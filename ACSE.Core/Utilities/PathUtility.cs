using System.IO;
using System.Reflection;

namespace ACSE.Core.Utilities
{
    public static class PathUtility
    {
        /// <summary>
        /// Gets the path to the current executable.
        /// </summary>
        /// <returns>The path string of the current executable.</returns>
        public static string GetExeLocation() => Assembly.GetEntryAssembly().Location;

        /// <summary>
        /// Gets the directory of the executable folder.
        /// </summary>
        /// <returns>The path string of the folder the executable was launched in.</returns>
        public static string GetExeDirectory() => Path.GetDirectoryName(GetExeLocation());

        /// <summary>
        /// Gets the directory of the Resources folder.
        /// </summary>
        /// <returns>The path string of the Resources folder.</returns>
        public static string GetResourcesDirectory() => Path.Combine(GetExeDirectory(), "Resources");

        /// <summary>
        /// Gets the directory of the Images folder.
        /// </summary>
        /// <returns>The path string of the Images folder.</returns>
        public static string GetImagesDirectory() => Path.Combine(GetResourcesDirectory(), "Images");

        /// <summary>
        /// Gets the directory of the Icons folder.
        /// </summary>
        /// <returns>The path string of the Icons folder.</returns>
        public static string GetIconsDirectory() => Path.Combine(GetImagesDirectory(), "Icons");
    }
}
