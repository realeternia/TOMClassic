using System.Drawing;
using TaleofMonsters.Core;

namespace TaleofMonsters.MainItem.Scenes.SceneObjects
{
    internal class SceneObject
    {
        public int Id { get; protected set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public virtual void Draw(Graphics g, int target)
        {
            Point[] dts = new Point[4];
            dts[0] = new Point(X-Width/2,Y + Height / 2);
            dts[1] = new Point(X - Width / 2 + Width, Y + Height / 2);
            dts[2] = new Point(X - Width / 2 + Width + (int)(Width* GameConstants.SceneTileGradient), Y + Height / 2 - Height);
            dts[3] = new Point(X - Width / 2+ (int)(Width * GameConstants.SceneTileGradient), Y + Height / 2 - Height);
            

            Brush brush = new SolidBrush(Color.FromArgb(100, Color.White));
            g.FillPolygon(brush, dts);
            brush.Dispose();

            Pen pen = new Pen(Color.DimGray, 2);
            g.DrawPolygon(pen, dts);
            pen.Dispose();
        }
    }
}
