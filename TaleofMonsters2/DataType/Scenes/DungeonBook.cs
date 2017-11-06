using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.Resource;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.DataType.Scenes
{
    internal static class DungeonBook
    {
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
            TipImage tipData = new TipImage();

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