using System.Drawing;

namespace TaleofMonsters.Datas.Effects
{
    internal class CoverEffect
    {
        private Effect effect;
        private int frameId;
        public Point Point { get; private set; }
        public Size Size { get; private set; }

        private int speedDownFactor;//为了可以降速播放
        private int speedRunIndex;

        public string Name
        {
            get { return effect.Name; }
        }

        public bool PlayOnce { get; set; }
        public bool IsDie { get; private set; }

        public CoverEffect(Effect effect, Point location, Size size)
        {
            this.effect = effect;
            Point = location;
            Size = size;
            frameId = -1;
            speedDownFactor = effect.SpeedDown;
        }

        public bool Next()
        {
            speedRunIndex++;
            if ((speedRunIndex % speedDownFactor) != 0)
                return false;

            frameId++;
            if (frameId >= effect.Frames.Length)
            {
                if (PlayOnce)
                    IsDie = true;
                else
                    frameId = 0;    
            }
            return true;
        }

        public void Draw(Graphics g)
        {
            if (IsDie)
                return;

            if (frameId >= 0 && frameId < effect.Frames.Length)
            {
                int x = Point.X;
                int y = Point.Y;
                effect.Frames[frameId].Draw(g, x, y, Size.Width, Size.Height);
            }
        }
    }
}
