using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Log;
using NarlonLib.Math;
using TaleofMonsters.DataType.Equips;
using TaleofMonsters.DataType.Items;

namespace TaleofMonsters.DataType.Drops
{
    public static class DropBook
    {
        private static Dictionary<string, int> itemNameIdDict;
        public static int GetDropId(string ename)
        {
            if (itemNameIdDict == null)
            {
                itemNameIdDict = new Dictionary<string, int>();
                foreach (var hItemConfig in ConfigData.DropDict.Values)
                {
                    if (itemNameIdDict.ContainsKey(hItemConfig.Ename))
                    {
                        NLog.Warn("GetDropId key={0} exsited", hItemConfig.Ename);
                        continue;
                    }
                    itemNameIdDict[hItemConfig.Ename] = hItemConfig.Id;
                }
            }
            return itemNameIdDict[ename];
        }

        public static List<int> GetDropItemList(string groupName)
        {
            return GetDropItemList(GetDropId(groupName));
        }

        public static List<int> GetDropItemList(int groupId)
        {
            List<int> items = new List<int>();
            var dropConfig = ConfigData.GetDropConfig(groupId);
            
            for (int j = 0; j < dropConfig.Count; j++)
            {
                if (dropConfig.Items.Length > 0)
                    DropItems(dropConfig.Items, dropConfig.ItemRate, items);
                else if (dropConfig.EquipQualityMin > 0 || dropConfig.EquipQualityMax > 0)
                    DropEquips(dropConfig.EquipQualityMin, dropConfig.EquipQualityMax, items);
                else if(dropConfig.RandomItemRate.Length > 0)
                    DropLevelItems(dropConfig.RandomItemRate, items);
            }
            return items;
        }

        private static void DropItems(string[] dropItems, int[] rates, List<int> items)
        {
            int roll = MathTool.GetRandom(100);
            int sum = 0;
            for (int i = 0; i < dropItems.Length; i++)
            {
                sum += rates[i];
                if (sum > roll)
                {
                    var itemId = HItemBook.GetItemId(dropItems[i]);
                    items.Add(itemId);
                    break;
                }
            }
        }

        private static void DropEquips(int qualMin, int qualMax, List<int> items)
        {
            int resultQual = 0;
            if (qualMax == qualMin)
            {
                resultQual = qualMin;
            }
            else
            {
                resultQual = qualMin;
                while (MathTool.GetRandom(3) == 0 && resultQual < qualMax)
                    resultQual ++;
            }

            int resultItemId = 0;
            while (resultItemId == 0)
            {
                resultItemId = EquipBook.GetRandEquipByLevelQuality(resultQual);
                resultQual--;//如果该品质没有道具，就降低一档品质继续查找
            }
            
            items.Add(resultItemId);
        }

        private static void DropLevelItems(int[] itemLevelRate, List<int> items)
        {
            int sum = 0;
            foreach (var rate in itemLevelRate)
                sum += rate;
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

            items.Add(HItemBook.GetRandRareItemIdWithGroup(HItemRandomGroups.Fight, rare));
        }

        /// <summary>
        /// 按概率获得采集道具列表
        /// </summary>
        public static int[] GetCollectItems(int type, int sceneId)
        {
            List<int> itemList = new List<int>();
            List<float> rateList = new List<float>();
            var sceneConfig = ConfigData.GetSceneConfig(sceneId);
            foreach (var itemConfig in ConfigData.ItemCollectDict.Values)
            {
                if (itemConfig.Type != type)
                    continue;

                itemList.Add(itemConfig.Id);
                rateList.Add(GetCollectDropRate(itemConfig, sceneConfig));
            }

            return NLRandomPicker<int>.RandomPickN(itemList.ToArray(), rateList.ToArray(), 2);
        }

        private static float GetCollectDropRate(ItemCollectConfig itemCollectConfig, SceneConfig sceneConfig)
        {
            var itemConfig = ConfigData.GetHItemConfig(itemCollectConfig.Id);
            var baseDrop = 100f/itemConfig.Rare;

            int attrDiffer = Math.Abs(itemCollectConfig.Temperature - sceneConfig.Temperature) +
                             Math.Abs(itemCollectConfig.Humitity - sceneConfig.Humitity) +
                             Math.Abs(itemCollectConfig.Altitude - sceneConfig.Altitude) + 1;

            if (attrDiffer > 0)
                baseDrop /= attrDiffer;

            if (itemCollectConfig.Temperature == sceneConfig.Temperature)
                baseDrop *= 1.5f;
            if (itemCollectConfig.Humitity == sceneConfig.Humitity)
                baseDrop *= 1.5f;
            if (itemCollectConfig.Altitude == sceneConfig.Altitude)
                baseDrop *= 1.5f;

            return baseDrop;
        }
    }
}