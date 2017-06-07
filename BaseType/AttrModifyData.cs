namespace ConfigDatas
{
    public class AttrModifyData
    {
        public double Source { get; set; }

        public double Adder { get; set; }

        public double Multiter { get; set; }

        public bool Locked { get; set; } //锁定后就无法修改了

        public AttrModifyData(float sourceValue)
        {
            Source = sourceValue;
        }

        public AttrModifyData(AttrModifyData sourceValue)
        {
            Source = sourceValue.Source;
            Adder = sourceValue.Adder;
            Multiter = sourceValue.Multiter;
        }

        public static AttrModifyData operator +(AttrModifyData data, double value)
        {
            if (data.Locked)
            {
                return data;
            }
            var rt = new AttrModifyData(data);
            rt.Adder += (float)value;
            return rt;
        }

        public static AttrModifyData operator -(AttrModifyData data, double value)
        {
            if (data.Locked)
            {
                return data;
            }
            var rt = new AttrModifyData(data);
            rt.Adder -= (float)value;
            return rt;
        }

        public static AttrModifyData operator *(AttrModifyData data, double value)
        {
            if (data.Locked)
            {
                return data;
            }
            var rt = new AttrModifyData(data);
            rt.Multiter += (float)value;
            return rt;
        }

        public static AttrModifyData operator /(AttrModifyData data, double value)
        {
            if (data.Locked)
            {
                return data;
            }
            var rt = new AttrModifyData(data);
            rt.Multiter -= (float)value;
            return rt;
        }

        public static bool operator <(AttrModifyData x, AttrModifyData y)
        {
            return (x.Source * (1 + x.Multiter) + x.Adder) < (y.Source * (1 + y.Multiter) + y.Adder);
        }

        public static bool operator >(AttrModifyData x, AttrModifyData y)
        {
            return (x.Source * (1 + x.Multiter) + x.Adder) > (y.Source * (1 + y.Multiter) + y.Adder);
        }

        public static bool operator ==(AttrModifyData x, AttrModifyData y)
        {
            return System.Math.Abs((x.Source * (1 + x.Multiter) + x.Adder) - (y.Source * (1 + y.Multiter) + y.Adder)) < 0.01;
        }

        public static bool operator !=(AttrModifyData x, AttrModifyData y)
        {
            return !(x == y);
        }
        
        public static implicit  operator double(AttrModifyData data)
        {
            return data.Source * (1 + data.Multiter) + data.Adder;
        }
        
        public static implicit  operator int(AttrModifyData data)
        {
            return (int)(data.Source * (1 + data.Multiter) + data.Adder);
        }
        
        public bool Equals(AttrModifyData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other==this;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (AttrModifyData)) return false;
            return Equals((AttrModifyData) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = Source.GetHashCode();
                result = (result*397) ^ Adder.GetHashCode();
                result = (result*397) ^ Multiter.GetHashCode();
                return result;
            }
        }
    }
}