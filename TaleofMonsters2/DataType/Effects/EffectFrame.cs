using System.Drawing;

namespace TaleofMonsters.DataType.Effects
{
    internal class EffectFrame
    {
        private EffectFrameUnit[] units;

        public EffectFrameUnit[] Units
        {
            get { return units; }
            set { units = value; }
        }

        public void Draw(Graphics g, int x, int y, int wp, int hp)
        {
            for (int i = 0; i < units.Length; i++)
            {
                EffectFrameUnit efu = units[i];
                int ewid = efu.width*wp/100;
                int eheg = efu.height*hp/100;
                var img = EffectBook.GetEffectImage(efu.frameid.ToString(), efu.effect, false);
                if (img != null)
                {
                    g.DrawImage(img, x + efu.x * wp / 100, y + efu.y * hp / 100, ewid, eheg);
                }
            }
        }
    }
}
