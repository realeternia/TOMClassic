using System.Drawing;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.Forms;

namespace TaleofMonsters.MainItem.Scenes.SceneObjects
{ 
    internal class SceneQuest : SceneObject
    {
        public SceneQuest(int wid, int wx, int wy, int wwidth, int wheight, bool disabled)
            :base(wid,wx,wy,wwidth,wheight, disabled)
        {
        }

        public override bool OnClick()
        {
            if (!base.OnClick())
            {
                return false;
            }

            if (!Disabled)
            {
                BeginEvent();
                SetEnable(false);
            }

            return true;
        }

        private void BeginEvent()
        {
            NpcTalkForm sw = new NpcTalkForm();
            sw.EventId = 1;//todo
            sw.NeedBlackForm = true;
            MainForm.Instance.DealPanel(sw);
        }

        public override void Draw(Graphics g, int target)
        {
            base.Draw(g, target);

            Image markQuest = PicLoader.Read("Map", "SymQuest.PNG");
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
            markQuest.Dispose();
        }
    }
}

