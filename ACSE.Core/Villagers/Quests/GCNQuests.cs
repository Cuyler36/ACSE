using System;
using ACSE.Core.Saves;

namespace ACSE.Core.Villagers.Quests
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
