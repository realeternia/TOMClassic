using System.Collections.Generic;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Equips;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    /// <summary>
    /// 负责装备属性对单位的影响
    /// </summary>
    internal class EquipModifier
    {
        private Equip equipAddon;//来自装备的属性加成

        private List<int> monsterBoostItemList;

        public int CoreId { get; set; }
        public int Weapon1Id { get; set; }
        public int Weapon2Id { get; set; }
        public int Wall1Id { get; set; }
        public int Wall2Id { get; set; }

        public EquipModifier()
        {
            equipAddon = new Equip();
        }

        public void UpdateInfo(Equip equip, List<int> itemList)
        {
            equipAddon = equip;
            monsterBoostItemList = itemList;
        }

        public void CheckMonsterEvent(bool isAdd, LiveMonster mon)
        {
            if (!isAdd)
                return;

            if (mon.Type == (int)CardTypeSub.KingTower)
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

                mon.Atk.Locked = true;
                mon.MaxHp.Locked = true;
                mon.Def.Locked = true;
                mon.Mag.Locked = true;
                mon.Spd.Locked = true;
                mon.Hit.Locked = true;
                mon.Dhit.Locked = true;
                mon.Crt.Locked = true;
                mon.Luk.Locked = true;
            }
            else if (mon.Type == (int) CardTypeSub.NormalTower)
            {
            }
            else
            {
                if (monsterBoostItemList != null)
                {
                    foreach (var monId in monsterBoostItemList)
                    {
                        var equipConfig = ConfigData.GetEquipConfig(monId);
                        if (equipConfig.PickMethod(mon))
                        {
                            if (equipConfig.MonsterAtk > 0)
                                mon.Atk.Source += mon.Atk.Source*equipConfig.MonsterAtk/100;
                            if (equipConfig.MonsterHp > 0)
                            {
                                var addon = mon.MaxHp.Source*equipConfig.MonsterHp/100;
                                mon.MaxHp.Source += addon;
                                mon.AddHp(addon); //顺便把hp也加上
                            }
                        }
                    }
                }
            }
        }
    }
}
