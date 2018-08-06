namespace ConfigDatas
{
    public interface IHitDamage
    {
        int Value { get;}//伤害值
        bool UseSource { get; set; }//是否无视防御
        int SourceValue { get; }

        bool SetPDamageRate(double rate);

        bool SetMDamageRate(double rate);

        bool AddPDamage(double damage);

        bool AddMDamage(double damage);
    }
}