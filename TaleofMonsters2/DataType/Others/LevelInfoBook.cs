using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.Resource;

namespace TaleofMonsters.DataType.Others
{
    internal static class LevelInfoBook
    {
        public static int[] GetLevelInfosByLevel(int level)
        {
            List<int> ids = new List<int>();
            foreach (LevelInfoConfig levelInfoConfig in ConfigData.LevelInfoDict.Values)
            {
                if (levelInfoConfig.Level == level)
                {
                    ids.Add(levelInfoConfig.Id);
                }
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
