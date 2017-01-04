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

        public static TalkEventItem CreateEventItem(int eventId, Control c, Rectangle r, SceneQuestEvent e)
        {
            switch (e.Type)
            {
                case "roll": return new TalkEventItemRoll(eventId,r, e);
                case "fight": return new TalkEventItemFight(eventId, r, e); 
                case "reward": return new TalkEventItemReward(eventId, c, r, e); 
                case "punish": return new TalkEventItemPunish(eventId, c, r, e); 
                case "reset": return new TalkEventItemReset(eventId, r, e);
                case "nd": return new TalkEventItemEnd(eventId, r, e);
                default: return new TalkEventItem(eventId, r, e); 
            }
        }

        protected Rectangle pos;
        protected SceneQuestConfig config;
        protected SceneQuestEvent evt;
        protected SceneQuestBlock result;

        public TalkEventState RunningState { get; set; }

        public TalkEventItem(int evtId, Rectangle r, SceneQuestEvent e)
        {
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
