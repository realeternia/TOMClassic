using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NarlonLib.Math;

namespace NarlonLib.Tools
{
    public class ArraysUtils
    {
        public static void RandomShuffle<T>(IList<T> list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                int var = MathTool.GetRandom(list.Count);
                var temp = list[i];
                list[i] = list[var];
                list[var] = temp;
            }
        }

        public static void RandomShuffle<T>(IList<T> list, Random r)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                int var = r.Next(list.Count);
                var temp = list[i];
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


        public static bool ArrayEqual<T>(IList<T> oldData, IList<T> newData)
        {
            if (oldData.Count != newData.Count)
            {
                return false;
            }

            for (int i = 0; i < oldData.Count; i++)
            {
                if (oldData[i].Equals(newData[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool ArrayCompare(byte[] a, int begin, string b)
        {
            var lenth = b.Length;
            if (a.Length < begin + lenth || lenth <= 0)
            {
                return false;
            }

            if (b.Length != lenth)
            {
                return false;
            }

            for (int i = 0; i < lenth; i++)
            {
                if (a[i + begin] != b[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static T[] GetSubArray<T>(T[] a, int begin, int count)
        {
            T[] newArray = new T[count];
            Array.Copy(a, begin, newArray, 0, count);
            return newArray;
        }
        public static List<T> GetSubArray<T>(List<T> a, int begin, int count)
        {
            if (a.Count <= begin)
                return new List<T>();
            if (a.Count < begin + count)
                return a.GetRange(begin, a.Count-begin);
            return a.GetRange(begin, count);
        }

        public static string GetArrayString<T>(T[] a, int begin, int count)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                T data = a[i + begin];
                sb.Append(data + " ");
            }
            return sb.ToString();
        }

        public static IList<T> ArrayNotIn<T>(IList<T> list1, IList<T> list2)
        {
            List<T> output = new List<T>();
            foreach (var unit in list2)
            {
                if (!list1.Contains(unit))
                    output.Add(unit);
            }
            return output;
        }
    }
}
