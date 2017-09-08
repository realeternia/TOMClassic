namespace TaleofMonsters.DataType.Skills
{
    internal class MonsterSkill
    {
        public int SkillId { get; private set; }
        public int Level { get; private set; }
        public int Percent { get; private set; }

        public MonsterSkill(int sid, int percent, int level)
        {
            Level = level;
            SkillId = sid;
            Percent = percent;
        }
    }
}