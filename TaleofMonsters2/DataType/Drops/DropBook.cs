using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;

namespace TaleofMonsters.DataType.Drops
{
    public static class DropBook
    {
        public static List<int> GetDropItemList(int groupId)
        {
            List<int> items = new List<int>();
            var itemConfig = ConfigData.GetDropConfig(groupId).Items;
            if (itemConfig.Count > 0)
            {
                for (int j = 0; j < ConfigData.GetDropConfig(groupId).Count; j++)
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
            }
            return items;
        }
    }
}