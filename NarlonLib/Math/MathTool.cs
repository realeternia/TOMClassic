using System;
using System.Collections.Generic;

namespace NarlonLib.Math
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

        public static bool IsPointInRegion(int px, int py, int x1, int y1, int x2, int y2, bool include)
        {
            if (include)
            {
                return px >= x1 && px <= x2 && py >= y1 && py <= y2;
            }
            return px > x1 && px < x2 && py > y1 && py < y2;
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

        public static double GetRandom(double min, double max)
        {
            if (System.Math.Abs(max - min) < 0.001)
            {
                return min;
            }
            return r.NextDouble()*(max - min) + min;
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

        public static double GetFormulaResult(string exp)
        {
            return new Calculator().MathExpressionValue(exp);
        }

        public static double GetFormulaResult(string exp, Dictionary<string, string> datas)
        {
            string realexp = exp;
            foreach (string key in datas.Keys)
            {
                realexp = realexp.Replace(key, datas[key]);
            }
            return GetFormulaResult(realexp);
        }

        public static bool ValueBetween(int value, int min, int max)
        {
            return value >= min && value <= max;
        }
    }

}
