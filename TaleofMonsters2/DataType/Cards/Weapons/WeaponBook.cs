using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Log;
using NarlonLib.Math;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Tools;

namespace TaleofMonsters.DataType.Cards.Weapons
{
    internal static class WeaponBook
    {
        private static List<int> randomWeaponIdList;

        public static int GetRandWeaponId()
        {
            if (randomWeaponIdList == null)
            {
                randomWeaponIdList = new List<int>();
                foreach (WeaponConfig weaponConfig in ConfigData.WeaponDict.Values)
                {
                    if (weaponConfig.IsSpecial == 0)
                        randomWeaponIdList.Add(weaponConfig.Id);
                }
            }
            return randomWeaponIdList[MathTool.GetRandom(randomWeaponIdList.Count)];  
        }

        public static int[] GetSkillWids(int sid)
        {
            List<int> idList = new List<int>();
            foreach (WeaponConfig weaponConfig in ConfigData.WeaponDict.Values)
            {
                if (weaponConfig.SkillId==sid && weaponConfig.IsSpecial == 0)
                    idList.Add(weaponConfig.Id);
            }
            return idList.ToArray();
        }

        public static string GetAttrByString(int id, string des)
        {
            WeaponConfig weaponConfig = ConfigData.GetWeaponConfig(id);

            switch (des)
            {
                case "attr": return Core.HSTypes.I2Attr(weaponConfig.Attr);
                case "star": return weaponConfig.Star.ToString();
                case "atf": return string.Format("{0}/{1}", weaponConfig.AtkP, weaponConfig.Def);
                case "skill": return weaponConfig.SkillId == 0 ? "无" : ConfigData.GetSkillConfig(weaponConfig.SkillId).Name;
            }
            return "";
        }

        public static Image GetWeaponImage(int id, int width, int height)
        {
            WeaponConfig weaponConfig = ConfigData.GetWeaponConfig(id);
            string fname = string.Format("Weapon/{0}{1}x{2}", weaponConfig.Icon, width, height);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Weapon", string.Format("{0}.JPG", weaponConfig.Icon));
                if (image == null)
                {
                    NLog.Error("GetWeaponImage {0} {1} not found", id, fname);
                    return null;
                }
#if DEBUG
                if (weaponConfig.Remark.Contains("未完成"))
                {
                    Graphics g = Graphics.FromImage(image);
                    var icon = PicLoader.Read("System", "NotFinish.PNG");
                    g.DrawImage(icon, 0, 0, 180, 180);
                    g.Save();
                }
#endif

                if (image.Width != width || image.Height != height)
                    image = image.GetThumbnailImage(width, height, null, new IntPtr(0));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }
    }
}
