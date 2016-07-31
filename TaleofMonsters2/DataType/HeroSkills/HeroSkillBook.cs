using System;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.Resource;

namespace TaleofMonsters.DataType.HeroSkills
{
    /// <summary>
    /// 英雄的主动技能
    /// </summary>
    internal static class HeroSkillBook
    {
        static public Image GetHeroSkillImage(int id)
        {
            HeroSkillConfig skillConfig = ConfigData.GetHeroSkillConfig(id);

            string fname = string.Format("HeroSkill/{0}", skillConfig.Icon);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("HeroSkill", string.Format("{0}.JPG", skillConfig.Icon));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }
        
        static public Image GetSkillPreview(int id)
        {
            HeroSkillConfig heroSkillConfig = ConfigData.GetHeroSkillConfig(id);
            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(heroSkillConfig.Name, "White", 20);
            tipData.AddLine();
            tipData.AddTextNewLine("英雄技能", "Red");
            tipData.AddTextLines(heroSkillConfig.Des, "Lime",15,true);
            tipData.AddLine();
            var cost = CardConfigManager.GetCardConfig(heroSkillConfig.CardId).Cost;
            tipData.AddTextNewLine("消耗：", "White");
            if (heroSkillConfig.Type == (int)CardTypes.Monster)
            {
                tipData.AddText(cost + "AP", "Red");
            }
            else
            {
                tipData.AddText(cost + "MP", "Blue"); 
            }

            return tipData.Image;
        }
    }
}
