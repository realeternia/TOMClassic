using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.DataType.Equips;
using TaleofMonsters.DataType.Items;

namespace TaleofMonsters.DataType.Drops
{
    public static class DropBook
    {
        public static List<int> GetDropItemList(int groupId)
        {
            List<int> items = new List<int>();
            var dropConfig = ConfigData.GetDropConfig(groupId);
            
            for (int j = 0; j < dropConfig.Count; j++)
            {
                if (dropConfig.Items.Count > 0)
                {
                    DropItems(dropConfig.Items, items);
                }
                else if (dropConfig.EquipLevel > 0)
                {
                    DropEquips(dropConfig.EquipLevel, items);
                }
                else if(dropConfig.RandomItemRate.Length > 0)
                {
                    DropLevelItems(dropConfig.RandomItemRate, items);
                }
            }
            return items;
        }

        private static void DropItems(RLIdValueList itemConfig, List<int> items)
        {
            int roll = MathTool.GetRandom(100);
            int sum = 0;
            for (int i = 0; i < itemConfig.Count; i++)
            {
                sum += itemConfig[i].Value;
                if (sum > roll)
                {
                    items.Add(itemConfig[i].Id);
                    break;
                }
            }
        }

        private static void DropEquips(int equipLevel, List<int> items)
        {
            int[] qualRate = {70, 20, 5, 4, 1};
            int resultQual = 0;
            int sum = 0;
            int roll = MathTool.GetRandom(100);
            for (int i = 0; i < 5; i++)
            {
                sum += qualRate[i];
                if (roll < sum)
                {
                    resultQual = i;
                    break;
                }
            }

            int resultItemId = 0;
            while (resultItemId == 0)
            {
                resultItemId = EquipBook.GetRandEquipByLevelQuality(equipLevel, resultQual);
                resultQual--;//如果该品质没有道具，就降低一档品质继续查找
            }
            
            items.Add(resultItemId);
        }

        private static void DropLevelItems(int[] itemLevelRate, List<int> items)
        {
            int sum = 0;
            foreach (var r in itemLevelRate)
            {
                sum += r;
            }
            int roll = MathTool.GetRandom(sum);
            int rare = 0;
            sum = 0;
            for (int j = 0; j < itemLevelRate.Length; j++)
            {
                sum += itemLevelRate[j];
                if (roll < sum)
                {
                    rare = j + 1;
                    break;
                }
            }

            items.Add(HItemBook.GetRandRareItemId(HItemRandomGroups.Fight, rare));
        }
    }
}