using System.Drawing;

namespace TaleofMonsters.Forms.Items.Regions
{
    internal class ButtonRegion : SubVirtualRegion
    {
        private string path1;
        private string path2;

        public ButtonRegion(int id, int x, int y, int width, int height, string path1, string path2)
            : base(id, x, y, width, height)
        {
            this.path1 = path1;
            this.path2 = path2;
        }

        public override void Draw(Graphics g)
        {
            Image img = Controler.Loader.PicLoader.Read("Button", isIn | state != RegionState.Free ? path2 : path1);
            if (img != null)
            {
                g.DrawImage(img, X, Y, Width, Height);
                img.Dispose();
            }

            foreach (var decorator in decorators)
            {
                decorator.Draw(g, X, Y, Width, Height);
            }

            if (state== RegionState.Rectangled)
            {
                g.DrawRectangle(Pens.Firebrick, X, Y, Width, Height);                
                Brush brush = new SolidBrush(Color.FromArgb(100, Color.Red));
                g.FillRectangle(brush, X, Y, Width, Height);
                brush.Dispose();
            }
            else if (state == RegionState.Blacken)
            {
                Brush brush = new SolidBrush(Color.FromArgb(60, Color.Black));
                g.FillRectangle(brush, X, Y, Width, Height);
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
