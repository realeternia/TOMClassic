using System;
using System.Collections.Generic;

namespace NarlonLib.Math
{
    public class RandomShuffle
    {
        public static List<int> Process(int[] datas)
        {
            List<int> list = new List<int>(datas);
            for (int i = 0; i < list.Count; ++i)
            {
                int var = MathTool.GetRandom(list.Count);
                int temp = list[i];
                list[i] = list[var];
                list[var] = temp;
            }

            return list;
        }
    }
}
