﻿using System.Collections.Generic;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Equips;

namespace TaleofMonsters.Controler.Battle.Data.Players.Frag
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
                    mon.Atk += equipAddon.Atk;    
                if (equipAddon.Hp > 0)
                {
                    mon.MaxHp += equipAddon.Hp;
                    mon.AddHp(equipAddon.Hp);//顺便把hp也加上
                }
                if (equipAddon.Def > 0)
                    mon.Def += equipAddon.Def;
                if (equipAddon.Mag > 0)
                    mon.Mag += equipAddon.Mag;
                if (equipAddon.Spd > 0)
                    mon.Spd += equipAddon.Spd;
                if (equipAddon.Hit > 0)
                    mon.Hit += equipAddon.Hit;
                if (equipAddon.Dhit > 0)
                    mon.Dhit += equipAddon.Dhit;
                if (equipAddon.Crt > 0)
                    mon.Crt += equipAddon.Crt;
                if (equipAddon.Luk > 0)
                    mon.Luk += equipAddon.Luk;
                if (equipAddon.Range > 0)
                    mon.Avatar.Range += equipAddon.Range;
                if (equipAddon.CommonSkillList.Count > 0)
                    mon.SkillManager.AddSkillBeforeInit(equipAddon.CommonSkillList, SkillSourceTypes.Equip);
            }
            else if (mon.Type == (int) CardTypeSub.NormalTower)
            {
                if (equipAddon.Atk > 0)
                    mon.Atk += (int)(equipAddon.Atk * 0.5);
                if (equipAddon.Hp > 0)
                {
                    mon.MaxHp += (int)(equipAddon.Hp * 0.5);
                    mon.AddHp(equipAddon.Hp * 0.5);//顺便把hp也加上
                }
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
                                mon.Atk += mon.Atk*equipConfig.MonsterAtk/100;
                            if (equipConfig.MonsterHp > 0)
                            {
                                var addon = mon.MaxHp*equipConfig.MonsterHp/100;
                                mon.MaxHp += addon;
                                mon.AddHp(addon); //顺便把hp也加上
                            }
                        }
                    }
                }
            }
        }
    }
}
