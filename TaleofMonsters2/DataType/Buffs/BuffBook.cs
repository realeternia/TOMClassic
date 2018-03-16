using System;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Tools;

namespace TaleofMonsters.DataType.Buffs
{
    internal static class BuffBook
    {
        public static bool HasEffect(int id, BuffEffectTypes etype)
        {
            BuffConfig buffConfig = ConfigData.BuffDict[id];
            foreach (int eff in buffConfig.Effect)
            {
                if ((int)etype == eff)
                    return true;
            }
            return false;
        }

        public static Color GetBuffColor(int id)
        {
            BuffConfig buffConfig = ConfigData.BuffDict[id];
            var color = buffConfig.Color;
            var colorStr = color.Split(',');
            return Color.FromArgb(100, int.Parse(colorStr[0]), int.Parse(colorStr[1]), int.Parse(colorStr[2]));
        }

        public static Image GetBuffImage(int id,int index)
        {
            BuffConfig buffConfig = ConfigData.BuffDict[id];
            string indexTxt = index == 0 ? "" : index.ToString();
            string fname = string.Format("Buff/{0}{1}", buffConfig.Icon, indexTxt);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Buff", string.Format("{0}{1}.PNG", buffConfig.Icon, indexTxt));
                ImageManager.AddImage(fname, image.GetThumbnailImage(20, 20, null, new IntPtr(0)));
            }
            return ImageManager.GetImage(fname);
        }
    }
}
