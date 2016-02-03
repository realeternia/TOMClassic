using System.Drawing;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.DataTent;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType.Effects;

namespace TaleofMonsters.Controler.Battle.Data.MemEffect
{
    internal class ActiveEffect : BaseEffect
    {
        private LiveMonster mon;
        private Point point;

        public ActiveEffect(Effect effect, LiveMonster mon, bool isMute)
            :base(effect, isMute)
        {
            this.mon = mon;
            frameId = -1;
        }

        public ActiveEffect(Effect effect, Point p, bool isMute)
            : base(effect, isMute)
        {
            this.mon = null;
            this.point = p;
            frameId = -1;
        }

        public override bool Next()
        {
            if (base.Next())
            {
                frameId++;
                if (repeat && mon.Life <= 0)
                {
                    IsFinished = IsFinished == RunState.Run ? RunState.Finished : RunState.Zombie;
                    frameId = effect.Frames.Length - 1;
                }
                else if (frameId >= effect.Frames.Length)
                {
                    if (repeat && mon.Life > 0)
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
                int x = ((mon == null) ? point.X - size / 2 : mon.Position.X);
                int y = ((mon == null) ? point.Y - size / 2 : mon.Position.Y);
                effect.Frames[frameId].Draw(g, x, y, size, size);
            }
        }
    }
}
