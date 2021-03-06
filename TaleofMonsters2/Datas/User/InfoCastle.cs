﻿using System;
using System.Collections.Generic;
using ConfigDatas;
using TaleofMonsters.Core;
using TaleofMonsters.Datas.Cards.Monsters;
using TaleofMonsters.Datas.Equips;
using TaleofMonsters.Datas.Items;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User.Db;
using TaleofMonsters.Forms.CMain;

namespace TaleofMonsters.Datas.User
{
    public class InfoCastle
    {
        [FieldIndex(Index = 6)] public DbEquip[] Equipon;
        [FieldIndex(Index = 7)] public List<DbEquip> EquipAvail;

        [FieldIndex(Index = 10)] public DbFarmState[] DbFarmState;
        [FieldIndex(Index = 11)] public int OreDigEp;
        [FieldIndex(Index = 12)] public int HuntMonsterId;
        [FieldIndex(Index = 13)] public int HuntHpLeft;
        [FieldIndex(Index = 14)] public int HuntPoint;
        [FieldIndex(Index = 15)] public int WealthHpLeft;
        [FieldIndex(Index = 16)] public int WealthPoint;

        private const int MainHouseIndex = 5;

        public InfoCastle()
        {
            Equipon = new DbEquip[GameConstants.EquipOnCount+1];
            EquipAvail = new List<DbEquip>();
            for (int i = 0; i < Equipon.Length; i++)
                Equipon[i] = new DbEquip();

            DbFarmState = new DbFarmState[GameConstants.PlayFarmCount];
            DbFarmState[0] = new DbFarmState(0);//初始送1片田
            for (int i = 1; i < DbFarmState.Length; i++)
                DbFarmState[i] = new DbFarmState(-1);
        }

        public void AddEquip(int eid, int expAdd)
        {
            EquipConfig equipConfig = ConfigData.GetEquipConfig(eid);

            var equip = EquipAvail.Find(edata => edata.BaseId == eid);
            if (equip != null)
            {
                equip.Exp += expAdd;
                MainTipManager.AddTip(string.Format("|装备|{0}|{1}||经验+{2}", HSTypes.I2QualityColor(equipConfig.Quality), equipConfig.Name, expAdd), "White");
            }
            else
            {
                EquipAvail.Add(new DbEquip { BaseId = eid, Level = 1 });
                MainTipManager.AddTip(string.Format("|获得装备-|{0}|{1}", HSTypes.I2QualityColor(equipConfig.Quality), equipConfig.Name), "White");
                UserProfile.InfoRecord.AddRecordById(RecordInfoConfig.Indexer.EquipGet, 1);
            }
        }

        public bool CanEquip(int equipId, int slotId)
        {
            //先判定slot可用性
            EquipSlotConfig slotConfig = ConfigData.GetEquipSlotConfig(slotId);
            if (slotId != MainHouseIndex)//主楼格永远可以装备
            {
                if (Equipon[MainHouseIndex].BaseId == 0)
                    return false;
                EquipConfig equipConfig = ConfigData.GetEquipConfig(Equipon[MainHouseIndex].BaseId);
                if (equipConfig.SlotId != null && Array.IndexOf(equipConfig.SlotId, slotId) < 0)
                    return false;
            }

            if (equipId > 0)
            {
                EquipConfig equipConfig = ConfigData.GetEquipConfig(equipId);
                return equipConfig.Position == slotConfig.Type;
            }
            return true;
        }

        public void DoEquip(int equipPos, int equipId)
        {
            if (equipPos == MainHouseIndex) //如果主楼，移除所有其他建筑
            {
                for (int i = 0; i < Equipon.Length; i++)
                    Equipon[i] = new DbEquip();
            }
            UserProfile.InfoCastle.Equipon[equipPos] = GetEquipById(equipId);
            UserProfile.InfoDungeon.RecalculateAttr(); //会影响力量啥的属性
        }

        public List<Equip> GetValidEquipsList()
        {
            List<Equip> equips = new List<Equip>();

            for (int i = 0; i < GameConstants.EquipOnCount; i++)
            {
                var equip = GetEquipOn(i+1);
                if (equip.BaseId == 0)
                    continue;

                if (CanEquip(equip.BaseId, i + 1))
                {
                    var equipD = new Equip(equip.BaseId);
                    if (equip.Level > 1)
                        equipD.UpgradeToLevel(equip.Level);
                    equips.Add(equipD);
                }
            }
            return equips;
        }

        public bool HasEquip(int id)
        {
            return EquipAvail.Find(eq => eq.BaseId == id) != null;
        }
        public bool HasEquipOn(int id)
        {
            return Array.Find(Equipon, eq => eq.BaseId == id) != null;
        }
        public DbEquip GetEquipById(int id)
        {
            return EquipAvail.Find(eq => eq.BaseId == id);
        }
        public DbEquip GetEquipOn(int id)
        {
            return Equipon[id];
        }
        public List<DbEquip> GetEquipList(int type)
        {
            var list = new List<DbEquip>();
            foreach (var dbEquip in EquipAvail)
            {
                var equipConfig = ConfigData.GetEquipConfig(dbEquip.BaseId);
                if(equipConfig.Position == type)
                    list.Add(dbEquip);
            }
            return list;
        }

        public bool AddExp(int id, int exp)
        {
            var equip = GetEquipById(id);
            if (equip != null)
            {
                equip.Exp += exp;
                var expNeed = ExpTree.GetNextRequiredEquip(equip.Level);
                if (equip.Exp >= expNeed)
                {
                    equip.Exp -= expNeed;
                    equip.Level++;
                    return true;
                }
            }
            return false;
        }

        public bool AddEp(int epAdd)
        {
            bool flag = false;
            if (HasEquipOn(HItemBook.GetItemId("eqkuang")))
            {
                OreDigEp += epAdd;
                flag = true;
            }
            if (HasEquipOn(HItemBook.GetItemId("eqtian")))
            {
                foreach (var state in DbFarmState)
                {
                    if (state.Type > 0)
                        state.Ep += epAdd;
                }
                flag = true;
            }
            if (HasEquipOn(HItemBook.GetItemId("eqliechang")))
            {
                HuntPoint+= epAdd;
                flag = true;
            }
            if (HasEquipOn(HItemBook.GetItemId("eqhaidaowan")))
            {
                WealthPoint+= epAdd;
                flag = true;
            }
            return flag;
        }

        #region 农场
        public DbFarmState GetFarmState(int id)
        {
            return DbFarmState[id];
        }

        public void SetFarmState(int id, DbFarmState state)
        {
            DbFarmState[id] = state;
        }

        public int GetFarmAvailCount()
        {
            int count = 0;
            foreach (var state in DbFarmState)
            {
                if (state.Type != -1)
                    count++;
            }
            return count;
        }

        public bool UseSeed(int type, int epNeed)
        {
            for (int i = 0; i < 9; i++)
            {
                var targetCell = DbFarmState[i];
                if (targetCell.Type == 0)
                {
                    targetCell.Type = type;
                    targetCell.Ep = 0;
                    targetCell.EpNeed = epNeed;
                    return true;
                }
            }
            return false;
        }
        #endregion

        public void RefreshHuntMonster(bool force)
        {
            if (HuntMonsterId == 0 || force || HuntHpLeft <= 0)
            {
                HuntMonsterId = MonsterBook.GetRandMonsterId();
                HuntHpLeft = ConfigData.GetMonsterConfig(HuntMonsterId).Quality*5 + 5;
            }
        }
    }
}
