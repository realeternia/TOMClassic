using System.Collections.Generic;
using TaleofMonsters.Controler.Battle.Data.MemEffect;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType.Effects;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster.Component
{
    internal class MonsterCoverBox
    {
        private LiveMonster liveMonster;
        private List<ActiveEffect> coverEffectList = new List<ActiveEffect>();//变身时需要重算

        public MonsterCoverBox(LiveMonster liveMonster)
        {
            CheckCover();
            this.liveMonster = liveMonster;
        }

        public void CheckCover()
        {
            string cover = liveMonster.Avatar.MonsterConfig.Cover;
            if (!string.IsNullOrEmpty(cover))
            {
                ActiveEffect ef = new ActiveEffect(EffectBook.GetEffect(cover), liveMonster, true);
                ef.Repeat = true;
                BattleManager.Instance.EffectQueue.Add(ef);
                coverEffectList.Add(ef);
            }

            liveMonster.SkillManager.CheckCover(coverEffectList);
        }

        public void RemoveAllCover()
        {
            foreach (var activeEffect in coverEffectList)
            {
                activeEffect.IsFinished = RunState.Finished;
            }
            coverEffectList.Clear();
        }
    }
}