using System.Drawing;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType.Effects;

namespace TaleofMonsters.Controler.Battle.Data.MemEffect
{
    internal class FlyEffect : BaseEffect
    {
        private Point point;

        private Point start;
        private Point end;
        private int posCount;//持续多少帧
        private int posNow;

        public FlyEffect(Effect effect, Point startP, Point endP, int frameCount, bool isMute)
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
                frameId++;

                posNow++;
                if (posNow > posCount)
                {
                    IsFinished = IsFinished == RunState.Run ? RunState.Finished : RunState.Zombie;
                }
                if (frameId >= effect.Frames.Length)
                {
                    if (repeat)
                    {
                        frameId = 0;
                    }
                    else
                    {
                        IsFinished = IsFinished == RunState.Run ? RunState.Finished : RunState.Zombie;
                        frameId = effect.Frames.Length - 1;
                    }
                }
            }

            return true;
        }

        public override void Draw(Graphics g)
        {
            if (frameId >= 0 && frameId < effect.Frames.Length)
            {
                int size = BattleManager.Instance.MemMap.CardSize;
                int x = start.X+ (  end.X - start.X)*posNow/posCount;
                int y = start.Y + (end.Y - start.Y) * posNow / posCount;
                effect.Frames[frameId].Draw(g, x, y, size, size);
            }
        }
    }
}
