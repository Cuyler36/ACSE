using System.Drawing;
using System.IO;

namespace ACSE
{
    public static class TrainStation
    {
        public static bool HasModifiableTrainStation(SaveGeneration Generation)
            => Generation == SaveGeneration.N64 || Generation == SaveGeneration.GCN;

        public static Image GetStationImage(int Station)
        {
            Image StationImg = null;
            var StationFile = MainForm.Assembly_Location + "\\Resources\\Images\\Icons\\Stations\\" + (Station + 1).ToString() + ".png";
            if (File.Exists(StationFile))
            {
                try
                {
                    StationImg = Image.FromFile(StationFile);
                }
                catch
                {
                    MainForm.Debug_Manager.WriteLine("Unable to load station image #" + (Station + 1), DebugLevel.Error);
                }
            }
            return StationImg;
        }
    }
}
