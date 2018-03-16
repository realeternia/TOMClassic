using System;
using System.Collections.Generic;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Datas;

namespace TaleofMonsters.Controler.Battle.Data
{
    internal class MonsterAuro:IMonsterAuro
    {
        private LiveMonster self;
        private int buffId; //buff id
        private int level; //buff等级

        private string target;
        private int targetMonsterId; //特定怪id
        private int starMin = 0; //最小星级影响
        private int starMax = 10; //最大星级影响

        private List<CardTypeSub> raceList = new List<CardTypeSub>();
        private List<CardElements> attrList = new List<CardElements>();

        public MonsterAuro(LiveMonster mon, int buff, int lv, string tar)
        {
            self = mon;
            buffId = buff;
            level = lv;
            target = tar;
        }

        public IMonsterAuro AddRace(string race)
        {
            raceList.Add((CardTypeSub)Enum.Parse(typeof(CardTypeSub), race));
            return this;
        }

        public IMonsterAuro AddAttr(string attr)
        {
            attrList.Add((CardElements)Enum.Parse(typeof(CardElements), attr));
            return this;
        }

        public IMonsterAuro SetMid(int mid)
        {
            targetMonsterId = mid;
            return this;
        }

        public IMonsterAuro SetStar(int min, int max)
        {
            starMin = min;
            starMax = max;
            return this;
        }

        #region IMonsterAuro 成员

        public void CheckAuroState()
        {
            foreach (LiveMonster mon in BattleManager.Instance.MonsterQueue.Enumerator)
            {
                if (mon.IsGhost || mon.Id == self.Id)
                    continue;
                if (target[0] != 'A'
                    && ((BattleTargetManager.IsSpellEnemyMonster(target[0]) && self.IsLeft == mon.IsLeft)
                    || (BattleTargetManager.IsSpellFriendMonster(target[0]) && self.IsLeft != mon.IsLeft)))
                    continue;
                if (targetMonsterId != 0 && mon.Avatar.Id != targetMonsterId)
                    continue;
                if (mon.Star > starMax || mon.Star < starMin)
                    continue;
                if (raceList.Count > 0 && !raceList.Contains((CardTypeSub)mon.Avatar.MonsterConfig.Type))
                    continue;
                if (attrList.Count > 0 && !attrList.Contains((CardElements)mon.Avatar.MonsterConfig.Attr))
                    continue;

                mon.BuffManager.AddBuff(buffId, level, 0.05);
            }
        }

        #endregion
    }
}
