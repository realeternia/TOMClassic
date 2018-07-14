using System;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Battle;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.Peoples;
using TaleofMonsters.Datas.Scenes;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain.Blesses;
using TaleofMonsters.Forms.CMain.Quests.SceneQuests;

namespace TaleofMonsters.Forms.CMain.Quests
{
    internal class TalkEventItemFight : TalkEventItem
    {
        private bool isEndFight = false;

        public TalkEventItemFight(int evtId, int level, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {

        }

        public override void Init()
        {
            HsActionCallback winCallback = OnWin;
            HsActionCallback failCallback = OnFail;
            var parm = new PeopleFightParm();
            parm.Reason = PeopleFightReason.SceneQuest;
            if (evt.ParamList.Count > 1)
            {
                parm.RuleAddon = (PeopleFightRuleAddon)Enum.Parse(typeof(PeopleFightRuleAddon), evt.ParamList[0]);
                parm.RuleLevel = int.Parse(evt.ParamList[1]);
            }
            int enemyId = 0;
            if (config.EnemyName == "check")//特殊处理标记
                enemyId = UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.SceneQuestRandPeopleId);
            else if (config.EnemyName == "fight")//特殊处理标记
                enemyId = SceneBook.GetRandomEnemy(UserProfile.InfoBasic.MapId, false);
            else if (config.EnemyName == "fighte")//特殊处理标记
                enemyId = SceneBook.GetRandomEnemy(UserProfile.InfoBasic.MapId, true);
            else
                enemyId = PeopleBook.GetPeopleId(config.EnemyName);
            int fightLevel = Math.Max(1, level + hardness + BlessManager.FightLevelChange);
            var peopleConfig = ConfigData.GetPeopleConfig(enemyId);

            PeopleBook.Fight(enemyId, peopleConfig.BattleMap, fightLevel, parm, winCallback, failCallback, failCallback);
        }

        private void OnFail()
        {
            result = evt.ChooseTarget(0);
            isEndFight = true;

            UserProfile.InfoDungeon.FightLoss ++;
            if (BlessManager.FightFailSubHealth > 0)
            {
                var healthSub = GameResourceBook.OutHealthSceneQuest(BlessManager.FightFailSubHealth * 100);
                if (healthSub > 0)
                    UserProfile.Profile.InfoBasic.SubHealth(healthSub);
            }
            if (BlessManager.FightFailSubMental > 0)
            {
                var mentalSub = GameResourceBook.OutMentalSceneQuest(BlessManager.FightFailSubMental * 100);
                if (mentalSub > 0)
                    UserProfile.Profile.InfoBasic.SubMental(mentalSub);
            }
        }

        private void OnWin()
        {
            result = evt.ChooseTarget(1);
            isEndFight = true;

            UserProfile.InfoDungeon.FightWin++;
            UserProfile.InfoGismo.CheckWinCount();
            if (BlessManager.FightWinAddHealth > 0)
            {
                var healthAdd = GameResourceBook.InHealthSceneQuest(BlessManager.FightWinAddHealth * 100);
                if (healthAdd > 0)
                    UserProfile.Profile.InfoBasic.AddHealth(healthAdd);
            }
            if (BlessManager.FightWinAddExp > 0)
            {
                var expAdd = GameResourceBook.InExpSceneQuest(level, BlessManager.FightWinAddExp * 100);
                if (expAdd > 0)
                    UserProfile.Profile.InfoBasic.AddExp((int)expAdd);
            }
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

