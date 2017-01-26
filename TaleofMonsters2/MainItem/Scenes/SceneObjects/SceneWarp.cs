using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.MainItem.Scenes.SceneObjects
{ 
    internal class SceneWarp : SceneObject
    {
        private int targetMap;
        private int targetPos;

        public SceneWarp(int wid, int wx, int wy, int wwidth, int wheight, int info, int info2)
            : base(wid, wx, wy, wwidth, wheight)
        {
            targetMap = info;
            targetPos = info2;
        }

        private bool CanWarp()
        {
            if (Disabled)
            {
                return false;
            }

            return true;
        }

        public override void MoveEnd()
        {
            base.MoveEnd();

            if (!CanWarp())
            {
                MainForm.Instance.AddTip(HSErrorTypes.GetDescript(HSErrorTypes.SceneLevelNeed), "Red");
                return;
            }

            UserProfile.InfoBasic.Position = targetPos;
            Scene.Instance.ChangeMap(targetMap, true);
        }

        public override bool CanBeReplaced()
        {
            return false;
        }
        public override void Draw(Graphics g, int target)
        {
            base.Draw(g, target);

            Image markQuest = PicLoader.Read("Map", "SymWarp.PNG");
            int drawWidth = markQuest.Width * Width / GameConstants.SceneTileStandardWidth;
            int drawHeight = markQuest.Height * Height / GameConstants.SceneTileStandardHeight;
            var destRect = new Rectangle(X - drawWidth / 2 + Width / 8, Y - drawHeight / 2, drawWidth, drawHeight);
            if (Disabled)
            {
                g.DrawImage(markQuest, destRect, 0, 0, markQuest.Width, markQuest.Height, GraphicsUnit.Pixel, HSImageAttributes.ToGray);
            }
            else
            {
                g.DrawImage(markQuest, destRect, 0, 0, markQuest.Width, markQuest.Height, GraphicsUnit.Pixel);
            }

            var targetName = ConfigData.GetSceneConfig(targetMap).Name;
            Font fontName = new Font("ËÎÌו", 11*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            g.DrawString(targetName, fontName, Brushes.Black, X - drawWidth / 2 + Width / 8 + 2, Y - drawHeight / 2 + 1);
            g.DrawString(targetName, fontName, Disabled ? Brushes.Gray : Brushes.Wheat, X - drawWidth / 2 + Width / 8, Y - drawHeight / 2);
            fontName.Dispose();
            markQuest.Dispose();
        }
    }
}

