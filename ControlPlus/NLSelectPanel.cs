using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace ControlPlus
{
    public class NLSelectPanel
    {
        public delegate void SelectEventHandler();
        public event SelectEventHandler SelectIndexChanged;

        public delegate void SelectPanelCellDrawHandler(Graphics g, int info, int xOff, int yOff, bool inMouseOn, bool isTarget, bool onlyBorder);
        public event SelectPanelCellDrawHandler DrawCell;

        public int ItemHeight { get; set; }
        public int ItemsPerRow { get; set; }

        private int selectIndex;
        private int moveIndex;
        private List<int> infos;

        private int x;
        private int y;
        private int width;
        private int height;
        private Control parent;

        private Bitmap tempImage;

        public bool UseCache { get; set; }

        public NLSelectPanel(int x, int y, int width, int height, Control parent)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.parent = parent;
            ItemsPerRow = 1;
            parent.MouseMove += OnMouseMove;
            parent.MouseClick += OnMouseClick;
            parent.Paint += OnPaint;
            infos = new List<int>();
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

        public void AddContent(List<int> infoData)
        {
            infos.Clear();
            infos.AddRange(infoData);

            if (UseCache)
            {
                if (tempImage != null)
                    tempImage.Dispose();

                tempImage = new Bitmap(width, height);
                var itemWidth = width / ItemsPerRow;
                Graphics g = Graphics.FromImage(tempImage);
                for (int i = 0; i < infoData.Count; i++)
                {
                    int drawX = (i % ItemsPerRow) * itemWidth;
                    int drawY = (i / ItemsPerRow) * ItemHeight;
                    if (DrawCell != null)
                    {
                        DrawCell(g, infos[i], drawX, drawY, false, false, false);
                    }
                }
                g.Dispose();
            }

        }

        private void OnMouseMove(object o, MouseEventArgs e)
        {
            var itemWidth = width/ItemsPerRow;
            int index = -1;
            if (e.X > x && e.X < x + width && e.Y > y && e.Y < y + height)
            {
                index = (e.Y - y) / ItemHeight * ItemsPerRow + (e.X - x) / itemWidth;
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
            if (UseCache && tempImage != null)
                e.Graphics.DrawImage(tempImage, x, y);
            var itemWidth = width / ItemsPerRow;
            for (int i = 0; i < infos.Count; i++)
            {
                int drawX = x + (i%ItemsPerRow)*itemWidth;
                int drawY = y + (i / ItemsPerRow) * ItemHeight;

                if (DrawCell != null)
                {
                    DrawCell(e.Graphics, infos[i], drawX, drawY, i == moveIndex, i == selectIndex, UseCache);
                }
            }
        }
    }
}
