using System.Drawing;
using NarlonLib.Core;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType.Effects;

namespace TaleofMonsters.Controler.Battle.Data.MemEffect
{
    internal class MissileEffect : BaseEffect
    {
        public NLPointF Position { get; set; }

        public MissileEffect(Effect effect, bool isMute)
            :base(effect, isMute)
        {
            frameId = -1;
            repeat = true;
        }

        public override bool Next()
        {
            if (base.Next())
            {
                frameId++;
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

        public void Die()
        {
            IsFinished = IsFinished == RunState.Run ? RunState.Finished : RunState.Zombie;
        }

        public override void Draw(Graphics g)
        {
            if (frameId >= 0 && frameId < effect.Frames.Length)
            {
                int size = BattleManager.Instance.MemMap.CardSize;
                effect.Frames[frameId].Draw(g, (int)Position.X, (int)Position.Y, size, size);
            }
        }
    }
}
