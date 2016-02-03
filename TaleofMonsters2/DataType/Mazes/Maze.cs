using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.Mazes
{
    internal class Maze
    {
        private int id;
        public MazeConfig MazeConfig;
        public List<MazeItem> items;

        public int Id
        {
            get { return id; }
        }

        public Maze(int mazeId)
        {
            id = mazeId;
            MazeConfig = ConfigData.GetMazeConfig(mazeId);
            items = new List<MazeItem>();
            foreach (int mazeItemId in MazeBook.GetMazeItemIds(mazeId))
            {
                items.Add(new MazeItem(mazeItemId, MazeConfig.Level));
            }
        }

        private int GetItemIndex(int x, int y)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].MazeItemConfig.X == x && items[i].MazeItemConfig.Y == y)
                {
                    if (items[i].IsOn())
                    {
                        return i;
                    }
                    return -1;
                }
            }

            return -1;
        }

        public string GetItemType(int x, int y)
        {
            foreach (MazeItem mazeItem in items)
            {
                if (mazeItem.MazeItemConfig.X == x && mazeItem.MazeItemConfig.Y == y)
                {
                    return mazeItem.IsOn() ? mazeItem.MazeItemConfig.Type : "";
                }
            }
            return "";
        }

        public bool IsBlock(int x, int y)
        {
            return GetItemType(x, y) == "block";
        }

        public void CheckEvent(Forms.BasePanel panel, int x, int y, HsActionCallback success, HsActionCallback fail)
        {
            int idex;
            if ((idex = GetItemIndex(x, y)) >= 0)
            {
                items[idex].Effect(panel, MazeConfig.Map, success, fail);
            }
        }

        public void DrawIcon(Graphics g, Rectangle dest, int x, int y, int frame)
        {
            int idex;
            if ((idex = GetItemIndex(x, y)) >= 0)
            {
                items[idex].DrawIcon(g, dest, frame);
            }
        }
    }
}
