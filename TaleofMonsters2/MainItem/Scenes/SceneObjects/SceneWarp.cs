using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.MainItem.Scenes.SceneObjects
{ 
    internal class SceneWarp : SceneObject
    {
        public int TargetMap { get; private set; }

        public SceneWarp(int wid, int wx, int wy, int wwidth, int wheight, int info, int info2)
            : base(wid, wx, wy, wwidth, wheight)
        {
            TargetMap = info;
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

            int lastMapId = UserProfile.InfoBasic.MapId;
            Scene.Instance.ChangeMap(TargetMap, true);
            UserProfile.InfoBasic.Position = Scene.Instance.GetWarpPosByMapId(lastMapId);
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

            var targetName = ConfigData.GetSceneConfig(TargetMap).Name;
            Font fontName = new Font("ËÎÌו", 11*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            g.DrawString(targetName, fontName, Brushes.Black, X - drawWidth / 2 + Width / 8 + 2, Y - drawHeight / 2 + 1);
            g.DrawString(targetName, fontName, Disabled ? Brushes.Gray : Brushes.Wheat, X - drawWidth / 2 + Width / 8, Y - drawHeight / 2);
            fontName.Dispose();
            markQuest.Dispose();
        }
    }
}

