using System.Collections.Generic;
using System.Drawing;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;

namespace TaleofMonsters.MainItem.Scenes.SceneObjects
{ 
    internal class SceneQuest : SceneObject
    {
        public SceneQuest(int wid, int wx, int wy, int wwidth, int wheight)
        {
            Id = wid;
            X = wx;
            Y = wy;
            Width = wwidth;
            Height = wheight;
        }

        public override void Draw(Graphics g, int target)
        {
            base.Draw(g, target);

            Image markQuest = PicLoader.Read("Map", "SymQuest.PNG");
            int drawWidth = markQuest.Width * Width / GameConstants.SceneTileStandardWidth;
            int drawHeight = markQuest.Height * Height / GameConstants.SceneTileStandardHeight;
            g.DrawImage(markQuest, X - drawWidth / 2 + Width / 8, Y - drawHeight / 2, drawWidth, drawHeight);
            markQuest.Dispose();
        }
    }
}

