using System.Drawing;

namespace TaleofMonsters.DataType.Effects
{
    internal class EffectFrame
    {
        public EffectFrameUnit[] Units { get; set; }

        public void Draw(Graphics g, int x, int y, int wp, int hp)
        {
            for (int i = 0; i < Units.Length; i++)
            {
                EffectFrameUnit efu = Units[i];
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
