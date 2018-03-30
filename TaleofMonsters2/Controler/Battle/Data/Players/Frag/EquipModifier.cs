using System.Collections.Generic;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Datas;

namespace TaleofMonsters.Controler.Battle.Data.Players.Frag
{
    /// <summary>
    /// 负责装备属性对单位的影响
    /// </summary>
    internal class EquipModifier
    {
        public class EquipModifyState
        {
            public int Id;
            public int Value;
        }

        private List<EquipModifyState> addonList;
        private List<RLIdValue> skillList;

        public int CoreId { get; set; }
        public int Weapon1Id { get; set; }
        public int Weapon2Id { get; set; }
        public int Wall1Id { get; set; }
        public int Wall2Id { get; set; }

        public EquipModifier()
        {
            addonList = new List<EquipModifyState>();
            skillList = new List<RLIdValue>();
        }

        public void UpdateInfo(List<EquipModifyState> itemList, List<RLIdValue> skills)
        {
            foreach (var rlIdValue in itemList)
            {
                var checkItem = addonList.Find(p => p.Id == rlIdValue.Id);
                if (checkItem != null)
                    checkItem.Value += rlIdValue.Value;
                else
                    addonList.Add(new EquipModifyState {Id = rlIdValue.Id, Value = rlIdValue.Value});
            }
            skillList.AddRange(skills);
        }

        public int GetAttr(EquipAttrs type)
        {
            var target = addonList.Find(a => a.Id == (int) type);
            if (target == null)
                return 0;

            return target.Value;
        }

        public void CheckMonsterEvent(bool isAdd, LiveMonster mon)
        {
            if (!isAdd)
                return;

            int type = 1;
            if (mon.Type != (int)CardTypeSub.KingTower && mon.Type != (int) CardTypeSub.NormalTower)
                type = 2;

            foreach (var equipModifyState in addonList)
            {
                var addonConfig = ConfigData.GetEquipAddonConfig(equipModifyState.Id);
                if (addonConfig.Type != type)
                    continue;

                if(addonConfig.PickMethod != null && !addonConfig.PickMethod(mon))
                    continue;

                if (addonConfig.AtkP > 0)
                    mon.Atk += (int)(equipModifyState.Value);
                if (addonConfig.VitP > 0)
                    mon.MaxHp += (int)(equipModifyState.Value);
                if (addonConfig.Def > 0)
                    mon.Def += equipModifyState.Value;
                if (addonConfig.Mag > 0)
                    mon.Mag += equipModifyState.Value;
                if (addonConfig.Spd > 0)
                    mon.Spd += equipModifyState.Value;
                if (addonConfig.Hit > 0)
                    mon.Hit += equipModifyState.Value;
                if (addonConfig.Dhit > 0)
                    mon.Dhit += equipModifyState.Value;
                if (addonConfig.Crt > 0)
                    mon.Crt += equipModifyState.Value;
                if (addonConfig.Luk > 0)
                    mon.Luk += equipModifyState.Value;
                if (addonConfig.Range > 0)
                    mon.Avatar.Range += equipModifyState.Value;
            }
            mon.RefreshAttrs();
            if(mon.Hp != mon.MaxHp)
                mon.HpBar.AddHp(mon.MaxHp - mon.Hp);

            if (mon.Type == (int)CardTypeSub.KingTower)
            {
                if (skillList.Count > 0)
                    mon.SkillManager.AddSkillBeforeInit(skillList, SkillSourceTypes.Equip);
            }
        }
    }
}
