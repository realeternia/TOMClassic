using System;
using System.Collections.Generic;
using System.Drawing;
using NarlonLib.Log;
using NarlonLib.Math;
using ConfigDatas;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Tools;

namespace TaleofMonsters.DataType.Cards.Spells
{
    internal static class SpellBook
    {
        private static List<int> randomSpellIdList;

        public static int GetRandSpellId()
        {
            if (randomSpellIdList == null)
            {
                randomSpellIdList = new List<int>();
                foreach (SpellConfig spellConfig in ConfigData.SpellDict.Values)
                {
                    if (spellConfig.IsSpecial > 0)
                        continue;
                    randomSpellIdList.Add(spellConfig.Id);
                }
            }
            return randomSpellIdList[MathTool.GetRandom(randomSpellIdList.Count)];
        }

        public static string GetAttrByString(int id, string info)
        {
            SpellConfig spellConfig = ConfigData.GetSpellConfig(id);
            switch (info)
            {
                case "star": return spellConfig.Star.ToString();
                case "des": return new Spell(id).Descript;
            }
            return "";
        }

        public static bool IsTrap(int id)
        {
            if (ConfigIdManager.GetCardType(id) != CardTypes.Spell)
                return false;
            return ConfigData.GetSpellConfig(id).Remark.Contains("陷阱");
        }

        public static Image GetSpellImage(int id, int width, int height)
        {
            SpellConfig spellConfig = ConfigData.GetSpellConfig(id);
            string fname = string.Format("Spell/{0}{1}x{2}", spellConfig.Icon, width, height);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Spell", string.Format("{0}.JPG", spellConfig.Icon));
                if (image == null)
                {
                    NLog.Error("GetWeaponImage {0} {1} not found", id, fname);
                    return null;
                }
#if DEBUG
                if (spellConfig.Remark.Contains("未完成"))
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
