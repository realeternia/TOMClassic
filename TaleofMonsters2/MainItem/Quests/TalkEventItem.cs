using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using TaleofMonsters.MainItem.Quests.SceneQuests;

namespace TaleofMonsters.MainItem.Quests
{
    internal class TalkEventItem
    {
        internal enum TalkEventState
        {
            Running, Finish
        }

        public static TalkEventItem CreateEventItem(int eventId, int level, Control c, Rectangle r, SceneQuestEvent e)
        {
            switch (e.Type)
            {
                case "roll": return new TalkEventItemRoll(eventId, level,r, e);
                case "fight": return new TalkEventItemFight(eventId, level, r, e); 
                case "reward": return new TalkEventItemReward(eventId, level, c, r, e); 
                case "punish": return new TalkEventItemPunish(eventId, level, c, r, e); 
                case "reset": return new TalkEventItemReset(eventId, level, r, e);
                case "nd": return new TalkEventItemEnd(eventId, level, r, e);
                default: return new TalkEventItem(eventId, level, r, e); 
            }
        }

        protected Rectangle pos;
        protected SceneQuestConfig config;
        protected SceneQuestEvent evt;
        protected SceneQuestBlock result;

        protected int level;

        public TalkEventState RunningState { get; set; }

        public TalkEventItem(int evtId, int l, Rectangle r, SceneQuestEvent e)
        {
            level = l;
            pos = r;
            evt = e;
            config = ConfigData.GetSceneQuestConfig(evtId);
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
         //   g.DrawRectangle(Pens.White, pos);
        }

        public virtual bool AutoClose()
        {
            return false;
        }
    }
}
