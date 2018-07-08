using System;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.User;

namespace TaleofMonsters.Forms.CMain.Scenes.SceneObjects
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
                MainTipManager.AddTip(HSErrors.GetDescript(ErrorConfig.Indexer.SceneWarpNeedActive), "Red");
                return;
            }

            int lastMapId = UserProfile.InfoBasic.MapId;
            Scene.Instance.ChangeMap(TargetMap, true);
            Scene.Instance.MoveTo(Scene.Instance.GetWarpPosByMapId(lastMapId));

            Scene.Instance.CheckALiveAndQuestState();
        }

        public override bool CanBeReplaced()
        {
            return false;
        }

        public override void OnTick()
        {
            base.OnTick();

            if (Disabled || (tickCount % 3) != 0)//降频，降低cpu开销
                return;

            int drawWidth = 85 * Width / GameConstants.SceneTileStandardWidth;
            int drawHeight = 61 * Height / GameConstants.SceneTileStandardHeight;
            var destRect = new Rectangle(X - drawWidth / 2 + Width / 8, Y - drawHeight / 2, drawWidth, drawHeight);

            MainForm.Instance.RefreshView(destRect);
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
                var size = Math.Sin((float)tickCount/4) + 4;
                destRect = new Rectangle(destRect.X+(int)(destRect.Width * (0.5 - size / 5 / 2)), destRect.Y+(int)(destRect.Height * (0.5 - size / 5 / 2)), (int)(destRect.Width * size / 5), (int)(destRect.Height * size / 5));
                g.DrawImage(markQuest, destRect, 0, 0, markQuest.Width, markQuest.Height, GraphicsUnit.Pixel);
            }

            var targetName = ConfigData.GetSceneConfig(TargetMap).Name;
            Font fontName = new Font("宋体", 11*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            g.DrawString(targetName, fontName, Brushes.Black, X - drawWidth / 2 + Width / 8 + 10, Y - drawHeight / 2 + 1);
            g.DrawString(targetName, fontName, Disabled ? Brushes.Gray : Brushes.Wheat, X - drawWidth / 2 + Width / 8 + 8, Y - drawHeight / 2);
            fontName.Dispose();
            markQuest.Dispose();
        }
    }
}

