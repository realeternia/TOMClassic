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

        public SceneWarp(int wid, int wx, int wy, int wwidth, int wheight, int info)
            : base(wid, wx, wy, wwidth, wheight)
        {
            TargetMap = info;
        }

        public override void MoveEnd()
        {
            base.MoveEnd();

            if (Disabled)
            {
                MainTipManager.AddTip(HSErrorTypes.GetDescript(HSErrorTypes.SceneWarpNeedActive), "Red");
                return;
            }

            int sceneLevel = ConfigData.GetSceneConfig(TargetMap).Level;
            if (sceneLevel > UserProfile.InfoBasic.Level)
            {
                MainTipManager.AddTip(string.Format(HSErrorTypes.GetDescript(HSErrorTypes.SceneLevelNeed), sceneLevel), "Red");
                return;
            }

            int lastMapId = UserProfile.InfoBasic.MapId;
            Scene.Instance.ChangeMap(TargetMap, true);
            UserProfile.InfoBasic.Position = Scene.Instance.GetWarpPosByMapId(lastMapId);

            Scene.Instance.OnEventFinish();
        }

        public override bool CanBeReplaced()
        {
            return false;
        }
        public override void Draw(Graphics g, bool isTarget)
        {
            base.Draw(g, isTarget);

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
            int sceneLevel = ConfigData.GetSceneConfig(TargetMap).Level;
            Brush brush = Brushes.Wheat;
            if (sceneLevel > UserProfile.InfoBasic.Level)
            {
                targetName = "等级" + sceneLevel;
                brush = Brushes.Red;
            }
            Font fontName = new Font("宋体", 11*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            g.DrawString(targetName, fontName, Brushes.Black, X - drawWidth / 2 + Width / 8 + 1, Y - drawHeight / 2 + 1);
            g.DrawString(targetName, fontName, Disabled ? Brushes.Gray : brush, X - drawWidth / 2 + Width / 8, Y - drawHeight / 2);
            fontName.Dispose();
            markQuest.Dispose();
        }
    }
}

