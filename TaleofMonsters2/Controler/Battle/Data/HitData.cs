using ConfigDatas;

namespace TaleofMonsters.Controler.Battle.Data
{
    internal class HitData : IHitDamage
    {
        public int Value { get; private set; }//伤害值
        public int SourceValue { get; private set; }//无视防御的伤害值
        public bool UseSource { get; set; }//是否无视防御


        public HitData(int val)
        {
            Value = val;
            SourceValue = val;
        }

        public bool AddPDamage(double damage)
        {
            Value = (int) (Value + damage);
            return true;
        }

        public bool AddMDamage(double damage)
        {
            Value = (int)(Value + damage);
            return true;
        }

        public bool SetPDamageRate(double rate)
        {
            Value = (int)(rate);
            return true;
        }

        public bool SetMDamageRate(double rate)
        {
            Value = (int)(rate);
            return true;
        }
    }
}