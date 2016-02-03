using System.Drawing;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms.Items.Regions
{
    internal class ButtonRegion : SubVirtualRegion
    {
        private string path1;
        private string path2;

        public ButtonRegion(int id, int x, int y, int width, int height, int info, string path1, string path2)
            : base(id, x, y, width, height, info)
        {
            this.path1 = path1;
            this.path2 = path2;
        }

        public override void Draw(Graphics g)
        {
            Image img = Controler.Loader.PicLoader.Read("Button", IsIn | state != RegionState.Free ? path2 : path1);
            if (img != null)
            {
                g.DrawImage(img, x, y, width, height);
                img.Dispose();
            }

            foreach (RegionTextDecorator decorator in decorators)
            {
                decorator.Draw(g);
            }

            if (state== RegionState.Rectangled)
            {
                g.DrawRectangle(Pens.Firebrick, x, y, width, height);                
                Brush brush = new SolidBrush(Color.FromArgb(100, Color.Red));
                g.FillRectangle(brush, x, y, width, height);
                brush.Dispose();
            }
            else if (state == RegionState.Blacken)
            {
                Brush brush = new SolidBrush(Color.FromArgb(60, Color.Black));
                g.FillRectangle(brush, x, y, width, height);
                brush.Dispose();
            }
        }

        public override int GetKeyValue()
        {
            return 1;//·µ»Ø·Ç0
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
