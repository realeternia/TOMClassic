namespace ConfigDatas
{
    public class HitDamage
    {
        public int Value { get; private set; }

        public int Element { get; private set; }

        public DamageTypes Dtype { get; private set; }

        public HitDamage(int damage, int element, DamageTypes type)
        {
            Value = damage;
            Element = element;
            Dtype = type;
        }
        
        public bool AddPDamage(double damage)
        {
            return	AddDamage(DamageTypes.Physical,(int)damage);
        }
                
        public bool AddMDamage(double damage)
        {
            return	AddDamage(DamageTypes.Magic,(int)damage);
        }

        public bool AddDamage(DamageTypes type, int damage)
        {
            if (Dtype == type || (type == DamageTypes.Physical && Dtype != DamageTypes.Magic) || type == DamageTypes.All)
            {
                Value += damage;
                return true;
            }
            return false;
        }
                
        public bool SetPDamageRate(double rate)
        {
            return	SetDamage(DamageTypes.Physical,(int)(Value*rate));
        }    
                    
        public bool SetMDamageRate(double rate)
        {
            return	SetDamage(DamageTypes.Magic,(int)(Value*rate));
        }

        public bool SetDamage(DamageTypes type, int damage)
        {
            if (Dtype == type || (type == DamageTypes.Physical && Dtype != DamageTypes.Magic) || type == DamageTypes.All)
            {
                Value = damage;
                return true;
            }
            return false;
        }
    }
}