using System;
using System.Security.Cryptography.X509Certificates;
using ACSE.Core.Players;
using ACSE.Core.Saves;
using ACSE.Core.Villagers;

namespace ACSE.Core.Quests
{
    public enum QuestCategory
    {
        Delivery,
        Errand,
        Contest,
        None
    }

    public enum QuestDeliveryType
    {
        QuestBox1,
        QuestBox2,
        QuestBox3,
        Invalid,
        LostItem
    }

    public enum QuestErrandType
    {
        Invalid,
        Cloth,
        Seed,
        Invalid2,
        Letter,
        Invalid3,
        Invalid4,
        Invalid5,
        Letter2,
        Invalid6,
        Hello,
        Invalid7
    }

    public enum QuestContestType
    {
        Ball = 0,
        Flowers = 1,
        Fish = 2,
        Insect = 3,
        Letter = 4,
        Furniture = 5,
        Shop = 6,
        Weeds = 7,
        Sick = 8
    }

    public static class GCNQuests
    {
        // TODO: This should be moved to a generic quests class.
        private static int GetErrandQuestInfoOffset()
        {
            switch (Save.SaveInstance.SaveType)
            {
                case SaveType.DoubutsuNoMori:
                case SaveType.DongwuSenlin:
                    return 0xEE3C;

                case SaveType.DoubutsuNoMoriPlus:
                    return 0x19804;

                case SaveType.AnimalCrossing:
                    return 0x20550;

                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                    return 0x22478;

                default:
                    return -1;
            }
        }

        public static bool IsPartTimeJobActive(this Player player)
        {
            var offset = GetErrandQuestInfoOffset();
            return offset != -1 && Save.SaveInstance.ReadUInt32(offset, true, true).GetBit(player.Index + 2) == 1;
        }

        public static void SetPartTimeJobStatus(this Player player, bool enabled)
        {
            var offset = GetErrandQuestInfoOffset();
            if (offset == -1) return;

            var currentValue = Save.SaveInstance.ReadUInt32(offset, true, true);
            Save.SaveInstance.Write(offset, currentValue.SetBit(player.Index + 2, enabled), true, true);
        }

        public static void GetCurrentQuest(Save save, Villager villager)
        {
            byte questInfo;

            switch (save.SaveType)
            {
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                    questInfo = save.ReadByte(villager.Offset + 0x59C);

                    var questCategory = (QuestCategory) (questInfo >> 6);

                    switch (questCategory)
                    {
                        case QuestCategory.Delivery:
                            Console.WriteLine(
                                $"Quest Category: {questCategory.ToString()} | Quest Type: {((QuestDeliveryType) (questInfo & 0x3F)).ToString()}");
                            break;
                        case QuestCategory.Errand:
                            Console.WriteLine(
                                $"Quest Category: {questCategory.ToString()} | Quest Type: {((QuestErrandType)(questInfo & 0x3F)).ToString()}");
                            break;
                        case QuestCategory.Contest:
                            Console.WriteLine(
                                $"Quest Category: {questCategory.ToString()} | Quest Type: {((QuestContestType)(questInfo & 0x3F)).ToString()}");
                            break;

                        case QuestCategory.None:
                            Console.WriteLine($"No quest for villager #{villager.Index}");
                            break;
                    }

                    break;
            }
        }
    }
}
