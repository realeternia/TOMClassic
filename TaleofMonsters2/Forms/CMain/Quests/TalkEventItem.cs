using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.CMain.Quests.SceneQuests;

namespace TaleofMonsters.Forms.CMain.Quests
{
    internal class TalkEventItem : IDisposable
    {
        internal enum TalkEventState
        {
            Running, Finish
        }

        public static TalkEventItem CreateEventItem(int cellId, int eventId, int level, Control c, Rectangle r, SceneQuestEvent e)
        {
            switch (e.Type)
            {
                case "roll": return new TalkEventItemRoll(eventId, level,r, e);
                case "choose": return new TalkEventItemChoose(eventId, level, c, r, e);
                case "fight": return new TalkEventItemFight(eventId, level, r, e);
                case "game": return new TalkEventItemGame(eventId, level, r, e);
                case "reward": return new TalkEventItemReward(eventId, level, c, r, e);
                case "rewardq": return new TalkEventItemRewardQuest(eventId, level, c, r, e);
                case "trade": return new TalkEventItemTrade(eventId, level, c, r, e); 
                case "punish": return new TalkEventItemPunish(eventId, level, c, r, e);
                case "test": return new TalkEventItemTest(eventId, level, r, e);
                case "pay": return new TalkEventItemPay(eventId, level, c, r, e);
                case "npc": return new TalkEventItemNpc(eventId, level, r, e);
                case "nd": return new TalkEventItemEnd(eventId, level, r, e);
                default: return new TalkEventItemAction(cellId, eventId, level, r, e); 
            }
        }

        protected Rectangle pos;
        protected SceneQuestConfig config;
        protected SceneQuestEvent evt;
        protected SceneQuestBlock result;

        protected int level;
        protected int hardness; //难度改变

        public TalkEventState RunningState { get; set; }
        public int Id { get { return config.Id; } }
        public string Type { get { return evt.Type; } }
        protected bool inited;

        public TalkEventItem(int evtId, int l, Rectangle r, SceneQuestEvent e)
        {
            level = l;
            pos = r;
            evt = e;
            config = ConfigData.GetSceneQuestConfig(evtId);
            if (config.TriggerDNAHard != null && config.TriggerDNAHard.Length > 0)
            {
                for (int i = 0; i < config.TriggerDNAHard.Length; i++)
                {
                    if (UserProfile.InfoBasic.HasDna(DnaBook.GetDnaId(config.TriggerDNAHard[i])))
                        hardness += DnaBook.GetDnaEffect(config.TriggerDNAHard[i]);
                }
            }
            RunningState = TalkEventState.Running;
        }

        public virtual void Init()
        {
            inited = true;
        }

        public SceneQuestBlock GetResult()
        {
            return result;
        }

        public virtual void OnFrame(int tick)
        {
            if (inited)
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

        public virtual void Dispose()
        {

        }
    }
}
