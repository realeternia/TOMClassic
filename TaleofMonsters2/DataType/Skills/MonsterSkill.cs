namespace TaleofMonsters.DataType.Skills
{
    internal class MonsterSkill
    {
        public MonsterSkill(int sid, int percent, int level)
        {
            Level = level;
            SkillId = sid;
            Percent = percent;
        }

        public int Level;
        public int SkillId;
        public int Percent;
    }
}