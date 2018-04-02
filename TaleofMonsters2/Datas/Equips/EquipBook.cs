using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Tools;

namespace TaleofMonsters.Datas.Equips
{
    internal static class EquipBook
    {
        public static Image GetEquipImage(int id)
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
            List<int> equipList = new List<int>();
            foreach (var equipConfig in ConfigData.EquipDict.Values)
            {
                if (!equipConfig.CanMerge)
                    continue;
                equipList.Add(equipConfig.Id);//返回所有
            }
            return equipList.ToArray();
        }

        public static int GetCanDropId()
        {
            var equipList = new List<int>();
            foreach (var equipConfig in ConfigData.EquipDict.Values)
            {
                if (!equipConfig.RandomDrop)
                    continue;
                equipList.Add(equipConfig.Id);
            }

            return equipList[MathTool.GetRandom(equipList.Count)];
        }
    }
}
