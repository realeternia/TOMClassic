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

        public static void DrawOnDeck(int id, Graphics g, int xoff, int yoff)
        {
            HItemConfig hItemConfig = ConfigData.GetHItemConfig(id);

            int basel = yoff;
            g.FillRectangle(new SolidBrush(Color.FromArgb(190, 175, 160)), xoff + 10, basel, 180, 20);
            for (int i = 0; i < 1; i++)
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(215, 210, 200)), xoff + 10, basel + 35 + i * 30, 180, 15);
            }
            g.FillRectangle(new SolidBrush(Color.FromArgb(190, 175, 160)), xoff + 10, basel + 55, 180, 20);
            for (int i = 0; i < 2; i++)
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(215, 210, 200)), xoff + 10, basel + 90 + i * 30, 180, 15);
            }
            g.FillRectangle(new SolidBrush(Color.FromArgb(215, 210, 200)), xoff + 10, basel + 125, 180, 45);

            Font fontblack = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            Font fontsong = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString("主要信息", fontblack, Brushes.White, xoff + 10, basel + 2);
            g.DrawString(string.Format("{0}", hItemConfig.Name), fontsong, Brushes.Black, xoff + 10, basel + 21);
            g.DrawString(("★★★★★★★★★★").Substring(10 - hItemConfig.Rare), fontsong, Brushes.DeepPink, xoff + 10, basel + 36);

            SolidBrush sb = new SolidBrush(Color.FromArgb(100, 50, 0));

            g.DrawString("效果", fontblack, Brushes.White, xoff + 10, basel + 57);
            g.DrawString(string.Format("等级: {0}", hItemConfig.Level), fontsong, sb, xoff + 10, basel + 76);
            g.DrawString(string.Format("类型: {0}", HSTypes.I2HItemType(hItemConfig.Type)), fontsong, sb, xoff + 10, basel + 91);
            g.DrawString(string.Format("价值: {0}", hItemConfig.Value), fontsong, sb, xoff + 10, basel + 106);

            string descript = hItemConfig.Descript;
            if (descript.Length < 13)
            {
                g.DrawString(descript, fontsong, sb, xoff + 10, basel + 121);
            }
            else if (descript.Length < 26)
            {
                g.DrawString(descript.Substring(0, 13), fontsong, sb, xoff + 10, basel + 121);
                g.DrawString(descript.Substring(13), fontsong, sb, xoff + 10, basel + 136);
            }
            else
            {
                g.DrawString(descript.Substring(0, 13), fontsong, sb, xoff + 10, basel + 121);
                g.DrawString(descript.Substring(13, 13), fontsong, sb, xoff + 10, basel + 136);
                g.DrawString(descript.Substring(26), fontsong, sb, xoff + 10, basel + 151);
            }
            fontblack.Dispose();
            fontsong.Dispose();
            sb.Dispose();
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

            string des = hItemConfig.Descript;
            while (true)
            {
                tipData.AddTextNewLine(des.Substring(0, Math.Min(des.Length, 20)), "White");
                if (des.Length <= 20)
                    break;
                des = des.Substring(20);
            }
            tipData.AddTextNewLine(string.Format("出售价格:{0}", hItemConfig.Value), "Yellow");
            tipData.AddImage(HSIcons.GetIconsByEName("res1"));
            tipData.AddImageXY(GetHItemImage(id), 8, 8, 48, 48, 7, 24, 32, 32);

            return tipData.Image;
        }

    }
}
