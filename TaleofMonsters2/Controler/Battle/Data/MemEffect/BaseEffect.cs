using System.Drawing;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Effects;

namespace TaleofMonsters.Controler.Battle.Data.MemEffect
{
    internal abstract class BaseEffect
    {
        protected Effect effect;
        protected int frameId;
        private bool isMute;
        protected bool repeat;

        private int speedDownFactor;//为了可以降速播放
        private int speedRunIndex;

        public BaseEffect(Effect effect, bool isMute)
        {
            this.effect = effect;
            this.isMute = isMute;
            frameId = -1;
            IsFinished = RunState.Run;
            speedDownFactor = effect.SpeedDown;
        }

        public virtual bool Next()
        {
            if (frameId == 0 && effect.SoundName != "null" && !isMute)
            {
                SoundManager.Play("Effect", string.Format("{0}.mp3", effect.SoundName));
            }

            speedRunIndex++;
            if ((speedRunIndex % speedDownFactor) != 0)
            {
                IsFinished = RunState.Run;
                return false;
            }

            return true;
        }

        public RunState IsFinished { get; set; }

        public bool Repeat
        {
            set { repeat = value; }
        }

        public virtual void Draw(Graphics g)
        {

        }
    }

    internal enum RunState { Run, Finished, Zombie};
}
