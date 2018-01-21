using System.Drawing;

namespace TaleofMonsters.DataType.Effects
{
    internal class CoverEffect
    {
        private Effect effect;
        private int frameId;
        private Point point;
        private Size size;

        private int speedDownFactor;//为了可以降速播放
        private int speedRunIndex;

        public string Name
        {
            get { return effect.Name; }
        }

        public bool PlayOnce { get; set; }
        private bool isDie;

        public CoverEffect(Effect effect, Point location, Size size)
        {
            this.effect = effect;
            point = location;
            this.size = size;
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
                    isDie = true;
                else
                    frameId = 0;    
            }
            return true;
        }

        public void Draw(Graphics g)
        {
            if (isDie)
                return;

            if (frameId >= 0 && frameId < effect.Frames.Length)
            {
                int x = point.X;
                int y = point.Y;
                effect.Frames[frameId].Draw(g, x, y, size.Width, size.Height);
            }
        }
    }
}
