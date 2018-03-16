using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Tools;

namespace TaleofMonsters.Datas.Others
{
    internal static class LevelInfoBook
    {
        public static int[] GetLevelInfosByLevel(int level)
        {
            List<int> ids = new List<int>();
            foreach (var levelInfoConfig in ConfigData.LevelInfoDict.Values)
            {
                if (levelInfoConfig.Level == level)
                    ids.Add(levelInfoConfig.Id);
            }
            return ids.ToArray();
        }

        public static Image GetLevelInfoImage(int id)
        {
            LevelInfoConfig levelInfoConfig = ConfigData.GetLevelInfoConfig(id);

            string fname = string.Format("LevelInfo/{0}.JPG", levelInfoConfig.Icon);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("LevelInfo", string.Format("{0}.JPG", levelInfoConfig.Icon));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }
    }
}
