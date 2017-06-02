using System.Collections.Generic;
using ConfigDatas;
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

        private List<int> monsterBoostItemList;

        private AutoDictionary<int, int> monsterTypeCounts = new AutoDictionary<int, int>();//属性类型为key

        public PlayerState()
        {
            equipAddon = new Equip();
        }

        public void UpdateAttr(Equip equip, List<int> itemList)
        {
            equipAddon = equip;
            monsterBoostItemList = itemList;
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
                    if (equipAddon.Def > 0)
                        mon.Def.Source += equipAddon.Def;
                    if (equipAddon.Mag > 0)
                        mon.Mag.Source += equipAddon.Mag;
                    if (equipAddon.Spd > 0)
                        mon.Spd.Source += equipAddon.Spd;
                    if (equipAddon.Hit > 0)
                        mon.Hit.Source += equipAddon.Hit;
                    if (equipAddon.Dhit > 0)
                        mon.Dhit.Source += equipAddon.Dhit;
                    if (equipAddon.Crt > 0)
                        mon.Crt.Source += equipAddon.Crt;
                    if (equipAddon.Luk > 0)
                        mon.Luk.Source += equipAddon.Luk;
                    if (equipAddon.Range > 0)
                        mon.Avatar.Range += equipAddon.Range;
                    if (equipAddon.CommonSkillList.Count > 0)
                        mon.SkillManager.AddSkillBeforeInit(equipAddon.CommonSkillList, SkillSourceTypes.Equip);
                }
                else
                {
                    foreach (var monId in monsterBoostItemList)
                    {
                        var equipConfig = ConfigData.GetEquipConfig(monId);
                        if (equipConfig.PickMethod(mon))
                        {
                            if (equipConfig.MonsterAtk > 0)
                                mon.Atk.Source += mon.Atk.Source * equipConfig.MonsterAtk / 100;
                            if (equipConfig.MonsterHp > 0)
                            {
                                var addon = mon.MaxHp.Source*equipConfig.MonsterHp/100;
                                mon.MaxHp.Source += addon;
                                mon.AddHp(addon);//顺便把hp也加上
                            }
                        }
                    }
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
