using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.Resource;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.DataType.Achieves
{
    internal static class AchieveBook
    {
        public static AchieveConfig[] GetAchievesByType(int type)
        {
            List<AchieveConfig> achs = new List<AchieveConfig>();
            foreach (AchieveConfig ach in ConfigData.AchieveDict.Values)
            {
                if (ach.Type == type)
                {
                    achs.Add(ach);
                }
            }
            return achs.ToArray();
        }

        public static void CheckByCheckType(string ctype)
        {
            foreach (AchieveConfig ach in ConfigData.AchieveDict.Values)
            {
                if (ach.CheckType == ctype && !UserProfile.Profile.InfoAchieve.GetAchieve(ach.Id))
                {
                    if (UserProfile.Profile.GetAchieveState(ach.Id) >= ach.Condition.Value)
                    {
                        UserProfile.Profile.InfoAchieve.SetAchieve(ach.Id);
                        MainForm.Instance.AddTip(string.Format("|获得成就-|Gold|{0}", ach.Name), "White");
                        UserProfile.InfoBag.AddDiamond(ach.Money);
                    }
                }
            }
        }

        public static Image GetPreview(int id)
        {
            AchieveConfig achieveConfig = ConfigData.AchieveDict[id];

            int wid = 120, heg = 10;
            Font fontsong = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            Font fontsong2 = new Font("宋体", 10*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            List<string> datas = new List<string>();
            List<string> colors = new List<string>();
            datas.Add(achieveConfig.Name);
            colors.Add("Gold");
            datas.Add(achieveConfig.Descript);
            colors.Add("LightBlue");
            datas.Add("");
            colors.Add("White");
            datas.Add("完成情况");
            colors.Add("White");
            int bound = achieveConfig.Condition.Value;
            int get = UserProfile.Profile.GetAchieveState(id);
            if (get >= bound)
            {
                datas.Add("已达成");
                colors.Add("Lime");
            }
            else
            {
                if (achieveConfig.Type == AchieveTypes.Battle)
                {
                    datas.Add("0/1");
                    colors.Add("LemonChiffon");
                }
                else if (get == 0)
                {
                    datas.Add(string.Format("{0}/{1}", get, bound));
                    colors.Add("Red");
                }
                else if (get < bound)
                {
                    datas.Add(string.Format("{0}/{1}", get, bound));
                    colors.Add("LemonChiffon");
                }
                datas.Add(string.Format("达成奖励: {0}", achieveConfig.Money.ToString().PadLeft(2, ' ')));
                colors.Add("Cyan");
            }

            Bitmap bmp = new Bitmap(300, 300);
            Graphics g = Graphics.FromImage(bmp);
            foreach (string s in datas)
            {
                wid = Math.Max(wid, (int)g.MeasureString(s, fontsong).Width);
            }
            heg = datas.Count * 16;
            wid += 5; heg += 5;
            g.Dispose();
            bmp.Dispose();
            bmp = new Bitmap(wid, heg);
            g = Graphics.FromImage(bmp);
            g.FillRectangle(Brushes.Black, 0, 0, wid, heg);
            g.FillRectangle(new SolidBrush(Color.FromArgb(30, 30, 30)), 0, 0, wid, 18);
            Pen pen = new Pen(Brushes.Gray, 2);
            g.DrawRectangle(pen, 1, 1, wid - 3, heg - 3);
            pen.Dispose();
            int y = 3;
            g.DrawString(datas[0], fontsong2, new SolidBrush(Color.FromName(colors[0])), 3, y);
            for (int i = 1; i < datas.Count; i++)
            {
                y += 16;
                Brush brush = new SolidBrush(Color.FromName(colors[i]));
                g.DrawString(datas[i], fontsong, brush, 3, y, StringFormat.GenericTypographic);
                brush.Dispose();
            }

            if (get < bound)
                g.DrawImage(HSIcons.GetIconsByEName("res8"), 82, y - 1, 14, 14);
            fontsong.Dispose();
            fontsong2.Dispose();
            g.Dispose();
            return bmp;
        }

        public static Image GetAchieveImage(int id)
        {
            string fname = string.Format("Achieve/{0}.PNG", ConfigData.AchieveDict[id].Icon);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Achieve", string.Format("{0}.PNG", ConfigData.AchieveDict[id].Icon));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }
    }
}
