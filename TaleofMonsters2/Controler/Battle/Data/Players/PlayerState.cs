using ConfigDatas;
using NarlonLib.Core;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.DataType;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    /// <summary>
    /// 只管战斗过程中一些状态的记录
    /// </summary>
    internal class PlayerState
    {
        public AutoDictionary<int, int> skills = new AutoDictionary<int, int>();
        private int atkAddon; //来自装备的属性加成，加给主基地
        private int hpAddon; //来自装备的属性加成

        private AutoDictionary<int, int> MonsterTypeCounts = new AutoDictionary<int, int>();//属性类型为key

        public PlayerState()
        {
        }

        public void UpdateAttr(int atk, int life)
        {
            atkAddon = atk;
            hpAddon = life;
        }

        public void UpdateSkills(int[] sidArray, int[] svalueArray)
        {
            for (int i = 0; i < sidArray.Length; i ++)
            {
                var sid = sidArray[i];
                switch (ConfigData.GetEquipAddonConfig(sid).Type)
                {
                    //todo save the data
                }
            }
        }

        public void CheckMonsterEvent(bool isAdd, LiveMonster mon)
        {
            if (isAdd)
            {
                if (mon.IsHero)
                {
                    if (atkAddon > 0)
                    {
                        mon.Atk.Source += atkAddon;    
                    }
                    if (hpAddon > 0)
                    {
                        mon.MaxHp.Source += hpAddon;
                        mon.AddHp(hpAddon);//顺便把hp也加上
                    }
                    
                }

                //if (Avatar.MonsterConfig.Type != (int)CardTypeSub.Hero)
                //    EAddonBook.UpdateMonsterData(this, OwnerPlayer.State.Monsterskills.Keys(), OwnerPlayer.State.Monsterskills.Values());
                MonsterTypeCounts[(int) MonsterCountTypes.Total]++;
                MonsterTypeCounts[mon.Attr + 10]++;
                MonsterTypeCounts[mon.Type + 20]++;
            }
            else
            {
                MonsterTypeCounts[(int)MonsterCountTypes.Total]--;
                MonsterTypeCounts[mon.Attr + 10]--;
                MonsterTypeCounts[mon.Type + 20]--;
            }
        }

        public int GetMonsterCountByType(MonsterCountTypes type)
        {
            return MonsterTypeCounts[(int)type];
        }
    }
}
