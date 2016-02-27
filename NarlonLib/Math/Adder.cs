namespace NarlonLib.Math
{
    public class Adder
    {
        private int start;
        private int addon;
        private int value;
        private bool firstRun = true;

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
                if (firstRun)
                {
                    value = start;
                    firstRun = false;
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
