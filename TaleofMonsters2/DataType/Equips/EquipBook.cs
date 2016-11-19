using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.Resource;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.DataType.Equips
{
    internal static class EquipBook
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

        public static int[] GetCanMergeId(int level)
        {
            List<int> datas = new List<int>();
            foreach (var equipConfig in ConfigData.EquipDict.Values)
            {
                if (equipConfig.LvNeed > level + 5 || equipConfig.LvNeed < level - 5)
                    continue;

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
                vEquip.Hp += equip.Hp;
                vEquip.MpRate += equip.MpRate;
                vEquip.PpRate += equip.PpRate;
                vEquip.LpRate += equip.LpRate;
            }
            return vEquip;
        }
    }
}
