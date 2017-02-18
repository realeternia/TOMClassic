using System;

namespace EffectPlayer
{
    public class MathTool
    {
        private static readonly Random r = new Random();

        public static int GetDistance(System.Drawing.Point x, System.Drawing.Point y)
        {
            return GetDistance(x.X, x.Y, y.X, y.Y);
        }

        public static int GetDistance(int x1, int y1, int x2, int y2)
        {
            return (int)System.Math.Sqrt(System.Math.Pow(x1 - x2, 2) + System.Math.Pow(y1 - y2, 2));
        }

        public static int GetRandom(int max)
        {
            return r.Next(max);
        }

        public static int GetRandom(int min, int max)
        {
            if (min == max)
            {
                return min;
            }
            return r.Next(min, max);
        }

		public static int GetSqrtMulti10(int value)
		{
			int[] datas = {0, 10, 14, 17, 20, 22, 24, 26, 28, 30};
			if(value < 0)
			{
				return 0;
			}
			if(value > 9)
			{
				return datas[9];
			}
			return datas[value];
		}

        public static int GetRound(int value, int checker)
        {
            if (value <= checker)
            {
                return value;
            }
           
            int small = value%checker;
            int rt = value - small;
            if (small>checker/2)
            {
                rt += checker;
            }

            return rt;
        }
    }
}
