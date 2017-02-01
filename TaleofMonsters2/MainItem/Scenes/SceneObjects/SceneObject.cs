using System.Drawing;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User;
using TaleofMonsters.MainItem.Blesses;

namespace TaleofMonsters.MainItem.Scenes.SceneObjects
{
    internal abstract class SceneObject
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

        public void SetEnable(bool isEnable)
        {
            Disabled = !isEnable;

            UserProfile.Profile.InfoWorld.UpdatePosEnable(Id, isEnable);
        }

        public void SetMapSetting(bool isSet)
        {
            MapSetting = isSet;

            UserProfile.Profile.InfoWorld.UpdatePosMapSetting(Id, isSet);
        }

        public bool IsMouseIn(int mx, int my)
        {
            if (my < Y + Height / 2 - Height || my > Y + Height / 2)
                return false;

            int xDiff = (my - (Y + Height/2))*(int) (Width*GameConstants.SceneTileGradient)/Height;
            if (mx < X - Width/2 - xDiff || mx > X + Width/2 - xDiff)
                return false;
            return true;
        }

        public virtual bool OnClick()
        {
            if (!SceneManager.CanMove(Id, UserProfile.Profile.InfoBasic.Position))
            {
                return false;
            }

            uint moveCost = BlessManager.GetSceneMove(GameConstants.SceneMoveCost);
            if (UserProfile.Profile.InfoBasic.FoodPoint >= moveCost)
            {
                UserProfile.InfoBasic.SubFood(moveCost);
                UserProfile.InfoBasic.AddHealth(moveCost);
            }
            else
            {
                UserProfile.InfoBasic.SubHealth(moveCost * 2);
            }
            

            return true;
        }

        public virtual void MoveEnd()
        {
            UserProfile.Profile.InfoBasic.Position = Id;

            BlessManager.OnMove();
        }

        public virtual bool CanBeReplaced()
        {
            return true;
        }

        public virtual void Draw(Graphics g, int target)
        {
            Color tileColor = Color.White;
            Color lineColor = Color.DimGray;

            if (target == Id)
            {
                if (SceneManager.CanMove(Id, UserProfile.Profile.InfoBasic.Position))
                {
                    tileColor = Color.Yellow;
                    lineColor = Color.Orange;
                }
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
        }
    }
}
