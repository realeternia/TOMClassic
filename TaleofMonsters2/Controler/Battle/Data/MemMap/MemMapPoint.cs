using System.Drawing;

namespace TaleofMonsters.Controler.Battle.Data.MemMap
{
    internal class MemMapPoint
    {
        private int xid;
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Tile { get; set; }
        public int SideIndex { get; private set; }
        public int Owner { get; private set; }

        public MemMapPoint(int xid, int x, int y, int total, int tile)
        {
            this.xid = xid;
            X = x;
            Y = y;
            if (xid <= 4)
            {
                SideIndex = xid;
            }
            else if (xid > total - 5)
            {
                SideIndex = total - xid - 1;
            }
            else
            {
                SideIndex = 99;
            }
            Tile = tile;
            Owner = 0;
        }
        
        public bool IsLeft
        {
            get { return xid <= 4; }
        }
        
        public void UpdateOwner(int id)
        {
            Owner = id;
        }

        public Point ToPoint()
        {
            return new Point(X, Y);
        }

        public override string ToString()
        {
            return string.Format("x={0},y={1},tile={2}", X, Y, Tile);
        }
    }
}
