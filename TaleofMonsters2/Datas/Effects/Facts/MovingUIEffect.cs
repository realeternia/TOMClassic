using System.Drawing;

namespace TaleofMonsters.Datas.Effects.Facts
{
    /// <summary>
    /// 一边播放一边移动的特效
    /// </summary>
    internal class MovingUIEffect : BaseEffect
    {
        private Point start;
        private Point end;
        private int posCount;//持续多少帧
        private int posNow;

        public MovingUIEffect(Effect effect, Point startP, Point endP, int frameCount, bool isMute)
            :base(effect, isMute)
        {
            start = startP;
            end = endP;
            posCount = frameCount;
            Repeat = true;

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
                int x = start.X + (end.X - start.X)*posNow/posCount;
                int y = start.Y + (end.Y - start.Y)*posNow/posCount;
                effect.Frames[frameId].Draw(g, x, y, size, size);
            }
        }
    }
}
