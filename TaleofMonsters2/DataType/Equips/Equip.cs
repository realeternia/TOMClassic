using ConfigDatas;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.HeroSkills;
using TaleofMonsters.DataType.User;
using System.Drawing;

namespace TaleofMonsters.DataType.Equips
{
    internal class Equip
    {
        public int TemplateId;//装备的id
        private const int AttrFactor = 20;//相当于装备属性是怪属性的20%

        public int Atk;//实际的攻击力
        public int Hp;

        public Equip()
        {
        }

        public Equip(int id)
        {
            TemplateId = id;

            UpgradeToLevel();
        }

        public int GetAttrByIndex(PlayerAttrs attr)
        {
            switch (attr)
            {
                case PlayerAttrs.Atk: return Atk;
                case PlayerAttrs.Hp: return Hp;
            }
            return 0;
        }

        public void UpgradeToLevel()
        {
            EquipConfig equipConfig = ConfigData.GetEquipConfig(TemplateId);
            int qual = equipConfig.Quality; 
            int standardValue = (35 + qual * 5) * (equipConfig.Level + 9) / 10 * AttrFactor / 100;
            if (equipConfig.AtkP > 0)
                Atk = standardValue * (100 + equipConfig.AtkP) / 100; //200
            if (equipConfig.VitP > 0)
                Hp = standardValue * (100 + equipConfig.VitP) / 100 * 3; //200
        }

        public Image GetPreview()
        {
            EquipConfig equipConfig = ConfigData.GetEquipConfig(TemplateId);

            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(equipConfig.Name, HSTypes.I2QualityColor(equipConfig.Quality), 20);
            tipData.AddTextNewLine(string.Format("       装备部位:{0}", HSTypes.I2EPosition(equipConfig.Position)), "White");
            tipData.AddTextNewLine(string.Format("       装备等级:{0}", equipConfig.Level), "White");
            tipData.AddTextNewLine("", "White");
            if (Atk > 0)
            {
                EquipAddonConfig eAddon = ConfigData.GetEquipAddonConfig((int) (PlayerAttrs.Atk+ 1));
                tipData.AddTextNewLine(string.Format(eAddon.Format, Atk), HSTypes.I2EaddonColor(eAddon.Rare));
            }
            if (Hp > 0)
            {
                EquipAddonConfig eAddon = ConfigData.GetEquipAddonConfig((int)(PlayerAttrs.Hp + 1));
                tipData.AddTextNewLine(string.Format(eAddon.Format, Hp), HSTypes.I2EaddonColor(eAddon.Rare));
            }
          
            if (equipConfig.EnergyRate[0] + equipConfig.EnergyRate[1] + equipConfig.EnergyRate[2] > 0)
            {
                tipData.AddLine();
                if (equipConfig.Job > 0)//转换职业
                {
                    var jobConfig = ConfigData.GetJobConfig(equipConfig.Job);
                    tipData.AddTextNewLine(string.Format("转职: {0}", jobConfig.Name), "Pink");
                }
                tipData.AddTextNewLine(string.Format("领导 {0}", equipConfig.EnergyRate[0]), "Gold");
                tipData.AddBar(100, equipConfig.EnergyRate[0], Color.Yellow, Color.Gold);
                tipData.AddTextNewLine(string.Format("力量 {0}", equipConfig.EnergyRate[1]), "Red");
                tipData.AddBar(100, equipConfig.EnergyRate[1], Color.Pink, Color.Red);
                tipData.AddTextNewLine(string.Format("魔力 {0}", equipConfig.EnergyRate[2]), "Blue");
                tipData.AddBar(100, equipConfig.EnergyRate[2], Color.Cyan, Color.Blue);
            }
            
            if (equipConfig.SpecialSkill > 0)
            {
                tipData.AddLine();
                tipData.AddImageNewLine(HeroSkillBook.GetHeroSkillImage(equipConfig.SpecialSkill));
                HeroSkillConfig skillConfig = ConfigData.GetHeroSkillConfig(equipConfig.SpecialSkill);
                string tp = string.Format("{0}:{1}", skillConfig.Name, skillConfig.Des);
                if (tp.Length > 12)
                {
                    tipData.AddText(tp.Substring(0, 11), "White");
                    tipData.AddTextNewLine(tp.Substring(11), "White");
                }
                else
                {
                    tipData.AddText(tp, "White");
                }
            }
            tipData.AddLine();
            tipData.AddTextNewLine(string.Format("需要等级:{0}", equipConfig.LvNeed), UserProfile.InfoBasic.Level < equipConfig.LvNeed ? "Red" : "Gray");
            tipData.AddTextNewLine(string.Format("出售价格:{0}", equipConfig.Value), "Yellow");
            tipData.AddImage(HSIcons.GetIconsByEName("res1"));
            tipData.AddImageXY(EquipBook.GetEquipImage(TemplateId), 8, 8, 48, 48, 7, 24, 32, 32);
            return tipData.Image;
        }
    }
}