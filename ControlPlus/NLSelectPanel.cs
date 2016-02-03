using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace ControlPlus
{
    public class NLSelectPanel
    {
        public delegate void SelectEventHandler();
        public event SelectEventHandler SelectIndexChanged;

        public delegate void SelectPanelCellDrawHandler(Graphics g, int info, int xOff, int yOff);
        public event SelectPanelCellDrawHandler DrawCell;

        private int itemHeight;
        private int selectIndex;
        private int moveIndex;
        private List<int> infos;

        private int x;
        private int y;
        private int width;
        private int height;
        private Control parent;

        public NLSelectPanel(int x, int y, int width, int height, Control parent)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.parent = parent;
            parent.MouseMove += OnMouseMove;
            parent.MouseClick += OnMouseClick;
            parent.Paint += OnPaint;
            infos = new List<int>();
        }

        public int ItemHeight
        {
            get { return itemHeight; }
            set { itemHeight = value; }
        }

        public int SelectIndex
        {
            get { return selectIndex; }
            set { selectIndex = value; SelectIndexChanged(); }
        }

        public int SelectInfo
        {
            get { return infos[selectIndex]; }
        }

        public Rectangle Rectangle
        {
            get { return new Rectangle(x, y, width, height); }
        }

        public void ClearContent()
        {
            infos.Clear();
        }

        public void AddContent(int info)
        {
            infos.Add(info);
        }

        public void UpdateContent(int info)
        {
            infos[selectIndex] = info;
        }

        private void OnMouseMove(object o, MouseEventArgs e)
        {
            int index = -1;
            if (e.X > x && e.X < x + width && e.Y > y && e.Y < y + height)
            {
                index = (e.Y - y) / itemHeight;
                if (index < 0 || index >= infos.Count)
                {
                    index = -1;
                }
            }

            if (index != moveIndex)
            {
                moveIndex = index;
                parent.Invalidate(new Rectangle(x, y, width, height));
            }
        }

        private void OnMouseClick(object o, MouseEventArgs e)
        {
            if (moveIndex != selectIndex && moveIndex != -1)
            {
                selectIndex = moveIndex;
                SelectIndexChanged();
                parent.Invalidate(new Rectangle(x, y, width, height));
            }
        }

        private void OnPaint(object o, PaintEventArgs e)
        {
            for (int i = 0; i < infos.Count; i++)
            {
                if (i == selectIndex)
                {
                    e.Graphics.FillRectangle(Brushes.DarkGreen, x, itemHeight * i + y, width, itemHeight);
                }
                else if (i == moveIndex)
                {
                    e.Graphics.FillRectangle(Brushes.DarkCyan, x, itemHeight * i + y, width, itemHeight);
                }
                e.Graphics.DrawRectangle(Pens.Thistle, 1 + x, itemHeight * i + 2 + y, width - 2, itemHeight - 4);

                if (DrawCell != null)
                {
                    DrawCell(e.Graphics, infos[i], x, itemHeight*i + y);
                }
            }
        }
    }
}
