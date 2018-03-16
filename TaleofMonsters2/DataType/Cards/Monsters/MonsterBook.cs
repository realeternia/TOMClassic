using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Log;
using NarlonLib.Math;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Tools;

namespace TaleofMonsters.DataType.Cards.Monsters
{
    internal static class MonsterBook
    {
        private static Dictionary<int, List<int>> starMidDict;
        private static List<int> randomMonsterIdList;

        public static int GetRandMonsterId()
        {
            if (randomMonsterIdList == null)
            {
                randomMonsterIdList = new List<int>();
                foreach (MonsterConfig monsterConfig in ConfigData.MonsterDict.Values)
                {
                    if (monsterConfig.IsSpecial == 0)
                        randomMonsterIdList.Add(monsterConfig.Id);
                }
            }
            return randomMonsterIdList[MathTool.GetRandom(randomMonsterIdList.Count)];
        }

        public static int GetRandStarMid(int star)
        {
            if (starMidDict == null)
            {
                starMidDict = new Dictionary<int, List<int>>();
                foreach (MonsterConfig monsterConfig in ConfigData.MonsterDict.Values)
                {
                    if (monsterConfig.IsSpecial > 0)
                        continue;
                    if (!starMidDict.ContainsKey(monsterConfig.Star))
                        starMidDict.Add(monsterConfig.Star, new List<int>());
                    starMidDict[monsterConfig.Star].Add(monsterConfig.Id);
                }
            }

            return starMidDict[star][MathTool.GetRandom(starMidDict[star].Count)];
        }

        public static int[] GetSkillMids(int sid)
        {
            List<int> mids = new List<int>();
            foreach (MonsterConfig monsterConfig in ConfigData.MonsterDict.Values)
            {
                if (monsterConfig.IsSpecial > 0)
                    continue;
                foreach (var skill in GetSkillList(monsterConfig.Id))
                {
                    if (skill.Id == sid)
                    {
                        mids.Add(monsterConfig.Id);
                        break;
                    }
                }
            }
            return mids.ToArray();
        }

        public static string GetAttrByString(int id, string des)
        {
            MonsterConfig monsterConfig = ConfigData.GetMonsterConfig(id);
            switch (des)
            {
                case "hp": return monsterConfig.VitP.ToString();//todo
                case "race": return Core.HSTypes.I2CardTypeSub(monsterConfig.Type);
                case "type": return Core.HSTypes.I2Attr(monsterConfig.Attr);
                case "star": return monsterConfig.Star.ToString();
                case "atk": return monsterConfig.AtkP.ToString();
           //     case "def": return monsterConfig.DefP.ToString();
            }
            return "";
        }

        public static Image GetMonsterImage(int id, int width, int height)
        {
            MonsterConfig monsterConfig = ConfigData.GetMonsterConfig(id);

            string fname = string.Format("Monsters/{0}{1}x{2}", monsterConfig.Icon, width, height);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Monsters", string.Format("{0}.JPG", monsterConfig.Icon));
                if (image == null)
                {
                    NLog.Error("GetMonsterImage {0} {1} not found",id, fname);
                    return null;
                }
#if DEBUG
                if (monsterConfig.Remark.Contains("未完成"))
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

       public static SkillConfig GetAreaSkill(int mid)
       {
           foreach (var skill in GetSkillList(mid))
           {
               var skilConfig = ConfigData.GetSkillConfig(skill.Id);
               if (skilConfig.Target != "" && skilConfig.OnAdd != null && skilConfig.PointSelf)
                   return skilConfig;
           }
           return null;
       }

        public static bool HasTag(int mid, string tag)
        {
            foreach (var skill in GetSkillList(mid))
            {
                var skilConfig = ConfigData.GetSkillConfig(skill.Id);
                if (skilConfig.Tag == tag)
                    return true;
            }
            return false;
        }

        public static List<RLIdValue> GetSkillList(int mid)
        {
            var monsterConfig = ConfigData.GetMonsterConfig(mid);
            List<RLIdValue> idValues = new List<RLIdValue>();
            if (monsterConfig.Skill1 > 0)
                idValues.Add(new RLIdValue {Id = monsterConfig.Skill1, Value = monsterConfig.SkillRate1});
            if (monsterConfig.Skill2 > 0)
                idValues.Add(new RLIdValue {Id = monsterConfig.Skill2, Value = monsterConfig.SkillRate2});
            return idValues;
        }
    }
}
