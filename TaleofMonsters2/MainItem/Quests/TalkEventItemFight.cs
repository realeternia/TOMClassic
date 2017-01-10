using System;
using System.Drawing;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Peoples;
using TaleofMonsters.MainItem.Quests.SceneQuests;

namespace TaleofMonsters.MainItem.Quests
{
    internal class TalkEventItemFight : TalkEventItem
    {
        private bool isEndFight = false;

        public TalkEventItemFight(int evtId, int level, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            int enemyId = config.EnemyId;
            HsActionCallback winCallback = () => { result = evt.ChooseTarget(1); isEndFight = true; };
            HsActionCallback failCallback = () => { result = evt.ChooseTarget(0); isEndFight = true; };
            var parm = new PeopleFightParm();
            parm.Reason = PeopleFightReason.SceneQuest;
            if (evt.ParamList.Count > 1)
            {
                parm.RuleAddon = (PeopleFightRuleAddon)Enum.Parse(typeof (PeopleFightRuleAddon), evt.ParamList[0]);
                parm.RuleLevel = int.Parse(evt.ParamList[1]);
            }
            PeopleBook.Fight(enemyId, config.BattleMap, level, parm, winCallback, failCallback, failCallback);
        }

        public override void OnFrame(int tick)
        {
            if (isEndFight)
            {
                RunningState = TalkEventState.Finish;
                isEndFight = false;
            }
        }
    }
}

