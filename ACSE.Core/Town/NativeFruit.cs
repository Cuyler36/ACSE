using ACSE.Core.Saves;

namespace ACSE.Core.Town
{
    public static class NativeFruit
    {
        private static readonly string[] Gen1Fruits = {"Apple", "Cherry", "Pear", "Peach", "Orange"}; // N64 -> GCN

        private static readonly string[] WildWorldFruits = {"Apple", "Orange", "Pear", "Peach", "Cherry"};

        private static readonly string[] CityFolkFruits = {"Apple", "Orange", "Pear", "Peach", "Cherry"};

        private static readonly string[] NewLeafFruits = {"Apple", "Orange", "Pear", "Peach", "Cherry"};

        public static string[] GetNativeFruitTypes(SaveGeneration saveGeneration)
        {
            switch (saveGeneration)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                case SaveGeneration.iQue:
                    return Gen1Fruits;

                case SaveGeneration.NDS:
                    return WildWorldFruits;

                case SaveGeneration.Wii:
                    return CityFolkFruits;

                case SaveGeneration.N3DS:
                    return NewLeafFruits;

                default:
                    return null;
            }
        }

        public static int GetNativeFruit(Save save)
        {
            var fruitIdx = -1;

            switch (save.SaveType)
            {
                case SaveType.DoubutsuNoMori:
                case SaveType.DongwuSenlin:
                    fruitIdx = save.ReadUInt16(0xEF58, save.IsBigEndian, true) & 0xFF;
                    break;

                case SaveType.DoubutsuNoMoriPlus:
                    fruitIdx = save.ReadUInt16(0x19928, save.IsBigEndian, true) & 0xFF;
                    break;

                case SaveType.AnimalCrossing:
                    fruitIdx = save.ReadUInt16(0x20688, save.IsBigEndian, true) & 0xFF;
                    break;

                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                    fruitIdx = save.ReadUInt16(0x2259C, save.IsBigEndian, true) & 0xFF;
                    break;

                case SaveType.WildWorld:
                    fruitIdx = -1; // TODO: Research Wild World native fruit. It's not stored like the rest of the games apparently.
                    break;

                case SaveType.CityFolk:
                    fruitIdx = (save.ReadUInt16(0x683C2, save.IsBigEndian, true) >> 2) & 0xFF; // City Folk's fruit is every 4 ids.
                    break;

                case SaveType.NewLeaf:
                    fruitIdx = save.ReadUInt16(0x5C7B6, save.IsBigEndian, true) & 0xFF;
                    break;

                case SaveType.WelcomeAmiibo:
                    fruitIdx = save.ReadUInt16(0x621BA, save.IsBigEndian, true) & 0xFF;
                    break;
            }

            return fruitIdx < 0 || fruitIdx > 4 ? 0 : fruitIdx;
        }

        public static void SetNativeFruit(Save save, int fruitIdx)
        {
            if (fruitIdx < 0 || fruitIdx > 4)
            {
                fruitIdx = 0;
            }

            switch (save.SaveType)
            {
                case SaveType.DoubutsuNoMori:
                case SaveType.DongwuSenlin:
                    save.Write(0xEF58, (ushort)(0x2800 | fruitIdx), save.IsBigEndian, true);
                    break;

                case SaveType.DoubutsuNoMoriPlus:
                    save.Write(0x19928, (ushort)(0x2800 | fruitIdx), save.IsBigEndian, true);
                    break;

                case SaveType.AnimalCrossing:
                    save.Write(0x20688, (ushort)(0x2800 | fruitIdx), save.IsBigEndian, true);
                    break;

                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                    save.Write(0x2259C, (ushort)(0x2800 | fruitIdx), save.IsBigEndian, true);
                    break;

                case SaveType.WildWorld:
                    break; // TODO: Research Wild World native fruit. It's not stored like the rest of the games apparently.

                case SaveType.CityFolk:
                    save.Write(0x683C2, (ushort)(0x9000 | (fruitIdx << 2)), save.IsBigEndian, true);
                    break;

                case SaveType.NewLeaf:
                    save.Write(0x5C7B6, (ushort)(0x2000 | fruitIdx), save.IsBigEndian, true);
                    break;

                case SaveType.WelcomeAmiibo:
                    save.Write(0x621BA, (ushort)(0x2000 | fruitIdx), save.IsBigEndian, true);
                    break;
            }
        }
    }
}
