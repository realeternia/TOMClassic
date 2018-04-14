using System.Drawing;
using TaleofMonsters.Core;

namespace TaleofMonsters.Datas.Effects.Facts
{
    internal abstract class BaseEffect
    {
        protected Effect effect;
        protected int frameId;
        private bool isMute;

        private int speedDownFactor;//为了可以降速播放
        private int speedRunIndex;
        public bool Repeat { get; set; }
        public string Name { get { return effect.Name; } }
        public RunState IsFinished { get; set; }

        protected BaseEffect(Effect effect, bool isMute)
        {
            this.effect = effect;
            this.isMute = isMute;
            frameId = -1;
            IsFinished = RunState.Run;
            speedDownFactor = effect.SpeedDown;
        }

        public virtual bool Next()
        {
            if (speedRunIndex == 0 && effect.SoundName != "null" && !isMute)
                SoundManager.Play("Effect", string.Format("{0}.mp3", effect.SoundName));

            speedRunIndex++;
            if ((speedRunIndex % speedDownFactor) != 0)
                return false;

            return true;
        }

        public virtual void Draw(Graphics g)
        {

        }
    }

    internal enum RunState { Run, Finished, Zombie};
}
