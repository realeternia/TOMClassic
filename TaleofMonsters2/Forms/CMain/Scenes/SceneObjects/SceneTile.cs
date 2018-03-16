using System.Drawing;

namespace TaleofMonsters.Forms.CMain.Scenes.SceneObjects
{
    internal class SceneTile : SceneObject
    {
        public SceneTile(int wid, int wx, int wy, int wwidth, int wheight)
            : base(wid, wx, wy, wwidth, wheight)
        {
        }
        public override void MoveEnd()
        {
            base.MoveEnd();
            Scene.Instance.CheckALiveAndQuestState();
        }

        public override void Draw(Graphics g, bool isTarget)
        {
            base.Draw(g, isTarget);

#if DEBUG
            Font font = new Font("微软雅黑", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            g.DrawString(Id.ToString(), font, Brushes.Black, X + 1, Y + 1);
            g.DrawString(Id.ToString(), font, Brushes.White, X, Y);
            font.Dispose();
#endif
        }
    }
}
