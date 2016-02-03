using ConfigDatas;

namespace TaleofMonsters.DataType.Skills
{
    internal class Skill
    {
        public SkillConfig SkillConfig;

        private int lv = 1;

        public int Id
        {
            get { return SkillConfig.Id; }
        }

        public string Name
        {
            get
            {
                return SkillConfig.Name;
            }
        }

        public Skill(int id)
        {
            SkillConfig = ConfigData.SkillDict[id];
        }

        public string Descript
        {
            get
            {
                if (SkillConfig.DescriptBuffId > 0)
                    return SkillConfig.GetDescript(lv) + ConfigDatas.ConfigData.GetBuffConfig(SkillConfig.DescriptBuffId).GetDescript(lv);
                return SkillConfig.GetDescript(lv);
            }
        }

        public Skill UpgradeToLevel(int newLevel)
        {
            lv = newLevel;
            return this;
        }
    }
}
