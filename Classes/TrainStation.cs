using System.Drawing;
using System.IO;

namespace ACSE
{
    public static class TrainStation
    {
        public static bool HasModifiableTrainStation(SaveGeneration generation)
            => generation == SaveGeneration.N64 || generation == SaveGeneration.GCN;

        public static Image GetStationImage(int station)
        {
            Image stationImg = null;
            var stationFile = MainForm.AssemblyLocation + "\\Resources\\Images\\Icons\\Stations\\" + (station + 1).ToString() + ".png";
            if (!File.Exists(stationFile)) return null;
            try
            {
                stationImg = Image.FromFile(stationFile);
            }
            catch
            {
                MainForm.DebugManager.WriteLine("Unable to load station image #" + (station + 1), DebugLevel.Error);
            }
            return stationImg;
        }
    }
}
