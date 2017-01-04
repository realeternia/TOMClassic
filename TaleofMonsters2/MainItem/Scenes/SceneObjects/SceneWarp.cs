using System.Collections.Generic;
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

        public SceneWarp(int wid, int wx, int wy, int wwidth, int wheight, bool disabled, int info)
            : base(wid, wx, wy, wwidth, wheight, disabled)
        {
            targetMap = info;
        }

        private bool CanWarp()
        {
            SceneConfig sceneConfig = ConfigData.GetSceneConfig(targetMap);
            if (UserProfile.InfoBasic.Level < sceneConfig.Level)
            {
                return false;
            }

            return true;
        }

        public override bool OnClick()
        {
            if (!base.OnClick())
            {
                return false;
            }

            if (!CanWarp())
            {
                SceneConfig sceneConfig = ConfigData.GetSceneConfig(targetMap);
                string err = string.Format(HSErrorTypes.GetDescript(HSErrorTypes.SceneLevelNeed), sceneConfig.Level);
                MainForm.Instance.AddTip(err, "Red");
                return false;
            }

            Scene.Instance.ChangeMap(targetMap, true);

            return true;
        }

        public override void Draw(Graphics g, int target)
        {
            base.Draw(g, target);

            Image markQuest = PicLoader.Read("Map", "SymWarp.PNG");
            int drawWidth = markQuest.Width * Width / GameConstants.SceneTileStandardWidth;
            int drawHeight = markQuest.Height * Height / GameConstants.SceneTileStandardHeight;
            g.DrawImage(markQuest, X - drawWidth / 2 + Width / 8, Y - drawHeight / 2, drawWidth, drawHeight);
            markQuest.Dispose();
        }
    }
}

