using ACSE.Core.Saves;

namespace ACSE.Core.Town
{
    /// <summary>
    /// Class for retrieving information about item list priorities.
    /// </summary>
    public static class ListPriority
    {
        /// <summary>
        /// Generation 1 (N64, iQue, GCN) priority index titles.
        /// </summary>
        private static readonly string[] PriorityNames = {"Furniture", "Stationary", "Clothes", "Carpets", "Wallpaper"};

        /// <summary>
        /// Names for each level of priority.
        /// </summary>
        private static readonly string[] Priorities = {"Common", "Uncommon", "Rare"};

        /// <summary>
        /// Retrieves the priorities for the A, B, and C (generic) item lists.
        /// </summary>
        /// <param name="priorityIdx">The priority index to retrieve.</param>
        /// <returns>(<see langword="int"/> priorityListA, <see langword="int"/> priorityListB, <see langword="int"/> priorityListC)</returns>
        public static (int, int, int) GetABCListPriority(int priorityIdx)
        {
            var offset = -1;

            switch (Save.SaveInstance?.SaveType)
            {
                case SaveType.DoubutsuNoMori:
                case SaveType.DongwuSenlin:
                    break;

                case SaveType.DoubutsuNoMoriPlus:
                    if (priorityIdx == 5)
                    {
                        priorityIdx = 0;
                    }

                    offset = 0x1963C;
                    break;

                case SaveType.AnimalCrossing:
                    if (priorityIdx == 5)
                    {
                        priorityIdx = 0;
                    }

                    offset = 0x20340;
                    break;

                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                    offset = 0x222EC;
                    break;
            }

            if (Save.SaveInstance == null || offset < 0) return (-1, -1, -1);

            var listPriorityByte = Save.SaveInstance.ReadByte(offset + priorityIdx, true);
            return ((listPriorityByte >> 6) & 3, (listPriorityByte >> 4) & 3, (listPriorityByte >> 2) & 3);
        }
    }
}
