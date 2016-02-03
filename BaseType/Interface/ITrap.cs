namespace ConfigDatas
{
    public interface ITrap
    {
        int Level { get; }//触发技能的等级
        int Damage { get; }//陷阱伤害
    }
}