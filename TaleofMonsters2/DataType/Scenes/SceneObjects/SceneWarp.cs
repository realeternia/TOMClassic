using System.Drawing;
using ConfigDatas;
using TaleofMonsters.DataType.User;
using TaleofMonsters.MainItem;

namespace TaleofMonsters.DataType.Scenes.SceneObjects
{
    class SceneWarp : SceneObject
    {
        public SceneWarp(int wid, int wx, int wy, int wmap)
        {
            Id = wid; 
            X = wx;
            Y = wy;
            Width = 60;
            Height = 60;
            TargetMap = wmap;
            Figue = "warp";
            Name = ConfigData.GetSceneConfig(wmap).Name;
        }

        public int TargetMap { get; private set; }

        public override string Figue
        {
            get
            {
                if (CanWarp())
                {
                    return base.Figue;
                }
                return "warperr";
            }
        }

        private bool CanWarp()
        {
            SceneConfig sceneConfig = ConfigData.GetSceneConfig(TargetMap);
            if (UserProfile.InfoBasic.Level < sceneConfig.Level)
            {
                return false;
            }

            return true;
        }

        public override void CheckClick()
        {
            string err = "";

            SceneConfig sceneConfig = ConfigData.GetSceneConfig(TargetMap);
            if (UserProfile.InfoBasic.Level < sceneConfig.Level)
            {
                err = string.Format("需要等级到达{0}级", sceneConfig.Level);
            }
            if (err != "")
            {
                MainForm.Instance.AddTip(err, "Red");
                return;
            }

            Scene.Instance.ChangeMap(TargetMap);
        }

        public override void Draw(Graphics g, int target)
        {
            base.Draw(g, target);

#if DEBUG
            Font font = new Font("微软雅黑", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            g.DrawString(Id.ToString(), font, Brushes.Black, X + 3, Y + 47);
            g.DrawString(Id.ToString(), font, Brushes.White, X, Y + 45);
            font.Dispose();
#endif
        }
    }
}
