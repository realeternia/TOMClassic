using System;
using ConfigDatas;
using NarlonLib.Core;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User.Db;

namespace TaleofMonsters.DataType.User
{
    public class InfoEquip
    {
        [FieldIndex(Index = 1)] public DbEquip[] Equipon;

        [FieldIndex(Index = 2)] public DbEquip[] Equipoff;

        public InfoEquip()
        {
            Equipon = new DbEquip[GameConstants.EquipOnCount];
            Equipoff = new DbEquip[GameConstants.EquipOffCount];
            for (int i = 0; i < Equipon.Length; i++)
                Equipon[i] = new DbEquip();
            for (int i = 0; i < Equipoff.Length; i++)
                Equipoff[i] = new DbEquip();
        }
        
        public void AddEquip(int id, int minuteLast)
        {
            EquipConfig equipConfig = ConfigData.GetEquipConfig(id);
            MainForm.Instance.AddTip(string.Format("|获得装备-|{0}|{1}", HSTypes.I2QualityColor(equipConfig.Quality), equipConfig.Name), "White");

            for (int i = 0; i < GameConstants.EquipOffCount; i++)
            {
                if (Equipoff[i].BaseId == 0)
                {
                    Equipoff[i].BaseId = id;
                    Equipoff[i].Dura = equipConfig.Durable;
                    Equipoff[i].ExpireTime = minuteLast <= 0 ? 0 : TimeTool.GetNowUnixTime() + minuteLast*60;
                    return;
                }
            }
        }

        //public void DeleteEquip(int id)
        //{
        //    for (int i = 0; i < GameConstants.EquipOffCount; i++)
        //    {
        //        if (Equipoff[i] == id)
        //        {
        //            Equipoff[i] = 0;
        //            return;
        //        }
        //    }
        //}

        public int GetBlankEquipPos()
        {
            int i;
            for (i = 0; i < GameConstants.EquipOffCount; i++)
            {
                if (Equipoff[i].BaseId == 0)
                    break;
            }
            if (i == GameConstants.EquipOffCount)//没有空格了
                return -1;
            return i;
        }

        public int GetEquipCount(int id)
        {
            int count = 0;
            for (int i = 0; i < GameConstants.EquipOffCount; i++)
            {
                if (Equipoff[i].BaseId == id)
                {
                    count++;
                }
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
                    equip.Reset();
                if (equip.ExpireTime > 0 && TimeTool.GetNowUnixTime() > equip.ExpireTime)
                    equip.Reset();
            }
            foreach (var equip in Equipoff)
            {
                if (!checkOnly && equip.BaseId > 0 && equip.Dura > 0)
                    equip.Dura--;
                if (equip.BaseId > 0 && equip.Dura == 0)
                    equip.Reset();
                if (equip.ExpireTime > 0 && TimeTool.GetNowUnixTime() > equip.ExpireTime)
                    equip.Reset();
            }
        }
    }
}
