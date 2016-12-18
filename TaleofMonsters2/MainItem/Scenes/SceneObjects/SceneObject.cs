using System.Drawing;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.MainItem.Scenes.SceneObjects
{
    internal class SceneObject
    {
        public int Id { get; protected set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public bool IsMouseIn(int mx, int my)
        {
            if (my < Y + Height / 2 - Height || my > Y + Height / 2)
                return false;

            int xDiff = (my - (Y + Height/2))*(int) (Width*GameConstants.SceneTileGradient)/Height;
            if (mx < X - Width/2 - xDiff || mx > X + Width/2 - xDiff)
                return false;
            return true;
        }

        public bool OnClick()
        {
            if (SceneManager.CanMove(Id, UserProfile.Profile.InfoBasic.Position))
            {
                UserProfile.Profile.InfoBasic.Position = Id;
                return true;
            }
            return false;
        }

        public virtual void Draw(Graphics g, int target)
        {
            Color tileColor = Color.White;
            Color lineColor = Color.DimGray;

            if (target == Id)
            {
                tileColor = Color.Yellow;
                lineColor = Color.Orange;
            }

            Point[] dts = new Point[4];
            dts[0] = new Point(X-Width/2,Y + Height / 2);
            dts[1] = new Point(X - Width / 2 + Width, Y + Height / 2);
            dts[2] = new Point(X - Width / 2 + Width + (int)(Width* GameConstants.SceneTileGradient), Y + Height / 2 - Height);
            dts[3] = new Point(X - Width / 2+ (int)(Width * GameConstants.SceneTileGradient), Y + Height / 2 - Height);
            

            Brush brush = new SolidBrush(Color.FromArgb(100, tileColor));
            g.FillPolygon(brush, dts);
            brush.Dispose();

            Pen pen = new Pen(lineColor, 2);
            g.DrawPolygon(pen, dts);
            pen.Dispose();
            if (Id == UserProfile.Profile.InfoBasic.Position)
            {
                Image token = PicLoader.Read("System", "Token.PNG");
                int drawWidth = token.Width * Width / GameConstants.SceneTileStandardWidth;
                int drawHeight = token.Height * Height / GameConstants.SceneTileStandardHeight;
                g.DrawImage(token, X - drawWidth / 2 + Width / 8, Y - drawHeight + Height / 3, drawWidth, drawHeight);
                token.Dispose();
            }
        }
    }
}
