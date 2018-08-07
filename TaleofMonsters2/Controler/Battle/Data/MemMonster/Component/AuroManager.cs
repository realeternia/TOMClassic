using System.Collections.Generic;
using ConfigDatas;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster.Component
{
    internal class AuroManager
    {
        private LiveMonster self;
        private List<MonsterAuro> auroList = new List<MonsterAuro>();//光环

        public AuroManager(LiveMonster mon)
        {
            self = mon;
        }

        public void Reload()
        {
            auroList.Clear();
        }

        public void CheckAuroEffect()
        {
            foreach (var auro in auroList)
                auro.CheckAuroState();
        }

        public IMonsterAuro AddAuro(ISkill skill)
        {
            var auro = new MonsterAuro(self, skill);
            auroList.Add(auro);
            return auro;
        }
    }
}