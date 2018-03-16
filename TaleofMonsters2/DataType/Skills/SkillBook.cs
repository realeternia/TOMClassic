using System;
using System.Collections.Generic;
using System.Drawing;
using NarlonLib.Log;
using NarlonLib.Math;
using ConfigDatas;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Tools;

namespace TaleofMonsters.DataType.Skills
{
    internal static class SkillBook
    {
        private static List<int> randomSkillIds;

        public static int GetRandSkillId()
        {
            if (randomSkillIds == null)
            {
                randomSkillIds = new List<int>();
                foreach (SkillConfig skillConfig in ConfigData.SkillDict.Values)
                {
                    if (skillConfig.IsRandom)
                        randomSkillIds.Add(skillConfig.Id);
                }
            }
            return randomSkillIds[MathTool.GetRandom(randomSkillIds.Count)];            
        }

        public static string GetAttrByString(int id, string info)
        {
            SkillConfig skillConfig = ConfigData.GetSkillConfig(id);

            switch (info)
            {
                case "type": return skillConfig.Type;
                case "des": return new Skill(id).Descript;
            }
            return "";
        }

        public static Image GetSkillImage(int id)
        {
            return GetSkillImage(id,64,64);
        }

        public static Image GetSkillImage(int id, int width, int height)
        {
            SkillConfig skillConfig = ConfigData.GetSkillConfig(id);

            string fname = string.Format("Skill/{0}{1}x{2}", id, width, height);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Skill", string.Format("{0}.JPG", skillConfig.Icon));
                if (image == null)
                {
                    NLog.Error("GetSkillImage {0} {1} not found", id, fname);
                    return null;
                }

#if DEBUG
                if (skillConfig.Remark.Contains("未完成"))
                {
                    Graphics g = Graphics.FromImage(image);
                    var icon = PicLoader.Read("System", "NotFinish2.PNG");
                    g.DrawImage(icon, 0, 0, 64, 64);
                    g.Save();
                    g.Dispose();
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
