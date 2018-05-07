using System;
using System.Collections.Generic;
using System.Drawing;

namespace ControlPlus
{
    public class CellItemBox
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        List<ICellItem> itemList = new List<ICellItem>();

        public CellItemBox(int x, int y,int w, int h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        public void AddItem(ICellItem cell)
        {
            itemList.Add(cell);
            if (itemList.Count == 0)
            {
                cell.X = X;
                cell.Y = Y;
            }
            else
            {
                var lastCell = itemList[itemList.Count - 1];
                cell.X = lastCell.X + lastCell.Width;
                cell.Y = lastCell.Y;
                if (cell.X + lastCell.Width > X + Width)
                {
                    cell.X = X;
                    cell.Y += lastCell.Height;
                }
            }
        }

        public void Refresh(int index, object data)
        {
            itemList[index].RefreshData(data);
        }

        public void Draw(Graphics g)
        {
            foreach (var ctl in itemList)
                ctl.Draw(g);
        }

        public void OnFrame()
        {
            foreach (var ctl in itemList)
                ctl.OnFrame();
        }

        public void Dispose()
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                itemList[i].Dispose();
                itemList[i] = null;
            }
        }
    }

    public interface ICellItem : IDisposable
    {
        void Init(int idx);

        void RefreshData(object data);

        int X { set; get; }
        int Y { set; get; }
        int Width { get; }
        int Height { get; }

        void Draw(Graphics g);

        void OnFrame();
    }
}