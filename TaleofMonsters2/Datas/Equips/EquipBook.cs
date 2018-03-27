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
        private static Dictionary<int, List<int>> equipQualDict;

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
            List<int> datas = new List<int>();
            foreach (var equipConfig in ConfigData.EquipDict.Values)
            {
                if (!equipConfig.CanMerge)
                    continue;
                datas.Add(equipConfig.Id);//返回所有
            }
            return datas.ToArray();
        }

        public static int GetRandEquipByLevelQuality(int qual)
        {
            if (equipQualDict == null)
            {
                equipQualDict = new Dictionary<int, List<int>>();
                foreach (var equipConfig in ConfigData.EquipDict.Values)
                {
                    if (!equipConfig.RandomDrop)
                        continue;
                    if (!equipQualDict.ContainsKey(equipConfig.Quality))
                    {
                        equipQualDict.Add(equipConfig.Quality, new List<int>());
                    }
                    equipQualDict[equipConfig.Quality].Add(equipConfig.Id);
                }
            }

            List<int> datas;
            if (!equipQualDict.TryGetValue(qual, out datas))
            {
                return 0;
            }
            if (datas.Count == 0)
            {
                return 0;
            }
            return datas[MathTool.GetRandom(datas.Count)];
        }
    }
}
