using System.Drawing;

namespace TaleofMonsters.MainItem.Scenes.SceneObjects
{
    internal class SceneTile : SceneObject
    {
        public SceneTile(int wid, int wx, int wy, int wwidth, int wheight)
            : base(wid, wx, wy, wwidth, wheight)
        {
        }

        public override void Draw(Graphics g, int target)
        {
            base.Draw(g, target);

#if DEBUG
            Font font = new Font("Î¢ÈíÑÅºÚ", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            g.DrawString(Id.ToString(), font, Brushes.Black, X + 3, Y + 3);
            g.DrawString(Id.ToString(), font, Brushes.White, X, Y);
            font.Dispose();
#endif
        }
    }
}
