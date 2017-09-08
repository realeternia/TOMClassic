using System.Drawing;

namespace TaleofMonsters.DataType.Effects
{
    internal class EffectFrame
    {
        public struct EffectFrameUnit
        {
            public int Frameid;
            public int X;
            public int Y;
            public int Height;
            public int Width;
            public int Effect;
        }

        public EffectFrameUnit[] Units { get; set; }

        public void Draw(Graphics g, int x, int y, int wp, int hp)
        {
            for (int i = 0; i < Units.Length; i++)
            {
                EffectFrameUnit efu = Units[i];
                int ewid = efu.Width*wp/100;
                int eheg = efu.Height*hp/100;
                var img = EffectBook.GetEffectImage(efu.Frameid.ToString(), efu.Effect, false);
                if (img != null)
                {
                    g.DrawImage(img, x + efu.X * wp / 100, y + efu.Y * hp / 100, ewid, eheg);
                }
            }
        }
    }
}
