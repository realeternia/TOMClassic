using System;
using System.Collections.Generic;

namespace NarlonLib.Math
{
    public class ListTool
    {
        public static void RandomShuffle(List<int> list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                int var = MathTool.GetRandom(list.Count);
                int temp = list[i];
                list[i] = list[var];
                list[var] = temp;
            }
        }
        public static void RandomShuffle(List<int> list, Random r)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                int var = r.Next(list.Count);
                int temp = list[i];
                list[i] = list[var];
                list[var] = temp;
            }
        }

        public static void Fill(List<int> list, int toFill, int total)
        {
            while (list.Count < total)
            {
                list.Add(toFill);
            }
        }
    }
}
