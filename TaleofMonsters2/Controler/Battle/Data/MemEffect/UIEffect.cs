using System.Drawing;
using TaleofMonsters.Datas.Effects;

namespace TaleofMonsters.Controler.Battle.Data.MemEffect
{
    internal class UIEffect : BaseEffect
    {
        private Point start;
        private Point end;
        private int posCount;//持续多少帧
        private int posNow;

        public UIEffect(Effect effect, Point startP, Point endP, int frameCount, bool isMute)
            :base(effect, isMute)
        {
            start = startP;
            end = endP;
            posCount = frameCount;
            frameId = -1;
            repeat = true;

            posNow = 0;
        }

        public override bool Next()
        {
            if (base.Next())
            {
                frameId = posNow * effect.Frames.Length / posCount;

                posNow++;
                if (posNow > posCount)
                {
                    IsFinished = IsFinished == RunState.Run ? RunState.Finished : RunState.Zombie;
                }
                if (frameId >= effect.Frames.Length)
                {
                    frameId = effect.Frames.Length - 1;
                }
            }

            return true;
        }

        public override void Draw(Graphics g)
        {
            if (frameId >= 0 && frameId < effect.Frames.Length)
            {
                int size = 100; //都按照100的尺寸来画
                int x = start.X+ (  end.X - start.X)*posNow/posCount;
                int y = start.Y + (end.Y - start.Y) * posNow / posCount;
                effect.Frames[frameId].Draw(g, x, y, size, size);
            }
        }
    }
}
