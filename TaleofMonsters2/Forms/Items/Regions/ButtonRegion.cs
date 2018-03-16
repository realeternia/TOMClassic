using System.Drawing;
using TaleofMonsters.Core.Loader;

namespace TaleofMonsters.Forms.Items.Regions
{
    internal class ButtonRegion : SubVirtualRegion
    {
        private string path1;
        private string path2;

        private Image drawImage;

        public ButtonRegion(int id, int x, int y, int width, int height, string path1, string path2)
            : base(id, x, y, width, height)
        {
            this.path1 = path1;
            this.path2 = path2;
        }


        public ButtonRegion(int id, int x, int y, int width, int height, Image img1)
            : base(id, x, y, width, height)
        {
            drawImage = img1;
        }

        public override void Draw(Graphics g)
        {
            if (drawImage != null)
            {
                if (isMouseDown)
                    g.DrawImage(drawImage, new Rectangle(X + 2, Y + 2, Width - 2, Height - 2), 0, 0, Width - 2, Height - 2, GraphicsUnit.Pixel);
                else
                    g.DrawImage(drawImage, X, Y, Width, Height);

                if (isIn | state != RegionState.Free)
                {
                    Brush b = new SolidBrush(Color.FromArgb(150, Color.Gray));
                    g.FillRectangle(b, X, Y, Width, Height);
                    b.Dispose();
                }
            }
            else
            {
                if (string.IsNullOrEmpty(path2))
                {
                    Image img = PicLoader.Read("Button", path1);
                    if (img != null)
                    {
                        if (isMouseDown)
                            g.DrawImage(img, new Rectangle(X + 2, Y + 2, Width - 2, Height - 2), 0, 0, Width - 2, Height - 2, GraphicsUnit.Pixel);
                        else
                            g.DrawImage(img, X, Y, Width, Height);

                        img.Dispose();

                        if (isIn | state != RegionState.Free)
                        {
                            Brush b = new SolidBrush(Color.FromArgb(150, Color.Gray));
                            g.FillRectangle(b, X, Y, Width, Height);
                            b.Dispose();
                        }
                    }
                }
                else
                {
                    Image img = PicLoader.Read("Button", isIn | state != RegionState.Free ? path2 : path1);
                    if (img != null)
                    {
                        if (isMouseDown)
                            g.DrawImage(img, new Rectangle(X + 2, Y + 2, Width - 2, Height - 2), 0, 0, Width - 2, Height - 2, GraphicsUnit.Pixel);
                        else
                            g.DrawImage(img, X, Y, Width, Height);
                        img.Dispose();
                    }
                }
            }
            
            foreach (var decorator in decorators)
            {
                if (isMouseDown)
                    decorator.Draw(g, X + 2, Y + 2, Width - 2, Height - 2);
                else
                    decorator.Draw(g, X, Y, Width, Height);
            }

            if (state == RegionState.Rectangled)
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

        public override void MouseDown()
        {
            base.MouseDown();
            if (parent != null)
            {
                parent.Invalidate(new Rectangle(X, Y, Width, Height));
            }
        }

        public override void MouseUp()
        {
            base.MouseUp();
            if (parent != null)
            {
                parent.Invalidate(new Rectangle(X, Y, Width, Height));
            }
        }
    }
}
