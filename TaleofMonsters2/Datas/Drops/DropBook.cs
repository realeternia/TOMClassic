using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Log;
using NarlonLib.Math;
using TaleofMonsters.Datas.Equips;
using TaleofMonsters.Datas.Items;

namespace TaleofMonsters.Datas.Drops
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
                else if(dropConfig.MaterialRate.Length > 0)
                    DropMaterials(dropConfig.MaterialRate, items);
                else if (dropConfig.DropEquip > 0)
                    DropEquips(dropConfig.DropEquip, items);
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

        private static void DropMaterials(int[] itemLevelRate, List<int> items)
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

        private static void DropEquips(double rate, List<int> items)
        {
            if(MathTool.GetRandom(0f, 1f) < rate)
                items.Add(EquipBook.GetCanDropId());
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