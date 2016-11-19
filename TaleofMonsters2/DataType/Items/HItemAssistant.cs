using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Config;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.DataType.Items
{
    internal static class HItemAssistant
    {
        public static bool UseItemsById(int id, int useMethod)
        {
            HItemConfig itemConfig = ConfigData.GetHItemConfig(id);
            if (itemConfig.Id == ConfigData.NoneHItem.Id)
                return false;

            ItemConsumerConfig consumerConfig = ConfigData.GetItemConsumerConfig(id);
            if (useMethod == HItemTypes.Common)
            {
                if (itemConfig.SubType == HItemTypes.Gift)
                    return UseGift(id);
                if (itemConfig.SubType == HItemTypes.Ore)
                    return Consumer.UseOre(consumerConfig);
                if (itemConfig.SubType == HItemTypes.Item)
                    return Consumer.UseItem(consumerConfig);
                if (itemConfig.SubType == HItemTypes.People)
                    return Consumer.UsePcard(consumerConfig);
                if (itemConfig.SubType == HItemTypes.RandomCard)
                    return Consumer.UseScard(consumerConfig);
            }
            else if (useMethod == HItemTypes.Fight)
            {
                if (itemConfig.SubType == HItemTypes.Fight)
                    return Consumer.UseFightItem(consumerConfig);
            }
            else if (useMethod == HItemTypes.Seed)
            {
                if (itemConfig.SubType == HItemTypes.Seed)
                    return Consumer.UseSeedItem(consumerConfig);
            }

            return false;
        }

        private static bool UseGift(int id)
        {
            RLIItemRateCountList items = ConfigData.GetItemGiftConfig(id).Items;

            int roll = MathTool.GetRandom(1, 101);
            for (int i = 0; i < items.Count; i++)
            {
                RLIItemRateCount item = items[i];

                if (roll < item.RollMin || roll > item.RollMax)
                    continue;
                if (item.Type == 1)
                {
                    UserProfile.InfoBag.AddItem(item.Id, item.Count);
                }
                else if (item.Type == 2)
                {
                    UserProfile.InfoEquip.AddEquip(item.Id);
                }
                else if (item.Type == 3)
                {
                    if (CardConfigManager.GetCardConfig(item.Id).Id>0)
                    {
                        UserProfile.InfoCard.AddCard(item.Id);
                    }
                }
            }

            return true;
        }

    }
}
