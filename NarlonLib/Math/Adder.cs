namespace NarlonLib.Math
{
    public class Adder
    {
        private int start;
        private int addon;
        private int value;

        public Adder(int st, int ao)
        {
            start = st;
            addon = ao;
        }

        public int Now
        {
         get { return value; }   
        }

        public int Next
        {
            get
            {
                if (value == 0)
                {
                    value = start;
                }
                else
                {
                    value += addon;
                }
                return value;
            }
        }
    }
}
