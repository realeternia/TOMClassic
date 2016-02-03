using System.Collections.Generic;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;
using System.Drawing;
using TaleofMonsters.Controler.Resource;

namespace TaleofMonsters.DataType.Mazes
{
    static class MazeBook
    {
        public static Image GetMoveImage(string name)
        {
            string fname = string.Format("Move/{0}.PNG", name);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Move", string.Format("{0}.PNG", name));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }

        public static int[] GetMazeItemIds(int mazeId)
        {
            List<int> ids = new List<int>();
            foreach(MazeItemConfig mazeItemConfig in ConfigData.MazeItemDict.Values)
            {
                if(mazeItemConfig.MazeId == mazeId)
                {
                    ids.Add(mazeItemConfig.Id);
                }
            }
            return ids.ToArray();
        }
    }
}
