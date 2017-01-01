using System.Drawing;
using TaleofMonsters.MainItem.Quests.SceneQuests;

namespace TaleofMonsters.MainItem.Quests
{
    internal class TalkEventItem
    {
        internal enum TalkEventState
        {
            Running, Finish
        }

        public static TalkEventItem CreateEventItem(Rectangle r, SceneQuestEvent e)
        {
            switch (e.Type)
            {
                case "roll": return new TalkEventItemRoll(r, e);break;
                default: return new TalkEventItem(r, e); break;
            }
        }

        protected Rectangle pos;
        protected SceneQuestEvent evt;
        protected SceneQuestBlock result;

        public TalkEventState RunningState { get; set; }

        public TalkEventItem(Rectangle r, SceneQuestEvent e)
        {
            pos = r;
            evt = e;
            RunningState = TalkEventState.Running;
        }


        public SceneQuestBlock GetResult()
        {
            return result;
        }

        public virtual void OnFrame(int tick)
        {
            RunningState = TalkEventState.Finish;
        }

        public virtual void Draw(Graphics g)
        {
            g.DrawRectangle(Pens.White, pos);
        }
    }
}
