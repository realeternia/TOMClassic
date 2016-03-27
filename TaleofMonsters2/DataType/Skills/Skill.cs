using ConfigDatas;

namespace TaleofMonsters.DataType.Skills
{
    internal class Skill : ITargetMeasurable
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

        public string Target
        {
            get { return SkillConfig.Target.Substring(1, 1); }
        }

        public string Shape
        {
            get { return SkillConfig.Target.Substring(2, 1); }
        }

        public int Range
        {
            get { return SkillConfig.Range; }
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
