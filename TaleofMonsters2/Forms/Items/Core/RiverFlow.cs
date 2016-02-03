using System.Drawing;

namespace TaleofMonsters.Forms.Items.Core
{
    internal class RiverFlow
    {
        private int x;
        private int y;
        private int width;
        private int height;
        private int margin;
        private int ox;
        private int oy;
        private IconDirections direction;

        public RiverFlow(int x, int y, int width, int height, int margin, IconDirections direction)
        {
            this.x = ox = x;
            this.y = oy = y;
            this.width = width;
            this.height = height;
            this.margin = margin;
            this.direction = direction;
        }

        public int Height
        {
            get { return height; }
        }

        public int Width
        {
            get { return width; }
        }

        public Point GetNextPosition()
        {
            Point rt = new Point(x, y);
            if (direction == IconDirections.LeftToRight)
            {
                x += Width + margin;
            }
            else if (direction == IconDirections.RightToLeft)
            {
                x -= Width + margin;
            }
            else if (direction == IconDirections.UpToDown)
            {
                y += Height + margin;
            }
            else if (direction == IconDirections.DownToUp)
            {
                y -= Height + margin;
            }
            return rt;
        }

        public void Reset()
        {
            x = ox;
            y = oy;
        }
    }

    public enum IconDirections
    {
        LeftToRight,
        RightToLeft,
        UpToDown,
        DownToUp
    }
}
