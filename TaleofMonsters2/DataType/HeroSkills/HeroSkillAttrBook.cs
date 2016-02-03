using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.Resource;

namespace TaleofMonsters.DataType.HeroSkills
{
    internal static class HeroSkillAttrBook
    {
        static public int GetCost(int id, int level)
        {
            HeroSkillAttrConfig heroSkillAttrConfig = ConfigData.GetHeroSkillAttrConfig(id);
            return level * level * (10 + heroSkillAttrConfig.HeroLevel / 5) / 10 + heroSkillAttrConfig.HeroLevel * level + 50 + heroSkillAttrConfig.HeroLevel * 10;
        }

        static private string GetDes(int id, int level)
        {
            HeroSkillAttrConfig heroSkillAttrConfig = ConfigData.GetHeroSkillAttrConfig(id);
            string tp = heroSkillAttrConfig.Des;
            tp = tp.Replace("{atk}", (heroSkillAttrConfig.Atk * level).ToString());
            tp = tp.Replace("{def}", (heroSkillAttrConfig.Def * level).ToString());
            tp = tp.Replace("{magic}", (heroSkillAttrConfig.Magic * level).ToString());
            tp = tp.Replace("{hit}", (heroSkillAttrConfig.Hit * level).ToString());
            tp = tp.Replace("{dhit}", (heroSkillAttrConfig.Dhit * level).ToString());
            tp = tp.Replace("{spd}", (heroSkillAttrConfig.Spd * level).ToString());
            tp = tp.Replace("{hp}", (heroSkillAttrConfig.Hp * level).ToString());
            return tp;
        }

        static public Image GetPreview(int id, int level)
        {
            HeroSkillAttrConfig heroSkillAttrConfig = ConfigData.GetHeroSkillAttrConfig(id);
            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(heroSkillAttrConfig.Name, "White", 20);
            tipData.AddTextNewLine(string.Format("当前等级: {0}级", level), "White");
            tipData.AddTextNewLine(GetDes(id,level), "Lime");
            tipData.AddLine(20);
            tipData.AddTextNewLine(string.Format("下一等级: {0}级", level + 1), "White");
            tipData.AddTextNewLine(GetDes(id, level + 1), "Lime");
            tipData.AddTextNewLine(string.Format("需要等级: {0}", level + 1), (User.UserProfile.InfoBasic.Level >= (level + 1)) ? "Gray" : "Red");
            tipData.AddTextNewLine(string.Format("需要阅历: {0}", GetCost(id, level + 1)), (User.UserProfile.InfoBasic.AttrPoint >= GetCost(id, level + 1)) ? "Gray" : "Red");
            return tipData.Image;
        }

        public static int[] GetAvailSkills(int level)
        {
            List<int> skills = new List<int>();
            foreach (HeroSkillAttrConfig heroSkillAttrConfig in ConfigData.HeroSkillAttrDict.Values)
            {
                if (heroSkillAttrConfig.HeroLevel <= level)
                {
                    skills.Add(heroSkillAttrConfig.Id);
                }
            }
            return skills.ToArray();
        }

        static public Image GetHeroSkillAttrImage(int id)
        {
            string fname = string.Format("HeroSkill/Attr/{0}.JPG", ConfigData.HeroSkillAttrDict[id].Icon);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("HeroSkill.Attr", string.Format("{0}.JPG", ConfigData.HeroSkillAttrDict[id].Icon));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }

    }
}
