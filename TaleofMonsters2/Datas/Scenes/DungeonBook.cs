using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Log;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Tools;

namespace TaleofMonsters.Datas.Scenes
{
    internal static class DungeonBook
    {
        private static Dictionary<string, int> itemNameIdDict;
        public static int GetDungeonItemId(string ename)
        {
            if (itemNameIdDict == null)
            {
                itemNameIdDict = new Dictionary<string, int>();
                foreach (var hItemConfig in ConfigData.DungeonItemDict.Values)
                {
                    if (itemNameIdDict.ContainsKey(hItemConfig.Ename))
                    {
                        NLog.Warn("GetDungeonItemId key={0} exsited", hItemConfig.Ename);
                        continue;
                    }
                    itemNameIdDict[hItemConfig.Ename] = hItemConfig.Id;
                }
            }
            return itemNameIdDict[ename];
        }

        public static Image GetDungeonItemImage(int id)
        {
            var itemConfig = ConfigData.GetDungeonItemConfig(id);
            string fname = string.Format("Dungeon/Item/{0}.PNG", itemConfig.Url);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Dungeon.Item", string.Format("{0}.PNG", itemConfig.Url));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }

        public static List<int> GetGismoListByDungeon(int dungeonId)
        {
            List<int> gisList = new List<int>();
            foreach (var gisConfig in ConfigData.DungeonGismoDict.Values)
            {
                if (gisConfig.DungeonId == dungeonId)
                    gisList.Add(gisConfig.Id);
            }

            return gisList;
        }

        public static Image GetGismoImage(int id)
        {
            var gismoConfig = ConfigData.GetDungeonGismoConfig(id);
            string fname = string.Format("Dungeon/Gismo/{0}.PNG", gismoConfig.Icon);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Dungeon.Gismo", string.Format("{0}.PNG", gismoConfig.Icon));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }

        public static Image GetPreview(int key)
        {
            var config = ConfigData.GetDungeonGismoConfig(key);
            TipImage tipData = new TipImage(PaintTool.GetTalkColor);

            var hasGismo = UserProfile.InfoGismo.GetGismo(key);
            if (hasGismo)
            {
                tipData.AddTextNewLine(config.Name, "White", 20);
            }
            else
            {
                tipData.AddTextNewLine(config.Name + "(未达成)", "Red", 20);
                tipData.AddTextNewLine("难度："+ ("★★★★★").Substring(5 - config.Hard), "Gold", 20);
            }
            tipData.AddLine(2);
            tipData.AddTextLines(config.Descript, "White", 15, true);
            return tipData.Image;
        }
    }
}