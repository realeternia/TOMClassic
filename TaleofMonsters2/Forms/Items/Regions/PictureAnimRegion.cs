using System.Drawing;

namespace TaleofMonsters.Forms.Items.Regions
{
    internal class PictureAnimRegion : PictureRegion
    {
        public PictureAnimRegion(int id, int x, int y, int width, int height, PictureRegionCellType type, int nid)
            : base(id, x, y, width, height, type, nid)
        {
        }

        public override void Draw(Graphics g)
        {
            base.Draw(g);

            if (isIn && nid > 0)
            {
                g.DrawRectangle(Pens.Yellow, X, Y, Width - 1, Height - 1);
            }
        }

        public override void Enter()
        {
            base.Enter();
            if (parent != null)
            {
                parent.Invalidate(new Rectangle(X, Y, Width, Height));
            }
        }

        public override void Left()
        {
            base.Left();
            if (parent != null)
            {
                parent.Invalidate(new Rectangle(X, Y, Width, Height));
            }
        }
    }
}
