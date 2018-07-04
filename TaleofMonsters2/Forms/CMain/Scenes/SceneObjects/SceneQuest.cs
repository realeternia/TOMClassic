using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Scenes;

namespace TaleofMonsters.Forms.CMain.Scenes.SceneObjects
{ 
    internal class SceneQuest : SceneObject
    {
        public int EventId { get; private set; }
        public SceneQuest(int wid, int wx, int wy, int wwidth, int wheight, int info)
            :base(wid,wx,wy,wwidth,wheight)
        {
            EventId = info;
        }


        public override void MoveEnd()
        {
            base.MoveEnd();

            if (!Disabled)
                BeginEvent();
            else
                Scene.Instance.CheckALiveAndQuestState();
        }

        private void BeginEvent()
        {
            if (EventId == 0)
            {
                SetEnable(false);
                return;
            }

            PanelManager.DealPanel(new NpcTalkForm { EventId = EventId, CellId = Id });
        }

        public override bool CanBeReplaced()
        {
            SceneQuestConfig config = ConfigData.GetSceneQuestConfig(EventId);
            return config.Ename != "magnet";
        }

        public override void Draw(Graphics g, bool isTarget)
        {
            base.Draw(g, isTarget);

            if (Disabled && EventId == 0)
                return;
            string iconName = "SymQuest";
            SceneQuestConfig config = null;
            if (EventId > 0 && !HasFlag(ScenePosFlagType.SymHidden))
            {
                config = ConfigData.GetSceneQuestConfig(EventId);
                if (config.MapIcon != "")
                    iconName = config.MapIcon;
            }

            Image markQuest = PicLoader.Read("Map", iconName + ".PNG");
            int drawWidth = markQuest.Width * Width / GameConstants.SceneTileStandardWidth;
            int drawHeight = markQuest.Height * Height / GameConstants.SceneTileStandardHeight;
            var destRect = new Rectangle(X - drawWidth / 2 + Width / 8, Y - drawHeight / 2, drawWidth, drawHeight);
            if (Disabled)
            {
                g.DrawImage(markQuest, destRect, 0, 0, markQuest.Width, markQuest.Height, GraphicsUnit.Pixel, HSImageAttributes.ToGray);
                if(config != null)
                g.DrawImage(SceneQuestBook.GetSceneQuestImage(config.Id), new Rectangle(X, Y - Width / 2 + Height / 2, Width / 2, Width / 2), 0, 0, 180, 180, GraphicsUnit.Pixel, HSImageAttributes.ToGray);
            }
            else
            {
                g.DrawImage(markQuest, destRect, 0, 0, markQuest.Width, markQuest.Height, GraphicsUnit.Pixel);
                if (EventId > 0)
                {
                    if (MapSetting || HasFlag(ScenePosFlagType.Detected))
                    {
                        g.DrawImage(SceneQuestBook.GetSceneQuestImage(EventId), new Rectangle(X, Y - Width / 2 + Height / 2, Width / 2, Width / 2), 0, 0, 180, 180, GraphicsUnit.Pixel);

                        if (config != null)
                        {
                            Font font = new Font("宋体", 11*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                            g.DrawString(config.Name, font, Brushes.Black, X - drawWidth/2 + Width/8 + 2, Y - drawHeight/2 + 1);
                            g.DrawString(config.Name, font, Brushes.Wheat, X - drawWidth/2 + Width/8, Y - drawHeight/2);
                            font.Dispose();
                        }
                    }
                }
            }

            markQuest.Dispose();
        }
    }
}

