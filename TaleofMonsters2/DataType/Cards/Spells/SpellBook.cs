using System;
using System.Collections.Generic;
using System.Drawing;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.Resource;
using ConfigDatas;

namespace TaleofMonsters.DataType.Cards.Spells
{
    static class SpellBook
    {
        private static List<int> randomSpellIdList;

        static public int GetRandSpellId()
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

        static public string GetAttrByString(int id, string info)
        {
            SpellConfig spellConfig = ConfigData.GetSpellConfig(id);
            switch (info)
            {
                case "star": return spellConfig.Star.ToString();
                case "des": return new Spell(id).Descript;
            }
            return "";
        }

        static public Image GetSpellImage(int id, int width, int height)
        {
            string fname = string.Format("Spell/{0}{1}x{2}", ConfigData.SpellDict[id].Icon, width, height);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Spell", string.Format("{0}.JPG", ConfigData.SpellDict[id].Icon));
                if (image.Width != width || image.Height != height)
                {
                    image = image.GetThumbnailImage(width, height, null, new IntPtr(0));
                }
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }
    }
}
