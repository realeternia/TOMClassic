using ConfigDatas;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.User
{
    public class InfoEquip
    {
        [FieldIndex(Index = 1)]
        public int[] Equipon;
        [FieldIndex(Index = 2)]
        public int[] Equipoff;

        public InfoEquip()
        {
            Equipon = new int[4];
            Equipoff = new int[60];
        }

        /// <summary>
        /// 直接把一件装备穿上，初始阶段使用
        /// </summary>
        /// <param name="id"></param>
        public void DirectAddEquipOn(int id)
        {
            EquipConfig equipConfig = ConfigData.GetEquipConfig(id);
            Equipon[equipConfig.Position - 1] = id;
            OnEquipOn(id);
        }
        
        public void AddEquip(int id)
        {
            EquipConfig equipConfig = ConfigData.GetEquipConfig(id);
            MainForm.Instance.AddTip(string.Format("|获得装备-|{0}|{1}", HSTypes.I2QualityColor(equipConfig.Quality), equipConfig.Name), "White");

            for (int i = 0; i < 36; i++)
            {
                if (Equipoff[i] == 0)
                {
                    Equipoff[i] = id;
                    return;
                }
            }
        }

        public void DeleteEquip(int id)
        {
            for (int i = 0; i < 60; i++)
            {
                if (Equipoff[i] == id)
                {
                    Equipoff[i] = 0;
                    return;
                }
            }
        }

        public int GetBlankEquipPos()
        {
            int i;
            for (i = 0; i < 60; i++)
            {
                if (Equipoff[i] == 0)
                    break;
            }
            if (i == 60)//没有空格了
                return -1;
            return i;
        }

        public int GetEquipCount(int id)
        {
            int count = 0;
            for (int i = 0; i < 60; i++)
            {
                if (Equipoff[i] == id)
                {
                    count++;
                }
            }
            return count;
        }

        public void OnEquipOn(int id)
        {
            EquipConfig equipConfig = ConfigData.GetEquipConfig(id);
            if (equipConfig.Job > 0)
            {
                UserProfile.InfoBasic.Job = equipConfig.Job;
            }
        }

        public void OnEquipOff(int id)
        {
            EquipConfig equipConfig = ConfigData.GetEquipConfig(id);
            if (equipConfig.Job > 0)
            {
                UserProfile.InfoBasic.Job = JobConfig.Indexer.NewBie;//变成职业新手！
            }
        }
    }
}
