using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Drawing;
using NarlonLib.Log;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.Resource;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Others;

namespace TaleofMonsters.DataType.Items
{
    internal static class HItemBook
    {
        class ItemRandomData
        {
            public int ItemId;
            public int Counter;
        }

        class RareItemListData
        {
            public List<ItemRandomData> ItemList;//随机组 
            public int CounterTotal;

            public RareItemListData()
            {
                ItemList = new List<ItemRandomData>();
            }
        }

        private static List<RareItemListData> rareItemList;

        public static int GetRandRareItemId(int rare)
        {
            if (rareItemList == null)
            {
                rareItemList = new List<RareItemListData>();
                for (int i = 0; i < 8; i++) //7个稀有度
                    rareItemList.Add(new RareItemListData());

                foreach (var hItemConfig in ConfigData.HItemDict.Values)
                {
                    if (hItemConfig.Frequency > 0) //需要随机
                    {
                        var counter = rareItemList[hItemConfig.Rare].CounterTotal + hItemConfig.Frequency;
                        rareItemList[hItemConfig.Rare].CounterTotal = counter;
                        var item = new ItemRandomData();
                        item.ItemId = hItemConfig.Id;
                        item.Counter = counter;
                        rareItemList[hItemConfig.Rare].ItemList.Add(item);
                    }
                }
            }

            var rareList = rareItemList[rare].ItemList;
            int roll = MathTool.GetRandom(rareItemList[rare].CounterTotal);

            int low = 1;
            int high = rareList.Count - 1;
            while (high > low+1)//binary search
            {
                int middle = (high+low)/2;
                if (roll<rareList[middle].Counter)
                    high = middle;
                else
                    low = middle;
            }
            return rareList[low].ItemId;
        }

        private static Dictionary<HItemRandomGroups, Dictionary<int, List<int>>> rareMidDict;//随机组，稀有度

        public static int GetRandRareItemIdWithGroup(HItemRandomGroups group, int rare)
        {
            if (rareMidDict == null)
            {
                rareMidDict = new Dictionary<HItemRandomGroups, Dictionary<int, List<int>>>();
                rareMidDict[HItemRandomGroups.Gather] = new Dictionary<int, List<int>>();
                rareMidDict[HItemRandomGroups.Fight] = new Dictionary<int, List<int>>();
                rareMidDict[HItemRandomGroups.Shopping] = new Dictionary<int, List<int>>();//目前只有3个随机组
                foreach (var hItemConfig in ConfigData.HItemDict.Values)
                {
                    if (hItemConfig.RandomGroup > 0)
                    {
                        var group1 = (HItemRandomGroups)hItemConfig.RandomGroup;
                        if (!rareMidDict[group1].ContainsKey(hItemConfig.Rare))
                        {
                            rareMidDict[group1].Add(hItemConfig.Rare, new List<int>());
                        }
                        rareMidDict[group1][hItemConfig.Rare].Add(hItemConfig.Id);
                    }
                }
            }

            var rareList = rareMidDict[group][rare];
            return rareList[MathTool.GetRandom(rareList.Count)];
        }

        private static Dictionary<string, int> itemNameIdDict;
        public static int GetItemId(string ename)
        {
            if (itemNameIdDict == null)
            {
                itemNameIdDict = new Dictionary<string, int>();
                foreach (var hItemConfig in ConfigData.HItemDict.Values)
                {
                    if (itemNameIdDict.ContainsKey(hItemConfig.Ename))
                    {
                        NLog.Warn("GetItemId key={0} exsited", hItemConfig.Ename);
                        continue;
                    }
                    itemNameIdDict[hItemConfig.Ename] = hItemConfig.Id;
                }
                foreach (var hItemConfig in ConfigData.EquipDict.Values)
                {
                    if (itemNameIdDict.ContainsKey(hItemConfig.Ename))
                    {
                        NLog.Warn("GetItemId key={0} existed", hItemConfig.Ename);
                        continue;
                    }
                    itemNameIdDict[hItemConfig.Ename] = hItemConfig.Id;
                }
            }
            return itemNameIdDict[ename];
        }

        public static Image GetHItemImage(int id)
        {
            HItemConfig hItemConfig = ConfigData.GetHItemConfig(id);

            string fname = string.Format("Item/{0}", hItemConfig.Url);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Item", string.Format("{0}.JPG", hItemConfig.Url));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }

        public static Image GetPreview(int id)
        {
            HItemConfig hItemConfig = ConfigData.GetHItemConfig(id);
            if (hItemConfig.Id <= 0)
            {
                return DrawTool.GetImageByString("unknown", 100);
            }

            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(hItemConfig.Name, HSTypes.I2RareColor(hItemConfig.Rare), 20);
            if (hItemConfig.IsUsable)
            {
                if (hItemConfig.SubType == (int)HItemTypes.Fight)
                {
                    tipData.AddTextNewLine("       战斗中双击使用", "Red");
                }
                else if (hItemConfig.SubType == (int)HItemTypes.Seed)
                {
                    tipData.AddTextNewLine("       农场中双击使用", "Red");
                }
                else
                {
                    tipData.AddTextNewLine("       双击使用", "Green");
                }
            }
            if (hItemConfig.Type == (int)HItemTypes.Task)
            {
                tipData.AddTextNewLine("       任务物品", "DarkBlue");
            }
            else if (hItemConfig.Type == (int)HItemTypes.Material)
            {
                tipData.AddTextNewLine(string.Format("       材料(稀有度:{0})", hItemConfig.Rare), "White");
            }
            if (hItemConfig.Attributes != null && hItemConfig.Attributes.Length > 0)
            {
                tipData.AddTextNewLine(string.Format("       特性:{0}", string.Join(",", hItemConfig.Attributes)), "Lime");
            }
            tipData.AddTextNewLine(string.Format("       等级:{0}", hItemConfig.Level), "White");
            tipData.AddTextNewLine("", "White");
            tipData.AddTextLines(hItemConfig.Descript, "White",20,true);
            if (hItemConfig.SubType == (int)HItemTypes.RandomCard)
            {
                var consumerConfig = ConfigData.GetItemConsumerConfig(hItemConfig.Id);
                int totalRate = 0;
                foreach (var rate in consumerConfig.RandomCardRate)
                    totalRate += rate;
                tipData.AddLine();
                tipData.AddTextNewLine("抽卡概率", "White");
                tipData.AddTextNewLine("", "White");
                tipData.AddImage(HSIcons.GetIconsByEName("gem5"));
                tipData.AddText(string.Format("{0:0}%  ", (float)consumerConfig.RandomCardRate[3]*100/ totalRate), "White");
                tipData.AddImage(HSIcons.GetIconsByEName("gem4"));
                tipData.AddText(string.Format("{0:0}%  ", (float)consumerConfig.RandomCardRate[2] * 100 / totalRate), "White");
                tipData.AddLine();
            }
            if (hItemConfig.ShowCollectTip)
            {
                ItemCollectConfig collectConfig = ConfigData.GetItemCollectConfig(id);
                tipData.AddTextNewLine(string.Format("产出环境:{0} {1} {2}", HSTypes.I2TemperatureName(collectConfig.Temperature),
                    HSTypes.I2HumitityName(collectConfig.Humitity), HSTypes.I2AltitudeName(collectConfig.Altitude)), "Green");
            }
            tipData.AddTextNewLine(string.Format("出售价格:{0}", GameResourceBook.InGoldSellItem(hItemConfig.Rare, hItemConfig.ValueFactor)), "Yellow");
            tipData.AddImage(HSIcons.GetIconsByEName("res1"));
            tipData.AddImageXY(GetHItemImage(id), 8, 8, 48, 48, 7, 24, 32, 32);

            return tipData.Image;
        }

    }
}
