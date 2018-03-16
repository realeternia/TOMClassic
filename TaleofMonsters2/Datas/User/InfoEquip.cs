using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Tools;
using TaleofMonsters.Core;
using TaleofMonsters.Datas.User.Db;
using TaleofMonsters.Forms.CMain;

namespace TaleofMonsters.Datas.User
{
    public class InfoEquip
    {
        [FieldIndex(Index = 1)] public DbEquip[] Equipon;

        [FieldIndex(Index = 2)] public List<DbEquip> Equipoff;

        [FieldIndex(Index = 3)] public List<int> EquipComposeAvail;

        private const int MainHouseIndex = 5;

        public InfoEquip()
        {
            Equipon = new DbEquip[GameConstants.EquipOnCount];
            Equipoff = new List<DbEquip>();
            for (int i = 0; i < Equipon.Length; i++)
                Equipon[i] = new DbEquip();
            EquipComposeAvail = new List<int>();
        }

        public DbEquip GetEquipOff(int index)
        {
            if (index >=0 && index < Equipoff.Count)
                return Equipoff[index];
            return new DbEquip();
        }

        public void SetEquipOff(int index, int id, int dura, int expire)
        {
            if (index >= 0 && index < Equipoff.Count)
            {
                Equipoff[index].BaseId = id;
                Equipoff[index].Dura = dura;
                Equipoff[index].ExpireTime = expire;
            }
            else if (index < GameConstants.EquipOffCount && index == Equipoff.Count)
            {
                Equipoff.Add(new DbEquip
                {
                    BaseId = id,
                    Dura = dura,
                    ExpireTime = expire
                });
            }
        }

        public void AddEquip(int id, int minuteLast)
        {
            EquipConfig equipConfig = ConfigData.GetEquipConfig(id);
            if (equipConfig.Id == 0)
                return;
            MainTipManager.AddTip(string.Format("|获得装备-|{0}|{1}", HSTypes.I2QualityColor(equipConfig.Quality), equipConfig.Name), "White");
            UserProfile.InfoRecord.AddRecordById((int)MemPlayerRecordTypes.EquipGet, 1);

            for (int i = 0; i < Equipoff.Count; i++)
            {
                if (Equipoff[i].BaseId == 0)
                {
                    SetEquipOff(i, id, equipConfig.Durable, minuteLast <= 0 ? 0 : TimeTool.GetNowUnixTime() + minuteLast*60);
                    return;
                }
            }

            if (Equipoff.Count < GameConstants.EquipOffCount)
            {
                SetEquipOff(Equipoff.Count, id, equipConfig.Durable, minuteLast <= 0 ? 0 : TimeTool.GetNowUnixTime() + minuteLast * 60);
            }
        }

        public int GetBlankEquipPos()
        {
            int i;
            for (i = 0; i < Equipoff.Count; i++)
            {
                if (Equipoff[i].BaseId == 0)
                    return i;
            }
            if (i < GameConstants.EquipOffCount)
                return i;
            return -1;
        }

        public int GetEquipCount(int id)
        {
            int count = 0;
            for (int i = 0; i < Equipoff.Count; i++)
            {
                if (Equipoff[i].BaseId == id)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// 检查下装备耐久和过期情况，适当删除
        /// </summary>
        public void CheckExpireAndDura(bool checkOnly)
        {
            foreach (var equip in Equipon)
            {
                if (!checkOnly && equip.BaseId > 0 && equip.Dura > 0)
                    equip.Dura--;
                if (equip.BaseId > 0 && equip.Dura == 0)
                    ResetItem(equip);
                if (equip.ExpireTime > 0 && TimeTool.GetNowUnixTime() > equip.ExpireTime)
                    ResetItem(equip);
            }
            foreach (var equip in Equipoff)
            {
                if (!checkOnly && equip.BaseId > 0 && equip.Dura > 0)
                    equip.Dura--;
                if (equip.BaseId > 0 && equip.Dura == 0)
                    ResetItem(equip);
                if (equip.ExpireTime > 0 && TimeTool.GetNowUnixTime() > equip.ExpireTime)
                    ResetItem(equip);
            }
        }

        public void AddEquipCompose(int eid)
        {
            if (!EquipComposeAvail.Contains(eid))
                EquipComposeAvail.Add(eid);
        }

        public bool CanEquip(int equipId, int slotId)
        {
            //先判定slot可用性
            EquipSlotConfig slotConfig = ConfigData.GetEquipSlotConfig(slotId);
            if (slotId != MainHouseIndex)//主楼格永远可以装备
            {
                if (Equipon[MainHouseIndex - 1].BaseId == 0)
                    return false;
                EquipConfig equipConfig = ConfigData.GetEquipConfig(Equipon[MainHouseIndex-1].BaseId);
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

        public void DoEquip(int equipPos, int slotId)
        {
            var oldItem = UserProfile.InfoEquip.Equipon[equipPos];
            UserProfile.InfoEquip.Equipon[equipPos] = UserProfile.InfoEquip.Equipoff[slotId];
            UserProfile.InfoEquip.Equipoff[slotId] = oldItem;
            UserProfile.InfoDungeon.RecalculateAttr(); //会影响力量啥的属性
        }

        public void PutOff(int equipPos, int slotId)
        {
            var offEquip = UserProfile.InfoEquip.Equipon[equipPos];
            SetEquipOff(slotId, offEquip.BaseId, offEquip.Dura, offEquip.ExpireTime);
            offEquip.Reset();
            UserProfile.InfoDungeon.RecalculateAttr(); //会影响力量啥的属性
        }

        private void ResetItem(DbEquip equip)
        {
            equip.Reset();
            UserProfile.InfoDungeon.RecalculateAttr(); //会影响力量啥的属性
        }
        
        public List<int> GetValidEquipsList()
        {
            List<int> equips = new List<int>();

            for (int i = 0; i < GameConstants.EquipOnCount; i++)
            {
                var equip = Equipon[i];
                if (equip.BaseId == 0)
                    continue;

                if (CanEquip(equip.BaseId, i + 1))
                    equips.Add(equip.BaseId);
            }
            return equips;
        }
    }
}
