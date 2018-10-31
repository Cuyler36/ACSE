using ACSE.Core.Debug;
using ACSE.Core.Saves;

namespace ACSE.Core.Utilities
{
    public static class DebugUtility
    {
        /// <summary>
        /// The current DebugManager instance.
        /// </summary>
        public static DebugManager DebugManagerInstance { get; private set; }

        /// <summary>
        /// Initializes the DebugManager and returns the instance. If it is already initialized, it will return that instance.
        /// </summary>
        /// <param name="saveFile">The current save file to initialize the debug manager with.</param>
        /// <param name="debugLevel">The debug level to initialize the manager with.</param>
        /// <returns>The <see cref="DebugManager"/> object that is now initialized.</returns>
        public static DebugManager InitializeDebugManager(Save saveFile, DebugLevel debugLevel = DebugLevel.Info) =>
            DebugManagerInstance ?? (DebugManagerInstance = new DebugManager(saveFile, debugLevel));
    }
}
