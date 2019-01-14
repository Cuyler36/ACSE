using System.Drawing;
using System.IO;
using ACSE.Core.Debug;
using ACSE.Core.Saves;
using ACSE.Core.Utilities;

namespace ACSE.Core.Town.TownBuildings
{
    public static class TrainStation
    {
        public static bool HasModifiableTrainStation(SaveGeneration generation)
            => generation == SaveGeneration.N64 || generation == SaveGeneration.GCN;

        public static Image GetStationImage(int station)
        {
            Image stationImg = null;
            var stationFile = Path.Combine(PathUtility.GetResourcesDirectory(), "Images", "Icons",
                "Stations", $"{station + 1}.png");

            if (!File.Exists(stationFile)) return null;
            try
            {
                stationImg = Image.FromFile(stationFile);
            }
            catch
            {
                DebugUtility.DebugManagerInstance.WriteLine("Unable to load station image #" + (station + 1),
                    DebugLevel.Error);
            }
            return stationImg;
        }
    }
}
