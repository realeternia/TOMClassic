using System;
using System.Drawing;

namespace SceneMaker
{
    internal class SceneObject
    {
        public readonly int Id;
        public readonly int X;
        public readonly int Y;
        public readonly int Width;
        public readonly int Height;
        public bool Disabled { get; set; }
        public bool MapSetting { get; set; }

        public SceneObject(int wid, int wx, int wy, int wwidth, int wheight)
        {
            Id = wid;
            X = wx;
            Y = wy;
            Width = wwidth;
            Height = wheight;
        }

        public virtual void Draw(Graphics g, int target)
        {
            Color tileColor = Color.White;
            Color lineColor = Color.DimGray;

            if (Id < 1000)
            {
                tileColor = Color.FromArgb((100 + Id*359)%255, (176 + Id*77)%255, (7 + Id*3507)%255);
            }

            Point[] dts = new Point[4];
            dts[0] = new Point(X - Width / 2, Y + Height / 2);
            dts[1] = new Point(X - Width / 2 + Width, Y + Height / 2);
            dts[2] = new Point(X - Width / 2 + Width + (int)(Width * GameConstants.SceneTileGradient), Y + Height / 2 - Height);
            dts[3] = new Point(X - Width / 2 + (int)(Width * GameConstants.SceneTileGradient), Y + Height / 2 - Height);


            Brush brush = new SolidBrush(Color.FromArgb(100, tileColor));
            g.FillPolygon(brush, dts);
            brush.Dispose();

            Pen pen = new Pen(lineColor, 2);
            g.DrawPolygon(pen, dts);
            pen.Dispose();

#if DEBUG
            Font font = new Font("宋体", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            g.DrawString(Id.ToString(), font, Brushes.Black, X + 1, Y + 1);
            g.DrawString(Id.ToString(), font, Brushes.White, X, Y);
            font.Dispose();
#endif
        }
    }
}