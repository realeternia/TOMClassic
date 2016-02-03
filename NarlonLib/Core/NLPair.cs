namespace NarlonLib.Core
{
    public class NLPair<T, T2>
    {
        public T Value1;
        public T2 Value2;

        public NLPair()
        {
            Value1 = default(T);
            Value2 = default(T2);
        }

        public NLPair(T d1,T2 d2)
        {
            Value1 = d1;
            Value2 = d2;
        }
    }
}