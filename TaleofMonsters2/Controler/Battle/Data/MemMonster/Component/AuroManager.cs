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

        public IMonsterAuro AddAuro(int buff, int lv, string tar)
        {
            var auro = new MonsterAuro(self, buff, lv, tar);
            auroList.Add(auro);
            return auro;
        }
    }
}