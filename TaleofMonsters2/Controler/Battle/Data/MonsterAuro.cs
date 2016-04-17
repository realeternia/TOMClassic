using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType;

namespace TaleofMonsters.Controler.Battle.Data
{
    internal class MonsterAuro:IMonsterAuro
    {
        private LiveMonster self;
        private int buffId; //buff id
        private int level; //buff等级

        private int range = -1;
        private string target;
        private int targetMonsterId; //特定怪id

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

        public IMonsterAuro SetRange(int range)
        {
            this.range = range;
            return this;
        }

        public IMonsterAuro SetMid(int mid)
        {
            targetMonsterId = mid;
            return this;
        }

        #region IMonsterAuro 成员

        public void CheckAuroState()
        {
            int size = BattleManager.Instance.MemMap.CardSize;
            foreach (LiveMonster mon in BattleManager.Instance.MonsterQueue.Enumerator)
            {
                if (mon.IsGhost || mon.Id == self.Id)
                    continue;
                if (target[0] != 'A' && ((BattleTargetManager.IsSpellEnemyMonster(target[0]) && self.IsLeft != mon.IsLeft) || (BattleTargetManager.IsSpellFriendMonster(target[0]) && self.IsLeft == mon.IsLeft)))
                    continue;
                if (targetMonsterId != 0 && mon.Avatar.Id != targetMonsterId)
                    continue;
                if (raceList.Count > 0 && !raceList.Contains((CardTypeSub)mon.Avatar.MonsterConfig.Type))
                    continue;
                if (attrList.Count > 0 && !attrList.Contains((CardElements)mon.Avatar.MonsterConfig.Attr))
                    continue;

                BuffConfig buffConfig = ConfigData.GetBuffConfig(buffId);
                if (mon.BuffManager.HasBuff(BuffEffectTypes.NoAuro) && buffConfig.Type[1] == 'a')//=='a'表示是光环
                    continue;

                int truedis = MathTool.GetDistance(self.Position, mon.Position);
                if (range != -1 && range*size/10 <= truedis)
                    continue;

                mon.AddBuff(buffId, level, 0.05);
            }
        }

        #endregion
    }
}
