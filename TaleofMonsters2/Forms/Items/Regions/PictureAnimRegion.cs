using System.Drawing;

namespace TaleofMonsters.Forms.Items.Regions
{
    internal class PictureAnimRegion : PictureRegion
    {
        public PictureAnimRegion(int id, int x, int y, int width, int height, int info, VirtualRegionCellType type, int nid)
            : base(id, x, y, width, height, info, type, nid)
        {
        }

        public override void Draw(Graphics g)
        {
            base.Draw(g);

            if (IsIn && nid > 0)
            {
                g.DrawRectangle(Pens.Yellow, x, y, width - 1, height - 1);
            }
        }

        public override void Enter()
        {
            base.Enter();
            if (Parent != null)
            {
                Parent.Invalidate(new Rectangle(x, y, width, height));
            }
        }

        public override void Left()
        {
            base.Left();
            if (Parent != null)
            {
                Parent.Invalidate(new Rectangle(x, y, width, height));
            }
        }
    }
}
