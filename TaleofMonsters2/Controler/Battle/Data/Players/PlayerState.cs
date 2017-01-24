using NarlonLib.Core;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Equips;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    /// <summary>
    /// 只管战斗过程中一些状态的记录
    /// </summary>
    internal class PlayerState
    {
        public AutoDictionary<int, int> skills = new AutoDictionary<int, int>();

        private Equip equipAddon;//来自装备的属性加成

        private AutoDictionary<int, int> monsterTypeCounts = new AutoDictionary<int, int>();//属性类型为key

        public PlayerState()
        {
            equipAddon = new Equip();
        }

        public void UpdateAttr(Equip equip)
        {
            equipAddon = equip;
        }

        public void CheckMonsterEvent(bool isAdd, LiveMonster mon)
        {
            if (isAdd)
            {
                if (mon.IsHero)
                {
                    if (equipAddon.Atk > 0)
                        mon.Atk.Source += equipAddon.Atk;    
                    if (equipAddon.Hp > 0)
                    {
                        mon.MaxHp.Source += equipAddon.Hp;
                        mon.AddHp(equipAddon.Hp);//顺便把hp也加上
                    }
                    if (equipAddon.Spd > 0)
                        mon.Spd.Source += equipAddon.Spd;
                    if (equipAddon.Range > 0)
                        mon.Avatar.Range += equipAddon.Range;
                    if (equipAddon.CommonSkillId > 0)
                        mon.SkillManager.AddSkillBeforeInit(equipAddon.CommonSkillId, equipAddon.CommonSkillRate, SkillSourceTypes.Equip);
                }

                //if (Avatar.MonsterConfig.Type != (int)CardTypeSub.Hero)
                //    EAddonBook.UpdateMonsterData(this, OwnerPlayer.State.Monsterskills.Keys(), OwnerPlayer.State.Monsterskills.Values());
                monsterTypeCounts[(int) MonsterCountTypes.Total]++;
                monsterTypeCounts[mon.Attr + 10]++;
                monsterTypeCounts[mon.Type + 20]++;
            }
            else
            {
                monsterTypeCounts[(int)MonsterCountTypes.Total]--;
                monsterTypeCounts[mon.Attr + 10]--;
                monsterTypeCounts[mon.Type + 20]--;
            }
        }

        public int GetMonsterCountByType(MonsterCountTypes type)
        {
            return monsterTypeCounts[(int)type];
        }
    }
}
