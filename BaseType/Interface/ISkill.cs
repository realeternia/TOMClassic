namespace ConfigDatas
{
    public interface ISkill : ITargetMeasurable
    {
        int Id { get; }
        int Level { get; }
        IMonster Owner { get; }
    }
}
