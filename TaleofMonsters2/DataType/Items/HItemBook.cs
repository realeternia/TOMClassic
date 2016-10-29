using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.Resource;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.Items
{
    static class HItemBook
    {
        private static Dictionary<int, List<int>> rareMidDict;

        static public int GetRandRareMid(int rare)
        {
            if (rareMidDict == null)
            {
                rareMidDict = new Dictionary<int, List<int>>();
                foreach (HItemConfig hItemConfig in ConfigData.HItemDict.Values)
                {
                    if (hItemConfig.IsRandom)
                    {
                        if (!rareMidDict.ContainsKey(hItemConfig.Rare))
                        {
                            rareMidDict.Add(hItemConfig.Rare, new List<int>());
                        }
                        rareMidDict[hItemConfig.Rare].Add(hItemConfig.Id);
                    }
                }
            }

            return rareMidDict[rare][MathTool.GetRandom(rareMidDict[rare].Count)];
        }

        static public Image GetHItemImage(int id)
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

        public static bool IsGiftHasCard(int id)
        {
            foreach (ItemGiftConfig itemGiftConfig in ConfigData.ItemGiftDict.Values)
            {
                for (int i = 0; i < itemGiftConfig.Items.Count; i++)
                {
                    if (itemGiftConfig.Items[i].Id == id && itemGiftConfig.Items[i].Type==3)
                    {
                        return true;
                    }
                }

            }
            return false;
        }

        public static Image GetPreview(int id)
        {
            HItemConfig hItemConfig = ConfigData.GetHItemConfig(id);

            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(hItemConfig.Name, HSTypes.I2RareColor(hItemConfig.Rare), 20);
            if (hItemConfig.IsUsable)
            {
                if (hItemConfig.SubType == HItemTypes.Fight)
                {
                    tipData.AddTextNewLine("       战斗中双击使用", "Red");
                }
                else if (hItemConfig.SubType == HItemTypes.Seed)
                {
                    tipData.AddTextNewLine("       农场中双击使用", "Red");
                }
                else
                {
                    tipData.AddTextNewLine("       双击使用", "Green");
                }
            }
            else if (hItemConfig.SubType == HItemTypes.Task)
            {
                tipData.AddTextNewLine("       任务物品", "DarkBlue");
            }
            else if (hItemConfig.SubType == HItemTypes.Material)
            {
                tipData.AddTextNewLine(string.Format("       材料(稀有度:{0})", hItemConfig.Rare), "White");
            }
            else
            {
                tipData.AddTextNewLine("", "White");
            }
            tipData.AddTextNewLine(string.Format("       等级:{0}", hItemConfig.Level), "White");
            tipData.AddTextNewLine("", "White");
            tipData.AddTextLines(hItemConfig.Descript, "White",20,true);
            tipData.AddTextNewLine(string.Format("出售价格:{0}", hItemConfig.Value), "Yellow");
            tipData.AddImage(HSIcons.GetIconsByEName("res1"));
            tipData.AddImageXY(GetHItemImage(id), 8, 8, 48, 48, 7, 24, 32, 32);

            return tipData.Image;
        }

    }
}
