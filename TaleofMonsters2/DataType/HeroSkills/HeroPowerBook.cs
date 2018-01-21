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
    internal static class HeroPowerBook
    {
        public static Image GetImage(int id)
        {
            var powerConfig = ConfigData.GetHeroPowerConfig(id);

            string fname = string.Format("HeroSkill/{0}", powerConfig.Icon);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("HeroSkill", string.Format("{0}.JPG", powerConfig.Icon));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }
        
        public static Image GetPreview(int id)
        {
            var powerConfig = ConfigData.GetHeroPowerConfig(id);
            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(powerConfig.Name, "White", 20);
            tipData.AddLine();
            tipData.AddTextNewLine("英雄技能", "Red");
            tipData.AddTextLines(powerConfig.Des, "Lime",15,true);
            tipData.AddLine();
            var cost = CardConfigManager.GetCardConfig(powerConfig.CardId).Cost;
            tipData.AddTextNewLine("消耗：", "White");
            if (powerConfig.Type == (int)CardTypes.Monster)
                tipData.AddText(cost + "LP", "Yellow");
            else if (powerConfig.Type == (int)CardTypes.Weapon)
                tipData.AddText(cost + "PP", "Red");
            else
                tipData.AddText(cost + "MP", "Blue"); 

            return tipData.Image;
        }
    }
}
