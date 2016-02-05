using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.Resource;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.DataType.Equips
{
    static class EquipBook
    {
        static public Image GetEquipImage(int id)
        {
            string fname = string.Format("Equip/{0}.JPG", ConfigData.GetEquipConfig(id).Url);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Equip", string.Format("{0}.JPG", ConfigData.GetEquipConfig(id).Url));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }

        public static int[] GetCanMergeId()
        {
            List<int> datas = new List<int>();
            foreach (EquipConfig equipConfig in ConfigData.EquipDict.Values)
            {
                datas.Add(equipConfig.Id);//返回所有
            }
            return datas.ToArray();
        }

        static public bool CanEquip(int id)
        {
            EquipConfig equipConfig = ConfigData.GetEquipConfig(id);

            if (UserProfile.InfoBasic.Level < equipConfig.LvNeed)
            {
                return false;
            }

            return true;
        }

        public static List<Equip> GetEquipsList(int[] equipIds)
        {
            List<Equip> equips = new List<Equip>();

            foreach (int eid in equipIds)
            {
                if (eid == 0)
                {
                    continue;
                }

                equips.Add(new Equip(eid));
            }
            return equips;
        }


        public static Equip GetVirtualEquips(List<Equip> equipList)
        {
            var vEquip = new Equip();
            foreach (var equip in equipList)
            {
                vEquip.Atk += equip.Atk;
                vEquip.Def += equip.Def;
                vEquip.Mag += equip.Mag;
                vEquip.Spd += equip.Spd;
                vEquip.Hp += equip.Hp;
            }
            return vEquip;
        }
    }
}
